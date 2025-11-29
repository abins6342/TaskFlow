using Microsoft.AspNetCore.Mvc;
using User_Service.DTOs;
using User_Service.Interfaces;

namespace User_Service.Controllers
{
    [Route("api/v1/authenticate/")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> RegisterUserDto(RegisterUserDto registerUserDto)
        {
            var result = await _userService.RegisterUser(registerUserDto);
            return Ok(result);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var token = await _userService.Login(loginDto);
            return Ok(token);
        }
    }
}
