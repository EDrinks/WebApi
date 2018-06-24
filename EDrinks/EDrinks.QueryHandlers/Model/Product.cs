using System;

namespace EDrinks.QueryHandlers.Model
{
    public class Product
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }
    }
}