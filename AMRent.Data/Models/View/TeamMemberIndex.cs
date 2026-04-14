namespace AMRent.Data.Models.View
{
    public class TeamMemberIndex
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Job { get; set; }
        public int SortNumber { get; set; }
        public bool IsLowestSortNumber { get; set; } = false;
        public bool IsHighestSortNumber { get; set; } = false;
    }
}
