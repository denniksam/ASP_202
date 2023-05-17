namespace ASP_202.Data.Entity
{
    public class Sight
    {
        public Guid Id { get; set; }
        public Guid ItemId { get; set; }
        public DateTime Moment { get; set; }
        public Guid? UserId { get; set; }
    }
}
