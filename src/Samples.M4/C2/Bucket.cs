using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Samples.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samples.M4.C2
{
    internal class Bucket : ITestDatabaseSample
    {
        public async Task RunAsync(IMongoDatabase db, Configuration config)
        {
            var coll = db.GetCollection<MeasurementsPerHour>("m4c2_bucket");
            await coll.DeleteManyAsync(FilterDefinition<MeasurementsPerHour>.Empty);

            var sensor = "My sensor";
            var measurements = new Measurement[]
            {
                new() { Timestamp = new DateTime(2023, 06, 07, 12, 34, 56, DateTimeKind.Utc), Value = 10 },
                new() { Timestamp = new DateTime(2023, 06, 07, 12, 36, 56, DateTimeKind.Utc), Value = 7 },
                new() { Timestamp = new DateTime(2023, 06, 07, 12, 36, 57, DateTimeKind.Utc), Value = 12 },
                new() { Timestamp = new DateTime(2023, 06, 07, 13, 34, 56, DateTimeKind.Utc), Value = 8 },
            };

            foreach (var m in measurements)
            {
                var startTime = new DateTime(
                    m.Timestamp.Year,
                    m.Timestamp.Month,
                    m.Timestamp.Day,
                    m.Timestamp.Hour,
                    0,
                    0,
                    DateTimeKind.Utc);
                var endTime = startTime.AddHours(1);
                var filter = Builders<MeasurementsPerHour>.Filter.Eq(x => x.Sensor, sensor)
                    & Builders<MeasurementsPerHour>.Filter.Eq(x => x.StartTime, startTime);
                var update = Builders<MeasurementsPerHour>.Update
                    .SetOnInsert(x => x.EndTime, endTime)
                    .Inc(x => x.Total, m.Value)
                    .Inc(x => x.Count, 1)
                    .Max(x => x.Max, m.Value)
                    .Min(x => x.Min, m.Value)
                    .Push(x => x.Measurements, m);
                await coll.UpdateOneAsync(filter, update, new UpdateOptions() { IsUpsert = true });
            }

            var options = new FindOptions<MeasurementsPerHour, MeasurementsPerHour>()
            {
                Sort = Builders<MeasurementsPerHour>.Sort.Ascending(x => x.StartTime),
            };
            var measurementsPerHour = (await coll.FindAsync(FilterDefinition<MeasurementsPerHour>.Empty, options)).ToEnumerable();
            Console.WriteLine(string.Join(
                "\t",
                "Sensor".PadRight(9),
                "Start time".PadRight(19),
                "Count",
                "Avg",
                "Max",
                "Min"));
            foreach (var mPerHour in measurementsPerHour)
            {
                Console.WriteLine(string.Join(
                    "\t",
                    mPerHour.Sensor,
                    mPerHour.StartTime,
                    mPerHour.Count,
                    (mPerHour.Total / (float)mPerHour.Count).ToString("F2"),
                    mPerHour.Max,
                    mPerHour.Min));
            }
        }

        public class MeasurementsPerHour
        {
            [BsonRepresentation(BsonType.ObjectId)]
            public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

            public string Sensor { get; set; } = string.Empty;

            public DateTime StartTime { get; set; }

            public DateTime EndTime { get; set; }

            public int Total { get; set; }

            public int Count { get; set; }

            public int Max { get; set; }

            public int Min { get; set; }

            public ICollection<Measurement> Measurements { get; set; } = new List<Measurement>();
        }

        public class Measurement
        {
            public DateTime Timestamp { get; set; }

            public int Value { get; set; }
        }
    }
}
