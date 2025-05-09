namespace DbManager.Models
{
    public class ImageModel
    {
        public long Id { get; set; }
        public string Url { get; set; } = null!;
        public string FileName { get; set; } = null!;
        public string DriveFileId { get; set; } = null!;
        public long? FileSize { get; set; } = null;
        public string? Description { get; set; } = null;
        public bool IsActive { get; set; } = false;
        public bool IsSaved { get; set; } = false;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public long CreatedById { get; set; }
        public UserModel CreatedBy { get; set; } = null!;
        public long UpdatedById { get; set; }
        public UserModel UpdatedBy { get; set; } = null!;

        public long LocationId { get; set; }
        public LocationModel Location { get; set; } = null!;
    }
}
