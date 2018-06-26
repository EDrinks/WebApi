using System;
using System.Threading;
using System.Threading.Tasks;
using EDrinks.Events;
using EDrinks.Events.Products;
using EDrinks.EventSource;
using MediatR;

namespace EDrinks.CommandHandlers
{
    public class UpdateProductCommand : IRequest<bool>
    {
        public Guid ProductId { get; set; }

        public string ProductName { get; set; }

        public decimal ProductPrice { get; set; }
    }

    public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, bool>
    {
        private readonly IEventSourceFacade _eventSource;

        public UpdateProductHandler(IEventSourceFacade eventSource)
        {
            _eventSource = eventSource;
        }

        public async Task<bool> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            await _eventSource.WriteEvents(new BaseEvent[]
            {
                new ProductNameChanged()
                {
                    ProductId = request.ProductId,
                    Name = request.ProductName
                },
                new ProductPriceChanged()
                {
                    ProductId = request.ProductId,
                    Price = request.ProductPrice
                }
            });

            return true;
        }
    }
}