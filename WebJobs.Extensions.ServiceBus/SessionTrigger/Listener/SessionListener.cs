// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Azure.WebJobs.Host.Executors;
using Microsoft.Azure.WebJobs.Host.Listeners;
using Microsoft.Extensions.Logging;

namespace Microsoft.Azure.WebJobs.Extensions.ServiceBusSession.Listeners
{
    [Singleton(Mode = SingletonMode.Listener)]
    internal sealed class SessionListener : IListener
    {
        private readonly SessionTriggerAttribute _attribute;
        private readonly HandlerOptions _options;
        private readonly ITriggeredFunctionExecutor _executor;
        private readonly ILogger _logger;
        private readonly CancellationTokenSource _cancellationTokenSource;

        private bool _disposed;
        

        public SessionListener(SessionTriggerAttribute attribute,  string handlerName, HandlerOptions options, ITriggeredFunctionExecutor executor, ILogger logger)
        {
            _attribute = attribute;
          
            _options = options;
            _executor = executor;
            _logger = logger;
            _cancellationTokenSource = new CancellationTokenSource();           
           
        }

       
       
      

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            ThrowIfDisposed();           

            // if schedule monitoring is enabled, record (or initialize)
            // the current schedule status
            bool isPastDue = false;

            // we use DateTime.Now rather than DateTime.UtcNow to allow the local machine to set the time zone. In Azure this will be
            // UTC by default, but can be configured to use any time zone if it makes scheduling easier.
            DateTime now = DateTime.Now;
            if (isPastDue)
            {
               
                await InvokeJobFunction(now, isPastDue: true);
            }
            else if (_attribute.RunOnStartup)
            {   
                await InvokeJobFunction(now, runOnStartup: true);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

          

            return Task.FromResult<bool>(true);
        }

        public void Cancel()
        {
            ThrowIfDisposed();
            _cancellationTokenSource.Cancel();
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                // Running callers might still be using the cancellation token.
                // Mark it canceled but don't dispose of the source while the callers are running.
                // Otherwise, callers would receive ObjectDisposedException when calling token.Register.
                // For now, rely on finalization to clean up _cancellationTokenSource's wait handle (if allocated).
                _cancellationTokenSource.Cancel();
 
                _disposed = true;
            }
        }

        private void OnTimer(object sender, ElapsedEventArgs e)
        {
            HandleTimerEvent().Wait();
        }

        internal async Task HandleTimerEvent()
        {
          

            await InvokeJobFunction(DateTime.Now, false);

            
        }

        /// <summary>
        /// Invokes the job function.
        /// </summary>
        /// <param name="invocationTime">The time of the invocation, likely DateTime.Now.</param>
        /// <param name="isPastDue">True if the invocation is because the invocation is due to a past due timer.</param>
        /// <param name="runOnStartup">True if the invocation is because the timer is configured to run on startup.</param>
        internal async Task InvokeJobFunction(DateTime invocationTime, bool isPastDue = false, bool runOnStartup = false)
        {
            CancellationToken token = _cancellationTokenSource.Token;
           
            SessionInfo sessionInfo = new SessionInfo();
            TriggeredFunctionData input = new TriggeredFunctionData
            {
                TriggerValue = sessionInfo
            };

            try
            {
                FunctionResult result = await _executor.TryExecuteAsync(input, token);
                if (!result.Succeeded)
                {
                    token.ThrowIfCancellationRequested();
                }
            }
            catch
            {
                // We don't want any function errors to stop the execution
                // schedule. Errors will be logged to Dashboard already.
            }
             
        }

       

      

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(null);
            }
        }
    }
}
