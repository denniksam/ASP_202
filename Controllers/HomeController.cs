using ASP_202.Data;
using ASP_202.Models;
using ASP_202.Services;
using ASP_202.Services.Hash;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ASP_202.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly TimeService  _timeService;
        private readonly DateService  _dateService;
        private readonly DtService    _dtService;
        private readonly IHashService _hashService;
        private readonly DataContext  _dataContext;

        public HomeController(ILogger<HomeController> logger,
                              TimeService timeService,
                              DateService dateService,
                              DtService dtService,
                              IHashService hashService,
                              DataContext dataContext)
        {
            _logger = logger;
            _timeService = timeService;
            _dateService = dateService;
            _dtService = dtService;
            _hashService = hashService;
            _dataContext = dataContext;
        }
        public ViewResult Context()
        {
            ViewData["UsersCount"] = _dataContext.Users.Count();
            return View();
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
        public ViewResult Services()
        {
            ViewData["now"] = _timeService.GetTime();
            ViewData["hashCode"] = _timeService.GetHashCode();

            ViewData["date_now"] = _dateService.GetDate();
            ViewData["date_hashCode"] = _dateService.GetHashCode();

            ViewData["dt_now"] = _dtService.GetNow();
            ViewData["dt_hashCode"] = _dtService.GetHashCode();

            ViewData["hash"] = _hashService.Hash("123");

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}