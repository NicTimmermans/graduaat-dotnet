using Restaurant.Data.Repository.Interfaces;

namespace Restaurant.Data.Repository.Classes;

public class LogRepository : GenericRepository<Log>, ILogRepository
{
    public LogRepository(RestaurantContext context) : base(context){}
    

}