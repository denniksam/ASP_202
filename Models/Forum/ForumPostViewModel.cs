namespace ASP_202.Models.Forum
{
    public class ForumPostViewModel
    {
        public String  Content         { get; set; } = null!;
        public String  CreatedDtString { get; set; } = null!;
        public String  AuthorAvatar    { get; set; } = null!;
        public String  AuthorName      { get; set; } = null!;
        public String? ReplyPreview    { get; set; } = null!;
    }
}
