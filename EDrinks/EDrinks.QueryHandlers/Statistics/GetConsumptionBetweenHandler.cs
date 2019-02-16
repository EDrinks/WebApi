using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EDrinks.Common;
using EDrinks.QueryHandlers.Model;

namespace EDrinks.QueryHandlers.Statistics
{
    public class GetConsumptionBetweenQuery : IQueryRequest<List<DataPoint>>
    {
        public Guid? ProductId { get; set; }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }
    }

    public class GetConsumptionBetweenHandler : QueryHandler<GetConsumptionBetweenQuery, List<DataPoint>>
    {
        private readonly IDataContext _dataContext;

        public GetConsumptionBetweenHandler(IReadModel readModel, IDataContext dataContext) : base(readModel)
        {
            _dataContext = dataContext;
        }

        protected override async Task<HandlerResult<List<DataPoint>>> DoHandle(GetConsumptionBetweenQuery request)
        {
            if (request.Start > request.End)
            {
                return Error("Start date must be smaller than end date");
            }

            if ((request.End - request.Start).Days > 90)
            {
                return Error("Maximum of 90 days are allowed between start and end date");
            }

            var dataPoints = new List<DataPoint>();
            DateTime currentDate = request.Start.Date;
            while (currentDate <= request.End)
            {
                var date = currentDate;
                var quantity = _dataContext.AllOrders
                    .Where(e => (e.DateTime >= date && e.DateTime < date.AddHours(1)) &&
                                (request.ProductId == null || e.ProductId == request.ProductId))
                    .Sum(e => e.Quantity);

                dataPoints.Add(new DataPoint()
                {
                    Label = date.ToString("yyyy-MM-dd HH:mm")
                            + "-" + date.AddHours(1).ToString("HH:mm"),
                    Value = quantity
                });

                currentDate = currentDate.AddHours(1);
            }

            return Ok(dataPoints);
        }
    }
}