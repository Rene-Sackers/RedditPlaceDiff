using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace RedditPlaceTemplateGen.PlaceNl.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ApiController : ControllerBase
{
	[Route("/template.png")]
	[ResponseCache(Location = ResponseCacheLocation.Any, Duration = 60)]
	public async Task<IActionResult> GetTemplateImage()
	{
		try
		{
			var template = await TemplateGenPlaceNl.GetTemplateImageCachedAsync();
		
			return File(template.Data, "image/png", template.LastModified, new($"\"{template.ETag}\""));
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			return StatusCode((int)HttpStatusCode.InternalServerError);
		}
	}
}