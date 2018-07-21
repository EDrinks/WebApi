using System;

namespace EDrinks.Events.Tabs
{
    public class TabNameChanged : BaseEvent
    {
        public Guid TabId { get; set; }

        public string Name { get; set; }
    }
}