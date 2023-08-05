// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Microsoft.Test.Logging;
using Microsoft.Test.Hosting;
using System.Windows;
using Microsoft.Test.UIAutomaion;
using System.Windows.Input;


namespace Avalon.Test.ComponentModel
{
    /// <summary>
    /// Testing InvokePattern for Controls Below
    /// Button
    /// RepeatButton
    /// MenuItem
    /// </summary>
    [Serializable]
    public class UiaInvokeTest : UiaSimpleTestcase
    {
        bool _invoked = false;

        public override void Init(object target)
        {
            //attach clickhandler
            ButtonBase buttonbase = target as ButtonBase;
            
            if (buttonbase != null)
                buttonbase.Click += new RoutedEventHandler(target_Click);

            MenuItem menuitem = target as MenuItem;
            if (target is MenuItem)
                ((MenuItem)target).Click += new RoutedEventHandler(target_Click);
        }

        void target_Click(object sender, RoutedEventArgs e)
        {
            _invoked = true;
            TestLog.Current.LogEvidence("The click event was raised");
        }

        public override void DoTest(AutomationElement target)
        {
            InvokePattern ip = target.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
            ip.Invoke();
        }

        public override void Validate(object target)
        {
            if (_invoked)
            {
                TestLog.Current.Result = TestResult.Pass;
            }
            else
            {
                TestLog.Current.LogEvidence("The click event was not raised");
                TestLog.Current.Result = TestResult.Fail;
            }
        }
    }
}
