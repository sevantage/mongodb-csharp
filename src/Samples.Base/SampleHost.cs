using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using ZstdSharp.Unsafe;

namespace Samples.Base
{
    public class SampleHost
    {
        private readonly Configuration _config;
        private readonly ISample[] _samples;
        private readonly Func<Configuration, IMongoClient> _clientBldr;
        private readonly ConsoleHelper _consoleHelper;

        static SampleHost()
        {
            BsonSerializer.RegisterSerializer(typeof(Guid), new GuidSerializer(GuidRepresentation.Standard));
#pragma warning disable CS0618 // Type or member is obsolete
            BsonDefaults.GuidRepresentationMode = GuidRepresentationMode.V3;
#pragma warning restore CS0618 // Type or member is obsolete
        }

        public SampleHost(
            IOptions<Configuration> configOptions,
            IEnumerable<ISample> samples,
            Func<Configuration, IMongoClient> clientBldr,
            ConsoleHelper consoleHelper)
        {
            _config = configOptions.Value;
            _samples = samples.OrderBy(x => x.GetType().FullName).ToArray();
            _clientBldr = clientBldr;
            _consoleHelper = consoleHelper;
        }

        public Task RunAsync()
        {
            var sampleToRun = GetSampleToRun();
            Console.WriteLine($"Running sample {sampleToRun.GetType().FullName}");
            _consoleHelper.Separator();
            var client = _clientBldr(_config);

            switch (sampleToRun)
            {
                case IMovieSample movieSample:
                    return RunSampleAsync(client, movieSample);
                case IRestaurantsSample restaurantsSample:
                    return RunSampleAsync(client, restaurantsSample);
                case ITestDatabaseSample testSample:
                    return RunSampleAsync(client, testSample);
                case ISampleWithClient clientSample:
                    return RunSampleAsync(client, clientSample);
                case ISampleWithConfig configSample:
                    return RunSampleAsync(configSample);
            }
            throw new InvalidOperationException($"{sampleToRun.GetType().FullName} does not implement ÎSampleWithConfig, ISampleWithClient, IMovieSample or IRestaurantsSample.");
        }

        private Task RunSampleAsync(IMongoClient client, IMovieSample movieSample)
        {
            var db = client.GetDatabase(_config.SampleMoviesDatabaseName);
            var coll = db.GetCollection<Movie>("movies");
            return movieSample.RunAsync(coll, db, _config);
        }

        private Task RunSampleAsync(IMongoClient client, IRestaurantsSample restaurantsSample)
        {
            var db = client.GetDatabase(_config.SampleRestaurantsDatabaseName);
            var coll = db.GetCollection<Restaurant>("restaurants");
            return restaurantsSample.RunAsync(coll, db, _config);
        }

        private Task RunSampleAsync(IMongoClient client, ITestDatabaseSample testSample)
        {
            var db = client.GetDatabase(_config.TestDatabaseName);
            return testSample.RunAsync(db, _config);
        }

        private Task RunSampleAsync(ISampleWithConfig configSample)
        {
            return configSample.RunAsync(_config);
        }

        private Task RunSampleAsync(IMongoClient client, ISampleWithClient clientSample)
        {
            return clientSample.RunAsync(client, _config);
        }

        private ISample GetSampleToRun()
        {
            if (!string.IsNullOrWhiteSpace(_config.Sample))
            {
                var samples = _samples.Where(x => x.GetType().FullName!.ToLowerInvariant().EndsWith(_config.Sample.ToLowerInvariant()));
                if (!samples.Any())
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
                {
                    Console.WriteLine();
                    return _samples[noOfSample - 1];
                }
            }
        }
    }
}