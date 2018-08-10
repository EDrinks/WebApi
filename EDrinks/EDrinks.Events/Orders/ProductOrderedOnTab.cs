using System;

namespace EDrinks.Events.Orders
{
    public class ProductOrderedOnTab : BaseEvent
    {
        public Guid OrderId { get; set; }
        
        public Guid TabId { get; set; }

        public Guid ProductId { get; set; }

        public int Quantity { get; set; }
    }
}