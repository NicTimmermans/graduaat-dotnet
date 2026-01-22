

using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Restaurant.Data.Repository.Interfaces;

namespace Restaurant.Data.UnitOfWork
{
    public interface IUnitOfWork
    {
        IMailRepository MailRepository { get; }
        ILogRepository LogRepository { get; }
        IMenuRepository MenuRepository { get; }
        IBestellingRepository BestellingRepository { get; }

        public IGenericRepository<Land> LandRepository { get; }
        //public IGenericRepository<Categorie> CategorieRepository { get; }
        public IGenericRepository<CategorieType> CategorieTypeRepository { get; }
        public IGenericRepository<PrijsProduct> PrijsRepository { get; }
        public void SaveChanges();
        IReservatieRepository ReservatieRepository { get; }
        ITafelRepository TafelRepository { get; }
        ITijdslotRepository TijdslotRepository { get; }
        ISluitingsdagRepository SluitingsdagRepository { get; }
        IUserRepository UserRepository { get; }
        IParameterRepository ParameterRepository { get; }

        IProductRepository ProductRepository { get; }
        ICategorieRepository CategorieRepository { get; }

        ITafelLijstRepository TafelLijstRepository { get; }

        Task SaveChangesAsync();

        void RemoveEntity<TEntity>(TEntity entity) where TEntity : class;
    }
}
