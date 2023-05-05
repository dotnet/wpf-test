// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using System.ComponentModel;
using System.Globalization;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Documents;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>





    [Test(1, "Regressions.Part1", "DefaultValidationAdornerSize")]
    public class DefaultValidationAdornerSize : XamlTest
    {
        #region Private Data

        private Places _places;
        private StackPanel _stackPanel;
        private Button _myButton;
        private ListBox _listBox;
        private TextBox _textBox;
        private double _actualWidth;

        #endregion

        #region Constructors

        public DefaultValidationAdornerSize()
            : base(@"DefaultValidationAdornerSize.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(Validate);
        }

        #endregion

        #region Private Members

        private TestResult Setup()
        {
            _stackPanel = (StackPanel)RootElement.FindName("stackPanel");
            _listBox = (ListBox)RootElement.FindName("listBox");
            _myButton = (Button)RootElement.FindName("myButton");
            _places = new Places();

            _listBox.ItemsSource = _places;

            return TestResult.Pass;
        }

        private TestResult Validate()
        {
            WaitForPriority(DispatcherPriority.Background);
			
            // Get to Text field and set focus            
            _textBox = FindTextBox(_listBox);
            _textBox.Focus();

            // Enter invalid Data into field
            _textBox.Text = "Toronto";

            // Focus away to generate TemplatedAdorner
            _myButton.Focus();
            
            WaitForPriority(DispatcherPriority.Background);
            
            // Get the TemplatedAdorner width via Visual Tree
            _actualWidth = GetTemplatedAdornerWidth(_listBox);            
            
            // Verify Adorner is max allowable (also adjust for padding in differnt themes)
            if (_actualWidth < (_textBox.Width - 10))
            {
                LogComment("Default Adorner Layer width is set to not max possible.");                
                LogComment("Expected Value " + _textBox.Width.ToString());
                LogComment("Actual Value " + _actualWidth.ToString());
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        #endregion


        #region Helper Methods
        
        // Find the Text box given the listbox
        private TextBox FindTextBox(ListBox listBox)
        {            
            ListBoxItem lbi = (ListBoxItem)listBox.ItemContainerGenerator.ContainerFromIndex(0);
            ContentPresenter cp = (ContentPresenter)Util.FindVisualByType(typeof(ContentPresenter), lbi, false);
            return (TextBox)VisualTreeHelper.GetChild(cp, 0);            
        }

        // Find the Adorner Layer and return ActualWidth
        private double GetTemplatedAdornerWidth(ListBox listBox)
        {
            ScrollViewer s = (ScrollViewer)Util.FindVisualByType(typeof(ScrollViewer), listBox, false);
            Grid g = (Grid)VisualTreeHelper.GetChild(s, 0);
            ScrollContentPresenter scp = (ScrollContentPresenter)VisualTreeHelper.GetChild(g, 1);
            AdornerLayer al = (AdornerLayer)VisualTreeHelper.GetChild(scp, 1);
            
            //Since Templated Adroner is an internal type, cast it to Framework Element
            FrameworkElement templatedAdorner = (FrameworkElement) VisualTreeHelper.GetChild(al, 0);

            return templatedAdorner.ActualWidth;
        }

        #endregion

    }


    #region Helper Classes
       
    public class MyValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string name = value as string;

            if (name == "Toronto")
            {
                return new ValidationResult(false, "Bogus name");
            }
            else
            {
                return ValidationResult.ValidResult;
            }
        }
    }

    #endregion

}
