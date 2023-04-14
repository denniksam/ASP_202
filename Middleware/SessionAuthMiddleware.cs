using ASP_202.Data;
using ASP_202.Data.Entity;
using Microsoft.Extensions.Diagnostics.HealthChecks;

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
