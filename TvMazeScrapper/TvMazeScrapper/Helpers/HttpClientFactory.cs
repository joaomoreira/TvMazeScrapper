using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace TvMazeScrapper.Helpers
{
    public class HttpClientFactory : IHttpClientFactory
    {
        private readonly IConfiguration _configuration;

        public HttpClientFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public HttpClient CreateClient()
        {
            var client = new HttpClient();
            SetupClientDefaults(client);
            return client;
        }

        protected virtual void SetupClientDefaults(HttpClient client)
        {
            client.BaseAddress = new Uri(_configuration.GetValue<string>("BaseApiAddress"));
        }
    }
}
