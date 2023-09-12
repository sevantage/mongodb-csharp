using MongoDB.Driver;
using Samples.Base;

namespace Samples.M2
{
    internal class Program
    {
        async static Task Main(string[] args)
        {
            var host = new SampleHostBuilder()
                .WithSamplesFrom(System.Reflection.Assembly.GetExecutingAssembly())
                .WithClientBuilder(config =>
                {
                    var settings = MongoClientSettings.FromConnectionString(config.ConnectionString);
                    settings.LinqProvider = MongoDB.Driver.Linq.LinqProvider.V3;
                    return new MongoClient(settings);
                })
                .Build(args);
            await host.RunAsync();
        }
    }
}