using System;
using System.Text.Json.Serialization;
using NJsonSchema.Annotations;

namespace RestApi.Models
{
    [JsonSchema(name: "WeatherForecast")]
    public class WeatherForecastV2
    {
        [JsonPropertyName("date")]
        public DateTime Date { get; set; }
        [JsonPropertyName("area")]
        public string Area { get; set; }
        [JsonPropertyName("summary")]
        public string Summary { get; set; }
        [JsonPropertyName("temperatureC")]
        public int TemperatureC { get; set; }
        [JsonPropertyName("temperatureF")]
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }
}
