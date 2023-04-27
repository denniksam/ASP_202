using System.Net;
using System.Net.Mail;

namespace ASP_202.Services.Email
{
    public class GmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<GmailService> _logger;

        public GmailService(IConfiguration configuration, ILogger<GmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public bool Send(String templateName, Object model)
        {
            if(templateName is null) 
                throw new ArgumentNullException(nameof(templateName));
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            // Шукаємо файл шаблону
            String[] possibleNames = new String[]
            {
                templateName,
                templateName + ".html",
                "EmailTemplates/" + templateName,
                "EmailTemplates/" + templateName + ".html"
            };
            String? template = null;
            foreach (String possibleName in possibleNames)
            {
                if(System.IO.File.Exists(possibleName))
                {
                    template = System.IO.File.ReadAllText(possibleName);
                    break;
                }
            }
            if (template is null)
            {
                throw new ArgumentException($"File not found for template '{templateName}'");
            }

            /* Заповнюємо шаблон даними з моделі:
             * скануємо властивості (Properties) моделі, формуємо з їх імен плейсхолдер
             * '{{Name}}'
             * шукаємо та замінюємо їх на відповідні значення
             * Заразом зберігаємо окремо поле Email
             */
            String? userEmail = null;
            foreach(var prop in model.GetType().GetProperties())
            {
                if (prop.Name == "Email") userEmail = prop.GetValue(model)?.ToString();
                String placeholder = $"{{{{{prop.Name}}}}}";
                if (template.Contains(placeholder))
                {
                    template = template.Replace(
                        placeholder, 
                        prop.GetValue(model)?.ToString() ?? "");
                }
            }
            // TODO: перевірити template на залишок {{\w+}} фрагментів,
            // Exception - модель не заповнює шаблон
            if(userEmail is null)
            {
                throw new ArgumentException("Model has no 'Email' property");
            }

            // Збираємо конфігурацію підключення до SMTP
            String? host = _configuration["Smtp:Gmail:Host"]
                ?? throw new MissingFieldException("Missing configuration 'Smtp:Gmail:Host'");
            String? mailbox = _configuration["Smtp:Gmail:Email"]
                ?? throw new MissingFieldException("Missing configuration 'Smtp:Gmail:Email'");
            String? password = _configuration["Smtp:Gmail:Password"]
                ?? throw new MissingFieldException("Missing configuration 'Smtp:Gmail:Password'");
            int port;
            try { port = Convert.ToInt32(_configuration["Smtp:Gmail:Port"]); }
            catch { throw new MissingFieldException("Missing or invalid <int> configuration 'Smtp:Gmail:Port'"); }
            bool ssl;
            try { ssl = Convert.ToBoolean(_configuration["Smtp:Gmail:Ssl"]); }
            catch { throw new MissingFieldException("Missing or invalid <bool> configuration 'Smtp:Gmail:Ssl'"); }

            // формуємо листа та надсилаємо
            MailMessage mailMessage = new()
            {
                From = new MailAddress(mailbox),
                Subject = "ASP-202 Project",
                IsBodyHtml = true,
                Body = template
            };
            mailMessage.To.Add(userEmail);

            using SmtpClient smtpClient = new(host, port)
            {
                Credentials = new NetworkCredential(mailbox, password),
                EnableSsl = ssl
            };
            try
            {
                smtpClient.Send(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Email sent error '{ex}'", ex.Message);
                return false;
            }            
        }
    }
}
