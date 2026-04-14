using System.ComponentModel;

namespace AMRent.Data.Enums
{
    public enum UserRoles
    {
        [Description("Cliente")]
        Customer = 1,
        [Description("Developer")]
        Developer = 2,
        [Description("Administrador")]
        Administrator = 3,
        [Description("Marketing")]
        Marketing = 4,
        [Description("Renting")]
        Renting = 5,
        [Description("Reservas")]
        Administrative = 6
    }
}
