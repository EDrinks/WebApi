using System.Threading.Tasks;
using Xunit;

namespace EDrinks.Test.Integration.Endpoints.ProductsController
{
    public class GetProducts : ServiceTest
    {
        public GetProducts(ServiceFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task TestGetProducts()
        {
            Assert.True(false);
        }
    }
}