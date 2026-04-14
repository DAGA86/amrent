using System.ComponentModel;

namespace AMRent.Data.Enums
{
    public enum ViaVerdeMovementStatus
    {
        [Description("Registado")]
        Registered = 10,
        [Description("Rejeitado - Reboque")]
        RejectedTow = 20,
        [Description("Rejeitado - Cobrança impossível")]
        RejectedNotPayed = 30,
        [Description("Validado")]
        Validated = 40
    }
}
