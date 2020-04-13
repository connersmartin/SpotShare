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

        public async Task<Dictionary<string, object>> GetData(string collection,string document, string param=null, string val=null)
        {
            try
            {
                var serviceAcct = GoogleCredential.FromJson(_config.GetValue<string>("jsonCreds"));
                Channel channel = new Channel(
                           FirestoreClient.DefaultEndpoint.Host, FirestoreClient.DefaultEndpoint.Port,
                           serviceAcct.ToChannelCredentials());
                FirestoreClient client = FirestoreClient.Create(channel);
                FirestoreDb db = FirestoreDb.Create(_config.GetValue<string>("projectId"),client);
                Dictionary<string, object> results = new Dictionary<string, object>();
                CollectionReference colRef = db.Collection(collection);
                if (param != null && val != null)
                {
                    DocumentReference docRef = colRef.Document(document);

                    var snapshot = await docRef.GetSnapshotAsync();
                    results = snapshot.ToDictionary();
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
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task AddData<T>(string collection, string document, T obj , string id)
        {
            try
            {
                var serviceAcct = GoogleCredential.FromJson(_config.GetValue<string>("jsonCreds"));
                Channel channel = new Channel(
                           FirestoreClient.DefaultEndpoint.Host, FirestoreClient.DefaultEndpoint.Port,
                           serviceAcct.ToChannelCredentials());
                FirestoreClient client = FirestoreClient.Create(channel);

                FirestoreDb db = FirestoreDb.Create(_config.GetValue<string>("projectId"),client);

                DocumentReference docRef = db.Collection(collection).Document(document);

                var objDict = new Dictionary<string, object>(){ { id, obj } };

                await docRef.SetAsync(objDict);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
