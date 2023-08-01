// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Implements the EventFilter as a store 
 *          for filtering events.
 * 
 *
 
  
 * Revision:         $Revision: 2 $
 
********************************************************************/
namespace Avalon.Test.CoreUI
{
    /// <summary>
    /// Event filter. Used to direct the EventRecorder on which
    /// events to record.
    /// </summary>
    public class EventFilter : TypeMemberFilter
    {
        /// <summary>
        /// Specifies whether or not to record handled events.
        /// </summary>
        public bool HandledEventsToo
        {
            get { return _handledEventsToo; }
            set { _handledEventsToo = value; }
        }

        /// <summary>
        /// Specifies whether or not to add and remove handlers via
        /// a static method.
        /// </summary>
        public bool UseStaticMethod
        {
            get { return _useStaticMethod; }
            set { _useStaticMethod = value; }
        }

        /// <summary>
        /// Specifies whether or not to set the RoutedEventArgs.Handled property.
        /// </summary>
        /// <remarks>
        /// Throws if the event isn't a RoutedEvent.
        /// </remarks>
        public bool MarkHandled
        {
            get { return _markHandled; }
            set { _markHandled = value; }
        }

        private bool _handledEventsToo = false;
        private bool _useStaticMethod = false;
        private bool _markHandled = false;
    }
}

