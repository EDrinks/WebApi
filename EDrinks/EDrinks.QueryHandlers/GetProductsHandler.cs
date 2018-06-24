using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EDrinks.QueryHandlers.Model;
using MediatR;

namespace EDrinks.QueryHandlers
{
    public class GetProductsQuery : IRequest<List<Product>>
    {
    }

    public class GetProductsHandler : IRequestHandler<GetProductsQuery, List<Product>>
    {
        private readonly IReadModel _readModel;

        public GetProductsHandler(IReadModel readModel)
        {
            _readModel = readModel;
        }
        
        public async Task<List<Product>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            return _readModel.Products.Values.ToList();
        }
    }
}