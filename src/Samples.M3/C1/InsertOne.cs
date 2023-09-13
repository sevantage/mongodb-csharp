using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Samples.Base;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Samples.M3.C1
{
    internal class InsertOne : ITestDatabaseSample
    {
        private const string CollectionName = "m3c1_insertOne";

        public async Task RunAsync(IMongoDatabase db, Configuration config)
        {
            var coll = db.GetCollection<TestDocument>(CollectionName);
            await coll.DeleteManyAsync(Builders<TestDocument>.Filter.Empty);

            await InsertOneAsync(db, new TestDocument(), "Inserting document with Id == null");
            await InsertOneAsync(db, new TestDocument(), "Inserting another document with Id == null");

            await InsertOneAsync(db, new TestDocumentWithStringId(), "Inserting document with string Id == null");
            await InsertOneAsync(db, new TestDocumentWithStringId(), "Inserting another document with string Id == null");

            TestDocumentWithId doc = new TestDocumentWithId();
            await InsertOneAsync(db, doc, "Inserting document with Id value assigned in code");
            await InsertOneAsync(db, doc, "Inserting document with same Id value assigned in code");
        }

        private async Task InsertOneAsync<T>(IMongoDatabase db, T doc, string msg)
            where T : ITestDocument
        {
            Console.WriteLine(new string('-', 50));
            var coll = db.GetCollection<T>(CollectionName);
            Console.WriteLine(msg);
            Console.WriteLine($"Id before insert: {doc.Id}");
            try
            {
                await coll.InsertOneAsync(doc);
                Console.WriteLine($"Successfully inserted document with Id {doc.Id}");
            }
            catch (Exception ex)
            {
                var color = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ForegroundColor = color;
            }
        }

        public interface ITestDocument
        {
            string? Id { get; }

            string Type { get; }
        }

        public class TestDocument : ITestDocument
        {
            [BsonRepresentation(BsonType.ObjectId)]
            public string Id { get; set; } = string.Empty;

            [BsonElement]
            public string Type => GetType().Name;
        }

        public class TestDocumentWithId : ITestDocument
        {
            [BsonRepresentation(BsonType.ObjectId)]
            public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

            [BsonElement]
            public string Type => GetType().Name;
        }

        public class TestDocumentWithStringId : ITestDocument
        {
            public string? Id { get; set; }

            [BsonElement]
            public string Type => GetType().Name;
        }
    }
}
