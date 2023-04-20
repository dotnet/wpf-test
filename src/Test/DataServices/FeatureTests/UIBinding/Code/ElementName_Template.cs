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
    /// ElementName should be able to peer outside Template from in a DataTrigger.
    /// </description>
    /// <relatedBugs>

    /// </relatedBugs>
    /// </summary>



    [Test(1, "Binding", "ElementName_Template")]
    public class ElementName_Template : XamlTest
    {
        TextBox _textbox;
        TextBlock _textblock;
        public ElementName_Template()
            : base(@"ElementNameTemplate.xaml")
        {
            InitializeSteps += new TestStep(Init);
            RunSteps += new TestStep(ChangeTextBoxValue);
            RunSteps += new TestStep(VerifyStyle);

        }

        TestResult Init()
        {
            WaitForPriority(System.Windows.Threading.DispatcherPriority.ApplicationIdle);
            _textblock = LogicalTreeHelper.FindLogicalNode(RootElement, "textblock") as TextBlock;
            _textbox = LogicalTreeHelper.FindLogicalNode(RootElement, "textbox") as TextBox;

            if (_textblock == null)
            {
                LogComment("TextBlock was not found in the Logical Tree!");
                return TestResult.Fail;
            }
            if (_textblock.Foreground != System.Windows.Media.Brushes.Blue) //#FF0000FF
            {
                LogComment("TextBlock Foreground was not set to Blue!");
                return TestResult.Fail;
            }


            if (_textbox == null)
            {
                LogComment("TextBox was not found in the Logical Tree!");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }


        TestResult ChangeTextBoxValue()
        {
            _textbox.Text = "ChangeColorToGreen";
            return TestResult.Pass;
        }

        TestResult VerifyStyle()
        {
            WaitForPriority(System.Windows.Threading.DispatcherPriority.ApplicationIdle);
            if (_textblock.Foreground != System.Windows.Media.Brushes.Green) //#FF008000
            {
                LogComment("TextBlock Foreground was changed to Green!");
                return TestResult.Fail;
            }
            return TestResult.Pass;
        }

    }
}
