using System.ComponentModel;

namespace AMRent.Data.Enums
{
    public enum DaysOfWeek
    {
        [Description("Segunda")]
        Monday = 1,
        [Description("Terça")]
        Tuesday = 2,
        [Description("Quarta")]
        Wednesday = 3,
        [Description("Quinta")]
        Thursday = 4,
        [Description("Sexta")]
        Friday = 5,
        [Description("Sábado")]
        Saturday = 6,
        [Description("Domingo")]
        Sunday = 7,
    }
}
