using MongoDB.Driver;
using Samples.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samples.M4.C2
{
    internal class TimeSeries : ITestDatabaseSample
    {
        private const string CollectionName = "m4c2_timeSeries";

        private readonly ConsoleHelper _consoleHelper;

        public TimeSeries(ConsoleHelper consoleHelper)
        {
            _consoleHelper = consoleHelper;
        }

        public async Task RunAsync(IMongoDatabase db, Configuration config)
        {
            Console.WriteLine("Dropping ts collection");
            await db.DropCollectionAsync(CollectionName);
            var options = new CreateCollectionOptions()
            {
                TimeSeriesOptions = new TimeSeriesOptions(nameof(Measurement.Timestamp), nameof(Measurement.Meta), TimeSeriesGranularity.Minutes),
            };
            await db.CreateCollectionAsync(CollectionName, options);
            var coll = db.GetCollection<Measurement>(CollectionName);

            _consoleHelper.Separator();
            Console.WriteLine("Inserting 100k measurements");
            var start = new DateTime(2023, 05, 04, 01, 02, 03, DateTimeKind.Utc);
            var meta = new Metadata[]
            {
                new() { Sensor = "Outside_A", Type = TemperatureType.Outside, },
                new() { Sensor = "Outside_B", Type = TemperatureType.Outside, },
                new() { Sensor = "Inside_A", Type = TemperatureType.Inside, },
                new() { Sensor = "Inside_B", Type = TemperatureType.Inside, },
                new() { Sensor = "Inside_C", Type = TemperatureType.Inside, },
                new() { Sensor = "Ambient_A", Type = TemperatureType.Ambient, },
                new() { Sensor = "Ambient_A", Type = TemperatureType.Ambient, },
            };
            var rnd = new Random();
            var measurements = Enumerable.Range(0, 100000).Select(x => new Measurement()
            {
                Timestamp = start.AddMinutes(x / 10),
                Meta = meta[x % meta.Length],
                Value = rnd.Next(1500, 3500) / 100f,
            });
            await coll.InsertManyAsync(measurements, new InsertManyOptions() { IsOrdered = false });

            _consoleHelper.Separator();
            Console.WriteLine("Getting top 10 values");
            FindOptions<Measurement, Measurement> findOptions = new FindOptions<Measurement, Measurement>()
            {
                Limit = 10,
                Sort = Builders<Measurement>.Sort.Descending(x => x.Value),
            };
            var top10Measurements = await (await coll.FindAsync(FilterDefinition<Measurement>.Empty, findOptions)).ToListAsync();
            Console.WriteLine(string.Join(
                "\t",
                "Sensor",
                "Type",
                "Timestamp",
                "Value"));
            foreach (var m in top10Measurements)
            {
                Console.WriteLine(string.Join(
                    "\t",
                    m.Meta.Sensor,
                    m.Meta.Type,
                    m.Timestamp,
                    m.Value.ToString("F2")));
            }
        }

        public class Measurement
        {
            public Metadata Meta { get; set; } = new Metadata();

            public DateTime Timestamp { get; set; } = DateTime.UtcNow;

            public float Value { get; set; }
        }

        public enum TemperatureType
        {
            Inside,
            Outside,
            Ambient,
        }

        public class Metadata
        {
            public string Sensor { get; set; } = string.Empty;

            public TemperatureType Type { get; set; }
        }
    }
}
