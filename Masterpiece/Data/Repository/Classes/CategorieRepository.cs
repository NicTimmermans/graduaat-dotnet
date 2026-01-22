using Restaurant.Data.Repository.Interfaces;

namespace Restaurant.Data.Repository.Classes
{
    public class CategorieRepository : GenericRepository<Categorie>, ICategorieRepository
    {
        public CategorieRepository(RestaurantContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Categorie>> GetCategorieAsync()
        {
            return await _context.Categorien.Include(c => c.Type).ToListAsync();
        }

        public async Task<Categorie?> GetCategorieMetProductenAsync(int id)
        {
            return await _context.Categorien
                .Include(c => c.Producten)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<bool> ExistsByNameAsync(string naam, int? excludeId = null)
        {
            return await _context.Categorien.AnyAsync(c =>
                c.Naam == naam &&
                (!excludeId.HasValue || c.Id != excludeId.Value));
        }
    }
}
