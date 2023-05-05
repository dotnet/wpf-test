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
using Microsoft.Test.DataServices;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
	/// <summary>
	/// <description>
	/// Binds the same XmlDataProvider source to 2 different properties of TextBox.
	/// </description>
	/// </summary>
    [Test(3, "Binding", "ComplexPropTest")]
	public class ComplexPropTest : XamlTest
    {
        private TextBlock _testText;

        public ComplexPropTest() : base(@"complexProp.xaml")
        {
            InitializeSteps += new TestStep(SetUp);
            RunSteps += new TestStep(Verify);
        }

        private TestResult SetUp()
        {
            _testText = (TextBlock)Util.FindElement(((Canvas)RootElement), "testText");
            if (_testText == null)
            {
                LogComment("Unable to find TextBlock element");
                return TestResult.Fail;
            }
            
            return TestResult.Pass;
        }

        private TestResult Verify()
        {
            Status("Verification of values");
            
            WaitForPriority(DispatcherPriority.SystemIdle);
            
            TestResult result = TestResult.Pass;

            // Needed for slower machines (like VMs with low memory)
            int retryCount = 0;
            while (retryCount < 5 && (_testText.Text != "50"))
            {
                retryCount++;
                LogComment("Pausing then retrying (slow machine)... #" + retryCount + "/5");
                Thread.Sleep(1000);
                WaitForPriority(DispatcherPriority.SystemIdle);
            }

            if (_testText.Text != "50")
            {
                LogComment("Text was \'" + _testText.Text + "\'  expected: \'50\'");
                result = TestResult.Fail;
            }
			// notice that Top is now a double, not a Lenght or an int
            if(!Util.CompareObjects(50.0, _testText.GetValue(Canvas.TopProperty)))
            {
                LogComment("Canvas.TopProperty was \'" + _testText.GetValue(Canvas.TopProperty).ToString() + "\' expected: \'" + 50.ToString() + "\'");
                result = TestResult.Fail;
            }
            LogComment("Verification completed");
            return result;
        }
    }
}

