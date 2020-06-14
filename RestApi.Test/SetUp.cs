﻿using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Serilog;
using Serilog.Extensions.Logging;

namespace RestApi.Test
{
    [SetUpFixture]
    public class SetUp
    {
        public const int ApiServiceStartUpTimeoutSeconds = 60;
        public static Task ApiService;

        public const string ApiServiceBaseUrl = "https://localhost:62184";
        public const string UrlToSwagger = ApiServiceBaseUrl + "/swagger";
        public const string UrlToV1Example = ApiServiceBaseUrl + "/api/v1/WeatherForecast";
        public const string UrlToV1_1Example = ApiServiceBaseUrl + "/api/v1.1/WeatherForecast";
        public const string UrlToV2Example = ApiServiceBaseUrl + "/api/v2/WeatherForecast";

        static ILoggerFactory _loggerFactory;
        static Microsoft.Extensions.Logging.ILogger _logger;

        public static IHost Host { get; private set; }

        public static Microsoft.Extensions.Logging.ILogger CreateLogger<T>()
            => CreateLogger(typeof(T));

        public static Microsoft.Extensions.Logging.ILogger CreateLogger(Type type)
        {
            if (_loggerFactory == null)
            {
                _loggerFactory = new SerilogLoggerFactory(new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.Console()
                    .CreateLogger());
            }

            return _loggerFactory.CreateLogger(type);
        }

        [OneTimeSetUp]
        public async Task SetUpBeforeAll()
        {
            _logger = CreateLogger<SetUp>();

            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
            Environment.SetEnvironmentVariable("ASPNETCORE_URLS", ApiServiceBaseUrl);
            ApiService = Task.Run(async () =>
            {
                Host = Program.CreateHostBuilder(new string[] { }).Build();
                await Host.RunAsync();
            });

            Stream stream = null;
            DateTime start = DateTime.UtcNow;
            DateTime timeout = DateTime.UtcNow + TimeSpan.FromSeconds(ApiServiceStartUpTimeoutSeconds);
            Exception exception = null;
            while (true)
            {
                try
                {
                    await Task.Delay(500);
                    _logger.LogDebug("Checking service startup...");
                    var client = new WebClient();
                    using (stream = await client.OpenReadTaskAsync(new Uri(ApiServiceBaseUrl))) { }
                    var wakeup = DateTime.UtcNow - start;
                    _logger.LogInformation("API service started successfully.");
                    break;
                }
                catch (Exception e)
                {
                    exception = e;
                    if (e is WebException && ((WebException)e).Status != WebExceptionStatus.UnknownError)
                    {
                        break;
                    }

                    if (DateTime.UtcNow > timeout)
                    {
                        var error = $"Test target API service did not start up within {ApiServiceStartUpTimeoutSeconds} seconds";
                        _logger.LogError(error);
                        throw new Exception(error, exception);
                    }
                }
            }
        }

        [OneTimeTearDown]
        public void TearDownAfterAll()
        {
            _loggerFactory.Dispose();
            Host.Dispose();
        }
    }
}
