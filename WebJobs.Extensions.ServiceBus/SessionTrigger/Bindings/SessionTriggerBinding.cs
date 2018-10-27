// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Extensions.ServiceBusSession.Listeners;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Listeners;
using Microsoft.Azure.WebJobs.Host.Protocols;
using Microsoft.Azure.WebJobs.Host.Triggers;
using Microsoft.Extensions.Logging;

namespace Microsoft.Azure.WebJobs.Extensions.ServiceBusSession.Bindings
{
    internal class SessionTriggerBinding : ITriggerBinding
    {
        private readonly ParameterInfo _parameter;
        private readonly SessionTriggerAttribute _attribute;       
        private readonly HandlerOptions _options;
        private readonly ILogger _logger;       
        private IReadOnlyDictionary<string, Type> _bindingContract;
        private string _handlerName;

        public SessionTriggerBinding(ParameterInfo parameter, SessionTriggerAttribute attribute, HandlerOptions options, ILogger logger)
        {
            _attribute = attribute;
        
            _parameter = parameter;
            _options = options;
            _logger = logger;
          
            _bindingContract = CreateBindingDataContract();

            MethodInfo methodInfo = (MethodInfo)parameter.Member;
            _handlerName = string.Format("{0}.{1}", methodInfo.DeclaringType.FullName, methodInfo.Name);
        }

        public Type TriggerValueType
        {
            get
            {
                return typeof(SessionInfo);
            }
        }

        public IReadOnlyDictionary<string, Type> BindingDataContract
        {
            get { return _bindingContract; }
        }

        public async Task<ITriggerData> BindAsync(object value, ValueBindingContext context)
        {
            await Task.CompletedTask;
            SessionInfo sessionInfo = value as SessionInfo;
            if (sessionInfo == null)
            {
              
                sessionInfo = new SessionInfo();
            }

            IValueProvider valueProvider = new ValueProvider(sessionInfo);
            IReadOnlyDictionary<string, object> bindingData = CreateBindingData();

            return new TriggerData(valueProvider, bindingData);
        }

        public Task<IListener> CreateListenerAsync(ListenerFactoryContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            return Task.FromResult<IListener>(new SessionListener(_attribute,  _handlerName, _options, context.Executor, _logger));
        }

        public ParameterDescriptor ToParameterDescriptor()
        {
            SessionTriggerParameterDescriptor descriptor = new SessionTriggerParameterDescriptor
            {
                Name = _parameter.Name,
                DisplayHints = new ParameterDisplayHints
                {
                    Description = string.Format("New message received for handler:({0})", _handlerName)
                }
            };
            return descriptor;
        }

        private IReadOnlyDictionary<string, Type> CreateBindingDataContract()
        {
            Dictionary<string, Type> contract = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
            contract.Add("SessionTrigger", typeof(string));

            return contract;
        }

        private IReadOnlyDictionary<string, object> CreateBindingData()
        {
            Dictionary<string, object> bindingData = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            bindingData.Add("SessionTrigger", DateTime.Now.ToString());

            return bindingData;
        }

        private class ValueProvider : IValueProvider
        {
            private readonly object _value;

            public ValueProvider(object value)
            {
                _value = value;
            }

            public Type Type
            {
                get { return typeof(SessionInfo); }
            }

            public Task<object> GetValueAsync()
            {
                return Task.FromResult(_value);
            }

            public string ToInvokeString()
            {
                return DateTime.Now.ToString("o");
            }
        }

        private class SessionTriggerParameterDescriptor : TriggerParameterDescriptor
        {
            public override string GetTriggerReason(IDictionary<string, string> arguments)
            {
                return string.Format("New message session received at {0}", DateTime.Now.ToString("o"));
            }
        }
    }
}
