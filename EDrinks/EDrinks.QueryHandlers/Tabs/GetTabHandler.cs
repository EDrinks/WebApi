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
        private readonly IDataContext _dataContext;

        public GetTabHandler(IReadModel readModel, IDataContext dataContext) : base(readModel)
        {
            _dataContext = dataContext;
        }
        
        protected override async Task<HandlerResult<Tab>> DoHandle(GetTabQuery request)
        {
            Tab tab = _dataContext.Tabs.FirstOrDefault(e => e.Id == request.TabId);

            if (tab == null)
            {
                return NotFound();
            }

            return Ok(tab);
        }
    }
}