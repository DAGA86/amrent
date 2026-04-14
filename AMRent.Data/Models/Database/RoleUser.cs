namespace AMRent.Data.Models.Database
{
    public class RoleUser : dCore.Identity.Models.RoleUser
    {
        public User User { get; set; }
        public Role Role { get; set; }
    }
}
