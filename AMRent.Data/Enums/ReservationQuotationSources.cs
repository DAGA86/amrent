using System.ComponentModel;

namespace AMRent.Data.Enums
{
    public enum ReservationQuotationSources
    {
        [Description("Website")]
        W = 1,
        [Description("Telefone")]
        T = 2,
        [Description("Email")]
        E = 3,
        [Description("Presencial")]
        P = 4,
        [Description("Meta")]
        M = 5
    }
}
