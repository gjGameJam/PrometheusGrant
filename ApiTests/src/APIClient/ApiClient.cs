using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace ApiTests.ApiClient
{
    //wrapper around HttpClient to simplify REST API calls
    public sealed class ApiClient : IDisposable // Implements IDisposable to allow cleanup of HttpClient
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        private bool _disposed;

        /// Initializes ApiClient with configuration settings.
        public ApiClient(IConfiguration config)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));

            var baseUrl = config["ApiConfig:BaseUrl"] ?? throw new InvalidOperationException("ApiConfig:BaseUrl missing in configuration");
            var timeoutSeconds = int.TryParse(config["ApiConfig:TimeoutSeconds"], out var t) ? t : 30;

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(baseUrl),
                Timeout = TimeSpan.FromSeconds(timeoutSeconds)
            };

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        #region Typed convenience methods (happy-path, throws on non-success)
        //get, post, put, delete with automatic JSON serialization/deserialization
        //When we only care about the deserialized body
        public async Task<T?> GetAsync<T>(string endpoint)
        {
            var response = await _httpClient.GetAsync(endpoint).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonSerializer.Deserialize<T?>(json, _jsonOptions);
        }

        public async Task<T?> PostAsync<T>(string endpoint, object data)
        {
            var json = JsonSerializer.Serialize(data, _jsonOptions);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(endpoint, content).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonSerializer.Deserialize<T?>(responseJson, _jsonOptions);
        }

        public async Task<T?> PutAsync<T>(string endpoint, object data)
        {
            var json = JsonSerializer.Serialize(data, _jsonOptions);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync(endpoint, content).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonSerializer.Deserialize<T?>(responseJson, _jsonOptions);
        }

        public async Task<bool> DeleteAsync(string endpoint)
        {
            var response = await _httpClient.DeleteAsync(endpoint).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            return response.IsSuccessStatusCode;
        }

        #endregion

        #region Raw response methods (do NOT throw, return ApiResponse<T> for assertions)

        /// Returns ApiResponse (may contain empty content). Caller must dispose the returned ApiResponse to free HttpResponseMessage resources.
        // When we care about the entire HTTP response (status code, success flag, headers, and maybe body).
        public async Task<ApiResponse<T?>> GetWithResponseAsync<T>(string endpoint)
        {
            var response = await _httpClient.GetAsync(endpoint).ConfigureAwait(false);
            var content = await SafeReadContentAsync(response).ConfigureAwait(false);

            T? body = response.IsSuccessStatusCode ? TryDeserialize<T?>(content) : default;
            return new ApiResponse<T?>(response, body, content);
        }


        public async Task<ApiResponse<T?>> PostWithResponseAsync<T>(string endpoint, object data)
        {
            var json = JsonSerializer.Serialize(data, _jsonOptions);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(endpoint, content).ConfigureAwait(false);
            var responseContent = await SafeReadContentAsync(response).ConfigureAwait(false);

            T? body = TryDeserialize<T?>(responseContent);
            return new ApiResponse<T?>(response, body, responseContent);
        }

        public async Task<ApiResponse<T?>> PutWithResponseAsync<T>(string endpoint, object data)
        {
            var json = JsonSerializer.Serialize(data, _jsonOptions);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync(endpoint, content).ConfigureAwait(false);
            var responseContent = await SafeReadContentAsync(response).ConfigureAwait(false);

            T? body = TryDeserialize<T?>(responseContent);
            return new ApiResponse<T?>(response, body, responseContent);
        }

        /// Returns ApiResponse (may contain empty content). Caller must dispose the returned ApiResponse to free HttpResponseMessage resources.
        public async Task<ApiResponse<T?>> DeleteWithResponseAsync<T>(string endpoint)
        {
            var response = await _httpClient.DeleteAsync(endpoint).ConfigureAwait(false);
            var content = await SafeReadContentAsync(response).ConfigureAwait(false);

            T? body = TryDeserialize<T?>(content);
            return new ApiResponse<T?>(response, body, content);
        }

        #endregion

        #region Helpers

        //helpers to safely read content and deserialize without throwing
        private async Task<string> SafeReadContentAsync(HttpResponseMessage response)
        {
            if (response.Content == null) return string.Empty;
            try
            {
                return await response.Content.ReadAsStringAsync().ConfigureAwait(false) ?? string.Empty;
            }
            catch
            {
                // If reading fails, return empty string so tests can still inspect status/headers
                return string.Empty;
            }
        }

        //helper to try deserializing JSON, returns default(T) on failure
        private T? TryDeserialize<T>(string content)
        {
            if (string.IsNullOrWhiteSpace(content)) return default;
            try
            {
                return JsonSerializer.Deserialize<T>(content, _jsonOptions);
            }
            catch
            {
                return default;
            }
        }

        #endregion

        #region Disposal

        //dispose pattern to clean up HttpClient
        public void Dispose()
        {
            if (_disposed) return;
            _httpClient?.Dispose();
            _disposed = true;
        }

        #endregion
    }
}
