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
    [ApiVersion("1.0")]
    [ApiVersion("1.1")]
    [Route("api/v{version:apiVersion}/WeatherForecast")]
    [OpenApiTag("API Example: Weather Forecast")]
    public partial class WeatherForecastV1Controller : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching",
        };

        private readonly ILogger _logger;

        public WeatherForecastV1Controller(ILogger<WeatherForecastV1Controller> logger)
        {
            _logger = logger;
        }

        private bool TryParseDateParam(string param, out DateTime dateTime)
        {
            dateTime = DateTime.UtcNow;
            return param == null || DateTime.TryParseExact(param, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime);
        }

        [HttpGet("{date}")]
        [MapToApiVersion("1.0")]
        public IEnumerable<WeatherForecastV1> GetV1([FromRoute] string date)
        {
            if (!TryParseDateParam(date, out var baseDate))
            {
                throw new ArgumentException($"'{date}' in URL should be in yyyyMMdd format.", "date");
            }

            var random = new Random();
            return Enumerable.Range(0, 5)
                .Select(index => new WeatherForecastV1
                {
                    Date = baseDate.AddDays(index).ToString("d MMM, yyyy"),
                    Summary = Summaries[random.Next(Summaries.Length)],
                });
        }

        // This can be also done by endpoint above with optional route parameter in ASP.NET Core Web API.
        // However, OpenAPI Specification does not support it, and neither do NSwag.
        [HttpGet]
        [MapToApiVersion("1.0")]
        public IEnumerable<WeatherForecastV1> GetV1()
            => GetV1(null);

        [HttpGet("{date}")]
        [MapToApiVersion("1.1")]
        public IEnumerable<WeatherForecastV1_1> GetV1_1([FromRoute] string date)
        {
            if (!TryParseDateParam(date, out var baseDate))
            {
                throw new ArgumentException($"'{date}' in URL should be in yyyyMMdd format.", "date");
            }

            var random = new Random();
            return Enumerable.Range(0, 5)
                .Select(index => new WeatherForecastV1_1
                {
                    Date = baseDate.AddDays(index).ToString("d MMM, yyyy"),
                    Summary = Summaries[random.Next(Summaries.Length)],
                    TemperatureC = random.Next(-20, 55),
                });
        }

        // This can be also done by endpoint above with optional route parameter in ASP.NET Core Web API.
        // However, OpenAPI Specification does not support it, and neither do NSwag.
        [HttpGet]
        [MapToApiVersion("1.1")]
        public IEnumerable<WeatherForecastV1_1> GetV1_1()
            => GetV1_1(null);
    }
}
