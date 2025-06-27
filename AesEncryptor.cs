// =============================
// FILE: AesEncryptor.cs
// =============================
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace FirebaseAESLib.Mobile
{
    public class AesEncryptor
    {
        private readonly byte[] _key;
        private readonly byte[] _iv;

        public AesEncryptor(string base64Key, string base64IV)
        {
            _key = Convert.FromBase64String(base64Key);
            _iv = Convert.FromBase64String(base64IV);
        }

        // Encrypt a string to Base64
        public string EncryptString(string plainText)
        {
            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;

            var encryptor = aes.CreateEncryptor();
            var input = Encoding.UTF8.GetBytes(plainText);
            var output = encryptor.TransformFinalBlock(input, 0, input.Length);

            return Convert.ToBase64String(output);
        }

        // Decrypt a Base64 string back to plaintext
        public string DecryptString(string cipherText)
        {
            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;

            var decryptor = aes.CreateDecryptor();
            var input = Convert.FromBase64String(cipherText);
            var output = decryptor.TransformFinalBlock(input, 0, input.Length);

            return Encoding.UTF8.GetString(output);
        }

        // Generic encrypt object
        public object? EncryptObject(object? obj)
        {
            if (obj == null) return null;
            return obj switch
            {
                string str => EncryptString(str),
                Dictionary<string, object> dict => EncryptDictionary(dict),
                IEnumerable<object> list => EncryptList(list),
                _ => EncryptString(obj.ToString() ?? "")
            };
        }

        // Generic decrypt object
        public object? DecryptObject(object? obj)
        {
            if (obj == null) return null;
            return obj switch
            {
                string str => TrySafeDecryptString(str),
                Dictionary<string, object> dict => DecryptDictionary(dict),
                IEnumerable<object> list => DecryptList(list),
                _ => TrySafeDecryptString(obj.ToString() ?? "")
            };
        }

        // Encrypt a dictionary recursively
        public Dictionary<string, object> EncryptDictionary(Dictionary<string, object> input)
        {
            var result = new Dictionary<string, object>();
            foreach (var kvp in input)
                result[kvp.Key] = EncryptObject(kvp.Value);
            return result;
        }

        // Decrypt a dictionary recursively
        public Dictionary<string, object> DecryptDictionary(Dictionary<string, object> input)
        {
            var result = new Dictionary<string, object>();
            foreach (var kvp in input)
                result[kvp.Key] = DecryptObject(kvp.Value);
            return result;
        }

        // Encrypt a list of objects
        private List<object> EncryptList(IEnumerable<object> list)
        {
            var result = new List<object>();
            foreach (var item in list)
                result.Add(EncryptObject(item));
            return result;
        }

        // Decrypt a list of objects
        private List<object> DecryptList(IEnumerable<object> list)
        {
            var result = new List<object>();
            foreach (var item in list)
                result.Add(DecryptObject(item));
            return result;
        }

        // Try-decrypt a string, fallback to original if not base64
        private string TrySafeDecryptString(string value)
        {
            try
            {
                return DecryptString(value);
            }
            catch (FormatException)
            {
                return value; // not base64 or not encrypted
            }
            catch (CryptographicException)
            {
                return value; // decryption failed, fallback
            }
        }
    }
}
