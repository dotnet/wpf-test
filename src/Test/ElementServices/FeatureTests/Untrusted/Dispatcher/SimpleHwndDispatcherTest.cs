// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Microsoft.Test;
using Microsoft.Test.Discovery;
using Avalon.Test.CoreUI;
using System.Threading;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Media;
using Avalon.Test.CoreUI.Common;

using System.Runtime.InteropServices;
using Avalon.Test.CoreUI.Source;
using Microsoft.Test.Win32;
using Microsoft.Test.Threading;
using System.Windows.Interop;
using Avalon.Test.CoreUI.Win32;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;


namespace Avalon.Test.CoreUI.Dispatchers
{
    [Test(0, @"Dispatcher", TestCaseSecurityLevel.PartialTrust, "SimpleHwndDispatcherTest")]
    public class SimpleHwndDispatcherTest : TestCase
    {
        #region Private Data
        private DispatcherFrame _frame = null;
        #endregion

        
        #region Constructor
        /******************************************************************************
        * Function:          SimpleHwndDispatcherTest Constructor
        ******************************************************************************/
        /// <summary>
        /// Constructor.  On the base class pass TestCaseType.ContextSupport 
        /// </summary>
        public SimpleHwndDispatcherTest() :base(TestCaseType.ContextSupport)
        {
            RunSteps += new TestStep(StartTest);
        }
        #endregion


        #region Test Steps
        /******************************************************************************
        * Function:          StartTest
        ******************************************************************************/
        TestResult StartTest()
        {
            try
            {
                MainDispatcher.BeginInvoke(DispatcherPriority.Normal, new DispatcherOperationCallback(_postPushFrame), null);

                Win32GenericMessagePump.Run();
            }
            finally
            {
            }

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
        }
        #endregion


        #region Private Members
        private object _postPushFrame(object o)
        {
            MainDispatcher.BeginInvoke(DispatcherPriority.Normal, new DispatcherOperationCallback(_postPushFrameExit), null);
            _frame = new DispatcherFrame();
            DispatcherHelper.PushFrame(_frame);
            return null;
        }


        private object _postPushFrameExit(object o)
        {
            MainDispatcher.BeginInvoke(DispatcherPriority.Normal, new DispatcherOperationCallback(_postExit),null);
            _frame.Continue=false;
            return null;
        }

        private object _postExit(object o)
        {
            if (!MainDispatcher.CheckAccess())
                throw new Microsoft.Test.TestValidationException("This thread should be the dispatcher thread");

            Win32GenericMessagePump.Stop();
            return null;
        }
        #endregion
    }
}

