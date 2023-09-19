using MongoDB.Driver;
using Samples.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samples.M2.C2
{
    internal class TextFilterCollation : IMovieSample
    {
        public async Task RunAsync(IMongoCollection<Movie> movies, IMongoDatabase db, Configuration config)
        {
            var searchText = "tHe gOdFaThEr";
            var binaryResult = await (await movies.FindAsync(x => x.Title == searchText)).ToListAsync();
            PrintResult(binaryResult, "Binary comparison");
            
            var collEn = new Collation("en");
            var enResult = await (await movies.FindAsync(x => x.Title == searchText, new FindOptions<Movie, Movie>() { Collation = collEn })).ToListAsync();
            PrintResult(enResult, "Comparison using collation EN");
            var collEnLowerStrength = new Collation("en", strength: CollationStrength.Secondary);
            var enLowerStrengthResult = await (await movies.FindAsync(x => x.Title == searchText, new FindOptions<Movie, Movie>() { Collation = collEnLowerStrength })).ToListAsync();
            PrintResult(enLowerStrengthResult, "Comparison using collation EN and strength 2");
        }

        private void PrintResult(IEnumerable<Movie> movies, string text)
        {
            var output = movies.Any() ? "a" : "no";
            Console.WriteLine($"{text} found {output} movie");
        }
    }
}
