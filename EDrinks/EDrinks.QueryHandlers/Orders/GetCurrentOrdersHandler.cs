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
        private readonly IDataContext _dataContext;

        public GetCurrentOrdersHandler(IReadModel readModel, IDataContext dataContext) : base(readModel)
        {
            _dataContext = dataContext;
        }
        
        protected override async Task<HandlerResult<List<Order>>> DoHandle(GetCurrentOrdersQuery request)
        {
            return Ok(_dataContext.CurrentOrders);
        }
    }
}