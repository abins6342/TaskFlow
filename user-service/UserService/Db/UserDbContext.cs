using Microsoft.EntityFrameworkCore;
using User_Service.Entities;

namespace User_Service.Db
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
        {
            
        }
        public DbSet<User> Users { get; set; }
    }
}
