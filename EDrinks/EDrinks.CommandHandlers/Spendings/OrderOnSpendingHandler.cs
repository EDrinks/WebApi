using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using EDrinks.Events;
using EDrinks.Events.Spendings;
using MediatR;

namespace EDrinks.CommandHandlers.Spendings
{
    public class OrderOnSpendingCommand : IRequest<Guid>
    {
        public Guid SpendingId { get; set; }
        
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
    
    public class OrderOnSpendingHandler : IRequestHandler<OrderOnSpendingCommand, Guid>
    {
        private readonly IEventSourceFacade _eventSource;

        public OrderOnSpendingHandler(IEventSourceFacade eventSource)
        {
            _eventSource = eventSource;
        }
        
        public async Task<Guid> Handle(OrderOnSpendingCommand request, CancellationToken cancellationToken)
        {
            var orderId = Guid.NewGuid();
            
            await _eventSource.WriteEvent(new ProductOrderedOnSpending()
            {
                OrderId = orderId,
                SpendingId = request.SpendingId,
                Quantity = request.Quantity
            });

            return orderId;
        }
    }
}