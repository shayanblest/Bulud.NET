using MediatR;

namespace Bulud.EntityFrameworkCore.Queries;

public class ListRequestQuery<T> : RequestQuery, IRequest<ListResult<T>> where T : class
{
    
}
