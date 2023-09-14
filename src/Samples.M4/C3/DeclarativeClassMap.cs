using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MongoDB.Driver;
using Samples.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samples.M4.C3
{
    internal class DeclarativeClassMap : ITestDatabaseSample
    {
        public async Task RunAsync(IMongoDatabase db, Configuration config)
        {
            var coll = db.GetCollection<MyDocument>("m4c3_declarativeClassMap");
            await coll.DeleteManyAsync(FilterDefinition<MyDocument>.Empty);

            var docDefault = new MyDocument() { Name = "Default value" };
            await coll.InsertOneAsync(docDefault);
            var docSpecial = new MyDocument() { Name = "Special value", Value = 246 };
            await coll.InsertOneAsync(docSpecial);

            var bsonColl = db.GetCollection<BsonDocument>(coll.CollectionNamespace.CollectionName);
            var bsonDefault = await (await bsonColl.FindAsync(Builders<BsonDocument>.Filter.Eq("n", "Default value"))).SingleAsync();
            Console.WriteLine("BSON document with default value:");
            Console.WriteLine(bsonDefault.ToString());
            var bsonSpecial = await (await bsonColl.FindAsync(Builders<BsonDocument>.Filter.Eq("n", "Special value"))).SingleAsync();
            Console.WriteLine("BSON document with special value:");
            Console.WriteLine(bsonSpecial.ToString());
        }

        public class MyDocument
        {
            [BsonId]
            [BsonRepresentation(BsonType.ObjectId)]
            public string Key { get; set; } = ObjectId.GenerateNewId().ToString();

            [BsonElement("n")]
            public string Name { get; set; } = string.Empty;

            [BsonIgnore]
            public string DoNotStore { get; set; } = string.Empty;

            [BsonIgnoreIfDefault]
            [BsonDefaultValue(123)]
            public int Value { get; set; } = 123;
        }
    }
}
