using System.Threading.Tasks;
using Xunit;

namespace EDrinks.Test.Integration.Workflows
{
    public class SettlementWorkflows : ServiceTest
    {
        public SettlementWorkflows(ServiceFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task TestAfterSettlementTabShouldNotHaveActiveOrders()
        {
            
        }
    }
}