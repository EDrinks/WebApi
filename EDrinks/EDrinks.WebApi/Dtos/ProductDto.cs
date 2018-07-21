using System.ComponentModel.DataAnnotations;

namespace EDrinks.WebApi.Dtos
{
    public class ProductDto
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Name { get; set; }

        [Range(0, int.MaxValue)]
        [RegularExpression(@"\d+(\.\d{1,2})?", ErrorMessage = "Invalid price")]
        public decimal Price { get; set; }
    }
}