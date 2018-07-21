using System;
using EDrinks.Events.Tabs;

namespace EDrinks.QueryHandlers.Model
{
    public class Tab
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public void Apply(TabCreated tabCreated)
        {
            Id = tabCreated.TabId;
        }

        public void Apply(TabNameChanged tabNameChanged)
        {
            Name = tabNameChanged.Name;
        }
    }
}