using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SpotShare.Services
{

    public class SpotApiService
    {
        private readonly ILogger<SpotApiService> _logger;
        private readonly IConfiguration _config;
        public SpotApiService(ILogger<SpotApiService> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }
        //Call to interact with spotify service
        public async Task<string> Access(string method, string auth, string path, string json)
        {
            var url = _config.GetValue<string>("spotServiceUrl");
            var response = new HttpResponseMessage();

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Authorization", "Bearer "+auth);
                switch (method)
                {
                    case "get":
                        response = await client.GetAsync(url + path);
                        break;
                    case "put":
                        var putParams = new StringContent(json, Encoding.UTF8, "application/json");
                        response = await client.PostAsync(url + path, putParams);
                        break;
                    default:
                        response = new HttpResponseMessage() { Content = new StringContent("Invalid method invoked") };
                        break;
                }
            }
            return await response.Content.ReadAsStringAsync();
        }

    }
}
