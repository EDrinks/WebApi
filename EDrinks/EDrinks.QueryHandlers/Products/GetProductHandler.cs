using System;
using System.Linq;
using System.Threading.Tasks;
using EDrinks.Common;
using EDrinks.QueryHandlers.Model;

namespace EDrinks.QueryHandlers.Products
{
    public class GetProductQuery : IQueryRequest<Product>
    {
        public Guid Id { get; set; }
    }

    public class GetProductHandler : QueryHandler<GetProductQuery, Product>
    {
        private readonly IDataContext _dataContext;

        public GetProductHandler(IReadModel readModel, IDataContext dataContext) : base(readModel)
        {
            _dataContext = dataContext;
        }

        protected override Task<HandlerResult<Product>> DoHandle(GetProductQuery request)
        {
            var product = _dataContext.Products.FirstOrDefault(e => e.Id == request.Id);

            return Task.FromResult(product != null ? Ok(product) : NotFound());
        }
    }
}