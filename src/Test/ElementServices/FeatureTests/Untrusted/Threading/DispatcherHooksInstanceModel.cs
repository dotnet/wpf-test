// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Threading;

using Avalon.Test.CoreUI.Trusted;

using Microsoft.Test.Discovery;
using Microsoft.Test;
using Microsoft.Test.Modeling;
using Microsoft.Test.Threading;


namespace Avalon.Test.CoreUI.Threading
{

    /// <summary>
    /// DispatcherHooks model for instanciation. Super simple stateless model.
    /// Adding and Removing DispatcherHooks event on different threads
    /// The model can be found at:
    /// %SDXROOT%\testsrc\windowstestdata\REDIST\client\wcptests\core\Base\Testcases\Threading\DispatcherHooksInstance.mbt    
    /// </summary>
    [Model(@"FeatureTests\ElementServices\DispatcherHooksInstance.xtc", 1, @"Threading\Operation", TestCaseSecurityLevel.FullTrust, "DispatcherHooksInstanceModel", Description = "Adding and Removing DispatcherHooks event on different threads.", ExpandModelCases = true, Area = "ElementServices")]    
    public class DispatcherHooksInstanceModel : CoreModel 
    {
        /// <summary>
        /// Creates a DispatcherOperationCompleteModel Model instance
        /// </summary>
        public DispatcherHooksInstanceModel(): base()
        {
            Name = "DispatcherHooksInstance";
            Description = "DispatcherHooksInstanceModel";
            ModelPath = "MODEL_PATH_TOKEN";

            //Attach Event Handlers
            OnBeginCase += new StateEventHandler(OnBeginCase_Handler);
            OnEndCase += new StateEventHandler(OnEndCase_Handler);
            
            //Add Action Handlers
            AddAction("SetupTest", new ActionHandler(SetupTest));

        }


        /// <summary>
        /// Sets the Model as necessary to begin the case with the given State
        /// </summary>
        /// <remarks>
        /// Attached to OnBeginCase event which is fired by the Traversal
        /// before a new case begins
        /// </remarks>
        /// <param name="sender">Model that fired the event</param>
        /// <param name="e">The Initial State in a StateEventArgs</param>
        private void OnBeginCase_Handler(object sender, StateEventArgs e)
        {

        }


        /// <summary>
        /// Sets the Model as necessary when a case ends with the given State
        /// </summary>
        /// <remarks>
        /// Attached to OnEndCase event which is fired by the Traversal
        /// after a case ends
        /// </remarks>
        /// <param name="sender">Model that fired the event</param>
        /// <param name="e">The End State in a StateEventArgs</param>
        private void OnEndCase_Handler(object sender, StateEventArgs e)
        {
        }


        /// <summary>
        /// </summary>        
        private bool SetupTest(State endState, State inParameters, State outParameters )
        {
            LogComment( "Action SetupTest" );

            LogComment( inParameters.ToString() );
            

            bool succeed = true;

            DispatcherHooksInstanceAutomation.RunTest(
                int.Parse(inParameters["NumOfReading"]),
                int.Parse(inParameters["NumOfDispatcher"]),            
                int.Parse(inParameters["DispatcherTypeSeed"])
                );


            return succeed;
        }
    }

    /// <summary>
    /// Automation Model read the DispatcherHooks form the Dispatcher on different threads.
    /// The model can be found at:
    /// %SDXROOT%\testsrc\windowstestdata\REDIST\client\wcptests\core\Base\Testcases\Threading\DispatcherHooksInstance.mbt
    /// </summary>        
    static class DispatcherHooksInstanceAutomation
    {
        /// <summary>
        /// Run the test case using the specified parameters.
        /// </summary> 
        public static void RunTest(int numberOfReading, int numberOfDispatchers, int dispatcherTypeSeed)
        {
            // We create random dispatchers (Win32 and Avalon);
            DispatcherWrapper[] dwList = DispatcherWrapper.CreateRandomDispatcherWrappers(numberOfDispatchers,  dispatcherTypeSeed);
            
            List<DispatcherThreadCaseInfo> caseInfos = new List<DispatcherThreadCaseInfo>();

            object[] oArray = {numberOfReading};
            
            for (int i = 0; i < dwList.Length; i++)
            {

                // Each dispatcher thread will run the code below.
                DispatcherThreadCaseInfo caseInfo = new DispatcherThreadCaseInfo(dwList[i],oArray);
                caseInfo.DispatcherFirstIdleDispatched += delegate (ThreadCaseInfo info, EventArgs args)
                    {
                        int readings = (int)((object[])info.Argument)[0];

                        object oldValue = null;
                        bool added = false;
                        
                        for (int j = 0; j < readings; j++)
                        {
                            DispatcherHooks hooks = DispatcherHelper.GetHooks();

                            if (!added)
                            {
                                added = true;
                                
                                lock(s_syncRoot)
                                {
                                    s_hooksQueue.Enqueue(hooks);                             
                                }
                            }
                            
                            // Validating that we always get the same dispatcher.
                            if (j == 0 && hooks == null)
                            {
                                CoreLogger.LogTestResult(false,"The Dispatcher.Hooks should not be null.");
                            }

                            if (j > 0 && hooks != oldValue)
                            {
                                CoreLogger.LogTestResult(false,"The Dispatcher.Hooks should not be null.");
                            }

                            oldValue = hooks;
                                                            
                        }                                       
                    
                        DispatcherWrapper.FromDispatcher(Dispatcher.CurrentDispatcher).ShutDown();
                        return;                        
                    };

                caseInfos.Add(caseInfo);
            }


            
            MultipleDispatcherThreadTestCase testCase = new MultipleDispatcherThreadTestCase();
            testCase.InitializeDispatcherThreads(caseInfos.ToArray());
            testCase.Run();

            // Making sure there were no exception on any of the threads.

            if (testCase.ExceptionList.Length != 0)
            {
                CoreLogger.LogTestResult(false,"Exception caught!.");                
            }


            // Making sure there are no duplicate DispatcherHooks accross multiple dispatchers.
            while (s_hooksQueue.Count > 1)
            {
                DispatcherHooks hooks = s_hooksQueue.Dequeue();

                if (s_hooksQueue.Contains(hooks))
                {
                    CoreLogger.LogTestResult(false,"It should not be duplicates.");                                    
                }
            }


        }


        static Queue<DispatcherHooks> s_hooksQueue = new Queue<DispatcherHooks>(5);
        static object s_syncRoot = new object();
           
    }
    
}
