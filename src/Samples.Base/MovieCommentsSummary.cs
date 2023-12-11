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
    public class MovieCommentsSummary
    {
        [BsonElement("title")]
        public string Title { get; set; } = string.Empty;

        [BsonElement("num_mflix_comments")]
        public int NumberOfComments { get; set; }

        public override string ToString()
        {
            return Title;
        }
    }
}
