using Newtonsoft.Json;
using System.Text;

namespace AutomationFW.Common.Helpers
{
    public static class HttpRequestFactory
    {
        public static async Task<HttpResponseMessage> Get(string requestUri)
        {
            var httpRequest = new HttpRequestBuilder()
                .AddMethod(HttpMethod.Get)
                .AddRequestUri(requestUri);

            return await httpRequest.SendAsync();
        }

        public static async Task<HttpResponseMessage> Post(string requestUri, object value)
        {
            var httpRequest = new HttpRequestBuilder()
                .AddMethod(HttpMethod.Post)
                .AddRequestUri(requestUri)
                .AddContent(new StringContent(JsonConvert.SerializeObject(value), Encoding.UTF8, "application/json"));

            return await httpRequest.SendAsync();
        }
    }
}
