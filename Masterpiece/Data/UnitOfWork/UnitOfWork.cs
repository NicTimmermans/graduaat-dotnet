

using Restaurant.Data.Repository.Classes;
using Restaurant.Data.Repository.Interfaces;

namespace Restaurant.Data.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly RestaurantContext _context;
        private ILogRepository logRepository;
        private IMailRepository mailRepository;

        private IMenuRepository menuRepository;
        private IBestellingRepository bestellingRepository;


        private IGenericRepository<Land> landRepository;
        private IGenericRepository<CategorieType> categorieTypeRepository;
        private IGenericRepository<PrijsProduct> prijsRepository;
        private IReservatieRepository _reservatieRepository;
        private ITafelRepository _tafelRepository;
        private ITijdslotRepository _tijdslotRepository;
        private ISluitingsdagRepository _sluitingsdagRepository;
        private IUserRepository _userRepository;
        private IParameterRepository _parameterRepository;
        private IProductRepository _productRepository;
        private ICategorieRepository _catergorieRepository;
        private ITafelLijstRepository _tafelLijstRepository;



        public UnitOfWork(RestaurantContext context)
        {
            _context = context;
        }

        public IMailRepository MailRepository
        {
            get
            {
                return mailRepository ??= new MailRepository(_context);
            }
        }

        public ILogRepository LogRepository
        {
            get
            {
                return logRepository ??= new LogRepository(_context);
            }
        }


        public IMenuRepository MenuRepository
        {
            get
            {
                return menuRepository ??= new MenuRepository(_context);
            }
        }

        public IBestellingRepository BestellingRepository
        {
            get
            {
                return bestellingRepository ??= new BestellingRepository(_context);
            }
        }



        public IGenericRepository<Land> LandRepository
        {
            get
            {
                return landRepository ??= new GenericRepository<Land>(_context);
            }
        }
 
        public IGenericRepository<CategorieType> CategorieTypeRepository
        {
            get
            {
                return categorieTypeRepository ??= new GenericRepository<CategorieType>(_context);
            }
        }
        public IGenericRepository<PrijsProduct> PrijsRepository
        {
            get
            {
                return prijsRepository ??= new GenericRepository<PrijsProduct>(_context);
            }
        }

        public IProductRepository ProductRepository
        {
            get
            {
                return _productRepository ??= new ProductRepository(_context);
            }
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public IReservatieRepository ReservatieRepository
            => _reservatieRepository ??= new ReservatieRepository(_context);

        public ITafelRepository TafelRepository
            => _tafelRepository ??= new TafelRepository(_context);

        public ITijdslotRepository TijdslotRepository
            => _tijdslotRepository ??= new TijdslotRepository(_context);

        public ISluitingsdagRepository SluitingsdagRepository
            => _sluitingsdagRepository ??= new SluitingsdagRepository(_context);

        public IUserRepository UserRepository
            => _userRepository ??= new UserRepository(_context);

        public IParameterRepository ParameterRepository
        => _parameterRepository ??= new ParameterRepository(_context);
        public ICategorieRepository CategorieRepository
        => _catergorieRepository ??= new CategorieRepository(_context);

        public ITafelLijstRepository TafelLijstRepository
    => _tafelLijstRepository ??= new TafelLijstRepository(_context);


        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();

        }

        public void RemoveEntity<TEntity>(TEntity entity) where TEntity : class
        {
            _context.Set<TEntity>().Remove(entity);
        }
    }
}
