using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EDrinks.Events
{
    public interface IEventLookup
    {
        Type GetType(string eventName);
    }
    
    public class EventLookup : IEventLookup
    {
        private readonly Dictionary<string, Type> _typeToEvent;
        
        public EventLookup()
        {
            var eventClasses = Assembly.GetAssembly(typeof(EventLookup))
                .GetTypes()
                .Where(e => e.IsClass && !e.IsAbstract && e.IsSubclassOf(typeof(BaseEvent)));
            
            _typeToEvent = new Dictionary<string, Type>();
            foreach (var eventClass in eventClasses)
            {
                _typeToEvent.Add(eventClass.Name, eventClass);
            }
        }

        public Type GetType(string eventName)
        {
            _typeToEvent.TryGetValue(eventName, out var type);
            return type;
        }
    }
}