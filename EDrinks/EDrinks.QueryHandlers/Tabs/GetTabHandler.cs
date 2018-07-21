using System;
using System.Linq;
using System.Threading.Tasks;
using EDrinks.Common;
using EDrinks.QueryHandlers.Model;

namespace EDrinks.QueryHandlers.Tabs
{
    public class GetTabQuery : IQueryRequest<Tab>
    {
        public Guid TabId { get; set; }
    }

    public class GetTabHandler : QueryHandler<GetTabQuery, Tab>
    {
        private readonly IReadModel _readModel;

        public GetTabHandler(IReadModel readModel)
        {
            _readModel = readModel;
        }
        
        protected override async Task<HandlerResult<Tab>> DoHandle(GetTabQuery request)
        {
            Tab tab = (await _readModel.GetTabs()).FirstOrDefault(e => e.Id == request.TabId);

            if (tab == null)
            {
                return NotFound();
            }

            return Ok(tab);
        }
    }
}