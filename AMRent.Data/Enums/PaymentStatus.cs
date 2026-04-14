using System.ComponentModel;

namespace AMRent.Data.Enums
{
    public enum PaymentStatus
    {
        [Description("Pendente")]
        Pending = 1,
        [Description("Pago")]
        Paid = 2,
        [Description("Falhado")]
        Failed = 3
    }
}
