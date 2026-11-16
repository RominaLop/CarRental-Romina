
using System.ComponentModel.DataAnnotations;

namespace CarRentalServices.Entities
{
    public class Country
    {
        [Key]
        public int CountryId { get; set; }

        public string CountryName { get; set; }
    }
}
