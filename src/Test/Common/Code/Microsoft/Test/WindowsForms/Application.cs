// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using SWF = System.Windows.Forms;

namespace Microsoft.Test.WindowsForms
{
    ///<summary>
    /// Security Wrapper for Winfoms Application
    ///</summary>
    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Assert, Name = "FullTrust")]
    public static class Application
    {
        ///<summary>
        ///</summary>
        public static void Run()
        {
           SWF.Application.Run();
        }

        ///<summary>
        ///</summary>
        public static void Exit()
        {
           SWF.Application.Exit();
        }
    }
}
