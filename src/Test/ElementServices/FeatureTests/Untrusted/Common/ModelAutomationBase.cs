// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using System.Collections;
using Microsoft.Win32;
using Microsoft.Test.Modeling;
using Microsoft.Test.Win32;
using Avalon.Test.CoreUI.Trusted;
using Microsoft.Test.Logging;

namespace Avalon.Test.CoreUI
{

    ///<summary>
    ///</summary>
    public class ModelAutomationBase
    {
        ///<summary>
        ///</summary>        
        public ModelAutomationBase(AsyncActionsManager asyncManager)
        {
            AsyncManager = asyncManager;
        }

        ///<summary>
        ///
        ///  This method check if we are inside of a nested pump. if we already on a pump we need to pull the next ITE Action from the 
        ///  the model.  Right now we ask to pull inside of a queue item. There is no need for this, //



        public void GetNextActionOnAvalonQueue(Dispatcher dispatcher)
        {
            GetNextActionOnAvalonQueue(dispatcher, DispatcherPriority.SystemIdle);
        }

        ///<summary>
        ///
        ///  This method check if we are inside of a nested pump. if we already on a pump we need to pull the next ITE Action from the 
        ///  the model.  Right now we ask to pull inside of a queue item. There is no need for this, //



        public void GetNextActionOnAvalonQueue(Dispatcher dispatcher, DispatcherPriority priority)
        {

            if (DispatcherRunning != DispatcherType.None)
            {
                Log("BeginInvoke GetNextAction");
                dispatcher.BeginInvoke(priority, new DispatcherOperationCallback(GetModelNextAction), null);
            }
        }


        ///<summary>
        ///  Asking to pull the next ITE action
        ///</summary>
        public object GetModelNextAction(object o)
        {
            Log("GetNextAction");
            AsyncManager.ExecuteAsyncNextAction();
           return null;
        }
       
        ///<summary>
        ///  Asking to pull the next ITE action
        ///</summary>
        public void Log(string s)
        {
            lock (_syncRootInstance)
            {
                CoreLogger.LogStatus(s);
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        public virtual void Dispose()
        {
            IsDisposed = true;
        }

        ///<summary>
        ///</summary>
        protected bool IsDisposed = false;
        
        ///<summary>
        ///</summary>
        protected AsyncActionsManager AsyncManager;

        ///<summary>
        ///</summary>
        protected DispatcherType DispatcherRunning = DispatcherType.None;

        /// <summary>
        /// 
        /// </summary>
        protected static object _syncRootInstance = new Object();

    }

}
