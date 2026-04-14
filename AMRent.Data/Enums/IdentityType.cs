using System.ComponentModel;

namespace AMRent.Data.Enums
{
    public enum IdentityType
    {
        [Description("Reserva.Detalhe.DadosCondutor.TipoIdentificacao.CartaoCidadaoBilheteIdentidade")]
        CitizenCard = 1,
        [Description("Reserva.Detalhe.DadosCondutor.TipoIdentificacao.Passaporte")]
        Passport = 2
    }
}
