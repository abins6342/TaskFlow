using User_Service.DTOs;
using User_Service.Entities;

namespace User_Service.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetByEmail(string email);
        Task AddUser(User user);
    }
}
