using Restaurant.Data.Repository.Interfaces;

namespace Restaurant.Data.Repository.Classes
{
    public class TafelRepository : GenericRepository<Tafel>, ITafelRepository
    {
        public TafelRepository(RestaurantContext context) : base(context) { }

        public async Task<IEnumerable<Tafel>> GetAllAsync()
            => await _context.Tafels.ToListAsync();

        public async Task<IEnumerable<Tafel>> GetActiveAsync()
            => await _context.Tafels.Where(t => t.Actief).ToListAsync();

        public async Task<Tafel?> GetByIdAsync(int id)
            => await _context.Tafels.FindAsync(id);

        public override async Task AddAsync(Tafel tafel)
        {
            // Id genereren zoals je eerst had
            if (tafel.Id == 0)
            {
                var maxId = await _context.Tafels
                    .Select(t => (int?)t.Id)
                    .MaxAsync() ?? 0;

                tafel.Id = maxId + 1;
            }

            // fallback als er toch geen TafelNummer werd ingevuld
            if (string.IsNullOrWhiteSpace(tafel.TafelNummer))
            {
                var existingNumbers = await _context.Tafels
                    .Where(t => t.TafelNummer != null)
                    .Select(t => t.TafelNummer!)
                    .ToListAsync();

                int maxNum = 0;

                foreach (var txt in existingNumbers)
                {
                    if (int.TryParse(txt.TrimStart('T', 't'), out var parsed) && parsed > maxNum)
                        maxNum = parsed;
                }

                tafel.TafelNummer = $"T{maxNum + 1:00}";
            }

            await _context.Tafels.AddAsync(tafel);
        }

        public async Task<List<DateTime>> GetBeschikbareData(int aantalPersonen)
        {
            var today = DateTime.Today;
            var maxDays = 50;
            var result = new List<DateTime>();

            int totaleCapaciteit = await _context.Tafels
                .Where(t => t.Actief)
                .SumAsync(t => t.AantalPersonen);

            // marge van 10 plaatsen
            totaleCapaciteit -= 10;

            for (int i = 0; i <= maxDays; i++)
            {
                var datum = today.AddDays(i);

                bool isGesloten = await _context.Sluitingsdagen.AnyAsync(s => s.Datum == datum);
                if (isGesloten) continue;

                if (aantalPersonen <= totaleCapaciteit)
                    result.Add(datum);
            }

            return result;
        }

        public async Task<bool> HasLinkedReservatiesAsync(int tafelId)
        {
            // gebruik Set<TafelLijst> i.p.v. niet-bestaande _context.Tafellijsten
            return await _context.Set<TafelLijst>()
                                 .AnyAsync(tl => tl.TafelId == tafelId);
        }


    }
}
