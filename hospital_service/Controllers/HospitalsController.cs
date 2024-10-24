using Microsoft.AspNetCore.Mvc;

namespace document_service.Controllers
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

        [HttpGet]
        public IActionResult GetHospitals()
        {
            return Ok(SuccessStatusCode);
        }


        [HttpGet("{id}")]
        public IActionResult GetHospitalById(int id)
        {
            return Ok(SuccessStatusCode);
        }

        [HttpGet("{id}/Rooms")]
        public IActionResult GetHospitalRooms(int id)
        {
            return Ok(SuccessStatusCode);
        }

        [HttpPost]
        public IActionResult CreateHospital()
        {
            return Ok(SuccessStatusCode);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateHospital(int id)
        {
            return Ok(SuccessStatusCode);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteHospital(int id) 
        { 
            return Ok(SuccessStatusCode); 
        }
    }
}
