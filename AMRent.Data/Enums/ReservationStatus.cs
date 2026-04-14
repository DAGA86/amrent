using System.ComponentModel;

namespace AMRent.Data.Enums
{
    public enum ReservationStatus
    {
        //[Description("Cotação")]
        //Quotation = 10,
        [Description("Registado")]
        Registered = 20,
        [Description("Confirmado")]
        Confirmed = 30,
        [Description("Finalizado")]
        Finished = 40,
        [Description("Cancelado")]
        Cancelled = 50
    }
}
