using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using recruitlab.server.Services.Interface;
using Server.Data;
using Server.Model.DTO;
using Server.Model.Entities;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var (success, message, data) = await _userService.LoginAsync(dto);

            if (!success)
            {
                return Unauthorized(new { message });
            }
            return Ok(data);
        }

        [HttpPost("register-candidate")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterCandidate([FromBody] CandidateSelfRegisterDto dto)
        {
            var (success, message) = await _userService.RegisterCandidateAsync(dto);

            if (!success)
            {
                return BadRequest(new { message });
            }
            return Ok(new { message });
        }

        [HttpPost("verify-otp")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDto dto)
        {
            var (success, message, data) = await _userService.VerifyCandidateEmailAsync(dto);

            if (!success)
            {
                return BadRequest(new { message });
            }
            return Ok(data);
        }

        [HttpPost("create-candidate-by-recruiter")]
        [Authorize(Roles = "Admin,Recruiter")]
        public async Task<IActionResult> CreateCandidateByRecruiter([FromBody] RecruiterCreateCandidateDto dto)
        {
            var recruiterId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var (success, message) = await _userService.CreateCandidateByRecruiterAsync(dto, recruiterId);

            if (!success)
            {
                return BadRequest(new { message });
            }
            return Ok(new { message });
        }

        [HttpPost("set-password")]
        [AllowAnonymous]
        public async Task<IActionResult> SetPassword([FromBody] SetPasswordDto dto)
        {
            var (success, message) = await _userService.SetPasswordFromTokenAsync(dto);
            if (!success)
            {
                return BadRequest(new { message });
            }
            return Ok(new { message });
        }
    }
}
