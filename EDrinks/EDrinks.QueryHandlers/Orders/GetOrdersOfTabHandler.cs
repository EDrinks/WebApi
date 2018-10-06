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
        private readonly IReadModel _readModel;

        public GetOrdersOfTabHandler(IReadModel readModel) : base(readModel)
        {
            _readModel = readModel;
        }

        protected override async Task<HandlerResult<List<Order>>> DoHandle(GetOrdersOfTabQuery request)
        {
            var allOrders = await _readModel.GetOrders();

            return Ok(allOrders.Where(e => e.TabId == request.TabId).ToList());
        }
    }
}