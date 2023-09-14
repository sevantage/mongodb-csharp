using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Samples.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Samples.M4.C3.DeclarativeClassMap;

namespace Samples.M4.C3
{
    internal class ImperativeClassMap : ITestDatabaseSample
    {
        public async Task RunAsync(IMongoDatabase db, Configuration config)
        {
            BsonClassMap.RegisterClassMap<MyDocument>(cm => 
            { 
                cm.AutoMap(); 
                cm.MapIdMember(c => c.Key); 
                cm.MapMember(c => c.Name).SetElementName("n"); 
                cm.UnmapMember(c => c.DoNotStore);
                cm.MapMember(c => c.Value).SetIgnoreIfDefault(true).SetDefaultValue(123); });

            var coll = db.GetCollection<MyDocument>("m4c3_imperativeClassMap");
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
            public string Key { get; set; } = ObjectId.GenerateNewId().ToString();

            public string Name { get; set; } = string.Empty;

            public string DoNotStore { get; set; } = string.Empty;

            public int Value { get; set; } = 123;
        }
    }
}
