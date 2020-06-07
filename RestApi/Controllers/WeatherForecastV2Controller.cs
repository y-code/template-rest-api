using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using RestApi.Models;

namespace RestApi.Controllers
{
    [ApiController]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/WeatherForecast")]
    [OpenApiTag("API Example: Weather Forecast")]
    public partial class WeatherForecastV2Controller : ControllerBase
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

        public WeatherForecastV2Controller(ILogger<WeatherForecastV2Controller> logger)
        {
            _logger = logger;
        }

        [HttpGet("Area/{area}")]
        public IEnumerable<WeatherForecastV2> GetWeatherForecast(
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
                .Select(index => areas.Select(a => new WeatherForecastV2
                {
                    Area = a,
                    Date = baseDate.AddDays(index),
                    TemperatureC = random.Next(-20, 55),
                    Summary = Summaries[random.Next(Summaries.Length)],
                }))
                .SelectMany(w => w);
        }

        // This can be also done by endpoint above with optional route parameter in ASP.NET Core Web API.
        // However, OpenAPI Specification does not support it, and neither do NSwag.
        [HttpGet("Area")]
        [HttpGet]
        public IEnumerable<WeatherForecastV2> GetWeatherForecast(
            [FromQuery] string from,
            [FromQuery] int? days)
            => GetWeatherForecast(null, from, days);
    }
}
