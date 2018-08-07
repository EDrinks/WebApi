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
    
    public class GetTabsHandler: QueryHandler<GetTabsQuery, List<Tab>>
    {
        private readonly IReadModel _readModel;

        public GetTabsHandler(IReadModel readModel)
        {
            _readModel = readModel;
        }
        
        protected override async Task<HandlerResult<List<Tab>>> DoHandle(GetTabsQuery request)
        {
            return Ok((await _readModel.GetTabs()).OrderBy(e => e.Name).ToList());
        }
    }
}