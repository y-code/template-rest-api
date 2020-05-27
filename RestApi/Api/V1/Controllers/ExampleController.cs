using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using RestApi.Api.V1.Models;

namespace RestApi.Api.V1.Controllers
{
    // Version is determined by Namespace Convention, which is set up in Startup.cs
    // See more details about the convention in https://github.com/microsoft/aspnet-api-versioning/wiki/API-Version-Conventions

    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [OpenApiTag("API Example: Weather Forecast")]
    public partial class ExampleController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching",
        };

        private readonly ILogger _logger;

        public ExampleController(ILogger<ExampleController> logger)
        {
            _logger = logger;
        }

        [HttpGet("{date}")]
        public virtual IEnumerable<WeatherForecast> Get([FromRoute] string date)
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
                .Select(index => new WeatherForecast
                {
                    Date = baseDate.AddDays(index).ToString("d MMM, yyyy"),
                    TemperatureC = random.Next(-20, 55),
                    Summary = Summaries[random.Next(Summaries.Length)],
                })
                .ToArray();
        }

        [HttpGet()]
        public virtual IEnumerable<WeatherForecast> Get()
            => Get(null);
    }
}
