﻿using System;

namespace EDrinks.QueryHandlers.Model
{
    public class Order
    {
        public Guid Id { get; set; }

        public DateTime DateTime { get; set; }

        public Guid ProductId { get; set; }

        public int Quantity { get; set; }
    }
}