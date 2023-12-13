// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
 * Purpose: WindowTest for Effect 
 * Owner: Microsoft 
 ********************************************************************/
using System.Windows;
using System.Windows.Controls;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Effects
{
    /// <summary>
    /// Effects WindowTest: create a window that render in the same way with different DPIs, and themes. The window has 
    /// content ContentRoot, which is a ContentControl; Test inherits this can just change the content of the ContentRoot. 
    /// ContentRoot is added because it is require for Dpi Scaling.
    /// </summary>`
    public class EffectsWindowTest : WindowTest
    {


        #region Constructors
        /// <summary>
        /// Creates a test with Window with specific width and height.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public EffectsWindowTest(double width, double height) : this()
        {
            _windowWidth = width;
            _windowHeight = height;
        }

        /// <summary>
        /// Creates an Testcase that will create a Window with invariant rendering under different configuration. 
        /// </summary>
        protected EffectsWindowTest()
        {
            InitializeSteps += new TestStep(SetUpWindow);
        }

        #endregion

        #region Private Members

        private double _windowWidth = 200;
        private double _windowHeight = 200;

        #endregion

        #region Protected Members


        /// <summary>
        /// The root element of the window.
        /// </summary>
        protected ContentControl ContentRoot
        {
            get;
            private set;
        }

        #endregion

        #region Methods

        TestResult SetUpWindow()
        {
            ContentRoot = new ContentControl();
            EffectsTestHelper.SetupEffectsTestWindow(Window, _windowWidth, _windowHeight, ContentRoot);
            
            return TestResult.Pass;
        }

        #endregion

    }
}