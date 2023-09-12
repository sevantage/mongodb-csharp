using MongoDB.Driver;
using Samples.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samples.M2.C2
{
    internal class FilterLambda : ISampleWithDatabase
    {
        public async Task RunAsync(IMongoDatabase db, Configuration config)
        {
            var coll = db.GetCollection<Movie>("movies");
            var docs = await (await coll.FindAsync(x => x.Title == "The Godfather" && x.Year >= 1970)).ToListAsync();
            Console.WriteLine($"Retrieved {docs.Count} documents");
        }
    }
}
