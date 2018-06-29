using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace EDrinks.Test.Integration
{
    [Collection("Service")]
    public abstract class ServiceTest
    {
        protected readonly ServiceFixture _fixture;

        public ServiceTest(ServiceFixture fixture)
        {
            _fixture = fixture;
        }

        protected async Task<T> Deserialize<T>(HttpResponseMessage response)
        {
            return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
        }

        protected dynamic Parse(HttpResponseMessage response)
        {
            return JObject.Parse(response.Content.ReadAsStringAsync().Result);
        }

        protected HttpContent Serialize(object data)
        {
            var jsonContent = JsonConvert.SerializeObject(data);

            var buffer = System.Text.Encoding.UTF8.GetBytes(jsonContent);
            
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return byteContent;
        }
    }
}