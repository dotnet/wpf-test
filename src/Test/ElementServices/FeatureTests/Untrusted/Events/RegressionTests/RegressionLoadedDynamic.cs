// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Globalization;
using System.Reflection;
using System.Security.Permissions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Threading;

namespace Microsoft.Test.ElementServices.PropertyEngine.RegressionTests
{
    /// <summary>
    /// Regression test for 

    [Test(1, @"Events.RegressionTests", TestCaseSecurityLevel.FullTrust, "RegressionLoadedDynamic", SupportFiles=@"FeatureTests\ElementServices\RegressionTests\*.xaml")]
    public class RegressionLoadedDynamic : XamlTest
    {
        #region Private Data

        private DispatcherTimer                 _aTimer = null;
        private static DispatcherSignalHelper   s_signalHelper;
        private Grid                            _grid;
        private Button                          _button;
        private int                             _actualEventCount = 0;

        #endregion


        #region Constructors
        /******************************************************************************
        * Function:          RegressionLoadedDynamic Constructor
        ******************************************************************************/
        public RegressionLoadedDynamic(): base(@"RegressionLoadedDynamic.xaml")
        {
            InitializeSteps += new TestStep(Initialize);
            RunSteps += new TestStep(StartTest);
            RunSteps += new TestStep(Verify);
        }
        #endregion


        #region Test Steps

        /******************************************************************************
        * Function:          Initialize
        ******************************************************************************/
        /// <summary>
        /// Initialize: 
        /// </summary>
        /// <returns>TestResult.Pass;</returns>
        TestResult Initialize()
        {
            s_signalHelper = new DispatcherSignalHelper();

            _grid = (Grid)LogicalTreeHelper.FindLogicalNode((Page)Window.Content,"Grid1");

            _button = new Button();
            _button.Content = "WPF!";
            _button.Loaded += OnLoadedButton;

            return TestResult.Pass;
        }

        /******************************************************************************
        * Function:          StartTest
        ******************************************************************************/
        /// <summary>
        /// StartTest: Start a DispatcherTimer to control verification; add a new Button to the Grid.
        /// </summary>
        /// <returns>TestResult.Pass</returns>
        TestResult StartTest()
        {
            s_signalHelper = new DispatcherSignalHelper();

            GlobalLog.LogStatus("---StartTest---");
            _aTimer = new DispatcherTimer(DispatcherPriority.Normal);
            _aTimer.Tick += new EventHandler(OnTick);
            _aTimer.Interval = new TimeSpan(0,0,0,0,1000);
            _aTimer.Start();

            _grid.Children.Add(_button);

            s_signalHelper.WaitForSignal("Finished");

            return TestResult.Pass;
        }

        #endregion


        #region Private Members

        /******************************************************************************
        * Function:          OnTick
        ******************************************************************************/
        /// <summary>
        /// Invoked every time the DispatcherTimer ticks. Used to control the timing of verification.
        /// </summary>
        /// <returns></returns>
        private void OnTick(object sender, EventArgs e)          
        {
            _aTimer.Stop();

            s_signalHelper.Signal("Finished", TestResult.Ignore);
        }

        /******************************************************************************
        * Function:          OnLoadedButton
        ******************************************************************************/
        /// <summary>
        /// Fires when the Button is loaded.
        /// </summary>
        /// <returns></returns>
        private void OnLoadedButton(Object sender, RoutedEventArgs args)
        {
            _actualEventCount++;
        }

        /******************************************************************************
        * Function:          Verify
        ******************************************************************************/
        /// <summary>
        /// Verifies the result of the test case, and returns a Pass/Fail result.
        /// Two things are verified:  event firing and the Button's IsLoaded property.
        /// </summary>
        /// <returns>A TestResult, indicating pass or fail</returns>
        TestResult Verify()
        {
            int expectedEventCount = 1;

            GlobalLog.LogEvidence("Expected Loaded Event Count: " + expectedEventCount);
            GlobalLog.LogEvidence("Actual Loaded Event Count:   " + _actualEventCount);
            GlobalLog.LogEvidence("Expected IsLoaded: True");
            GlobalLog.LogEvidence("Actual IsLoaded:   " + _button.IsLoaded);

            if (_actualEventCount == expectedEventCount && _button.IsLoaded)
            {
                return TestResult.Pass;
            }
            else
            {
                return TestResult.Fail;
            }
        }

        #endregion
    }
}

