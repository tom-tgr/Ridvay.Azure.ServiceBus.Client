using System.Text.Json;

namespace Ridvay.Azure.ServiceBus.Client
{
    public interface IMessageSerialize
    {
        string Serialize<T>(T obj);
        T Deserialize<T>(string obj);
    }

    public class MessageSerialize : IMessageSerialize
    {
        public string Serialize<T>(T obj)
        {
            return JsonSerializer.Serialize(obj);
        }

        public T Deserialize<T>(string obj)
        {
            return JsonSerializer.Deserialize<T>(obj);
        }
    }
}