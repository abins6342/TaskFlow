using User_Service.DTOs;
using User_Service.Entities;
using User_Service.Interfaces;
using UserService.Helper;
using UserService.Interfaces;

namespace User_Service.Services
{
    public class UserServices : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly IPasswordHasher _hash;
        private readonly ITokenService _tokenService;
        public UserServices(
            IUserRepository repository, 
            IPasswordHasher hash,
            ITokenService tokenService)
        {
            _repository = repository;
            _hash = hash;
            _tokenService = tokenService;
        }

        public async Task<UserResponseDto> RegisterUser(RegisterUserDto registerUserDto)
        {
            var userExists = await _repository.GetByEmail(registerUserDto.Email);
            if (userExists != null)
            {
                throw new Exception("User already exists");
            }

            var hashedPass = _hash.Hash(registerUserDto.Password);

            var user = new User()
            {
                Id = Guid.NewGuid(),
                Username = registerUserDto.Username,
                Email = registerUserDto.Email,
                PasswordHash = hashedPass
,
                CreatedAt = DateTime.UtcNow,
            };

            await _repository.AddUser(user);

            return new UserResponseDto()
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
            };
        }

        public async Task<string> Login(LoginDto loginDto)
        {
            var user = await _repository.GetByEmail(loginDto.Email);
            if(user == null || !_hash.Verify(loginDto.Password, user.PasswordHash))
            {
                throw new Exception("Invalid Credentials");
            }

            return _tokenService.GenerateToken(user);
        }
    }
}
