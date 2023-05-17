namespace ASP_202.Models.Forum
{
    public class ForumSectionModel
    {
        public String Title           { get; set; } = null!;
        public String Description     { get; set; } = null!;
        public String Logo            { get; set; } = null!;
        public String CreatedDtString { get; set; } = null!;
        public String UrlIdString     { get; set; } = null!;

        public String AuthorName   { get; set; } = null!;
        public String AuthorAvatar { get; set; } = null!;

        public int  LikesCount    { get; set; }
        public int  DislikesCount { get; set; }
        public int? GivenRate     { get; set; }

        public int Sights { get; set; }

    }
}
/* Д.З. Реалізувати передачу даних про Id розділа у шаблон представлення
 * сформувати посилання у вигляді /Forum/Section/a12c3-.....(id)
 * При переході відобразити сторінку, яка показує тільки Id (перевірка
 * пройому параметра), можна вивести Id у логер
 * ** продумати механізм формування посилань замість Id у вигляді 
 *    часткової (не довже 50 символів) транслітерації назви розділу
 */