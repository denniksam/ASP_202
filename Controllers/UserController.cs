using ASP_202.Models.User;
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
        public IActionResult RegisterUser(UserRegistrationModel userRegistrationModel)
        {
            // Перший етап оброблення даних - валідація
            // Є декілька підходів: а) до першої помилки, б) повна перевірка
            // За повної перевірки кожне з полів може мати своє повідомлення - 
            // потрібна додаткова модель UserValidationModel

            UserValidationModel validationResult = new();
            bool isModelValid = true;

            if(String.IsNullOrEmpty(userRegistrationModel.Login))
            {
                validationResult.LoginMessage = "Логін не може бути порожним";
                isModelValid = false;
            }

            ViewData["validationResult"] = validationResult;
            
            // спосіб перейти на представлення, що не збігається з назвою методу
            return View("Registration");
        }
    }
}
