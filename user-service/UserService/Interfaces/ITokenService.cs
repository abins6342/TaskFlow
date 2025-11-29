using User_Service.Entities;

namespace UserService.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}
