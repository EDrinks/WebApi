using EDrinks.Common;

namespace EDrinks.EventSourceSql
{
    public class EventSourceFacade
    {
        private readonly IStreamResolver _streamResolver;

        public EventSourceFacade(IStreamResolver streamResolver)
        {
            _streamResolver = streamResolver;
        }
    }
}