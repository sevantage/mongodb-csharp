using MongoDB.Driver;
using Samples.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samples.M2.C2
{
    internal class FilterArray : IMovieSample
    {
        public async Task RunAsync(IMongoCollection<Movie> movies, IMongoDatabase db, Configuration config)
        {
            var fltrBldr = Builders<Movie>.Filter;

            // { languages: "German" }
            var moviesInGerman = await (await movies.FindAsync(fltrBldr.AnyEq(x => x.Languages, "German"))).ToListAsync();
            Console.WriteLine($"Found {moviesInGerman.Count} in German");

            var moviesInGermanLambda = await (await movies.FindAsync(x => x.Languages.Contains("German"))).ToListAsync();
            Console.WriteLine($"Found {moviesInGermanLambda.Count} in German (LAMBDA)");

            // { languages: [ "English", "German" ] }
            var moviesInEnglishAndGerman = await (await movies.FindAsync(x => x.Languages == new List<string>() { "English", "German" })).ToListAsync();
            Console.WriteLine($"Found {moviesInEnglishAndGerman.Count} in English and German");

            // { languages: [ "German", "English" ] }
            var moviesInGermanAndEnglish = await (await movies.FindAsync(x => x.Languages == new List<string>() { "German", "English" })).ToListAsync();
            Console.WriteLine($"Found {moviesInGermanAndEnglish.Count} in German and English");

            // { languages: { $all: [ "German", "English" ] } }
            var moviesContainingBothEnglishAndGerman = await (await movies.FindAsync(fltrBldr.All(x => x.Languages, new string[] { "German", "English" }))).ToListAsync();
            Console.WriteLine($"Found {moviesContainingBothEnglishAndGerman.Count} having both an English and a German translation");
        }
    }
}
