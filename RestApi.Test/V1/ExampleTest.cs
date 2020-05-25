using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using NUnit.Framework;
using RestApi.Models.V1;
using RestApi.Test.Models;

namespace RestApi.Test.V1
{
    [TestFixture]
    public class ExampleTest : TestBase
    {
        [TestCase("")]
        [TestCase("/")]
        public async Task TestExample(string route)
        {
            await CatchWebException(async () =>
            {
                using (var client = new WebClient())
                using (var stream = await client.OpenReadTaskAsync(
                    new Uri(SetUp.UrlToV1Example + route)))
                {
                    var data = await JsonSerializer.DeserializeAsync<WeatherForecast[]>(stream);

                    Assert.That(data.Length, Is.EqualTo(5));
                }
            });
        }

        [TestCase("20200303", new[] { "3 Mar, 2020", "4 Mar, 2020", "5 Mar, 2020", "6 Mar, 2020", "7 Mar, 2020" })]
        public async Task TestExampleWithDate(string date, string[] dates)
        {
            await CatchWebException(async () =>
            {
                WeatherForecast[] data;
                using (var client = new WebClient())
                using (var stream = await client.OpenReadTaskAsync(
                    new Uri($"{SetUp.UrlToV1Example}/{date}")))
                {
                    data = await JsonSerializer.DeserializeAsync<WeatherForecast[]>(stream);

                    Assert.That(data.Length, Is.EqualTo(5));
                    Assert.That(data[0].Date, Is.EqualTo(dates[0]));
                    Assert.That(data[1].Date, Is.EqualTo(dates[1]));
                    Assert.That(data[2].Date, Is.EqualTo(dates[2]));
                    Assert.That(data[3].Date, Is.EqualTo(dates[3]));
                    Assert.That(data[4].Date, Is.EqualTo(dates[4]));
                }
            });
        }

        [TestCase("202003030")]
        [TestCase("2020033")]
        [TestCase("test")]
        public async Task TestExampleWithInvalidDate(string date)
        {
            try
            {
                using (var client = new WebClient())
                using (var stream = await client.OpenReadTaskAsync(
                    new Uri($"{SetUp.UrlToV1Example}/{date}")))
                {
                    var data = await JsonSerializer.DeserializeAsync<WeatherForecast[]>(stream);
                }

                Assert.Fail("This test should have ended up with an error response.");
            }
            catch (WebException e)
            {
                Assert.That((e.Response as HttpWebResponse)?.StatusCode,
                    Is.EqualTo(HttpStatusCode.InternalServerError));
                var err = await JsonSerializer.DeserializeAsync<ValidationErrorResponseModel>(e.Response.GetResponseStream());
                Assert.That(err.Title,
                    Is.EqualTo($"'{date}' in URL should be in yyyyMMdd format. (Parameter 'date')"));
            }
        }
    }
}
