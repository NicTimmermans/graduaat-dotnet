namespace Restaurant.Data.Repository.Interfaces
{
    public interface IReservatieRepository : IGenericRepository<Reservatie>
    {
        Task AddAsync(Reservatie reservatie);
        Task<Reservatie?> GetByIdAsync(int id);
        Task<IEnumerable<Reservatie>> GetByDatumAsync(DateTime datum);

        Task<int> GetAantalPersonenGereserveerd(DateTime datum, int tijdslotId);

        Task<IEnumerable<Reservatie>> GetAllWithUserAndTijdslotAsync();
        Task<List<Reservatie>> GetAllActiveAsync();
        Task<Reservatie?> GetNextReservationForTableAsync(int tafelId, DateTime vanaf);

        Task<List<Reservatie>> GetReservatiesVoorDashboardAsync(string klantId);

        Task<IEnumerable<Reservatie>> GetAllWitEnqueteAsync();

        Task<IEnumerable<Reservatie>> GetAllWitEnqueteWithStarParamAsync(int param);


    }
}
