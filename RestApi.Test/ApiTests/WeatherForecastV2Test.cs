using System;
using System.IO;
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
    public class WeatherForecastV2Test : TestBase
    {
        [TestCase("")]
        public async Task TestResponseType(string route)
        {
            await CatchWebException(async () =>
            {
                using (var client = new WebClient())
                using (var stream = await client.OpenReadTaskAsync(
                    new Uri(SetUp.UrlToV2Example + route)))
                {
                    var data = await JsonSerializer.DeserializeAsync<WeatherForecastV2[]>(stream);

                    Assert.That(data.Length, Is.EqualTo(5/* days */ * 3/* areas */));
                    Assert.That(data[0].TemperatureC, Is.Not.EqualTo(0));
                }
            });
        }

        [TestCase("")]
        [TestCase("/")]
        public async Task TestWithEmptyDate(string route)
        {
            await CatchWebException(async () =>
            {
                using (var client = new WebClient())
                using (var stream = await client.OpenReadTaskAsync(
                    new Uri(SetUp.UrlToV2Example + route)))
                {
                    var data = await JsonSerializer.DeserializeAsync<WeatherForecastV2[]>(stream);

                    Assert.That(data.Length, Is.EqualTo(5/* days */ * 3/* areas */));
                }
            });
        }

        [TestCase("20200303", "2020-03-03")]
        [TestCase("", null)]
        public async Task TestWithValidDate(string date, DateTime? firstDate)
        {
            await CatchWebException(async () =>
            {
                var now = DateTime.UtcNow;
                using (var client = new WebClient())
                using (var stream = await client.OpenReadTaskAsync(
                    new Uri($"{SetUp.UrlToV2Example}?from={date}")))
                {
                    var data = await JsonSerializer.DeserializeAsync<WeatherForecastV2[]>(stream);

                    Assert.That(data.Length, Is.EqualTo(5/* days */ * 3/* areas */));
                    Assert.That(data.Min(d => d.Date),
                        Is.EqualTo(firstDate ?? new DateTime(now.Year, now.Month, now.Day)));
                }
            });
        }

        [TestCase("202003030")]
        [TestCase("2020033")]
        [TestCase("New York")]
        public async Task TestWithInvalidDate(string date)
        {
            try
            {
                using (var client = new WebClient())
                using (var stream = await client.OpenReadTaskAsync(
                    new Uri($"{SetUp.UrlToV2Example}?from={date}")))
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

        [TestCase("", "20200305", 4, "2020-03-05")]
        [TestCase("", "20200305", null, "2020-03-05")]
        [TestCase("", "", 3, null)]
        [TestCase("", "", null, null)]
        [TestCase("/", "20200305", 4, "2020-03-05")]
        [TestCase("/", "20200305", null, "2020-03-05")]
        [TestCase("/", "", 3, null)]
        [TestCase("/", "", null, null)]
        public async Task TestWithEmptyAreaAndValidDate(string route, string date, int? days, DateTime? firstDate)
        {
            await CatchWebException(async () =>
            {
                var now = DateTime.UtcNow;
                using (var client = new WebClient())
                using (var stream = await client.OpenReadTaskAsync(
                    new Uri($"{SetUp.UrlToV2Example}/Area{route}?from={date}{(days == null ?  "" : $"&days={days}")}")))
                {
                    var data = await JsonSerializer.DeserializeAsync<WeatherForecastV2[]>(stream);

                    Assert.That(data.Length, Is.EqualTo((days ?? 5/* days */) * 3/* areas */));
                    Assert.That(data.Min(d => d.Date),
                        Is.EqualTo(firstDate ?? new DateTime(now.Year, now.Month, now.Day)));
                }
            });
        }

        [TestCase("New York", "20200305", 4, "2020-03-05")]
        [TestCase("New York", "20200305", null, "2020-03-05")]
        [TestCase("New York", "", 3, null)]
        [TestCase("New York", "", null, null)]
        public async Task TestWithValidAreaAndValidDate(string area, string date, int? days, DateTime? firstDate)
        {
            await CatchWebException(async () =>
            {
                var now = DateTime.UtcNow;
                using (var client = new WebClient())
                using (var stream = await client.OpenReadTaskAsync(
                    new Uri($"{SetUp.UrlToV2Example}/Area/{area}?from={date}{(days == null ? "" : $"&days={days}")}")))
                {
                    var data = await JsonSerializer.DeserializeAsync<WeatherForecastV2[]>(stream);

                    Assert.That(data.Length, Is.EqualTo(days ?? 5/* days */));
                    Assert.That(data.Min(d => d.Date),
                        Is.EqualTo(firstDate ?? new DateTime(now.Year, now.Month, now.Day)));
                }
            });
        }

        [TestCase("Auckland", "20200303")]
        public async Task TestWithInvalidAreaAndValidDate(string area, string date)
        {
            try
            {
                using (var client = new WebClient())
                using (var stream = await client.OpenReadTaskAsync(
                    new Uri($"{SetUp.UrlToV2Example}/Area/{area}?from={date}")))
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
        public async Task TestWithValidAreaAndInvalidDate(string area, string date)
        {
            try
            {
                using (var client = new WebClient())
                using (var stream = await client.OpenReadTaskAsync(
                    new Uri($"{SetUp.UrlToV2Example}/Area/{area}?from={date}")))
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
