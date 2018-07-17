using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using EDrinks.Events;
using EventStore.ClientAPI;
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

        protected void DeleteStream()
        {
            _fixture.Connection.DeleteStreamAsync(_fixture.StreamResolver.GetStream(), ExpectedVersion.Any, false).Wait();
        }

        protected async Task WriteToStream(BaseEvent evt)
        {
            var metaDataStr = JsonConvert.SerializeObject(evt.MetaData);
            var contentStr = JsonConvert.SerializeObject(evt);

            var eventData = new EventData(Guid.NewGuid(), evt.GetType().Name, true,
                Encoding.UTF8.GetBytes(contentStr), Encoding.UTF8.GetBytes(metaDataStr));

            await _fixture.Connection.AppendToStreamAsync(_fixture.StreamResolver.GetStream(), ExpectedVersion.Any, eventData);
        }
    }
}