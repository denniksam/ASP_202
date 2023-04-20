using ASP_202.Data;
using ASP_202.Data.Entity;
using ASP_202.Models.User;
using ASP_202.Services.Hash;
using ASP_202.Services.Kdf;
using ASP_202.Services.Random;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace ASP_202.Controllers
{
    // [Route("User")]
    public class UserController : Controller
    {
        private readonly IHashService _hashService;
        private readonly ILogger<UserController> _logger;
        private readonly DataContext _dataContext;
        private readonly IRandomService _randomService;
        private readonly IKdfService _kdfService;

        public UserController(IHashService hashService, ILogger<UserController> logger, DataContext dataContext, IRandomService randomService, IKdfService kdfService)
        {
            _hashService = hashService;
            _logger = logger;
            _dataContext = dataContext;
            _randomService = randomService;
            _kdfService = kdfService;
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
            if(_dataContext.Users.Any(u => u.Login.ToLower() == userRegistrationModel.Login.ToLower()))
            {
                validationResult.LoginMessage = 
                    $"Логін '{userRegistrationModel.Login}' вже у вжитку";
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
            String avatarFilename = null!;
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
                avatarFilename = hash + ext;
                string path = "wwwroot/avatars/" + avatarFilename;
                /* Д.З. Реалізувати додаткову перевірку на те, що файл із згенерованим
                 * ім'ям вже є у папці "wwwroot/avatars/". Зробити перевірку циклічною
                 * на випадок повторного збігу.
                 */
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    userRegistrationModel.Avatar.CopyTo(fileStream);
                }
                ViewData["avatarFilename"] = avatarFilename;
                /* Д.З. Додати до сервісу RandomService метод формування
                 * випадкового імені файлу (заданої довжини) - без розширення.
                 * Перелік символів для імені файлу - безпечні символи
                 * для імен (тільки lower case, літери, цифри, "_-=", тощо)
                 * Замінити алгоритм формування імені для файлу аватара
                 * на використання створеного методу у сервісі.
                 */
            }
            #endregion

            if (isModelValid)
            {
                // формуємо сутність для БД
                String salt = _randomService.RandomString(8);
                User user = new()
                {
                    Id = Guid.NewGuid(),
                    Login = userRegistrationModel.Login,
                    PasswordSalt = salt,
                    PasswordHash = _kdfService.GetDerivedKey(userRegistrationModel.Password, salt),
                    Avatar = avatarFilename,
                    Email = userRegistrationModel.Email,
                    RealName = userRegistrationModel.RealName,
                    RegisterDt = DateTime.Now,
                    LastEnterDt = null,
                    EmailCode = _randomService.ConfirmCode(6),
                };
                /* Д.З. Передати ім'я файлу-аватара (без шляху, тільки файл)
                 * у об'єкт user.
                 * Згенерувати 6-літерний випадковий код підтвердження пошти,
                 * включити його у той самий об'єкт user
                 */

                // додаємо її до контексту
                _dataContext.Users.Add(user);
                _dataContext.SaveChanges();

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
        
        [HttpPost]   // метод доступний тільки POST - запитом
        public String AuthUser()
        {
            // Альтернативний спосіб дістатись параметрів форми (окрім моделі)
            StringValues loginValues = Request.Form["user-login"];
            if(loginValues.Count == 0)
            {
                return "No login";
            }
            String login = loginValues[0] ?? "";

            StringValues passwordValues = Request.Form["user-password"];
            if (passwordValues.Count == 0)
            {
                return "No password";
            }
            String password = passwordValues[0] ?? "";

            User? user = _dataContext.Users.Where(u => u.Login == login).FirstOrDefault();
            if (user is not null)   // знайдений користувач
            { 
                if(user.PasswordHash == _kdfService.GetDerivedKey(password, user.PasswordSalt))
                {
                    HttpContext.Session.SetString("authUserId", user.Id.ToString());
                    return $"OK";
                }
            }

            return $"Автентифікацію віхилено";
        }

        public RedirectToActionResult Logout()
        {
            HttpContext.Session.Remove("authUserId");
            return RedirectToAction("Index", "Home");
            /* Redirect - як це працює?
             * Browser               Server
             * GET /home -----------> Home-Index->View()
             *   page    <----------- 200 OK <!doctype html>...
             *   
             *            GET /Logout
             * <a logout> ----------> User-Logout->Redirect(...)
             *            <---------- 302 Redirect /Home/Index
             * GET /Home/Index -----> Home-Index->View()
             *   page    <----------- 200 OK <!doctype html>... 
             *   
             * Це зовнішній редірект, мається на увазі те, що браузер змінює
             * URL та повторює запит
             * Існують внутрішні редіректи (forward), які змінюють контроллер/
             * представлення без повідомлення браузера. У браузері залишається
             * введений URL, а сервер за ним видає іншу сторінку.
             */
        }

        public IActionResult Profile( [FromRoute] String id )
        {
            // _logger.LogInformation(id);
            Data.Entity.User? user = _dataContext.Users.FirstOrDefault(u => u.Login == id);
            if(user is not null)  // Є користувач із запитаним Login
            {
                Models.User.ProfileModel model = new(user);
                if(String.IsNullOrEmpty(model.Avatar))
                {
                    model.Avatar = "no-avatar.png";
                }
                // перевіряємо, чи є автентифікований користувач і чи це його логін
                // HttpContext.User - закладено (або ні) в AuthMiddleware
                if (HttpContext.User.Identity is not null
                 && HttpContext.User.Identity.IsAuthenticated)
                {
                    // користувач автентифікований
                    String userLogin =  // логін автентифікованого користувача
                        HttpContext.User.Claims
                            .First(claim => claim.Type == ClaimTypes.NameIdentifier)
                            .Value;

                    if (model.Login == userLogin)
                    {
                        model.IsPersonal = true;
                    }
                }
                    
                return View(model);
            }
            else
            {
                return NotFound();
            }
            
            /* ТЗ до сторінки користувача
             * 1. Чи буде ця сторінка доступна для інших користувачів?
             * Так, але відображення персональних даних може бути обмежене
             * 
             * 2. Як будуть формуватись посилання (URL) на цю сторінку?
             *     /User/Profile/??????
             *   а) Id
             *   б) Login
             * Вибираємо Login, це тягне за собою
             *  - логін має бути унікальним                                v
             *  - логін має бути URL-безпечним (без спецсимволів)          TODO
             * 
             * Д.З. Реалізувати умовне відображення для електронної пошти
             * на персональній сторінці (Profile)
             * в залежності від того, які налаштування IsEmailPublic
             * у відповідного користувача
             */
        }
    }
}
