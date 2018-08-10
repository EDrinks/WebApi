using System;
using System.Text;
using System.Threading.Tasks;
using EDrinks.Common;
using EDrinks.Events;
using EventStore.ClientAPI;
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

            await _connection.AppendToStreamAsync(_streamResolver.GetStream(), ExpectedVersion.Any, eventDatas);
        }
    }
}