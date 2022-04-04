using System.Collections.Concurrent;
using Newtonsoft.Json;

namespace RedditPlaceTemplateGen.PlaceNl;

public class TemplateCacheInfo
{
	public byte[] Data { get; init; }
	
	public DateTimeOffset LastModified { get; init; }
	
	public string MapUrl { get; init; }
	
	public string ETag { get; init; }
}

public class CachedData
{
	public DateTime Expires { get; init; }
	
	public object Data { get; init; }
}

public static class CacheService
{
	private static readonly ConcurrentDictionary<string, CachedData> CachedValues = new();

	public static (bool gotFromCache, T data) GetFromCache<T>(string key)
	{
		if (!CachedValues.TryGetValue(key, out var cached))
			return (false, default);

		if (cached.Expires <= DateTime.Now)
			return (false, default);

		return (true, (T)cached.Data);
	}

	public static void AddToCache<T>(string key, T data, TimeSpan expiresIn)
	{
		CachedValues[key] = new()
		{
			Data = data,
			Expires = DateTime.Now.Add(expiresIn)
		};
	}
}

public static class TemplateGenPlaceNl
{
	private static readonly HttpClient HttpClient = new();
	private static readonly TimeSpan ApiMapurlCacheTime = TimeSpan.FromMinutes(1);
	private static readonly SemaphoreSlim GenerateSemaphore = new(1);
	
	private static TaskCompletionSource<TemplateCacheInfo> _generateTemplateTask;
	private static TemplateCacheInfo _templateCacheInfo;
	
	public static async Task<string> GetCurrentMapUrlAsync()
	{
		const string cacheKey = "api-map-url";
		var (gotFromCache, url) = CacheService.GetFromCache<string>(cacheKey);
		if (gotFromCache)
			return url;
		
		var json = await HttpClient.GetStringAsync("https://placenl.noahvdaa.me/api/stats");
		var apiData = JsonConvert.DeserializeObject<ApiModel>(json);
		url = "https://placenl.noahvdaa.me/maps/" + apiData.CurrentMap;
		
		CacheService.AddToCache(cacheKey, url, ApiMapurlCacheTime);

		return url;
	}
	
	public static async Task<MemoryStream> GetTemplateImageAsync(string mapUrl)
	{
		Console.WriteLine($"Downloading map: {mapUrl}");
		var mapRequest = await HttpClient.GetAsync(mapUrl);
		await using var imageStream = await mapRequest.Content.ReadAsStreamAsync();

		Console.WriteLine("Generating template");
		return await TemplateGen.GenerateTemplateAsync(imageStream);
	}
	
	public static async Task<TemplateCacheInfo> GetTemplateImageCachedAsync()
	{
		var mapUrl = await GetCurrentMapUrlAsync();
		if (mapUrl == _templateCacheInfo?.MapUrl)
			return _templateCacheInfo;

		await GenerateSemaphore.WaitAsync();

		if (_generateTemplateTask != null)
		{
			GenerateSemaphore.Release();
			return await _generateTemplateTask.Task;
		}

		_generateTemplateTask = new();

		try
		{
			await using var templateStream = await GetTemplateImageAsync(mapUrl);
			_templateCacheInfo = new()
			{
				Data = templateStream.ToArray(),
				ETag = Guid.NewGuid().ToString("N"),
				MapUrl = mapUrl,
				LastModified = DateTimeOffset.Now
			};

			_generateTemplateTask.TrySetResult(_templateCacheInfo);
			_generateTemplateTask = null;
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
		}
		
		GenerateSemaphore.Release();

		return _templateCacheInfo;
	}
}