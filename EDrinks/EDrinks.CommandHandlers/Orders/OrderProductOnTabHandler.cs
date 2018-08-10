using System;
using System.Threading.Tasks;
using EDrinks.Common;
using EDrinks.Events.Orders;
using EDrinks.EventSource;

namespace EDrinks.CommandHandlers.Orders
{
    public class OrderProductOnTabCommand : ICommandRequest
    {
        public Guid TabId { get; set; }

        public Guid ProductId { get; set; }

        public int Quantity { get; set; }
    }
    
    public class OrderProductOnTabHandler : CommandHandler<OrderProductOnTabCommand>
    {
        private readonly IEventSourceFacade _eventSource;

        public OrderProductOnTabHandler(IEventSourceFacade eventSource)
        {
            _eventSource = eventSource;
        }
        
        protected override async Task<HandlerResult> DoHandle(OrderProductOnTabCommand request)
        {
            await _eventSource.WriteEvent(new ProductOrderedOnTab()
            {
                TabId = request.TabId,
                ProductId = request.ProductId,
                Quantity = request.Quantity
            });

            return Ok();
        }
    }
}