using Ridvay.Azure.ServiceBus.Client.Abstractions;

namespace Ridvay.Azure.ServiceBus.Client.UnitTest
{
    public class CustomMessageSerialize : IMessageSerialize
    {
        public string Serialize<T>(T obj)
        {
            return "👍";
        }

        public T Deserialize<T>(string obj)
        {
            return default;
        }
    }

    public class MessageConsumer : IMessageConsumer
    {
    }
}