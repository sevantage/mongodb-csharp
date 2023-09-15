using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Samples.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samples.M5.C2
{
    internal class Merge : ITestDatabaseSample
    {
        public async Task RunAsync(IMongoDatabase db, Configuration config)
        {
            var movies = db.Client.GetDatabase(config.SampleMoviesDatabaseName).GetCollection<Movie>("movies");
            var coll = db.GetCollection<Decade>("m5c2_topMovies_merge");
            await coll.DeleteManyAsync(FilterDefinition<Decade>.Empty);

            PipelineDefinition<Movie, Decade> pipeline = new EmptyPipelineDefinition<Movie>()
                .Match(Builders<Movie>.Filter.Gt("imdb.rating", 0))
                .FixYear()
                .Decade()
                .Group(x => x.Decade, grp => new Decade() { Id = grp.Key, TopMovies = grp.Select(x => new MovieRating() { Title = x.Title, Rating = x.Imdb.Rating }).ToList() })
                .SortAndSliceTopMovies()
                .Sort(Builders<Decade>.Sort.Descending(x => x.Id))
                .Merge(coll, new MergeStageOptions<Decade>());

            await movies.AggregateToCollectionAsync(pipeline);
        }
    }

    public static class MergePipelineExtensions
    {
        public static PipelineDefinition<Movie, Movie> FixYear(this PipelineDefinition<Movie, Movie> pipeline)
        {
            var stageStr = @"
                {
                    $set: {
                        year: {
                        $cond: {
                            if: { $isNumber: ""$year"" },
                            then: ""$year"",
                            else: {
                            $toInt: {
                                $substr: [""$year"", 0, 4],
                            },
                            },
                        },
                        },
                    },
                }
            ";
            var stage = BsonDocument.Parse(stageStr);
            return pipeline
                .AppendStage<Movie, Movie, Movie>(stage);
        }

        public static PipelineDefinition<Movie, MovieWithDecade> Decade(this PipelineDefinition<Movie, Movie> pipeline)
        {
            var stageStr = @"
                {
                    $set: {
                        decade: {
                        $multiply: [
                            {
                            $toInt: {
                                $divide: [""$year"", 10],
                            },
                            },
                            10,
                        ],
                        },
                    },
                }
            ";
            var stage = BsonDocument.Parse(stageStr);
            return pipeline
                .AppendStage<Movie, Movie, MovieWithDecade>(stage);
        }

        public static PipelineDefinition<Movie, Decade> SortAndSliceTopMovies(this PipelineDefinition<Movie, Decade> pipeline)
        {
            var stageStr = @"
                {
                    $set: {
                      top_movies: {
                        $slice: [
                          {
                            $sortArray: {
                              input: ""$top_movies"",
                              sortBy: {
                                rating: -1,
                                title: 1,
                              },
                            },
                          },
                          10,
                        ],
                      },
                    },
                }";
            var stage = BsonDocument.Parse(stageStr);
            return pipeline
                .AppendStage<Movie, Decade, Decade>(stage);
        }
    }

    public class MovieWithDecade : Movie
    {
        [BsonElement("decade")]
        public int Decade { get; set; }
    }

    public class Decade
    {
        [BsonElement("_id")]
        public int Id { get; set; }

        [BsonElement("top_movies")]
        public ICollection<MovieRating> TopMovies { get; set; } = new List<MovieRating>();
    }

    public class MovieRating
    {
        [BsonElement("title")]
        public string Title { get; set; } = string.Empty;

        [BsonElement("rating")]
        public double Rating { get; set; }
    }
}
