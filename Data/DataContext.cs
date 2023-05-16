using Microsoft.EntityFrameworkCore;

namespace ASP_202.Data
{
    public class DataContext : DbContext
    {
        public DbSet<Entity.User> Users { get; set; }
        public DbSet<Entity.EmailConfirmToken> EmailConfirmTokens { get; set; }

        public DbSet<Entity.Section> Sections { get; set; }
        public DbSet<Entity.Theme> Themes { get; set; }
        public DbSet<Entity.Topic> Topics { get; set; }
        public DbSet<Entity.Post> Posts { get; set; }

        public DbSet<Entity.Rate> Rates { get; set; }


        public DataContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Entity.Rate>()
                .HasKey(nameof(Entity.Rate.ItemId), nameof(Entity.Rate.UserId));

            modelBuilder.Entity<Entity.Section>()
                .HasOne(s => s.Author)
                .WithMany()
                .HasForeignKey(s => s.AuthorId);

            modelBuilder.Entity<Entity.Section>()
                .HasMany(s => s.RateList)
                .WithOne()
                .HasForeignKey(r => r.ItemId);


            modelBuilder.Entity<Entity.Theme>()
               .HasOne(t => t.Author)
               .WithMany()
               .HasForeignKey(t => t.AuthorId);

            modelBuilder.Entity<Entity.Post>()
               .HasOne(p => p.Author)
               .WithMany()
               .HasForeignKey(p => p.AuthorId);

            modelBuilder.Entity<Entity.Post>()
               .HasOne(p => p.Reply)
               .WithMany()
               .HasForeignKey(p => p.ReplyId);
        }
    }
}
