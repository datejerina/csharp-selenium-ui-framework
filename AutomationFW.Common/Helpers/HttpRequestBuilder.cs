using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AutomationFW.Common.Helpers
{
    public class HttpRequestBuilder
    {
        private HttpMethod _httpMethod = null;
        private string _requestUri = string.Empty;
        private HttpContent _content = null;
        private string _acceptHeader = "application/json";

        private static HttpClient _httpClient = new HttpClient(
            new HttpClientHandler() { UseDefaultCredentials = true });

        public HttpRequestBuilder()
        {
        }
        public HttpRequestBuilder AddMethod(HttpMethod method)
        {
            _httpMethod = method;
            return this;
        }

        public HttpRequestBuilder AddRequestUri(string requestUri)
        {
            _requestUri = requestUri;
            return this;
        }

        public HttpRequestBuilder AddContent(HttpContent content)
        {
            _content = content;
            return this;
        }

        public HttpRequestBuilder AddAcceptHeader(string acceptHeader)
        {
            _acceptHeader = acceptHeader;
            return this;
        }

        public async Task<HttpResponseMessage> SendAsync()
        {
            var request = new HttpRequestMessage
            {
                Method = _httpMethod,
                RequestUri = new Uri(_requestUri)
            };

            if (_content != null)
            {
                request.Content = _content;
            }

            request.Headers.Accept.Clear();

            if (!string.IsNullOrEmpty(_acceptHeader))
            {
                request.Headers.Accept.Add(
                   new MediaTypeWithQualityHeaderValue(_acceptHeader));
            }

            return await _httpClient.SendAsync(request);
        }
    }
}
