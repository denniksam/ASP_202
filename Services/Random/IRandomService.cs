namespace ASP_202.Services.Random
{
    public interface IRandomService
    {
        String RandomString(int length);
        String ConfirmCode(int length);
    }
}
