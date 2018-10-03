using System;
using System.Threading.Tasks;
using EDrinks.Common;
using EDrinks.Events.Orders;
using EDrinks.EventSource;

namespace EDrinks.CommandHandlers.Orders
{
    public class DeleteOrderCommand : ICommandRequest
    {
        public Guid OrderId { get; set; }
    }
    
    public class DeleteOrderHandler : CommandHandler<DeleteOrderCommand>
    {
        private readonly IEventSourceFacade _eventSource;

        public DeleteOrderHandler(IEventSourceFacade eventSource)
        {
            _eventSource = eventSource;
        }
        
        protected override async Task<HandlerResult> DoHandle(DeleteOrderCommand request)
        {
            await _eventSource.WriteEvent(new OrderDeleted()
            {
                OrderId = request.OrderId
            });
            
            return Ok();
        }
    }
}