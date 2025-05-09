using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DbManager.Models.Configurations
{
    public class ImageConfig : IEntityTypeConfiguration<ImageModel>
    {
        public void Configure(EntityTypeBuilder<ImageModel> builder)
        {
            builder.ToTable("images");
            builder.HasKey(i => i.Id);
            builder.Property(i => i.Description).IsRequired(false).HasMaxLength(1000);
            builder.Property(i => i.IsActive).HasDefaultValue(false);
            builder.Property(l => l.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAdd().IsRequired();
            builder.Property(l => l.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAddOrUpdate().IsRequired();
            builder.Property(i => i.FileName).IsRequired(true).HasMaxLength(500);
            builder.Property(i => i.Url).IsRequired(true).HasMaxLength(1000);
            builder.Property(i => i.DriveFileId).IsRequired(true).HasMaxLength(500);
            builder.Property(i => i.FileSize).IsRequired(false);
            builder.Property(i => i.IsSaved).HasDefaultValue(false);
        }
    }
}
