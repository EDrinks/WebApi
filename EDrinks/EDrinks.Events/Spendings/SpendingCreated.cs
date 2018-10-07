using System;

namespace EDrinks.Events.Spendings
{
    public class SpendingCreated : BaseEvent
    {
        public Guid SpendingId { get; set; }

        public Guid TabId { get; set; }

        public Guid ProductId { get; set; }

        public int Quantity { get; set; }
    }
}