using System;
using System.Text;
using Newtonsoft.Json;

namespace Common
{
    public interface IEncryptor
    {
        string Encrypt(object data);
        T Decrypt<T>(string stringData);
    }

    public class Encryptor : IEncryptor
    {
        public string Encrypt(object data)
        {
            var jsonString = JsonConvert.SerializeObject(data);
            var jsonBytes = Encoding.UTF8.GetBytes(jsonString);
            return Convert.ToBase64String(jsonBytes);
        }

        public T Decrypt<T>(string stringData)
        {
            try
            {
                var jsonBytes = Convert.FromBase64String(stringData);
                var jsonString = Encoding.UTF8.GetString(jsonBytes);
                return JsonConvert.DeserializeObject<T>(jsonString);
            }
            catch
            {
                return default(T);
            }
        }
    }
}