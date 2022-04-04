using RedditPlaceTemplateGen.PlaceNl;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddResponseCaching();
builder.Logging.SetMinimumLevel(LogLevel.Warning);
var app = builder.Build();

Console.WriteLine("Preloading template");
_ = await TemplateGenPlaceNl.GetTemplateImageCachedAsync();

app.UseResponseCaching();
app.MapControllers();
app.Run();