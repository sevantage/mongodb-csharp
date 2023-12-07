using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using Samples.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samples.M4.C3
{
    public class Conventions : ITestDatabaseSample
    {
        public async Task RunAsync(IMongoDatabase db, Configuration config)
        {
            var pack = new ConventionPack();
            pack.Add(new CamelCaseElementNameConvention());
            ConventionRegistry.Register(
                "Camel Case Convention",
                pack,
                t => true);

            var coll = db.GetCollection<TestDocument>("m4c3_conventions");
            await coll.DeleteManyAsync(FilterDefinition<TestDocument>.Empty);

            await coll.InsertOneAsync(new TestDocument() { Name = "My name", Value = 123 });
        }

        [BsonNoId]
        public class TestDocument
        {
            public string Name { get; set; } = string.Empty;

            public int Value { get; set; }
        }
    }
}
