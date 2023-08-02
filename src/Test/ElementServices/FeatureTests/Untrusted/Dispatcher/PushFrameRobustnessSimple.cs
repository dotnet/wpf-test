// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Markup;
using System.Windows.Threading;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Source;
using Avalon.Test.CoreUI.Trusted;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Threading;
using Microsoft.Test.Win32;

namespace Avalon.Test.Framework.Dispatchers
{
    /******************************************************************************
    * CLASS:          PushFrameRobustnessSimple
    ******************************************************************************/
    /// <summary>
    /// Writing test case for 



    [Test(1, "Dispatcher", "PushFrameRobustnessSimple")]
    public class PushFrameRobustnessSimple : TestCase
    {
        #region Constructor
        ValidationTracking _vTrack = null;
        #endregion


        #region Constructor
        /******************************************************************************
        * Function:          PushFrameRobustnessSimple Constructor
        ******************************************************************************/
        /// <summary>
        /// Constructor
        /// </summary>
        public PushFrameRobustnessSimple() :base(TestCaseType.None)
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
            _vTrack = new ValidationTracking(2);
            _vTrack.AddValidation(0,"First exception was not throw");
            _vTrack.AddValidation(1,"The item was not executed after reenter Dispatcher.Run");

            MainDispatcher = Dispatcher.CurrentDispatcher;
                
            MainDispatcher.BeginInvoke(DispatcherPriority.SystemIdle, new DispatcherOperationCallback(throwException), null);
            MainDispatcher.BeginInvoke(DispatcherPriority.SystemIdle, new DispatcherOperationCallback(executeItem), null);
           
            try
            {
                DispatcherHelper.RunDispatcher();
            }
            catch(Exception)
            {
                _vTrack.SetValue(0,true);
            }

             DispatcherHelper.RunDispatcher();

            _vTrack.ValidateAll();

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
        }
        #endregion

        #region Private Members
        private object throwException(object o)
        {

            throw new Exception("On purpose");
        }

     
        private object executeItem(object o)
        {
            
            Microsoft.Test.Threading.DispatcherHelper.ShutDown();
            _vTrack.SetValue(1,true);
            return null;
        }
        #endregion        
    }
}
