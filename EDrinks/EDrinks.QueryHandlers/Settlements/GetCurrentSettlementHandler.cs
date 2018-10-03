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
        private readonly IReadModel _readModel;

        public GetCurrentSettlementHandler(IReadModel readModel)
        {
            _readModel = readModel;
        }
        
        protected override async Task<HandlerResult<Settlement>> DoHandle(GetCurrentSettlementQuery request)
        {
            return Ok(await _readModel.GetCurrentSettlement());
        }
    }
}