using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EDrinks.Common;
using EDrinks.QueryHandlers.Model;

namespace EDrinks.QueryHandlers.Orders
{
    public class GetOrdersOfTabQuery : IQueryRequest<List<Order>>
    {
        public Guid TabId { get; set; }
    }

    public class GetOrdersOfTabHandler : QueryHandler<GetOrdersOfTabQuery, List<Order>>
    {
        private readonly IDataContext _dataContext;

        public GetOrdersOfTabHandler(IReadModel readModel, IDataContext dataContext) : base(readModel)
        {
            _dataContext = dataContext;
        }

        protected override Task<HandlerResult<List<Order>>> DoHandle(GetOrdersOfTabQuery request)
        {
            return Task.FromResult(Ok(_dataContext.CurrentOrders.Where(e => e.TabId == request.TabId)
                .ToList()));
        }
    }
}