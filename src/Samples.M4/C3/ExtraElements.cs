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

namespace Samples.M4.C3
{
    internal class ExtraElements : ITestDatabaseSample
    {
        private readonly ConsoleHelper _consoleHelper;

        public ExtraElements(ConsoleHelper consoleHelper)
        {
            _consoleHelper = consoleHelper;
        }

        public async Task RunAsync(IMongoDatabase db, Configuration config)
        {
            var bsonColl = db.GetCollection<BsonDocument>("m4c3_extraElements");
            await bsonColl.DeleteManyAsync(FilterDefinition<BsonDocument>.Empty);
            // Creating a document with extra elements
            var doc = new BsonDocument
            {
                { "t", "Text" },
                { "v", 123 },
            };
            await bsonColl.InsertOneAsync(doc);

            Console.WriteLine("Loading document without extra elements handling");
            try
            {
                var noExtraElementsHandlingColl = db.GetCollection<NoExtraElementsHandling>(bsonColl.CollectionNamespace.CollectionName);
                await (await noExtraElementsHandlingColl.FindAsync(FilterDefinition<NoExtraElementsHandling>.Empty)).SingleAsync();
            }
            catch (Exception ex)
            {
                _consoleHelper.WriteError(ex, "Error when loading document without extra elements handling");
            }

            _consoleHelper.Separator();
            Console.WriteLine("Loading document with ignore extra elements");
            var ignoreExtraElementsColl = db.GetCollection<IgnoreExtraElements>(bsonColl.CollectionNamespace.CollectionName);
            await (await ignoreExtraElementsColl.FindAsync(FilterDefinition<IgnoreExtraElements>.Empty)).SingleAsync();
            Console.WriteLine("OK");

            _consoleHelper.Separator();
            Console.WriteLine("Loading document with extra elements dictionary");
            var extraElementsDictColl = db.GetCollection<ExtraElementsDictionary>(bsonColl.CollectionNamespace.CollectionName);
            var extraElements = await (await extraElementsDictColl.FindAsync(FilterDefinition<ExtraElementsDictionary>.Empty)).SingleAsync();
            Console.WriteLine("OK - ExtraElements = " + JsonSerializer.Serialize(extraElements.ExtraElements));
        }

        private class NoExtraElementsHandling
        {
            [BsonRepresentation(BsonType.ObjectId)]
            public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
        }

        [BsonIgnoreExtraElements]
        private class IgnoreExtraElements
        {
            [BsonRepresentation(BsonType.ObjectId)]
            public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
        }

        private class ExtraElementsDictionary
        {
            [BsonRepresentation(BsonType.ObjectId)]
            public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

            [BsonExtraElements]
            public IDictionary<string, object> ExtraElements { get; set; } = new Dictionary<string, object>();
        }
    }
}
