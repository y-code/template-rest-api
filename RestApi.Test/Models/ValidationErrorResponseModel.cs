using System;
using System.Text.Json.Serialization;

namespace RestApi.Test.Models
{
    public class ValidationErrorResponseModel
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }
    }
}
