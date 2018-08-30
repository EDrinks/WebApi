using System;
using System.Threading;
using System.Threading.Tasks;
using EDrinks.Events;
using EDrinks.Events.Tabs;
using EDrinks.EventSource;
using MediatR;

namespace EDrinks.CommandHandlers.Tabs
{
    public class CreateTabCommand : IRequest<Guid>
    {
        public string Name { get; set; }
    }

    public class CreateTabHandler : IRequestHandler<CreateTabCommand, Guid>
    {
        private readonly IEventSourceFacade _eventSource;

        public CreateTabHandler(IEventSourceFacade eventSource)
        {
            _eventSource = eventSource;
        }

        public async Task<Guid> Handle(CreateTabCommand request, CancellationToken cancellationToken)
        {
            var tabId = Guid.NewGuid();

            await _eventSource.WriteEvents(new BaseEvent[]
            {
                new TabCreated() {TabId = tabId},
                new TabNameChanged() {TabId = tabId, Name = request.Name}
            });

            return tabId;
        }
    }
}