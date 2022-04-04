using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;

namespace RedditPlaceTemplateGen;

public static class TemplateGen
{
	public static async Task<MemoryStream> GenerateTemplateAsync(Stream commandImage)
	{
		const int sectionSize = 1000;
		const int scaleFactor = 3;

		using var command = Image.Load<Rgba32>(commandImage, new PngDecoder());

		if (command.Width != sectionSize * 2 || command.Height != sectionSize * 2)
		{
			throw new($"Unexpected size of target image: {sectionSize * 2}x{sectionSize * 2}, got: {command.Width}x{command.Height}");
		}

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

		var outputStream = new MemoryStream();
		await image.SaveAsync(outputStream, new PngEncoder());
		outputStream.Seek(0, SeekOrigin.Begin);
		return outputStream;
	}
}