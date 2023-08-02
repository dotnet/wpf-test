// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/////////////////////////////////////////////////////////////////////
// 









using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Markup;
using System.Windows.Threading;
using System.Reflection;
using System.IO;
using System.Windows;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using System.Threading;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Threading;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.Trusted.Controls.KeyboardInteropModelControls;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Modeling;
using Microsoft.Test.Threading;
using Microsoft.Test.Logging;
using Microsoft.Test.Win32;

using SWF = System.Windows.Forms;

namespace Avalon.Test.Hosting
{
    /// <summary>
    /// KeyboardInteropModel
    /// </summary>
    /// 
    [Model(@"FeatureTests\ElementServices\KeyboardInterop.xtc", 1, @"Hosting\WinformsKeyboardInteropModel", TestCaseSecurityLevel.FullTrust, "WinformsKeyboardInteropModel", ExpandModelCases=true, Area="AppModel", Disabled=true)]
    public class KeyboardInteropModel : CoreModel
    {
        /// <summary>
        /// Construct an instance of KeyboardInteropModel.
        /// </summary>
        public KeyboardInteropModel()
            : base()
        {
            Name = "KeyboardInteropModel";
            Description = "KeyboardInterop Model";     

            // Add Action Handlers

            if (_test == null)
            {
                _test = new KeyboardInteropTest();
            }

            OnBeginCase += new StateEventHandler(OnBeginCase_Handler);

            OnEndCase += new StateEventHandler(OnEndCase_Handler);

            AddAction("launch", new ActionHandler(LaunchApplication));
            AddAction("acc", new ActionHandler(VerifyAccelerator));
            AddAction("mnemonic", new ActionHandler(VerifyMnemonic));
            AddAction("tab", new ActionHandler(VerifyTab));
        }

        private void OnBeginCase_Handler(object sender, StateEventArgs e)
        {
            if (_test == null)
            {
                _test = new KeyboardInteropTest();
            }
        }

        private void OnEndCase_Handler(object sender, StateEventArgs e)
        {
            if (_test.ReportResult() == false)
                CoreLogger.LogTestResult(false, "Tese case failed");
        }

        private bool LaunchApplication(State endState, State inParams, State outParams)
        {
            CoreLogger.LogStatus("LauchApplication");
            return _test.LaunchApplication(endState, inParams, outParams);
            
        }

        private bool VerifyAccelerator(State endState, State inParams, State outParams)
        {
            return _test.VerifyAccelerator(endState, inParams, outParams);
        }

        private bool VerifyMnemonic(State endState, State inParams, State outParams)
        {
            return _test.VerifyMnemonic(endState, inParams, outParams);
        }


        private bool VerifyTab(State endState, State inParams, State outParams)
        {
            return _test.VerifyTab(endState, inParams, outParams);
        }

        private KeyboardInteropTest _test;
    }
     
    /// <summary>
    /// Run test cases and verify states
    /// </summary>
    public partial class KeyboardInteropTest
    {
        List<IKeyboardInteropTestControl> _testControls;
        IKeyboardInteropTestControl _parent;
        IKeyboardInteropTestControl _childA;
        IKeyboardInteropTestControl _grandchildAA;
        List<Key> _testMnemonics;  // List of characters assigned as Alt-* mnemonic to each test control.
        WinForm _form;

        // To report the test result
        private struct ActionInfo
        {
            public string Action;   
            public string Param;
            public bool Result;
        }

        List<ActionInfo> _actionInfoList; 

        /// <summary>
        /// construct an instance 
        /// </summary>
        public KeyboardInteropTest()
        {
            _actionInfoList = new List<ActionInfo>();
        }

        /// <summary>
        /// Create layered windows
        /// </summary>
        /// <param name="endState"></param>
        /// <param name="inParams">layered windows types</param>
        /// <param name="outParams"></param>
        /// <returns></returns>
        public bool LaunchApplication(State endState, State inParams, State outParams)
        {
            BuildTree(inParams);

            return true;
        }

        // Create layered windows
        private void BuildTree(State inParams)
        {
            _testControls = new List<IKeyboardInteropTestControl>();

            ActionInfo ai = new ActionInfo();

            ai.Action = "LoadHwnds";
            ai.Param = "Parent: ";

            string param = (string) inParams["Parent"];

            if (param == "WinForm")
            {
                _form = LaunchForm();

                // Wait for WinForm thread to start, then bring window to foreground
                Thread.Sleep(500);
                _form.Activate();
                
                _parent = (IKeyboardInteropTestControl)_form;

                _testControls.Add(_parent);
                ai.Param += param;

                param = (string)inParams["Child"];

                if (param != "SourcedAvalon")
                {
                    throw new Exception("Child Type" + param + " not supported");
                }

                ai.Param = ai.Param + " Child: " + param;

                // Since Windowsformshost\ElementHost does not support nested level
                // we can make its child hard-coded.
                _childA = (IKeyboardInteropTestControl)_form.Child;
                if (_childA == null)
                {
                    throw new Exception("Failed to add Sourced Avalon");
                }

                _testControls.Add(_childA);

            }
            else
            {
                if (param == "Avalon")
                {
                    _parent = (IKeyboardInteropTestControl)new AvalonWindow();
                }
                else if (param == "HwndHost")
                {
                    _parent = (IKeyboardInteropTestControl)new Win32Window();
                }
                else
                {
                    throw new Exception("Parent Type" + param + " not supported");
                }

                if (_parent == null)
                {
                    throw new Exception("can not create top level " + param + " window");
                }

                _testControls.Add(_parent);

                ai.Param += param;

                param = (string)inParams["Child"];
                _childA = _parent.AddChild(param);
                if (_childA == null)
                {
                    throw new Exception("Can not create " + param + " child.");
                }

                _testControls.Add(_childA);

                ai.Param = ai.Param + " Child: " + param;

                param = (string)inParams["Grandchild"];
                if ((param != "None"))
                {
                    _grandchildAA = _childA.AddChild(param);
                    if (_grandchildAA == null)
                    {
                        throw new Exception("Can not create " + param + " grandchild.");
                    }
                    _testControls.Add(_grandchildAA);
                }

                ai.Param = ai.Param + " Grandchild: " + param;
            }
   
            ai.Result = true;

            _actionInfoList.Add(ai);
       }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="endState"></param>
        /// <param name="inParams"></param>
        /// <param name="outParams"></param>
        /// <returns></returns>
        public bool VerifyAccelerator(State endState, State inParams, State outParams)
        {
            bool bPass = false;

            string param = (string)inParams["accelerator"];

            CoreLogger.LogStatus("****Validating "+param+" Accelerator************", ConsoleColor.Yellow);

            switch (param)
            {
                case "common":
                    bPass = VerifyCommonAccelerator();
                    break;
                case "coverride":
                    bPass = VerifyChildAccelerator();
                    break;
                case "unique":
                    bPass = VerifyUniqueAccelerator();
                    break;
                case "poverride":
                    bPass = VerifyParentAccelerator();
                    break;
                case "parentfirst":
                    bPass = VerifyParentFirstAccelerator();
                    break;
                default:
                    throw new Exception("Acc type " + inParams["acc"] + " not supported");
            }

            ResetAllTestStates();

            ActionInfo ai = new ActionInfo();
            ai.Action = "Accelerator";
            ai.Param = (string)inParams["accelerator"];
            ai.Result = bPass;

            _actionInfoList.Add(ai);

            if (bPass)
                CoreLogger.LogStatus("Accelerator: " + param + " passed", ConsoleColor.Green);
            else
                CoreLogger.LogStatus("Accelerator: " + param + " failed", ConsoleColor.Red);

            CoreLogger.LogStatus("****Validating "+param+" Accelerator Done*********", ConsoleColor.Yellow);

            return true;
        }


        // child override accelerator
        private bool VerifyChildAccelerator()
        {
            bool testPassed = true;

            if (_parent is Win32Window || _parent is WinForm || _childA is HostedWinFormsControl)
            {
                CoreLogger.LogStatus("not supported");
                return true;
            }

            foreach (IKeyboardInteropTestControl testControl in _testControls)
            {
                testControl.SetFocusToFirstChild(true);
                InjectChildAccelerator();
            }

            foreach (IKeyboardInteropTestControl testControl in _testControls)
            {
                string owner = testControl.ToString();
                AcceleratorTestState accTestState = testControl.RecordedAcceleratorTestState;
                if (owner != accTestState.Owner)
                {
                    testPassed = false;
                    CoreLogger.LogStatus(testControl + " child failed to override accelerator", ConsoleColor.Red);
                }
                if (accTestState.ControlState != true)
                {
                    testPassed = false;
                    CoreLogger.LogStatus(testControl + " control key not typed", ConsoleColor.Red);
                }
                if (accTestState.KeyState != true)
                {
                    testPassed = false;
                    CoreLogger.LogStatus(testControl + " key not typed", ConsoleColor.Red);
                }
            }

            return testPassed;
        }

        private bool VerifyParentFirstAccelerator()
        {
            bool testPassed = true;

            if (_parent is WinForm || _childA is HostedWinFormsControl)
            {
                CoreLogger.LogStatus("not supported");
                return true;
            }
            else if (_parent is AvalonWindow && !(_grandchildAA is SourcedAvalon))
            {
                CoreLogger.LogStatus("test case can not test parent first handle acc scenario");
                return true;
            }

            ExpectedAccelTestState eAcc = new ExpectedAccelTestState();
            eAcc.TestType = AccelTestType.PFirst;

            foreach (IKeyboardInteropTestControl testControl in _testControls)
            {
                testControl.ExpectedAcceleratorTestState = eAcc;
            }

            

            if (_grandchildAA != null) 
                _grandchildAA.SetFocusToFirstChild(true);
            else
                _childA.SetFocusToFirstChild(true);

            InjectPFirstAccelerator();
            
            int sumCommand = 0;
            foreach (IKeyboardInteropTestControl testControl in _testControls)
            {
                AcceleratorTestState accTestState = testControl.RecordedAcceleratorTestState;
                sumCommand += accTestState.SumCommand;
             }

             if (sumCommand != 2)
             {
                 testPassed = false;
                 CoreLogger.LogStatus("Accelerator should be handled: 2 times, unexpected: " + sumCommand, ConsoleColor.Red);
             }

            return testPassed;

        }

        // parent override 
        private bool VerifyParentAccelerator()
        {
            bool testPassed = true;

            if (_childA is HostedWinFormsControl || _parent is WinForm)
            {
                CoreLogger.LogStatus("This test case can not test this scenario");
                return true;
            }


            foreach (IKeyboardInteropTestControl testControl in _testControls)
            {
                testControl.SetFocusToFirstChild(true);
                InjectParentAccelerator();
            }

            foreach (IKeyboardInteropTestControl testControl in _testControls)
            {
                AcceleratorTestState accState = testControl.RecordedAcceleratorTestState;
                if (testControl is AvalonWindow || testControl is Win32Window)
                {
                    if (accState.SumCommand != _testControls.Count)
                    {
                        testPassed = false;
                        CoreLogger.LogStatus("Parent: " + testControl + " Expected called time: " + _testControls.Count + " unexpected: " + accState.SumCommand, ConsoleColor.Red);
                    }
                }
                else
                {
                    if (accState.SumCommand != 0)
                    {
                        testPassed = false;
                        CoreLogger.LogStatus("Child: " + testControl + " Expected called time: 0 unexpected: " + accState.SumCommand);
                    }
                }

                //if (accState.ControlState == false || accState.KeyState == false)
                //{
                //    testFailed = true;
                //    CoreLogger.LogStatus(testControl + " no the same accelerator");
                //}
            }

            return testPassed;
        }

        // common accelerators
        private bool VerifyCommonAccelerator()
        {
            bool testPassed = true;

            // simulate input
            foreach (IKeyboardInteropTestControl testControl in _testControls)
            {
                testControl.SetFocusToFirstChild(true);
                InjectCommonAccelerator();
            }

            if (_parent is WinForm)
            {
                // make sure form thread runs first
                Thread.Sleep(500);
            }
           
            // validate behaviors
            foreach (IKeyboardInteropTestControl testControl in _testControls)
            {
                AcceleratorTestState accTestState = testControl.RecordedAcceleratorTestState;
                if (accTestState.SumCommand != 1)
                {
                    CoreLogger.LogStatus(testControl + " Common accelerator: unexpected called times: " + accTestState.SumCommand +
                                            " expected 1", ConsoleColor.Red);
                    testPassed = false;
                }
                if (accTestState.ControlState == false)
                {
                    CoreLogger.LogStatus(testControl + " Common accelerator: unexpected modifier key", ConsoleColor.Red);
                    testPassed = false;
                }
                if (accTestState.KeyState == false)
                {
                    CoreLogger.LogStatus(testControl + " Common accelerator: unexpected key", ConsoleColor.Red);
                    testPassed = false;
                }
            }

            return testPassed;
        }

        private bool VerifyUniqueAccelerator()
        {
            bool testPassed = true;

            // simulate input
            foreach (IKeyboardInteropTestControl testControl in _testControls)
            {
                testControl.SetFocusToFirstChild(true);
                InjectUniqueAccelerator();
            }

            if (_parent is WinForm)
            {
                // make sure form thread runs first
                Thread.Sleep(500);
            }
            
            // validate behaviors
             for(int index=0; index < _testControls.Count; index++)
            {
                IKeyboardInteropTestControl testControl = _testControls[index];
                AcceleratorTestState accTestState = testControl.RecordedAcceleratorTestState;
                if (testControl is HostedWinFormsControl)
                {
                    if (accTestState.SumCommand != 1)
                    {
                        CoreLogger.LogStatus(testControl + " Unique accelerator: unexpected called times: " + accTestState.SumCommand +
                                                " expected: " + (_testControls.Count - index), ConsoleColor.Red);
                        testPassed = false;
                    }
                }
                else
                {
                    if (accTestState.SumCommand != (_testControls.Count - index))
                    {
                        CoreLogger.LogStatus(testControl + " Unique accelerator: unexpected called times: " + accTestState.SumCommand +
                                                " expected: " + (_testControls.Count - index), ConsoleColor.Red);
                        testPassed = false;
                    }
                }
                if (accTestState.ControlState == false)
                {
                    CoreLogger.LogStatus(testControl + " Unique accelerator: unexpected modifier key", ConsoleColor.Red);
                    testPassed = false;
                }
                if (accTestState.KeyState == false)
                {
                    CoreLogger.LogStatus(testControl + " Unique accelerator: unexpected key", ConsoleColor.Red);
                    testPassed = false;
                }
            }

            return testPassed;
        }

  

        private bool VerifyCommonMnemonics()
        {
            bool testPassed = true;

            foreach (IKeyboardInteropTestControl testControl in _testControls)
            {
                // Set focus on the current control's first child.
                testControl.SetFocusToFirstChild(true);

                // Inject every mnemonic with focus on this control.
                injectMnemonics(true);

                if (_parent is WinForm)
                {
                    // wait for WinForm thread 
                    Thread.Sleep(500);
                }

                // Verify each test control received its mnemonic and recieved it only once.
                
                int sumMnemonics = 0;
                foreach (IKeyboardInteropTestControl checkControl in _testControls)
                {
                    sumMnemonics += checkControl.RecordedMnemonics;
                }
                if (sumMnemonics != 1)
                {
                    testPassed = false;
                    CoreLogger.LogStatus("Common Mnemonic: unexpected: " + sumMnemonics + " expected 1", ConsoleColor.Red);
                }
                ResetAllTestStates();
           }
         
            return testPassed;
        }

        private bool VerifyUniqueMnemonics()
        {
            bool testPassed = true;

            // Apply all mnemonics with focus on each control. All controls should receive their mnemonic
            // on each pass.
            foreach (IKeyboardInteropTestControl testControl in _testControls)
            {
                // Set focus on the current control's first child.
                testControl.SetFocusToFirstChild(true);

                // Inject every mnemonic with focus on this control.
                injectMnemonics(false);

                if (_parent is WinForm)
                {
                    Thread.Sleep(500);
                }

                // Allow injected mnemonics time to be handled.
                DispatcherHelper.DoEvents(200);

                // Verify each test control received its mnemonic and recieved it only once.
                foreach (IKeyboardInteropTestControl checkControl in _testControls)
                {
                    if (checkControl.RecordedMnemonics != 1)
                    {
                        testPassed = false;
                        CoreLogger.LogStatus("Control " + checkControl + " got " + checkControl.RecordedMnemonics + " wanted 1");
                    }
                }
                ResetAllTestStates();
            }

            return testPassed;
 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="endState"></param>
        /// <param name="inParams"></param>
        /// <param name="outParams"></param>
        /// <returns></returns>
        public bool VerifyMnemonic(State endState, State inParams, State outParams)
        {
            bool bPass = false;

            string param = (string)inParams["mnemonics"];

            AddMnemonicsToControls(param);

            CoreLogger.LogStatus("*** Validating mnemonics", ConsoleColor.Yellow);

            switch (param)
            {
                case "common":
                    bPass = VerifyCommonMnemonics();
                    break;
                case "unique":
                    bPass = VerifyUniqueMnemonics();
                    break;
                default:
                    CoreLogger.LogStatus("mnemonic type not supported");
                    bPass = false;
                    break;
            }

            ResetAllTestStates();

            if (bPass)
                CoreLogger.LogStatus(param + " mnemonic:  passed", ConsoleColor.Green);
            else
                CoreLogger.LogStatus(param + " mnemonic:  failed", ConsoleColor.Red);

            CoreLogger.LogStatus("*** Done validating mnemonics**************", ConsoleColor.Yellow);

            ActionInfo ai = new ActionInfo();
            ai.Action = "mnemonic";
            ai.Param = param;
            ai.Result = bPass;

            _actionInfoList.Add(ai);

            return true;
        }

        private bool VerifyTab(string tabType)
        {
            bool testPassed = true;

            int tabStops = 0;

            foreach (IKeyboardInteropTestControl testControl in _testControls)
            {
                testControl.NeedRecord = true;
                tabStops += testControl.ExpectedTabTestState.RecordedTabStops;
            }

            CoreLogger.LogStatus("SetFocus To First Child");
            if (tabType == "tab")
                _testControls[0].SetFocusToFirstChild(true);
            else
                _testControls[0].SetFocusToFirstChild(false);


            // Insert tabs starting from first control.
            for (int t = 0; t < tabStops - 1; t++)
            {
                CoreLogger.LogStatus("Tab!", ConsoleColor.Cyan);
                if (tabType == "tab")
                    KeyboardHelper.TypeKey(Key.Tab);
                else
                    KeyboardHelper.TypeKey(Key.Tab, ModifierKeys.Shift);
            }

            if (_parent is WinForm)
            {
                // make sure that form thread runs first
                Thread.Sleep(500);
            }

            CoreLogger.LogStatus("Done tabbing.", ConsoleColor.Cyan);

            // Check tabs counted by each test control.
            foreach (IKeyboardInteropTestControl testControl in _testControls)
            {
                TabTestState tabTest = testControl.RecordedTabTestState;
                TabTestState expTest = testControl.ExpectedTabTestState;
                //bool tabUnequal = false;
                // make exceptions to Avalon, SourcedAvalon due to that gotKeyboardFocus is sticky
                if (testControl is AvalonWindow || testControl is SourcedAvalon)
                {
                    if ((tabTest.RecordedTabStops != expTest.RecordedTabStops
                            && tabTest.RecordedTabStops != expTest.RecordedTabStops + 1))
                    {
                        CoreLogger.LogStatus(testControl + " RecordedTabStops: " + tabTest.RecordedTabStops + " Expected: " + expTest.RecordedTabStops,
                                            ConsoleColor.Red);
                        testPassed = false;
                    }

                }
                else
                {
                    if (tabTest.RecordedTabStops != expTest.RecordedTabStops)
                    {
                        CoreLogger.LogStatus(testControl + " RecordedTabStops: " + tabTest.RecordedTabStops + " Expected: " + expTest.RecordedTabStops,
                                            ConsoleColor.Red);
                        testPassed = false;
                    }
                }

                //CoreLogger.LogStatus("Recorded First tab: " + tabTest.RecordedFirstTabElement);

                //tabUnequal = ((tabTest.RecordedFirstTab != expTest.RecordedFirstTab)
                //                    || (tabTest.RecordedFirstTabElement != expTest.RecordedFirstTabElement));
                //if (tabUnequal)
                //{
                //    testPassed = false;
                //    CoreLogger.LogStatus(testControl + "RecordedFirstTab != Expected First tab", ConsoleColor.Red);
                //}
            }

            return testPassed;
        }

        // tab wrap: only test against SourcedAvalon
        private bool VerifyTabLooping(string tabType)
        {
            if (!(_parent is Win32Window))
            {
                CoreLogger.LogStatus("not supported");
                return true;
            }

            int totalTabStops = 0;
            bool testPassed = true;

            TabTestState e = new TabTestState();
            e.TestType = TabTestType.Loop;

            //TabTestState tabTest;

            foreach (IKeyboardInteropTestControl testControl in _testControls)
            {
                // Set Test Type
                testControl.ExpectedTabTestState = e;
                testControl.NeedRecord = true;
                totalTabStops += testControl.ExpectedTabTestState.RecordedTabStops;
            }

            if (tabType == "tab")
                _testControls[0].SetFocusToFirstChild(true);
            else
                _testControls[0].SetFocusToFirstChild(false);


            // Insert tabs starting from first control.
            for (int t = 0; t < totalTabStops - 1; t++)
            {
                CoreLogger.LogStatus("Tab!", ConsoleColor.Cyan);
                if (tabType == "tab")
                    KeyboardHelper.TypeKey(Key.Tab);
                else
                    KeyboardHelper.TypeKey(Key.Tab, ModifierKeys.Shift);
            }

           
            CoreLogger.LogStatus("Done tabbing", ConsoleColor.Cyan);

            if (_childA is SourcedAvalon)
            {
                // there is only one button, i.e. one tab stop 
                if (_parent.RecordedTabTestState.RecordedTabStops != 1)
                {
                    testPassed = false;
                    CoreLogger.LogStatus("Parent recorded tab: " + _parent.RecordedTabTestState.RecordedTabStops
                                                + " expected: 1", ConsoleColor.Red);
                }
            }

            if (_grandchildAA is SourcedAvalon)
            {
                // there is only one button, i.e. one tab stop 
                if (_childA.RecordedTabTestState.RecordedTabStops != 1)
                {
                    testPassed = false;
                    CoreLogger.LogStatus("Child recorded tab: " + _childA.RecordedTabTestState.RecordedTabStops
                                                + " expected: 1", ConsoleColor.Red);
                }
            }

            return testPassed;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="endState"></param>
        /// <param name="inParams"></param>
        /// <param name="outParams"></param>
        /// <returns></returns>
        public bool VerifyTab(State endState, State inParams, State outParams)
        {
 
            string tabType = (string)inParams["tabs"];
            string tabKey = (string)inParams["tabkey"];

            bool bPass = false;

            CoreLogger.LogStatus("****Validating Tab*********", ConsoleColor.Yellow);
            CoreLogger.LogStatus("*Action: " + tabType + "      Keys: " + tabKey, ConsoleColor.Yellow);

            switch (tabType)
            {
                case "tab":
                case "shifttab":
                    bPass = VerifyTab(tabType);
                    break;
                case "normal":
                    bPass = VerifyTabAsNormal(tabKey);
                    break;
                case "wrap":
                    bPass = VerifyTabLooping(tabKey);
                    break;
            }

            ResetAllTestStates();

            if (bPass)
               CoreLogger.LogStatus(tabType + " tab passed", ConsoleColor.Green);
            else
               CoreLogger.LogStatus(tabType + " tab failed", ConsoleColor.Red);

            ActionInfo ai = new ActionInfo();
            ai.Action = "Tabbing";
            ai.Param = tabType + " Keys: " + tabKey;
            ai.Result = bPass;

            _actionInfoList.Add(ai);

            CoreLogger.LogStatus("****Validating Tab Done*********", ConsoleColor.Yellow);


            return true;
        }

        // Only HostedHwndControl takes tab as normal input, the others should take it as regular tab
        bool VerifyTabAsNormal(string tabType)
        {
            int tabStops = 0;
            bool testPassed = true;

            //if (!(_parent is AvalonWindow && _childA is HostedHwndControl))
            if (_childA is HostedHwndControl)
            {
                // ignore the test case for now, tabbing on HostedHwndControl 
                // requires further investigation
                CoreLogger.LogStatus("This test case can not test this scenario");
                return true;
            }


            TabTestState e = new TabTestState();
            e.TestType = TabTestType.Normal;
            foreach (IKeyboardInteropTestControl testControl in _testControls)
            {
                // Set Test Type
                testControl.ExpectedTabTestState = e;
                testControl.NeedRecord = true;
                tabStops += testControl.ExpectedTabTestState.RecordedTabStops;
            }

            // Set Focus
            if (tabType == "tab")
                _testControls[0].SetFocusToFirstChild(true);
            else
                _testControls[0].SetFocusToFirstChild(false);


            // Insert tabs starting from first control.
            for (int t = 0; t < tabStops - 1; t++)
            {
                CoreLogger.LogStatus("Tab!", ConsoleColor.Cyan);
                if (tabType == "tab")
                    KeyboardHelper.TypeKey(Key.Tab);
                else
                    KeyboardHelper.TypeKey(Key.Tab, ModifierKeys.Shift);
            }
            CoreLogger.LogStatus("Done tabbing.", ConsoleColor.Cyan);

            // Check tabs counted by each test control.
            TabTestState tabTest = _childA.RecordedTabTestState;
            if (tabTest.RecordedTabStops == 0)
            {
                testPassed = false;
                CoreLogger.LogStatus(_childA + " RecordedTabStops: " + tabTest.RecordedTabStops + " Expected: 1",
                                    ConsoleColor.Red);
            }

            if (!testPassed)
            {
                CoreLogger.LogStatus(_childA + " failed to take tab as normal input", ConsoleColor.Red);
            }

            return testPassed;
        }

        private void ResetAllTestStates()
        {
            foreach (IKeyboardInteropTestControl testControl in _testControls)
            {
                testControl.ResetTestState();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool ReportResult()
        {
            bool bResult = true;

            CoreLogger.LogStatus("********************Failures Report :********************", ConsoleColor.Green);
            foreach (ActionInfo ai in _actionInfoList)
            {
                if (ai.Action == "LoadHwnds")
                {
                    CoreLogger.LogStatus("*     "+ai.Param);
                    continue;
                }
                if (!ai.Result)
                {
                    CoreLogger.LogStatus("*     Action: " + ai.Action + " Type: " + ai.Param + " failed");
                    bResult = false;
                }
            }
            if (bResult)
                CoreLogger.LogStatus("*     No failures     *", ConsoleColor.Green);
            CoreLogger.LogStatus("*******************Done Failures Report :********************", ConsoleColor.Green);

            return bResult;
        }
    }
}

