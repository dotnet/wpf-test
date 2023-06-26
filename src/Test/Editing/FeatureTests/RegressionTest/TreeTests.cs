// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Regression test cases targetting the tree functionality.

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.Xml;
    using System.Collections;
    using System.Threading; using System.Windows.Threading;
    
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Input;

    using System.Windows.Documents;
    using Microsoft.Test.Discovery;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Loggers;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;

    #endregion Namespaces.

    /// <summary>
    /// Verifies that elements can be found in the logical tree
    /// of a window.
    /// </summary>
    [Test(0, "TextBox", "WindowReproRegression_Bug901", MethodParams = "/TestCaseType=WindowReproRegression_Bug901")]
    [TestOwner("Microsoft"), TestTitle("WindowReproRegression_Bug901"), TestTactics("468"), TestLastUpdatedOn("Jan 25,2007")]
    public class WindowReproRegression_Bug901: CustomTestCase
    {
        #region Main flow.

        /// <summary>Creates a new test case instance.</summary>
        public WindowReproRegression_Bug901()
        {
            StartupPage = "TextBox.xaml";
        }

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            Log("Looking for element TestControl1...");
            UIElement element = (UIElement) LogicalTreeHelper.FindLogicalNode(
                MainWindow, "TestControl1");
            Verifier.Verify(element != null, "Element found", true);

            Logger.Current.ReportSuccess();
        }

        #endregion Main flow.
    }
}
