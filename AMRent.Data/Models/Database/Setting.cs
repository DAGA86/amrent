using System.ComponentModel.DataAnnotations;

namespace AMRent.Data.Models.Database
{
    public class Setting
    {
        public int Id { get; set; }
        [StringLength(128)]
        public string Key { get; set; }
        public string Value { get; set; }
        [StringLength(256)]
        public string Description { get; set; }
    }
}
