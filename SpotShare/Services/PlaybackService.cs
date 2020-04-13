using SpotShare.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace SpotShare.Services
{
    public class PlaybackService
    {
		private readonly SpotApiService _spotApiService;
		public PlaybackService(SpotApiService spotApiService)
		{
			_spotApiService = spotApiService;
		}
		//This is where we'll make the call to start playback

		public async Task ProcessPush(List<UserData> users,string uri )
		{
			await Task.WhenAll(users.Select(x => PlayTrack(x,uri)));
		}

		public async Task PlayTrack(UserData user,string uri)
		{
			//Api call to play the track
			var json = JsonSerializer.Serialize(new Dictionary<string, string>() { { "context_uri", uri } });
			//PUT https://api.spotify.com/v1/me/player/play
			//Authorization
			//{			"context_uri":"spotify:playlist:___________" }
			await _spotApiService.Access("put", user.Token, "me/player/play", json);
		}
}
}
