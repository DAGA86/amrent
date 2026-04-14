namespace AMRent.Data.Models.Database
{
    public class Role : dCore.Identity.Models.Role
    {
        public List<User> Users { get; set; } = new();
        public List<RolePermission> RolePermissions { get; set; } = new();
    }
}
