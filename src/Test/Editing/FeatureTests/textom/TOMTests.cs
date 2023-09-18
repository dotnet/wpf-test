// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Testing TOM

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.Threading;

    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Input;
    using System.Windows.Documents;
    using System.Windows.Markup;
    using Microsoft.Test;
    using Microsoft.Test.Discovery;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Loggers;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;

    #endregion Namespaces.

    /// <summary>
    /// Verify that logical tree parenting is done right.
    /// 1.Repro Regression_Bug312
    /// </summary>
    [Test(2, "TextOM", "LogicalTreeParentingTest", MethodParameters = "/TestCaseType=LogicalTreeParentingTest")]
    [TestOwner("Microsoft"), TestTactics("379"), TestBugs("312"), TestLastUpdatedOn("Jan 25, 2007")]
    public class LogicalTreeParentingTest : CustomTestCase
    {
        #region PrivateMembers
        StackPanel _testFlowPanel;
        FlowDocumentScrollViewer _testTextPanel;
        TextBlock _testText;
        Border _testBorder;
        bool _testPassed = true;

        DependencyObject _textPanelChild,_textChild,_borderChild;
        InlineUIContainer _textPanelChildContainer,_textChildContainer;
        #endregion PrivateMembers

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            _testFlowPanel = new StackPanel();

            _testTextPanel = new FlowDocumentScrollViewer();
            _testTextPanel.Document = new FlowDocument();
            _testTextPanel.Document.Blocks.Clear();
            _textPanelChild = new Decorator();
            _textPanelChildContainer = new InlineUIContainer((UIElement)_textPanelChild);
            _testTextPanel.Document.Blocks.Add(new Paragraph(_textPanelChildContainer));            
            _testFlowPanel.Children.Add(_testTextPanel);

            _testText = new TextBlock();
            _testText.Inlines.Clear();
            _textChild = new Button();
            _textChildContainer = new InlineUIContainer((UIElement)_textChild);
            _testText.Inlines.Add(_textChildContainer);            
            _testFlowPanel.Children.Add(_testText);

            _testBorder = new Border();
            _borderChild = new ListBox();
            _testBorder.Child = (UIElement)_borderChild;
            _testFlowPanel.Children.Add(_testBorder);

            MainWindow.Content = _testFlowPanel;
            QueueDelegate(RemoveContent);
        }

        private void RemoveContent()
        {
            new TextRange(_testTextPanel.Document.ContentStart, _testTextPanel.Document.ContentEnd).Text = "";            
            if (_textPanelChildContainer.Parent != null)
            {                
                Log("textPanelChild.Parent != null");
                _testPassed = false;
            }

            _testText.Text = "";
            if (_textChildContainer.Parent != null)
            {
                Log("textChild.Parent != null");
                _testPassed = false;
            }

            _testBorder.Child = null;
            if (LogicalTreeHelper.GetParent(_borderChild) != null)
            {
                Log("borderChild.Parent != null");
                _testPassed = false;
            }

            if (_testPassed)
                Logger.Current.ReportSuccess();
            else
                Logger.Current.ReportResult(false, "Test Failed: Back pointers in logical tree are not invalidated", false);
        }
    }
}
