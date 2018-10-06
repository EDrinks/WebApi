using System;
using System.Linq;
using System.Threading.Tasks;
using EDrinks.Common;
using EDrinks.QueryHandlers.Model;

namespace EDrinks.QueryHandlers.Settlements
{
    public class GetSettlementQuery : IQueryRequest<Settlement>
    {
        public Guid SettlementId { get; set; }
    }
    
    public class GetSettlementHandler : QueryHandler<GetSettlementQuery, Settlement>
    {
        private readonly IReadModel _readModel;

        public GetSettlementHandler(IReadModel readModel) : base(readModel)
        {
            _readModel = readModel;
        }
        
        protected override async Task<HandlerResult<Settlement>> DoHandle(GetSettlementQuery request)
        {
            var settlements = await _readModel.GetSettlements();

            var settlement = settlements.FirstOrDefault(e => e.Id == request.SettlementId);

            return settlement == null ? NotFound() : Ok(settlement);
        }
    }
}