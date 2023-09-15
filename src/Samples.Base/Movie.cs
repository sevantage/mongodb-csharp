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
    public class Movie
    {
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        [BsonElement("title")]
        public string Title { get; set; } = string.Empty;

        [BsonElement("year")]
        [BsonSerializer(typeof(YearSerializer))]
        public int Year { get; set; }

        [BsonElement("languages")]
        public ICollection<string> Languages { get; set; } = new List<string>();

        [BsonElement("genres")]
        public ICollection<string> Genres { get; set; } = new List<string>();

        [BsonElement("num_mflix_comments")]
        public int NumberOfComments { get; set; }

        [BsonElement("imdb")]
        public Imdb Imdb { get; set; } = new();
    }

    [BsonIgnoreExtraElements]
    public class Imdb
    {
        [BsonElement("rating")]
        public double Rating { get; set; }

        [BsonElement("votes")]
        public int Votes { get; set; }
    }
}
