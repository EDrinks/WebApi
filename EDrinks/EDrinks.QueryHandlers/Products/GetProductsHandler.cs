using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EDrinks.Common;
using EDrinks.QueryHandlers.Model;

namespace EDrinks.QueryHandlers.Products
{
    public class GetProductsQuery : IQueryRequest<List<Product>>
    {
    }

    public class GetProductsHandler : QueryHandler<GetProductsQuery, List<Product>>
    {
        private readonly IDataContext _dataContext;

        public GetProductsHandler(IReadModel readModel, IDataContext dataContext) : base(readModel)
        {
            _dataContext = dataContext;
        }

        protected override Task<HandlerResult<List<Product>>> DoHandle(GetProductsQuery request)
        {
            return Task.FromResult(Ok(_dataContext.Products.OrderBy(e => e.Name)
                .ToList()));
        }
    }
}