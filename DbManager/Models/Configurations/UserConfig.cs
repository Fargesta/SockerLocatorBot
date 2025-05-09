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

            builder.HasMany(l => l.Locations)
                .WithOne(u => u.CreatedBy)
                .HasForeignKey(l => l.Id)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(l => l.Locations)
                .WithOne(u => u.UpdatedBy)
                .HasForeignKey(l => l.Id)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(i => i.Images)
                .WithOne(u => u.CreatedBy)
                .HasForeignKey(i => i.Id)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(i => i.Images)
                .WithOne(u => u.UpdatedBy)
                .HasForeignKey(i => i.Id)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
