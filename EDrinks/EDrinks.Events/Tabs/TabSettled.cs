using System;

namespace EDrinks.Events.Tabs
{
    public class TabSettled : BaseEvent
    {
        public Guid SettlementId { get; set; }

        public Guid TabId { get; set; }
    }
}