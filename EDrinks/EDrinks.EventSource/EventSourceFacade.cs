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
            var metaDataStr = JsonConvert.SerializeObject(evt.MetaData);
            var contentStr = JsonConvert.SerializeObject(evt);

            var eventData = new EventData(Guid.NewGuid(), evt.GetEventName(), true,
                Encoding.UTF8.GetBytes(contentStr), Encoding.UTF8.GetBytes(metaDataStr));

            await _connection.AppendToStreamAsync("edrinks", ExpectedVersion.Any, eventData);
        }
    }
}