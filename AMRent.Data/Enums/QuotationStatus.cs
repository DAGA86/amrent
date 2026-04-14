using System.ComponentModel;

namespace AMRent.Data.Enums
{
    public enum QuotationStatus
    {
        [Description("Pedido")]
        Requested = 4,
        [Description("Registado")]
        Registered = 10,
        [Description("Finalizado")]
        Finished = 20,
        [Description("Cancelado")]
        Cancelled = 30
    }
}
