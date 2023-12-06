using MongoDB.Bson;
using MongoDB.Driver;
using Samples.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samples.M2.C1
{
    internal class Connect : ISampleWithConfig
    {
        public async Task RunAsync(Configuration config)
        {
            var client = new MongoClient(config.ConnectionString);
            var db = client.GetDatabase(config.SampleMoviesDatabaseName);
            var coll = db.GetCollection<BsonDocument>("movies");
            var allDocs = (await coll.FindAsync(FilterDefinition<BsonDocument>.Empty, new FindOptions<BsonDocument, BsonDocument>() { Limit = 10 })).ToEnumerable();
            foreach (var doc in allDocs)
            {
                Console.WriteLine($"{doc["title"].AsString.PadRight(80)} | {doc["year"]}");
            }
        }
    }
}
