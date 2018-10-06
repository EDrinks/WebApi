using System.Threading.Tasks;
using EDrinks.Common;
using EDrinks.QueryHandlers.Model;

namespace EDrinks.QueryHandlers.Settlements
{
    public class GetCurrentSettlementQuery : IQueryRequest<Settlement>
    {
    }
    
    public class GetCurrentSettlementHandler : QueryHandler<GetCurrentSettlementQuery, Settlement>
    {
        private readonly IDataContext _dataContext;

        public GetCurrentSettlementHandler(IReadModel readModel, IDataContext dataContext) : base(readModel)
        {
            _dataContext = dataContext;
        }
        
        protected override async Task<HandlerResult<Settlement>> DoHandle(GetCurrentSettlementQuery request)
        {
            return Ok(_dataContext.CurrentSettlement);
        }
    }
}