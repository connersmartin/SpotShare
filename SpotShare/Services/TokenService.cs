using Microsoft.Extensions.Configuration;
using SpotShare.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace SpotShare.Services
{
    public class TokenService
    {
        private readonly IConfiguration _config;
        public TokenService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<string> RefreshToken(string refreshToken)
        {
            HttpResponseMessage response;
            var responseContent = "";
            using (var client = new HttpClient())
            {
                var baseUrl = new Uri("https://accounts.spotify.com/api/token");
                var body = new Dictionary<string, string>()
                    {
                        { "grant_type","refresh_token" },
                        { "refresh_token", refreshToken },
                        {"redirect_uri",  _config.GetValue<string>("redirectUri") },
                        {"client_id",  _config.GetValue<string>("clientId")},
                        {"client_secret",  _config.GetValue<string>("clientSecret") }
                    };
                response = await client.PostAsync(baseUrl, new FormUrlEncodedContent(body)).ConfigureAwait(true);
                responseContent = response.Content.ReadAsStringAsync().Result;
            }
            var tokenCookie = JsonSerializer.Deserialize<Token>(responseContent);
            return tokenCookie.access_token;
        }
    }
}
