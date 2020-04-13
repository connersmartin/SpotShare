using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotShare.Models
{
    [FirestoreData]
    public class PlaylistData
    {
        [FirestoreProperty]
        public string Id { get; set; }
        [FirestoreProperty]
        public string UserId { get; set; }
        [FirestoreProperty]
        public string SpotifyUri { get; set; }        
    }
}
