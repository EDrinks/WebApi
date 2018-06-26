using EDrinks.Common;
using MediatR;

namespace EDrinks.CommandHandlers
{
    public interface ICommandRequest : IRequest<HandlerResult>
    {
    }
}