using System.ComponentModel.DataAnnotations;

namespace EDrinks.WebApi.Dtos
{
    public class TabDto
    {
        [Required]
        [StringLength(150, MinimumLength = 3)]
        public string Name { get; set; }
    }
}