using System;
using System.Text.Json.Serialization;

namespace RestApi.Models.V1
{
    public class WeatherForecast
    {
        [JsonPropertyName("date")]
        public string Date { get; set; }
        [JsonPropertyName("summary")]
        public string Summary { get; set; }
        [JsonPropertyName("temperatureC")]
        public int TemperatureC { get; set; }
        [JsonPropertyName("temperatureF")]
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }
}
