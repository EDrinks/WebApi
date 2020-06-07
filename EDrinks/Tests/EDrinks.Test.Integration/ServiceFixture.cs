using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using EDrinks.Common;
using EDrinks.EventSourceSql.Model;
using EDrinks.Test.Integration.DataGenerator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace EDrinks.Test.Integration
{
    public class ServiceFixture : IDisposable
    {
        public DomainContext Context { get; set; }
        public HttpClient Client { get; set; }
        public Generator Generator { get; set; }

        public ServiceFixture()
        {
            var factory = new CustomWebAppFactory();
            Client = factory.CreateClient();

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var dataDirectory = configuration.GetValue<string>("Data:BaseDir");
            if (!Directory.Exists(dataDirectory))
            {
                Directory.CreateDirectory(dataDirectory);
            }

            var userDbFileName = CreateSystemDb(configuration, dataDirectory);
            CreateEventDb(dataDirectory, userDbFileName);
        }

        private string CreateSystemDb(IConfigurationRoot configuration, string dataDirectory)
        {
            var systemDbFile = configuration.GetValue<string>("Data:SystemDb");
            var systemDbPath = Path.Join(dataDirectory, systemDbFile);

            var options = new DbContextOptionsBuilder<SystemContext>()
                .UseSqlite($"Data Source={systemDbPath}")
                .Options;

            var systemContext = new SystemContext(options);
            systemContext.Database.EnsureCreated();

            var resolver = new TestStreamResolver();
            var authId = resolver.GetStream();
            var userDbFileName = HashUtil.CreateMd5(authId) + ".db";

            if (!systemContext.Users.Any(e => e.AuthIdentifier == authId))
            {
                systemContext.Users.Add(new User()
                {
                    AuthIdentifier = authId,
                    EventDbFile = userDbFileName
                });
                systemContext.SaveChanges();
            }

            return userDbFileName;
        }

        private void CreateEventDb(string dataDirectory, string userDbFileName)
        {
            var eventDbPath = Path.Join(dataDirectory, userDbFileName);
            var domainOptions = new DbContextOptionsBuilder<DomainContext>()
                .UseSqlite($"Data Source={eventDbPath}")
                .Options;
            Context = new DomainContext(domainOptions);
            Context.Database.EnsureCreated();

            Generator = new Generator(Context);
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