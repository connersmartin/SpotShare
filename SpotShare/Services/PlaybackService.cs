using SpotShare.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotShare.Services
{
    public class PlaybackService
    {
		//This is where we'll make the call to start playback

		public async Task ProcessPush(List<UserData> users)
		{
			await Task.WhenAll(users.Select(x => PlayTrack(x)));
		}

		public async Task PlayTrack(UserData user)
		{
			//Api call to play the track

			//PUT https://api.spotify.com/v1/me/player/play
			//Authorization
			//{			"context_uri":"spotify:playlist:___________" }
		}
}
}
