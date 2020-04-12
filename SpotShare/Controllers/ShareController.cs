using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using SpotShare.Models;

namespace SpotShare.Controllers
{
    public class ShareController : Controller
    {
        private readonly ILogger<ShareController> _logger;
        private readonly IConfiguration _config;

        public ShareController(ILogger<ShareController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public IActionResult Index()
        {
            //Should immediately auth (user = true) unless cookie exists
            //Then option to create share or view shares
            return View();
        }

        public IActionResult CreateShare()
        {
            //Get playlist names and uris

            return View();
        }

        [HttpPost]
        public IActionResult CreateShare(PlaylistData playlist)
        {
            //Returns a view with a shareable link
            return View();
        }

        [HttpPost]
        public void StartShare(PlaylistData playlist)
        {
            //Re-auth self
           
            //Get users for the playlist
            //PlaybackService process push
        }

        [HttpPost]
        public void AddToShare(string id)
        {
            //This is link clicked from user
            //This will auth them
            //Add them to the share

            //return text instructions on how to make sure to use this
        }

        //Auth workflow
        public async Task<RedirectResult> Auth(bool user = false)
        {
            var scope = "user-modify-playback-state";
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
                baseUrl += "&redirect_uri=" + _config.GetValue<string>("redirectUri");
                baseUrl += "&scope="+scope;
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
                HttpContext.Response.Cookies.Append("spotauthtoke", tokenCookie.access_token);

            }

            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
