﻿using Samples.Base;

namespace Samples.M6
{
    internal class Program
    {
        async static Task Main(string[] args)
        {
            var host = new SampleHostBuilder()
                .WithSamplesFrom(System.Reflection.Assembly.GetExecutingAssembly())
                .Build(args);
            await host.RunAsync();
        }
    }
}