using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DbManager.Models.Configurations
{
    public class LocationConfig : IEntityTypeConfiguration<LocationModel>
    {
        public void Configure(EntityTypeBuilder<LocationModel> builder)
        {
            builder.ToTable("locations");
            builder.HasKey(l => l.Id);
            builder.Property(l => l.Location).IsRequired(true);
            builder.Property(l => l.SocketType).IsRequired(true).HasMaxLength(4);
            builder.Property(l => l.Description).IsRequired(false).HasMaxLength(1000);
            builder.Property(l => l.IsActive).HasDefaultValue(false);

            builder.Property(l => l.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAdd().IsRequired();
            builder.Property(l => l.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAddOrUpdate().IsRequired();

            builder.Ignore(u => u.UpdatedBy);

            builder.HasMany(u => u.Images)
                .WithOne(i => i.Location)
                .HasForeignKey(i => i.Id)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
