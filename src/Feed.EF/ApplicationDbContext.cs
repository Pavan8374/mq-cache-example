using Feed.Domain.Contents;
using Feed.Domain.Follows;
using Feed.Domain.UserInteractions;
using Feed.Domain.Users;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Feed.EF
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> dbContextOptions) : base(dbContextOptions)
        {
            
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Content> Contents { get; set; }
        public DbSet<UserLike> UserLikes { get; set; }
        public DbSet<Follow> Follows { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(
               Assembly.GetAssembly(typeof(ApplicationDbContext)));
        }
    }
}
