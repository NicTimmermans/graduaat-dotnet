using Restaurant.Data.Repository.Interfaces;

namespace Restaurant.Data.Repository.Classes
{
    public class TijdslotRepository : GenericRepository<Tijdslot>, ITijdslotRepository
    {
        public TijdslotRepository(RestaurantContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Tijdslot>> GetAllAsync()
       => await _context.Tijdslots.ToListAsync();

        public async Task<Tijdslot?> GetByIdAsync(int id)
            => await _context.Tijdslots.FindAsync(id);

        public async Task<IEnumerable<Tijdslot>> GetActieveTijdsloten()
            => await _context.Tijdslots.Where(t => t.Actief).ToListAsync();
    }
}
