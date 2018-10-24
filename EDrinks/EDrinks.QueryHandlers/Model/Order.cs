using System;

namespace EDrinks.QueryHandlers.Model
{
    public class Order
    {
        public Guid Id { get; set; }

        public DateTime DateTime { get; set; }

        public Guid ProductId { get; set; }

        public Guid TabId { get; set; }

        public Guid? SpendingId { get; set; }

        public int Quantity { get; set; }

        public decimal ProductPrice { get; set; }
    }
}