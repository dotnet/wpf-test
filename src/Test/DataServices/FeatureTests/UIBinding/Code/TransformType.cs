// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Threading; 
using System.Windows.Threading;
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
	/// TODO
	/// </description>
	/// </summary>
    [Test(0, "Binding", "TransformTypeTest")]
    public class TransformTypeTest : XamlTest
    {
        TextBox _targetText;

        Canvas _sourceCanvas;

        SolidColorBrush _purpleBrush;

        public TransformTypeTest() : base(@"TransformType.xaml")
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
            _targetText = (TextBox)Util.FindElement(RootElement, "targetText");
            _sourceCanvas = (Canvas)Util.FindElement(RootElement, "sourceCanvas");
            if (_sourceCanvas == null)
            {
                LogComment("Unable to reference sourceCanvas element.");
                return TestResult.Fail;
            }

            if (_targetText == null)
            {
                LogComment("Unable to reference targetText element.");
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
            _sourceCanvas.Background = _purpleBrush;
            WaitForPriority(DispatcherPriority.Background);
            return Verify("UpdateSource Verify");
        }

        private TestResult UpdateTarget()
        {
            _targetText.Text = "#FF00FF00";
            WaitForPriority(DispatcherPriority.Background);
            return Verify("UpdateTarget Verify");
        }

        private TestResult Verify(string StepName)
        {
            bool passed = true;
            SolidColorBrush brush = _sourceCanvas.Background as SolidColorBrush;

            if (brush == null)
            {
                LogComment("Brush from sourceCanvas.Background was null");
                return TestResult.Fail;
            }

            Color color = brush.Color;

            if (_targetText.Text != color.ToString())
            {
                LogComment("targetText.Text was " + _targetText.Text + ", expected " + color.ToString());
                passed = false;
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

