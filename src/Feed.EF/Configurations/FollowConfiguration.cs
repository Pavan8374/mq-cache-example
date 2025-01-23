using Feed.Domain.Follows;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Feed.EF.Configurations
{
    public class FollowConfiguration : IEntityTypeConfiguration<Follow>
    {
        public void Configure(EntityTypeBuilder<Follow> builder)
        {
            builder.ToTable("Follows");
            builder.HasKey(t => t.Id);
            builder.Property(x => x.FollowerId).HasColumnName("FollowerId");
            builder.Property(x => x.FollowingId).HasColumnName("FollowingId");
            builder.Property(x => x.IsActive).HasColumnName("IsActive").HasColumnType("bit").HasDefaultValue(true);
            builder.Property(x => x.CreatedAt).HasDefaultValueSql("getutcdate()");
            builder.Property(x => x.UpdatedAt).HasDefaultValueSql("getutcdate()");


            builder.HasOne(f => f.Follower)
                .WithMany(u => u.Followers)
                .HasForeignKey(f => f.FollowerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(f => f.Following)
                .WithMany(u => u.Followings)
                .HasForeignKey(f => f.FollowingId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
