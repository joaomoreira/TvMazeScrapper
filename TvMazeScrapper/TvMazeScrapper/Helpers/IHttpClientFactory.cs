using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace TvMazeScrapper.Helpers
{
    public interface IHttpClientFactory 
    {
        HttpClient CreateClient();
    }
}
