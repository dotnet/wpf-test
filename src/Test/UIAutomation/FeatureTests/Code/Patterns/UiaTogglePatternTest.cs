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
    /// Testing TogglePattern for Controls below
    /// ToggleButton
    /// CheckBox
    /// MenuItem
    /// </summary>
    [Serializable]
    public class UiaToggleTest : UiaSimpleTestcase
    {
        bool? _initialCheckBoxIsChecked;

        public bool? InitialCheckBoxIsChecked
        {
            get { return _initialCheckBoxIsChecked; }
            set { _initialCheckBoxIsChecked = value; }
        }

        bool _initialMenuItemIsChecked;

        public bool InitialMenuItemIsChecked
        {
            get { return _initialMenuItemIsChecked; }
            set { _initialMenuItemIsChecked = value; }
        }

        bool _initialIsThreeState = false;

        public bool InitialIsThreeState
        {
            get { return _initialIsThreeState; }
            set { _initialIsThreeState = value; }
        }

        public override void Init(object target)
        {
            ToggleButton togglebutton = target as ToggleButton;
            if (togglebutton != null)
            {
                //make sure that the checkbox is in the intial state
                if (togglebutton.IsChecked != _initialCheckBoxIsChecked)
                    togglebutton.IsChecked = _initialCheckBoxIsChecked;
                if (togglebutton.IsThreeState != _initialIsThreeState)
                    togglebutton.IsThreeState = _initialIsThreeState;
            }

            MenuItem menuitem = target as MenuItem;
            if (menuitem != null)
            {
                if (!menuitem.IsCheckable)
                {
                    menuitem.IsCheckable = true;
                }
                if (menuitem.IsChecked != _initialMenuItemIsChecked)
                    menuitem.IsChecked = _initialMenuItemIsChecked;
            }
        }

        public override void DoTest(AutomationElement target)
        {
            //UIVerifyLogger.LogEvidence("Calling TestRuns.RunAllTests(" + Library.GetUISpyLook(target) + ")");
            //TestRuns.RunAllTests(target, true, TestPriorities.Pri0, TestCaseType.Generic, true, true, null);

            TogglePattern tp = target.GetCurrentPattern(TogglePattern.Pattern) as TogglePattern;
            tp.Toggle();
            SharedState["targetToggleState"] = tp.Current.ToggleState;

        }

        public override void Validate(object target)
        {
            if (target is MenuItem || target is ToggleButton)
            {
                TestLog.Current.Result = TestResult.Pass;
            }
            else
            {
                TestLog.Current.LogEvidence("Unknown test object.");
                TestLog.Current.Result = TestResult.Fail;
            }
            if (target.GetType() == typeof(MenuItem))
            {
                MenuItem menuitem = target as MenuItem;
                if (menuitem != null)
                {
                    if (_initialMenuItemIsChecked == false)
                    {
                        if (menuitem.IsChecked != true)
                        {
                            TestLog.Current.LogEvidence("The IsChecked doesn't become true from false");
                            TestLog.Current.Result = TestResult.Fail;
                        }
                        if ((ToggleState)SharedState["targetToggleState"] != ToggleState.On)
                        {
                            TestLog.Current.LogEvidence("The ToggleState is not On when IsChecked is true");
                            TestLog.Current.Result = TestResult.Fail;
                        }
                    }
                    else if (_initialMenuItemIsChecked == true)
                    {
                        if (menuitem.IsChecked != false)
                        {
                            TestLog.Current.LogEvidence("The IsChecked doesn't become false from true");
                            TestLog.Current.Result = TestResult.Fail;
                        }
                        if ((ToggleState)SharedState["targetToggleState"] != ToggleState.Off)
                        {
                            TestLog.Current.LogEvidence("The ToggleState is not Off when IsChecked is false");
                            TestLog.Current.Result = TestResult.Fail;
                        }
                    }
                }
            }
            else if(target is ToggleButton)
            {
                ToggleButton togglebutton = target as ToggleButton;
                if (togglebutton != null)
                {
                    if (_initialIsThreeState)
                    {
                        if (_initialCheckBoxIsChecked == false)
                        {
                            if (togglebutton.IsChecked != true)
                            {
                                TestLog.Current.LogEvidence("The IsChecked doesn't become true from false");
                                TestLog.Current.Result = TestResult.Fail;
                            }
                            if ((ToggleState)SharedState["targetToggleState"] != ToggleState.On)
                            {
                                TestLog.Current.LogEvidence("The ToggleState is not On when IsChecked is true");
                                TestLog.Current.Result = TestResult.Fail;
                            }
                        }
                        else if (_initialCheckBoxIsChecked == true)
                        {
                            if (togglebutton.IsChecked != null)
                            {
                                TestLog.Current.LogEvidence("The IsChecked doesn't become null from true");
                                TestLog.Current.Result = TestResult.Fail;
                            }
                            if ((ToggleState)SharedState["targetToggleState"] != ToggleState.Indeterminate)
                            {
                                TestLog.Current.LogEvidence("The ToggleState is not Indeterminate when IsChecked is null");
                                TestLog.Current.Result = TestResult.Fail;
                            }
                        }
                        else if (_initialCheckBoxIsChecked == null)
                        {
                            if (togglebutton.IsChecked != false)
                            {
                                TestLog.Current.LogEvidence("The IsChecked doesn't become false from null");
                                TestLog.Current.Result = TestResult.Fail;
                            }
                            if ((ToggleState)SharedState["targetToggleState"] != ToggleState.Off)
                            {
                                TestLog.Current.LogEvidence("The ToggleState is not Off when IsChecked is false");
                                TestLog.Current.Result = TestResult.Fail;
                            }
                        }
                    }
                    else
                    {
                        if (_initialCheckBoxIsChecked == false)
                        {
                            if (togglebutton.IsChecked != true)
                            {
                                TestLog.Current.LogEvidence("The IsChecked doesn't become true from false");
                                TestLog.Current.Result = TestResult.Fail;
                            }
                            if ((ToggleState)SharedState["targetToggleState"] != ToggleState.On)
                            {
                                TestLog.Current.LogEvidence("The ToggleState is not On when IsChecked is true");
                                TestLog.Current.Result = TestResult.Fail;
                            }
                        }
                        else if (_initialCheckBoxIsChecked == true)
                        {
                            if (togglebutton.IsChecked != false)
                            {
                                TestLog.Current.LogEvidence("The IsChecked doesn't become false from true");
                                TestLog.Current.Result = TestResult.Fail;
                            }
                            if ((ToggleState)SharedState["targetToggleState"] != ToggleState.Off)
                            {
                                TestLog.Current.LogEvidence("The ToggleState is not Off when IsChecked is false");
                                TestLog.Current.Result = TestResult.Fail;
                            }
                        }
                    }
               }
            }
        }
    }
}
