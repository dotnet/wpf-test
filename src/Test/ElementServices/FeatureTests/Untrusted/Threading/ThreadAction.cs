// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************************
*    Source Control Information
*    
 
  
*    Revision:         $Revision: 1 $
 
*
******************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Test.Modeling;
using System.Windows.Threading;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.Win32;
using System.Threading;
using Microsoft.Test.Discovery;

namespace Avalon.Test.CoreUI.Threading
{
    /// <summary>
    /// </summary>
    public class ThreadDispatcherAction
    {
        /// <summary>
        /// </summary>        
        public ThreadDispatcherAction(DispatcherPriority priority, Delegate callback, object[] parameters)
        {
            _callback = callback;
            _params = parameters;
            _priority = priority;
        }   
        
        /// <summary>
        /// </summary>        
        public Delegate Callback
        {   
            get
            {
                return _callback;
            }
        }

        /// <summary>
        /// </summary>   
        public DispatcherPriority Priority
        {
            get
            {
                return _priority;
            }
            set
            {
                _priority = value;
            }
        }

        /// <summary>
        /// </summary>        
        public object[] Params
        {
            get
            {
                return _params;
            }
        }

        /// <summary>
        /// </summary>        
        public DispatcherWrapper DispatcherW
        {
            get
            {
                return _dispatcherW;
            }
        }

        /// <summary>
        /// </summary>        
        public void SetDispatcherW(DispatcherWrapper dispatcherWrapper)
        {
            _dispatcherW = dispatcherWrapper;
            dispatcherWrapper.AddPriorityCount(Priority);
        }

        DispatcherWrapper _dispatcherW = null;
        DispatcherPriority _priority;
        Delegate _callback;
        object[] _params = null;
    }
}


