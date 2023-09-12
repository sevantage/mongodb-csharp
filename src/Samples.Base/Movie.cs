using MongoDB.Bson.Serialization.Attributes;
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
        [BsonIgnoreIfDefault]
        [BsonDefaultValue("")]
        public string Id { get; set; } = string.Empty;

        [BsonElement("title")]
        public string Title { get; set; } = string.Empty;

        [BsonElement("year")]
        public int Year { get; set; }
    }
}
