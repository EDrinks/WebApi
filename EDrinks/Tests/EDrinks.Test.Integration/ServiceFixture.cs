using System;
using System.Net;
using System.Net.Http;
using EDrinks.Common;
using EDrinks.WebApi;
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

        public ServiceFixture()
        {
            var testServer = new TestServer(WebHost.CreateDefaultBuilder()
                .UseStartup<Startup>()
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