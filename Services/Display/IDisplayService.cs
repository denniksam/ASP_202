namespace ASP_202.Services.Display
{
    public interface IDisplayService
    {
        String DateString(DateTime dateTime);
        String ReduceString(String source, int maxLength);
    }
}
