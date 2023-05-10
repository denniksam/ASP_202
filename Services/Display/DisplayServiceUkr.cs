namespace ASP_202.Services.Display
{
    public class DisplayServiceUkr : IDisplayService
    {
        public string DateString(DateTime dateTime)
        {
            return DateTime.Today == dateTime.Date
                ? "Сьогодні " + dateTime.ToString("HH:mm")
                : dateTime.ToString("dd.MM.yyyy HH:mm");
        }

        public string ReduceString(string source, int maxLength)
        {
            if(source.Length <= maxLength) return source;

            source = source[..(maxLength - 3)];   // 3 - '...'

            int lastSpaceIndex = source.LastIndexOf(' ');

            if(maxLength - 3 - lastSpaceIndex < 15 
            && maxLength - 3 - lastSpaceIndex > 0) 
            {
                source = source[..(lastSpaceIndex+1)];
            }
            return source + "...";
        }
    }
}
