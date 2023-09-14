// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Implements the PropertyRecord and its functionality
 *          for filtering and recording events and properties.
 * 
 *
 
  
 * Revision:         $Revision: 2 $
 
********************************************************************/
using System;

namespace Avalon.Test.CoreUI
{
    /// <summary>
    /// Represents an instance of a property value change in a called event handler.
    /// A PropertyRecord is stored when the EventRecorder detects that a property
    /// has changed from one event handler call to another call.
    /// </summary>
    public class PropertyRecord: Object
    {
        /// <summary>
        /// </summary>
        public PropertyRecord()
        {
            _element = "";
            _name = "";
            _value = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Element + "." + Name + " = '" + Value + "'";
        }

        /// <summary>
        /// </summary>
        public override int GetHashCode()
        {
            int hashCode = this.Element.GetHashCode() ^ this.Name.GetHashCode() ^ this.Value.GetHashCode();

            return hashCode;
        }

        /// <summary>
        /// </summary>
        public override bool Equals(object obj)
        {
            if (!(obj is PropertyRecord))
                return false;

            PropertyRecord record = (PropertyRecord)obj;

            if (!this.Element.Equals(record.Element))
                return false;

            if (!this.Name.Equals(record.Name))
                return false;

            if (!this.Value.Equals(record.Value))
                return false;

            return true;
        }

        /// <summary>
        /// </summary>
        public string Element
        {
            get
            {
                return _element;
            }
            set
            {
                _element = value;
            }
        }

        /// <summary>
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        /// <summary>
        /// </summary>
        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        private string _element;
        private string _name;
        private string _value;
    }
}

