using System;
using System.Linq;
using System.Threading.Tasks;
using EDrinks.Common;
using EDrinks.QueryHandlers.Model;

namespace EDrinks.QueryHandlers
{
    public class GetProductQuery : IQueryRequest<Product>
    {
        public Guid Id { get; set; }
    }
    
    public class GetProductHandler : QueryHandler<GetProductQuery, Product>
    {
        private readonly IReadModel _readModel;

        public GetProductHandler(IReadModel readModel)
        {
            _readModel = readModel;
        }
        
        protected override async Task<HandlerResult<Product>> DoHandle(GetProductQuery request)
        {
            var product = (await _readModel.GetProducts()).FirstOrDefault(e => e.Id == request.Id);

            return product != null ? Ok(product) : NotFound();
        }
    }
}