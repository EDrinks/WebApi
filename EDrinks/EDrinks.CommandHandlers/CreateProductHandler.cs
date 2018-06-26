using System;
using System.Threading.Tasks;
using EDrinks.Common;
using EDrinks.Events;
using EDrinks.Events.Products;
using EDrinks.EventSource;

namespace EDrinks.CommandHandlers
{
    public class CreateProductCommand : ICommandRequest
    {
        public string Name { get; set; }

        public decimal Price { get; set; }
    }

    public class CreateProductHandler : CommandHandler<CreateProductCommand>
    {
        private readonly IEventSourceFacade _eventSource;

        public CreateProductHandler(IEventSourceFacade eventSource)
        {
            _eventSource = eventSource;
        }

        protected override async Task<HandlerResult> DoHandle(CreateProductCommand request)
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

            return new HandlerResult()
            {
                ResultCode = ResultCode.Ok
            };
        }
    }
}