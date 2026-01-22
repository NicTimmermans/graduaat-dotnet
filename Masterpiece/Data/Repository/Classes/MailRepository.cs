using Restaurant.Data.Repository.Interfaces;

namespace Restaurant.Data.Repository.Classes
{
    public class MailRepository: GenericRepository<Mail>, IMailRepository
    {
        public MailRepository(RestaurantContext context) : base(context){}


    }
}
