using System;

namespace EDrinks.QueryHandlers.Model
{
    public class Spending
    {
        public Guid Id { get; set; }

        public Guid TabId { get; set; }

        public Guid ProductId { get; set; }

        public int Quantity { get; set; }

        public int Current { get; set; }
    }
}