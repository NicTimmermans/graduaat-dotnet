using Restaurant.Data.Repository.Interfaces;

namespace Restaurant.Data.Repository.Classes
{
    public class ReservatieRepository : GenericRepository<Reservatie>, IReservatieRepository
    {
        public ReservatieRepository(RestaurantContext context) : base(context) { }

        public async Task AddAsync(Reservatie reservatie)
            => await _context.Reservaties.AddAsync(reservatie);

        public async Task<Reservatie?> GetByIdAsync(int id)
            => await _context.Reservaties
                .Include(r => r.Tafellijsten)
                    .ThenInclude(r => r.Tafel)
                .FirstOrDefaultAsync(r => r.Id == id);

        public async Task<IEnumerable<Reservatie>> GetByDatumAsync(DateTime datum)
            => await _context.Reservaties
                .Include(r => r.Tafellijsten)
                .Where(r => r.Datum == datum)
                .ToListAsync();

        public async Task<int> GetAantalPersonenGereserveerd(DateTime datum, int tijdslotId)
        {
            return await _context.Reservaties
                .Where(r => r.Datum == datum && r.TijdSlotId == tijdslotId)
                .SumAsync(r => (int?)r.AantalPersonen) ?? 0;
        }

        //=======================
        //     BEHEER        
        //======================= 

        public async Task<IEnumerable<Reservatie>> GetAllWithUserAndTijdslotAsync()
        {
            return await _context.Reservaties
                .Include(r => r.CustomUser)
                .Include(r => r.Tijdslot)
                .Include(r => r.Tafellijsten)
                    .ThenInclude(tl => tl.Tafel)
                .ToListAsync();
        }
        public async Task<IEnumerable<Reservatie>> GetAllWitEnqueteAsync()
        {
            return await _context.Reservaties
                .Include(r => r.CustomUser)
                .Where(r => r.EvaluatieAantalSterren != 0)
                //.Where (r => r.EvaluatieOpmerkingen != null || r.EvaluatieAantalSterren != 0)
                .OrderByDescending (r => r.Datum)
                .ToListAsync();
        }
        public async Task<IEnumerable<Reservatie>> GetAllWitEnqueteWithStarParamAsync(int param)
        {
            return await _context.Reservaties
                .Include(r => r.CustomUser)
                .Where (r => r.EvaluatieAantalSterren >= param)
                .Where(r => r.CustomUser.Achternaam != "<>")
                //.Where (r => r.EvaluatieOpmerkingen != null || r.EvaluatieAantalSterren != 0)
                
                .ToListAsync();
        }


        // >>> CHANGE: aangepast voor tafels-beheer / grondplan / tafels-toewijzen
        // Voorheen:
        //   .Where(r => r.IsAanwezig == true)
        // Nu:
        //   "actief" = vandaag en toekomstige reservaties
        //   zodat de plattegrond alle relevante reservaties kan tonen.
        public async Task<List<Reservatie>> GetAllActiveAsync()
        {
            var today = DateTime.Today;

            return await _context.Reservaties
                .Include(r => r.Bestellingen)
                    .ThenInclude(b => b.Product)
                        .ThenInclude(p => p.PrijsProducten)
                .Include(r => r.Tafellijsten)
                    .ThenInclude(tl => tl.Tafel)
                .Include(r => r.CustomUser)
                .Where(r => r.Datum >= today)
                .ToListAsync();
        }
        // <<< END CHANGE

        // show off extra on editing bestelling on active table in tafels beheren
        public async Task<Reservatie?> GetNextReservationForTableAsync(int tafelId, DateTime vanaf)
        {
            return await _context.Reservaties
                .Include(r => r.Tafellijsten)
                    .ThenInclude(tl => tl.Tafel)
                .Include(r => r.CustomUser)
                .Where(r =>
                    r.Datum > vanaf &&
                    r.Tafellijsten.Any(tl => tl.TafelId == tafelId))
                .OrderBy(r => r.Datum)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Reservatie>> GetAllWithTafelsForKlantAsync(string klantId)
        {
            return await _context.Reservaties
                .Include(r => r.Tijdslot)
                .Include(r => r.Tafellijsten)
                    .ThenInclude(tl => tl.Tafel)
                .Where(r => r.KlantId == klantId && r.Datum >= DateTime.Today)
                .ToListAsync();
        }

        public async Task<List<Reservatie>> GetReservatiesVoorDashboardAsync(string klantId)
        {
            return await _context.Reservaties
                .Include(r => r.Tijdslot)
                .Include(r => r.Tafellijsten)
                    .ThenInclude(tl => tl.Tafel)
                .Where(r => r.KlantId == klantId)
                .OrderBy(r => r.Datum)
                .ToListAsync();
        }

       
    }
}
