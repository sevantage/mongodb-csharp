using MongoDB.Driver;
using Samples.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samples.M2.C2
{
    public class CursorHandling : IMovieSample
    {
        private readonly ConsoleHelper _consoleHelper;

        public CursorHandling(ConsoleHelper consoleHelper)
        {
            _consoleHelper = consoleHelper;
        }

        public async Task RunAsync(IMongoCollection<Movie> movies, IMongoDatabase db, Configuration config)
        {
            // ToEnumerable
            var enumerable = (await movies.FindAsync(FilterDefinition<Movie>.Empty, new FindOptions<Movie, Movie>() { Limit = 10 })).ToEnumerable();
            PrintMovies(enumerable);

            // ToListAsync
            var list = await (await movies.FindAsync(FilterDefinition<Movie>.Empty, new FindOptions<Movie, Movie>() { Limit = 100 })).ToListAsync();
            _consoleHelper.Separator();
            Console.WriteLine($"List contains {list.Count} movies");
        }

        private static void PrintMovies(IEnumerable<Movie> movies)
        {
            foreach (var movie in movies)
            {
                Console.WriteLine(movie.Title);
            }
        }
    }
}
