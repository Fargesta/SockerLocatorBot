namespace DbManager.Models
{
    public class UserModel
    {
        public long Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string? LastName { get; set; } = null!;
        public string? UserName { get; set; } = null!;
        public string? LanguageCode { get; set; } = null!;
        public bool IsActive { get; set; } = false;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public long RoleId { get; set; }
        public RoleModel Role { get; set; } = null!;

        public List<LocationModel> CretedLocations { get; set; } = [];
        public List<LocationModel> UpdatedLocations { get; set; } = [];
        public List<ImageModel> CreatedImages { get; set; } = [];
        public List<ImageModel> UpdatedImages { get; set; } = [];
    }
}
