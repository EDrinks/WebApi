using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EDrinks.Common;
using EDrinks.Events;
using EventStore.ClientAPI;
using Newtonsoft.Json;

namespace EDrinks.EventSource
{
    public class EventSourceFacade : IEventSourceFacade
    {
        private readonly IEventLookup _eventLookup;

        private readonly IStreamResolver _streamResolver;

        private readonly IEventStoreConnection _connection;

        public EventSourceFacade(IEventStoreConnection connection, IEventLookup eventLookup,
            IStreamResolver streamResolver)
        {
            _eventLookup = eventLookup;
            _streamResolver = streamResolver;
            _connection = connection;
        }

        public async Task WriteEvent(BaseEvent evt)
        {
            await WriteEvents(new[] {evt});
        }

        public async Task WriteEvents(IEnumerable<BaseEvent> evts)
        {
            var eventDatas = new List<EventData>();

            foreach (var evt in evts)
            {
                var metaDataStr = JsonConvert.SerializeObject(evt.MetaData);
                var contentStr = JsonConvert.SerializeObject(evt);

                eventDatas.Add(new EventData(Guid.NewGuid(), evt.GetType().Name, true,
                    Encoding.UTF8.GetBytes(contentStr), Encoding.UTF8.GetBytes(metaDataStr)));
            }

            await _connection.AppendToStreamAsync(_streamResolver.GetStream(), ExpectedVersion.Any, eventDatas);
        }
    }
}