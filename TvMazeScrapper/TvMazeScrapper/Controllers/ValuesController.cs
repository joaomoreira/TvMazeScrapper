using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using TvMazeScrapper.Helpers;
using TvMazeScrapper.Models;

namespace TvMazeScrapper.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private Helpers.IHttpClientFactory _httpClientFactory;
        private HttpClient _client;
      

        public ValuesController(IConfiguration configuration,
            Helpers.IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _client = _httpClientFactory.CreateClient();
        }

        [HttpGet()]
        public ActionResult<string> Get()
        {
            List<TvShowModel> savedTvShows = new List<TvShowModel>();
            string fileName = _configuration.GetValue<string>("FileName");
            int pageLimit = _configuration.GetValue<int>("PageLimit");

            if (System.IO.File.Exists(fileName))
            {
                Console.WriteLine("Reading saved file");
                Stream openFileStream = System.IO.File.OpenRead(fileName);
                BinaryFormatter deserializer = new BinaryFormatter();
                savedTvShows = (List<TvShowModel>)deserializer.Deserialize(openFileStream);
                openFileStream.Close();
            }

            return JsonConvert.SerializeObject(savedTvShows.OrderBy(tv=> tv.Id));
        }

        // GET api/values/5
        [HttpGet("{page}")]
        public async Task<ActionResult<string>> GetAsync(int page)
        {
            List<TvShowModel> tvShows= new List<TvShowModel>();
            List<TvShowModel> savedTvShows = new List<TvShowModel>();

            string fileName = _configuration.GetValue<string>("FileName");
            int pageLimit = _configuration.GetValue<int>("PageLimit");

            if (System.IO.File.Exists(fileName))
            {
                Console.WriteLine("Reading saved file");
                Stream openFileStream = System.IO.File.OpenRead(fileName);
                BinaryFormatter deserializer = new BinaryFormatter();
                savedTvShows = (List<TvShowModel>)deserializer.Deserialize(openFileStream);
                openFileStream.Close();
            
                tvShows = savedTvShows.Where(tv =>
                tv.Id >= page * pageLimit
                && tv.Id <= (page == 0 ? pageLimit : (page+1) * pageLimit) - 1).ToList();
            }

            bool hasDataInStorage = tvShows.Count() > 0;

            HttpResponseMessage response = null;
            string jsonString = string.Empty;

            if (!hasDataInStorage)
            {
                _client.DefaultRequestHeaders.Accept.Clear();
                _client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                ServicePointManager
                  .ServerCertificateValidationCallback +=
                  (sender, cert, chain, sslPolicyErrors) => true;

                string reportName = string.Empty;
                response = await _client.GetAsync("shows?page=" + page);
                jsonString = await response.Content.ReadAsStringAsync();
                tvShows = JsonConvert.DeserializeObject<List<TvShowModel>>(jsonString);
            }

            foreach (TvShowModel tvshow in tvShows)
            {
                try
                {
                    if (tvshow.Cast == null || tvshow.Cast.Count() == 0)
                    {
                        tvshow.Cast = new List<CastMemberModel>();
                        response = await _client.GetAsync(string.Format("shows/{0}/cast", tvshow.Id));
                        jsonString = await response.Content.ReadAsStringAsync();
                        tvshow.Cast = JsonConvert.DeserializeObject<List<CastMemberModel>>(jsonString);
                    }

                    tvshow.Cast = tvshow.Cast.OrderByDescending(cm => cm.Person.Birthday).ToList();
                }
                catch (Exception ex)
                {
                    continue;
                }
            }

            Stream SaveFileStream = System.IO.File.Create(fileName);
            BinaryFormatter serializer = new BinaryFormatter();
            serializer.Serialize(SaveFileStream, savedTvShows.Union(tvShows).ToList());
            SaveFileStream.Close();
            
            return JsonConvert.SerializeObject(tvShows);
        }
    }
}
