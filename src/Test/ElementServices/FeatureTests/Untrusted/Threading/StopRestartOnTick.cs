// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
//using System.Diagnostics;
using System.Diagnostics;
using System.Threading;
using System.Windows.Threading;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.Common;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Threading;


namespace Avalon.Test.CoreUI.Threading
{
    /******************************************************************************
    * CLASS:          StopRestartOnTick
    ******************************************************************************/
    [Test(0, "Threading.Timing", "StopRestartOnTick")]
    public class StopRestartOnTick : AvalonTest
    {
        #region Private Data
        private int _count = 0;
        private static readonly long s_lowTolerance = 50;
        #endregion


        #region Constructor
        /******************************************************************************
        * Function:          StopRestartOnTick Constructor
        ******************************************************************************/
        /// <summary>
        /// Constructor
        /// </summary>
        public StopRestartOnTick()
        {
            RunSteps += new TestStep(StartTest);
        }
        #endregion


        #region Test Steps
        /******************************************************************************
        * Function:          StartTest
        ******************************************************************************/
        /// <summary>
        /// Restart the dispatcherTimer during Tick event.
        /// </summary>  
        TestResult StartTest()
        {
            DispatcherHelper.EnqueueBackgroundCallback(Dispatcher.CurrentDispatcher,
                (DispatcherOperationCallback)delegate (object notUsed)
                {
                    DispatcherTimer t = new DispatcherTimer();
                    t.Tick += new EventHandler(StopRestartTickHandler);
                    t.Interval = TimeSpan.FromSeconds(1);
                    t.Tag = Stopwatch.StartNew();
                    t.Start();
                    return null;
                },null);


            DispatcherHelper.RunDispatcher();

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
        }
        #endregion


        #region Private Members
        /******************************************************************************
        * Function:          StopRestartTickHandler
        ******************************************************************************/
        private void StopRestartTickHandler(object o, EventArgs args)
        {
            DispatcherTimer timer = (DispatcherTimer)o;
            Stopwatch watch = (Stopwatch)timer.Tag;
            watch.Stop();

            if (watch.ElapsedMilliseconds < 1000 - s_lowTolerance)
            {   
                throw new Microsoft.Test.TestValidationException("A time was fired sooner than expected.  Real: " + watch.ElapsedMilliseconds.ToString() + "; Expected: " + (1000 - s_lowTolerance).ToString());
            }

            if (_count < 1)
            {
                watch.Reset();
                watch.Start();
                timer.Start();
            }
            else
            {
                DispatcherHelper.ShutDownBackground();
            }

            _count++;
        }
        #endregion
    }
}
