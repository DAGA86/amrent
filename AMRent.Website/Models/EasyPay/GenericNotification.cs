namespace AMRent.Website.Models.EasyPay
{
    public class GenericNotification
    {
        public string Id { get; set; }
        public string Key { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public List<string> Messages { get; set; }
        public string Date { get; set; }
    }
}
