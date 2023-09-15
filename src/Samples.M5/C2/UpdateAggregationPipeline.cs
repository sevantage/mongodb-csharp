using MongoDB.Bson;
using MongoDB.Driver;
using Samples.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Samples.M5.C2
{
    internal class UpdateAggregationPipeline : ITestDatabaseSample
    {
        private readonly ConsoleHelper _consoleHelper;

        public UpdateAggregationPipeline(ConsoleHelper consoleHelper)
        {
            _consoleHelper = consoleHelper;
        }

        public async Task RunAsync(IMongoDatabase db, Configuration config)
        {
            var coll = db.GetCollection<TestDocument>("m5c2_updateAggregationPipeline");
            await coll.DeleteManyAsync(FilterDefinition<TestDocument>.Empty);

            var yesterday = new DateTime(2023, 08, 15, 0, 0, 0, DateTimeKind.Utc);
            var doc = new TestDocument()
            {
                Id = 1,
                Avg = 79,
                Count = 4,
                Measurements = new Measurement[]
                {
                    new() { Timestamp = new DateTime(2023, 08, 14, 01, 0, 0, DateTimeKind.Utc), Value = 100 },
                    new() { Timestamp = new DateTime(2023, 08, 14, 03, 0, 0, DateTimeKind.Utc), Value = 200 },
                    new() { Timestamp = new DateTime(2023, 08, 15, 01, 0, 0, DateTimeKind.Utc), Value = 9 },
                    new() { Timestamp = new DateTime(2023, 08, 15, 03, 0, 0, DateTimeKind.Utc), Value = 7 },
                },
            };
            await coll.InsertOneAsync(doc);

            await LoadAndPrintDocAsync("BEFORE UPDATE", coll, 1);

            var newMeasurement = new Measurement() { Timestamp = new DateTime(2023, 08, 15, 12, 34, 56, DateTimeKind.Utc), Value = 2 };
            PipelineDefinition<TestDocument, TestDocument> pipeline = new EmptyPipelineDefinition<TestDocument>()
                .AddPurgeAndAddToMeasurements(newMeasurement, yesterday)
                .AddUpdateAvgAndCount();
            var update = Builders<TestDocument>.Update.Pipeline(pipeline);
            await coll.UpdateOneAsync(x => x.Id == 1, update);

            _consoleHelper.Separator();
            await LoadAndPrintDocAsync("AFTER UPDATE", coll, 1);
        }

        private async Task LoadAndPrintDocAsync(string msg, IMongoCollection<TestDocument> coll, int id)
        {
            var doc = await (await coll.FindAsync(x => x.Id == id)).SingleAsync();
            Console.WriteLine(msg);
            Console.WriteLine(JsonSerializer.Serialize(doc, new JsonSerializerOptions() { WriteIndented = true }));
        }
    }

    public class TestDocument
    {
        public int Id { get; set; } = 1;

        public int Avg { get; set; }

        public int Count { get; set; }

        public ICollection<Measurement> Measurements { get; set; } = new List<Measurement>();
    }

    public class Measurement
    {
        public DateTime Timestamp { get; set; }

        public int Value { get; set; }
    }

    internal static class PipelineExtensions
    {
        public static PipelineDefinition<TestDocument, TestDocument> AddPurgeAndAddToMeasurements(this EmptyPipelineDefinition<TestDocument> pipeline, Measurement measurement, DateTime yesterday)
        {
            var stage = new BsonDocument()
                {
                    {
                        "$set",
                        new BsonDocument()
                        {
                            {
                                nameof(TestDocument.Measurements),
                                new BsonDocument()
                                {
                                    {
                                        "$concatArrays",
                                        new BsonArray()
                                        {
                                            new BsonDocument()
                                            {
                                                {
                                                    "$filter",
                                                    new BsonDocument()
                                                    {
                                                        { "input", "$" + nameof(TestDocument.Measurements) },
                                                        {
                                                            "cond",
                                                            new BsonDocument()
                                                            {
                                                                { "$gte", new BsonArray() { "$$this." + nameof(Measurement.Timestamp), yesterday } }
                                                            }
                                                        }
                                                    }
                                                 }
                                            }, 
                                            new BsonArray()
                                            {
                                                new BsonDocument()
                                                {
                                                    { nameof(Measurement.Timestamp), measurement.Timestamp },
                                                    { nameof(Measurement.Value), measurement.Value },
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    },
                };
            return pipeline
                .AppendStage<TestDocument, TestDocument, TestDocument>(stage);
        }

        public static PipelineDefinition<TestDocument, TestDocument> AddUpdateAvgAndCount(this PipelineDefinition<TestDocument, TestDocument> pipeline)
        {
            var stageStr = @"
                {
                      $set: {
                        Count: { $size: ""$Measurements"" },
                        Avg: { $avg: ""$Measurements.Value"" },
                      },
                }";
            var stage = BsonDocument.Parse(stageStr);
            return pipeline
                .AppendStage<TestDocument, TestDocument, TestDocument>(stage);
        }
    }
}
