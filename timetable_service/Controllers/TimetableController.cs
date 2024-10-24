using Microsoft.AspNetCore.Mvc;

namespace timetable_service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimetableController : ControllerBase
    {
        private readonly ILogger<TimetableController> _logger;

        public TimetableController(ILogger<TimetableController> logger)
        {
            _logger = logger;
        }

        private const int SuccessStatusCode = 0;

        [HttpPost]
        public IActionResult CreateNewTimetable()
        {
            return Ok(SuccessStatusCode);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateTimetable(int id) 
        { 
            return Ok(SuccessStatusCode); 
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteTimetable(int id) 
        { 
            return Ok(SuccessStatusCode); 
        }

        [HttpGet("{id}/Appointments")]
        public IActionResult GetAppointments(int id)
        {
            return Ok(SuccessStatusCode);
        }

        [HttpPost("{id}/Appointments")]
        public IActionResult CreateAppointment(int id)
        {
            return Ok(SuccessStatusCode);
        }

        [HttpDelete("Appointment/{id}")]
        public IActionResult DeleteAppointment(int id)
        {
            return Ok(SuccessStatusCode);
        }
    }
}
