// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 *
 
  
 * Revision:         $Revision: 2 $
 
********************************************************************/
using System;
using System.Windows;

namespace Avalon.Test.CoreUI
{
    /// <summary>
    /// Class wrapper for System.Windows.DependencyPropertyChangedEventArgs.
    /// </summary>
    public class DependencyPropertyChangedEventArgsWrapper : EventArgs
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public DependencyPropertyChangedEventArgsWrapper(DependencyPropertyChangedEventArgs dpArgs)
        {
            _dpArgs = dpArgs;
        }

        /// <summary>
        /// The property whose value changed.
        /// </summary>
        public DependencyProperty Property
        {
            get { return _dpArgs.Property; }
        }
        
        /// <summary>
        /// The value of the property before the change.
        /// </summary>
        public object OldValue
        {
            get 
            {
                return _dpArgs.OldValue; 
            }
        }

        /// <summary>
        /// The value of the property after the change.
        /// </summary>
        public object NewValue
        {
            get 
            {
                return _dpArgs.NewValue;                 
            }
        }

        DependencyPropertyChangedEventArgs _dpArgs;
    }
}

