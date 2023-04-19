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
using System.Windows.Shapes;
using Microsoft.Test.Logging;                   // TestLog, TestStage
using Microsoft.Test.Loaders;

namespace testapp
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>

    public partial class TransitionElement_NavWindow : System.Windows.Navigation.NavigationWindow
    {
        private static TestLog s_log = TestLog.Current;

        public void verifyNoCrash(object sender, EventArgs args)
        {
            //If there was not a crash, then the test passes
            s_log.LogEvidence("SUCCESS!!! NavigationWindow was able to host TransitionElement");
            s_log.Result = TestResult.Pass;
            s_log.Close();
            Application.Current.Shutdown();
        }

        public TransitionElement_NavWindow()
        {
            InitializeComponent();
        }
    }
}
