namespace AMRent.Data.Models.View
{
    public class CarSegmentIndex
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int Seats { get; set; }
        public string Fuel { get; set; }
        public string Gearbox { get; set; }
        public bool IsActive { get; set; }
        public bool HasReservations { get; set; }
    }
}
