using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Samples.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samples.M4.C3
{
    public class GuidSerialization : ITestDatabaseSample
    {
        public async Task RunAsync(IMongoDatabase db, Configuration config)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            Console.WriteLine($"Current Guid representation mode = {BsonDefaults.GuidRepresentationMode}");
#pragma warning restore CS0618 // Type or member is obsolete
            var coll = db.GetCollection<GuidDocument>("m4c3_guids");
            await coll.DeleteManyAsync(FilterDefinition<GuidDocument>.Empty);
            var doc = new GuidDocument();
            await coll.InsertOneAsync(doc);

            // Read as BSON document to get subtype
            var bsonColl = db.GetCollection<BsonDocument>(coll.CollectionNamespace.CollectionName);
            var bsonDoc = await (await bsonColl.FindAsync(FilterDefinition<BsonDocument>.Empty, new FindOptions<BsonDocument, BsonDocument>() { Limit = 1 })).FirstOrDefaultAsync();
            var guidProp = bsonDoc["Guid"].AsBsonBinaryData;
            Console.WriteLine($"Guid stored as binary subtype {guidProp.SubType}");
        }

        public class GuidDocument
        {
            [BsonRepresentation(BsonType.ObjectId)]
            public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

            public Guid Guid { get; set; } = Guid.NewGuid();
        }
    }
}
