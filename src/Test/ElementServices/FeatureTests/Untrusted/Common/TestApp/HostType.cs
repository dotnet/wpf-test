// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Document various host types we support.
********************************************************************/

namespace Avalon.Test.CoreUI.Common
{
    /// <summary>
    /// Different HostTypes that can be used.
    /// </summary>
    public enum HostType
    {
        /// <summary>
        /// The source should be the Browser.    
        /// </summary>
        Browser,

        /// <summary>
        /// The source should be an HwndSource.
        /// </summary>
        HwndSource,

        /// <summary>
        /// The source should be a Window
        /// </summary>
        Window,

        /// <summary>
        /// The source should be a NavigationWindow (back and forward buttons)
        /// </summary>
        NavigationWindow,

        /// <summary>
        /// The source should be a NavigationWindow (back and forward buttons)
        /// </summary>
        WindowsFormSource        
    }
}
