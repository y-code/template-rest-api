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
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/Example")]
    [OpenApiTag("API Example: Weather Forecast")]
    public partial class ExampleV1Controller : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching",
        };

        private readonly ILogger _logger;

        public ExampleV1Controller(ILogger<ExampleV1Controller> logger)
        {
            _logger = logger;
        }

        [HttpGet("{date}")]
        public IEnumerable<WeatherForecastV1> Get([FromRoute] string date)
        {
            DateTime baseDate = DateTime.UtcNow;
            if (date != null
                && !DateTime.TryParseExact(
                    date,
                    "yyyyMMdd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out baseDate))
            {
                throw new ArgumentException($"'{date}' in URL should be in yyyyMMdd format.", "date");
            }

            var random = new Random();
            return Enumerable.Range(0, 5)
                .Select(index => new WeatherForecastV1
                {
                    Date = baseDate.AddDays(index).ToString("d MMM, yyyy"),
                    TemperatureC = random.Next(-20, 55),
                    Summary = Summaries[random.Next(Summaries.Length)],
                });
        }

        [HttpGet]
        public IEnumerable<WeatherForecastV1> Get()
            => Get(null);
    }
}
