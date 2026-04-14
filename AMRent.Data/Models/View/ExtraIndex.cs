namespace AMRent.Data.Models.View
{
    public class ExtraIndex
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal ValueInsuranceLevel1 { get; set; }
        public decimal ValueInsuranceLevel2 { get; set; }
        public decimal ValueInsuranceLevel3 { get; set; }
        public bool IsActive { get; set; }
    }
}
