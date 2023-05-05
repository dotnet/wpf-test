// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/* ***************  Storyboard Methods Testing *****************
*     Description:
*          Tests using parameter-based and parameterless Storyboard methods.
*     Pass Conditions:
*          The test case will Pass if GetCurrentValue returns the correct value for each Animation.
*     Note:
*          For details of the new feature being introduced in .Net3.5-SP1, refer to the test spec
*          located at:  Test\Animation\Specifications\Storyboard Parameterless Methods Test Spec.docx
*
*     Framework:          A CLR executable is created.
*     Area:               Animation/Timing
*     Dev Owner:          Microsoft
*     Dependencies:       TestRuntime.dll
*     Support Files:
* *******************************************************/
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Globalization;
using System.Threading;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Media.Animation;

using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Animation
{
    /// <summary>
    /// <area>Storyboards.LowLevelScenarios.MixedMethods</area>
    /// <priority>2</priority>
    /// <description>
    /// Verifying mixing old and new Begin methods on a Storyboard.
    /// </description>
    /// </summary>
    [Test(2, "Storyboards.LowLevelScenarios", "MixedMethodsTest", Keywords = "Localization_Suite")]
    public class MixedMethodsTest : WindowTest
    {
        #region Test case members

        private string                  _inputString;
        private Button                  _button1;
        private Storyboard              _storyboard;
        private DependencyProperty      _dp;
        private DependencyObject        _dobj;
        
        #endregion


        #region Constructor

        [Variation("TargetNameBeginNoParm")]
        [Variation("TargetBeginNoParm")]
        [Variation("TargetNameBeginWithDO")]
        [Variation("TargetNameTargetBeginNoParm")]
        [Variation("TargetTargetNameBegin")]
        [Variation("TargetNameTargetBeginWithDO")]
        [Variation("TargetTargetNameBeginWithDO")]

        /******************************************************************************
        * Function:          MixedMethodsTest Constructor
        ******************************************************************************/
        public MixedMethodsTest(string variation)
        {
            _inputString = variation;
            RunSteps += new TestStep(CreateTree);
            RunSteps += new TestStep(TestExceptions);
        }

        #endregion

        #region Test Steps

        /******************************************************************************
        * Function:          CreateTree
        ******************************************************************************/
        /// <summary>
        /// Creates the window content.
        /// </summary>
        /// <returns>Returns TestResult.Pass</returns>
        TestResult CreateTree()
        {
            Window.Width                = 400;
            Window.Height               = 200;
            Window.Title                = "Storyboard Methods Testing";
            Window.Left                 = 0;
            Window.Top                  = 0;
            Window.Topmost              = true;

            NameScope.SetNameScope(Window, new NameScope());

            Canvas body = new Canvas();
            body.Background          = Brushes.BurlyWood;

            _button1 = new Button();
            _button1.Width               = 150d;
            _button1.Height              = 75;
            _button1.Background          = Brushes.MintCream;
            _button1.Content             = "WPF!";
            _button1.Name                = "button";

            body.Children.Add(_button1);
            Window.RegisterName(_button1.Name, _button1);

            _dp = Button.WidthProperty;
            _dobj = _button1;

            Window.Content = body;

            return TestResult.Pass;
        }

        /******************************************************************************
        * Function:          TestExceptions
        ******************************************************************************/
        /// <summary>
        /// TestExceptions: Verify the correct Exception is thrown when old and new methods
        /// are mixed.
        /// </summary>
        TestResult TestExceptions()
        {
            GlobalLog.LogStatus("---TestExceptions---");

            //------------------- CREATE ANIMATION------------------------------
            DoubleAnimation animDouble = new DoubleAnimation();
            animDouble.By               = -100;
            animDouble.BeginTime        = TimeSpan.FromMilliseconds(0d);
            animDouble.Duration         = new Duration(TimeSpan.FromMilliseconds(1000d));
            animDouble.FillBehavior     = FillBehavior.HoldEnd;

            //------------------- CREATE STORYBOARD-----------------------------
            GlobalLog.LogStatus("----Create Storyboard----");
            _storyboard = new Storyboard();
            _storyboard.Name = "story";

            _storyboard.Children.Add(animDouble);
            PropertyPath expPath = new PropertyPath( "(0)", _dp );
            Storyboard.SetTargetProperty(_storyboard, expPath);

            switch (_inputString)
            {
                case "TargetNameBeginNoParm":
                    GlobalLog.LogStatus("---Scenario #1: SetTargetName / Begin()");
                    ExceptionHelper.ExpectException<InvalidOperationException>
                    (
                        delegate()
                        {
                            Storyboard.SetTargetName(_storyboard, _button1.Name);
                            _storyboard.Begin();
                        },
                        delegate(InvalidOperationException e1) { ;}
                    );
                    break;

                case "NoTargetBeginNoParm":
                    GlobalLog.LogStatus("---Scenario #2: No SetTarget / Begin()");
                    ExceptionHelper.ExpectException<InvalidOperationException>
                    (
                        delegate()
                        {
                            //In this case, no SetTarget is invoked.
                            _storyboard.Begin();
                        },
                        delegate(InvalidOperationException e2) { ;}
                    );
                    break;

                case "NoTargetNameBeginWithDO":
                    GlobalLog.LogStatus("---Scenario #3: No SetTargetName / Begin(do)");
                    //No exception should be thrown in this case.
                    _storyboard.Begin(_button1);
                    break;

                case "TargetNameTargetBeginNoParm":
                    GlobalLog.LogStatus("---Scenario #4: SetTargetName then SetTarget / Begin()");
                    //No exception should be thrown in this case.
                    Storyboard.SetTargetName(_storyboard, _button1.Name);
                    Storyboard.SetTarget(_storyboard, _button1);
                    _storyboard.Begin();
                    break;

                case "TargetTargetNameBegin":
                    GlobalLog.LogStatus("---Scenario #5: SetTarget then SetTargetName / Begin()");
                    //No exception should be thrown in this case.
                    Storyboard.SetTarget(_storyboard, _button1);
                    Storyboard.SetTargetName(_storyboard, _button1.Name);
                    _storyboard.Begin();
                    break;

                case "TargetNameTargetBeginWithDO":
                    GlobalLog.LogStatus("---Scenario #6: SetTargetName then SetTarget / Begin(do)");
                    //No exception should be thrown in this case.
                    Storyboard.SetTargetName(_storyboard, _button1.Name);
                    Storyboard.SetTarget(_storyboard, _button1);
                    _storyboard.Begin(_button1);
                    break;

                case "TargetTargetNameBeginWithDO":
                    GlobalLog.LogStatus("---Scenario #7: SetTarget then SetTargetName / Begin(do)");
                    //No exception should be thrown in this case.
                    Storyboard.SetTarget(_storyboard, _button1);
                    Storyboard.SetTargetName(_storyboard, _button1.Name);
                    _storyboard.Begin(_button1);
                    break;
            }
            
            //If the expected Exception is found or if no Exception is expected, the test case passes.
            return TestResult.Pass;
        }

        #endregion
    }
}
