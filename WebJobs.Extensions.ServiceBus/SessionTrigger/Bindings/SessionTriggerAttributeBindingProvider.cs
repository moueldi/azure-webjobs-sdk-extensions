// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Triggers;
using Microsoft.Extensions.Logging;

namespace Microsoft.Azure.WebJobs.Extensions.ServiceBusSession.Bindings
{
    internal class SessionTriggerAttributeBindingProvider : ITriggerBindingProvider
    {
        private readonly HandlerOptions _options;
        private readonly INameResolver _nameResolver;
        private readonly ILogger _logger;
        

        public SessionTriggerAttributeBindingProvider(HandlerOptions options, INameResolver nameResolver, ILogger logger)
        {
            _options = options;
            _nameResolver = nameResolver;
            _logger = logger;
            
        }

        public Task<ITriggerBinding> TryCreateAsync(TriggerBindingProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            ParameterInfo parameter = context.Parameter;
            SessionTriggerAttribute sessionTriggerAttribute = parameter.GetCustomAttribute<SessionTriggerAttribute>(inherit: false);

            if (sessionTriggerAttribute == null)
            {
                return Task.FromResult<ITriggerBinding>(null);
            }

            if (parameter.ParameterType != typeof(SessionInfo))
            {
                throw new InvalidOperationException(string.Format("Can't binds SessionTriggerAttribute to type '{0}'.", parameter.ParameterType));
            }

           

            return Task.FromResult<ITriggerBinding>(new SessionTriggerBinding(parameter, sessionTriggerAttribute, _options, _logger));
        }
    }
}
