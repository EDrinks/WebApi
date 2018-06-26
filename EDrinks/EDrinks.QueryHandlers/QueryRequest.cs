using EDrinks.Common;
using MediatR;

namespace EDrinks.QueryHandlers
{
    public interface IQueryRequest<TReturn> : IRequest<HandlerResult<TReturn>>
    {
    }
}