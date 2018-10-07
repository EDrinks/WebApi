using System;

namespace EDrinks.Events.Spendings
{
    public class SpendingClosed : BaseEvent
    {
        public Guid SpendingId { get; set; }
    }
}