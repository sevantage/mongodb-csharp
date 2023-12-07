using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Samples.Base.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samples.Base
{
    [BsonIgnoreExtraElements]
    public class Restaurant
    {
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("borough")]
        public string Borough { get; set; } = string.Empty;

        [BsonElement("cuisine")]
        public string Cuisine { get; set; } = string.Empty;

        [BsonElement("grades")]
        public ICollection<Rating> Grades { get; set; } = new List<Rating>();

        public override string ToString()
        {
            return Name;
        }
    }

    [BsonIgnoreExtraElements]
    public class Rating
    {
        [BsonElement("date")]
        public DateTime Date { get; set; }

        [BsonElement("grade")]
        public string Grade { get; set; } = string.Empty;

        [BsonElement("score")]
        public int Score { get; set; }
    }
}
