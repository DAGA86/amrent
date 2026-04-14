using System.ComponentModel.DataAnnotations;

namespace AMRent.Data.Models.Database
{
    public class SocialNetwork
    {
        public int Id { get; set; }
        [StringLength(64)]
        public string Name { get; set; }
        [StringLength(256)]
        public string Url { get; set; }
        [StringLength(64)]
        public string FontAwesomeIconCode { get; set; }
    }
}
