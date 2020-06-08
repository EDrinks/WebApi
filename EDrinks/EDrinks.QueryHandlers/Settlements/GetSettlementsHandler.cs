using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EDrinks.Common;
using EDrinks.QueryHandlers.Model;

namespace EDrinks.QueryHandlers.Settlements
{
    public class GetSettlementsQuery : IQueryRequest<List<Settlement>>
    {
        public DateTime Start { get; set; } = DateTime.MinValue;

        public DateTime End { get; set; } = DateTime.MaxValue;

        public int PageSize { get; set; } = 25;

        public int Offset { get; set; } = 0;
    }
    
    public class GetSettlementsHandler : QueryHandler<GetSettlementsQuery, List<Settlement>>
    {
        private readonly IReadModel _readModel;
        private readonly IDataContext _dataContext;

        public GetSettlementsHandler(IReadModel readModel, IDataContext dataContext) : base(readModel)
        {
            _readModel = readModel;
            _dataContext = dataContext;
        }
        
        protected override Task<HandlerResult<List<Settlement>>> DoHandle(GetSettlementsQuery request)
        {
            var errors = ValidateRequest(request);
            if (errors.Any()) return Task.FromResult(Error(errors));

            var settlements = _dataContext.Settlements
                .OrderByDescending(e => e.DateTime)
                .Where(e => e.DateTime >= request.Start && e.DateTime <= request.End)
                .Skip(request.Offset * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            return Task.FromResult(Ok(settlements));
        }
        
        private List<string> ValidateRequest(GetSettlementsQuery request)
        {
            var errorMessages = new List<string>();

            if (request.PageSize < 0 || request.PageSize > 100)
            {
                errorMessages.Add("Page size must be between 0 and 100");
            }

            if (request.Offset < 0)
            {
                errorMessages.Add("Offset must be positive");
            }

            if (request.End < request.Start)
            {
                errorMessages.Add("End date must come after start date");
            }

            return errorMessages;
        }
    }
}