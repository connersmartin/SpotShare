using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SpotShare.Models;

namespace SpotShare.Controllers
{
    public class AuthController : Controller
    {
        private readonly IConfiguration _config;
        public AuthController(IConfiguration config)
        {
            _config = config;
        }
        //Sharer Auth workflow
        public async Task<RedirectResult> Auth(bool user = false, string id = null)
        {
            var redirect = _config.GetValue<string>("redirectUri");
            var scope = "user-modify-playback-state user-read-playback-state";
            if (id!=null)
            {
                Response.Cookies.Append("playlistId", id);
            }
            if (user)
            {
                scope = "user-read-email playlist-read-collaborative";
            }
            HttpResponseMessage response = new HttpResponseMessage();
            using (var client = new HttpClient())
            {
                var baseUrl = "https://accounts.spotify.com/authorize";
                baseUrl += "?client_id=" + _config.GetValue<string>("clientId");
                baseUrl += "&response_type=code";
                baseUrl += "&redirect_uri=" + redirect;
                baseUrl += "&scope=" + scope;
                var baseUri = new Uri(baseUrl);
                response = await client.GetAsync(baseUri).ConfigureAwait(true);
            }
            return Redirect(response.RequestMessage.RequestUri.ToString());
        }

        //Second part of auth workflow
        public async Task<IActionResult> CodeToken()
        {
            if (Request.Query.ContainsKey("code"))
            {
                HttpResponseMessage response;
                var responseContent = "";
                using (var client = new HttpClient())
                {
                    var baseUrl = new Uri("https://accounts.spotify.com/api/token");
                    var body = new Dictionary<string, string>()
                    {
                        { "grant_type","authorization_code" },
                        { "code", Request.Query["code"].ToString() },
                        {"redirect_uri",  _config.GetValue<string>("redirectUri") },
                        {"client_id",  _config.GetValue<string>("clientId")},
                        {"client_secret",  _config.GetValue<string>("clientSecret") }
                    };
                    response = await client.PostAsync(baseUrl, new FormUrlEncodedContent(body)).ConfigureAwait(true);
                    responseContent = response.Content.ReadAsStringAsync().Result;
                }
                var tokenCookie = JsonSerializer.Deserialize<Token>(responseContent);
                HttpContext.Response.Cookies.Append("spottoke", tokenCookie.access_token);
                HttpContext.Response.Cookies.Append("spotrefresh", tokenCookie.refresh_token);
            }
            if (Request.Cookies.Keys.Contains("playlistId"))
            {
                return RedirectToAction("AddToShare", "Share", new { id = Request.Cookies["playlistId"].ToString() });
            }
            else
            {
                return RedirectToAction("Index", "Share");
            }

        }
    }
}