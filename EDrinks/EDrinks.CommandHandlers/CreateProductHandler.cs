using System;
using System.Threading;
using System.Threading.Tasks;
using EDrinks.Events.Products;
using EDrinks.EventSource;
using MediatR;

namespace EDrinks.CommandHandlers
{
    public class CreateProductCommand : IRequest<bool>
    {
        public string Name { get; set; }

        public decimal Price { get; set; }
    }

    public class CreateProductHandler : IRequestHandler<CreateProductCommand, bool>
    {
        private readonly IEventSourceFacade _eventSource;

        public CreateProductHandler(IEventSourceFacade eventSource)
        {
            _eventSource = eventSource;
        }
        
        public async Task<bool> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var productId = Guid.NewGuid();
            
            await _eventSource.WriteEvent(new ProductCreated()
            {
                ProductId = productId
            });
            await _eventSource.WriteEvent(new ProductNameChanged()
            {
                ProductId = productId,
                Name = request.Name
            });
            await _eventSource.WriteEvent(new ProductPriceChanged()
            {
                ProductId = productId,
                Price = request.Price
            });

            return true;
        }
    }
}