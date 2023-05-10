namespace ASP_202.Models.Forum
{
    public class ForumTopicViewModel
    {
        public String Title { get; set; } = null!;
        public String Description { get; set; } = null!;
        public String CreatedDtString { get; set; } = null!;
        public String UrlIdString { get; set; } = null!;

        public String AuthorName { get; set; } = null!;
        public String AuthorAvatar { get; set; } = null!;
    }
}
