using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using EDrinks.Events.Spendings;
using EDrinks.EventSource;
using MediatR;

namespace EDrinks.CommandHandlers.Spendings
{
    public class OrderOnSpendingCommand : IRequest
    {
        public Guid SpendingId { get; set; }
        
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
    
    public class OrderOnSpendingHandler : AsyncRequestHandler<OrderOnSpendingCommand>
    {
        private readonly IEventSourceFacade _eventSource;

        public OrderOnSpendingHandler(IEventSourceFacade eventSource)
        {
            _eventSource = eventSource;
        }
        
        protected override async Task Handle(OrderOnSpendingCommand request, CancellationToken cancellationToken)
        {
            await _eventSource.WriteEvent(new ProductOrderedOnSpending()
            {
                OrderId = Guid.NewGuid(),
                SpendingId = request.SpendingId,
                Quantity = request.Quantity
            });
        }
    }
}