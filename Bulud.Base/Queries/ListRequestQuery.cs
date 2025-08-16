using MediatR;

namespace Bulud.Base.Queries;

public class ListRequestQuery<T> : RequestQuery, IRequest<ListResult<T>> where T : class
{
    
}