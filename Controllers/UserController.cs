using ASP_202.Models.User;
using ASP_202.Services.Hash;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace ASP_202.Controllers
{
    // [Route("User")]
    public class UserController : Controller
    {
        private readonly IHashService _hashService;
        private readonly ILogger<UserController> _logger;

        public UserController(IHashService hashService, ILogger<UserController> logger)
        {
            _hashService = hashService;
            _logger = logger;
        }

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
            byte minPasswordLength = 3;

            #region Login Validation
            if (String.IsNullOrEmpty(userRegistrationModel.Login))
            {
                validationResult.LoginMessage = "Логін не може бути порожним";
                isModelValid = false;
            }
            #endregion
            #region Password / Repeat Validation
            if (String.IsNullOrEmpty(userRegistrationModel.Password))
            {
                validationResult.PasswordMessage = "Пароль не може бути порожним";
                isModelValid = false;
            }
            else
            {
                if(userRegistrationModel.Password.Length < minPasswordLength) 
                {
                    validationResult.PasswordMessage = 
                        $"Пароль закороткий, щонайменше {minPasswordLength} символи";
                    isModelValid = false;
                }
                if ( ! userRegistrationModel.Password.Equals(userRegistrationModel.RepeatPassword))
                {
                    validationResult.RepeatPasswordMessage = "Паролі не збігаються";
                    isModelValid = false;
                }
            }
            #endregion
            #region Email Validation
            if (String.IsNullOrEmpty(userRegistrationModel.Email))
            {
                validationResult.EmailMessage = "Email не може бути порожним";
                isModelValid = false;
            }
            else
            {
                String emailRegex = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,})+)$";
                if (!Regex.IsMatch(userRegistrationModel.Email, emailRegex, RegexOptions.IgnoreCase))
                {
                    validationResult.EmailMessage = "Email не відповідає шаблону";
                    isModelValid = false;
                }
            }
            #endregion
            #region RealName Validation
            if (String.IsNullOrEmpty(userRegistrationModel.RealName))
            {
                validationResult.RealNameMessage = "Реальне ім'я не може бути порожним";
                isModelValid = false;
            }
            else
            {
                String nameRegex = @"^.+$";
                if (!Regex.IsMatch(userRegistrationModel.RealName, nameRegex))
                {
                    validationResult.RealNameMessage = "Реальне ім'я не відповідає шаблону";
                    isModelValid = false;
                }
            }
            #endregion
            #region IsAgree Validation
            if(userRegistrationModel.IsAgree == false)
            {
                validationResult.IsAgreeMessage = "Реєстрація дозволяється тільки з дотриманням правил сайту";
                isModelValid = false;
            }
            #endregion
            #region Avatar Uploading
            if(userRegistrationModel.Avatar is not null)
            {
                // завантажуємо файл, якщо він є. Відсутність файлу - припустима
                // схема: перевіямо файл що він є картинкою, переносимо у /avatars
                // TODO: Перевірити розмір файлу (більше 1кБ), видати повідомлення якщо це не так
                // TODO: Перевірити тип (Avatar.ContentType) - має бути "image/***"
                // TODO: Перевірити на наявність такого файлу у папці "wwwroot/avatars/"

                // Відокремлюємо розширення файлу
                String ext = Path.GetExtension(userRegistrationModel.Avatar.FileName);
                // хешуємо ім'я файлу
                String hash = (_hashService.Hash(
                    userRegistrationModel.Avatar.FileName + Guid.NewGuid()))[..16];
                // формуємо нове ім'я
                string path = "wwwroot/avatars/" + hash + ext;
                /* Д.З. Реалізувати додаткову перевірку на те, що файл із згенерованим
                 * ім'ям вже є у папці "wwwroot/avatars/". Зробити перевірку циклічною
                 * на випадок повторного збігу.
                 */
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    userRegistrationModel.Avatar.CopyTo(fileStream);
                }
            }
            #endregion

            if (isModelValid)
            {
                // показуємо сторінку з підтвердженням реєстрації
                return View(userRegistrationModel);
            }
            else
            {
                // повертаємо на форму реєстрації
                ViewData["validationResult"] = validationResult;
                // спосіб перейти на представлення, що не збігається з назвою методу
                return View("Registration");
            }            
        }
    }
}
