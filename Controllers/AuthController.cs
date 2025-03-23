using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using taskManagerAPI.Models;

namespace taskManagerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private List<User> users = new()
        {
            new User { Username = "admin", Password = "admin123", Role = "Admin" },
            new User { Username = "user", Password = "user123", Role = "User" }
        };

        [HttpPost("login")]
        public IActionResult Login([FromBody] User login)
        {
            var user = users.SingleOrDefault(u => u.Username == login.Username && u.Password == login.Password);
            if (user == null)
                return Unauthorized("Invalid credentials");

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("THIS_IS_MY_SUPER_DUPER_TOP_SECRET_KEY_12345!");


            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwt = tokenHandler.WriteToken(token);

            return Ok(new { token = jwt });
        }
    }
}
