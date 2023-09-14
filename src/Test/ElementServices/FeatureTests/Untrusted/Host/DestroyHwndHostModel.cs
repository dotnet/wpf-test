// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
 * Purpose: Test Case for DispatcherTimer
 *
 
  
 * Revision:         $Revision: 2 $
 
********************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Media;
using System.Windows.Interop;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Threading;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.Trusted.Controls;

using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.Modeling;
using Microsoft.Test.Threading;
using Microsoft.Test.Win32;

namespace Avalon.Test.CoreUI.Hosting
{    

    /// <summary>
    /// DestroyHwndHostModel Model class
    /// The purpose  of this test case is to validate DestroyWindowCore is called correctly.
    /// </summary>
    // todo: attribute!
    public class DestroyHwndHostModel : Model , IHostedTest
    {
        /// <summary>
        /// Creates a DestroyHwndHostModel Model instance
        /// </summary>
        public DestroyHwndHostModel(): base()
        {
            Name = "DestroyHwndHostModel";
            Description = "DestroyHwndHostModel Model";
            ModelPath = "MODEL_PATH_TOKEN";
                        
            //Add Action Handlers
            AddAction("SetupAndRunTest", new ActionHandler(SetupAndRunTest));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Handler for SetupAndRunTest</remarks>
        private bool SetupAndRunTest(State endState, State inParams, State outParams)
        {
            HwndHostDestroyState state = new HwndHostDestroyState(inParams);

            CoreModelState.Persist(state);

            StartTestCase(state);

            return true;
        }

        /// <summary>
        /// Represents the current ITestContainer for this IHostedTest.
        /// </summary>
        public ITestContainer TestContainer
        {
            get
            {
                return _testContainer;
            }
            set
            {
                _testContainer = value;
            }
        }


        void StartTestCase(HwndHostDestroyState state)
        {

            DockPanel dockPanel = new DockPanel();

            TestContainer.DisplayObject(dockPanel, 10,10,500,500);

            if (state.HwndHostType == "Win32ButtonCtrl")
            {
                _hostingControl = new Win32ButtonCtrl();
                CoreLogger.LogStatus("Win32ButtonCtrl created.");
            }

            if (state.HwndHostTreeState != "NoAddedToSource")
            {
                dockPanel.Children.Add(_hostingControl);
                CoreLogger.LogStatus("HwndHost control added to the tree.");
            }

            if (state.HwndHostTreeState == "RemovedFromSource")
            {
                DispatcherHelper.EnqueueBackgroundCallback( (DispatcherOperationCallback) delegate(object notUsed)
                {
                    dockPanel.Children.Remove(_hostingControl);
                    CoreLogger.LogStatus("HwndHost control removed to the tree.");                    
                    return null;
                },null);
            }

            // Destroy HWND async

            DispatcherHelper.EnqueueBackgroundCallback((DispatcherOperationCallback)delegate(object notUsed)
            {
                CoreLogger.LogStatus("Async Destroy HWND...");

                if (state.TypeOfDestroyCall == "WM_DESTROY")
                {
                    CoreLogger.LogStatus("Calling DestroyWindow (Win32)");  
                    NativeMethods.DestroyWindow(new HandleRef(null,_hostingControl.Handle));
                    DispatcherHelper.EnqueueBackgroundCallback( new DispatcherOperationCallback(ShutDownTest), null);
                }
                else if (state.TypeOfDestroyCall == "Dispose")
                {
                    CoreLogger.LogStatus("Calling HwndHost.Dispose()");  
                    _hostingControl.Dispose();
                    DispatcherHelper.EnqueueBackgroundCallback( new DispatcherOperationCallback(ShutDownTest), null);
                }
                else if (state.TypeOfDestroyCall == "DisposeFromWorkerThread")
                {           
                    ThreadPool.QueueUserWorkItem(
                        (WaitCallback) delegate (object o)
                        {
                            try
                            {
                                _hostingControl.Dispose();
                            }
                            catch(Exception e)
                            {
                                s_exceptionList.Add(e);
                            }
                            finally
                            {
                                DispatcherHelper.EnqueueBackgroundCallback((Dispatcher)o, new DispatcherOperationCallback(ShutDownTest), null);
                            }
                        }, Dispatcher.CurrentDispatcher);
                }           
                else // TypeOfDestroyCall == "DispatcherShutdown"
                {
                    // Run verification after Dispatcher shutdown is finished.
                    Dispatcher.CurrentDispatcher.ShutdownFinished += (EventHandler)delegate(object o,EventArgs args)
                        {
                            // Non-null argument: No dispatcher.
                            ShutDownTest(new object());
                        };
                    DispatcherHelper.ShutDown();
                }
                return null;
            },null);

            Dispatcher.Run();

        }

        
        /// <summary>
        /// Do verification and shutdown case.
        /// </summary>
        private object ShutDownTest(object o)
        {

            ValidateHwndHwndHostIsDestroy();

            CoreLogger.LogStatus("Shutdown the test");  
            if (o != null)
            {
                // There is no dispatcher left
                CoreLogger.LogStatus("End test now"); 
                TestContainer.EndTestNow();
            }
            else
            {
                CoreLogger.LogStatus("End test");
                TestContainer.EndTestNow();
                Dispatcher.CurrentDispatcher.InvokeShutdown();
            }
            return null;
        }


        /// <summary>
        /// </summary>
        protected virtual void RequestedDispatcherExit()
        {
            ValidateHwndHwndHostIsDestroy();
        }

        private void ValidateHwndHwndHostIsDestroy()
        {
            if (s_exceptionList.Count != 0)
            {
                CoreLogger.LogTestResult(false,"An exception was caught.");
                CoreLogger.LogStatus(s_exceptionList[0].StackTrace.ToString());
            }

            CoreModelState coreState = CoreModelState.Load();
            if (coreState == null)
            {
                throw new Microsoft.Test.TestValidationException("couldn't load CoreModelState");
            }
            else if (coreState.Dictionary == null)
            {
                throw new Exception("No dictionary");
            }

            HwndHostDestroyState state = new HwndHostDestroyState(coreState.Dictionary);

            if (state.TypeOfDestroyCall == "WM_DESTROY")
            {
                // verify not destroyed
                if (((IDisposedTest)_hostingControl).IsControlDestroyed)
                {
                    CoreLogger.LogTestResult(false, "DestroyWindowCore was called after hosted Hwnd was sent WM_DESTROY.");
                }
                else
                {
                    CoreLogger.LogStatus("DestroyWindowCore was not called after HwndHost was sent WM_DESTROY.", ConsoleColor.Green);
                }
            }
            else if (state.HwndHostTreeState == "NoAddedToSource")
            {
                // verify not destroyed
                if (((IDisposedTest)_hostingControl).IsControlDestroyed)
                {
                    CoreLogger.LogTestResult(false, "DestroyWindowCore was called on HwndHost that was not in tree.");
                }
                else
                {
                    CoreLogger.LogStatus("DestroyWindowCore was not called on HwndHost that was not in the tree.", ConsoleColor.Green);
                }
            }
            else
            {
                // verify was destroyed
                if (!((IDisposedTest)_hostingControl).IsControlDestroyed)
                {
                    CoreLogger.LogTestResult(false, "DestroyWindowCore was not called");
                }
                else
                {
                    CoreLogger.LogStatus("DestroyWindowCore was called", ConsoleColor.Green);
                }
            }

        }

        static List<Exception> s_exceptionList = new List<Exception>();
        HwndHost _hostingControl = null;


        private ITestContainer _testContainer;        
        
    }
}

//This file was generated using MDE on: Sunday, February 20, 2005 1:08:11 PM


