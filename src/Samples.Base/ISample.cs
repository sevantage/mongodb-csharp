using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samples.Base
{
    public interface ISample
    {
    }

    public interface ISampleWithClient : ISample
    {
        Task RunAsync(IMongoClient client, Configuration config);
    }

    public interface IMovieSample : ISample
    {
        Task RunAsync(IMongoCollection<Movie> movies, IMongoDatabase db, Configuration config);
    }

    public interface IRestaurantsSample : ISample
    {
        Task RunAsync(IMongoCollection<Restaurant> restaurants, IMongoDatabase db, Configuration config);
    }
}
