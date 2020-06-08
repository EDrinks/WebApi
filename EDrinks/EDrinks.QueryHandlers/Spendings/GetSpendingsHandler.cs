using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EDrinks.Common;
using EDrinks.QueryHandlers.Model;

namespace EDrinks.QueryHandlers.Spendings
{
    public class GetSpendingsQuery : IQueryRequest<List<Spending>>
    {
    }

    public class GetSpendingsHandler : QueryHandler<GetSpendingsQuery, List<Spending>>
    {
        private readonly IDataContext _dataContext;

        public GetSpendingsHandler(IReadModel readModel, IDataContext dataContext) : base(readModel)
        {
            _dataContext = dataContext;
        }

        protected override Task<HandlerResult<List<Spending>>> DoHandle(GetSpendingsQuery request)
        {
            return Task.FromResult(Ok(_dataContext.Spendings.Where(e => e.Current < e.Quantity)
                .ToList()));
        }
    }
}