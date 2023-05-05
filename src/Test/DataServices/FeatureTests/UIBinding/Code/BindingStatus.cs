// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading;
using System.ComponentModel;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Markup;
using Microsoft.Test.DataServices;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
	/// <summary>
	/// <description>
	/// Test BindingStatus states.
	/// </description>
	/// </summary>
    [Test(2, "Binding", "BindingStatusTest")]
	public class BindingStatusTest : WindowTest
	{
        TextBox _textbox;
        CLRBook _pc;
        DockPanel _dp;

        public BindingStatusTest()
	{
	    InitializeSteps +=new TestStep(datasetTest_InitializeSteps);

            RunSteps += new TestStep(UnattachedStatus);
            RunSteps += new TestStep(AddDataContext);
            RunSteps += new TestStep(InactiveStatus);
            RunSteps += new TestStep(UpdateErrorStatus);
            RunSteps += new TestStep(PathErrorStatus);
            RunSteps += new TestStep(AsyncRequestPendingStatus);
            RunSteps += new TestStep(DataTransferErrorStatus);
            RunSteps += new TestStep(PathErrorStatus);
            RunSteps += new TestStep(DetachedStatus);
        }

        TestResult DataTransferErrorStatus()
        {
            Status("Testing BindingStatus.UpdateTargetError");

            TextBox t = _dp.Children[3] as TextBox;
            BindingExpression b = t.GetBindingExpression(TextBox.WidthProperty);
            while (b.Status == BindingStatus.Unattached)
                 { WaitFor(100); }

            // Negetive number is an invalid value for width.
            ((CLRBook)b.DataItem).Price = -2d;
            if (b.Status != BindingStatus.UpdateTargetError)
            {
                Status("Status is incorrect - Expected:DataTransferError Actual:" + b.Status.ToString());
                return TestResult.Fail;
            }
            return TestResult.Pass;
        }
        TestResult AsyncRequestPendingStatus()
        {
            Status("Testing BindingStatus.AsyncRequestPending");
            TextBlock t = _dp.Children[1] as TextBlock;
            BindingExpression b = t.GetBindingExpression(TextBlock.TextProperty);
            _pc.Author = "Bent";


            if (b.Status != BindingStatus.AsyncRequestPending)
            {
                Status("Status is incorrect - Expected:AsyncRequestPending Actual:" + b.Status.ToString());
                return TestResult.Fail;
            }


            return TestResult.Pass;

        }
        TestResult PathErrorStatus()
        {
            Status("Testing BindingStatus.PathError");
            TextBlock t = _dp.Children[2] as TextBlock;
            BindingExpression b = t.GetBindingExpression(TextBlock.TextProperty);

            if (b.Status != BindingStatus.PathError)
            {
                Status("Status is incorrect - Expected:PathError Actual:" + b.Status.ToString());
                return TestResult.Fail;
            }
            return TestResult.Pass;
        }
        TestResult UpdateErrorStatus()
        {
            TextBox t = _dp.Children[3] as TextBox;
            BindingExpression b = t.GetBindingExpression(TextBox.TextProperty);
            while (b.Status == BindingStatus.Unattached)
            { WaitFor(100); }
            //Source takes a bool
            _textbox.Text = "cause error";
            if (b.Status != BindingStatus.UpdateSourceError)
            {
                Status("Status is incorrect - Expected:UpdateError Actual:" + b.Status.ToString());
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }
        TestResult UnattachedStatus()
        {
            TextBlock t = _dp.Children[0] as TextBlock;
            BindingExpression b = t.GetBindingExpression(TextBlock.TextProperty);
            if (b.Status != BindingStatus.Unattached)
            {
                Status("Status is incorrect - Expected:Unattached Actual:" + b.Status.ToString());
                return TestResult.Fail;
            }


            return TestResult.Pass;
        }
        TestResult InactiveStatus()
        {
            TextBlock t = _dp.Children[4] as TextBlock;
            BindingExpression b = t.GetBindingExpression(TextBlock.TextProperty);
            PriorityBindingExpression pb = BindingOperations.GetPriorityBindingExpression(t, TextBlock.TextProperty);
            while (pb.BindingExpressions[0].Status != BindingStatus.Active)
                { WaitFor(100); }
            if (pb.BindingExpressions[1].Status != BindingStatus.Inactive)
            {
                Status("Status is incorrect - Expected:UpdateError Actual:" + b.Status.ToString());
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }
        TestResult DetachedStatus()
        {
            TextBlock t = _dp.Children[0] as TextBlock;
            BindingExpression b = t.GetBindingExpression(TextBlock.TextProperty);
//            Binding.ClearAllBindings(t);
            BindingOperations.ClearBinding(t, TextBlock.TextProperty);
            //Detached
            if (b.Status != BindingStatus.Detached)
            {
                Status("Status is incorrect - Expected:Detached Actual:" + b.Status.ToString());
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }
        TestResult AddDataContext()
        {
            _pc = new CLRBook("title", "author", 0, 23, BookType.Fiction);
            _dp.DataContext = _pc;
            return TestResult.Pass;
        }
        TestResult datasetTest_InitializeSteps()
		{
            _dp = new DockPanel();

            //Used with UnattachedStatus & InactiveStatus
            Binding b1 = new Binding("Title");
            b1.UpdateSourceTrigger = UpdateSourceTrigger.LostFocus;
            TextBlock textbox1 = new TextBlock();
            textbox1.SetBinding(TextBlock.TextProperty, b1);
            textbox1.SetValue(DockPanel.DockProperty, Dock.Top);
            _dp.Children.Add(textbox1);

            // Used with AsyncRequestPendingStatus
            Binding b2 = new Binding("Author");
            b2.IsAsync = true;
            TextBlock textbox2 = new TextBlock();
            textbox2.SetBinding(TextBlock.TextProperty, b2);
            textbox2.SetValue(DockPanel.DockProperty, Dock.Top);
            _dp.Children.Add(textbox2);

            //Used with PathErrorStatus
            Binding b3 = new Binding("badpath");
            TextBlock textbox3 = new TextBlock();
            textbox3.SetBinding(TextBlock.TextProperty, b3);
            textbox3.SetValue(DockPanel.DockProperty, Dock.Top);
            _dp.Children.Add(textbox3);

            //used with UpdateErrorStatus
            Binding b4 = new Binding("Year");
            b4.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            _textbox = new TextBox();
            _textbox.SetBinding(TextBox.TextProperty, b4);
            _textbox.SetValue(DockPanel.DockProperty, Dock.Top);

            //Used with DataTransferError
            Binding d4 = new Binding("Price");
            _textbox.SetBinding(TextBox.WidthProperty, d4);
            _textbox.SetValue(TextBox.HeightProperty, 40d);
            _dp.Children.Add(_textbox);

            System.Windows.Data.PriorityBinding prioritybinding1 = new System.Windows.Data.PriorityBinding();
            prioritybinding1.Bindings.Add(b4);
            prioritybinding1.Bindings.Add(b2);
            TextBlock textbox5 = new TextBlock();
            textbox5.SetBinding(TextBlock.TextProperty, prioritybinding1);
            textbox5.SetValue(DockPanel.DockProperty, Dock.Top);
            _dp.Children.Add(textbox5);

            Window.Content = _dp;

            return TestResult.Pass;
	}


    }


}
