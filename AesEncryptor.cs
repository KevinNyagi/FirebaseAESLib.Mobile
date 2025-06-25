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

        public string Encrypt(string plainText)
        {
            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;

            var encryptor = aes.CreateEncryptor();
            var input = Encoding.UTF8.GetBytes(plainText);
            var output = encryptor.TransformFinalBlock(input, 0, input.Length);

            return Convert.ToBase64String(output);
        }

        public string Decrypt(string cipherText)
        {
            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;

            var decryptor = aes.CreateDecryptor();
            var input = Convert.FromBase64String(cipherText);
            var output = decryptor.TransformFinalBlock(input, 0, input.Length);

            return Encoding.UTF8.GetString(output);
        }

        public object EncryptObject(object obj)
        {
            if (obj == null) return null;
            return obj switch
            {
                string str => Encrypt(str),
                Dictionary<string, object> dict => EncryptDictionary(dict),
                IEnumerable<object> list => EncryptList(list),
                _ => Encrypt(obj.ToString())
            };
        }

        public object DecryptObject(object obj)
        {
            if (obj == null) return null;
            return obj switch
            {
                string str => Decrypt(str),
                Dictionary<string, object> dict => DecryptDictionary(dict),
                IEnumerable<object> list => DecryptList(list),
                _ => Decrypt(obj.ToString())
            };
        }

        public Dictionary<string, object> EncryptDictionary(Dictionary<string, object> input)
        {
            var result = new Dictionary<string, object>();
            foreach (var kvp in input)
                result[kvp.Key] = EncryptObject(kvp.Value);
            return result;
        }

        public Dictionary<string, object> DecryptDictionary(Dictionary<string, object> input)
        {
            var result = new Dictionary<string, object>();
            foreach (var kvp in input)
                result[kvp.Key] = DecryptObject(kvp.Value);
            return result;
        }

        private List<object> EncryptList(IEnumerable<object> list)
        {
            var result = new List<object>();
            foreach (var item in list)
                result.Add(EncryptObject(item));
            return result;
        }

        private List<object> DecryptList(IEnumerable<object> list)
        {
            var result = new List<object>();
            foreach (var item in list)
                result.Add(DecryptObject(item));
            return result;
        }
    }
}

