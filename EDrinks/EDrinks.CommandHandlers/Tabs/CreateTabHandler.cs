using System;
using System.Threading.Tasks;
using EDrinks.Common;
using EDrinks.Events;
using EDrinks.Events.Tabs;
using EDrinks.EventSource;

namespace EDrinks.CommandHandlers.Tabs
{
    public class CreateTabCommand : ICommandRequest
    {
        public string Name { get; set; }
    }

    public class CreateTabHandler : CommandHandler<CreateTabCommand>
    {
        private readonly IEventSourceFacade _eventSource;

        public CreateTabHandler(IEventSourceFacade eventSource)
        {
            _eventSource = eventSource;
        }

        protected override async Task<HandlerResult> DoHandle(CreateTabCommand request)
        {
            var tabId = Guid.NewGuid();

            await _eventSource.WriteEvents(new BaseEvent[]
            {
                new TabCreated() {TabId = tabId},
                new TabNameChanged() {TabId = tabId, Name = request.Name}
            });

            return Ok();
        }
    }
}