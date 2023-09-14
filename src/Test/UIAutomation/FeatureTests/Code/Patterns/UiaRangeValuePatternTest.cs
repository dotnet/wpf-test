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
    /// Testing RangeValuePattern for Controls Below
    /// ProgressBar
    /// ScrollBar
    /// Slider
    /// </summary>
    [Serializable]
    public class UiaRangeValueTest : UiaSimpleTestcase
    {
        public override void Init(object target)
        {
        }

        public override void DoTest(AutomationElement target)
        {
            TestLog.Current.Result = TestResult.Pass;

            RangeValuePattern rvp = target.GetCurrentPattern(RangeValuePattern.Pattern) as RangeValuePattern;
            SharedState["targetValue"] = rvp.Current.Value;
            SharedState["targetIsReadOnly"] = rvp.Current.IsReadOnly;
            SharedState["targetLargeChange"] = rvp.Current.LargeChange;
            SharedState["targetMaximum"] = rvp.Current.Maximum;
            SharedState["targetMinimum"] = rvp.Current.Minimum;
            SharedState["targetSmallChange"] = rvp.Current.SmallChange;
        }

        public override void Validate(object target)
        {
            if (target.GetType() == typeof(ProgressBar))
            {
                ProgressBar progressbar = target as ProgressBar;
                if (progressbar != null)
                {
                    if (progressbar.Minimum != (double)SharedState["targetValue"])
                    {
                        TestLog.Current.LogEvidence("The value doesn't work.");
                        TestLog.Current.Result = TestResult.Fail;
                    }
                    else if (!(bool)SharedState["targetIsReadOnly"])
                    {
                        TestLog.Current.LogEvidence("The IsReadOnly is not true.");
                        TestLog.Current.Result = TestResult.Fail;
                    }
                    else if (progressbar.Minimum != (double)SharedState["targetMinimum"])
                    {
                        TestLog.Current.LogEvidence("The minimum doesn't work.");
                        TestLog.Current.Result = TestResult.Fail;
                    }
                    else if (progressbar.Maximum != (double)SharedState["targetMaximum"])
                    {
                        TestLog.Current.LogEvidence("The maximum doesn't work.");
                        TestLog.Current.Result = TestResult.Fail;
                    }
                    // LargeChange doesn't work on Japanese Tablet OS.
                    else if (!double.IsNaN((double)SharedState["targetLargeChange"]))
                    {
                        TestLog.Current.LogEvidence("The LargeChange doesn't work.");
                        TestLog.Current.LogEvidence("UIElement LargeChange = " + progressbar.LargeChange.ToString());
                        TestLog.Current.LogEvidence("AutomationElement LargeChange = " + SharedState["targetLargeChange"]);
                        double temp = 2.0 + (double)SharedState["targetLargeChange"];
                        TestLog.Current.LogEvidence("AutomationElement LargeChange + 2 = " + temp);
                        TestLog.Current.Result = TestResult.Fail;
                    }
                    else if (!double.IsNaN((double)SharedState["targetSmallChange"]))
                    {
                        TestLog.Current.LogEvidence("The SmallChange doesn't work.");
                        TestLog.Current.Result = TestResult.Fail;
                    }
                }
            }
            else if (target.GetType() == typeof(ScrollBar))
            {
                ScrollBar scrollbar = target as ScrollBar;
                if (scrollbar != null)
                {
                    if (scrollbar.Minimum != (double)SharedState["targetValue"])
                    {
                        TestLog.Current.LogEvidence("The value doesn't work.");
                        TestLog.Current.LogEvidence("TargetValue = ." + SharedState["targetValue"].ToString());
                        TestLog.Current.Result = TestResult.Fail;
                    }
                    else if ((bool)SharedState["targetIsReadOnly"])
                    {
                        TestLog.Current.LogEvidence("The IsReadOnly is not false.");
                        TestLog.Current.Result = TestResult.Fail;
                    }
                    else if (scrollbar.Minimum != (double)SharedState["targetMinimum"])
                    {
                        TestLog.Current.LogEvidence("The minimum doesn't work.");
                        TestLog.Current.Result = TestResult.Fail;
                    }
                    else if (scrollbar.Maximum != (double)SharedState["targetMaximum"])
                    {
                        TestLog.Current.LogEvidence("The maximum doesn't work.");
                        TestLog.Current.Result = TestResult.Fail;
                    }
                    else if (scrollbar.LargeChange != (double)SharedState["targetLargeChange"])
                    {
                        TestLog.Current.LogEvidence("The LargeChange doesn't work.");
                        TestLog.Current.Result = TestResult.Fail;
                    }
                    else if (scrollbar.SmallChange != (double)SharedState["targetSmallChange"])
                    {
                        TestLog.Current.LogEvidence("The SmallChange doesn't work.");
                        TestLog.Current.Result = TestResult.Fail;
                    }
                }
            }
            else if (target.GetType() == typeof(Slider))
            {
                Slider slider = target as Slider;
                if (slider != null)
                {
                    if (slider.Minimum != (double)SharedState["targetValue"])
                    {
                        TestLog.Current.LogEvidence("The value doesn't work.");
                        TestLog.Current.LogEvidence("TargetValue = ." + SharedState["targetValue"].ToString());
                        TestLog.Current.Result = TestResult.Fail;
                    }
                    else if ((bool)SharedState["targetIsReadOnly"])
                    {
                        TestLog.Current.LogEvidence("The IsReadOnly is not false.");
                        TestLog.Current.Result = TestResult.Fail;
                    }
                    else if (slider.Minimum != (double)SharedState["targetMinimum"])
                    {
                        TestLog.Current.LogEvidence("The minimum doesn't work.");
                        TestLog.Current.Result = TestResult.Fail;
                    }
                    else if (slider.Maximum != (double)SharedState["targetMaximum"])
                    {
                        TestLog.Current.LogEvidence("The maximum doesn't work.");
                        TestLog.Current.Result = TestResult.Fail;
                    }
                    else if (slider.LargeChange != (double)SharedState["targetLargeChange"])
                    {
                        TestLog.Current.LogEvidence("The LargeChange doesn't work.");
                        TestLog.Current.Result = TestResult.Fail;
                    }
                    else if (slider.SmallChange != (double)SharedState["targetSmallChange"])
                    {
                        TestLog.Current.LogEvidence("The SmallChange doesn't work.");
                        TestLog.Current.Result = TestResult.Fail;
                    }
                }
            }
            else
            {
                TestLog.Current.LogEvidence("Unknown test object.");
                TestLog.Current.Result = TestResult.Fail;
            }
        }
    }

}
