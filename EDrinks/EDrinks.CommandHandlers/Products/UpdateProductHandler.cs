using System;
using System.Threading.Tasks;
using EDrinks.Common;
using EDrinks.Events;
using EDrinks.Events.Products;

namespace EDrinks.CommandHandlers.Products
{
    public class UpdateProductCommand : ICommandRequest
    {
        public Guid ProductId { get; set; }

        public string ProductName { get; set; }

        public decimal ProductPrice { get; set; }
    }

    public class UpdateProductHandler : CommandHandler<UpdateProductCommand>
    {
        private readonly IEventSourceFacade _eventSource;

        public UpdateProductHandler(IEventSourceFacade eventSource)
        {
            _eventSource = eventSource;
        }

        protected override async Task<HandlerResult> DoHandle(UpdateProductCommand request)
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

            return Ok();
        }
    }
}