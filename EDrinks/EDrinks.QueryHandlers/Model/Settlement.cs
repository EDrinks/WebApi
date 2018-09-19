using System;
using System.Collections.Generic;

namespace EDrinks.QueryHandlers.Model
{
    public class Settlement
    {
        public Guid Id { get; set; }

        public DateTime DateTime { get; set; }

        public List<TabToOrders> TabToOrders { get; set; }

        public Settlement()
        {
            TabToOrders = new List<TabToOrders>();
        }
    }

    public class TabToOrders
    {
        public Tab Tab { get; set; }

        public List<Order> Orders { get; set; }

        public TabToOrders()
        {
            Orders = new List<Order>();
        }
    }
}