// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Security.Permissions;
using System.Security;
using Microsoft.Test.Logging;

namespace Avalon.Test.Annotations
{
    public class SecurityHelper
    {
        public static bool HasClipboadPermission
        {
            get
            {
                if (_clipboardPermission == null) 
                    _clipboardPermission = new UIPermission(UIPermissionClipboard.AllClipboard);

                try
                {
                    _clipboardPermission.Demand();
                }
                catch (SecurityException e)
                {
                    GlobalLog.LogEvidence(e);
                    return false;
                }
                return true;
            }
        }
        static UIPermission _clipboardPermission;
    }
}
