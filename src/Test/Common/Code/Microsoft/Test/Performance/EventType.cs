// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// File: EventType.cs

// Defines some common event trace type values

using System;

namespace Microsoft.Test.Performance
{
    /// <summary>
    /// These are the standard event types. Higher byte values can be used to define custom event 
    /// types within a given event class GUID.
    /// </summary>
    public static class EventType
    {
        public const byte Info = 0x00;        // Information or Point Event
        public const byte StartEvent = 0x01;  // Start of an activity
        public const byte EndEvent = 0x02;    // End of an activity
        public const byte Checkpoint = 0x08;  // Checkpoint Event
    }
}
