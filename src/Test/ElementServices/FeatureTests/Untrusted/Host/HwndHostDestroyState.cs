// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
 * Purpose: Test Case for DispatcherTimer
 *
 
  
 * Revision:         $Revision: 1 $
 
********************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Threading;
using Avalon.Test.CoreUI.Trusted;
//using Avalon.Test.CoreUI.Trusted.Controls;

using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.Modeling;
using Microsoft.Test.Win32;

namespace Avalon.Test.CoreUI.Hosting
{

    /// <summary>
    /// </summary>
    [Serializable()]    
    public class HwndHostDestroyState : HwndHostHostingState
    {

        /// <summary>
        /// </summary>
        public HwndHostDestroyState()
        {

        }
    
        /// <summary>
        /// </summary>
        public HwndHostDestroyState(IDictionary dictionary) : base(dictionary)
        {
            string destroyType = (string)dictionary["TypeOfDestroyCall"];

            string hwndHostTreeState = (string)dictionary["HwndHostTreeState"];

            Dictionary.Add("TypeOfDestroyCall",destroyType);   

            Dictionary.Add("HwndHostTreeState",hwndHostTreeState);
        }

        /// <summary>
        /// </summary>
        public string TypeOfDestroyCall
        {
            get
            {
                return (string)Dictionary["TypeOfDestroyCall"];
            }
        }

        /// <summary>
        /// </summary>
        public string HwndHostTreeState
        {
            get
            {
                return (string)Dictionary["HwndHostTreeState"];
            }
        }

    }
}

