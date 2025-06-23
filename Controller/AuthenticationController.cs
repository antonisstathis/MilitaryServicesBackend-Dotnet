using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using MilitaryServices.App.Dto;
using MilitaryServices.App.Services;
using MilitaryServices.App.Entity;
using MilitaryServices.App.Security;

namespace MilitaryServices.App.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IJwtUtil _jwtUtil;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IUserService _userService;
        private readonly IAuthorityService _authorityService;

        public AuthenticationController(
            IJwtUtil jwtUtil,
            IPasswordHasher<User> passwordHasher,
            IUserService userService,
            AuthorityService authorityService)
        {
            _jwtUtil = jwtUtil;
            _passwordHasher = passwordHasher;
            _userService = userService;
            _authorityService = authorityService;
        }

        [HttpPost("performLogin")]
        public IActionResult PerformLogin([FromBody] LoginRequest loginRequest)
        {
            var user = _userService.FindUser(loginRequest.Username);
            if (user == null)
                return Unauthorized("Invalid username");

            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, loginRequest.Password);
            if (result != PasswordVerificationResult.Success)
                return Unauthorized("Invalid password");

            var roles = _authorityService.FindRolesByUsername(user);
            var token = _jwtUtil.GenerateToken(loginRequest.Username, roles);

            return Ok(new JwtResponse { Token = token });
        }

        public class JwtResponse
        {
            public required string Token { get; set; }
        }
    }
}
