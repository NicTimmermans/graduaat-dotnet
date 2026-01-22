using Restaurant.Data.Repository.Interfaces;

namespace Restaurant.Data.Repository.Classes
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(RestaurantContext context) : base(context)
        { }

             public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Set<Product>().FindAsync(id);
        }

        public async Task UpdateAsync(Product product)
        {
            _context.Update(product);
            await _context.SaveChangesAsync();
        }
    }

    
}
