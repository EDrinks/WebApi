using System;

namespace EDrinks.Events.Products
{
    public class ProductCreated : BaseEvent
    {
        public Guid ProductId { get; set; }
        
        public override string GetEventName()
        {
            return "ProductCreated";
        }
    }
}