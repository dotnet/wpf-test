// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    ///  Regression coverage for bug where TabControl gets out of sync with data (when two TabControls are bound to the same data, and selected item is removed).
    /// </description>
    /// </summary>
    [Test(1, "Regressions.Part1", "TabControlSync")]
    public class TabControlSync : XamlTest
    {
        #region Private Data

        private StackPanel _myStackPanel;
        
        #endregion

        #region Constructors

        public TabControlSync()
            : base(@"TabControlSync.xaml")
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

    public class MyCustomCollection : ObservableCollection<String>
    {
        public MyCustomCollection()
        {
            Add("First");
            Add("Second");
            Add("Third");
        }

        public string SelectedElement
        {
            get { return _selectedElement; }
            set
            {
                _selectedElement = value;
                OnPropertyChanged(new PropertyChangedEventArgs("SelectedElement"));
            }
        }
        string _selectedElement;
    }

    #endregion
}
