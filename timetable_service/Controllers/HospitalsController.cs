using Microsoft.AspNetCore.Mvc;

namespace timetable_service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HospitalsController : ControllerBase
    {
        private readonly ILogger<HospitalsController> _logger;

        public HospitalsController(ILogger<HospitalsController> logger)
        {
            _logger = logger;
        }

        private const int SuccessStatusCode = 0;

        [HttpDelete("Hospital/{id}")]
        public IActionResult DeleteHospitalTimetable(int id)
        {
            return Ok(SuccessStatusCode);
        }

        [HttpGet("Hospital/{id}")]
        public IActionResult GetHospitalTimetable(int id)
        {
            return Ok(SuccessStatusCode);
        }

        [HttpGet("Hospital/{id}/Room/{room}")]
        public IActionResult GetRoomTimetable(int id, int room)
        {
            return Ok(SuccessStatusCode);
        }

    }
}
