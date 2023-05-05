// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using System.Globalization;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// Misc coverage for bug where content control does not display language correctly
    /// </description>
    /// <relatedBugs>
    
    /// </relatedBugs>
    /// </summary>
    [Test(1, "Regressions.Part1", "ContentControlLanguage")]
    public class ContentControlLanguage : XamlTest
    {
        #region Private Data

        private Label _myLabel;
        private DateTime _dateTime;
        private string _expectedOutput;
        private string _actualOutput;

        #endregion

        #region Constructors

        public ContentControlLanguage()
            : base(@"ContentControlLanguage.xaml")
        {
            InitializeSteps += new TestStep(Validate);            
        }

        #endregion

        #region Private Members

        private TestResult Validate()
        {
            WaitForPriority(DispatcherPriority.Background);

            // Generate actual string
            _myLabel = (Label)RootElement.FindName("myLabel");
            Border border = (Border)VisualTreeHelper.GetChild(_myLabel, 0);
            ContentPresenter cp = (ContentPresenter)VisualTreeHelper.GetChild(border, 0);
            TextBlock tb = (TextBlock) VisualTreeHelper.GetChild(cp, 0);
            _actualOutput = tb.Text;

            // Generate expected string
            _dateTime = DateTime.Now;
            _expectedOutput = _dateTime.ToString(CultureInfo.CreateSpecificCulture("de-de"));
            _expectedOutput = _expectedOutput.Substring(0, _actualOutput.Length);

            // Compare the strings
            if (_actualOutput.CompareTo(_expectedOutput) != 0)
            {
                LogComment("The actual date format is not the same as the expected format.");
                LogComment("Expected: " + _expectedOutput);
                LogComment("Actual:   " + _actualOutput);

                return TestResult.Fail;
            }
            
            LogComment("Date Format is correct.");
            return TestResult.Pass;
        }        

        #endregion

    }
}
