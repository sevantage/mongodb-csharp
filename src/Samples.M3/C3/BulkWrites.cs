using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Samples.Base;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samples.M3.C3
{
    internal class BulkWrites : ITestDatabaseSample
    {
        public async Task RunAsync(IMongoDatabase db, Configuration config)
        {
            var coll = db.GetCollection<TestDocument>("m3c3_bulkWrites");
            await coll.DeleteManyAsync(Builders<TestDocument>.Filter.Empty);

            var count = 100000;
            Console.WriteLine($"Starting bulk upsert of {count / 10} documents ({count} write operations)");
            var sw = Stopwatch.StartNew();
            var upserts = Enumerable.Range(0, count).Select(x => new UpdateOneModel<TestDocument>(
                Builders<TestDocument>.Filter.Eq(y => y.Id, x / 10),
                Builders<TestDocument>.Update.Inc(y => y.Counter, 1))
                { 
                    IsUpsert = true 
                });
            var result = await coll.BulkWriteAsync(upserts, new BulkWriteOptions() { IsOrdered = false });
            sw.Stop();
            Console.WriteLine($"Requests: {result.RequestCount}, Matched: {result.MatchedCount}, Modified: {result.ModifiedCount}, Upserts: {result.Upserts.Where(x => x != null).Count()}, Time taken: {sw.ElapsedMilliseconds}ms");
        }

        public class TestDocument
        {
            public int Id { get; set; }

            public int Counter { get; set; }
        }
    }
}
