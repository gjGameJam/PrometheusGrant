using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace ApiTests.ApiClient
{
    // Wrapper carrying both raw HttpResponseMessage and a deserialized body + raw content string.
    // This type owns the RawResponse and will dispose it when disposed.
    // Consumers should dispose ApiResponse when finished (or use 'using').
    public sealed class ApiResponse<T> : IDisposable //If it is not disposed, underlying resources (sockets, stream buffers) may remain open until GC finalizes them — 
    // this can lead to connection leaks and socket exhaustion under heavy test runs.
    {
        public HttpResponseMessage RawResponse { get; }
        public T? Body { get; }
        public string Content { get; }
        public HttpStatusCode StatusCode => RawResponse.StatusCode;
        public bool IsSuccess => RawResponse.IsSuccessStatusCode;
        public string? ReasonPhrase => RawResponse.ReasonPhrase;
        public HttpResponseHeaders Headers => RawResponse.Headers;

        // Constructs an ApiResponse wrapping the given raw response, deserialized body, and raw content string
        public ApiResponse(HttpResponseMessage rawResponse, T? body, string content)
        {
            RawResponse = rawResponse ?? throw new ArgumentNullException(nameof(rawResponse));
            Body = body;
            Content = content ?? string.Empty;
        }

        // Return the first header value for the given name, or null if not present.
        public string? GetHeader(string name)
            => Headers.TryGetValues(name, out var values) ? values.FirstOrDefault() : null;

        // Short debug string: status + truncated content preview.
        public override string ToString()
        {
            const int previewLength = 200;
            var preview = Content.Length <= previewLength ? Content : Content.Substring(0, previewLength) + "...";
            return $"Status: {(int)StatusCode} ({StatusCode}), BodyPresent: {Body != null}, ContentPreview: {preview}";
        }

        #region Disposal
        private bool _disposed;

        //dispose pattern to clean up HttpClient
        public void Dispose()
        {
            if (_disposed) return;
            RawResponse?.Dispose();
            _disposed = true;
        }
        #endregion
    }
}
