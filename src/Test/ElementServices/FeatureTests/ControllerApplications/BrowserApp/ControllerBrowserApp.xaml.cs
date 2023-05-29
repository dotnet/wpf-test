// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using Avalon.Test.CoreUI.Common;
using Microsoft.Test.TestTypes;

namespace Avalon.Test.CoreUI.Common
{
    public partial class ControllerBrowserApp : Application
    {
        void AppStartup(object sender, StartupEventArgs e)
        {
            ApplicationController proxy = new ApplicationController();
            proxy.RunVariationLoop();
        }
    }
}
