using System.Collections.Generic;
using System.Threading.Tasks;

namespace EDrinks.Events
{
    public interface IEventSourceFacade
    {
        Task WriteEvent(BaseEvent evt);

        Task WriteEvents(IEnumerable<BaseEvent> evts);
    }
}