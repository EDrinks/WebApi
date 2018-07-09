using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EDrinks.Common;
using EDrinks.Events;
using EDrinks.Events.Products;
using EDrinks.EventSource;
using EDrinks.QueryHandlers.Model;
using EventStore.ClientAPI;

namespace EDrinks.QueryHandlers
{
    public interface IReadModel
    {
        Dictionary<Guid, Product> Products { get; }
    }

    public class ReadModel : IReadModel
    {
        private readonly IEventStoreConnection _connection;
        private readonly IStreamResolver _streamResolver;

        public Dictionary<Guid, Product> Products { get; set; }

        public ReadModel(IEventStoreConnection connection, IStreamResolver streamResolver)
        {
            _connection = connection;
            _streamResolver = streamResolver;
            
            Products = new Dictionary<Guid, Product>();
        }

        private async Task EventAppeared(BaseEvent evt)
        {
            switch (evt)
            {
                case ProductCreated pc:
                    var product = new Product();
                    product.Apply(pc);
                    Products.Add(pc.ProductId, product);
                    break;
                case ProductNameChanged pnc:
                    Products[pnc.ProductId].Apply(pnc);
                    break;
                case ProductPriceChanged pcc:
                    Products[pcc.ProductId].Apply(pcc);
                    break;
                case ProductDeleted pd:
                    Products.Remove(pd.ProductId);
                    break;
            }
        }
    }
}