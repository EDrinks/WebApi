using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EDrinks.Events.Tabs;
using EDrinks.EventSource;
using MediatR;

namespace EDrinks.CommandHandlers.Tabs
{
    public class SettleTabsCommand : IRequest
    {
        public IEnumerable<Guid> TabIds { get; set; }
    }

    public class SettleTabsHandler : AsyncRequestHandler<SettleTabsCommand>
    {
        private readonly IEventSourceFacade _eventSource;

        public SettleTabsHandler(IEventSourceFacade eventSource)
        {
            _eventSource = eventSource;
        }

        protected override async Task Handle(SettleTabsCommand request, CancellationToken cancellationToken)
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
        }
    }
}