namespace Restaurant.Data.Repository
{ 
    public interface ITafelLijstRepository : IGenericRepository<TafelLijst>
    {
        Task<IEnumerable<TafelLijst>> GetByReservatieIdAsync(int reservatieId);
        Task<TafelLijst?> GetByIdAsync(int id);
    }
}
