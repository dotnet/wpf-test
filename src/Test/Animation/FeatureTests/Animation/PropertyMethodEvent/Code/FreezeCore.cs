// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Threading;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Media.Animation;

using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Animation
{
    /// <summary>
    /// <area>Animation.PropertyMethodEvent.Regressions</area>
    /// <priority>2</priority>



    [Test(2, "Animation.PropertyMethodEvent.Regressions", "FreezeCoreTest")]

    /******************************************************************************
    *******************************************************************************
    * CLASS:          FreezeCoreTest
    *******************************************************************************
    ******************************************************************************/
    public class FreezeCoreTest : WindowTest
    {

        #region Test case members

        private string                  _inputString     = null;
        private DoubleAnimPath          _doubleAnimPath  = null;
        private MatrixAnimPath          _matrixAnimPath  = null;
        private RectKF                  _rectKF          = null;
        private ColorAnim               _colorAnim       = null;
        
        #endregion


        #region Constructor

        [Variation("Double")]
        [Variation("Matrix")]
        [Variation("RectKF")]
        [Variation("Color")]

        /******************************************************************************
        * Function:          FreezeCoreTest Constructor
        ******************************************************************************/
        public FreezeCoreTest(string testValue)
        {
            _inputString = testValue;
            InitializeSteps += new TestStep(CreateAnimation);
            RunSteps += new TestStep(Verify);
        }

        #endregion


        #region Test Steps
        

        /******************************************************************************
        * Function:          CreateAnimation
        ******************************************************************************/
        /// <summary>
        /// Creates an Animation.
        /// </summary>
        /// <returns>Returns success</returns>
        TestResult CreateAnimation()
        {
            switch (_inputString)
            {
                case "Double" :
                    _doubleAnimPath = new DoubleAnimPath();
                    _doubleAnimPath.Freeze();  //This invokes FreezeCore().
                    break;

                case "Matrix" :
                    _matrixAnimPath = new MatrixAnimPath();
                    _matrixAnimPath.Freeze();  //This invokes FreezeCore().
                    break;

                case "RectKF" :
                    _rectKF = new RectKF();
                    _rectKF.Freeze();          //This invokes FreezeCore().
                    break;

                case "ColorAnim" :
                    _colorAnim = new ColorAnim();
                    _colorAnim.Freeze();         //This invokes FreezeCore().
                    break;
            }
            
            return TestResult.Pass;
        }

        /******************************************************************************
        * Function:          Verify
        ******************************************************************************/
        /// <summary>
        /// Changes the animation, which should throw an exception.
        /// </summary>
        /// <returns></returns>
        private TestResult Verify()
        {
            switch (_inputString)
            {
                case "Double" :
                    SetExpectedErrorTypeInStep(typeof(InvalidOperationException), "Outer");
                    _doubleAnimPath.IsAdditive = true;     //This throws the exception.
                    break;

                case "Matrix" :
                    SetExpectedErrorTypeInStep(typeof(InvalidOperationException), "Outer");
                    _matrixAnimPath.IsAdditive = true;     //This throws the exception.
                    break;

                case "RectKF" :
                    SetExpectedErrorTypeInStep(typeof(InvalidOperationException), "Outer");
                    _rectKF.IsAdditive = true;         //This throws the exception.
                    break;

                case "Int32KeyFrame" :
                    SetExpectedErrorTypeInStep(typeof(InvalidOperationException), "Outer");
                    _colorAnim.IsAdditive = true;        //This throws the exception.
                    break;
            }

            return TestResult.Pass;
        }

        #endregion
    }

    /******************************************************************************
    *******************************************************************************
    * CLASS:          DoubleAnimPath
    *******************************************************************************
    ******************************************************************************/
    public class DoubleAnimPath : DoubleAnimationUsingPath
    {
        /// <summary>
        /// Called by the base Freezable class to make this object frozen.
        /// </summary>
        protected override bool FreezeCore(bool isChecking)
        {
            bool canFreeze = base.FreezeCore(false);

            return canFreeze;
        }
    }

    /******************************************************************************
    *******************************************************************************
    * CLASS:          MatrixAnimPath
    *******************************************************************************
    ******************************************************************************/
    public class MatrixAnimPath : MatrixAnimationUsingPath
    {
        /// <summary>
        /// Called by the base Freezable class to make this object frozen.
        /// </summary>
        protected override bool FreezeCore(bool isChecking)
        {
            bool canFreeze = base.FreezeCore(false);

            return canFreeze;
        }
    }

    /******************************************************************************
    *******************************************************************************
    * CLASS:          RectKF
    *******************************************************************************
    ******************************************************************************/
    public class RectKF : RectAnimationUsingKeyFrames
    {
        /// <summary>
        /// Called by the base Freezable class to make this object frozen.
        /// </summary>
        protected override bool FreezeCore(bool isChecking)
        {
            bool canFreeze = base.FreezeCore(false);

            return canFreeze;
        }
    }

    /******************************************************************************
    *******************************************************************************
    * CLASS:          Int32KeyFrame
    *******************************************************************************
    ******************************************************************************/
    public class ColorAnim : ColorAnimation
    {
        /// <summary>
        /// Called by the base Freezable class to make this object frozen.
        /// </summary>
        protected override bool FreezeCore(bool isChecking)
        {
            bool canFreeze = base.FreezeCore(false);

            return canFreeze;
        }
    }
}
