using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EDrinks.Common;
using EDrinks.QueryHandlers.Model;

namespace EDrinks.QueryHandlers
{
    public class GetProductsQuery : IQueryRequest<List<Product>>
    {
    }

    public class GetProductsHandler : QueryHandler<GetProductsQuery, List<Product>>
    {
        private readonly IReadModel _readModel;

        public GetProductsHandler(IReadModel readModel)
        {
            _readModel = readModel;
        }

        protected override async Task<HandlerResult<List<Product>>> DoHandle(GetProductsQuery request)
        {
            return Ok(_readModel.Products.Values.ToList());
        }
    }
}