using account_service.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

namespace account_service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorsController : ControllerBase
    {
        private readonly ILogger<DoctorsController> _logger;
        private AccountApplicationContext _dbcontext = new AccountApplicationContext();

        public DoctorsController(ILogger<DoctorsController> logger)
        {
            _logger = logger;
        }

        [Authorize]
        [HttpGet]
        public IActionResult DoctorsList()
        {
            var userName = User.Identity.Name;

            if (string.IsNullOrEmpty(userName))
            {
                return Unauthorized("Username not found in the token.");
            }

            var user = _dbcontext.Accounts.FirstOrDefault(x => x.username == userName);
            if (user == null)
            {
                return NotFound("User  not found.");
            }

            var doctors = _dbcontext.AccountsRoles
                .Where(ar => ar.roles.id == 3)
                .Select(ar => ar.accounts).ToList();

            if (doctors == null || !doctors.Any())
            {
                return NotFound("No doctors found.");
            }

            return Ok(doctors);
        }

        [Authorize]
        [HttpGet("{id}")]
        public IActionResult GetDoctorById(int id)
        {
            var userName = User.Identity.Name;

            if (string.IsNullOrEmpty(userName))
            {
                return Unauthorized("Username not found in the token.");
            }

            var user = _dbcontext.Accounts.FirstOrDefault(x => x.username == userName);
            if (user == null)
            {
                return NotFound("User  not found.");
            }

            var doctor = _dbcontext.AccountsRoles
                .Where(ar => ar.roles.id == 3 && ar.accounts.id == id)
                .Select(ar => ar.accounts).FirstOrDefault();

            if (doctor == null)
            {
                return NotFound("Doctor not found.");
            }

            return Ok(doctor);
        }
    }
}

