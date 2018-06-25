using System;

namespace EDrinks.Events.Products
{
    public class ProductDeleted : BaseEvent
    {
        public Guid ProductId { get; set; }
    }
}