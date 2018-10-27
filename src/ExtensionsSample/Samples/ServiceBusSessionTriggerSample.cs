// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.ServiceBusSession;

namespace WebJobsSandbox
{
    public static class ServicebusSessionTriggerSample
    {
        /// <summary>
        /// Example job triggered by a crontab schedule.
        /// </summary>
        public static void MesssageSessionHandler([SessionTrigger("*/15 * * * * *")] TimerInfo timerInfo)
        {
            Console.WriteLine("Timer job fired!");
        }
    }
}
