// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*
        Test case to prevent the regression of Regression_Bug35
        
        1) Open a window with a textbox and a rich text box
        2) Both text boxes have the same content - a list followed by a uielement (button)
        3) Select from the top right corner of the textbox/rich textbox to the bottom of the list and onto the left half of the uielement
        
        The text box and the rich text box should not throw exceptions after mouse selection is completed
    */
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Threading;
using Microsoft.Test.Layout;
using Microsoft.Test.Input;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.FlowLayout
{   
    [Test(3, "Bottomless", "Regression_Bug35")]
    class TestRegression_Bug35: AvalonTest
    {       
        private RichTextBox _rtb;
        private Window _window;

        public TestRegression_Bug35()
            : base()
        {            
            InitializeSteps += new TestStep(Initialize);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(RunTests);
        }
            
        /// <summary>
        /// Initialize: setup tests
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult Initialize()
        {            
            Status("Initialize ....");

            StackPanel stackpanel = new StackPanel();
            _window = new Window();

            _rtb = new RichTextBox();
            _rtb.Width = 100;
            CreateContent(_rtb.Document);
            stackpanel.Children.Add(_rtb);
            _window.Content = stackpanel;
            _window.Show();
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            return TestResult.Pass;
        }

        private TestResult CleanUp()
        {
            _window.Close();
            return TestResult.Pass;
        }

        /// <summary>
        /// RunTests: Runs test.
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult RunTests()
        {           
            try
            {                
                Select(_rtb);
                CommonFunctionality.FlushDispatcher();
                CommonFunctionality.FlushDispatcher();
                CommonFunctionality.FlushDispatcher();
                CommonFunctionality.FlushDispatcher();
                return TestResult.Pass;
            }
            catch (System.Exception e)
            {
                Status("Regression_Bug3 may have regressed" + e.Message);
                return TestResult.Fail;               
            }                       
        }       
        
        // Select from center,top of textbox/rtextbox to left,top of button to right,bottom of button
        private void Select(RichTextBox fe)
        {
            TextPointer endOfFirstLine = null;
            TextPointer startOfLastLine = null;
            Rect r1;
            Rect r2;

            if (fe.Document.ContentStart != null)
            {
                // initialize endOfFirstLine
                // we move the start pointer to the start of next line
                // and then move one character backward

                TextPointer start = fe.Document.ContentStart;
                //We have to change the Logical direction of start to Forward
                start = start.GetNextInsertionPosition(LogicalDirection.Forward);
                //We have to go to a position where the user can insert text in order to call GetLineStartPosition
                start = start.GetPositionAtOffset(0, LogicalDirection.Forward);
                //get the position where the first line ends/the second line begins
                endOfFirstLine = start.GetLineStartPosition(1);

                // move startOfLastLine to the start of the last line
                // here we move to the max of GetLineStartPosition
                int dummy = 0;
                startOfLastLine = start.GetLineStartPosition(Int32.MaxValue, out dummy);

                r1 = endOfFirstLine.GetCharacterRect(LogicalDirection.Backward);
                r2 = startOfLastLine.GetCharacterRect(LogicalDirection.Forward);

                UserInput.MouseLeftClickCenter(fe);
                CommonFunctionality.FlushDispatcher();

                int margin = 4;
                UserInput.MouseMove(fe, (int)r1.Left, (int)r1.Top + margin);
                UserInput.MouseLeftDown(fe, (int)r2.Left + margin, (int)r2.Top - margin);
                UserInput.MouseMove(fe, (int)r2.Left + margin, (int)r2.Bottom - margin);
                UserInput.MouseLeftUp(fe, (int)r1.Left, (int)r2.Bottom - margin);
            }
            else
            {
                throw new InvalidOperationException("Cannot select on controls other than TextBox or RichTextBox");
            }
        }

        private void InsertListItem(List list, string text)
        {
            list.ListItems.Add(new ListItem(new Paragraph(new Run(text))));
        }

        private void CreateContent(FlowDocument fd)
        {
            List list = new List();
            InsertListItem(list, "first");
            InsertListItem(list, "second");
            InsertListItem(list, "third");
            InsertListItem(list, "fourth");
            fd.Blocks.Add(list);

            Button button = new Button();
            button.Content = "Click me";
            fd.Blocks.Add(new Paragraph(new InlineUIContainer(button)));
        }
    }
}
