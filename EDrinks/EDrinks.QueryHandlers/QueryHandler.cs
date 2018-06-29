﻿using System.Threading;
using System.Threading.Tasks;
using EDrinks.Common;
using MediatR;

namespace EDrinks.QueryHandlers
{
    public abstract class QueryHandler<TRequest, TReturn> : IRequestHandler<TRequest, HandlerResult<TReturn>>
        where TRequest : IQueryRequest<TReturn>
    {
        public async Task<HandlerResult<TReturn>> Handle(TRequest request, CancellationToken cancellationToken)
        {
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

        protected HandlerResult<TReturn> Error()
        {
            return new HandlerResult<TReturn>()
            {
                ResultCode = ResultCode.Error
            };
        }
    }
}