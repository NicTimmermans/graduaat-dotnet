using Restaurant.Data.Repository.Interfaces;

namespace Restaurant.Data.Repository.Classes;

public class UserRepository : GenericRepository<CustomUser>, IUserRepository
{
    private readonly RestaurantContext _context;

    public UserRepository(RestaurantContext context) : base(context)
    {
        _context = context;
    }

    public async Task<CustomUser?> GetByIdAsync(string id)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<CustomUser?> GetByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }
}