using System;
using System.Linq;
using System.Threading.Tasks;
using EDrinks.Common;
using EDrinks.QueryHandlers.Model;

namespace EDrinks.QueryHandlers.Spendings
{
    public class GetSpendingQuery : IQueryRequest<Spending>
    {
        public Guid SpendingId { get; set; }
    }
    
    public class GetSpendingHandler : QueryHandler<GetSpendingQuery, Spending>
    {
        private readonly IDataContext _dataContext;

        public GetSpendingHandler(IReadModel readModel, IDataContext dataContext) : base(readModel)
        {
            _dataContext = dataContext;
        }

        protected override async Task<HandlerResult<Spending>> DoHandle(GetSpendingQuery request)
        {
            var spending = _dataContext.Spendings.FirstOrDefault(e => e.Id == request.SpendingId);

            return spending != null ? Ok(spending) : NotFound();
        }
    }
}