using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Feed.Domain.Users;

namespace Feed.EF.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");
            
            builder.Property(x => x.IsActive).HasColumnName("IsActive").HasColumnType("bit").HasDefaultValue(true);
            builder.Property(x => x.CreatedAt).HasDefaultValueSql("getutcdate()");
            builder.Property(x => x.UpdatedAt).HasDefaultValueSql("getutcdate()");
        }
    }
}
