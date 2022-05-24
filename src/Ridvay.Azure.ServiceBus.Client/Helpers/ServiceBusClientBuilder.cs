using System.Collections;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Ridvay.Azure.ServiceBus.Client.Helpers
{
    public class ServiceBusClientBuilder : IServiceCollection
    {
        private readonly IServiceCollection _services;

        public ServiceBusClientBuilder(IServiceCollection services)
        {
            _services = services;
        }

        public IEnumerator<ServiceDescriptor> GetEnumerator()
        {
            return _services.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(ServiceDescriptor item)
        {
            _services.Add(item);
        }

        public void Clear()
        {
            _services.Clear();
        }

        public bool Contains(ServiceDescriptor item)
        {
            return _services.Contains(item);
        }

        public void CopyTo(ServiceDescriptor[] array, int arrayIndex)
        {
            _services.CopyTo(array, arrayIndex);
        }

        public bool Remove(ServiceDescriptor item)
        {
            return _services.Remove(item);
        }

        public int Count => _services.Count;
        public bool IsReadOnly => _services.IsReadOnly;
        public int IndexOf(ServiceDescriptor item)
        {
            return _services.IndexOf(item);
        }

        public void Insert(int index, ServiceDescriptor item)
        {
            _services.Insert(index,item);
        }

        public void RemoveAt(int index)
        {
            _services.RemoveAt(index);
        }

        public ServiceDescriptor this[int index]
        {
            get => _services[index];
            set => _services[index]=value;
        }
    }
}