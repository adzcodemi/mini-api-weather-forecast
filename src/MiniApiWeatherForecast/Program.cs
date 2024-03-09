using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using MiniApiWeatherForecast.Models;
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

// Configure exception handling middleware
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        var exception = exceptionHandlerPathFeature?.Error;
        // Get ILogger instance
        var _logger = context.RequestServices.GetRequiredService<ILogger<Program>>();


        // Log the exception
        _logger.LogCritical("An exception occurred: {exception}", exception);

        // Return a 500 status code with the ErrorResponse model
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsJsonAsync(new ErrorResponse
        {
            Message = "Internal server error occurred."
        });
    });
});

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

app.MapGet("/stations/{id}", async Task<IResult> (string id, IHttpClientFactory clientFactory, ILoggerFactory loggerFactory) =>
{
    var logger = loggerFactory.CreateLogger("stations");

    var client = clientFactory.CreateClient("WeatherForecastApi");
    var res = await client.GetAsync($"/flood-monitoring/id/stations/{id}");

    if (res.IsSuccessStatusCode)
    {
        var content = await res.Content.ReadAsStringAsync();
        return TypedResults.Ok(JsonConvert.DeserializeObject<StationResponse>(content));
    }
    else if (res.StatusCode == HttpStatusCode.NotFound)
    {
        var errorResponse = new ErrorResponse
        {
            Message = "NotFound Request",
            Detail =
        [
            new ErrorDetail
            {
                PropertyName = "Request",
                Message = "Not found for the specified stationId"
            }
        ]
        };
        return TypedResults.NotFound(JsonConvert.SerializeObject(errorResponse));
    }
    else if (res.StatusCode == HttpStatusCode.BadRequest)
    {
        var errorResponse = new ErrorResponse
        {
            Message = "Bad Request",
            Detail =
            [
                new ErrorDetail
                {
                    PropertyName = "Request",
                    Message = "The request is invalid."
                }
            ]
        };
        return TypedResults.BadRequest(JsonConvert.SerializeObject(errorResponse));
    }
    else
    {
        logger.LogError("Failed to retrieve data from the API. Status code: {err}", res.StatusCode);
        return TypedResults.Problem("An error occurred while processing the request.", "Station", (int)HttpStatusCode.InternalServerError, "Internal Server Error");
    }

})
.Produces<StationResponse>(StatusCodes.Status200OK, "application/json")
.Produces<ErrorResponse>(StatusCodes.Status404NotFound)
.Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
.Produces<ErrorResponse>(StatusCodes.Status500InternalServerError)
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
