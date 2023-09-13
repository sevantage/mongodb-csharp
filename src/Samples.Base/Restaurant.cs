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

        [BsonElement("grades")]
        public ICollection<Rating> Grades { get; set; } = new List<Rating>();
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
