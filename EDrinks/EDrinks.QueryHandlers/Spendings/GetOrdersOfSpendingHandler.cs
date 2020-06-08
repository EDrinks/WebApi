using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EDrinks.Common;
using EDrinks.QueryHandlers.Model;

namespace EDrinks.QueryHandlers.Spendings
{
    public class GetOrdersOfSpendingQuery : IQueryRequest<List<Order>>
    {
        public Guid SpendingId { get; set; }
    }
    
    public class GetOrdersOfSpendingHandler : QueryHandler<GetOrdersOfSpendingQuery, List<Order>>
    {
        private readonly IDataContext _dataContext;

        public GetOrdersOfSpendingHandler(IReadModel readModel, IDataContext dataContext) : base(readModel)
        {
            _dataContext = dataContext;
        }

        protected override Task<HandlerResult<List<Order>>> DoHandle(GetOrdersOfSpendingQuery request)
        {
            if (_dataContext.Spendings.All(e => e.Id != request.SpendingId))
            {
                return Task.FromResult(NotFound());
            }
            
            var orders = _dataContext.AllOrders.Where(e => e.SpendingId == request.SpendingId).ToList();
            return Task.FromResult(Ok(orders));
        }
    }
}