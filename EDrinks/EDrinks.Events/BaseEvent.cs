using Newtonsoft.Json;

namespace EDrinks.Events
{
    public abstract class BaseEvent
    {
        [JsonIgnore]
        public MetaData MetaData { get; set; }

        protected BaseEvent()
        {
            MetaData = new MetaData();
        }
    }

    public class MetaData
    {
        public string CreatedBy { get; set; } = "system";
    }
}