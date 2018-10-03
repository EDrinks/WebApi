using System;
using System.Net;
using System.Net.Http;
using EDrinks.Common;
using EDrinks.Test.Integration.DataGenerator;
using EventStore.ClientAPI;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace EDrinks.Test.Integration
{
    public class ServiceFixture : IDisposable
    {
        public IEventStoreConnection Connection;
        public IStreamResolver StreamResolver;
        public HttpClient Client { get; set; }
        public Generator Generator { get; set; }

        public ServiceFixture()
        {
            var testServer = new TestServer(WebHost.CreateDefaultBuilder()
                .UseStartup<TestStartup>()
                .ConfigureTestServices(services => { services.AddScoped<IStreamResolver, TestStreamResolver>(); }));
            Client = testServer.CreateClient();
            
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var ipAddress = configuration.GetValue<string>("EventStore:IPAddress");
            var port = configuration.GetValue<int>("EventStore:Port");
            StreamResolver = new TestStreamResolver();

            var settings = ConnectionSettings.Create();
            Connection =
                EventStoreConnection.Create(settings, new IPEndPoint(IPAddress.Parse(ipAddress), port));
            Connection.ConnectAsync().Wait();
            
            Generator = new Generator(Connection, StreamResolver.GetStream());
        }

        public void Dispose()
        {
        }
    }

    [CollectionDefinition("Service")]
    public class ServiceCollection : ICollectionFixture<ServiceFixture>
    {
    }
}