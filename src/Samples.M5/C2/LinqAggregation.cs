using MongoDB.Driver;
using Samples.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samples.M5.C2
{
    internal class LinqAggregation : IMovieSample
    {
        public Task RunAsync(IMongoCollection<Movie> movies, IMongoDatabase db, Configuration config)
        {
            var topTenGenres = movies
                .AsQueryable()
                .Where(x => x.Year == 1972)
                .SelectMany(x => x.Genres)
                .GroupBy(x => x)
                .Select(x => new { Genre = x.Key, Count = x.Count() })
                .OrderByDescending(x => x.Count)
                .Take(10)
                .ToArray();

            Console.WriteLine("Top ten genres of 1972:");
            foreach (var g in topTenGenres)
            {
                Console.WriteLine(string.Join(
                    "\t",
                    g.Genre.PadRight(15),
                    g.Count));
            }

            return Task.CompletedTask;
        }
    }
}
