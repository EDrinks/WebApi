using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EDrinks.Events.Products;
using EventStore.ClientAPI;
using MediatR;
using Newtonsoft.Json;

namespace EDrinks.CommandHandlers
{
    public class CreateProductCommand : IRequest<bool>
    {
        public string Name { get; set; }

        public decimal Price { get; set; }
    }

    public class CreateProductHandler : IRequestHandler<CreateProductCommand, bool>
    {
        public async Task<bool> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var productId = Guid.NewGuid();
            
            var connection = EventStoreConnection.Create(new IPEndPoint(IPAddress.Loopback, 1113));
            await connection.ConnectAsync();

            var jsonString = JsonConvert.SerializeObject(new ProductCreated()
            {
                ProductId = productId
            });
            var myEvent = new EventData(Guid.NewGuid(), "ProductCreatedEvent", true, 
                Encoding.UTF8.GetBytes(jsonString), null);
            await connection.AppendToStreamAsync("edrinks", ExpectedVersion.Any, myEvent);

            return true;
        }
    }
}