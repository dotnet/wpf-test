// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Threading;

//using Avalon.Test.CoreUI.Threading;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.Win32;

using Microsoft.Test.Logging;
using Microsoft.Test.Modeling;
using Microsoft.Test.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test;


namespace Avalon.Test.CoreUI.Threading
{
    /// <summary>
    /// NestingPumpModel Model class
    /// The mbt file for this model can be found at
    /// F:\NT\testsrc\windowstestdata\REDIST\client\wcptests\Core\Base\Testcases\Threading\NestingPumpModel.mbt
    /// Testing Posting Items from multiple threads    
    /// </summary>
    [Model(@"FeatureTests\ElementServices\NestingPumpMultipleDispatcher.xtc", 1, @"Threading\Nesting",
       TestCaseSecurityLevel.PartialTrust, "Model for Nesting multiple types of message pumps",
        ExpandModelCases = true, Area = "ElementServices")] 
    public class NestingPumpModel : CoreModel 
    {
        /// <summary>
        /// Creates a NestingPumpModel Model instance
        /// </summary>
        public NestingPumpModel(): base()
        {
            Name = "NestingPumpModel";
            Description = "NestingPumpModel Model";
            ModelPath = "MODEL_PATH_TOKEN";            
            
            //Add Action Handlers
            AddAction("SetupTestCase", new ActionHandler(SetupTestCase));
        }

        /// <summary>
        /// SEtup and RUn the test case
        /// </summary>
        private bool SetupTestCase(State endState, State inParams, State outParams)
        {
            NestingPumpModelAutomation nestingPump = new NestingPumpModelAutomation();           
            return nestingPump.SetupTestCase(inParams);
        }
    }


    /// <summary>
    /// This class actually implements all the code for test Nesting Pump
    /// </summary>
    public class NestingPumpModelAutomation : ModelAutomationBase
    {
        /// <summary>
        /// NestingPumpModel Model class
        /// </summary>
        public NestingPumpModelAutomation() : base(null)
        {
        }

        /// <summary>
        /// NestingPumpModel Model class
        /// </summary>
        public bool SetupTestCase(IDictionary dictionary)
        {
            string argSeedForNestingDepthLevel = (string)dictionary["SeedForNestingDepthLevel"];
            string argSeedForDispatcherNestingAction = (string)dictionary["SeedForDispatcherNestingAction"];

            Random randomNestingDepthLevel = new Random(Int32.Parse(argSeedForNestingDepthLevel));
            Random randomDispatcherNestingAction = new Random(Int32.Parse(argSeedForDispatcherNestingAction));
            
            // Creating the dispatcher List
            _dispatcherWList = DispatcherWrapper.CreateRandomDispatcherWrappers(dictionary);       

            
            List<DispatcherThreadCaseInfo> infoList = new List<DispatcherThreadCaseInfo>();
                

            // We loop through the list of dispatchers to create all the actions
            // that each dispatcher will create.
            for (int i = 0; i < _dispatcherWList.Length; i++)
            {
                List<NestingActionThread> actionList = new List<NestingActionThread>();

                // We need to choose the amount or nesting that this current
                // dispatcher thread will perform.
                int indexNesting = randomNestingDepthLevel.Next(0,100) % _nestingLevels.Length;
                int amountNesting = _nestingLevels[indexNesting];

                for (int j = 0; j < amountNesting; j++)
                {
                    // We need to choose what type of dispatcher nested pump
                    // will be executed
                    int indexType = randomDispatcherNestingAction.Next(0,100) % _dispatcherTypeList.Length;
                    actionList.Add(new NestingActionThread(_dispatcherTypeList[indexType]));
                }
               
                // All the actions are stored on the list. We save the list 
                _dispatchersActionHash.Add(_dispatcherWList[i], actionList);


                // Creating a Dispatcher Thread.
                DispatcherThreadCaseInfo info = new DispatcherThreadCaseInfo(_dispatcherWList[i], actionList);
                info.DispatcherFirstIdleDispatched += new ThreadTestCaseCallback(DispatcherCallback);
                infoList.Add(info);
            }

            MultipleDispatcherThreadTestCase mdttc = new MultipleDispatcherThreadTestCase();
            mdttc.InitializeDispatcherThreads(infoList.ToArray());

            // Blocking this thread until everything is completed
            mdttc.Run();

            CoreLogger.LogTestResult(ValidateDispatcherWrappers(),"Validation failed");


            if (mdttc.ExceptionList.Length != 0)
            {
                CoreLogger.LogTestResult(false,"There was an exception on any of the dispatchers threads");
            }
            
            return true;
        }


        private bool ValidateDispatcherWrappers()
        {
            for(int i=0; i < _dispatcherWList.Length;i++)
            {
                DispatcherWrapper dispatcherW = _dispatcherWList[i];
                List<NestingActionThread> actionList = (List<NestingActionThread>)_dispatchersActionHash[dispatcherW];

                if (actionList.Count != dispatcherW.DeepestNestingLevel)
                {
                    CoreLogger.LogStatus("The nesting pump count doesn't match on dispatcher.  Actual: " +
                        dispatcherW.DeepestNestingLevel.ToString() + "Real: " + actionList.Count.ToString());
                    
                    return false;
                }
                
            }
            
            return true;
        }

        /// <summary>
        /// Each dispatcher thread will execute this method
        /// after the first systemidle is achevied.
        /// </summary>
        public static void DispatcherCallback(ThreadCaseInfo info, EventArgs args)
        {
            List<NestingActionThread> actionList = (List<NestingActionThread>)info.Argument;
            DispatcherThreadCaseInfo dInfo = (DispatcherThreadCaseInfo)info;
             
            for (int i = 0; i < actionList.Count; i++)
            {
                object[] oArray = {dInfo.DispatcherW, actionList[i]};

                
                
                DispatcherHelper.EnqueueBackgroundCallback(
                    (DispatcherOperationCallback)delegate(object o)                
                    {
                        oArray = (object[])o;
                        NestingActionThread actionThread = (NestingActionThread)oArray[1];
                        DispatcherWrapper dispatcherW = (DispatcherWrapper)oArray[0];
                        
                        dispatcherW.PushNestedLoop(actionThread.CurrentDispatcherType);
                        return null;

                    }, oArray);
            }


            // We suppose to be deep down on the Nested Loop
            DispatcherHelper.EnqueueBackgroundCallback(
                (DispatcherOperationCallback)delegate(object o)                
                {
                    DispatcherThreadCaseInfo dispatcherInfo = (DispatcherThreadCaseInfo)o;
                    List<NestingActionThread> actionListThree = (List<NestingActionThread>)info.Argument;

                    if (dispatcherInfo.DispatcherW.CurrentNestingLevel != actionListThree.Count)
                    {
                        lock (_syncRootInstance)
                        {
                            CoreLogger.LogTestResult(false,"The nesting level doesn't match");
                        }
                    }
                    return null;

                }, info);


            // This code run a a DispatcherTimer each 150 milliseconds.
            DispatcherTimerHelper.CreateBackgroundPriority(TimeSpan.FromMilliseconds(150), 
                (EventHandler) delegate (object o, EventArgs ar)
                {
                    DispatcherTimer timer = (DispatcherTimer)o;

                    DispatcherThreadCaseInfo dispatcherInfo = (DispatcherThreadCaseInfo)timer.Tag;
                    List<NestingActionThread> actionListTwo = (List<NestingActionThread>)dispatcherInfo.Argument;

                    
                    if (dispatcherInfo.DispatcherW.CurrentNestingLevel == 1)
                    {
                        // If only one nested pump is left we stop the timer
                        // and enqueue an action to stop the dispatcher
                        timer.Stop();

                        DispatcherHelper.EnqueueBackgroundCallback(
                            (DispatcherOperationCallback)delegate(object dispatcher)                
                            {

                                DispatcherWrapper dispatcherW = (DispatcherWrapper)dispatcher;

                                dispatcherW.ShutDown();                                

                                return null;

                            }, dispatcherInfo.DispatcherW);

                    }



                    // Exiting only 1 nested pump at time                  
                    DispatcherHelper.EnqueueBackgroundCallback(
                        (DispatcherOperationCallback)delegate(object dispatcher)                
                        {

                            DispatcherWrapper dispatcherW = (DispatcherWrapper)dispatcher;

                            dispatcherW.PopNestedLoop();
                            

                            return null;

                        }, dispatcherInfo.DispatcherW);
  
                },dInfo);
        }


        DispatcherWrapper[] _dispatcherWList = null;
        Hashtable _dispatchersActionHash = new Hashtable();
        private readonly int[] _nestingLevels = {1,5,10,20,30};
        private readonly DispatcherType[] _dispatcherTypeList = {DispatcherType.Avalon, DispatcherType.Win32};
    }

    /// <summary>
    /// NestingPumpModel Model class
    /// </summary>
    public class NestingActionThread
    {
        /// <summary>
        /// NestingPumpModel Model class
        /// </summary>
        public NestingActionThread(DispatcherType dispatcherType)
        {
            _dispatcherType = dispatcherType;
        }

        /// <summary>
        /// NestingPumpModel Model class
        /// </summary>
        public DispatcherType CurrentDispatcherType
        {
            get
            {
                return _dispatcherType;
            }
        }

        DispatcherType _dispatcherType;
    }
}

//This file was generated using MDE on: Wednesday, February 16, 2005 9:20:08 PM

