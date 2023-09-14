using MongoDB.Bson;
using MongoDB.Bson.Serialization;
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
    internal class CustomSerializer : ITestDatabaseSample
    {
        public async Task RunAsync(IMongoDatabase db, Configuration config)
        {
            var coll = db.GetCollection<TestDocument>("m4c3_customSerializer");
            await coll.DeleteManyAsync(FilterDefinition<TestDocument>.Empty);

            Console.WriteLine("Inserting document with custom serializer");
            var doc = new TestDocument();
            await coll.InsertOneAsync(doc);

            Console.WriteLine("Loading document as BSON");
            var bsonColl = db.GetCollection<BsonDocument>(coll.CollectionNamespace.CollectionName);
            var bsonDoc = await (await bsonColl.FindAsync(FilterDefinition<BsonDocument>.Empty)).FirstAsync();
            Console.WriteLine(bsonDoc.ToString());

            Console.WriteLine("Loading document with custom serializer");
            var loadedDoc = await (await coll.FindAsync(FilterDefinition<TestDocument>.Empty)).FirstAsync();
            Console.WriteLine(JsonSerializer.Serialize(loadedDoc));
        }

        public class TestDocument
        {
            public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

            public CustomValue CustomValue { get; set; } = new CustomValue(123);
        }

        public class CustomValueSerializer : IBsonSerializer
        {
            public Type ValueType => typeof(CustomValue);

            public object Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
            {
                var value = context.Reader.ReadInt32();
                return new CustomValue(value);
            }

            public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
            {
                context.Writer.WriteInt32(((CustomValue)value).Value);
            }
        }

        [BsonSerializer(typeof(CustomValueSerializer))]
        public class CustomValue
        {
            public CustomValue(int value)
            {
                Value = value;
            }

            public int Value { get; }
        }
    }
}
