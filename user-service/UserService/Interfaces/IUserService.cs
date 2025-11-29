using User_Service.DTOs;

namespace User_Service.Interfaces
{
    public interface IUserService
    {
        Task<UserResponseDto> RegisterUser(RegisterUserDto registerUserDto);

        Task<string> Login(LoginDto loginDto);
    }
}
