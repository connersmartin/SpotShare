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
using SpotShare.Services;

namespace SpotShare.Controllers
{
    public class ShareController : Controller
    {
        private readonly ILogger<ShareController> _logger;
        private readonly IConfiguration _config;
        private readonly SpotApiService _spotService;
        private readonly DataService _dataService;
        private readonly Helper _helper;
        private readonly LinkService _linkService;
        private readonly PlaybackService _playbackService;
        private readonly TokenService _tokenService;

        public ShareController(ILogger<ShareController> logger,
                              IConfiguration config,
                              SpotApiService spotService,
                              Helper helper,
                              DataService dataService,
                              LinkService linkService,
                              PlaybackService playbackService,
                              TokenService tokenService)
        {
            _logger = logger;
            _config = config;
            _spotService = spotService;
            _helper = helper;
            _dataService = dataService;
            _linkService = linkService;
            _playbackService = playbackService;
            _tokenService = tokenService;
        }

        public IActionResult Index()
        {
            //Should immediately auth (user = true) unless cookie exists
            //Then option to create share or view shares
            return View();
        }
        [HttpGet]
        public IActionResult CreateShare()
        {
            //Get playlist names and uris

            return PartialView();
        }

        [HttpPost]
        public async Task<IActionResult> ShareCreate(string id)
        {
            var playlist = new PlaylistData() { SpotifyUri = id };
            //Get cookie
            var token = Request.Cookies["spottoke"];
            //Get userid from spotify with cookie

            //Get the actual user id
            playlist.UserId = await GetUserId(token);

            playlist.Id = Guid.NewGuid().ToString();            

            await _dataService.AddData<PlaylistData>("playlistdata", playlist, playlist.Id);

            var link = _linkService.GetLink(playlist);

            //Returns a view with a shareable link
            return PartialView("CreateShareResult", new Link() { Url = link });
        }

        [HttpGet]
        public async Task<IActionResult> StartShare()
        {
            var model = new List<PlaylistData>();
            //Re-auth self
            //Get cookie
            var token = Request.Cookies["spottoke"];
            //Get userid from spotify with cookie
            var userId = await GetUserId(token);
            //Get open shares
            var dataReponse = await _dataService.GetData("playlistdata", "UserId",userId);

            foreach (var data in dataReponse)
            {
                var interim = _helper.MapObject<PlaylistData>(data);
                model.Add(interim);
            }
            return PartialView(model);            
        }

        [HttpPost]
        public async Task<IActionResult> StartShare([FromBody] PlaylistData playlist)
        {
            var model = new List<UserData>();
            //Get users for the playlist
            var dataReponse = await _dataService.GetData("userdata","PlaylistId", playlist.Id);
            foreach (var data in dataReponse)
            {
                var interim = _helper.MapObject<UserData>(data);
                //refresh token just in case
                var refresh = await _tokenService.RefreshToken(interim.RefreshToken);
                interim.Token = refresh;
                model.Add(interim);
            }            
            //PlaybackService process push
            await _playbackService.ProcessPush(model,playlist.SpotifyUri);

            return PartialView("ShareSuccess");
        }

        [HttpGet]
        public async Task<IActionResult> AddToShare(string id)
        {
            //This is a redirect from auth

            //Check to see if they're already in the share
            var dataReponse = await _dataService.GetData("userdata", "PlaylistId", id);
            foreach (var data in dataReponse)
            {
                var interim = _helper.MapObject<UserData>(data);
                //Delete them if they do
                if (interim.UserId== await GetUserId(Request.Cookies["spottoke"]))
                {
                    await _dataService.DeleteData("userdata", "PlaylistId", id, "UserId", interim.UserId);
                    return View("RemoveFromShare");
                }
            }         
            //Add them to the share
            var user = new UserData()
            {
                Id = Guid.NewGuid().ToString(),
                PlaylistId = id,
                UserId = await GetUserId(Request.Cookies["spottoke"]),
                Token = Request.Cookies["spottoke"],
                RefreshToken = Request.Cookies["spotrefresh"]
            };
            await _dataService.AddData<UserData>("userdata", user, user.Id);

            //return text instructions on how to make sure to use this
            return View();
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


        private async Task<string> GetUserId(string token)
        {
            var userResponse = await _spotService.Access("get", token, "me", null);
            //Get the actual user id
            return JsonSerializer.Deserialize<Dictionary<string, object>>(userResponse)["id"].ToString();
        }
    }
}
