using System;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using NUnit.Framework;
using RestApi.Models;
using RestApi.Test.Models;

namespace RestApi.Test.ApiTests
{
    [TestFixture]
    public class ExampleV2Test : TestBase
    {
        [TestCase("")]
        [TestCase("/20200303")]
        public async Task TestExample(string route)
        {
            try
            {
                using (var client = new WebClient())
                using (var stream = await client.OpenReadTaskAsync(
                    new Uri(SetUp.UrlToV2Example + route)))
                {
                    var data = await JsonSerializer.DeserializeAsync<WeatherForecastV2[]>(stream);
                }

                Assert.Fail("This test should have ended up with an error response.");
            }
            catch (WebException e)
            {
                Assert.That((e.Response as HttpWebResponse)?.StatusCode,
                    Is.EqualTo(HttpStatusCode.BadRequest));
                var data = await JsonSerializer.DeserializeAsync<BadRequestResponseModel>(
                    e.Response.GetResponseStream());
                Assert.That(data?.Error?.Message,
                    Is.EqualTo($"The HTTP resource that matches the request URI 'https://localhost:62184/api/v2/Example{route}' does not support the API version '2'."));
            }
        }

        [TestCase("")]
        [TestCase("/")]
        public async Task TestExampleWF(string route)
        {
            await CatchWebException(async () =>
            {
                using (var client = new WebClient())
                using (var stream = await client.OpenReadTaskAsync(
                    new Uri(SetUp.UrlToV2ExampleWF + route)))
                {
                    var data = await JsonSerializer.DeserializeAsync<WeatherForecastV2[]>(stream);

                    Assert.That(data.Length, Is.EqualTo(5 * 3));
                }
            });
        }

        [TestCase("20200303", "2020-03-03")]
        [TestCase("", null)]
        public async Task TestExampleWFWithValidDate(string date, DateTime? firstDate)
        {
            await CatchWebException(async () =>
            {
                var now = DateTime.UtcNow;
                using (var client = new WebClient())
                using (var stream = await client.OpenReadTaskAsync(
                    new Uri($"{SetUp.UrlToV2ExampleWF}?from={date}")))
                {
                    var data = await JsonSerializer.DeserializeAsync<WeatherForecastV2[]>(stream);

                    Assert.That(data.Length, Is.EqualTo(5 * 3));
                    Assert.That(data.Min(d => d.Date),
                        Is.EqualTo(firstDate ?? new DateTime(now.Year, now.Month, now.Day)));
                }
            });
        }

        [TestCase("202003030")]
        [TestCase("2020033")]
        [TestCase("New York")]
        public async Task TestExampleWFWithInvalidDate(string date)
        {
            try
            {
                using (var client = new WebClient())
                using (var stream = await client.OpenReadTaskAsync(
                    new Uri($"{SetUp.UrlToV2ExampleWF}?from={date}")))
                {
                    var data = await JsonSerializer.DeserializeAsync<WeatherForecastV2[]>(stream);
                }

                Assert.Fail("This test should have ended up with an error response.");
            }
            catch (WebException e)
            {
                Assert.That((e.Response as HttpWebResponse)?.StatusCode,
                    Is.EqualTo(HttpStatusCode.InternalServerError));
                var data = await JsonSerializer.DeserializeAsync<ValidationErrorResponseModel>(
                    e.Response.GetResponseStream());
                Assert.That(data.Title,
                    Is.EqualTo($"'{date}' should be in yyyyMMdd format. (Parameter 'from')"));
            }
        }

        [TestCase("New York", "20200305", 4, "2020-03-05")]
        [TestCase("New York", "20200305", null, "2020-03-05")]
        [TestCase("New York", "", 3, null)]
        [TestCase("New York", "", null, null)]
        public async Task TestExampleWFWithValidAreaAndValidDate(string area, string date, int? days, DateTime? firstDate)
        {
            await CatchWebException(async () =>
            {
                var now = DateTime.UtcNow;
                using (var client = new WebClient())
                using (var stream = await client.OpenReadTaskAsync(
                    new Uri($"{SetUp.UrlToV2ExampleWF}/{area}?from={date}{(days == null ?  "" : $"&days={days}")}")))
                {
                    var data = await JsonSerializer.DeserializeAsync<WeatherForecastV2[]>(stream);

                    Assert.That(data.Length, Is.EqualTo(days ?? 5));
                    Assert.That(data.Min(d => d.Date),
                        Is.EqualTo(firstDate ?? new DateTime(now.Year, now.Month, now.Day)));
                }
            });
        }

        [TestCase("Auckland", "20200303")]
        public async Task TestExampleWFWithInvalidAreaAndValidDate(string area, string date)
        {
            try
            {
                using (var client = new WebClient())
                using (var stream = await client.OpenReadTaskAsync(
                    new Uri($"{SetUp.UrlToV2ExampleWF}/{area}?from={date}")))
                {
                    var data = await JsonSerializer.DeserializeAsync<WeatherForecastV2[]>(stream);
                }
                Assert.Fail("This test should have ended up with an error response.");
            }
            catch (WebException e)
            {
                Assert.That((e.Response as HttpWebResponse)?.StatusCode,
                    Is.EqualTo(HttpStatusCode.InternalServerError));
                var data = await JsonSerializer.DeserializeAsync<ValidationErrorResponseModel>(
                    e.Response.GetResponseStream());
                Assert.That(data.Title,
                    Is.EqualTo($"'{area}' is not supported area. (Parameter 'area')"));
            }
        }

        [TestCase("New York", "202003030")]
        [TestCase("New York", "2020033")]
        [TestCase("New York", "test")]
        public async Task TestExampleWFWithValidAreaAndInvalidDate(string area, string date)
        {
            try
            {
                using (var client = new WebClient())
                using (var stream = await client.OpenReadTaskAsync(
                    new Uri($"{SetUp.UrlToV2ExampleWF}/{area}?from={date}")))
                {
                    var data = await JsonSerializer.DeserializeAsync<WeatherForecastV2[]>(stream);
                }

                Assert.Fail("This test should have ended up with an error response.");
            }
            catch (WebException e)
            {
                Assert.That((e.Response as HttpWebResponse)?.StatusCode,
                    Is.EqualTo(HttpStatusCode.InternalServerError));
                var data = await JsonSerializer.DeserializeAsync<ValidationErrorResponseModel>(
                    e.Response.GetResponseStream());
                Assert.That(data.Title,
                    Is.EqualTo($"'{date}' should be in yyyyMMdd format. (Parameter 'from')"));
            }
        }
    }
}
