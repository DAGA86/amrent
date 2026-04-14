namespace AMRent.Data.Models.Database
{
    public class RolePermission
    {
        public int RoleId { get; set; }
        public Enums.Permissions Permission { get; set; }

        public Role Role { get; set; }
    }
}
