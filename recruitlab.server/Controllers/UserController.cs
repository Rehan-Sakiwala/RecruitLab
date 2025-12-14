using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using recruitlab.server.Data;

namespace recruitlab.server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Recruiter")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("interviewers")]
        public async Task<IActionResult> GetInterviewers()
        {
            var interviewers = await _context.Users
                .Include(u => u.Role)
                .Where(u => u.Role.Name == "Interviewer" || u.Role.Name == "HR")
                .Select(u => new
                {
                    u.Id,
                    Name = $"{u.FirstName} {u.LastName}",
                    Email = u.Email,
                    Role = u.Role.Name
                })
                .ToListAsync();

            return Ok(interviewers);
        }

        [HttpGet("staff")]
        public async Task<IActionResult> GetStaff()
        {
            var staff = await _context.Users
                .Include(u => u.Role)
                .Where(u => u.Role.Name == "Interviewer" || u.Role.Name == "HR")
                .Select(u => new
                {
                    u.Id,
                    Name = $"{u.FirstName} {u.LastName}",
                    Role = u.Role.Name
                })
                .ToListAsync();

            return Ok(staff);
        }
    }
}
