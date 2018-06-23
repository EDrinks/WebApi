using System;

namespace EDrinks.Events.Products
{
    public class ProductNameChanged : BaseEvent
    {
        public Guid ProductId { get; set; }
        
        public string Name { get; set; }
    }
}