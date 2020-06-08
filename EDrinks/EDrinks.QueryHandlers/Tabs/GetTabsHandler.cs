using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EDrinks.Common;
using EDrinks.QueryHandlers.Model;

namespace EDrinks.QueryHandlers.Tabs
{
    public class GetTabsQuery : IQueryRequest<List<Tab>>
    {
    }

    public class GetTabsHandler : QueryHandler<GetTabsQuery, List<Tab>>
    {
        private readonly IDataContext _dataContext;

        public GetTabsHandler(IReadModel readModel, IDataContext dataContext) : base(readModel)
        {
            _dataContext = dataContext;
        }

        protected override Task<HandlerResult<List<Tab>>> DoHandle(GetTabsQuery request)
        {
            return Task.FromResult(Ok(_dataContext.Tabs.OrderBy(e => e.Name).ToList()));
        }
    }
}