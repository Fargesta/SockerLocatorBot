using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DbManager.Models.Configurations
{
    public class UserConfig : IEntityTypeConfiguration<UserModel>
    {
        public void Configure(EntityTypeBuilder<UserModel> builder)
        {
            builder.ToTable("users");
            builder.HasKey(u => u.Id);
            builder.Property(u => u.FirstName).IsRequired(true).HasMaxLength(500);
            builder.Property(u => u.LastName).IsRequired(false).HasMaxLength(500);
            builder.Property(u => u.UserName).IsRequired(true).HasMaxLength(500);
            builder.Property(u => u.LanguageCode).IsRequired(false).HasMaxLength(10);
            builder.Property(u => u.IsActive).HasDefaultValue(false);

            builder.Property(l => l.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAdd().IsRequired();
            builder.Property(l => l.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAddOrUpdate().IsRequired();

            builder.HasMany(l => l.CretedLocations)
                .WithOne(u => u.CreatedBy)
                .HasForeignKey(l => l.CreatedById)
                .HasPrincipalKey(x => x.Id)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(l => l.UpdatedLocations)
                .WithOne(u => u.UpdatedBy)
                .HasForeignKey(l => l.UpdatedById)
                .HasPrincipalKey(x => x.Id)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(i => i.CreatedImages)
                .WithOne(u => u.CreatedBy)
                .HasForeignKey(i => i.CreatedById)
                .HasPrincipalKey(x => x.Id)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(i => i.UpdatedImages)
                .WithOne(u => u.UpdatedBy)
                .HasForeignKey(i => i.UpdatedById)
                .HasPrincipalKey(x => x.Id)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
