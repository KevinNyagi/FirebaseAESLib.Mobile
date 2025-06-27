
// =============================
// FILE: RealtimeRestClient.cs
// =============================
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace FirebaseAESLib.Mobile
{
    public class RealtimeRestClient
    {
        private readonly HttpClient _httpClient = new();
        private readonly string _baseUrl;
        private readonly string? _idToken;
        private readonly AesEncryptor _aes;

        public RealtimeRestClient(string projectId, string? idToken, AesEncryptor aes)
        {
            _baseUrl = $"https://{projectId}.firebaseio.com/";
            _idToken = idToken;
            _aes = aes;
        }

        //public async Task SetEncryptedAsync(string path, object data)
        //{
        //    var encrypted = _aes.EncryptObject(data);
        //    var json = JsonSerializer.Serialize(encrypted);
        //    var url = BuildUrl(path);
        //    var response = await _httpClient.PutAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));
        //    response.EnsureSuccessStatusCode();
        //}
        public async Task<string?> PushEncryptedAsync(string path, object data)
        {
            var encrypted = _aes.EncryptObject(data);
            var json = JsonSerializer.Serialize(encrypted);
            var url = BuildUrl(path);

            var response = await _httpClient.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            var doc = JsonSerializer.Deserialize<Dictionary<string, string>>(result);
            return doc != null && doc.TryGetValue("name", out var key) ? key : null;
        }
        public async Task<object?> GetDecryptedAsync(string path)
        {
            var url = BuildUrl(path);
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();

            var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            return _aes.DecryptObject(dict);
        }

        public string BuildUrl(string path)
        {
            var url = _baseUrl + path.Trim('/') + ".json";
            if (!string.IsNullOrEmpty(_idToken))
                url += "?auth=" + _idToken;
            return url;
        }
    }
}