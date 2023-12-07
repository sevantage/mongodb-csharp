using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Samples.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Samples.M3.C2
{
    internal class UpdateArray : ITestDatabaseSample
    {
        private readonly ConsoleHelper _consoleHelper;

        public UpdateArray(ConsoleHelper consoleHelper)
        {
            _consoleHelper = consoleHelper;
        }

        public async Task RunAsync(IMongoDatabase db, Configuration config)
        {
            var coll = db.GetCollection<TestDocument>("m3c2_updateArray");
            await coll.DeleteManyAsync(Builders<TestDocument>.Filter.Empty);

            TestDocument doc = new TestDocument()
            {
                Items = Enumerable.Range(0, 6).Select(x => new Item() { Key = new string((char)('A' + x), 1), Value = x }).ToList(),
            };
            await coll.InsertOneAsync(doc);
            await PrintArray(coll, doc.Id);

            var filter = Builders<TestDocument>.Filter.Eq(x => x.Id, doc.Id);
            // Add to set
            var result = await coll.UpdateOneAsync(filter, Builders<TestDocument>.Update.AddToSet(x => x.Items, new Item() { Key = "A", Value = 0 }));
            Console.WriteLine($"AddToSet (A-0) matched {result.MatchedCount} documents and modified {result.ModifiedCount} documents");
            await PrintArray(coll, doc.Id);
            // Push
            result = await coll.UpdateOneAsync(filter, Builders<TestDocument>.Update.Push(x => x.Items, new Item() { Key = "A", Value = 0 }));
            Console.WriteLine($"Push (A-0) matched {result.MatchedCount} documents and modified {result.ModifiedCount} documents");
            await PrintArray(coll, doc.Id);
            // Pull
            result = await coll.UpdateOneAsync(filter, Builders<TestDocument>.Update.Pull(x => x.Items, new Item() { Key = "A", Value = 0 }));
            Console.WriteLine($"Pull (A-0) matched {result.MatchedCount} documents and modified {result.ModifiedCount} documents");
            await PrintArray(coll, doc.Id);
            // PullAll
            result = await coll.UpdateOneAsync(filter, Builders<TestDocument>.Update.PullAll(x => x.Items, new Item[]
            {
                new () { Key = "A", Value = 0 },
                new () { Key = "B", Value = 1 },
                new () { Key = "C", Value = 2 },
            }));
            Console.WriteLine($"PullAll (A-0, B-1, C-2) matched {result.MatchedCount} documents and modified {result.ModifiedCount} documents");
            await PrintArray(coll, doc.Id);
            // PullFilter
            result = await coll.UpdateOneAsync(filter, Builders<TestDocument>.Update.PullFilter(x => x.Items, x => x.IsEven));
            Console.WriteLine($"PullFilter (IsEven) matched {result.MatchedCount} documents and modified {result.ModifiedCount} documents");
            await PrintArray(coll, doc.Id);
        }

        private async Task PrintArray(IMongoCollection<TestDocument> coll, string id)
        {
            var doc = await (await coll.FindAsync(x => x.Id == id)).SingleAsync();
            Console.WriteLine($"Array ({doc.Items.Count}):");
            foreach (var item in doc.Items)
                Console.WriteLine($"{item.Key}-{item.Value}");
            _consoleHelper.Separator();
        }

        public class TestDocument
        {
            [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
            public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

            public ICollection<Item> Items { get; set; } = new List<Item>();
        }

        public class Item
        {
            public string Key { get; set; } = string.Empty;

            public int Value { get; set; }

            [BsonElement]
            public bool IsEven => Value % 2 == 0;
        }
    }
}
