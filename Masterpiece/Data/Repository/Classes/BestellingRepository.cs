using Restaurant.Data.Repository.Interfaces;

namespace Restaurant.Data.Repository.Classes
{
    public class BestellingRepository : GenericRepository<Bestelling>, IBestellingRepository
    {
        public BestellingRepository(RestaurantContext context) : base(context)
        {
        }

        public async Task<List<Bestelling>> GetByReservatieAsync(int reservatieId, bool alleenActief = false)
        {
            var query = _context.Bestellingen
                .Include(p => p.Product)
                    .ThenInclude(p => p.PrijsProducten)
                .Include(b => b.Status)
                .Where(b => b.ReservatieId == reservatieId);

            if (alleenActief)
            {
                query = query.Where(b => (b.StatusId == 5)
                                         && b.Product.PrijsProducten.Any());
            }

            return await query.ToListAsync();
        }

        public async Task<List<Bestelling>> GetAllByReservatieAsync(int reservatieId)
        {
            var query = _context.Bestellingen
                .Include(p => p.Product)
                    .ThenInclude(p => p.PrijsProducten)
                .Include(b => b.Status);

            return await query.ToListAsync();
        }
        public async Task<List<Bestelling>> GetByReservatieViewModeAsync(int reservatieId, int ViewMode)
        {
            var query = _context.Bestellingen
                .Include(p => p.Product)
                    .ThenInclude(p => p.PrijsProducten)
                .Include(b => b.Status)
                .Where(b => b.ReservatieId == reservatieId);

            if (ViewMode == 1)
            {
                query = query.Where(b => (b.StatusId == 5)
                                         && b.Product.PrijsProducten.Any());
            }

            return await query.ToListAsync();
        }




        public async Task<Bestelling?> GetAsync(int id)
        {
            return await _context.Bestellingen
                .Include(b => b.Product)
                    .ThenInclude(p => p.PrijsProducten)
                .Include(b => b.Status)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public override async Task AddAsync(Bestelling bestelling)
        {
            bestelling.TijdstipBestelling = DateTime.Now;
            bestelling.StatusId = 5; // Alleen nieuw item
            await _context.Bestellingen.AddAsync(bestelling);
            await _context.SaveChangesAsync();
        }


        public async Task<decimal> GetTotalBestellenAsync(int reservatieId)
        {
            var items = await GetByReservatieAsync(reservatieId);

            return items
                .Where(x => x.StatusId == 5 && x.Product.PrijsProducten.Any())
                .Sum(b => b.Product.PrijsProducten
                            .OrderByDescending(pp => pp.DatumVanaf)
                            .First().Prijs * b.Aantal);
        }

        public async Task<decimal> GetTotalAfrekenenAsync(int reservatieId)
        {
            var items = await GetByReservatieAsync(reservatieId);

            return items
                .Where(x => x.StatusId == 3 && x.Product.PrijsProducten.Any())
                .Sum(b => b.Product.PrijsProducten
                            .OrderByDescending(pp => pp.DatumVanaf)
                            .First().Prijs * b.Aantal);
        }

        public async Task RemoveAsync(int id)
        {
            var item = await _context.Bestellingen.FindAsync(id);
            if (item == null) return;

            _context.Bestellingen.Remove(item);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAantalAsync(int id, int newAantal)
        {
            var item = await _context.Bestellingen.FindAsync(id);
            if (item == null) return;

            item.Aantal = newAantal;
            await _context.SaveChangesAsync();
        }


        public async Task ConfirmAsync(int reservatieId)
        {
            // Alleen bestellingen die nog "in bestelling" zijn (StatusId = 5)
            var bestellingen = await _context.Bestellingen
                .Where(b => b.ReservatieId == reservatieId && b.StatusId == 5)
                .ToListAsync();

            foreach (var item in bestellingen)
            {
                item.TijdstipBestelling = DateTime.Today.Add(DateTime.Now.TimeOfDay);
                item.StatusId = 6;
            }

            await _context.SaveChangesAsync();
        }


        public async Task<bool> ProductExistsAsync(int productId)
        {
            return await _context.Producten.AnyAsync(p => p.Id == productId);
        }

        public async Task<bool> BetalenAsync(int reservatieId, string betaalMethode, decimal? ontvangenBedrag = null)
        {
            var bestellingen = await GetByReservatieAsync(reservatieId);
            if (bestellingen == null || !bestellingen.Any())
                return false;

            var totaal = await GetTotalAfrekenenAsync(reservatieId);

            if (betaalMethode == "Payconiq")
            {
                var success = true;
                if (!success)
                    return false;
            }
            else if (betaalMethode == "Cash")
            {
                if (!ontvangenBedrag.HasValue || ontvangenBedrag.Value < totaal)
                    return false;
            }

            return true;
        }

        public async Task<List<Bestelling>> GetAllForKokAsync()
        {
            return await _context.Bestellingen
                .Include(b => b.Product)
                    .ThenInclude(p => p.Categorie)
                        .ThenInclude(c => c.Type)
                .Include(b => b.Product)
                    .ThenInclude(p => p.PrijsProducten)
                .Include(b => b.Status)
                .Include(b => b.Reservatie)
                    .ThenInclude(r => r.Tafellijsten)
                        .ThenInclude(tl => tl.Tafel)
                .Where(b =>
                    b.Product.Categorie.Type.Naam != "Dranken" &&
                    b.StatusId != 2 &&
                    b.StatusId != 3
                )
                .OrderBy(b => b.TijdstipBestelling)
                .ToListAsync();
        }

        public async Task<List<Bestelling>> GetAllForOberAsync()
        {
            return await _context.Bestellingen
                .Include(b => b.Product)
                    .ThenInclude(p => p.Categorie)
                        .ThenInclude(c => c.Type)
                .Include(b => b.Product)
                    .ThenInclude(p => p.PrijsProducten)
                .Include(b => b.Status)
                .Include(b => b.Reservatie)
                    .ThenInclude(r => r.Tafellijsten)
                        .ThenInclude(tl => tl.Tafel)
                .Where(b => b.StatusId != 3)
                .OrderBy(b => b.TijdstipBestelling)
                .ToListAsync();
        }


        public async Task UpdateStatusAsync(int bestellingId, int statusId)
        {
            var bestelling = await _context.Bestellingen.FindAsync(bestellingId);
            if (bestelling == null) return;

            bestelling.StatusId = statusId;
            await _context.SaveChangesAsync();
        }

        public async Task UpdateStatusAndNoteAsync(int bestellingId, int statusId, string opmerking)
        {
            var bestelling = await _context.Bestellingen.FindAsync(bestellingId);
            if (bestelling == null) return;

            bestelling.StatusId = statusId;
            bestelling.Opmerking = opmerking;
            await _context.SaveChangesAsync();
        }

        public async Task RemoveByReservatieAsync(int reservatieId)
        {
            var bestellingen = await _context.Bestellingen
                .Where(b => b.ReservatieId == reservatieId)
                .ToListAsync();

            if (!bestellingen.Any()) return;

            _context.Bestellingen.RemoveRange(bestellingen);
            await _context.SaveChangesAsync(); 
        }



    }
}
