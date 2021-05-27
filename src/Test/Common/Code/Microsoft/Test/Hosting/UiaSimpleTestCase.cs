// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Automation;

namespace Microsoft.Test.Hosting
{
    /// <summary>
    /// UIAutomation testcase base class
    /// </summary>
    [Serializable]
    public abstract class UiaSimpleTestcase : UiaDistributedTestcase
    {

        /// <summary>
        /// Creates a new UiaSimpleTestcase
        /// </summary>
        public UiaSimpleTestcase()
        {
            //Add steps for the test
            AddStep(new UiaDistributedStep(Init), UiaDistributedStepTarget.UiElement);

            //We cast to automationelement just to be nice
            AddStep(delegate(object target)
            {
                DoTest((AutomationElement)target);
            }, UiaDistributedStepTarget.AutomationElement);

            AddStep(new UiaDistributedStep(Validate), UiaDistributedStepTarget.UiElement);
        }

        /// <summary>
        /// Initialize the testcase in process
        /// </summary>
        /// <param name="target">Avalon UIElement</param>
        public abstract void Init(object target);

        /// <summary>
        /// Perform action on the test out of process
        /// </summary>
        /// <param name="target">UI Automation Element corresponding to the Avalon Element</param>
        public abstract void DoTest(AutomationElement target);

        /// <summary>
        /// Validating the test in process
        /// </summary>
        /// <param name="target">Avalon UIElement</param>
        public abstract void Validate(object target);

    }
}
