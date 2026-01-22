using Restaurant.Data.Repository.Interfaces;

namespace Restaurant.Data.Repository.Classes
{
    public class ParameterRepository : GenericRepository<Parameter>, IParameterRepository
    {
        public ParameterRepository(RestaurantContext context) : base(context) { }
        
            public async Task<Parameter?> GetByNameAsync(string naam)
        {
            return await _context.Parameters.FirstOrDefaultAsync(p => p.Naam == naam);
        }

    }

}
