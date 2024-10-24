using Microsoft.AspNetCore.Mvc;

namespace document_service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistoryController : ControllerBase
    {
        private readonly ILogger<HistoryController> _logger;


        public HistoryController(ILogger<HistoryController> logger)
        {
            _logger = logger;
        }


        private const int SuccessStatusCode = 0;

        [HttpGet("Account/{id}")]
        public IActionResult GetAccountHistory(int id)
        {
            return Ok(SuccessStatusCode);
        }

        [HttpGet("{id}")]
        public IActionResult GetInformation(int id)
        {
            return Ok(SuccessStatusCode);
        }

        [HttpPost]
        public IActionResult CreateInformation()
        {
            return Ok(SuccessStatusCode);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateInformation() 
        {
            return Ok(SuccessStatusCode);
        }
    }
}
