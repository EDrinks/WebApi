using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDrinks.Common;
using EDrinks.Events;
using EDrinks.EventSourceSql.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace EDrinks.EventSourceSql
{
    public class EventSourceFacade : IEventSourceFacade
    {
        private readonly IStreamResolver _streamResolver;
        private readonly DomainContext _context;

        public EventSourceFacade(IStreamResolver streamResolver, IConfiguration configuration)
        {
            _streamResolver = streamResolver;

            var dbBaseDirectory = configuration.GetValue<string>("Data:BaseDir");
            var systemDb = configuration.GetValue<string>("Data:SystemDb");
            var systemDbPath = Path.Join(dbBaseDirectory, systemDb);

            var systemOptions = new DbContextOptionsBuilder<SystemContext>()
                .UseSqlite($"Data Source={systemDbPath}")
                .Options;
            var systemContext = new SystemContext(systemOptions);
            systemContext.Database.EnsureCreated();

            var userId = _streamResolver.GetStream();
            var user = systemContext.Users.FirstOrDefault(e => e.AuthIdentifier == userId);
            string dbPath = "";
            if (user == null)
            {
                user = new User()
                {
                    AuthIdentifier = userId,
                    EventDbFile = "events_" + CreateMD5(userId) + ".db"
                };
                systemContext.Users.Add(user);
                systemContext.SaveChanges();
            }

            dbPath = Path.Join(dbBaseDirectory, user.EventDbFile);

            var options = new DbContextOptionsBuilder<DomainContext>()
                .UseSqlite($"Data Source={dbPath}")
                .Options;

            _context = new DomainContext(options);
            _context.Database.EnsureCreated();
        }

        public async Task WriteEvent(BaseEvent evt)
        {
            await WriteEvents(new[] {evt});
        }

        public async Task WriteEvents(IEnumerable<BaseEvent> evts)
        {
            foreach (var evt in evts)
            {
                await _context.DomainEvents.AddAsync(new DomainEvent()
                {
                    CreatedOn = DateTime.UtcNow,
                    CreatedBy = "system",
                    Content = JsonConvert.SerializeObject(evt)
                });
            }

            await _context.SaveChangesAsync();
        }

        private static string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
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
    }
}