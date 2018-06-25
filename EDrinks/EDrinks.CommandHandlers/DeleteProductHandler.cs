using System;
using System.Threading;
using System.Threading.Tasks;
using EDrinks.Events.Products;
using EDrinks.EventSource;
using MediatR;

namespace EDrinks.CommandHandlers
{
    public class DeleteProductCommand : IRequest<bool>
    {
        public Guid ProductId { get; set; }
    }
    
    public class DeleteProductHandler : IRequestHandler<DeleteProductCommand, bool>
    {
        private readonly IEventSourceFacade _eventSource;

        public DeleteProductHandler(IEventSourceFacade eventSource)
        {
            _eventSource = eventSource;
        }
        
        public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            await _eventSource.WriteEvent(new ProductDeleted()
            {
                ProductId = request.ProductId
            });
            
            return true;
        }
    }
}