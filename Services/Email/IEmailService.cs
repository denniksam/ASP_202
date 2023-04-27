namespace ASP_202.Services.Email
{
    public interface IEmailService
    {
        bool Send(String templateName, object model);
    }
}
