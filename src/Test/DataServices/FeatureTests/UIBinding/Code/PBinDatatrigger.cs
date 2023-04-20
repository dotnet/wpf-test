// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Media;
using Microsoft.Test;
using Microsoft.Test.DataServices;
using System.Collections.ObjectModel;
using System.Windows.Documents;
using System.Globalization;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// Testing PriorityBinding inside the DataTrigger 
    ///    and MultiDataTrigger.
    /// </description>
    /// </summary>

    [Test(1, "Binding", "PBinDataTrigger")]
    public class PBinDataTrigger : XamlTest
    {
        private TextBox _tb;
        private Style _textBoxStylewithPB;

        public PBinDataTrigger()
            : base(@"PBinDatatrigger.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(PBinDatatrigger);
        }

        private TestResult Setup()
        {
            Status("Setup");

            WaitForPriority(System.Windows.Threading.DispatcherPriority.ApplicationIdle);
            //Getting all the styles.
            _textBoxStylewithPB = RootElement.Resources["TextBoxStylewithPB"] as Style;

            if (_textBoxStylewithPB == null)
            {
                LogComment("Fail - Unable to reference Style TextBoxStylewithPB");
                return TestResult.Fail;
            }
            // Getting the TextBox.
            _tb = LogicalTreeHelper.FindLogicalNode(RootElement, "tb") as TextBox;
            if (_tb == null)
            {
                LogComment("Fail - Not able to reference TextBox tb");
                return TestResult.Fail;
            }
            return TestResult.Pass;
        }
        private TestResult PBinDatatrigger()
        {
            WaitForPriority(System.Windows.Threading.DispatcherPriority.ApplicationIdle);
            //Call verify function.
            return VerifyListboxItemcolor();
        }

        private TestResult VerifyListboxItemcolor()
        {
            TestResult result = WaitForSignal(10000);
            WaitForPriority(System.Windows.Threading.DispatcherPriority.ApplicationIdle);
            Status("Check the BackGroundcolor of the TextBox");
            // See if the TextBox color is Brown
            if ((_tb.Background) == ((Brushes.Brown)))
            {
                LogComment("TextBox Background color is brown" + _tb.Background.ToString());
                return TestResult.Pass;
            }
            else
            {
                LogComment("TextBox Background color is not brown" + _tb.Background.ToString());
                return TestResult.Fail;
            }
        }
    }
}


