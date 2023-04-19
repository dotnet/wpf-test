// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Test.Logging;
using Microsoft.Windows.Test.Client.AppSec.Navigation;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    public partial class panePage1CB
    {
        void OnLoaded(object source, EventArgs e)
        {
            NavigationWindow nwSource = source as NavigationWindow;
            Window wSource = source as Window;

            Log.Current.CurrentVariation.LogMessage("panePage1.xaml is loaded");

            // This is the Initialized event for the page raised by the parser
            // Make sure source is neither NavigationWindow or Window
            if (nwSource != null)
            {
                NavigationHelper.Fail("Unexpected. Loaded source is navwin");
            }
            else if (wSource != null)
            {
                NavigationHelper.Fail("Unexpected. Loaded source is window");
            }
            else
            {
                NavigationHelper.Output("Loaded source is neither window nor navigationwindow");
            }
        }
    }
}

