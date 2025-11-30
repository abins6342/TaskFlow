
using User_Service.Db;

namespace UserService.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly UserDbContext _context;
        public UnitOfWork(UserDbContext context)
        {
            _context = context;
        }
        public Task SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
