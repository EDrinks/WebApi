using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using EDrinks.Events;
using EDrinks.Events.Spendings;
using MediatR;

namespace EDrinks.CommandHandlers.Spendings
{
    public class CreateSpendingCommand : IRequest<Guid>
    {
        public Guid TabId { get; set; }

        public Guid ProductId { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
    
    public class CreateSpendingHandler : IRequestHandler<CreateSpendingCommand, Guid>
    {
        private readonly IEventSourceFacade _eventSource;

        public CreateSpendingHandler(IEventSourceFacade eventSource)
        {
            _eventSource = eventSource;
        }
        
        public async Task<Guid> Handle(CreateSpendingCommand request, CancellationToken cancellationToken)
        {
            var spendingId = Guid.NewGuid();

            await _eventSource.WriteEvent(new SpendingCreated()
            {
                SpendingId = spendingId,
                TabId = request.TabId,
                ProductId = request.ProductId,
                Quantity = request.Quantity
            });

            return spendingId;
        }
    }
}