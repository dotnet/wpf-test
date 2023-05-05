// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using Microsoft.Test;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Windows.Navigation;
using System.Windows.Controls;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
	/// <summary>
	/// <description>
	/// SetValue should overwrite Binding except when binding type is 2 way.
	/// </description>
    /// <relatedBugs>

    /// </relatedBugs>
	/// </summary>

    [Test(1, "Binding", "SetValueTest")]

    public class SetValueTest : XamlTest 
    {
        TextBlock _textblock1;
        TextBlock _textblock2;
        TextBox _textbox;
        public SetValueTest() : base(@"varioustest.xaml")
        {
            InitializeSteps += new TestStep(Init);                
             RunSteps += new TestStep(Step1);
             RunSteps += new TestStep(Step2);
             RunSteps += new TestStep(Step3);

        }

        TestResult Init()
        {
            WaitForPriority(DispatcherPriority.SystemIdle);
            _textblock1 = LogicalTreeHelper.FindLogicalNode(RootElement, "txt1") as TextBlock;
            if (_textblock1 == null)
            {
                LogComment("TextBlock1 was null!");
                return TestResult.Fail;
            }

            _textblock2 = LogicalTreeHelper.FindLogicalNode(RootElement, "txt2") as TextBlock;
            if (_textblock2 == null)
            {
                LogComment("TextBlock2 was null!");
                return TestResult.Fail;
            }
            _textbox = LogicalTreeHelper.FindLogicalNode(RootElement, "txtbox") as TextBox;
            if (_textbox == null)
            {
                LogComment("TextBox was null!");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        TestResult Step1()
        {
            // SetValue on TwoWay binding just updates the value in the binding
            BindingExpression be = _textbox.GetBindingExpression(TextBox.TextProperty);
            _textbox.SetValue(TextBox.TextProperty, "32");
            be.UpdateSource();
            WaitForPriority(DispatcherPriority.Render);
            //TextBlock.Test is bound the same value.
            if (_textblock2.Text != "32")
            {
                LogComment("SetValue didn't update the value in the Binding");
                return TestResult.Fail;
            }
            return TestResult.Pass;
        }

        TestResult Step2()
        {
            WaitForPriority(DispatcherPriority.Render);
            // SetValue on OneWay binding blows away the binding
            // However, in this case we get binding before it gets blown away.
            BindingExpression be = _textblock1.GetBindingExpression(TextBlock.TextProperty);
            _textblock1.SetValue(TextBlock.TextProperty, "1");
            SetExpectedErrorTypeInStep(typeof(InvalidOperationException));
            be.UpdateTarget();
            LogComment("BindingExpression should be invalid, thus UpdateTarget should throw");

            return TestResult.Fail;
        }
        TestResult Step3()
        {
            WaitForPriority(DispatcherPriority.Render);
            // SetValue on OneWay binding blows away the binding
            _textblock1.SetValue(TextBlock.TextProperty, "1");
            BindingExpression be = _textblock1.GetBindingExpression(TextBlock.TextProperty);
            if (be != null)
            {
                LogComment("Yikes, the BindingExpression is not null!");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

    }    

}
