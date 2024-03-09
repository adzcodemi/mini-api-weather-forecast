using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Serilog;
using System.Net;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration));

builder.Services.AddSwaggerGen();
// Configure HttpClient
builder.Services.AddHttpClient("WeatherForecastApi", client =>
{
    string? baseUrl = builder.Configuration["BaseUrl"];
    if (string.IsNullOrEmpty(baseUrl))
        throw new ApplicationException("BaseUrl is null");

    client.BaseAddress = new Uri(baseUrl);
});

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "1.0",
        Title = "Rainfall Api",
        Description = "An API which provides rainfall reading data",
        Contact = new OpenApiContact
        {
            Name = "Sorted",
            Url = new Uri("https://www.sorted.com")
        }
    });
});
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.UseHttpsRedirection();



app.MapGet("/", (ILoggerFactory loggerFactory) =>
{
    var logger = loggerFactory.CreateLogger("Index");
    logger.LogInformation("Navigating to Index endpoint.");

    return "Welcome to the UK weather forecast.";
})
.Produces<dynamic>(StatusCodes.Status200OK)
.WithName("Index")
.WithTags("Rainfall");

app.MapGet("/id/stations/{id}", async (string id, IHttpClientFactory clientFactory, ILoggerFactory loggerFactory) =>
{
    var logger = loggerFactory.CreateLogger("stations");
    try
    {
        var client = clientFactory.CreateClient("WeatherForecastApi");
        var res = await client.GetAsync($"/flood-monitoring/id/stations/{id}");

        if (res.IsSuccessStatusCode)
        {
            var content = await res.Content.ReadAsStringAsync();
            return Results.Ok(JsonConvert.SerializeObject(content));
        }
        else if (res.StatusCode == HttpStatusCode.NotFound)
        {

            return Results.NotFound();
        }
        else if (res.StatusCode == HttpStatusCode.BadRequest)
        {
            return Results.BadRequest();
        }
        else
        {
            logger.LogError("Failed to retrieve data from the API. Status code: {err}", res.StatusCode);

            return Results.Problem("An error occurred while processing the request.", "Station", (int)HttpStatusCode.InternalServerError, "Internal Server Error");
        }
    }
    catch(Exception ex)
    {
        logger.LogError("Exception occured station: {err}", ex.Message);
        return Results.Problem("An error occurred while processing the request.", "Station", (int)HttpStatusCode.InternalServerError, "Internal Server Error");
    }
})
.Produces<dynamic>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound)
.Produces(StatusCodes.Status400BadRequest)
.Produces(StatusCodes.Status500InternalServerError)
.WithName("GetStation")
.WithOpenApi(operation => new(operation)
{
    Summary = "Get rainfall station by station Id",
    Description = "Retrieve the station details for the specified stationId"
})
  .WithTags("Rainfall");


// How to use logging in Program.cs file		
app.Logger.LogInformation("The application started");


app.Run("https://localhost:3000");
