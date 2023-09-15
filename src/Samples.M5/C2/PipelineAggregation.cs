using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MongoDB.Driver;
using Samples.Base;
using Samples.Base.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samples.M5.C2
{
    internal class PipelineAggregation : IMovieSample
    {
        public async Task RunAsync(IMongoCollection<Movie> movies, IMongoDatabase db, Configuration config)
        {
            var pipeline = new EmptyPipelineDefinition<Movie>()
                .Match(x => x.Year == 1972)
                .Unwind<Movie, Movie, MovieSingleGenre>(x => x.Genres)
                .Group(x => x.Genres, g => new MoviesByGenre() { Genre = g.Key, Count = g.Count() })
                .Sort(Builders<MoviesByGenre>.Sort.Descending(x => x.Count))
                .Limit(10);
            var topTenGenres = await (await movies.AggregateAsync(pipeline)).ToListAsync();
            Console.WriteLine("Top ten genres of 1972:");
            foreach (var g in topTenGenres)
            {
                Console.WriteLine(string.Join(
                    "\t",
                    g.Genre.PadRight(15),
                    g.Count));
            }
        }

        public class MoviesByGenre
        {
            public string Genre { get; set; } = string.Empty;

            public  int Count { get; set; }
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
