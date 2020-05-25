using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace RestApi.Test
{
    public class TestBase
    {
        protected ILogger Logger;

        [OneTimeSetUp]
        public void SetUpBeforeAll()
        {
            Logger = SetUp.CreateLogger(this.GetType());
        }

        protected async Task CatchWebException(Func<Task> runTest)
        {
            try
            {
                await runTest();
            }
            catch (WebException e)
            {
                var error = $"Caught a web exception. {await GetErrorResponse(e)}";
                Logger.LogError($"Caught a web exception. {error}");
                Assert.Fail(error);
            }
        }

        protected async Task<string> GetErrorResponse(WebException e)
        {
            string body = null;
            var stream = e.Response.GetResponseStream();
            using (var reader = new StreamReader(stream))
            {
                body = await reader.ReadToEndAsync();
            }
            return body;
        }
    }
}
