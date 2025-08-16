using Bulud.Base.Queries;

namespace Bulud.Base.Infrastructure
{
    public interface IRepository<TEntity> where TEntity : class
    {
        Task<TEntity> Find(object id, RequestQuery? requestQuery = null);
        Task<ListResult<TEntity>> Get(RequestQuery query);
        Task AddAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);
        Task DeleteAsync(TEntity entity);
        Task<bool> ExistsAsync(Guid id);
    }
}
