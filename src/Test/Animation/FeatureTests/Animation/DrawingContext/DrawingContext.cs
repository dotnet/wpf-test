// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*
* ***************  MIL Basic Verification Test *****************************************
*     This is a driver file that calls a test case specified by the console parameter.
*     Animations applied within DrawingContexts are tested.
*
*     Framework:          An executable is created.
*     Area:               Animation/Timing
*     Dependencies:       \MIL\Animation\Common\animationUtils\$(O)\AnimationUtils.dll
*     Support Files:      

*********************************************************************************************/
using System;
using System.Globalization;
using System.Threading;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
     
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Animation
{
    /// <summary>
    /// <area>Animation.DrawingContext.BasicTests</area>
    /// <priority>1</priority>
    /// <description>
    /// Verify Animation using DrawingContext methods.
    /// </description>
    /// </summary>
    [Test(2, "Animation.DrawingContext.BasicTests", "DrawingContextTest")]

    class DrawingContextTest : WindowTest
    {
        #region Test case members
        private string      _inputString = "";
        #endregion


        #region Constructor
        
        [Variation("ColorDCTest", Priority=0)]
        [Variation("DoubleDCTest", Priority=0)]
        // [DISABLE WHILE PORTING]
        // [Variation("PointDCTest")]
        // [Variation("RectDCTest")]
        // [Variation("SizeDCTest")]
        
        /******************************************************************************
        * Function:          DrawingContextTest Constructor
        ******************************************************************************/
        public DrawingContextTest(string testValue)
        {
            _inputString = testValue;
            RunSteps += new TestStep(TestDrawingContext);
        }

        #endregion

        /******************************************************************************
        * Function:          TestDrawingContext
        ******************************************************************************/
        /// <summary>
        /// TestDrawingContext: Begin the DrawingContext test.
        /// </summary>
        /// <returns></returns>
        TestResult TestDrawingContext()
        {
            bool testPassed = false;
            
            switch (_inputString)
            {
                case "ColorDCTest":
                    ColorDCTest colorTest = new ColorDCTest();
                    testPassed = colorTest.StartTest();
                    break;
                case "DoubleDCTest":
                    DoubleDCTest doubleTest = new DoubleDCTest();
                    testPassed = doubleTest.StartTest();
                    break;
                case "PointDCTest":
                    PointDCTest pointTest = new PointDCTest();
                    testPassed = pointTest.StartTest();
                    break;
                case "RectDCTest":
                    RectDCTest rectTest = new RectDCTest();
                    testPassed = rectTest.StartTest();
                    break;
                case "SizeDCTest":
                    SizeDCTest sizeTest = new SizeDCTest();
                    testPassed = sizeTest.StartTest();
                    break;
                default:
                    break;
            }

            if (testPassed)
            {
                return TestResult.Pass;
            }
            else
            {
                return TestResult.Fail;
            }
        }
    }
}
