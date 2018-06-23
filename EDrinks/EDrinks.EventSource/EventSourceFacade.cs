using System;
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
    }

    public class EventSourceFacade : IEventSourceFacade
    {
        private IEventStoreConnection _connection;

        public EventSourceFacade(IOptions<EventStoreConfig> options)
        {
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

                eventDatas[i] = new EventData(Guid.NewGuid(), evts[i].GetEventName(), true,
                    Encoding.UTF8.GetBytes(contentStr), Encoding.UTF8.GetBytes(metaDataStr));
            }

            await _connection.AppendToStreamAsync("edrinks", ExpectedVersion.Any, eventDatas);
        }
    }
}