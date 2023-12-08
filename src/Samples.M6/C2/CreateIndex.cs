using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Samples.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samples.M6.C2
{
    public class CreateIndex : ITestDatabaseSample
    {
        public async Task RunAsync(IMongoDatabase db, Configuration config)
        {
            var coll = db.GetCollection<TestDocument>("m6_c2_create_index");
            await coll.Indexes.DropOneAsync("Title_1_Tags_1");
            var model = new CreateIndexModel<TestDocument>(
                Builders<TestDocument>
                    .IndexKeys
                    .Ascending(x => x.Title).Ascending(x => x.Tags));
            await coll.Indexes.CreateOneAsync(model);
        }

        public class TestDocument
        {
            [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
            public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

            public string Title { get; set; } = string.Empty;

            public ICollection<string> Tags { get; set; } = new HashSet<string>();
        }
    }
}
