using System;
using System.ComponentModel.DataAnnotations;

namespace EDrinks.WebApi.Dtos
{
    public class OrderDto
    {
        [Required]
        public Guid TabId { get; set; }

        [Required]
        public Guid ProductId { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}