using Microsoft.AspNetCore.Mvc;

namespace ASP_202.Models.Forum
{
    public class ForumSectionFormModel
    {
        [FromForm(Name = "section-title")]
        public String Title { get; set; } = null!;

        [FromForm(Name = "section-description")]
        public String Description { get; set; } = null!;
    }
}
