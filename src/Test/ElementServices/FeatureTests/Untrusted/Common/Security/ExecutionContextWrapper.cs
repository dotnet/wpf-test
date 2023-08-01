// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;

using System.Security;
using System.Threading;
using System.Windows.Threading;
using System.Security.Permissions;


namespace Avalon.Test.Security
{
    ///<summary>
    ///</summary>
    ///<remarks>
    ///     <filename>BeginInvokeSecurityTest.cs</filename>
    ///</remarks>
    public class SecurityContextSW
    {

        private static AsyncFlowControl _suppressFlow()
        {
            PermissionSet pSet = new PermissionSet(PermissionState.Unrestricted);
            SecurityPermission secP = new SecurityPermission(SecurityPermissionFlag.AllFlags);

            pSet.AddPermission(secP);
            pSet.Assert();
            AsyncFlowControl fcontrol  = ExecutionContext.SuppressFlow();
            CodeAccessPermission.RevertAssert();


            return  fcontrol;
        }

        ///<summary>
        ///</summary>
        public static AsyncFlowControl SuppressFlow()
        {
            return  _suppressFlow();
        }


    }
}

