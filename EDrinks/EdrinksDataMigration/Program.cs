using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using EDrinks.Events;
using EDrinks.EventSourceSql.Model;
using EventStore.ClientAPI;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace EdrinksDataMigration
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var settings = ConnectionSettings.Create();
            var connection = EventStoreConnection.Create(settings, new IPEndPoint(
                IPAddress.Parse("127.0.0.1"), 1113));
            connection.ConnectAsync().Wait();

            var dbPath = args[0];
            var options = new DbContextOptionsBuilder<DomainContext>()
                .UseSqlite($"Data Source={dbPath}")
                .Options;
            var context = new DomainContext(options);
            await context.Database.EnsureCreatedAsync();

            var events = new List<ResolvedEvent>();

            StreamEventsSlice currentSlice;
            var stream = args[1];
            var nextSliceStart = (long) StreamPosition.Start;
            do
            {
                currentSlice = await connection.ReadStreamEventsForwardAsync(stream,
                    nextSliceStart, 200, false);
                nextSliceStart = currentSlice.NextEventNumber;

                events.AddRange(currentSlice.Events);
            } while (!currentSlice.IsEndOfStream);

            var eventLookup = new EventLookup();
            long id = 1;
            foreach (var resolvedEvent in events)
            {
                var data = Encoding.UTF8.GetString(resolvedEvent.Event.Data);
                var metaData = Encoding.UTF8.GetString(resolvedEvent.Event.Metadata);
                var eventType = eventLookup.GetType(resolvedEvent.Event.EventType);
                if (eventType != null)
                {
                    var obj = (BaseEvent) JsonConvert.DeserializeObject(data, eventType);
                    obj.MetaData = JsonConvert.DeserializeObject<MetaData>(metaData);
                    
                    await context.DomainEvents.AddAsync(new DomainEvent()
                    {
                        Id = id,
                        CreatedOn = obj.MetaData.CreatedOn,
                        CreatedBy = obj.MetaData.ToString(),
                        EventType = obj.GetType().Name,
                        Content = JsonConvert.SerializeObject(obj)
                    });

                    id++;
                }
            }

            await context.SaveChangesAsync();
        }
    }
}