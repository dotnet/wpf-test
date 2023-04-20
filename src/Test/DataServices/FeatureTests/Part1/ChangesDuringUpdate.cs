// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.Test.DataServices
{

    [Test(1, "Regressions.Part1", "ChangesDuringUpdate")]
    public class ChangesDuringUpdate : XamlTest
    {
        #region Private Data

        private TextBox _textBox;
        private Button _button;

        #endregion

        #region Constructors

        public ChangesDuringUpdate()
            : base(@"ChangesDuringUpdate.xaml")
        {
            InitializeSteps += new TestStep(Setup);            
            RunSteps += new TestStep(Validate);
        }

        #endregion

        #region Private Members

        private TestResult Setup()
        {
            _textBox = RootElement.FindName("textBox") as TextBox;
            _button = RootElement.FindName("button") as Button;                   

            return TestResult.Pass;
        }
                
        private TestResult Validate()
        {
            // add lower case text to text box
            _textBox.Text += " some small case text.";

            // change focus
            _button.Focus();

            // Verify if text is now Upper Case
            if (_textBox.Text.CompareTo(_textBox.Text.ToUpper()) != 0)
            {
                LogComment("Binding value not updated. The Text was: " + _textBox.Text);
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        #endregion
    }

    #region Helper Class

    public class CustomConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ConvertBack(value, targetType, parameter, culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            String stringValue = value as String;
            return culture.TextInfo.ToUpper(stringValue);
        }

    }

    #endregion

}
