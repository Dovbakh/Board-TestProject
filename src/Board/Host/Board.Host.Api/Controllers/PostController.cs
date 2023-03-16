using Microsoft.AspNetCore.Mvc;

namespace Board.Host.Api.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    public class PostController : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return Ok();
        }
    }
}
