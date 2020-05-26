using System;
using System.Threading;
using System.Threading.Tasks;
using EDrinks.Events;
using EDrinks.Events.Orders;
using MediatR;

namespace EDrinks.CommandHandlers.Orders
{
    public class OrderProductOnTabCommand : IRequest<Guid>
    {
        public Guid TabId { get; set; }

        public Guid ProductId { get; set; }

        public int Quantity { get; set; }
    }
    
    public class OrderProductOnTabHandler : IRequestHandler<OrderProductOnTabCommand, Guid>
    {
        private readonly IEventSourceFacade _eventSource;

        public OrderProductOnTabHandler(IEventSourceFacade eventSource)
        {
            _eventSource = eventSource;
        }
        
        public async Task<Guid> Handle(OrderProductOnTabCommand request, CancellationToken cancellationToken)
        {
            var orderId = Guid.NewGuid();
            
            await _eventSource.WriteEvent(new ProductOrderedOnTab()
            {
                OrderId = orderId,
                TabId = request.TabId,
                ProductId = request.ProductId,
                Quantity = request.Quantity
            });

            return orderId;
        }
    }
}