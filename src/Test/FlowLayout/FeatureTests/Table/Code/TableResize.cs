// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

////////////////////////////////////////////////////////////////////////////////////
// Description:  Testing table on resizing the window.
//				 
// Verification: The window will be resized and so will the table be.Table should resize correctly without leaving any aritifacts.
//		 Property dump is used for verifying. The last window size will be captured.
//		 
// Created by:	Microsoft
////////////////////////////////////////////////////////////////////////////////////
using System.Windows.Threading;
using System.Windows;
using System.Windows.Media;
using Microsoft.Test.Layout;
using Microsoft.Test.Layout.PropertyDump;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;

namespace Microsoft.Test.FlowLayout
{
    /// <summary>   
    /// Testing Table Resize.   
    /// </summary>
    [Test(0, "Table", "TableResize", MethodName="Run", Variables="VscanMasterPath=FeatureTests\\FlowLayout\\Masters\\VSCAN")]
    class TableResize : AvalonTest
    {
        #region Test case members

        private Window _testWin;
        private string _inputString;

        #endregion

        #region Constructor

        [Variation("resizeExample")]
        public TableResize(string testInput)
            : base()
        {
            _inputString = testInput + ".xaml";
            InitializeSteps += new TestStep(Initialize);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(RunTests);
        }

        #endregion

        #region Test Steps

        /// <summary>
        /// Initialize: setup the test
        /// </summary>
        /// <returns>TestResult.Pass;</returns>
        private TestResult Initialize()
        {
            _testWin = (Window)TestWin.Launch(typeof(Window), 600, 800, 0, 0, _inputString, true, "");            
            _testWin.Resources.MergedDictionaries.Add(GenericStyles.LoadAllStyles());
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            return TestResult.Pass;
        }

        private TestResult CleanUp()
        {
            _testWin.Close();
            return TestResult.Pass;
        }

        /// <summary>
        /// RunTests: Runs a set of tests based on the input variation.
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult RunTests()
        {
            WaitForPriority(DispatcherPriority.SystemIdle);

            Resize(350, 550);
            Resize(750, 350);
            Resize(100, 100);
            Resize(350, 1500);
            Resize(600, 800);

            if (_testWin.Content is FrameworkElement)
            {
                // Setting Window.Content size to ensure same size of root element over all themes.  
                // Different themes have different sized window chrome which will cause property dump 
                // and vscan failures even though the rest of the content is the same.
                // 784x564 is the content size of a 800x600 window in Aero theme.
                ((FrameworkElement)_testWin.Content).Height = 564;
                ((FrameworkElement)_testWin.Content).Width = 784;
                WaitFor(500); //Wait for a split second to make sure content has rendered.
            }
            
            return Verify();
        }

        #endregion

        private void Resize(double height, double width)
        {
            Status(string.Format("Resizing the window.  Height: {0}, Width: {1}", height, width));
            _testWin.Height = height;
            _testWin.Width = width;
            WaitForPriority(DispatcherPriority.SystemIdle);
        }

        private TestResult Verify()
        {
            WaitForPriority(DispatcherPriority.SystemIdle);
            Status("Property Dump Verification");
            try
            {
                PropertyDumpHelper helper = new PropertyDumpHelper((Visual)_testWin.Content);
                if (helper.CompareLogShow(new Arguments(this)))
                {
                    return TestResult.Pass;
                }
                else
                {
                    return TestResult.Fail;
                }
            }
            catch (System.Xml.XmlException)
            {
                return TestResult.Ignore;
            }
        }
    }
}
