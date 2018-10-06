using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EDrinks.Common;
using EDrinks.Events;
using EDrinks.Events.Products;
using EDrinks.QueryHandlers.Model;

namespace EDrinks.QueryHandlers.Products
{
    public class GetProductsQuery : IQueryRequest<List<Product>>
    {
    }

    public class GetProductsHandler : QueryHandler<GetProductsQuery, List<Product>>
    {
        private Dictionary<Guid, Product> _products;

        public GetProductsHandler(IReadModel readModel) : base(readModel)
        {
            _products = new Dictionary<Guid, Product>();
        }

        protected override async Task<HandlerResult<List<Product>>> DoHandle(GetProductsQuery request)
        {
            return Ok(_products.Values.OrderBy(e => e.Name).ToList());
        }

        protected override void HandleEvent(BaseEvent baseEvent)
        {
            switch (baseEvent)
            {
                case ProductCreated pc:
                    var product = new Product();
                    product.Apply(pc);
                    _products.Add(pc.ProductId, product);
                    break;
                case ProductNameChanged pnc:
                    _products[pnc.ProductId].Apply(pnc);
                    break;
                case ProductPriceChanged pcc:
                    _products[pcc.ProductId].Apply(pcc);
                    break;
                case ProductDeleted pd:
                    _products.Remove(pd.ProductId);
                    break;
            }
        }
    }
}