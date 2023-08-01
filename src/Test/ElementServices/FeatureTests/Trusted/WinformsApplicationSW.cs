// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Security Wrapper for Winforms Application
 * 
 *
 
  
 * Revision:         $Revision: 2 $
 
********************************************************************/
using System;
using System.Windows.Forms;
using System.Security.Permissions;
using System.Security;


namespace Avalon.Test.CoreUI.Trusted
{
    /// <summary>
    /// We have this class here as small wrapper for Winforms Application
    /// we cannot this on the normal security wrapper because a name conflict 
    /// with Avalon class.
    /// </summary>    
    public static class WinformsApplicationSW
    {
        /// <summary>
        /// Calls into Application.Exit
        /// </summary>    
        public static void Exit()
        {
            (new SecurityPermission(SecurityPermissionFlag.UnmanagedCode)).Assert();
            System.Windows.Forms.Application.Exit();     
        }

    }
}
