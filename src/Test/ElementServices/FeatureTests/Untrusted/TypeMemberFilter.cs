// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Implements the EventRecorder and its supporting structures
 *          for filtering and recording events and properties.
 * 
 *
 
  
 * Revision:         $Revision: 2 $
 
********************************************************************/
namespace Avalon.Test.CoreUI
{
    /// <summary>
    /// Base class for EventFilter and PropertyFilter.  The filters direct 
    /// the EventRecorder on which properties and events to record.
    /// </summary>
    public abstract class TypeMemberFilter
    {
        /// <summary>
        /// </summary>
        public string ElementName
        {
            get { return _elementName; }
            set { _elementName = value; }
        }
        /// <summary>
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private string _elementName = null;
        private string _name = null;
    }
}

