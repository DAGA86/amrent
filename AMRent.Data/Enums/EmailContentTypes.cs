using System.ComponentModel;

namespace AMRent.Data.Enums
{
    public enum EmailContentTypes
    {
        [Description("Registo de reserva")]
        ReservationRegistration = 1,
        [Description("Registo de reserva a partir de cotação")]
        ReservationRegistrationFromQuotation = 8,
        [Description("Escolha de pagamento de reserva a partir de cotação")]
        PaymentChoiceReservationFromQuotation = 9,
        [Description("Confirmação de pagamento")]
        PaymentConfirmation = 2,
        [Description("Cancelamento de reserva")]
        ReservationCancellation = 3,
        [Description("Registo de cotação")]
        QuotationRegistration = 4,
        [Description("Aprovação de reserva")]
        ReservationApproval = 5,
        [Description("Falha no pagamento")]
        PaymentFailure = 6,
        [Description("Reserva concluída")]
        ReservationFinished = 7,
        [Description("Recuperação de password")]
        PasswordReset = 10,
        [Description("Cotação a expirar")]
        QuotationExpiring = 11,
        [Description("Conta registada")]
        AccountRegistered = 12
    }
}
