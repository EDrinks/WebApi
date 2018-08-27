using System;

namespace EDrinks.Events.Tabs
{
    public class TabSettled
    {
        public Guid SettlementId { get; set; }

        public Guid TabId { get; set; }
    }
}