using MongoDB.Driver;
using Samples.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samples.M2.C2
{
    public class TextUmlautCollation : ITestDatabaseSample
    {
        public async Task RunAsync(IMongoDatabase db, Configuration config)
        {
            var coll = db.GetCollection<TestDocument>("m2c2_test_umlaut_collation");
            await GenerateDataAsync(coll);
            var collations = new Collation[]
            {
                Collation.Simple,
                new Collation("en"),
                new Collation("en", strength: CollationStrength.Secondary),
                new Collation("en", strength: CollationStrength.Primary),
                new Collation("de"),
                new Collation("de", strength: CollationStrength.Secondary),
                new Collation("de", strength: CollationStrength.Primary),
            };
            foreach (var collation in collations)
            {
                Console.WriteLine($"Searching for A using collation {collation.Locale} with strength {collation.Strength}:");
                var result = (await coll.FindAsync(
                    x => x.Id == "A",
                    new FindOptions<TestDocument, TestDocument>() { Collation = collation }))
                    .ToEnumerable();
                foreach( var doc in result)
                {
                    Console.WriteLine(doc.Id);
                }
            }
        }

        private async Task GenerateDataAsync(IMongoCollection<TestDocument> coll)
        {
            await coll.DeleteManyAsync(FilterDefinition<TestDocument>.Empty);
            var docs = new string[] { "A", "Ä", "a", "ä" }
                .Select(x => x.ToString()).Select(x => new TestDocument() { Id = x });
            await coll.InsertManyAsync(docs);
        }

        public class TestDocument
        {
            public string Id { get; set; } = string.Empty;
        }
    }
}
