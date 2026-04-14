using System.ComponentModel;

namespace AMRent.Data.Enums
{
    public enum DiscountTypes
    {
        [Description("Percentagem")]
        Percentage = 1,
        [Description("Euro")]
        Euro = 2,
        [Description("Extras")]
        Extra = 3
    }
}
