using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson;
using MongoDB.Driver;
using Samples.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Samples.Base.Serialization;

namespace Samples.M5.C2
{
    internal class FluentAggregations : IMovieSample
    {
        public async Task RunAsync(IMongoCollection<Movie> movies, IMongoDatabase db, Configuration config)
        {
            var topTenGenres = await movies.Aggregate()
                .Match(x => x.Year == 1972)
                .Unwind<Movie, MovieSingleGenre>(x => x.Genres)
                .Group(x => x.Genres, g => new { Genre = g.Key, Count = g.Count() })
                .SortByDescending(x => x.Count)
                .Limit(10)
                .ToListAsync();
            Console.WriteLine("Top ten genres of 1972:");
            foreach (var g in topTenGenres)
            {
                Console.WriteLine(string.Join(
                    "\t",
                    g.Genre.PadRight(15),
                    g.Count));
            }
        }

        public class MovieSingleGenre
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
            public string Genres { get; set; } = string.Empty;

            [BsonElement("num_mflix_comments")]
            public int NumberOfComments { get; set; }
        }
    }
}
