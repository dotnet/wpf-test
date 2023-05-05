// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using System.ComponentModel;
using System.Globalization;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    ///  Regression coverage for bug where Binding converter is always handed initial XmlLanguage
    /// </description>
    /// </summary>
    [Test(1, "Regressions.Part1", "BindingConverterXmlLanguage")]
    public class BindingConverterXmlLanguage : XamlTest
    {
        #region Private Data

        private StackPanel _myStackPanel;
        private TextBox _myTextBox;
        
        #endregion

        #region Constructors

        public BindingConverterXmlLanguage()
            : base(@"BindingConverterXmlLanguage.xaml")
        {
            InitializeSteps += new TestStep(Setup);            
            RunSteps += new TestStep(Validate);
        }

        #endregion

        #region Private Members

        private TestResult Setup()
        {
            _myStackPanel = (StackPanel) RootElement.FindName("myStackPanel");
            _myTextBox = (TextBox)RootElement.FindName("myTextBox");

            if (_myStackPanel == null || _myTextBox == null)
            {
                LogComment("Failed to load Xaml correctly");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }
                
        private TestResult Validate()
        {
            WaitForPriority(DispatcherPriority.Render);            
            
            // Change the language on the window.
            this.Window.Language = System.Windows.Markup.XmlLanguage.GetLanguage("de-DE");

            WaitForPriority(DispatcherPriority.DataBind);            

            // Verify if binding was updated.
            if (_myTextBox.Text != "de-DE")
            {
                LogComment("Failed to convert language correctly on binding.");
                LogComment(_myTextBox.Text);
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        #endregion
        
    }
    
    #region Helper Classes

    public class SimpleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return culture.Name;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }

    #endregion
}
