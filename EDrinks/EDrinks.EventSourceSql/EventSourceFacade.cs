using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using EDrinks.Common;
using EDrinks.Events;
using EDrinks.EventSourceSql.Model;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace EDrinks.EventSourceSql
{
    public class EventSourceFacade : IEventSourceFacade
    {
        private readonly IStreamResolver _streamResolver;
        private readonly DomainContext _context;

        public EventSourceFacade(IStreamResolver streamResolver, IDatabaseLookup databaseLookup)
        {
            _streamResolver = streamResolver;

            var dbPath = databaseLookup.GetDatabase(streamResolver.GetStream());
            var options = new DbContextOptionsBuilder<DomainContext>()
                .UseSqlite($"Data Source={dbPath}")
                .Options;
            _context = new DomainContext(options);
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
                    EventType = evt.GetType().Name,
                    Content = JsonConvert.SerializeObject(evt)
                });
            }

            await _context.SaveChangesAsync();
        }

        private static string CreateMD5(string input)
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
    }
}