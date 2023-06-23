//---------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All rights reserved.
//
//---------------------------------------------------------------------------


#region Using Statements

using System;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using System.Windows.Input;
using System.Collections.Generic;

using Microsoft.Test.Logging;

using Avalon.Test.ComponentModel.Utilities;
using Avalon.Test.ComponentModel.Validations;

#endregion

namespace Avalon.Test.ComponentModel.Utilities
{
    /// <summary>
    /// Helper class for event testing 
    /// To use it, first, you should create an instance and let OnEvent method registered for your event.
    /// When all are done, you can verify whether the event is raised by means of calling Verify().
    /// </summary>
    /// <typeparam name="EventArgsType">the type of EventArgs</typeparam>
    internal class EventVerifier<EventArgsType>
    {
        public void OnEvent(object source, EventArgsType e)
        {
            events.Add(new EventArgInfo<EventArgsType>(source, e));
        }

        /// <summary>
        /// Verify event has been raised.
        /// </summary>
        /// <returns>true if event has been raised</returns>
        public bool Verify()
        {
            return events.Count > 0;
        }

        /// <summary>
        /// Verify the event has been raised specific times
        /// </summary>
        /// <param name="count">how many times to be expected for event to be raised</param>
        /// <returns>true if event has been raised expected times</returns>
        public bool Verify(int count)
        {
            return events.Count == count;
        }

        private List<EventArgInfo<EventArgsType>> events = new List<EventArgInfo<EventArgsType>>();
        /// <summary>
        /// Information for each event
        /// </summary>
        public EventArgInfo<EventArgsType>[] Events
        {
            get { return events.ToArray(); }
        }

    }

    /// <summary>
    /// Store event information including source and event arguments
    /// </summary>
    /// <typeparam name="EventArgsType">type of event arguments type</typeparam>
    internal struct EventArgInfo<EventArgsType>
    {
        private object source;

        /// <summary>
        /// Source of event
        /// </summary>
        public object Source
        { get { return source; } }

        private EventArgsType args;

        /// <summary>
        /// Arguments of event
        /// </summary>
        public EventArgsType Args
        { get { return args; } }

        public EventArgInfo(object source, EventArgsType args)
        {
            this.source = source;
            this.args = args;
        }
    }
}
