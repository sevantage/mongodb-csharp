using MongoDB.Driver;
using Samples.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samples.M2.C2
{
    internal class SortLimitProject : IMovieSample
    {
        public async Task RunAsync(IMongoCollection<Movie> movies, IMongoDatabase db, Configuration config)
        {
            var options = new FindOptions<Movie, Movie>()
            {
                Sort = Builders<Movie>.Sort.Descending(x => x.NumberOfComments).Ascending(x => x.Title),
                Limit = 10,
                Projection = Builders<Movie>.Projection.Include(x => x.Title).Include(x => x.NumberOfComments).Exclude(x => x.Id),
            };
            var docs = await (await movies.FindAsync(Builders<Movie>.Filter.Empty, options)).ToListAsync();
            Console.WriteLine("Top 10: movies with most comments");
            foreach (var doc in docs)
            {
                Console.WriteLine($"{doc.Title.PadRight(30)} - {doc.NumberOfComments.ToString().PadLeft(4)}");
            }
        }
    }
}
