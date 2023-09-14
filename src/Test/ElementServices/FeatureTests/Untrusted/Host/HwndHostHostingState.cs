// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
 * Purpose: Test Case for DispatcherTimer
 *
 
  
 * Revision:         $Revision: 1 $
 
********************************************************************/
using System;
using System.Threading;
using Avalon.Test.CoreUI;
using System.Windows.Media;
using Avalon.Test.CoreUI.Trusted;
using Microsoft.Test;
using Avalon.Test.CoreUI.Common;
using System.Windows.Threading;
using Microsoft.Test.Modeling;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Test.Logging;
using System.Diagnostics;
using Avalon.Test.CoreUI.Threading;

namespace Avalon.Test.CoreUI.Hosting
{
    /// <summary>
    /// </summary>
    [Serializable()]    
    public class HwndHostHostingState : CoreModelState
    {

        /// <summary>
        /// </summary>
        public HwndHostHostingState()
        {

        }
    
        /// <summary>
        /// </summary>
        public HwndHostHostingState(IDictionary dictionary)
        {
            string sourceType = (string)dictionary["SourceType"];
            Dictionary.Add("SourceType", sourceType);

            if (sourceType.IndexOf("NoCompiled") != 0)
            {
                // The default is not compiled                
                Dictionary.Add("CompiledVersion",true);            
            }
            else
            {
                Dictionary.Add("CompiledVersion",false);            
            }

            string[] tempArray = sourceType.Split("_".ToCharArray());

            if (tempArray.Length != 2)
            {
                throw new ArgumentException("The SourceType is not on the correct format","dictionary");
            }

            string shostType = tempArray[1];
            HostType hostType = (HostType)Enum.Parse(typeof(HostType),shostType);

            Dictionary.Add("Source",hostType);

            Dictionary.Add("HwndHostType", (string)dictionary["HostControlType"]);
        }

        
        /// <summary>
        /// </summary>
        public string HwndHostType
        {
            get
            {
                return (string)Dictionary["HwndHostType"];
            }
        }      
    }
}

