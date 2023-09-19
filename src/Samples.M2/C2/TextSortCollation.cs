using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Samples.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samples.M2.C2
{
    internal class TextSortCollation : ITestDatabaseSample
    {
        private readonly ConsoleHelper _consoleHelper;

        public TextSortCollation(ConsoleHelper consoleHelper)
        {
            _consoleHelper = consoleHelper;
        }

        public async Task RunAsync(IMongoDatabase db, Configuration config)
        {
            var coll = db.GetCollection<TestDocument>("m2c2_text_sort_collation");
            await PrepareDataAsync(coll);

            Console.WriteLine("WITHOUT COLLATION");
            await LoadAndPrintDocsAsync(coll, Collation.Simple);
            _consoleHelper.Separator();

            Console.WriteLine("WITH COLLATION");
            var collation = new Collation("en", numericOrdering: true);
            await LoadAndPrintDocsAsync(coll, collation);
        }

        private async Task LoadAndPrintDocsAsync(IMongoCollection<TestDocument> coll, Collation collation)
        {
            var sort = Builders<TestDocument>.Sort.Ascending(x => x.Version);
            var options = new FindOptions<TestDocument, TestDocument>()
            {
                Sort = sort,
                Collation = collation
            };
            var docs = (await coll.FindAsync(Builders<TestDocument>.Filter.Empty, options)).ToEnumerable();
            foreach (var doc in docs)
            {
                Console.WriteLine(doc.Version);
            }
        }

        private async Task PrepareDataAsync(IMongoCollection<TestDocument> coll)
        {
            await coll.DeleteManyAsync(Builders<TestDocument>.Filter.Empty);
            var docs = Enumerable.Range(1, 10).SelectMany(x => new TestDocument[]
            {
                new() { Version = $"{x}.{x % 2}.{x % 4}"},
                new() { Version = $"{x}.{x % 3}.{x % 6}"},
                new() { Version = $"{x}.{x % 4}.{x % 8}"},
                new() { Version = $"{x * 2}.{(x * 2) % 2}.{(x * 2) % 4}"},
                new() { Version = $"{x * 2}.{(x * 2) % 3}.{(x * 2) % 6}"},
                new() { Version = $"{x * 2}.{(x * 2) % 4}.{(x * 2) % 8}"},
                new() { Version = $"{x * 100}.{(x * 100) % 2}.{(x * 100) % 4}"},
                new() { Version = $"{x * 100}.{(x * 100) % 3}.{(x * 100) % 6}"},
                new() { Version = $"{x * 100}.{(x * 100) % 4}.{(x * 100) % 8}"},
            });
            await coll.InsertManyAsync(docs);
        }

        public class TestDocument
        {
            [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
            public string Id { get; set; } = string.Empty;

            public string Version { get; set; } = string.Empty;
        }
    }
}
