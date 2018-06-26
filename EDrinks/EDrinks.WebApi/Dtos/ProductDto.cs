using System.ComponentModel.DataAnnotations;

namespace EDrinks.WebApi.Dtos
{
    public class ProductDto
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Name { get; set; }

        [Range(0, int.MaxValue)]
        public decimal Price { get; set; }
    }
}