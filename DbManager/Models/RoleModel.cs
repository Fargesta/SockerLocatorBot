namespace DbManager.Models
{
    public class RoleModel
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public bool IsActive { get; set; }
        public string Code { get; set; } = null!;
        public DateTime CreatedAt { get; set; }

        public List<UserModel> Users { get; set; } = [];
    }
}
