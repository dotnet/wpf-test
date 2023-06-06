// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using Microsoft.Test.Logging;
using Microsoft.Test.Layout;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.FlowLayout
{
    /// <summary>   
    /// Testing Textblock null text content.   
    /// </summary>
    [Test(1, "TextBlock", "TestNullTextBlockContent")]
    class TestNullTextBlockContent : AvalonTest
    {
        private Window _w;

        #region Constructor
       
        public TestNullTextBlockContent()
            : base()
        {
            InitializeSteps += new TestStep(Initialize);
            CleanUpSteps += new TestStep(CleanUp);
        }

        #endregion

        #region Test Steps
      
        /// <summary>
        /// Initialize: Setup test
        /// </summary>
        /// <returns>TestResult.Pass;</returns>
        private TestResult Initialize()
        {
            Status("Initialize ....");

            _w = new Window();
            DockPanel d = new DockPanel();
            System.Windows.Controls.TextBlock t = new System.Windows.Controls.TextBlock();

            t.Text = null;
            d.Children.Add(t);
            _w.Content = d;

            _w.Show();
            CommonFunctionality.FlushDispatcher();
            LogComment("TextBlock element did not throw exception during layout");
            return TestResult.Pass;
        }

        private TestResult CleanUp()
        {
            _w.Close();
            return TestResult.Pass;
        }

        #endregion
    }
}
