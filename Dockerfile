FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/RedditPlaceTemplateGen.PlaceNl.Api/RedditPlaceTemplateGen.PlaceNl.Api.csproj", "RedditPlaceTemplateGen.PlaceNl.Api/"]
RUN dotnet restore "RedditPlaceTemplateGen.PlaceNl.Api/RedditPlaceTemplateGen.PlaceNl.Api.csproj"
COPY "src/" .
WORKDIR "/src/RedditPlaceTemplateGen.PlaceNl.Api"
RUN dotnet publish "RedditPlaceTemplateGen.PlaceNl.Api.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "RedditPlaceTemplateGen.PlaceNl.Api.dll"]
