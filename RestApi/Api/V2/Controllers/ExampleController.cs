using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using RestApi.Api.V2.Models;

namespace RestApi.Api.V2.Controllers
{
    // Version is determined by Namespace Convention, which is set up in Startup.cs
    // See more details about the convention in https://github.com/microsoft/aspnet-api-versioning/wiki/API-Version-Conventions

    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [OpenApiTag("API Example: Weather Forecast")]
    public partial class ExampleController : ControllerBase
    {
        private static readonly string[] Areas = new[]
        {
            "New York", "Tokyo", "Christchurch",
        };

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching",
        };

        private readonly ILogger _logger;

        public ExampleController(ILogger<ExampleController> logger)
        {
            _logger = logger;
        }

        [HttpGet("WeatherForecast/{area}")]
        public virtual IEnumerable<WeatherForecast> GetWeatherForecast(
            [FromRoute] string area,
            [FromQuery] string from,
            [FromQuery] int? days = null)
        {
            if (area != null && !Areas.Contains(area))
            {
                throw new ArgumentException($"'{area}' is not supported area.", "area");
            }

            DateTime baseDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day);
            if (from != null
                && !DateTime.TryParseExact(
                    from,
                    "yyyyMMdd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out baseDate))
            {
                throw new ArgumentException($"'{from}' should be in yyyyMMdd format.", "from");
            }

            if (days == null)
            {
                days = 5;
            }

            var areas = Areas.Where(a => area == null || a == area);

            var random = new Random();
            return Enumerable.Range(0, days.Value)
                .Select(index => areas.Select(a => new WeatherForecast
                {
                    Area = a,
                    Date = baseDate.AddDays(index),
                    TemperatureC = random.Next(-20, 55),
                    Summary = Summaries[random.Next(Summaries.Length)],
                }))
                .SelectMany(w => w)
                .ToArray();
        }

        [HttpGet("WeatherForecast")]
        public virtual IEnumerable<WeatherForecast> GetWeatherForecast(
            [FromQuery] string from,
            [FromQuery] int? days)
            => GetWeatherForecast(null, from, days);
    }
}
