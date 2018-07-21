using System;

namespace EDrinks.Events.Tabs
{
    public class TabCreated : BaseEvent
    {
        public Guid TabId { get; set; }
    }
}