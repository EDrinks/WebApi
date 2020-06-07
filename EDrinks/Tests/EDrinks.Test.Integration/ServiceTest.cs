using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EDrinks.Events;
using EDrinks.EventSourceSql.Model;
using Microsoft.EntityFrameworkCore;
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

            var buffer = Encoding.UTF8.GetBytes(jsonContent);

            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return byteContent;
        }

        protected void DeleteStream()
        {
            _fixture.Context.DomainEvents.RemoveRange(_fixture.Context.DomainEvents);
            _fixture.Context.SaveChanges();
        }

        protected async Task WriteToStream(BaseEvent evt)
        {
            await _fixture.Context.DomainEvents.AddAsync(new DomainEvent()
            {
                CreatedOn = DateTime.UtcNow,
                CreatedBy = "system",
                EventType = evt.GetType().Name,
                Content = JsonConvert.SerializeObject(evt)
            });
            await _fixture.Context.SaveChangesAsync();
        }
    }
}