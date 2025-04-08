using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;

namespace nhom4_quanlyadmin.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApiService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        // ✅ Phương thức GET với token
        public async Task<T> GetAsync<T>(string endpoint)
        {
            var token = _httpContextAccessor.HttpContext.Session.GetString("JWTToken");
            if (token != null)
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return await _httpClient.GetFromJsonAsync<T>(endpoint);
        }
        public async Task<HttpResponseMessage> PostAsync<T>(string endpoint, T data)
        {
            var token = _httpContextAccessor.HttpContext.Session.GetString("JWTToken");
            if (token != null)
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return await _httpClient.PostAsJsonAsync(endpoint, data);
        }
        public async Task<HttpResponseMessage> PutAsync<T>(string endpoint, T data)
        {
            var token = _httpContextAccessor.HttpContext.Session.GetString("JWTToken");
            if (token != null)
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            return await _httpClient.PutAsJsonAsync(endpoint, data);
        }
        public async Task<HttpResponseMessage> DeleteAsync(string endpoint)
        {
            var token = _httpContextAccessor.HttpContext.Session.GetString("JWTToken");
            if (token != null)
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            return await _httpClient.DeleteAsync(endpoint);
        }
    }
}