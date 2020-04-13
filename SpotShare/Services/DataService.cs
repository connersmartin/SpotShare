using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Google.Cloud.Storage.V1;
using Grpc.Auth;
using Grpc.Core;
using Microsoft.Extensions.Configuration;
using System;
using System.Text.Json;
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="collection">Top folder</param>
        /// <param name="param">What attribute are you looking for</param>
        /// <param name="val">what is that value?</param>
        /// <returns></returns>
        public async Task<List<Dictionary<string, object>>> GetData(string collection, string param=null, string val=null)
        {
            var resultDict = new List<Dictionary<string, object>>();
            try
            {
                var serviceAcct = GoogleCredential.FromJson(_config.GetValue<string>("jsonCreds"));
                Channel channel = new Channel(
                           FirestoreClient.DefaultEndpoint.Host, FirestoreClient.DefaultEndpoint.Port,
                           serviceAcct.ToChannelCredentials());
                FirestoreClient client = FirestoreClient.Create(channel);
                FirestoreDb db = FirestoreDb.Create(_config.GetValue<string>("projectId"), client);
                Dictionary<string, object> results = new Dictionary<string, object>();
                CollectionReference colRef = db.Collection(collection);


                Query query = colRef.WhereEqualTo(param, val);
                QuerySnapshot querySnapshot = await query.GetSnapshotAsync();

                foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
                {
                    resultDict.Add(documentSnapshot.ToDictionary());
                }

                return resultDict;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">What kind of object are you sending me</typeparam>
        /// <param name="collection">Top folder</param>
        /// <param name="obj">The data</param>
        /// <param name="id">The collection in the document</param>
        /// <returns></returns>
        public async Task AddData<T>(string collection, T obj , string id)
        {
            try
            {
                var serviceAcct = GoogleCredential.FromJson(_config.GetValue<string>("jsonCreds"));
                Channel channel = new Channel(
                           FirestoreClient.DefaultEndpoint.Host, FirestoreClient.DefaultEndpoint.Port,
                           serviceAcct.ToChannelCredentials());
                FirestoreClient client = FirestoreClient.Create(channel);

                FirestoreDb db = FirestoreDb.Create(_config.GetValue<string>("projectId"),client);

                CollectionReference docRef = db.Collection(collection);                             

                await docRef.AddAsync(obj);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
