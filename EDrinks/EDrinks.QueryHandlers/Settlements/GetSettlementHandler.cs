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
        private readonly IDataContext _dataContext;

        public GetSettlementHandler(IReadModel readModel, IDataContext dataContext) : base(readModel)
        {
            _dataContext = dataContext;
        }
        
        protected override async Task<HandlerResult<Settlement>> DoHandle(GetSettlementQuery request)
        {
            var settlement = _dataContext.Settlements.FirstOrDefault(e => e.Id == request.SettlementId);

            return settlement == null ? NotFound() : Ok(settlement);
        }
    }
}