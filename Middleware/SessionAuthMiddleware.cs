using ASP_202.Data;
using ASP_202.Data.Entity;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Security.Claims;

namespace ASP_202.Middleware
{
    public class SessionAuthMiddleware
    {
        // формування ланцюга здійснюється шляхом того, що кожна ланка викликає наступну
        // відомості про послідовність формуються у Program.cs, а кожен об'єкт отримує
        // посилання на наступну ланку _next через конструктор
        private readonly RequestDelegate _next;

        // схоже на інжекцію, але це формування ланцюга - передача кожній ланці посилання
        // на наступну
        public SessionAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context,   // context - перший (обов'язково)
            ILogger<SessionAuthMiddleware> logger,           // далі - інжекція сервісів
            DataContext dataContext) 
        {
            // дії Middleware
            String? userId = context.Session.GetString("authUserId");
            if(userId is not null)
            {
                try
                {
                    User? user = dataContext.Users.Find(Guid.Parse(userId));
                    if(user is not null)
                    {
                        // зберігаємо у контексті запиту
                        context.Items.Add("authUser", user);
                        /* Збереження у загальному контексті Entity-типів
                         * призводить до їх поширення у проєкті (хоча ці типи
                         * "службові" і потрібні для ORM - для роботи з БД)
                         * Альтернатива - система Claims (тверджень),
                         * які закладено у весь проект і кожна його частина
                         * може перевірити чи виконується те чи інше Claim
                         */
                        Claim[] claims = new Claim[]
                        {
                            new Claim(ClaimTypes.Sid, userId),  // Secure ID
                            new Claim(ClaimTypes.Name, user.RealName),
                            new Claim(ClaimTypes.NameIdentifier, user.Login),
                            new Claim(ClaimTypes.UserData, user.Avatar ?? String.Empty)
                        };
                        // з набору тверджень будуется власник (Principal)
                        var principal = new ClaimsPrincipal(
                            new ClaimsIdentity(
                                claims,
                                nameof(SessionAuthMiddleware)));
                        // відомості зберігаються у контексті у полі User
                        context.User = principal;
                    }
                }
                catch(Exception ex)
                {
                    logger.LogWarning(ex, "SessionAuthMiddleware");
                }
            }

            logger.LogInformation("SessionAuthMiddleware works");

            await _next(context);  // виклик наступної ланки
        }

        // стара (синхронна) схема
        // public void Invoke(HttpContext context) { _next(context); }
    }

    // Клас-розширення, який дозволить використовувати інструкцію app.UseSessionAuth()
    public static class SessionAuthMiddlewareExtension
    {
        public static IApplicationBuilder UseSessionAuth(this IApplicationBuilder app)
        {
            return app.UseMiddleware<SessionAuthMiddleware>();
        }
    }
}
/* Middleware - ПЗ проміжного рівня
 * request 
 *  [Middleware]
 *         - [кодування-UTF8]
 *         - [connect DB]
 *         - [locale (мова) - дістали з БД літерали даною мовою]
 *         - [auth - вибрати User/null]
 *  [Route]
 *         < Controller
 *         < html(потрібна мова, є User, знаємо що БД працює)
 */
