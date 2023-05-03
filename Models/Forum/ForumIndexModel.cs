namespace ASP_202.Models.Forum
{
    public class ForumIndexModel
    {
        public Boolean UserCanCreate { get; set; }
        public List<Data.Entity.Section> Sections { get; set; } = null!;

        // Дані від форми додавання розділу
        public String? CreateMessage { get; set; }
        public Boolean? IsMessagePositive { get; set; }
        public ForumSectionFormModel? FormModel { get; set; }
    }
}
