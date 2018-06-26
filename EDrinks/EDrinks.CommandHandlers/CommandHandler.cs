using System.Threading;
using System.Threading.Tasks;
using EDrinks.Common;
using MediatR;

namespace EDrinks.CommandHandlers
{
    public abstract class CommandHandler<TRequest> : IRequestHandler<TRequest, HandlerResult>
        where TRequest : IRequest<HandlerResult>
    {
        public async Task<HandlerResult> Handle(TRequest request, CancellationToken cancellationToken)
        {
            return await DoHandle(request);
        }

        protected abstract Task<HandlerResult> DoHandle(TRequest request);
    }
}