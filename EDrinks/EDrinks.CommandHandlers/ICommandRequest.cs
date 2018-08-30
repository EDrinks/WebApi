using EDrinks.Common;
using MediatR;

namespace EDrinks.CommandHandlers
{
    public interface ICommandRequest : IRequest<HandlerResult>
    {
    }

    public interface ICommandRequest<TReturn> : IRequest<HandlerResult<TReturn>>
    {
    }
}