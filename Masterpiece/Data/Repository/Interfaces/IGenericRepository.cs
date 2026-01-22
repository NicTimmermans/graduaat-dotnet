

namespace Restaurant.Data.Repository
{
    public interface IGenericRepository <TEntity> where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity?> GetByIdAsync<T>(T id);
	    Task AddAsync(TEntity entity);
	    void Update(TEntity entity);
        void Delete(TEntity entity);
        Task<IList<TEntity>> Find(Expression<Func<TEntity, bool>>? voorwaarden,
            params Expression<Func<TEntity, object>>[]? includes);
    }
}
