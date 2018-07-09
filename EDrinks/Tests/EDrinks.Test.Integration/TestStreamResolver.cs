using EDrinks.Common;

namespace EDrinks.Test.Integration
{
    public class TestStreamResolver : IStreamResolver
    {
        public string GetStream()
        {
            return "edrinks-integration-test";
        }
    }
}