using System.ComponentModel.DataAnnotations;

namespace CityInfo.Models
{
    public class PointOfInterestForUpdateDto
    {
        [Required(ErrorMessage = "Ypu should provide a name value")]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;
        [MaxLength(200)]
        public string? Description { get; set; }
    }
}
