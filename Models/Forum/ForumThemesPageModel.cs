namespace ASP_202.Models.Forum
{
    public class ForumThemesPageModel
    {
        public Boolean UserCanCreate { get; set; }
        public List<ForumTopicViewModel> Topics { get; set; } = null!;
        public String Title { get; set; } = null!;
        public String ThemeIdString { get; set; } = null!;

        // Дані від форми додавання топіку
        public String? CreateMessage { get; set; }
        public Boolean? IsMessagePositive { get; set; }
        public ForumTopicFormModel? FormModel { get; set; }
    }
}
