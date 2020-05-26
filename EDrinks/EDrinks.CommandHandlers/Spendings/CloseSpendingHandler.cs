using System;
using System.Threading;
using System.Threading.Tasks;
using EDrinks.Events;
using EDrinks.Events.Spendings;
using MediatR;

namespace EDrinks.CommandHandlers.Spendings
{
    public class CloseSpendingCommand : IRequest
    {
        public Guid SpendingId { get; set; }
    }
    
    public class CloseSpendingHandler : AsyncRequestHandler<CloseSpendingCommand>
    {
        private readonly IEventSourceFacade _eventSource;

        public CloseSpendingHandler(IEventSourceFacade eventSource)
        {
            _eventSource = eventSource;
        }
        
        protected override async Task Handle(CloseSpendingCommand request, CancellationToken cancellationToken)
        {
            await _eventSource.WriteEvent(new SpendingClosed()
            {
                SpendingId = request.SpendingId
            });
        }
    }
}