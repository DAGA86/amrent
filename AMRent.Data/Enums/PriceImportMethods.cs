using System.ComponentModel;

namespace AMRent.Data.Enums
{
    public enum PriceImportMethods
    {
        [Description("A partir da temporada actual")]
        FromCurrentSeason = 1,
        [Description("A partir da próxima temporada")]
        FromNextSeason = 2,
        [Description("Só temporadas sem valor")]
        OnlyEmpty = 3
    }
}
