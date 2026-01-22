namespace Restaurant.Data.Repository.Interfaces
{
    public interface ITafelRepository : IGenericRepository<Tafel>
    {
        Task<IEnumerable<Tafel>> GetAllAsync();
        Task<IEnumerable<Tafel>> GetActiveAsync();
        Task<Tafel?> GetByIdAsync(int id);
        Task AddAsync(Tafel tafel);

        // nog nodig voor andere usecases
        Task<List<DateTime>> GetBeschikbareData(int aantalPersonen);

        // extra helper voor FK-check bij hard delete
        Task<bool> HasLinkedReservatiesAsync(int tafelId);
    }
}
