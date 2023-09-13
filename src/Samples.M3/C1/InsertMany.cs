using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MongoDB.Driver;
using Samples.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Samples.M3.C1.InsertOne;

namespace Samples.M3.C1
{
    internal class InsertMany : ITestDatabaseSample
    {
        public async Task RunAsync(IMongoDatabase db, Configuration config)
        {
            var coll = db.GetCollection<TestDocument>("m3c1_insertMany");
            await coll.DeleteManyAsync(Builders<TestDocument>.Filter.Empty);

            await InsertManyAsync(coll, true);

            await InsertManyAsync(coll, false);
        }

        private async Task InsertManyAsync(IMongoCollection<TestDocument> coll, bool ordered)
        {
            var mode = ordered ? "Ordered" : "Unordered";
            Console.WriteLine(new string('-', 50));
            Console.WriteLine($"InsertMany for mode {mode}");
            var docs = GetDocuments(mode);
            try
            {
                var options = new InsertManyOptions() { IsOrdered = ordered };
                await coll.InsertManyAsync(docs, options);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            var insertedCount = await coll.CountDocumentsAsync(x => x.Mode == mode);
            Console.WriteLine($"Inserted {insertedCount} for mode {mode}");
        }

        private static IEnumerable<TestDocument> GetDocuments(string mode)
        {
            var docs = Enumerable.Range(0, 10).Select(x => new TestDocument() { Mode = mode }).ToArray();
            // Trigger duplicate Id error when inserting
            docs[1].Id = docs[0].Id;
            return docs;
        }

        public class TestDocument
        {
            [BsonRepresentation(BsonType.ObjectId)]
            public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

            public string Mode { get; set; } = string.Empty;
        }

    }
}
