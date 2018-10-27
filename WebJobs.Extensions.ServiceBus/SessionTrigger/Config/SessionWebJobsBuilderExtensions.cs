// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Extensions.Timers;
using Microsoft.Azure.WebJobs.Extensions.ServiceBusSession;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.Hosting
{
    /// <summary>
    /// Extension methods for sessions messages integration
    /// </summary>
    public static class SessionWebJobsBuilderExtensions
    {
        /// <summary>
        /// Adds the Service bus session trigger extension to the provided <see cref="IWebJobsBuilder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IWebJobsBuilder"/> to configure.</param>
        public static IWebJobsBuilder AddServiceBusSessionTrigger(this IWebJobsBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.AddExtension<SessionMessageExtensionConfigProvider>()
                .BindOptions<HandlerOptions>();
            

            return builder;
        }

        /// <summary>
        /// Adds the sessions extension to the provided <see cref="IWebJobsBuilder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IWebJobsBuilder"/> to configure.</param>
        /// <param name="configure">An <see cref="Action{TimersOptions}"/> to configure the provided <see cref="HandlerOptions"/>.</param>
        public static IWebJobsBuilder AddTimers(this IWebJobsBuilder builder, Action<HandlerOptions> configure)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            builder.AddServiceBusSessionTrigger();
            builder.Services.Configure(configure);

            return builder;
        }
    }
}
