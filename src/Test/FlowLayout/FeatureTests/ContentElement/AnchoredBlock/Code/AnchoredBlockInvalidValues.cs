// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description : Test invalid values and other various features for AnchoredBlocks.  
//                                                
// Verification: Basic API validation.  
// Created by:  Microsoft
////////////////////////////////////////////////////////////////////////////////////////////////////////////// 
using System;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;

namespace Microsoft.Test.FlowLayout
{
    /// <summary>   
    /// Test Invalid values in anchored block.   
    /// </summary>
    [Test(3, "AnchoredBlock", "AnchoredBlocksInvalidValuesTest", MethodName = "Run")]
    public class AnchoredBlocksInvalidValuesTest : AvalonTest
    {
        #region Test case members

        private Window _testWin;
 
        #endregion

        #region Constructor

        public AnchoredBlocksInvalidValuesTest()
            : base()
        {
            CreateLog = false;   
            RunSteps += new TestStep(InvalidFigureLengthTest);
            RunSteps += new TestStep(FigureLengthTypeCheckTest);
            RunSteps += new TestStep(ValidFigureLengthTypeConverterTest);
            CleanUpSteps += new TestStep(CleanUp);
        }

        private TestResult CleanUp()
        {
            _testWin.Close();
            return TestResult.Pass;
        }
      
        private TestResult InvalidFigureLengthTest()
        {           
            TestLog log = new TestLog("InvalidFigureLengthTest FigureLength > 1 Page");
            log.LogStatus("Setting FigureLength to > 1 Page...");
            SetFigureLength(1.1, FigureUnitType.Page, log);
            log.Close();
                      
            log = new TestLog("InvalidFigureLengthTest FigureLength > 1 Content");
            log.LogStatus("Setting FigureLength to > 1 Content...");
            SetFigureLength(1.5, FigureUnitType.Content, log);
            log.Close();
            
            log = new TestLog("InvalidFigureLengthTest FigureLength -100");
            log.LogStatus("Setting FigureLength to -100...");
            SetFigureLength(-100, FigureUnitType.Pixel, log);
            log.Close();
            
            log = new TestLog("InvalidFigureLengthTest FigureLength -.1");
            log.LogStatus("Setting FigureLength to -.1...");
            SetFigureLength(-.1, FigureUnitType.Column, log);
            log.Close();

            return TestResult.Pass;
        }

        private TestResult FigureLengthTypeCheckTest()
        {                       
            TestLog log = new TestLog("FigureLengthTypeCheckTest FigureLength default");
            log.LogStatus("Creating default FigureLength...");
            FigureLength figLength = new FigureLength();
            TestFigureLengthType(figLength, 1, log);
            log.Close();
            
            log = new TestLog("FigureLengthTypeCheckTest FigureLength.Pixel");
            log.LogStatus("Creating FigureLength of FigureUnitType.Pixel...");
            figLength = new FigureLength(1, FigureUnitType.Pixel);            
            TestFigureLengthType(figLength, 0, log);
            log.Close();
            
            log = new TestLog("FigureLengthTypeCheckTest FigureLength.Column");
            log.LogStatus("Creating FigureLength of FigureUnitType.Column...");
            figLength = new FigureLength(1, FigureUnitType.Column);
            TestFigureLengthType(figLength, 2, log);
            log.Close();
            
            log = new TestLog("FigureLengthTypeCheckTest FigureLength.Content");
            log.LogStatus("Creating FigureLength of FigureUnitType.Content...");
            figLength = new FigureLength(1, FigureUnitType.Content);
            TestFigureLengthType(figLength, 3, log);
            log.Close();
            
            log = new TestLog("FigureLengthTypeCheckTest FigureLength.Page");
            log.LogStatus("Creating FigureLength of FigureUnitType.Page...");
            figLength = new FigureLength(1, FigureUnitType.Page);
            TestFigureLengthType(figLength, 4, log);
            log.Close();

            return TestResult.Pass;
        }

        private TestResult ValidFigureLengthTypeConverterTest()
        {           
            return FigureLengthTypeConverterTest("AnchoredBlocks_FigureLengthTypeConverter_Valid.xaml");           
        }

        #endregion

        #region Helper Methods

        private TestResult _contentRenderedResult = TestResult.Unknown;
        private DispatcherFrame _frame;
        private TestResult FigureLengthTypeConverterTest(string content)
        {
            _testWin = new Window();
            LogComment("Trying to load " + content);
            _testWin = Microsoft.Test.Layout.TestWin.Launch(typeof(Window), 800, 600, 0, 0, content, true, "LayoutTestWindow");
            _testWin.ContentRendered += new EventHandler(LogTest);
            _frame = new DispatcherFrame();
            //Blocking the call until we get the ContentRenderedEvent so we know that we did not blow up on render
            //We release the block when the content renders.
            Dispatcher.PushFrame(_frame);
            return _contentRenderedResult;
        }

        private void LogTest(object sender, EventArgs args)
        {
            _contentRenderedResult = TestResult.Pass;
            _frame.Continue = false;
        }

        private void TestFigureLengthType(FigureLength fl, int value, TestLog log)
        {
            if (GetFigureLengthType(fl) != value)
            {
                log.LogStatus("Wrong FigureLength type!");
                log.LogStatus("Expected: " + ReturnType(value) + " Got: " + ReturnType(GetFigureLengthType(fl)));
                log.Result = TestResult.Fail;
            }
            else
            {
                log.Result = TestResult.Pass;
            }
        }

        private int GetFigureLengthType(FigureLength fl)
        {
            int value = -1;

            if (fl.IsAbsolute)
            {
                value = 0;
            }

            if (fl.IsAuto)
            {
                value = 1;
            }

            if (fl.IsColumn)
            {
                value = 2;
            }

            if (fl.IsContent)
            {
                value = 3;
            }

            if (fl.IsPage)
            {
                value = 4;
            }

            return value;
        }

        private string ReturnType(int value)
        {
            string result = "Unassigned";
            switch (value)
            {
                case 0:
                    result = "Absolute";
                    break;

                case 1:
                    result = "Auto";
                    break;

                case 2:
                    result = "Column";
                    break;

                case 3:
                    result = "Content";
                    break;

                case 4:
                    result = "Page";
                    break;
            }

            return result;
        }

        private void SetFigureLength(double value, FigureUnitType unitType, TestLog log)
        {
            string expectedExceptionType = "System.ArgumentOutOfRangeException";
            try
            {
                FigureLength fl = new FigureLength(value, unitType);
                log.LogStatus("Did not get an exception (should have).");
                log.Result = TestResult.Fail;
            }
            catch (Exception ex)
            {
                if (ex.GetType().ToString() != expectedExceptionType)
                {
                    log.LogStatus("Did not get the expected type of exception.");
                    log.LogStatus("Expected: " + expectedExceptionType + " Got: " + ex.GetType().ToString());
                    log.Result = TestResult.Fail;
                }
                else
                {
                    log.LogStatus("Got the expected type of exception.");
                    log.Result = TestResult.Pass;
                }
            }
        }
        #endregion
    }
}
