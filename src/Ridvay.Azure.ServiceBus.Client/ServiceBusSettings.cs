using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using Azure.Messaging.ServiceBus;

[assembly: InternalsVisibleTo("Ridvay.Azure.ServiceBus.Client.UnitTest")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
namespace Ridvay.Azure.ServiceBus.Client
{
    public class ServiceBusSettings
    {
        public string ConnectionString { get; set; }

        public ServiceBusClientOptions ClientOptions { get; set; }
    }
    

}
