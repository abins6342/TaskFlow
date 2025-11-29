using User_Service.DTOs;

namespace UserService.Interfaces
{
    public interface IUserValidator
    {
        void ValidateRegister(RegisterUserDto registerUserDto);
        void ValidateLogin(LoginDto loginDto);
    }
}
