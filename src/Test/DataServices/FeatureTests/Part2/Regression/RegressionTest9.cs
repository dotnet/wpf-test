// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// Regression Test - TwoWay Binding of System.Windows.Controls.DataGrid.CurrentCell is not possible
    /// </description>
    /// </summary>
    [Test(0, "Regression", "RegressionTest9")]
    public class RegressionTest9 : XamlTest
    {
        private TabControl _tabcontrol;

        public RegressionTest9()
            : base(@"RegressionTest9.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(VerifyBinding);
        }

        private TestResult Setup()
        {
            Status("Setup");
            this.Window.DataContext = new MyViewModel();

            _tabcontrol = (TabControl)Util.FindElement(RootElement, "MytabControl");
            Assert.IsNotNull(_tabcontrol, "Can't find TabControl!");

            WaitForPriority(DispatcherPriority.SystemIdle);
            return TestResult.Pass;
        }

        private TestResult VerifyBinding()
        {
            Status("Show an empty DataGrid to verify TwoWay Binding");

            try
            {
                LogComment("Select the second tab to show an empty DataGrid");
                _tabcontrol.SelectedIndex = 1;
                WaitForPriority(DispatcherPriority.SystemIdle);
            }
            catch (NullReferenceException nrException)
            {
                LogComment("Verify failed! hit NullReferenceException, this may caused by regression bug:\n");
                LogComment(nrException.ToString());
                return TestResult.Fail;
            }
            catch (Exception exception)
            {
                LogComment("Verify failed! But bit by this regression, Exception info:\n");
                LogComment(exception.ToString());
                return TestResult.Fail;
            }

            LogComment("Verify TwoWay binding of DataGrid.CurrentCell succeed!");
            return TestResult.Pass;
        }
    }

    #region Helper class

    public class MyViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private DataGridCellInfo _currentCell;

        public DataGridCellInfo CurrentCell
        {
            get
            {
                return _currentCell;
            }
            set
            {
                if (value == _currentCell) return;
                _currentCell = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("CurrentCell"));
                }
            }
        }
    }

    #endregion
}
