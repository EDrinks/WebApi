using System;
using System.Threading.Tasks;
using EDrinks.Common;
using EDrinks.Events.Tabs;
using EDrinks.EventSource;

namespace EDrinks.CommandHandlers.Tabs
{
    public class DeleteTabCommand : ICommandRequest
    {
        public Guid TabId { get; set; }
    }
    
    public class DeleteTabHandler : CommandHandler<DeleteTabCommand>
    {
        private readonly IEventSourceFacade _eventSource;

        public DeleteTabHandler(IEventSourceFacade eventSource)
        {
            _eventSource = eventSource;
        }
        
        protected override async Task<HandlerResult> DoHandle(DeleteTabCommand request)
        {
            await _eventSource.WriteEvent(new TabDeleted()
            {
                TabId = request.TabId
            });
            
            return Ok();
        }
    }
}