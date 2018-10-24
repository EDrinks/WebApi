using System;

namespace EDrinks.Events.Spendings
{
    public class OrderOnSpendingDeleted : BaseEvent
    {
        public Guid OrderId { get; set; }

        public Guid SpendingId { get; set; }
    }
}