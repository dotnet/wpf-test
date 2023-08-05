// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Microsoft.Test.Logging;
using Microsoft.Test.Hosting;
using Microsoft.Test.UIAutomaion;


namespace Avalon.Test.ComponentModel
{
    /// <summary>
    /// Testing ValuePattern for Controls Below
    /// ComboBox
    /// </summary>
    [Serializable]
    public class UiaValueTest : UiaSimpleTestcase
    {
        public override void Init(object target)
        {
            ((ComboBox)target).IsEditable = true;
        }

        public override void DoTest(AutomationElement target)
        {
            ValuePattern vp = target.GetCurrentPattern(ValuePattern.Pattern) as ValuePattern;
            vp.SetValue("Automation SetValue");
            SharedState["targetValue"] = vp.Current.Value;
            SharedState["targetIsReadOnly"] = vp.Current.IsReadOnly;
        }

        public override void Validate(object target)
        {
            TestLog.Current.Result = TestResult.Pass;
            if (((ComboBox)target).Text != (string)SharedState["targetValue"])
            {
                TestLog.Current.LogEvidence("The Value doesn't work");
                TestLog.Current.Result = TestResult.Fail;
            }
            else if (((ComboBox)target).IsReadOnly != (bool)SharedState["targetIsReadOnly"])
            {
                TestLog.Current.LogEvidence("The IsReadOnly doesn't work");
                TestLog.Current.Result = TestResult.Fail;
            }
        }
    }

}
