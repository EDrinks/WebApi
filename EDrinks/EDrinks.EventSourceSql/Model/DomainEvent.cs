using System;

namespace EDrinks.EventSourceSql.Model
{
    public class DomainEvent
    {
        public long Id { get; set; }

        public DateTime CreatedOn { get; set; }

        public string CreatedBy { get; set; }

        public string EventType { get; set; }

        public string Content { get; set; }
    }
}