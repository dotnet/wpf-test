// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Microsoft.Test;
using Avalon.Test.CoreUI;
using System.Windows;
using System.Threading;
using System.Windows.Threading;

using System.Windows.Media;
using System.Collections;

namespace Avalon.Test.CoreUI.Events
{
    /// <summary>
    ///     CustomContentElement class is a subclass of FrameworkContentElement
    /// </summary>
    /// <ExternalAPI/>
    public class CustomContentElement : FrameworkContentElement
    {
        #region Construction
        
        /// <summary>
        ///     Constructor for  CustomContentElement
        /// </summary>
        public CustomContentElement() : base()
        {
        }
        
        #endregion Construction
                
        #region External API
        
        /// <summary>
        ///     Appends model child
        /// </summary>
        public void AppendModelChild(object modelChild)
        {
            AddLogicalChild(modelChild);
        }

        #endregion External API

	}
}
