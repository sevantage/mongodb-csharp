using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Events;
using Samples.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samples.M6.C1
{
    internal class LogStatements : ISampleWithConfig
    {
        public async Task RunAsync(Configuration config)
        {
            var settings = MongoClientSettings.FromConnectionString(config.ConnectionString);
            settings.ClusterConfigurator = cb =>
            {
                cb.Subscribe<CommandStartedEvent>(x => LogEvent(x, x.Command.ToString()));
                cb.Subscribe<CommandSucceededEvent>(x => LogEvent(x, x.Reply.ToString()));
                cb.Subscribe<CommandFailedEvent>(x => LogEvent(x, x.Failure.Message));
            };
            var client = new MongoClient(settings);
            var db = client.GetDatabase(config.SampleMoviesDatabaseName);
            var coll = db.GetCollection<Movie>("movies");
            await (await coll.FindAsync(x => x.Title == "The Godfather")).ToListAsync();
        }

        private void LogEvent<T>(T ev, string info)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(typeof(T).Name);
            Console.ForegroundColor = color;
            Console.Write("> ");
            Console.WriteLine(info);
        }
    }
}
