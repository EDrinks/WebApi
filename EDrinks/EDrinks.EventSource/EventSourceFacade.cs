using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using EDrinks.Common.Config;
using EDrinks.Events;
using EventStore.ClientAPI;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace EDrinks.EventSource
{
    public interface IEventSourceFacade
    {
        Task WriteEvent(BaseEvent evt);

        Task WriteEvents(BaseEvent[] evts);

        void Subscribe(Func<BaseEvent, Task> callback);
    }

    public class EventSourceFacade : IEventSourceFacade
    {
        private readonly IEventLookup _eventLookup;
        private static readonly string STREAM = "edrinks";

        private IEventStoreConnection _connection;

        public EventSourceFacade(IOptions<EventStoreConfig> options, IEventLookup eventLookup)
        {
            _eventLookup = eventLookup;
            _connection =
                EventStoreConnection.Create(
                    new IPEndPoint(IPAddress.Parse(options.Value.IPAddress), options.Value.Port));
            _connection.ConnectAsync().Wait();
        }

        public async Task WriteEvent(BaseEvent evt)
        {
            await WriteEvents(new[] {evt});
        }

        public async Task WriteEvents(BaseEvent[] evts)
        {
            var eventDatas = new EventData[evts.Length];

            for (int i = 0; i < evts.Length; i++)
            {
                var metaDataStr = JsonConvert.SerializeObject(evts[i].MetaData);
                var contentStr = JsonConvert.SerializeObject(evts[i]);

                eventDatas[i] = new EventData(Guid.NewGuid(), evts[i].GetType().Name, true,
                    Encoding.UTF8.GetBytes(contentStr), Encoding.UTF8.GetBytes(metaDataStr));
            }

            await _connection.AppendToStreamAsync(STREAM, ExpectedVersion.Any, eventDatas);
        }

        public void Subscribe(Func<BaseEvent, Task> callback)
        {
            _connection.SubscribeToStreamFrom(STREAM, StreamCheckpoint.StreamStart,
                new CatchUpSubscriptionSettings(500, 20, false, false), async (subscription, resolvedEvent) =>
                {
                    var data = Encoding.UTF8.GetString(resolvedEvent.Event.Data);
                    Type eventType = _eventLookup.GetType(resolvedEvent.Event.EventType);
                    if (eventType != null)
                    {
                        var obj = (BaseEvent) JsonConvert.DeserializeObject(data, eventType);
                        await callback(obj);
                    }
                });
        }

        private async Task EventAppeared(EventStoreCatchUpSubscription subscription, ResolvedEvent resolvedEvent)
        {
            var data = Encoding.UTF8.GetString(resolvedEvent.Event.Data);
            Type eventType = _eventLookup.GetType(resolvedEvent.Event.EventType);
            if (eventType != null)
            {
                var obj = (BaseEvent) JsonConvert.DeserializeObject(data, eventType);
            }
        }
    }
}