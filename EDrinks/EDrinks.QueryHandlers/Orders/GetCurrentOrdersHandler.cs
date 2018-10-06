using System.Collections.Generic;
using System.Threading.Tasks;
using EDrinks.Common;
using EDrinks.QueryHandlers.Model;

namespace EDrinks.QueryHandlers.Orders
{
    public class GetCurrentOrdersQuery : IQueryRequest<List<Order>>
    {
    }

    public class GetCurrentOrdersHandler : QueryHandler<GetCurrentOrdersQuery, List<Order>>
    {
        private readonly IReadModel _readModel;

        public GetCurrentOrdersHandler(IReadModel readModel) : base(readModel)
        {
            _readModel = readModel;
        }
        
        protected override async Task<HandlerResult<List<Order>>> DoHandle(GetCurrentOrdersQuery request)
        {
            return Ok(await _readModel.GetOrders());
        }
    }
}