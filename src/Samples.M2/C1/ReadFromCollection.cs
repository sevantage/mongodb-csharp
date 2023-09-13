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
    internal class ReadFromCollection : ISampleWithClient
    {
        public async Task RunAsync(IMongoClient client, Configuration config)
        {
            var db = client.GetDatabase(config.SampleMoviesDatabaseName);
            var coll = db.GetCollection<BsonDocument>("movies");
            var allDocs = (await coll.FindAsync(FilterDefinition<BsonDocument>.Empty)).ToEnumerable();
            Console.WriteLine($"Read {allDocs.Count()} documents");
        }
    }
}
