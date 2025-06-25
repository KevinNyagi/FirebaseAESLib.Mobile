// =============================
// FILE: FirestoreRestClient.cs
// =============================
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace FirebaseAESLib.Mobile
{
    public class FirestoreRestClient
    {
        private readonly HttpClient _httpClient = new();
        private readonly string _baseUrl;
        private readonly string _apiKey;
        private readonly AesEncryptor _aes;

        public FirestoreRestClient(string projectId, string apiKey, AesEncryptor aes)
        {
            _baseUrl = $"https://firestore.googleapis.com/v1/projects/{projectId}/databases/(default)/documents/";
            _apiKey = apiKey;
            _aes = aes;
        }

        public async Task SetEncryptedAsync(string collection, string documentId, Dictionary<string, object> data)
        {
            var encrypted = _aes.EncryptDictionary(data);
            var firestoreDoc = new Dictionary<string, object> { { "fields", ToFirestoreFields(encrypted) } };

            var json = JsonSerializer.Serialize(firestoreDoc);
            var url = $"{_baseUrl}{collection}/{documentId}?key={_apiKey}";
            var response = await _httpClient.PatchAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();
        }

        public async Task<Dictionary<string, object>> GetDecryptedAsync(string collection, string documentId)
        {
            var url = $"{_baseUrl}{collection}/{documentId}?key={_apiKey}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);

            var fields = doc.RootElement.GetProperty("fields");
            var raw = FromFirestoreFields(fields);
            return _aes.DecryptDictionary(raw);
        }

        private static Dictionary<string, object> ToFirestoreFields(Dictionary<string, object> data)
        {
            var fields = new Dictionary<string, object>();
            foreach (var kvp in data)
            {
                fields[kvp.Key] = new Dictionary<string, object>
                {
                    { "stringValue", kvp.Value.ToString() ?? string.Empty }
                };
            }
            return fields;
        }

        private static Dictionary<string, object> FromFirestoreFields(JsonElement fields)
        {
            var result = new Dictionary<string, object>();
            foreach (var field in fields.EnumerateObject())
            {
                if (field.Value.TryGetProperty("stringValue", out var value))
                    result[field.Name] = value.GetString() ?? "";
            }
            return result;
        }
    }
}
