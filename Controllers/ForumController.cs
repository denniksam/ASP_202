using ASP_202.Data;
using ASP_202.Migrations;
using ASP_202.Models.Forum;
using ASP_202.Services.Display;
using ASP_202.Services.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ASP_202.Controllers
{
    public class ForumController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly ILogger<ForumController> _logger;
        private readonly IValidationService _validationService;
        private readonly IDisplayService _displayService;

        public ForumController(DataContext dataContext, ILogger<ForumController> logger, IValidationService validationService, IDisplayService displayService)
        {
            _dataContext = dataContext;
            _logger = logger;
            _validationService = validationService;
            _displayService = displayService;
        }

        private int counter;
        private int Counter { get =>  counter++; set { counter = value; } } 
        public IActionResult Index()
        {
            Counter = 0;
            String? userId = HttpContext.User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.Sid)?.Value;
            ForumIndexModel model = new()
            {
                UserCanCreate = HttpContext.User.Identity?.IsAuthenticated == true,
                Sections = _dataContext.Sections
                    .Include(s => s.Author)
                    .Include(s => s.RateList)
                    .OrderBy(s => s.CreatedDt)
                    .AsEnumerable()
                    .Select(s => new ForumSectionModel()
                    {
                        Title = s.Title,
                        Description = s.Description,
                        Logo = $"/img/logos/section{Counter}.png",
                        CreatedDtString = DateTime.Today == s.CreatedDt.Date
                            ? "Сьогодні " + s.CreatedDt.ToString("HH:mm")
                            : s.CreatedDt.ToString("dd.MM.yyyy HH:mm"),
                        UrlIdString = s.Id.ToString(),

                        AuthorName = s.Author.RealName,
                        AuthorAvatar = $"/avatars/{s.Author.Avatar}",

                        LikesCount = s.RateList.Count(r => r.Rating > 0),
                        DislikesCount = s.RateList.Count(r => r.Rating < 0),
                        GivenRate = userId == null ? null
                            : s.RateList.FirstOrDefault(r => r.UserId == Guid.Parse(userId))?.Rating,
                    })
                    .ToList(),
            };
            foreach(var section in model.Sections)
            {
                section.Sights = _dataContext.Sights.AsEnumerable()
                    .Count(sg => sg.ItemId == Guid.Parse(section.UrlIdString));
            }
            if (HttpContext.Session.GetString("CreateMessage") is String message)
            {
                model.CreateMessage = message;
                model.IsMessagePositive =
                    HttpContext.Session.GetInt32("IsMessagePositive") != 0;

                if(model.IsMessagePositive == false)
                {
                    model.FormModel = new()
                    {
                        Title = HttpContext.Session.GetString("SectionTitle")!,
                        Description = HttpContext.Session.GetString("SectionDescription")!
                    };
                    HttpContext.Session.Remove("SectionTitle");
                    HttpContext.Session.Remove("SectionDescription");
                }

                HttpContext.Session.Remove("CreateMessage");
                HttpContext.Session.Remove("IsMessagePositive");
            }
            return View(model);
        }

        public ViewResult Sections([FromRoute] String id)
        {
            ViewData["id"] = id;

            _dataContext.Sights.Add(new()
            {
                Id     = Guid.NewGuid(),
                Moment = DateTime.Now,
                ItemId = Guid.Parse(id),
                UserId = null
            });
            _dataContext.SaveChanges();

            ForumSectionsModel model = new()
            {
                UserCanCreate = HttpContext.User.Identity?.IsAuthenticated == true,
                Themes = _dataContext
                    .Themes
                    .Include(t => t.Author)
                    .Where(t => t.SectionId == Guid.Parse(id))
                    .Select(t => new ForumThemeModel()
                    {
                        Title = t.Title,
                        Description = t.Description,
                        UrlIdString = t.Id.ToString(),
                        CreatedDtString = DateTime.Today == t.CreatedDt.Date
                            ? "Сьогодні " + t.CreatedDt.ToString("HH:mm")
                            : t.CreatedDt.ToString("dd.MM.yyyy HH:mm"),
                        AuthorAvatar = $"/avatars/{t.Author.Avatar ?? "no-avatar.png"}",
                        AuthorName = t.Author.IsRealNamePublic ? t.Author.RealName : t.Author.Login,
                    })
                    .ToList()
            };

            if (HttpContext.Session.GetString("CreateMessage") is String message)
            {
                model.CreateMessage = message;
                model.IsMessagePositive =
                    HttpContext.Session.GetInt32("IsMessagePositive") != 0;

                if (model.IsMessagePositive == false)
                {
                    model.FormModel = new()
                    {
                        Title = HttpContext.Session.GetString("SectionTitle")!,
                        Description = HttpContext.Session.GetString("SectionDescription")!
                    };
                    HttpContext.Session.Remove("SectionTitle");
                    HttpContext.Session.Remove("SectionDescription");
                }

                HttpContext.Session.Remove("CreateMessage");
                HttpContext.Session.Remove("IsMessagePositive");
            }

            return View(model);
        }

        public IActionResult Themes([FromRoute] String id)
        {
            Data.Entity.Theme? theme = null;
            try
            {
                theme = _dataContext.Themes.Find(Guid.Parse(id));
            }
            catch { }
            if (theme == null)
            {
                return NotFound();
            }
            ForumThemesPageModel model = new()
            {
                Title = theme.Title,
                UserCanCreate = HttpContext.User.Identity?.IsAuthenticated == true,
                ThemeIdString = id,
                Topics = _dataContext
                    .Topics
                    .Where(t => t.ThemeId == theme.Id)
                    .AsEnumerable()
                    .Select(t => new ForumTopicViewModel
                    {
                        Title = t.Title,
                        Description = _displayService.ReduceString(t.Description, 120),
                        UrlIdString = t.Id.ToString(),
                        CreatedDtString = _displayService.DateString(t.CreatedDt),
                    })
                    .ToList()
            };

            if (HttpContext.Session.GetString("CreateMessage") is String message)
            {
                model.CreateMessage = message;
                model.IsMessagePositive =
                    HttpContext.Session.GetInt32("IsMessagePositive") != 0;
                if (model.IsMessagePositive == false)
                {
                    model.FormModel = new()
                    {
                        Title = HttpContext.Session.GetString("SectionTitle")!,
                        Description = HttpContext.Session.GetString("SectionDescription")!
                    };
                    HttpContext.Session.Remove("SectionTitle");
                    HttpContext.Session.Remove("SectionDescription");
                }
                HttpContext.Session.Remove("CreateMessage");
                HttpContext.Session.Remove("IsMessagePositive");
            }

            return View(model);
        }

        public IActionResult Topics([FromRoute] String id)
        {
            // TopicsPageModel PostFormModel PostViewModel
            Data.Entity.Topic? topic = null;
            try
            {
                topic = _dataContext.Topics.Find(Guid.Parse(id));
            }
            catch { }
            if (topic == null)
            {
                return NotFound();
            }
            ForumTopicsPageModel model = new()
            {
                UserCanCreate = HttpContext.User.Identity?.IsAuthenticated == true,
                Title = topic.Title,
                Description = topic.Description,
                TopicIdString = id,
                Posts = _dataContext
                    .Posts
                    .Include(p => p.Author)
                    .Include(p => p.Reply)
                    .Where(p => p.TopicId == topic.Id)
                    .Select(p => new ForumPostViewModel
                    {
                        Content = p.Content,
                        CreatedDtString = _displayService.DateString(p.CreatedDt),
                        AuthorAvatar = $"/avatars/{p.Author.Avatar ?? "no-avatar.png"}",
                        AuthorName = p.Author.IsRealNamePublic ? p.Author.RealName : p.Author.Login,
                        ReplyPreview = null
                    })
                    .ToList()
            };

            if (HttpContext.Session.GetString("CreateMessage") is String message)
            {
                model.CreateMessage = message;
                model.IsMessagePositive =
                    HttpContext.Session.GetInt32("IsMessagePositive") != 0;
                if (model.IsMessagePositive == false)
                {
                    model.FormModel = new()
                    {
                        Content = HttpContext.Session.GetString("PostContent")!,
                        ReplyId = HttpContext.Session.GetString("PostReply")!
                    };
                    HttpContext.Session.Remove("PostContent");
                    HttpContext.Session.Remove("PostReply");
                }
                HttpContext.Session.Remove("CreateMessage");
                HttpContext.Session.Remove("IsMessagePositive");
            }

            return View(model);
        }

        [HttpPost]
        public RedirectToActionResult CreateSection(ForumSectionFormModel model)
        {
            _logger.LogInformation("Title: {t}, Description: {d}", model.Title, model.Description);
            if(!_validationService.Validate(model.Title, ValidationTerms.NotEmpty))
            {
                HttpContext.Session.SetString("CreateMessage", "Назва не може бути порожною");
                HttpContext.Session.SetInt32("IsMessagePositive", 0);
                HttpContext.Session.SetString("SectionTitle", model.Title ?? String.Empty);
                HttpContext.Session.SetString("SectionDescription", model.Description ?? String.Empty);
            }
            else if (!_validationService.Validate(model.Description, ValidationTerms.NotEmpty))
            {
                HttpContext.Session.SetString("CreateMessage", "Опис не може бути порожним");
                HttpContext.Session.SetInt32("IsMessagePositive", 0);
                HttpContext.Session.SetString("SectionTitle", model.Title ?? String.Empty);
                HttpContext.Session.SetString("SectionDescription", model.Description ?? String.Empty);
            }
            else
            {
                Guid userId;
                try
                {
                    userId = Guid.Parse(
                        HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid)?.Value!
                    );
                    _dataContext.Sections.Add(new()
                    {
                        Id = Guid.NewGuid(),
                        AuthorId = userId,
                        Title = model.Title,
                        Description = model.Description,
                        CreatedDt = DateTime.Now
                    });
                    _dataContext.SaveChanges();

                    HttpContext.Session.SetString("CreateMessage", "Розділ успішно створено");
                    HttpContext.Session.SetInt32("IsMessagePositive", 1);
                }
                catch
                {
                    HttpContext.Session.SetString("CreateMessage", "Помилка авторизації");
                    HttpContext.Session.SetInt32("IsMessagePositive", 0);
                    HttpContext.Session.SetString("SectionTitle", model.Title ?? String.Empty);
                    HttpContext.Session.SetString("SectionDescription", model.Description ?? String.Empty);
                }                
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public RedirectToActionResult CreateTheme(ForumThemeFormModel model)
        {
            _logger.LogInformation("Title: {t}, Description: {d}", model.Title, model.Description);
            if (!_validationService.Validate(model.Title, ValidationTerms.NotEmpty))
            {
                HttpContext.Session.SetString("CreateMessage", "Назва не може бути порожною");
                HttpContext.Session.SetInt32("IsMessagePositive", 0);
                HttpContext.Session.SetString("SectionTitle", model.Title ?? String.Empty);
                HttpContext.Session.SetString("SectionDescription", model.Description ?? String.Empty);
            }
            else if (!_validationService.Validate(model.Description, ValidationTerms.NotEmpty))
            {
                HttpContext.Session.SetString("CreateMessage", "Опис не може бути порожним");
                HttpContext.Session.SetInt32("IsMessagePositive", 0);
                HttpContext.Session.SetString("SectionTitle", model.Title ?? String.Empty);
                HttpContext.Session.SetString("SectionDescription", model.Description ?? String.Empty);
            }
            else
            {
                Guid userId;
                try
                {
                    userId = Guid.Parse(
                        HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid)?.Value!
                    );
                    _dataContext.Themes.Add(new()
                    {
                        Id = Guid.NewGuid(),
                        AuthorId = userId,
                        Title = model.Title,
                        Description = model.Description,
                        CreatedDt = DateTime.Now,
                        SectionId = Guid.Parse(model.SectionId)
                    });
                    _dataContext.SaveChanges();

                    HttpContext.Session.SetString("CreateMessage", "Розділ успішно створено");
                    HttpContext.Session.SetInt32("IsMessagePositive", 1);
                }
                catch
                {
                    HttpContext.Session.SetString("CreateMessage", "Помилка авторизації");
                    HttpContext.Session.SetInt32("IsMessagePositive", 0);
                    HttpContext.Session.SetString("SectionTitle", model.Title ?? String.Empty);
                    HttpContext.Session.SetString("SectionDescription", model.Description ?? String.Empty);
                }
            }
            return RedirectToAction(nameof(Sections), new { id = model.SectionId });
        }

        [HttpPost]
        public RedirectToActionResult CreateTopic(ForumTopicFormModel model)
        {
            if (!_validationService.Validate(model.Title, ValidationTerms.NotEmpty))
            {
                HttpContext.Session.SetString("CreateMessage", "Назва не може бути порожною");
                HttpContext.Session.SetInt32("IsMessagePositive", 0);
                HttpContext.Session.SetString("SectionTitle", model.Title ?? String.Empty);
                HttpContext.Session.SetString("SectionDescription", model.Description ?? String.Empty);
            }
            else if (!_validationService.Validate(model.Description, ValidationTerms.NotEmpty))
            {
                HttpContext.Session.SetString("CreateMessage", "Опис не може бути порожним");
                HttpContext.Session.SetInt32("IsMessagePositive", 0);
                HttpContext.Session.SetString("SectionTitle", model.Title ?? String.Empty);
                HttpContext.Session.SetString("SectionDescription", model.Description ?? String.Empty);
            }
            else
            {
                Guid userId;
                try
                {
                    userId = Guid.Parse(
                        HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid)?.Value!
                    );
                    _dataContext.Topics.Add(new()
                    {
                        Id = Guid.NewGuid(),
                        AuthorId = userId,
                        Title = model.Title,
                        Description = model.Description,
                        CreatedDt = DateTime.Now,
                        ThemeId = Guid.Parse(model.ThemeId)
                    });
                    _dataContext.SaveChanges();

                    HttpContext.Session.SetString("CreateMessage", "Питання успішно створено");
                    HttpContext.Session.SetInt32("IsMessagePositive", 1);
                }
                catch
                {
                    HttpContext.Session.SetString("CreateMessage", "Помилка авторизації");
                    HttpContext.Session.SetInt32("IsMessagePositive", 0);
                    HttpContext.Session.SetString("SectionTitle", model.Title ?? String.Empty);
                    HttpContext.Session.SetString("SectionDescription", model.Description ?? String.Empty);
                }
            }
            return RedirectToAction(
                nameof(Themes),
                new { id = model.ThemeId }
            );
        }

        [HttpPost]
        public RedirectToActionResult CreatePost(ForumPostFormModel model)
        {
            if (!_validationService.Validate(model.Content, ValidationTerms.NotEmpty))
            {
                HttpContext.Session.SetString("CreateMessage", "Відповідь не може бути порожною");
                HttpContext.Session.SetInt32("IsMessagePositive", 0);
                HttpContext.Session.SetString("PostContent", model.Content ?? String.Empty);
                HttpContext.Session.SetString("PostReply", model.ReplyId ?? String.Empty);
            }
            else
            {
                Guid userId;
                try
                {
                    userId = Guid.Parse(
                        HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid)?.Value!
                    );
                    _dataContext.Posts.Add(new()
                    {
                        Id = Guid.NewGuid(),
                        AuthorId = userId,
                        Content = model.Content,
                        TopicId = Guid.Parse(model.TopicId),
                        CreatedDt = DateTime.Now,
                        ReplyId = String.IsNullOrEmpty(model.ReplyId) ? null : Guid.Parse(model.ReplyId)
                    });
                    _dataContext.SaveChanges();

                    HttpContext.Session.SetString("CreateMessage", "Відповідь успішно створено");
                    HttpContext.Session.SetInt32("IsMessagePositive", 1);
                }
                catch
                {
                    HttpContext.Session.SetString("CreateMessage", "Помилка авторизації");
                    HttpContext.Session.SetInt32("IsMessagePositive", 0);
                    HttpContext.Session.SetString("PostContent", model.Content ?? String.Empty);
                    HttpContext.Session.SetString("PostReply", model.ReplyId ?? String.Empty);
                }
            }
            return RedirectToAction(
                nameof(Topics),
                new { id = model.TopicId });
        }
    }
}
/* Д.З. Модифікувати форму створення нового розділу
 * додати можливість вибору логотипу (картинки) для даного розділу
 * Забезпечити виведення логотипів, для тих розділі, що не мають логотипу
 * залишити нумерований вибір зображень.
 */