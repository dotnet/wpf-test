// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Windows.Automation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Hosting
{
    /// <summary>
    /// Base class for implemeting distributed UIAutomated testcases
    /// </summary>
    [Serializable]
    public class UiaDistributedTestcase
    {
        #region Private Data

        private UiaTestState state = new UiaTestState();
        private List<UiaDistributedStepInfo> stepInfos = new List<UiaDistributedStepInfo>();

        #endregion

        #region Public Members

        /// <summary>
        /// Adds a Step to the testcase
        /// </summary>
        /// <param name="step">the Step to add</param>
        /// <param name="target">the target for the step</param>
        public void AddStep(UiaDistributedStep step, UiaDistributedStepTarget target)
        {
            AddStep(step, target, false);
        }

        /// <summary>
        /// Adds a Step to the testcase
        /// </summary>
        /// <param name="step">the Step to add</param>
        /// <param name="target">the target for the step</param>
        /// <param name="alwaysRun">true if the step should run even if other steps before it fail, otherwise, false</param>
        public void AddStep(UiaDistributedStep step, UiaDistributedStepTarget target, bool alwaysRun)
        {
            stepInfos.Add(new UiaDistributedStepInfo(step, target, alwaysRun));
        }

        /// <summary>
        /// State variables that can are shared in proc and out of proc
        /// </summary>
        /// <remarks>
        /// you can only put object in here that are marshal by ref or serailizable
        /// </remarks>
        public UiaTestState SharedState
        {
            get { return state; }
        }

        #endregion


        #region Internal Members

        internal void RunTest(UiaDistributedTestcaseHost host, AutomationElement automationElement, string uiElementName)
        {
            if (stepInfos.Count == 0)
                throw new InvalidOperationException("You must first add one or more steps run this testcase");

            //Because we the Testcase object has a MarshalByRef shared state, we
            //need to ensure that we have a server channel in this AppDomain.
            //

            
            //attach the testcase to the host
            host.AttachTestcase(this, uiElementName);

            //run all the steps
            int nextStep = 0;
            try
            {
                for (int i = 0; i < stepInfos.Count; i++)
                {
                    nextStep = i+1;
                    UiaDistributedStepInfo stepInfo = stepInfos[i];

                    //Invoke with the automation element, otherwise have the host invoke it with the UIElement
                    if (stepInfo.Target == UiaDistributedStepTarget.AutomationElement)
                        PerformStep(i, automationElement);
                    else //UiElement
                        host.PerformStep(i);
                }
            }
            finally
            {
                //run any remaining steps marked "alwaysRun"
                for (int i = nextStep; i < stepInfos.Count; i++)
                {
                    UiaDistributedStepInfo stepInfo = stepInfos[i];
                    if (!stepInfo.AlwaysRun)
                        continue;

                    try
                    {
                        //Invoke with the automation element, otherwise have the host invoke it with the UIElement
                        if (stepInfo.Target == UiaDistributedStepTarget.AutomationElement)
                            PerformStep(i, automationElement);
                        else //UiElement
                            host.PerformStep(i);
                    }
                    catch (Exception e)
                    {
                        //Because these are clean up steps we ---- exceptions thrown by them
                        GlobalLog.LogDebug("AlwaysRun Step Failed with the following exception: " + e.ToString());
                    }
                }

            }
        }

        //used by the host to invoke the step
        internal void PerformStep(int stepIndex, object target)
        {
            UiaDistributedStepInfo stepInfo = stepInfos[stepIndex];
            stepInfo.Step(target);
        }

        #endregion
    }


    /// <summary>
    /// Delegate for implementing test steps for distributed UIATests
    /// </summary>
    /// <param name="target"></param>
    public delegate void UiaDistributedStep(object target);
   

}
