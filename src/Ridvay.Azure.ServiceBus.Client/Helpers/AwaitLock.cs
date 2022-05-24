using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Ridvay.Azure.ServiceBus.Client.Helpers
{
    /// <summary>
    /// awaitable Lock
    /// </summary>
    public class AwaitLock : IAsyncDisposable
    {
        private readonly SemaphoreSlim _semaphoreSlim;
        
        private static Dictionary<string, SemaphoreSlim> _waiters;

        private static readonly object Lock = new object();
        private AwaitLock(string name)
        {
            lock (Lock)
            {
                _waiters ??= new Dictionary<string, SemaphoreSlim>();

                if (_waiters.ContainsKey(name))
                {
                    _semaphoreSlim = _waiters[name];
                }
                else
                {
                    _semaphoreSlim = new SemaphoreSlim(1,1);
                    _waiters.Add(name, _semaphoreSlim);
                }
            }
        }

        public static async Task<AwaitLock> Create(string name)
        {
            var retValue = new AwaitLock(name);
            await retValue.WaitAsync();
            return retValue;
        }
        public async Task WaitAsync()
        {
            await _semaphoreSlim.WaitAsync();
        }
        public ValueTask DisposeAsync()
        {
           _semaphoreSlim.Release();

           return ValueTask.CompletedTask;
        }
    }
}
