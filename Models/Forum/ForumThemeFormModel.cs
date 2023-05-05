using Microsoft.AspNetCore.Mvc;

namespace ASP_202.Models.Forum
{
    public class ForumThemeFormModel
    {
        [FromForm(Name = "theme-title")]
        public String Title { get; set; } = null!;

        [FromForm(Name = "theme-description")]
        public String Description { get; set; } = null!;

        [FromForm(Name = "section-id")]
        public String SectionId { get; set; } = null!;
    }
}
