// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Navigation;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    // StartMethodNavFwdPF
    public partial class NavigationTests : Application
    {
        void StartMethodNavFwdPF_Startup(object sender, StartupEventArgs e)
        {
            LaunchPageFunctions.Run(new string[] { "/test:startmethodhistorynavfwd" });
        }
    }
}
