using NetTopologySuite.Geometries;

namespace DbManager.Models
{
    public class LocationModel
    {
        public long Id { get; set; }
        public Point Location { get; set; } = null!;
        public string SocketType { get; set; } = null!;
        public string Description { get; set; } = null!;
        public bool IsActive { get; set; } = false;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public UserModel CreatedBy { get; set; } = null!;
        public UserModel UpdatedBy { get; set; } = null!;

        public List<ImageModel> Images { get; set; } = [];
    }
}
