namespace Restaurant.Data.Repository.Interfaces
{
    public interface ICategorieRepository : IGenericRepository<Categorie>
    {
        Task<IEnumerable<Categorie>> GetCategorieAsync();
        Task<Categorie?> GetCategorieMetProductenAsync(int id);
        Task<bool> ExistsByNameAsync(string naam, int? excludeId = null);
    }
}
