using User_Service.DTOs;
using UserService.Common;

namespace User_Service.Interfaces
{
    public interface IUserService
    {
        Task<ServiceResponse<UserResponseDto>> RegisterUser(RegisterUserDto registerUserDto);

        Task<ServiceResponse<string>> Login(LoginDto loginDto);
    }
}
