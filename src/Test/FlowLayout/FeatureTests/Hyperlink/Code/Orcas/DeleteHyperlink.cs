// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description : Regression test for Windows OS Regression_Bug18 [Hyperlink crashes when removed from a Click handler]
// Scenario    : Give Hyperlink focus, click, then remove the hyperlink from the document in the event handler
// Owner       : Microsoft
///////////////////////////////////////////////////////////////////////////////////////////////////////////////// 

using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using MTI = Microsoft.Test.Input;

namespace Microsoft.Test.FlowLayout
{
    /// <summary>    
    /// Hyperlink Regression test.    
    /// </summary>
    [Test(2, "Hyperlink", "DeleteHyperlink", MethodName = "Run")]
    public class DeleteHyperlink : AvalonTest
    {
        #region Private Data

        private Window _window = null;
        private Hyperlink _hyperlink = null;
        private TextBlock _textBlock = null;
        private int _tabCount = 0;

        #endregion

        #region Constructor

        public DeleteHyperlink()
            : base()
        {
            InitializeSteps += new TestStep(Initialize);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(RunTest);
        }

        #endregion

        #region Private Members

        /// <summary>
        /// Initialize: Create test window and loads test content.
        /// </summary>
        /// <returns>TestResult.Pass;</returns>
        private TestResult Initialize()
        {
            Status("Initialize");

            _hyperlink = new Hyperlink(new Span(new Run("This is a Hyperlink!")));
            _hyperlink.Click += new RoutedEventHandler(hyperLink_Click);
            _hyperlink.Focusable = true;

            _textBlock = new TextBlock();
            _textBlock.FontSize = 20;
            _textBlock.Text += "This is some sample text.";
            _textBlock.Inlines.Add(_hyperlink);
            
            StackPanel stackPanel = new StackPanel(); 
            stackPanel.Children.Add(_textBlock);

            _window = new Window();
            _window.Content = stackPanel;
            _window.Top = 0;
            _window.Left = 0;
            _window.Width = 800;
            _window.Height = 600;
            _window.Show();

            WaitForPriority(DispatcherPriority.ApplicationIdle);

            //Ensure test window has focus.
            _window.Focus();
            MTI.Input.MoveToAndClick(new Point(100, 15));
            
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            return TestResult.Pass;
        }

        private TestResult CleanUp()
        {
            _window.Close();
            return TestResult.Pass;
        }

        /// <summary>
        /// RunTests: Runs a set of tests based on the input variation.
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult RunTest()
        {
            TabToLink();
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            PressEnter();
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            return TestResult.Pass;
        }

        private void TabToLink()
        {
            int maxTabs = 10;
         
            while ((!_hyperlink.IsFocused) && (_tabCount <= maxTabs))
            {
                PressTab();
                WaitForPriority(DispatcherPriority.ApplicationIdle);
                _tabCount++;
            }
        }
        
        private void PressTab()
        {
            TestLog.Current.LogStatus(string.Format("Tab Count {0}", _tabCount.ToString()));
            Key key = Key.Tab;
            MTI.Input.SendKeyboardInput(key, true);
            MTI.Input.SendKeyboardInput(key, false);
        }
        
        private void PressEnter()
        {
            Key key = Key.Enter;
            MTI.Input.SendKeyboardInput(key, true);
            MTI.Input.SendKeyboardInput(key, false);
        }

        private void hyperLink_Click(object sender, RoutedEventArgs e)
        {
            _textBlock.Inlines.Remove(_hyperlink);
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            Status("Removed Hyperlink.");
        }
        
        #endregion
    }
}
