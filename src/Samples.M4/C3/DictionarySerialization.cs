using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Driver;
using Samples.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samples.M4.C3
{
    internal class DictionarySerialization : ITestDatabaseSample
    {
        public async Task RunAsync(IMongoDatabase db, Configuration config)
        {
            var coll = db.GetCollection<TestDocument>("m4c3_dictionarySerialization");
            await coll.DeleteManyAsync(FilterDefinition<TestDocument>.Empty);

            var dict = new Dictionary<string, object>()
            {
                { "A", 123 },
                { "B", DateTime.UtcNow },
                { "C", "Text" },
            };
            var doc = new TestDocument()
            {
                ArrayOfDocs = dict,
                ArrayOfArrays = dict,
                Document = dict,
            };
            await coll.InsertOneAsync(doc);

            var bsonColl = db.GetCollection<BsonDocument>(coll.CollectionNamespace.CollectionName);
            var bsonDoc = await (await bsonColl.FindAsync(FilterDefinition<BsonDocument>.Empty)).FirstOrDefaultAsync();
            Console.WriteLine("Array of documents:");
            Console.WriteLine(bsonDoc["ArrayOfDocs"]);
            Console.WriteLine("Array of arrays:");
            Console.WriteLine(bsonDoc["ArrayOfArrays"]);
            Console.WriteLine("Document:");
            Console.WriteLine(bsonDoc["Document"]);
        }

        public class TestDocument
        {
            [BsonRepresentation(BsonType.ObjectId)]
            public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

            [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfDocuments)] 
            public IDictionary<string, object> ArrayOfDocs { get; set;} = new Dictionary<string, object>();

            [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)] 
            public IDictionary<string, object> ArrayOfArrays { get; set;} = new Dictionary<string, object>();

            [BsonDictionaryOptions(DictionaryRepresentation.Document)] 
            public IDictionary<string, object> Document { get; set;} = new Dictionary<string, object>();
        }
    }
}
