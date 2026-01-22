namespace Restaurant.Data.Repository.Interfaces
{
    public interface ISluitingsdagRepository : IGenericRepository<Sluitingsdag>
    {
        
        Task<IEnumerable<Sluitingsdag>> GetAllAsync();
    }
}
