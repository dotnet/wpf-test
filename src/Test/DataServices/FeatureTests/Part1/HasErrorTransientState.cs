// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using System.ComponentModel;
using System.Windows;
using System;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    ///  Regression coverage for bug where Validation.HasError passes through transient 'false' state when Binding.ValidatesOnDataErrors=true
    /// </description>
    /// <relatedBugs>
    
    /// </relatedBugs>
    /// </summary>
    [Test(1, "Regressions.Part1", "HasErrorTransientState")]
    public class HasErrorTransientState : XamlTest
    {
        #region Private Data

        private StackPanel _myStackPanel;
        
        #endregion

        #region Constructors

        public HasErrorTransientState()
            : base(@"HasErrorTransientState.xaml")
        {
            InitializeSteps += new TestStep(Setup);            
            RunSteps += new TestStep(Validate);
        }

        #endregion

        #region Private Members

        private TestResult Setup()
        {
            _myStackPanel = (StackPanel) RootElement.FindName("myStackPanel");
            
            if (_myStackPanel == null)
            {
                LogComment("Failed to load Xaml correctly");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }
                
        private TestResult Validate()
        {
            WaitForPriority(DispatcherPriority.Render);         
            
            // Do some actions            

            // Verify 
            //if ()
            //{
            //    LogComment(" " + );
            //    return TestResult.Fail;
            //}

            return TestResult.Pass;
        }

        #endregion
        
    }
    
    #region Helper Classes
    public class MyBusinessObject : IDataErrorInfo, INotifyPropertyChanged
    {
        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler eh = this.PropertyChanged;

            if (null != eh)
            {
                eh(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion INotifyPropertyChanged

        #region IDataErrorInfo

        public string Error
        {
            get { return null; }
        }

        public string this[string columnName]
        {
            get { return this.Validate(columnName); }
        }

        protected string Validate(string columnName)
        {
            //Debug.Assert(columnName.Equals("Value"));

            MessageBox.Show("Observe the current state...");

            if (String.IsNullOrEmpty(this.Value))
            {
                return String.Empty;
            }
            if (this.Error != null) { MessageBox.Show("yo"); return "blue"; }

            return (this.Value.Length > 2) ? "Value is bad." : String.Empty;
        }

        #endregion IDataErrorInfo

        #region Properties

        private string _value;
        public string Value
        {
            get
            {
                return this._value;
            }

            set
            {
                this._value = value;
                this.NotifyPropertyChanged("Value");
            }
        }

        #endregion Properties
    }
    #endregion
}
