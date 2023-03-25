using System;

namespace Ridvay.Azure.ServiceBus.Client.Abstractions.FunctionTrigger
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class FunctionTimerTriggerAttribute : Attribute
    {
        public FunctionTimerTriggerAttribute(string functionName, string timerTrigger, Type messageType)
        {
        }
    }
}
