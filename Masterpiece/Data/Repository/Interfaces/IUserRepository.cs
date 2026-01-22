namespace Restaurant.Data.Repository.Interfaces
{
    public interface IUserRepository 
    {
        Task<CustomUser?> GetByIdAsync(string id);
        Task<CustomUser?> GetByEmailAsync(string email);
    }
}
