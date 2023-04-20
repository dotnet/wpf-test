// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;

namespace RegressionIssue130_Assembly20
{
    /// <summary>
    /// Regression test: "xaml compiler uses .NET 2.0 core and reports error".
    /// Scenario: compiling a project containing an assembly built with .Net 2.0 that employs System.dll.
    /// The bug: A build error occurs whenever you have a reference assembly that's built against .NET 2/3
    /// and references an attribute from System.dll.
    /// This test app compiles using "TestAssembly20.dll", which was built with .Net 2.0 and contains an
    /// attribute in AssemblyInfo.cs that references System.dll:
    ///     [assembly: System.Diagnostics.Switch("abc", typeof(string))]
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(0);
        }
    }
}
