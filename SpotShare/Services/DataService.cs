using Google.Cloud.Firestore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotShare.Services
{
    public class DataService
    {
        private readonly IConfiguration _config;

        public DataService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<Dictionary<string, object>> GetData(string collection,string document, string param=null, string val=null)
        {
            FirestoreDb db = FirestoreDb.Create(_config.GetValue<string>("projectId"));
            Dictionary<string, object> results = new Dictionary<string, object>();
            CollectionReference colRef = db.Collection(collection);
            if (param!=null && val !=null)
            {
                Query query = colRef.WhereEqualTo(param,val);
                QuerySnapshot snapshot = await query.GetSnapshotAsync();
                foreach (DocumentSnapshot doc in snapshot.Documents)
                {
                    results = doc.ToDictionary();
                }
            }
            else
            {
                QuerySnapshot snapshot = await colRef.GetSnapshotAsync();                    
                foreach (DocumentSnapshot doc in snapshot.Documents)
                {
                   results = doc.ToDictionary();
                }

            }
            return results;

        }

        public async Task AddData(string collection, string document, Dictionary<string, object> dataDict, string id)
        {
            FirestoreDb db = FirestoreDb.Create(_config.GetValue<string>("projectId"));

            DocumentReference docRef = db.Collection(collection).Document(document);

            await docRef.SetAsync(dataDict);
        }

    }
}
