using Microsoft.AspNetCore.Mvc;

namespace AutoValidation.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class DemoController : ControllerBase
    {

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            return Ok(new { Message = "Login successful!" });
        }

        [HttpPost]
        public async Task<IActionResult> Login2(LoginRequest request)
        {
            return Ok(new { Message = "Login successful!" });
        }

        [HttpGet]
        public async Task<IActionResult> Login3(string? userName, string? password)
        {
            return Ok(new { Message = $"{userName} + {password}" });
        }
    }
}
