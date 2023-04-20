// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Threading; using System.Windows.Threading;

using Microsoft.Test;
using System.Windows.Controls;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Windows.Navigation;
using System.Windows.Media;
using System.ComponentModel;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
	/// <summary>
	/// <description>
	///	TODO
	/// </description>
	/// </summary>
    [Test(0, "Binding", "SameTypeTest")]
    public class SameTypeTest : XamlTest
    {
        TextBlock _sourceText;

        TextBox _targetText;

        Canvas _sourceCanvas;

        Canvas _targetCanvas;

        SolidColorBrush _purpleBrush;

        public SameTypeTest() : base(@"SameType.xaml")
        {
            InitializeSteps += new TestStep(SetUp);
            RunSteps += new TestStep(InitialVerify);
            RunSteps += new TestStep(UpdateSource);
            RunSteps += new TestStep(UpdateTarget);
        }

        private TestResult SetUp()
        {
            Status("Referencing to test elements");

            Color purpleColor = new Color();

            purpleColor.A = 255;
            purpleColor.B = 255;
            purpleColor.R = 255;
            purpleColor.G = 0;
            _purpleBrush = new SolidColorBrush(purpleColor);
            _sourceText = (TextBlock)Util.FindElement(RootElement, "sourceText");
            _targetText = (TextBox)Util.FindElement(RootElement, "targetText");
            _sourceCanvas = (Canvas)Util.FindElement(RootElement, "sourceCanvas");
            _targetCanvas = (Canvas)Util.FindElement(RootElement, "targetCanvas");
            if (_sourceText == null)
            {
                LogComment("Unable to reference sourceText element.");
                return TestResult.Fail;
            }

            if (_targetText == null)
            {
                LogComment("Unable to reference targetText element.");
                return TestResult.Fail;
            }

            if (_sourceCanvas == null)
            {
                LogComment("Unable to reference sourceCanvas element.");
                return TestResult.Fail;
            }

            if (_targetCanvas == null)
            {
                LogComment("Unable to reference targetCanvas element.");
                return TestResult.Fail;
            }

            LogComment("Referenced all test elements successfully");
            return TestResult.Pass;
        }

        private TestResult InitialVerify()
        {
            WaitForPriority(DispatcherPriority.Background);
            return Verify("Initial Verify");
        }

        private TestResult UpdateSource()
        {
            _sourceText.Text = "Updating source text";
            _sourceCanvas.SetValue(Canvas.LeftProperty, 400d);
            _sourceText.Foreground = _purpleBrush;
            WaitForPriority(DispatcherPriority.Background);
            return Verify("UpdateSource Verify");
        }

        private TestResult UpdateTarget()
        {
            _targetText.Text = "Updating target text";
            
            WaitForPriority(DispatcherPriority.Background);
            return Verify("UpdateTarget Verify");
        }

        private TestResult Verify(string StepName)
        {
            bool passed = true;

            if (!Util.CompareObjects(_sourceText.Text, _targetText.Text))
            {
                passed = false;
                LogComment("sourceText.Text was \'" + _sourceText.Text + "\' targetText.Text was \'" + _targetText.Text + "\' they should be the same");
            }

            if (!Util.CompareObjects(_sourceCanvas.Background, _sourceText.Foreground))
            {
                passed = false;
                LogComment("sourceCanvas.Background does not have the correct color, it should match the sourceText.Foreground");
            }

            if (!Util.CompareObjects(_targetCanvas.Background, _targetText.Foreground))
            {
                passed = false;
                LogComment("targetCanvas.Background does not have the correct color, it should match the targetText.Foreground");
            }

            if (!Util.CompareObjects(_targetCanvas.GetValue(Canvas.TopProperty), _sourceCanvas.GetValue(Canvas.LeftProperty)))
            {
                passed = false;
                LogComment("targetCanvas.Top is " + _targetCanvas.GetValue(Canvas.TopProperty).ToString() + " expected to be " + _sourceCanvas.GetValue(Canvas.LeftProperty).ToString());
            }

            if (!Util.CompareObjects(_sourceCanvas.GetValue(Canvas.TopProperty), _targetCanvas.GetValue(Canvas.LeftProperty)))
            {
                passed = false;
                LogComment("sourceCanvas.Top is " + _sourceCanvas.GetValue(Canvas.TopProperty).ToString() + " expected to be " + _targetCanvas.GetValue(Canvas.LeftProperty).ToString());
            }

            if (passed)
            {
                LogComment("Values of the properties for the elements were as expected for " + StepName);
                return TestResult.Pass;
            }
            else
            {
                return TestResult.Fail;
            }
        }
    }
}

