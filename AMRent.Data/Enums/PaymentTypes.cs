using System.ComponentModel;

namespace AMRent.Data.Enums
{
    public enum PaymentTypes
    {
        [Description("Transferência bancária")]
        BankTransfer = 1,
        [Description("MB Way")]
        MBWay = 2,
        [Description("Referência MB")]
        MBReference = 3,
        [Description("Cartão")]
        CreditCard = 4,
        [Description("Paypal")]
        Paypal = 5,
        [Description("Conta corrente")]
        PostPaid = 6,
        [Description("Numerário")]
        Cash = 7,
    }
}
