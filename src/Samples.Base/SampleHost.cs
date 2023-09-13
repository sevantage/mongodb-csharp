using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using ZstdSharp.Unsafe;

namespace Samples.Base
{
    public class SampleHost
    {
        private readonly Configuration _config;
        private readonly ISample[] _samples;
        private readonly Func<Configuration, IMongoClient> _clientBldr;

        static SampleHost()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            BsonDefaults.GuidRepresentationMode = GuidRepresentationMode.V3;
#pragma warning restore CS0618 // Type or member is obsolete
        }

        public SampleHost(
            IOptions<Configuration> configOptions,
            IEnumerable<ISample> samples,
            Func<Configuration, IMongoClient> clientBldr)
        {
            _config = configOptions.Value;
            _samples = samples.OrderBy(x => x.GetType().FullName).ToArray();
            _clientBldr = clientBldr;
        }

        public async Task RunAsync()
        {
            var sampleToRun = GetSampleToRun();
            Console.WriteLine($"Running sample {sampleToRun.GetType().FullName}");
            Console.WriteLine(new string('-', 50));
            var client = _clientBldr(_config);
            var sampleWithMoviesDb = sampleToRun as IMovieSample;
            if (sampleWithMoviesDb != null)
            {
                var db = client.GetDatabase(_config.SampleMoviesDatabaseName);
                var coll = db.GetCollection<Movie>("movies");
                await sampleWithMoviesDb.RunAsync(coll, db, _config);
                return;
            }
            var sampleWithRestaurantsDb = sampleToRun as IRestaurantsSample;
            if (sampleWithRestaurantsDb != null)
            {
                var db = client.GetDatabase(_config.SampleRestaurantsDatabaseName);
                var coll = db.GetCollection<Restaurant>("restaurants");
                await sampleWithRestaurantsDb.RunAsync(coll, db, _config);
                return;
            }
            var sampleWithClient = sampleToRun as ISampleWithClient;
            if (sampleWithClient != null)
            {
                await sampleWithClient.RunAsync(client, _config);
                return;
            }
            throw new InvalidOperationException($"{sampleToRun.GetType().FullName} does not implement ISampleWithClient, IMovieSample or IRestaurantsSample.");
        }

        private ISample GetSampleToRun()
        {
            if (!string.IsNullOrWhiteSpace(_config.Sample))
            {
                var samples = _samples.Where(x => x.GetType().FullName!.ToLowerInvariant().EndsWith(_config.Sample.ToLowerInvariant()));
                if (samples.Any())
                    throw new InvalidOperationException($"Sample {_config.Sample} not found");
                else if (samples.Count() > 1)
                    throw new InvalidOperationException($"Encountered several samples for name {_config.Sample}");
                return samples.First();
            }
            Console.WriteLine("Please select a sample: ");
            for (var i = 1; i < _samples.Length + 1; i++)
                Console.WriteLine($"{i} - {_samples[i - 1].GetType().FullName}");

            while (true)
            {
                Console.Write("Please enter number of sample: ");
                var input = Console.ReadLine();
                if (int.TryParse(input, out var noOfSample) && noOfSample >= 1 && noOfSample <= _samples.Length)
                    return _samples[noOfSample - 1];
            }
        }
    }
}