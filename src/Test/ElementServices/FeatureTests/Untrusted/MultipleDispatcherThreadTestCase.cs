// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Abstracts core-level tree operations.  The intention
 *          of abstracting the operations is to reduce the effect
 *          of future API changes in the product's core level.
 *
 
  
 * Revision:         $Revision: 1 $
 
********************************************************************/

using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Threading;
using Avalon.Test.CoreUI.Threading;
using Avalon.Test.CoreUI.Trusted;

namespace Avalon.Test.CoreUI
{
    ///<summary>
    ///</summary>    
    public class MultipleDispatcherThreadTestCase : MultipleThreadTestCase
    {
        ///<summary>
        ///</summary>   
        public MultipleDispatcherThreadTestCase() : base(ThreadCaseSynchronization.None)
        {

        }

        ///<summary>
        ///</summary>   
        public void InitializeDispatcherThreads(DispatcherThreadCaseInfo[] dispatcherThreadCaseInfo)
        {
            _dispatchersWaitHandle = new CoreAutoResetEvent(false,dispatcherThreadCaseInfo.Length);

            ThreadCaseInfo[] infos = new ThreadCaseInfo[dispatcherThreadCaseInfo.Length];
            for (int i=0;i<dispatcherThreadCaseInfo.Length;i++)
            {
                infos[i] = dispatcherThreadCaseInfo[i];                
            }

            this.Initialize(infos);            
        }

        /// <summary>
        /// Run the specified dispatcher
        /// </summary> 
        protected override void RunDispatcher(ThreadCaseInfo tcInfo)
        {
            DispatcherThreadCaseInfo DTCI = (DispatcherThreadCaseInfo)tcInfo;
            DTCI.DispatcherW.Run();
        }
        

        ///<summary>
        ///</summary>   
        public static void DispatcherStartedCommon(ThreadCaseInfo info, EventArgs args)
        {
            DispatcherThreadCaseInfo DTCI = (DispatcherThreadCaseInfo)info;
            DTCI.DispatcherW.SetDispatcher(Dispatcher.CurrentDispatcher);
        }

        ///<summary>
        ///</summary>   
        public static void DispatcherFirstIdleCommon(ThreadCaseInfo info, EventArgs args)
        {
            ((MultipleDispatcherThreadTestCase)info.Owner)._dispatchersWaitHandle.Set();
        }

        ///<summary>
        ///</summary>           
        public WaitHandle AllDispatchersRunningWaitHandle
        {
            get
            {                
                return _dispatchersWaitHandle.AutoEvent;
            }
        }

        private CoreAutoResetEvent _dispatchersWaitHandle = null;
        
    }

    ///<summary>
    ///</summary>
    public class DispatcherThreadCaseInfo : ThreadCaseInfo
    {

        ///<summary>
        /// Constructor
        ///</summary>
        public DispatcherThreadCaseInfo(DispatcherWrapper dispatcherWrapper) : base(null)
        {         
        }
        
        ///<summary>
        /// Constructor
        ///</summary>
        public DispatcherThreadCaseInfo(DispatcherWrapper dispatcherWrapper, object argument) : base(argument)
        { 
            Initialization(dispatcherWrapper);
        }

        ///<summary>
        ///</summary>
        public DispatcherWrapper DispatcherW
        {
            get
            {
                return _dispatcherW;
            }
        }

        private void Initialization(DispatcherWrapper dispatcherWrapper)
        {
            _dispatcherW = dispatcherWrapper;
            this.ThreadStarted += new ThreadTestCaseCallback(MultipleDispatcherThreadTestCase.DispatcherStartedCommon);
            this.DispatcherFirstIdleDispatched += new ThreadTestCaseCallback(MultipleDispatcherThreadTestCase.DispatcherFirstIdleCommon);
            this.MessagePump = _dispatcherW.CurrentDispatcherType;
        }

        private DispatcherWrapper _dispatcherW = null;
    }
    
}

