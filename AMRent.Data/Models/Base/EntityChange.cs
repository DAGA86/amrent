namespace AMRent.Data.Models.Base
{
    public class EntityChange
    {
        public int Id { get; set; }
        public string FieldName { get; set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public DateTime ChangeDateTime { get; set; }

        public Guid UserId { get; set; }
        public Database.User User { get; set; }
    }
}
