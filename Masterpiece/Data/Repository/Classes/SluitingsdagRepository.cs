using Restaurant.Data.Repository.Interfaces;

namespace Restaurant.Data.Repository.Classes
{
    public class SluitingsdagRepository : GenericRepository<Sluitingsdag>, ISluitingsdagRepository
    {
        public SluitingsdagRepository(RestaurantContext context) : base(context)
        {
        }

       

        public async Task<IEnumerable<Sluitingsdag>> GetAllAsync()
            => await _context.Sluitingsdagen.ToListAsync();

    }
}
