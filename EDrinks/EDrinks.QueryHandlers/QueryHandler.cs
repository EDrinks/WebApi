using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EDrinks.Common;
using MediatR;

namespace EDrinks.QueryHandlers
{
    public abstract class QueryHandler<TRequest, TReturn> : IRequestHandler<TRequest, HandlerResult<TReturn>>
        where TRequest : IQueryRequest<TReturn>
    {
        private readonly IReadModel _readModel;

        public QueryHandler(IReadModel readModel)
        {
            _readModel = readModel;
        }
        
        public async Task<HandlerResult<TReturn>> Handle(TRequest request, CancellationToken cancellationToken)
        {
            await _readModel.ApplyAllEvents();
            return await DoHandle(request);
        }

        protected abstract Task<HandlerResult<TReturn>> DoHandle(TRequest request);

        protected HandlerResult<TReturn> Ok(TReturn payload)
        {
            return new HandlerResult<TReturn>()
            {
                ResultCode = ResultCode.Ok,
                Payload = payload
            };
        }

        protected HandlerResult<TReturn> Error(IEnumerable<string> errorMessages)
        {
            return new HandlerResult<TReturn>()
            {
                ResultCode = ResultCode.Error,
                ErrorMessages = errorMessages.ToList()
            };
        }
        
        protected HandlerResult<TReturn> Error(string errorMessage)
        {
            return Error(new[] {errorMessage});
        }

        protected HandlerResult<TReturn> NotFound()
        {
            return new HandlerResult<TReturn>()
            {
                ResultCode = ResultCode.NotFound
            };
        }
    }
}