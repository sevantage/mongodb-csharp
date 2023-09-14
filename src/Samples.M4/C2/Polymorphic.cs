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
    /// <summary>
    /// This sample demonstrates the polymorphic pattern by storing both sensor overview and sensor details in a single collection. 
    /// Hence, a single request allows both getting overview data for all sensors or all data for a specific sensor.
    /// </summary>
    internal class Polymorphic : ITestDatabaseSample
    {
        private readonly ConsoleHelper _consoleHelper;

        public Polymorphic(ConsoleHelper consoleHelper)
        {
            _consoleHelper = consoleHelper;
        }

        public async Task RunAsync(IMongoDatabase db, Configuration config)
        {
            var baseColl = db.GetCollection<SensorBase>("m4c2_polymorphic");
            await baseColl.DeleteManyAsync(FilterDefinition<SensorBase>.Empty);

            // Generate sensor data
            var sensorStateValues = Enum.GetValues<SensorState>();
            var sensorData = GenerateSensorData(sensorStateValues).ToArray();
            await baseColl.InsertManyAsync(sensorData, new InsertManyOptions() { IsOrdered = false });

            // Get all overviews
            var overviewColl = baseColl.OfType<SensorOverview>();
            var overviews = (await overviewColl.FindAsync(FilterDefinition<SensorOverview>.Empty)).ToEnumerable();

            Console.WriteLine("OVERVIEW VIEW");
            Console.WriteLine(string.Join(
                "\t",
                "Sensor Id".PadRight(24),
                "Name".PadRight(10),
                "State".PadRight(10),
                "Last Value"
                ));
            foreach (var ov in overviews)
            {
                PrintSensorOverview(ov);
            }

            // Get details data
            var sensorId = sensorData.OfType<SensorOverview>().Where(x => x.Name == "Sensor 7").Single().SensorId;
            var baseDocs = await (await baseColl.FindAsync(x => x.SensorId == sensorId)).ToListAsync();
            var overview = baseDocs.OfType<SensorOverview>().Single();
            _consoleHelper.Separator();
            Console.WriteLine("DETAILS VIEW");
            Console.WriteLine("Overview");
            PrintSensorOverview(overview);
            Console.WriteLine("Details");
            var details = baseDocs.OfType<SensorDetails>();
            Console.WriteLine(string.Join(
                "\t",
                "Feature name",
                "Value"));
            foreach (var d in details)
            {
                Console.WriteLine(string.Join(
                    "\t",
                    d.FeatureName,
                    d.Value));
            }
        }

        private static void PrintSensorOverview(SensorOverview ov)
        {
            Console.WriteLine(string.Join(
                "\t",
                ov.SensorId,
                ov.Name,
                ov.State.ToString().PadRight(10),
                ov.LastValue
                ));
        }

        private static IEnumerable<SensorBase> GenerateSensorData(SensorState[] sensorStateValues)
        {
            return Enumerable.Range(1, 8).SelectMany(x =>
            {
                var sensorId = ObjectId.GenerateNewId().ToString();

                var overview = new SensorOverview() { SensorId = sensorId, Name = $"Sensor {x}", State = sensorStateValues[x % sensorStateValues.Length], LastContact = DateTime.UtcNow, LastValue = 100 - x };
                var details = Enumerable.Range(1, 3).Select(detailId => new SensorDetails()
                {
                    SensorId = sensorId,
                    FeatureName = $"Feature {detailId}",
                    Value = 100 - detailId,
                });

                return new SensorBase[]
                {
                    overview,
                }.Union(details);
            });
        }

        [BsonKnownTypes(typeof(SensorOverview), typeof(SensorDetails))]
        public class SensorBase
        {
            [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
            public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

            [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
            public string SensorId { get; set; } = ObjectId.GenerateNewId().ToString();
        }

        public enum SensorState
        {
            Unknown,
            Critical,
            Warning,
            Ok,
        }

        public class SensorOverview : SensorBase
        {
            public string Name { get; set; } = string.Empty;

            public SensorState State { get; set; }

            public int LastValue { get; set; }

            public DateTime LastContact { get; set; }
        }

        public class SensorDetails : SensorBase
        {
            public string FeatureName { get; set; } = string.Empty;

            public int Value { get; set; }
        }
    }
}
