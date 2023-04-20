namespace ASP_202.Data.Entity
{
    public class User
    {
        public Guid      Id            { get; set; }
        public String    Login         { get; set; }  = null!;
        public String    RealName      { get; set; }  = null!;
        public String    Email         { get; set; }  = null!;
        public String?   EmailCode     { get; set; }  = null!;
        public String    PasswordHash  { get; set; }  = null!;
        public String    PasswordSalt  { get; set; }  = null!;
        public String?   Avatar        { get; set; }  = null!;
        public DateTime  RegisterDt    { get; set; }
        public DateTime? LastEnterDt   { get; set; }


        /// Додано 2023-04-19, робота над Profile
        public Boolean IsEmailPublic { get; set; } = false;  // відображати чи ні Email у профілі для інших користувачів
        // Додавання полів у БД із даними можливе або якщо це NULLABLE поле, або
        // якщо воно має значення за default
        public Boolean IsRealNamePublic { get; set; } = false;
        public Boolean IsDatesPublic { get; set; } = false;  // RegisterDt | LastEnterDt
    }
}
