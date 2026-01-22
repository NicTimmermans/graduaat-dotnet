namespace Restaurant.Data.Repository.Interfaces
{
    public interface IParameterRepository : IGenericRepository<Parameter>
    {
       
        Task<Parameter?> GetByNameAsync(string naam);

    }
}
