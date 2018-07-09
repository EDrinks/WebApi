using EDrinks.Common;
using EDrinks.EventSource;

namespace EDrinks.WebApi.Services
{
    public class StreamResolver : IStreamResolver
    {
        public string GetStream()
        {
            return "edrinks";
        }
    }
}