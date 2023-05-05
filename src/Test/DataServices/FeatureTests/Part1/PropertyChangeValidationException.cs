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
    ///  Regression coverage for bug where Validation exception is different depending on property change notifications
    /// </description>
    /// <relatedBugs>
    
    /// </relatedBugs>
    /// </summary>
    [Test(1, "Regressions.Part1", "PropertyChangeValidationException")]
    public class PropertyChangeValidationException : XamlTest
    {
        #region Private Data

        private StackPanel _myStackPanel;
        private TextBox _myTextBox;
        private string _actualOutput;
        private string _expectedOutput;
        
        #endregion

        #region Constructors

        public PropertyChangeValidationException()
            : base(@"PropertyChangeValidationException.xaml")
        {
            InitializeSteps += new TestStep(Setup);            
            RunSteps += new TestStep(Validate);
        }

        #endregion

        #region Private Members

        private TestResult Setup()
        {
            _myStackPanel = (StackPanel)RootElement.FindName("myStackPanel");
            _myTextBox = (TextBox)RootElement.FindName("myTextBox");

            if (_myTextBox == null || _myStackPanel == null)
            {
                LogComment("Failed to load Xaml correctly");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }
                
        private TestResult Validate()
        {
            WaitForPriority(DispatcherPriority.Background);

            // Cause validation error, by entering a negative number.
            _myTextBox.Focus();
            _myTextBox.Text = "-23";
            _myStackPanel.Focus();
            
            _actualOutput = _myTextBox.ToolTip.ToString();
            _expectedOutput = "ArgumentException!";

            // Verify 
            if (_actualOutput != _expectedOutput)
            {
                LogComment("Content does not match Expected Value");
                LogComment("Actual: " + _actualOutput);
                LogComment("Expected: " + _expectedOutput);
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        #endregion
        
    }
    
    #region Helper Classes

    public class MySource : INotifyPropertyChanged
    {
        private int _myValue;

        public int MyValue
        {
            get { return _myValue; }
            set
            {

                if (value < 0)
                {
                    throw new ArgumentException("ArgumentException!");
                }
                _myValue = value;
                OnPropertyChanged("MyValue");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

    }


    #endregion
}