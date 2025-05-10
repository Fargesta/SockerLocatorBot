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
            builder.Property(r => r.Code).IsRequired(true).HasMaxLength(4);
            builder.Property(r => r.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAdd().IsRequired();

            builder.HasMany(l => l.Users)
                .WithOne(r => r.Role)
                .HasForeignKey(l => l.RoleId)
                .HasPrincipalKey(x => x.Id)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasData(
                new RoleModel
                {
                    Id = 1,
                    Name = "Admin",
                    Description = "Administrator",
                    IsActive = true,
                    Code = "ADM"
                },
                new RoleModel
                {
                    Id = 2,
                    Name = "User",
                    Description = "Bot user can view and add locations",
                    IsActive = true,
                    Code = "USR"
                },
                new RoleModel
                {
                    Id = 3,
                    Name = "Guest",
                    Description = "Default role for new users. Has no access to bot functions. Must be confirmed by moderator to become user.",
                    IsActive = true,
                    Code = "GST"
                },
                new RoleModel
                {
                    Id = 4,
                    Name = "Moderator",
                    Description = "Can manage user access and review added locations.",
                    IsActive = true,
                    Code = "MOD"
                }
            );
        }
    }
}

