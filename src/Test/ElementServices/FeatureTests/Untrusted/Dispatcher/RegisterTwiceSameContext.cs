// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Source;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.Framework.Dispatchers;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Win32;


namespace Avalon.Test.Framework.Dispatchers.Registration
{
    /******************************************************************************
    * CLASS:          RegisterTwiceSameContext
    ******************************************************************************/
    ///<summary>
    /// This test was not ported from (or implemented in) v1.
    ///</summary>
    ///<remarks>
    ///     <filename>RegisterTwiceSameContext.cs</filename>
    ///</remarks>
    [Test(0, "Dispatcher", TestCaseSecurityLevel.FullTrust , "RegisterTwiceSameContext")]
    public class RegisterTwiceSameContext : TestCase
    {
        #region Constructor
        private bool _isPass = false;
        private string _testName = "";
        #endregion


        #region Constructor

        [Variation("TwiceSameContext")]
        [Variation("SameContextDifferentDispatcher")]
        [Variation("QuitandPushFrame")]

        /******************************************************************************
        * Function:          RegisterTwiceSameContext Constructor
        ******************************************************************************/
        public RegisterTwiceSameContext(string arg) :base(TestCaseType.None)
        {
            _testName = arg;
            RunSteps += new TestStep(StartTest);
        }
        #endregion


        #region Test Steps
        /******************************************************************************
        * Function:          StartTest
        ******************************************************************************/
        TestResult StartTest()
        {
            switch (_testName)
            {
                case "TwiceSameContext":
                    TwiceSameContext();
                    break;
                case "SameContextDifferentDispatcher":
                    SameContextDifferentDispatcher();
                    break;
                case "QuitandPushFrame":
                    QuitandPushFrame();
                    break;
                default:
                    throw new Microsoft.Test.TestValidationException("ERROR!!! Test case not found.");
            }

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
        }
        #endregion


        #region Public Members
        /******************************************************************************
        * Function:          TwiceSameContext
        ******************************************************************************/
        /// <summary>
        ///  Register the same context twice on the same dispatcher
        /// </summary>
        /// <remarks>
        ///  <ol>Description Steps:
        ///     <li>Create a Context and Win32Dispatcher</li>
        ///     <li>Register the context on the dispatcher</li>
        ///     <li>Register the same context on the same dispatcher</li>
        ///     <li>Waiting an exception and Validate the exception was thrown</li>
        ///  </ol>
        ///     <filename>RegisterTwiceSameContext.cs</filename>
        /// </remarks>
        public void TwiceSameContext()
        {
            Dispatcher c = new Dispatcher();
            ContextList.Add(c);

            Win32Dispatcher d = new Win32Dispatcher();

            d.RegisterContext(c);

            Exception Exp = null;

            try
            {
                d.RegisterContext(c);
            }
            catch(InvalidOperationException e)
            {
                Exp =e;
            }
            catch(Exception){}

            if (Exp == null)
            {
                throw new Microsoft.Test.TestValidationException("The expected exception was not thrown");
            }
        }

        /******************************************************************************
        * Function:          SameContextDifferentDispatcher
        ******************************************************************************/
        /// <summary>
        /// Register the same Dispatcher on two different dispatcher. Validating an exception shoud be thrown
        /// </summary>
        /// <remarks>
        ///  <ol>Description Steps:
        ///     <li>Create  1 context and 2 win32Dispatchers</li>
        ///     <li>Register the context on 1 dispatcher</li>
        ///     <li>Register the context on the other dispatcher</li>
        ///     <li>Validating the exception was thrown</li>
        ///  </ol>
        ///     <filename>RegisterTwiceSameContext.cs</filename>
        /// </remarks>
        public void SameContextDifferentDispatcher()
        {
            Dispatcher c;
            Win32Dispatcher dOne;
            Win32Dispatcher dTwo;
            Exception Exp = null;
            
            c = new Dispatcher();
            ContextList.Add(c);

            dOne = new Win32Dispatcher();
            dOne.RegisterContext(c);

            dTwo = new Win32Dispatcher();

            try
            {
                dTwo.RegisterContext(c);
            }
            catch(InvalidOperationException e)
            {
                Exp =e;
            }
            catch(Exception){}

            if (Exp == null)
            {
                throw new Microsoft.Test.TestValidationException("The expected exception was not thrown");
            }

        }

        /******************************************************************************
        * Function:          QuitandPushFrame
        ******************************************************************************/
        /// <summary>
        ///  Quitting the dispatcher before start PushFrame
        /// </summary>
        /// <remarks>
        ///  <ol>Description Steps:
        ///     <li>Create  1 context and 1 win32Dispatchers</li>
        ///     <li>Call Dispatcher.Quit</li>
        ///     <li>Post an Exit item to the dispatcher</li>
        ///     <li>Call PushFrame to the Dispatcher</li>
        ///     <li>Validating the dispatcher, dispatches the posted item</li>
        ///  </ol>
        ///     <filename>RegisterTwiceSameContext.cs</filename>
        /// </remarks>
        public void QuitandPushFrame()
        {
            Dispatcher c;
            Win32Dispatcher dOne;

            c = new Dispatcher();
            ContextList.Add(c);

            dOne = new Win32Dispatcher();
            dOne.RegisterContext(c);

            dOne.Quit();
            UIDispatcherFrame frame = new UIDispatcherFrame();

            c.BeginInvoke(new DispatcherOperationCallback(ExitDispatcher), frame);
            
            dOne.PushFrame(frame);

            if (!_isPass)
                    throw new Microsoft.Test.TestValidationException("The item was not dispatched");
        }
        #endregion

        
        #region Private Members
        /******************************************************************************
        * Function:          ExitDispatcher
        ******************************************************************************/
        object ExitDispatcher(object o)
        {
            UIDispatcherFrame frame = o as UIDispatcherFrame;

            frame.ExitRequested = true;

            _isPass = true;
            return null;
        }
        #endregion
     }
}







