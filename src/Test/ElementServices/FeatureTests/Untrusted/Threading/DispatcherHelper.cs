// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Threading;
using Microsoft.Test.Discovery;


namespace Avalon.Test.CoreUI.Threading
{
    /// <summary>
    /// </summary>
    public delegate void ZeroArgDelegate();

    /// <summary>
    /// </summary>
    public delegate void OneArgDelegate(object o);

    /// <summary>
    /// </summary>
    public delegate void TwoArgDelegate(object o1, object o2);    

    /// <summary>
    /// </summary>
    public static class DispatcherPriorityHelper
    {
        /// <summary>
        /// </summary>
        public static DispatcherPriority[] GetValidDispatcherPriorities()
        {
            ArrayList priorityArrayTemp = new ArrayList(Enum.GetValues(typeof(DispatcherPriority)));

            List<DispatcherPriority> priorityList = new List<DispatcherPriority>(priorityArrayTemp.Count - 1);

            for (int i = 0; i < priorityArrayTemp.Count; i++)
            {
                if ((DispatcherPriority)priorityArrayTemp[i] != DispatcherPriority.Invalid)
                {
                    priorityList.Add((DispatcherPriority)priorityArrayTemp[i]);
                }
            }
            
            return priorityList.ToArray();
        }
    }    
}
