using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SpotShare.Models;


namespace SpotShare.Services
{
    public class LinkService
    {
        private readonly IConfiguration _config;
        public LinkService(IConfiguration config)
        {
            _config = config;
        }
        public string GetLink(PlaylistData playlist)
        {
            var baseUrl = _config.GetValue<string>("baseUrl");

            return string.Format("{0}Share/AddToShare?id={1}", baseUrl, playlist.Id);
           
        }
    }
}
