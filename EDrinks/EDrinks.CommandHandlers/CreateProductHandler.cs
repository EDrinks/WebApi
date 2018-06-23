using System;
using System.Threading;
using System.Threading.Tasks;
using EDrinks.Events;
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

            await _eventSource.WriteEvents(new BaseEvent[]
            {
                new ProductCreated()
                {
                    ProductId = productId
                },
                new ProductNameChanged()
                {
                    ProductId = productId,
                    Name = request.Name
                },
                new ProductPriceChanged()
                {
                    ProductId = productId,
                    Price = request.Price
                }
            });

            return true;
        }
    }
}