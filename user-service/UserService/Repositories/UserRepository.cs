using Microsoft.EntityFrameworkCore;
using User_Service.Db;
using User_Service.Entities;
using User_Service.Interfaces;

namespace User_Service.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserDbContext _userDbContext;
        public UserRepository(UserDbContext userDbContext)
        {
            _userDbContext = userDbContext;
        }

        public async Task<User> GetByEmail(string email)
        {
            var result = await _userDbContext.Users.FirstOrDefaultAsync(user=>user.Email == email);
            return result;
        }

        public async Task AddUser(User user)
        {
            await _userDbContext.Users.AddAsync(user);
            await _userDbContext.SaveChangesAsync();
        }
    }
}
