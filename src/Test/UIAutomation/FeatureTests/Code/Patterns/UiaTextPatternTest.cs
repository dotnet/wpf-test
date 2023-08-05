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


namespace Avalon.Test.ComponentModel
{
    /// <summary>
    /// Testing TextPattern
    /// FlowDocument
    /// </summary>
    [Serializable]
    public class UiaTextTest : UiaSimpleTestcase
    {
        public override void Init(object target)
        {
        }

        public override void DoTest(AutomationElement target)
        {
            TextPattern tp = target.GetCurrentPattern(TextPattern.Pattern) as TextPattern;
            SharedState["SupportedTextSelection"] = tp.SupportedTextSelection.ToString();
        }

        public override void Validate(object target)
        {
            TestLog.Current.Result = TestResult.Pass;
            TestLog.Current.LogEvidence("SupportedTextSelection is " + (string)SharedState["SupportedTextSelection"]);
        }
    }

}
