using User_Service.DTOs;
using User_Service.Entities;
using User_Service.Interfaces;
using UserService.Common;
using UserService.Helper;
using UserService.Infrastructure;
using UserService.Interfaces;

namespace User_Service.Services
{
    public class UserServices : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly IPasswordHasher _hash;
        private readonly ITokenService _tokenService;
        private readonly ILogger<UserServices> _logger;
        private readonly IUnitOfWork _unitOfWork;
        public UserServices(
            IUserRepository repository, 
            IPasswordHasher hash,
            ITokenService tokenService,
            ILogger<UserServices> logger,
            IUnitOfWork unitOfWork
            )
        {
            _repository = repository;
            _hash = hash;
            _tokenService = tokenService;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResponse<UserResponseDto>> RegisterUser(RegisterUserDto registerUserDto)
        {
            try
            {
                var userExists = await _repository.GetByEmail(registerUserDto.Email);
                if (userExists != null)
                {
                    return ServiceResponse<UserResponseDto>.Fail( "User already exist");
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
                await _unitOfWork.SaveChangesAsync();

                return ServiceResponse<UserResponseDto>.Ok(new UserResponseDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                },
                "User Registered successfully"
                );
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Unexpected error in regisering user email = {Email}", registerUserDto.Email);
                return ServiceResponse<UserResponseDto>.Fail("Something went wrong");
            }
            

        }

        public async Task<ServiceResponse<string>> Login(LoginDto loginDto)
        {
            try
            {
                var user = await _repository.GetByEmail(loginDto.Email);
                if (user == null || !_hash.Verify(loginDto.Password, user.PasswordHash))
                {
                    return ServiceResponse<string>.Fail("Invalid credentials.");
                }

                var token = _tokenService.GenerateToken(user);
                
                return ServiceResponse<string>.Ok(token, "Login successful");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in LoginAsync. Email {Email}", loginDto.Email);
                return ServiceResponse<string>.Fail("Something went wrong. Please try again.");
            }
            
        }
    }
}
