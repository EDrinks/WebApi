using System;

namespace EDrinks.Events.Tabs
{
    public class TabDeleted : BaseEvent
    {
        public Guid TabId { get; set; }
    }
}