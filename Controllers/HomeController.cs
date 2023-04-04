using ASP_202.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ASP_202.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Intro()
        {
            return View();
        }
        public IActionResult Scheme()
        {
            ViewBag.bagdata = "Data from Bag";
            ViewData["viewdata"] = "Data from ViewData";

            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Razor()
        {
            return View();
        }
        public IActionResult Model()
        {
            Models.Home.Model model = new()
            {
                Header = "Моделі",
                Title = "Передача моделі у представлення",
                Departments = new()
                {
                    "Department 1",
                    "Department 2",
                    "Department 3",
                    "Department 4",
                    "Department 5"
                },
                Products = new()
                {
                    new() { Name = "Викрутка",   Price=50.0     },
                    new() { Name = "Дриль",      Price = 1200   },
                    new() { Name = "Smart TV",   Price = 9990   },
                    new() { Name = "Headphones", Price = 849.50 },
                    new() { Name = "Планшет",    Price = 8800   },
                    new() { Name = "Наліпка :)", Price = 1.50   }
                }
            };           

            return View(model);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}