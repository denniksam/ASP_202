using Microsoft.AspNetCore.Mvc;

namespace ASP_202.Controllers
{
    // [Route("User")]
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Registration()
        {
            return View();
        }
    }
}
