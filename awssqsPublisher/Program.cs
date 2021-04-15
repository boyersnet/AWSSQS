using System;
using System.IO;
using System.Threading.Tasks;
using Amazon.SQS;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace awssqsPublisher
{
    class Program
    {

        private static ServiceProvider _serviceProvider;
        private static IConfigurationRoot _configuration;

        static async Task Main(string[] args)
        {
            ServiceCollection serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            _serviceProvider = serviceCollection.BuildServiceProvider();

            var app = _serviceProvider.GetService<App>();
            app?.Run();

            DisposeServices();
        }

        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            serviceCollection.AddSingleton<IConfiguration>(_configuration);
            serviceCollection.AddDefaultAWSOptions(_configuration.GetAWSOptions());
            serviceCollection.AddAWSService<IAmazonSQS>();
            serviceCollection.AddTransient<App>();
        }

        private static void DisposeServices()
        {
            if (_serviceProvider == null)
            {
                return;
            }

            if (_serviceProvider is IDisposable)
            {
                ((IDisposable)_serviceProvider).Dispose();
            }
        }

    }
}
