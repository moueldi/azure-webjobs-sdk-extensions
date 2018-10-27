// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Microsoft.Azure.WebJobs.Description;
using Microsoft.Azure.WebJobs.Extensions.ServiceBusSession;
using Microsoft.Azure.WebJobs.Extensions.ServiceBusSession.Bindings;
using Microsoft.Azure.WebJobs.Host.Config;
using Microsoft.Azure.WebJobs.Logging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Microsoft.Azure.WebJobs.Extensions.Extensions.Timers
{
    [Extension("SessionMessages")]
    internal class SessionMessageExtensionConfigProvider : IExtensionConfigProvider
    {
        private readonly IOptions<HandlerOptions> _options;
        private readonly ILoggerFactory _loggerFactory;
        private readonly INameResolver _nameResolver;
        

        public SessionMessageExtensionConfigProvider(IOptions<HandlerOptions> options, ILoggerFactory loggerFactory,
            INameResolver nameResolver)
        {
            _options = options;
            _loggerFactory = loggerFactory;
            _nameResolver = nameResolver;
          
        }

        public void Initialize(ExtensionConfigContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            ILogger logger = _loggerFactory.CreateLogger(LogCategories.CreateTriggerCategory("ServiceBus"));
            var bindingProvider = new SessionTriggerAttributeBindingProvider(_options.Value, _nameResolver, logger);

            context.AddBindingRule<SessionTriggerAttribute>()
                .BindToTrigger(bindingProvider);
        }
    }
}
