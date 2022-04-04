using Newtonsoft.Json;
using RedditPlaceDiff;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;

var dataDir = AppContext.BaseDirectory;

const int sectionSize = 1000;
const int scaleFactor = 3;

Console.WriteLine("Downloading command image");

using var httpClient = new HttpClient();
var json = await httpClient.GetStringAsync("https://placenl.noahvdaa.me/api/stats");
var apiData = JsonConvert.DeserializeObject<ApiModel>(json);
var mapUrl = "https://placenl.noahvdaa.me/maps/" + apiData.CurrentMap;
var mapRequest = await httpClient.GetAsync(mapUrl);
var imageStream = await mapRequest.Content.ReadAsStreamAsync();
using var command = Image.Load<Rgba32>(imageStream, new PngDecoder());

Console.WriteLine("Generating image");

using var image = new Image<Rgba32>(sectionSize * 2 * scaleFactor, sectionSize * 2 * scaleFactor);

image.ProcessPixelRows(accessor =>
{
	for (var y = 1; y < image.Height; y += scaleFactor)
	{
		var pixelRow = accessor.GetRowSpan(y);
		
		for (var x = 1; x < image.Width; x += scaleFactor)
		{
			var commandPixel = command[x / scaleFactor, y / scaleFactor];
			if (commandPixel.A == 0)
				continue;
			
			ref var pixel = ref pixelRow[x];

			pixel = commandPixel;
		}
	}
});

var targetDir = Path.Combine(dataDir, "generated.png");
image.Save(targetDir);
Console.WriteLine($"Image saved to: {targetDir}");