// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************************
*    Source Control Information
*    
 
  
*    Revision:         $Revision: 1 $
 
*
******************************************************************************/
#define TRACE
using System;

using System.Threading;
using System.Reflection;
using System.Diagnostics;
using System.Collections;
using System.Windows.Threading;
using System.ComponentModel;
using System.Collections.Generic;
using Microsoft.Test.Threading;
using Microsoft.Test.Discovery;
using Avalon.Test.CoreUI.Trusted;

namespace Avalon.Test.CoreUI.Threading
{


    /// <summary>
    /// Helper class to keep track of the number of nested Wait calls(Globally)
    /// We only allow 15 ThreadPool enqueue items for workerThreads
    /// and 150 nested Wait per thread.
    /// </summary>
    static class InvokeWrapperHelper
    {


        /// <summary>
        /// Calls Wait on the DispatcherOperation depending on the argument passed.
        /// It also keeps track of the nested wait calls that ocurrs on this thread.
        /// This method should be called if the Operation.Dispatcher and CurrentDispatcher
        /// are the same.
        /// </summary>
        /// <param name="inWrapper">DispatcherOperationWrapper where the Wait call will be performed
        /// If any other time is passed we will use the DispatcherOperation.Wait(TimeSpan) API.
        /// </param>
        public static void InvokeUsingSameDispatcherThread(InvokeWrapper inWrapper)
        {
             InvokeUsingSameDispatcherThread(inWrapper, new TimeSpan(0));
        }


        
        /// <summary>
        /// Calls Wait on the DispatcherOperation depending on the argument passed.
        /// It also keeps track of the nested wait calls that ocurrs on this thread.
        /// This method should be called if the Operation.Dispatcher and CurrentDispatcher
        /// are the same.
        /// </summary>
        /// <param name="opWrapper">DispatcherOperationWrapper where the Wait call will be performed</param>
        /// <param name="timeSpan">If TimeSpan.MinValue is passed, DispatcherOperation.Wait will be called.
        /// If any other time is passed we will use the DispatcherOperation.Wait(TimeSpan) API.</param>
        public static void WaitUsingSameDispatcherThread(DispatcherOperationWrapper opWrapper, TimeSpan timeSpan)
        {
             InvokeUsingSameDispatcherThread(opWrapper, timeSpan);
        }


        static void InvokeUsingSameDispatcherThread(object opWrapper, TimeSpan timeSpan)
        {
                            
            if (!DispatcherHelper.IsDispatcherDisabledProcessing() && t_waitCountThisThread < 100)
            {                
                t_waitCountThisThread++; // We don't need to take a lock on this because it is ThreadStatic.
                     
                try
                {
                    if (opWrapper is DispatcherOperationWrapper)
                    {
                        DispatcherOperationWrapper opW = (DispatcherOperationWrapper)opWrapper;

                        bool expectingException = (opW.Status == DispatcherOperationStatus.Executing);

                        bool exceptionCaught = false;
                        
                        try
                        {
                            opW.WaitInternal(timeSpan);
                        }
                        catch(InvalidOperationException)
                        {
                            exceptionCaught = true;
                        }

                        if (expectingException)
                        {
                            if (!exceptionCaught)
                            {
                                CoreLogger.LogTestResult(false,"An exception was expected because the items was executing.");
                            }                            
                        }

                        
                    }
                    else 
                    {
                        ((InvokeWrapper)opWrapper).InvokeInternal();
                    }
                }
                finally
                {                    
                    t_waitCountThisThread--; // We don't need to take a lock on this because it is ThreadStatic.                               
                }
            }
        }


        /// <summary>
        /// Helper function that makes a Thread from the threadPool to enqueue a workitem to 
        /// execute a code where it will choose a random DispatcherOperationWrapper and a Wait* 
        /// call will be executed.
        /// Keeps track that only 13 items are enqueue on the ThreadPool. The reason is that 
        /// we don't want to flod the ThreadPool.
        /// </summary>
        public static void AddInvokeToThreadPool(InvokeWrapper inWrapper)
        {
            AddWaitToThreadPrivate((int)InvokingThread.ThreadPool, inWrapper);
        }

        /// <summary>
        /// Helper function that makes a Thread from the threadPool to enqueue a workitem to 
        /// execute a code where it will choose a random DispatcherOperationWrapper and a Wait* 
        /// call will be executed.
        /// Keeps track that only 13 items are enqueue on the ThreadPool. The reason is that 
        /// we don't want to flod the ThreadPool.
        /// </summary>
        public static void AddInvokeToThread(InvokingThread invokingThread, InvokeWrapper inWrapper)
        {
            AddWaitToThreadPrivate((int)invokingThread, inWrapper);
        }




        /// <summary>
        /// Helper function that makes a Thread from the threadPool to enqueue a workitem to 
        /// execute a code where it will choose a random DispatcherOperationWrapper and a Wait* 
        /// call will be executed.
        /// Keeps track that only 13 items are enqueue on the ThreadPool. The reason is that 
        /// we don't want to flod the ThreadPool.
        /// </summary>
        public static void AddWaitToThreadPool(DispatcherOperationWrapper target, TimeSpan timeSpan)
        {
            object[] objArray = {target, timeSpan};
            AddWaitToThreadPrivate((int)InvokingThread.ThreadPool, objArray);
        }


        private static void AddWaitToThreadPrivate(int threadIndex, object o)
        {
            lock(s_syncCount)
            {                              
                if (s_counter < 15)
                {
                    s_counter++;

                    if (threadIndex == 3)
                    {
                        ThreadPool.QueueUserWorkItem(new WaitCallback(InvokeWaitCallback), o);
                    }
                    else
                    {
                       Thread thread = new Thread(new ParameterizedThreadStart(InvokeWaitCallback));

                       if (threadIndex == 1)
                       {
                            thread.SetApartmentState(ApartmentState.STA);
                       }
                       else
                       {
                            thread.SetApartmentState(ApartmentState.MTA);
                       }                       
                    }
                    
                }
            }
        }


        /// <summary>
        /// </summary>        
        public static Exception ExceptionCaught = null;


        /// <summary>
        /// Callback that will be executed be the threadPool. 
        /// A random DispatcherPriorityWrapper is chosen and a Wait call is perform depending
        /// on the argument.
        /// </summary>
        /// <param name="o">TimeSpan that will be used on the Wait call.
        /// If TimeSpan.MinValue is used. DispatcherOperation.Wait() will be called.</param>
        static private void InvokeWaitCallback(object o)
        {
            DispatcherOperationWrapper opWrapper = null;
            TimeSpan timeSpan = new TimeSpan(0);
            
            if (!(o is InvokeWrapper))
            {
                 object[] objArray = (object[])o;
                 timeSpan = (TimeSpan)objArray[1];
                 opWrapper = (DispatcherOperationWrapper)objArray[0];
            }
                
            try
            {
                if (o is InvokeWrapper)
                {
                    ((InvokeWrapper)o).InvokeInternal();
                }
                else if (opWrapper != null)
                {
                    opWrapper.WaitInternal(timeSpan);
                }
            }
            catch(Exception e)
            {
                ExceptionCaught = e;
                
                if (o is InvokeWrapper)
                {
                    ((InvokeWrapper)o).Dispatcher.Invoke(DispatcherPriority.Send,
                        (DispatcherOperationCallback)delegate (object arg)
                        {
                            ThrowException(arg);
                            return null;
                        }, e);
                }
                else if (opWrapper != null)
                {
                    opWrapper.DispatcherW.RealDispatcher.BeginInvoke(DispatcherPriority.SystemIdle,
                        (DispatcherOperationCallback)delegate (object arg)
                        {
                            ThrowException(arg);
                            return null;
                        }, e);
                }                
            }
            finally
            {
                lock(s_syncCount)
                {                               
                    s_counter--;
                }
            }
                
        }


        static void ThrowException(object o )
        {
            throw (Exception)o;
        }
       
        /// <summary>
        /// Keeps track of the nested Wait calls on the same thread as the DispatcherThread.
        /// </summary>
        [ThreadStatic]
        static int t_waitCountThisThread = 0; // There is no need to take locks on this field.

        /// <summary>
        /// Keeps track on the ThreadPool calls.
        /// </summary>
        static int s_counter = 0;
        /// <summary>
        /// This is used to synchronaze the _counter property.
        /// </summary>
        static object s_syncCount = new object(); 


    }
}

