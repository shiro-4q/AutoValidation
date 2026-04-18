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
    }
}
