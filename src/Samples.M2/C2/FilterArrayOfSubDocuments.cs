using MongoDB.Driver;
using Samples.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samples.M2.C2
{
    internal class FilterArrayOfSubDocuments : IRestaurantsSample
    {
        public async Task RunAsync(IMongoCollection<Restaurant> restaurants, IMongoDatabase db, Configuration config)
        {
            var fltrBldr = Builders<Restaurant>.Filter;

            // { "grades.score": { $gte: 10 } }
            var havingScoreGte10 = await (await restaurants.FindAsync(fltrBldr.Gte("grades.score", 10))).ToListAsync();
            Console.WriteLine($"Found {havingScoreGte10.Count()} restaurants having a rating with a score >= 10");

            // { "grades.score": { $gte: 10 }, "grades.grade": "A" }
            var havingScoreGte10AndGradeA = await (await restaurants.FindAsync(fltrBldr.Gte("grades.score", 10) & fltrBldr.Eq("grades.grade", "A"))).ToListAsync();
            Console.WriteLine($"Found {havingScoreGte10AndGradeA.Count()} restaurants having a rating with a score >= 10 and (maybe another one) with grade A");

            // { "grades": { "$elemMatch": { "score": { "$gte": 10 }, "grade": "A" } } }
            var sameRatingWithScoreGte10AndGradeA = await (await restaurants.FindAsync(fltrBldr.ElemMatch(x => x.Grades, x => x.Score >= 10 && x.Grade == "A"))).ToListAsync();
            Console.WriteLine($"Found {sameRatingWithScoreGte10AndGradeA.Count()} restaurants having a rating with a score >= 10 and grade == A at the same time");
        }
    }
}
