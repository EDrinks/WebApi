using System;
using System.Threading.Tasks;
using EDrinks.Common;
using EDrinks.Events.Tabs;
using EDrinks.EventSource;

namespace EDrinks.CommandHandlers.Tabs
{
    public class UpdateTabCommand : ICommandRequest
    {
        public Guid TabId { get; set; }

        public string Name { get; set; }
    }
    
    public class UpdateTabHandler : CommandHandler<UpdateTabCommand>
    {
        private readonly IEventSourceFacade _eventSource;

        public UpdateTabHandler(IEventSourceFacade eventSource)
        {
            _eventSource = eventSource;
        }
        
        protected override async Task<HandlerResult> DoHandle(UpdateTabCommand request)
        {
            await _eventSource.WriteEvent(new TabNameChanged()
            {
                TabId = request.TabId,
                Name = request.Name
            });
            
            return Ok();
        }
    }
}