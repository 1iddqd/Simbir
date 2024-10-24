using Microsoft.AspNetCore.Mvc;

namespace timetable_service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorsController : ControllerBase
    {
        private readonly ILogger<DoctorsController> _logger;

        public DoctorsController(ILogger<DoctorsController> logger)
        {
            _logger = logger;
        }

        private const int SuccessStatusCode = 0;

        [HttpDelete("Doctor/{id}")]
        public IActionResult DeleteDoctorTimetable(int id)
        {
            return Ok(SuccessStatusCode);
        }

        [HttpGet("Doctor/{id}")]
        public IActionResult GetDoctorTimetable(int id)
        {
            return Ok(SuccessStatusCode);
        }
    }
}
