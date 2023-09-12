using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Samples.Base
{
    public class SampleHostBuilder
    {
        private Func<Configuration, IMongoClient> _clientBldr = DefaultClientBuilder;
        private ISet<Type> _samples = new HashSet<Type>();
        private Func<IServiceCollection, IServiceCollection>? _configureServices = null;

        public SampleHostBuilder WithClientBuilder(Func<Configuration, IMongoClient> clientBldr)
        {
            _clientBldr = clientBldr;
            return this;
        }

        public SampleHostBuilder WithSamplesFrom(Assembly assembly)
        {
            var types = assembly.GetTypes().Where(x => typeof(ISample).IsAssignableFrom(x));
            foreach (var type in types)
            {
                _samples.Add(type);
            }
            return this;
        }

        public SampleHostBuilder ConfigureServices(Func<IServiceCollection, IServiceCollection> configureServices)
        {
            _configureServices = configureServices;
            return this;
        }

        public SampleHost Build(string[] args)
        {
            if (!_samples.Any())
                throw new InvalidOperationException("No samples found");
            var config = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .AddUserSecrets(Assembly.GetExecutingAssembly(), true)
                .Build();
            IServiceCollection services = new ServiceCollection();
            if (_configureServices != null)
                services = _configureServices(services);
            services.Configure<Configuration>(config);
            services.AddSingleton(_clientBldr);
            services.AddSingleton<SampleHost>();
            foreach (var type in _samples)
                services.AddSingleton(typeof(ISample), type);
            var prov = services.BuildServiceProvider();
            return prov.GetRequiredService<SampleHost>();
        }

        private static IMongoClient DefaultClientBuilder(Configuration configuration)
        {
            var settings = MongoClientSettings.FromConnectionString(configuration.ConnectionString);
            settings.LinqProvider = MongoDB.Driver.Linq.LinqProvider.V3;
            return new MongoClient(settings);
        }
    }
}
