using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotShare.Models
{
    [FirestoreData]
    public class UserData
    {
        [FirestoreProperty]
        public string PlaylistId { get; set; }
        [FirestoreProperty]
        public string Token { get; set; }
        [FirestoreProperty]
        public string RefreshToken { get; set; }
        [FirestoreProperty]
        public string Id { get; set; }
    }
}
