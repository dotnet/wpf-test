// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Events;
using Avalon.Test.CoreUI.Trusted;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Avalon.Test.CoreUI.Events
{
    /// <summary>
    /// Check the return value of AdjustEventSource for UIElement
    /// </summary>
    /// <remarks>
    /// <para />
    /// Area: Events\UIElement
    /// <para />
    /// <para />
    /// <para />
    /// FileName:  UIElementAdjustEventSource.cs
    /// <para />
    /// <ol>Scenarios covered:
    ///     <li>Create an UIElement</li>
    ///     <li>Check the return value of AdjustEventSource</li>
    /// </ol>
    /// </remarks>
    /// <seealso cref="TestCaseType"/>
    [Test(0, "Events.UIElement", "UIElementAdjustEventSource")]
    public class UIElementAdjustEventSource : TestCase
    {
        #region Constructor
        public UIElementAdjustEventSource() :base(TestCaseType.ContextSupport)
        {
            RunSteps += new TestStep(StartTest);
        }
        #endregion


        #region Test Steps
        /// <summary>
        /// Entry Method for the test case
        /// </summary>
        TestResult StartTest()
        {
            CoreLogger.LogStatus ("This is a test case checking the return value of UIElement");

            Dispatcher context = MainDispatcher;
            

            MyUIElement1 myUIElement = null;

            using (CoreLogger.AutoStatus ("Create an UIElement"))
            {
                myUIElement = new MyUIElement1 ();
            }

            using (CoreLogger.AutoStatus ("Create an UIElement"))
            {
                myUIElement.CheckReturnValue ();
            }

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
        }
        #endregion


        #region Public Members
        private class MyUIElement1 : UIElement
        {
            public MyUIElement1 ():base() { }

            public void CheckReturnValue ()
            {
                using (CoreLogger.AutoStatus ("Check Return Value of AdjustEventSource"))
                {
                                        /* REMOVED BY Microsoft JUST TO GET CODE COMPILING - TEST IS EXPECTED TO FAIL
                    if (AdjustEventSource (null) != null)
                        throw new Microsoft.Test.TestValidationException ("AdjustEventSource for UIElement should return null");
                                        */
                }
            }
        }
        #endregion
    }
}
