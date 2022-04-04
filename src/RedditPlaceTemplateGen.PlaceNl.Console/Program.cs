using RedditPlaceTemplateGen.PlaceNl;

var mapUrl = await TemplateGenPlaceNl.GetCurrentMapUrlAsync();
var outputImage = await TemplateGenPlaceNl.GetTemplateImageAsync(mapUrl);

var targetPath = Path.Combine(AppContext.BaseDirectory, "generated.png");
await using var writer = File.OpenWrite(targetPath);
await writer.WriteAsync(outputImage.ToArray());

Console.WriteLine($"Image saved to: {targetPath}");