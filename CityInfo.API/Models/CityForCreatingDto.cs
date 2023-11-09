using System.ComponentModel.DataAnnotations;

namespace CityInfo.API.Models
{
    public class CityForCreatingDto
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(200)]
        public string? Description { get; set; }
    }
}
