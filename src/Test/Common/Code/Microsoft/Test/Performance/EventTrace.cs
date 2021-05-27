// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// File: EventTrace.cs

// A managed wrapper for Event Tracing for Windows

// Based on Trace.cs from windows\wcp\shared\ms\utility

using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Threading;
using System.Diagnostics;
using System.Security;

namespace Microsoft.Test.Performance
{
    /// <summary>
    /// This event tracing class provides a windows client interface to the
    /// TraceProvider interface.  Events are logged through the EventProvider
    /// object, using the following format:
    ///  EventTrace.EventProvider.TraceEvent(TRACEGUID, EVENT_TYPE, OPERATION_TYPE, [DATA,]*);
    ///
    ///  TRACEGUID   - Differentiates the format of the record placed in the ETL log file.
    ///  EVENT_TYPE  - Event types are defined in TraceProvider.cs
    ///     EventType.Info        - Information or point event.
    ///     EventType.StartEvent  - Start of operation.
    ///     EventType.EndEvent    - End of operation.
    ///     EventType.Checkpoint  - Checkpoint within bounded operation.  This might be used
    ///                             to indicate transfer of control from one resource to another.
    ///  OPERATION_TYPE - Avalon operation.
    ///  DATA           - Optional additional data logged with the TRACEGUID.
    ///
    ///  Example:
    ///
    ///  if (EventTrace.IsEnabled(EventTrace.Flags.Performance, EventTrace.Level.Normal))
    ///  {
    ///      EventTrace.EventProvider.TraceEvent(EventTrace.QUEUEIDLE_GUID, EventType.Info);
    ///  }
    /// </summary>
    /// 

    sealed public class EventTrace
    {
        #region Event GUIDs

        /// <summary>
        /// WpfPerfTest_Count (String Description, Int32)
        /// {dff4d956-9410-4cc5-aed2-f7820320ee07}
        /// </summary>
        static readonly public Guid COUNT_GUID = new Guid(0xdff4d956, 0x9410, 0x4cc5, 0xae, 0xd2, 0xf7, 0x82, 0x3, 0x20, 0xee, 0x7);

        /// <summary>
        /// WpfPerfTest_Info (String Message)
        /// {8ba42b19-e8e1-4476-a1bc-8dabadf0077d}
        /// </summary>
        static readonly public Guid INFO_GUID = new Guid(0x8ba42b19, 0xe8e1, 0x4476, 0xa1, 0xbc, 0x8d, 0xab, 0xad, 0xf0, 0x7, 0x7d);

        /// <summary>
        /// WpfPerfTest_Operation+ (String Description)
        /// WpfPerfTest_Operation- (String Description)
        /// {c90cf349-bcba-4e0d-afda-240fd4b1cde4}
        /// </summary>
        static readonly public Guid OPERATION_GUID = new Guid(0xc90cf349, 0xbcba, 0x4e0d, 0xaf, 0xda, 0x24, 0xf, 0xd4, 0xb1, 0xcd, 0xe4);

        /// <summary>
        /// WpfPerfTest_QueueIdle (String Operation)
        /// {891eedb2-45d2-4d87-89d7-807773790042}
        /// </summary>
        static readonly public Guid QUEUEIDLE_GUID = new Guid(0x891eedb2, 0x45d2, 0x4d87, 0x89, 0xd7, 0x80, 0x77, 0x73, 0x79, 0x0, 0x42);

        #endregion

        /// <summary>
        /// The users specifies the type of information they want logged by
        /// passing in one of these flags when enabling tracing.  This may
        /// change as we get more guidelines from the ETW team.
        /// </summary>
        [Flags]
        public enum Flags:int
        {
            
            Debugging      = 0x00000001,
            Performance    = 0x00000002, /// Performance analysis
            Stress         = 0x00000004,
            Security       = 0x00000008,
            UiAutomation   = 0x00000010,
            Response       = 0x00000020, /// Response time tracing (benchmarks)
            Trace          = 0x00000040,          
            PerToolSupport = 0x00000080, 
            All            = 0x7FFFFFFF
        }

        /// <summary>
        /// Calls to the TraceEvent API specify a level of detail, which the user
        /// selects when enabling tracing.  Setting the detail level to a higher
        /// level increases the number of events logged.  A higher level of
        /// tracing includes the lower level tracing events.  This is used with
        /// the flags to enable the right set of events.
        /// </summary>
        public enum Level: byte
        {
            Fatal         = 1, // Events signalling fatal errors
            Error         = 2, // Events that should never happen
            Warning       = 3, // Infrequent events that indicate a potential problem
            Normal        = 4, // Normal level of tracing
            Verbose       = 5, // Additional information            
        }

        /// <summary>
        /// Callers use this to check if they should be logging.
        /// </summary>
        static public bool IsEnabled(Flags flag, Level level)
        {
            if ((EventProvider != null) &&
                ((uint)level <= EventProvider.Level) &&
                (EventProvider.IsEnabled) &&
                (Convert.ToBoolean((uint)flag & EventProvider.Flags)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
        static public bool IsEnabled(Flags flags)
        {
            return IsEnabled(flags, Level.Normal);
        }

        /// <summary>
        ///  This class has only static methods, so use a private default constructor to
        ///  prevent instances from being created
        /// </summary>
        private EventTrace()
        {
        }

        /// <summary>
        /// Internal operations associated with initializing the event provider and
        /// monitoring the Dispatcher and input components.
        /// </summary>
        ///<SecurityNote>
        /// Critical:  This calls critical code in TraceProvider
        /// TreatAsSafe:  it generates the GUID that is passed into the TraceProvider
        /// {60a64207-9ac9-49af-bf8d-93e7bc0ae374}
        ///</SecurityNote>
        [SecurityCritical, SecurityTreatAsSafe]
        static EventTrace()
        {
            EventProvider = null;
            EventProvider = new TraceProvider(new Guid(0x60a64207,
                                                       0x9ac9,
                                                       0x49af,
                                                       0xbf,0x8d,
                                                       0x93,0xe7,0xbc,0x0a,0xe3,0x74));
        }

        static public void NormalTraceEvent(Guid guid, byte eventType)
        {
            if (EventTrace.IsEnabled(EventTrace.Flags.Performance, EventTrace.Level.Normal))
            {
                EventTrace.EventProvider.TraceEvent(guid, eventType);
            }
        }

        static public void NormalTraceEvent(Guid guid, byte eventType, string description)
        {
            if (EventTrace.IsEnabled(EventTrace.Flags.Performance, EventTrace.Level.Normal))
            {
                EventTrace.EventProvider.TraceEvent(guid, eventType, description);
            }
        }

        /// <summary>
        /// This is the provider class that actually provides the event tracing
        /// functionality.  
        /// </summary>
        static readonly public TraceProvider EventProvider;
    }
}
        
