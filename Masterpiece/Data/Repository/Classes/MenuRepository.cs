using Restaurant.Data.Repository.Interfaces;

namespace Restaurant.Data.Repository.Classes
{
    public class MenuRepository : GenericRepository<Product>, IMenuRepository
    {
        public MenuRepository(RestaurantContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            return await _context.Producten.Where(p => p.Actief).Include(x => x.PrijsProducten).Include(x => x.Categorie).ThenInclude(x => x.Type).ToListAsync();
        }
        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _context.Producten.Include(x => x.PrijsProducten).FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task<IEnumerable<Product>> GetProductsByTypesAsync(List<CategorieType> types)
        {
            return await _context.Producten.Include(x => x.PrijsProducten).Include(x => x.Categorie).ThenInclude(x => x.Type).Where(x => types.Contains(x.Categorie.Type)).ToListAsync();
        }
    }
}
