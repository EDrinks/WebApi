using System;

namespace EDrinks.Events.Spendings
{
    public class ProductOrderedOnSpending : BaseEvent
    {
        public Guid OrderId { get; set; }
        
        public Guid SpendingId { get; set; }

        public int Quantity { get; set; }
    }
}