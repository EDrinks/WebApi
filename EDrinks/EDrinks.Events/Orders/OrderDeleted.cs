using System;

namespace EDrinks.Events.Orders
{
    public class OrderDeleted : BaseEvent
    {
        public Guid OrderId { get; set; }
    }
}