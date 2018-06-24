using System;
using EDrinks.Events.Products;

namespace EDrinks.QueryHandlers.Model
{
    public class Product
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public void Apply(ProductCreated productCreated)
        {
            this.Id = productCreated.ProductId;
        }

        public void Apply(ProductNameChanged productNameChanged)
        {
            this.Name = productNameChanged.Name;
        }

        public void Apply(ProductPriceChanged productPriceChanged)
        {
            this.Price = productPriceChanged.Price;
        }
    }
}