using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EDrinks.Common;
using EDrinks.QueryHandlers.Model;

namespace EDrinks.QueryHandlers.Statistics
{
    public class GetTopTenQuery : IQueryRequest<List<BarChartEntry>>
    {
        public Guid ProductId { get; set; }

        public bool Current { get; set; }
    }
    
    public class GetTopTenHandler : QueryHandler<GetTopTenQuery, List<BarChartEntry>>
    {
        private readonly IDataContext _dataContext;
        
        public GetTopTenHandler(IReadModel readModel, IDataContext dataContext) : base(readModel)
        {
            _dataContext = dataContext;
        }

        protected override async Task<HandlerResult<List<BarChartEntry>>> DoHandle(GetTopTenQuery request)
        {
            var orderCollection = request.Current ? _dataContext.CurrentOrders : _dataContext.AllOrders;
            
            var topTen = orderCollection.Where(e => e.ProductId == request.ProductId)
                .GroupBy(e => e.TabId)
                .Select(e => new BarChartEntry()
                {
                    Label = _dataContext.Tabs.First(tab => tab.Id == e.Key).Name,
                    Value = e.Sum(o => o.Quantity)
                })
                .OrderByDescending(e => e.Value)
                .Take(10)
                .ToList();
            
            return Ok(topTen);
        }
    }
}