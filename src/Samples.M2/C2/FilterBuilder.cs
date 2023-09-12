using MongoDB.Driver;
using Samples.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samples.M2.C2
{
    internal class FilterBuilder : ISampleWithDatabase
    {
        public async Task RunAsync(IMongoDatabase db, Configuration config)
        {
            var coll = db.GetCollection<Movie>("movies");
            var filter = Builders<Movie>.Filter.Eq(x => x.Title, "The Godfather")
                & Builders<Movie>.Filter.Gte(x => x.Year, 1970);
            var docs = await (await coll.FindAsync(filter)).ToListAsync();
            Console.WriteLine($"Retrieved {docs.Count} documents");
        }
    }
}
