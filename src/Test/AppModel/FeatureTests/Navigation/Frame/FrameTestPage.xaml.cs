// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    /// <summary>
    /// Interaction logic for FrameTestPage.xaml
    /// </summary>

    public partial class FrameTestPage : System.Windows.Controls.Page
    {
        public FrameTestPage()
        {
            InitializeComponent();
            if (String.IsNullOrEmpty(NavigationHelper.GetTestName()))
                this.WindowTitle = "Frame/IslandFrame Test";
            else
            {
                // HACK: The old navigation area was allowed to have MANY tests expect this as a window title
                // And many tests set window title to the current test log
                // Thus we must name the current test log RedirectMitigation.  SaMadan to eventually fix this.
                this.WindowTitle = "RedirectMitigation";
            }
        }


    }
}
