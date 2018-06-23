using System;

namespace EDrinks.Events.Products
{
    public class ProductPriceChanged : BaseEvent
    {
        public Guid ProductId { get; set; }

        public decimal Price { get; set; }
        
        public override string GetEventName()
        {
            return "ProductPriceChanged";
        }
    }
}