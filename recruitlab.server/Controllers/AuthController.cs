using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Server.Data;
using Server.Model.DTO;
using Server.Model.Entities;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IRepository<User> userRepo;
        private readonly IRepository<Role> roleRepo;

        public AuthController(IRepository<User> userRepo, IRepository<Role> roleRepo)
        {
            this.userRepo = userRepo;
            this.roleRepo = roleRepo;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthDto model)
        {
            var users = await userRepo.GetAll();
            var user = users.Where(x => x.Email == model.Email).FirstOrDefault();

            if (user == null)
            {
                return new BadRequestObjectResult(new { message = "User not found! Please sign up!" });
            }

            if (user.Password != model.Password)
            {
                return new BadRequestObjectResult(new { message = "Password Incorrect! Please try again!" });
            }

            var roles = await roleRepo.GetAll();
            var role = roles.Where(r => r.Id == user.RoleId).FirstOrDefault();

            var token = GenerateToken(user.Email, role.Name);
            return Ok(new AuthTokenDto()
            {
                Id = user.Id,
                Email = user.Email,
                Role = user.Role.Name,
                Token = token
            });
        }

        private string GenerateToken(string email, string role)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("163f10144207a3d1950e6fb4b59a128e"));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.Name,email),
                new Claim(ClaimTypes.Role, role)
            };
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
             );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
