using System;
using System.Threading.Tasks;
using EDrinks.Common;
using EDrinks.Events.Products;
using EDrinks.EventSource;

namespace EDrinks.CommandHandlers.Products
{
    public class DeleteProductCommand : ICommandRequest
    {
        public Guid ProductId { get; set; }
    }
    
    public class DeleteProductHandler : CommandHandler<DeleteProductCommand>
    {
        private readonly IEventSourceFacade _eventSource;

        public DeleteProductHandler(IEventSourceFacade eventSource)
        {
            _eventSource = eventSource;
        }
        
        protected override async Task<HandlerResult> DoHandle(DeleteProductCommand request)
        {
            await _eventSource.WriteEvent(new ProductDeleted()
            {
                ProductId = request.ProductId
            });
            
            return Ok();
        }
    }
}