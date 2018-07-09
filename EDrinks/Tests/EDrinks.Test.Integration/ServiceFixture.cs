using System;
using System.Net;
using System.Net.Http;
using EDrinks.WebApi;
using EventStore.ClientAPI;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace EDrinks.Test.Integration
{
    public class ServiceFixture : IDisposable
    {
        public IEventStoreConnection Connection;
        public string Stream;
        public HttpClient Client { get; set; }

        public ServiceFixture()
        {
            //(new CustomWebApplicationFactory()).CreateClient();
            var testServer = new TestServer(WebHost.CreateDefaultBuilder()
                .UseStartup<Startup>());
            Client = testServer.CreateClient();

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var ipAddress = configuration.GetValue<string>("EventStore:IPAddress");
            var port = configuration.GetValue<int>("EventStore:Port");
            Stream = configuration.GetValue<string>("EventStore:Stream");

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