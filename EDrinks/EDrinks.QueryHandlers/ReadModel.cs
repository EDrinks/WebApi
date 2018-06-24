using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EDrinks.Events;
using EDrinks.Events.Products;
using EDrinks.EventSource;
using EDrinks.QueryHandlers.Model;

namespace EDrinks.QueryHandlers
{
    public interface IReadModel
    {
        void Init();
        
        Dictionary<Guid, Product> Products { get; }
    }

    public class ReadModel : IReadModel
    {
        private readonly IEventSourceFacade _facade;

        public Dictionary<Guid, Product> Products { get; }

        public ReadModel(IEventSourceFacade facade)
        {
            _facade = facade;
            
            Products = new Dictionary<Guid, Product>();
        }

        public void Init()
        {
            _facade.Subscribe(EventAppeared);
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
            }
        }
    }
}