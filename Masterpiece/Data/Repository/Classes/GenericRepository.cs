using Restaurant.Data.Repository.Interfaces;

namespace Restaurant.Data.Repository
{
    public class GenericRepository <TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        protected readonly RestaurantContext _context;

        public GenericRepository(RestaurantContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _context.Set<TEntity>().ToListAsync();
        }

        public async Task<TEntity?> GetByIdAsync<T>(T id)
        {
            return await _context.Set<TEntity>().FindAsync(id);
        }

        public virtual async Task AddAsync(TEntity entity)
        {
            try
            {
                await _context.Set<TEntity>().AddAsync(entity);
            }
            catch (Exception e)
            {
                throw new Exception("" + e.Message);
            }
        }

        public void Update(TEntity entity)
        {
            _context.Set<TEntity>().Update(entity);
        }

        public void Delete(TEntity entity)
        {
            _context.Set<TEntity>().Remove(entity);
        }

        public async Task<IList<TEntity>> Find(Expression<Func<TEntity, bool>>? voorwaarden,
            params Expression<Func<TEntity, object>>[]? includes)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();
            if (includes != null)
            {
                foreach (var item in includes)
                {
                    query = query.Include(item);
                }
            }
            if (voorwaarden != null)
            {
                query = query.Where(voorwaarden);
            }
            return await query.ToListAsync();
        }

    }
}
