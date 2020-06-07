using System;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
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

            var systemDbFile = configuration.GetValue<string>("Data:SystemDb");
            var systemDbPath = Path.Join(dataDirectory, systemDbFile);
            
            var options = new DbContextOptionsBuilder<SystemContext>()
                .UseSqlite($"Data Source={systemDbPath}")
                .Options;

            var systemContext = new SystemContext(options);
            systemContext.Database.EnsureCreated();
            
            var resolver = new TestStreamResolver();
            var authId = resolver.GetStream();
            var userDbFileName = CreateMd5(authId) + ".db";
            systemContext.Users.Add(new User()
            {
                AuthIdentifier = authId,
                EventDbFile = userDbFileName
            });
            systemContext.SaveChanges();

            var eventDbPath = Path.Join(dataDirectory, userDbFileName);
            var domainOptions = new DbContextOptionsBuilder<DomainContext>()
                .UseSqlite($"Data Source={eventDbPath}")
                .Options;
            Context = new DomainContext(domainOptions);
            Context.Database.EnsureCreated();
            
            Generator = new Generator(Context);
        }
        
        private static string CreateMd5(string input)
        {
            // Use input string to calculate MD5 hash
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }

                return sb.ToString();
            }
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