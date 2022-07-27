// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Automation;
using System.Windows.Threading;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.Reflection;
using System.ComponentModel;
using MS.Internal; // PointUtil

namespace DRT
{
    public class ProgressBarSuite : DrtTestSuite
    {
        public ProgressBarSuite()
            : base("ProgressBar")
        {
            Contact = "Microsoft";
        }

        ProgressBar _default;

        ProgressBar _horizontalProgressBar;
        ProgressBar _horizontalProgressBarIndeterminate;
        ProgressBar _horizontalProgressBarStyled;

        ProgressBar _verticalProgressBar;
        ProgressBar _verticalProgressBarIndeterminate;
        ProgressBar _verticalProgressBarStyled;

        // Parts from the style 
        FrameworkElement _horizontalTrack;
        FrameworkElement _horizontalIndicator;
        FrameworkElement _verticalTrack;
        FrameworkElement _verticalIndicator;

        #region Window setup and common hooks

        public override DrtTest[] PrepareTests()
        {
            Keyboard.Focus(null);

            DRT.LoadXamlFile("DrtProgressBar.xaml");
            DRT.Show(DRT.RootElement);

            _horizontalProgressBar = DRT.FindElementByID("HPB") as ProgressBar;
            _horizontalProgressBarIndeterminate = DRT.FindElementByID("HPBI") as ProgressBar;
            _horizontalProgressBarStyled = DRT.FindElementByID("HPBS") as ProgressBar;

            _verticalProgressBar = DRT.FindElementByID("VPB") as ProgressBar;
            _verticalProgressBarIndeterminate = DRT.FindElementByID("VPBI") as ProgressBar;
            _verticalProgressBarStyled = DRT.FindElementByID("VPBS") as ProgressBar;

            _horizontalTrack = DRT.FindElementByID("PART_Track", _horizontalProgressBarStyled) as FrameworkElement;
            _horizontalIndicator = DRT.FindElementByID("PART_Indicator", _horizontalProgressBarStyled) as FrameworkElement;

            _verticalTrack = DRT.FindElementByID("PART_Track", _verticalProgressBarStyled) as FrameworkElement;
            _verticalIndicator = DRT.FindElementByID("PART_Indicator", _verticalProgressBarStyled) as FrameworkElement;
            
            _default = new ProgressBar();

            List<DrtTest> tests = new List<DrtTest>();
            if (!DRT.KeepAlive)
            {
                tests.Add(new DrtTest(BasicTest));
                tests.Add(new DrtTest(PartsTest));
            }
            return tests.ToArray();
        }

        #endregion

        private bool AreClose(double a, double b)
        {
            return Math.Abs(a - b) < 1e-5;
        }

       
        #region BasicTest

        public void BasicTest()
        {
            if (DRT.Verbose) Console.WriteLine("\n---ProgressBar Basic Tests");
            
            // Test the default values for ProgressBar
            DRT.Assert(_default.Orientation == System.Windows.Controls.Orientation.Horizontal, "ProgressBar orientation should be Horizontal by default");
            DRT.Assert(_default.IsIndeterminate == false, "ProgressBar.IsIndeterminate should be false by default");
            DRT.Assert(_default.Minimum == 0.0, "ProgressBar.Minimum should be 0.0 by default");
            DRT.Assert(_default.Maximum == 100.0, "ProgressBar.Maximum should be 100.0 by default");
            DRT.Assert(_default.Value == 0.0, "ProgressBar.Value should be 0.0 by default");

            // Test setting orientation
            _default.Orientation = System.Windows.Controls.Orientation.Vertical;
            DRT.Assert(_default.Orientation == System.Windows.Controls.Orientation.Vertical, "ProgressBar orientation should be Vertical after setting property");
            _default.Orientation = System.Windows.Controls.Orientation.Horizontal;
            DRT.Assert(_default.Orientation == System.Windows.Controls.Orientation.Horizontal, "ProgressBar orientation should be Horizontal after setting property");

            // Test setting isIndeterminate
            _default.IsIndeterminate = true;
            DRT.Assert(_default.IsIndeterminate == true, "ProgressBar IsIndeterminate should be true after setting property");
            _default.IsIndeterminate = false;
            DRT.Assert(_default.IsIndeterminate == false, "ProgressBar IsIndeterminate should be false after setting property");

            DRT.ResumeAt(new DrtTest(BasicTestProc));

        }

        BasicTestStep _basicTestStep = BasicTestStep.Start;

        enum BasicTestStep
        {
            Start,

            Horizontal10,
            Horizontal30,
            HorizontalMinus10,
            Horizontal110,
            Horizontal100,
            Horizontal80,

            Vertical100,
            Vertical80,
            VerticalMinus30,
            Vertical50,

            End
        }

        // Make sure no exceptions thrown when changing values of ProgressBars
        public void BasicTestProc()
        {
            if (DRT.Verbose) Console.WriteLine("Basic test = " + _basicTestStep);

            switch (_basicTestStep)
            {
                case BasicTestStep.Start:
                    break;

                case BasicTestStep.Horizontal10:
                    _horizontalProgressBar.Value = 10.0;
                    break;

                case BasicTestStep.Horizontal30:
                    _horizontalProgressBar.Value = 30.0;
                    break;

                case BasicTestStep.HorizontalMinus10:
                    _horizontalProgressBar.Value = -10.0;
                    break;

                case BasicTestStep.Horizontal110:
                    _horizontalProgressBar.Value = 110.0;
                    break;

                case BasicTestStep.Horizontal100:
                    _horizontalProgressBar.Value = 100.0;
                    break;
            
                case BasicTestStep.Horizontal80:
                    _horizontalProgressBar.Value = 80.0;
                    break;

                    
                case BasicTestStep.Vertical100:
                    _verticalProgressBar.Value = 100.0;
                    break;

                case BasicTestStep.Vertical80:
                    _verticalProgressBar.Value = 80.0;
                    break;

                case BasicTestStep.VerticalMinus30:
                    _verticalProgressBar.Value = -30.0;
                    break;

                case BasicTestStep.Vertical50:
                    _verticalProgressBar.Value = 50.0;
                    break;
            
                case BasicTestStep.End:
                    break;
            }

            if (_basicTestStep != BasicTestStep.End)
            {
                _basicTestStep++;
                DRT.Pause(10);
                DRT.ResumeAt(new DrtTest(BasicTestProc));
            }
        }



        #endregion

        #region PartsTest

        public void PartsTest()
        {
            if (DRT.Verbose) Console.WriteLine("\n---ProgressBar Parts Tests");

            DRT.ResumeAt(new DrtTest(PartsTestProc));
        }

        PartsTestStep _partsTestStep = PartsTestStep.Start;

        enum PartsTestStep
        {
            Start,
            
            Horizontal0,
            Horizontal50,
            Horizontal75,
            Horizontal100,
            
            Vertical100,
            Vertical125,
            Vertical140,
            Vertical150,
            
            End
        }

        public void PartsTestProc()
        {
            if (DRT.Verbose) Console.WriteLine("Parts test = " + _partsTestStep);

            switch (_partsTestStep)
            {
                case PartsTestStep.Start:
                    break;

                case PartsTestStep.Horizontal0:
                    _horizontalProgressBarStyled.Value = 0;
                    break;

                case PartsTestStep.Horizontal50:
                    _horizontalProgressBarStyled.Value = 50;
                    break;

                case PartsTestStep.Horizontal75:
                    _horizontalProgressBarStyled.Value = 75;
                    break;

                case PartsTestStep.Horizontal100:
                    _horizontalProgressBarStyled.Value = 100;
                    break;


                case PartsTestStep.Vertical100:
                    _verticalProgressBarStyled.Value = 100;
                    break;

                case PartsTestStep.Vertical125:
                    _verticalProgressBarStyled.Value = 125;
                    break;

                case PartsTestStep.Vertical140:
                    _verticalProgressBarStyled.Value = 140;
                    break;

                case PartsTestStep.Vertical150:
                    _verticalProgressBarStyled.Value = 150;
                    break;

                case PartsTestStep.End:
                    break;
            }
            DRT.Pause(10);
            DRT.ResumeAt(new DrtTest(PartsTestVerifyProc));
        }

        private void PartsTestVerifyProc()
        {
            if (DRT.Verbose) Console.WriteLine("Parts test verify = " + _partsTestStep);

            double expected;

            switch (_partsTestStep)
            {
                case PartsTestStep.Start:
                    break;

                case PartsTestStep.Horizontal0:
                    expected = 0.0;
                    DRT.Assert(AreClose(_horizontalIndicator.ActualWidth, expected),
                       "HorizontalIndicator width {0} does not match expected {1}", _horizontalIndicator.ActualWidth, expected);
                    break;

                case PartsTestStep.Horizontal50:
                    expected = 0.5 * _horizontalTrack.ActualWidth;
                    DRT.Assert(AreClose(_horizontalIndicator.ActualWidth, expected),
                       "HorizontalIndicator width {0} does not match expected {1}", _horizontalIndicator.ActualWidth, expected);
                    break;

                case PartsTestStep.Horizontal75:
                    expected = 0.75 * _horizontalTrack.ActualWidth;
                    DRT.Assert(AreClose(_horizontalIndicator.ActualWidth, expected),
                       "HorizontalIndicator width {0} does not match expected {1}", _horizontalIndicator.ActualWidth, expected);
                    break;

                case PartsTestStep.Horizontal100:
                    expected = _horizontalTrack.ActualWidth;
                    DRT.Assert(AreClose(_horizontalIndicator.ActualWidth, expected),
                       "HorizontalIndicator width {0} does not match expected {1}", _horizontalIndicator.ActualWidth, expected);
                    break;


                case PartsTestStep.Vertical100:
                    expected = 0.0;
                    DRT.Assert(AreClose(_verticalIndicator.ActualWidth, expected),
                       "VerticalIndicator height {0} does not match expected {1}", _verticalIndicator.ActualHeight, expected);
                    break;

                case PartsTestStep.Vertical125:
                    expected = 25.0 / 50.0 * _verticalTrack.ActualWidth;
                    DRT.Assert(AreClose(_verticalIndicator.ActualWidth, expected),
                       "VerticalIndicator height {0} does not match expected {1}", _verticalIndicator.ActualHeight, expected);
                    break;

                case PartsTestStep.Vertical140:
                    expected = 40.0 / 50.0 * _verticalTrack.ActualWidth;
                    DRT.Assert(AreClose(_verticalIndicator.ActualWidth, expected),
                       "VerticalIndicator height {0} does not match expected {1}", _verticalIndicator.ActualHeight, expected);
                    break;

                case PartsTestStep.Vertical150:
                    expected = _verticalTrack.ActualWidth;
                    DRT.Assert(AreClose(_verticalIndicator.ActualWidth, expected),
                       "VerticalIndicator height {0} does not match expected {1}", _verticalIndicator.ActualHeight, expected);
                    break;

                case PartsTestStep.End:
                    break;
            }

            if (_partsTestStep != PartsTestStep.End)
            {
                _partsTestStep++;
                DRT.ResumeAt(new DrtTest(PartsTestProc));
            }
        }

        #endregion
    }
}
