using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Samples.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samples.M3.C2
{
    internal class UpdateDocument : ITestDatabaseSample
    {
        public async Task RunAsync(IMongoDatabase db, Configuration config)
        {
            var coll = db.GetCollection<TestDocument>("m3c2_updateDoc");
            await coll.DeleteManyAsync(Builders<TestDocument>.Filter.Empty);

            // Upsert document
            var filter = Builders<TestDocument>.Filter.Eq(x => x.Name, "Upserted");
            var update = Builders<TestDocument>.Update
                .SetOnInsert(x => x.InsertedAt, DateTime.UtcNow)
                .Push(x => x.Timestamps, DateTime.UtcNow);
            var result = await coll.UpdateOneAsync(filter, update, new UpdateOptions() { IsUpsert = true });
            Console.WriteLine($"Result: matched: {result.MatchedCount}, modified: {result.ModifiedCount}, upserted id: {result.UpsertedId?.AsObjectId.ToString()}");

            // Update again
            var update2 = Builders<TestDocument>.Update
                .SetOnInsert(x => x.InsertedAt, DateTime.UtcNow)
                .Push(x => x.Timestamps, DateTime.UtcNow);
            result = await coll.UpdateOneAsync(filter, update2, new UpdateOptions() { IsUpsert = true });
            Console.WriteLine($"Result: matched: {result.MatchedCount}, modified: {result.ModifiedCount}, upserted id: {result.UpsertedId?.AsObjectId.ToString()}");

            // Prepare for update many
            await coll.InsertManyAsync(Enumerable.Range(0, 3).Select(x => new TestDocument() { Name = $"Many{x}" }));

            var updateManyResult = await coll.UpdateManyAsync(x => new string[] { "Many0", "Many2" }.Contains(x.Name),
                Builders<TestDocument>.Update.Push(x => x.Timestamps, DateTime.UtcNow));
            Console.WriteLine($"UpdateMany Result: matched: {updateManyResult.MatchedCount}, modified: {updateManyResult.ModifiedCount}");
        }

        public class TestDocument
        {
            [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
            public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

            public string Name { get; set; } = string.Empty;

            public DateTime InsertedAt { get; set; } = DateTime.UtcNow;

            public ICollection<DateTime> Timestamps { get; set; } = new List<DateTime>();
        }
    }
}
