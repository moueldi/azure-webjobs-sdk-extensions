// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.WebJobs.Extensions.ServiceBusSession;

namespace Microsoft.Azure.WebJobs
{
    /// <summary>
    /// Provides access to timer schedule information for jobs triggered 
    /// by <see cref="SessionTriggerAttribute"/>
    /// </summary>
    public class SessionInfo
    {
        /// <summary>
        /// Constructs a new instance
        /// </summary>
        /// <param name="schedule">The timer trigger schedule.</param>
        /// <param name="status">The current schedule status.</param>
        /// <param name="isPastDue">True if the schedule is past due, false otherwise.</param>
        public SessionInfo()
        {
            //Schedule = schedule;
            //ScheduleStatus = status;
            //IsPastDue = isPastDue;
        }

    
        
    }
}
