using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EDrinks.Common;
using EDrinks.Events.Tabs;
using EDrinks.EventSource;

namespace EDrinks.CommandHandlers.Tabs
{
    public class SettleTabsCommand : ICommandRequest
    {
        public IEnumerable<Guid> TabIds { get; set; }
    }

    public class SettleTabsHandler : CommandHandler<SettleTabsCommand>
    {
        private readonly IEventSourceFacade _eventSource;

        public SettleTabsHandler(IEventSourceFacade eventSource)
        {
            _eventSource = eventSource;
        }
        
        protected override async Task<HandlerResult> DoHandle(SettleTabsCommand request)
        {
            var settlementEvents = new List<TabSettled>();
            var settlementId = Guid.NewGuid();
            
            foreach (var tabId in request.TabIds)
            {
                settlementEvents.Add(new TabSettled()
                {
                    SettlementId = settlementId,
                    TabId = tabId
                });
            }

            await _eventSource.WriteEvents(settlementEvents);

            return Ok();
        }
    }
}