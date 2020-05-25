using System;
using System.Text.Json.Serialization;

namespace RestApi.Test.Models
{
    public class BadRequestResponseModel
    {
        public class ErrorModel
        {
            [JsonPropertyName("message")]
            public string Message { get; set; }
        }

        [JsonPropertyName("error")]
        public ErrorModel Error { get; set; }
    }
}
