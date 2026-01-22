namespace Restaurant.Data.Repository
{
    public class TafelLijstRepository : GenericRepository<TafelLijst>, ITafelLijstRepository
    {
        public TafelLijstRepository(RestaurantContext context) : base(context)
        {
        }

        public async Task<IEnumerable<TafelLijst>> GetByReservatieIdAsync(int reservatieId)
        {
            return await _context.TafelLijsten
                .Include(tl => tl.Tafel)
                .Where(tl => tl.ReservatieId == reservatieId)
                .ToListAsync();
        }

        public async Task<TafelLijst?> GetByIdAsync(int id)
        {
            return await _context.TafelLijsten
                .Include(tl => tl.Tafel)
                .FirstOrDefaultAsync(tl => tl.Id == id);
        }
    }
}
