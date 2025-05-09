using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DbManager.Models.Configurations
{
    public class RoleConfig : IEntityTypeConfiguration<RoleModel>
    {
        public void Configure(EntityTypeBuilder<RoleModel> builder)
        {
            builder.ToTable("roles");
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Name).IsRequired(true).HasMaxLength(500);
            builder.Property(r => r.Description).IsRequired(false).HasMaxLength(1000);
            builder.Property(r => r.IsActive).HasDefaultValue(false);
            builder.Property(l => l.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAdd().IsRequired();
            builder.Property(r => r.Code).IsRequired(true).HasMaxLength(4);

            builder.HasMany(l => l.Users)
                .WithOne(r => r.Role)
                .HasForeignKey(l => l.Id)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
