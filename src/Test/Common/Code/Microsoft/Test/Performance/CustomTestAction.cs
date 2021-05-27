// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Microsoft.Test.Performance
{
    /// <summary>
    /// ActionDelegate is meant to allow the custom test action
    /// to easily expose Action functionality throuhg a delegate. This allows
    /// an Action implementory to easily create
    /// </summary>
    /// <returns></returns>
    public delegate ActionBehavior ActionDelegate();


    /// <summary>
    /// Custom test action is meant to allow easy custom Test Actions
    /// where you have a simple scenario and don't want to code up a whole
    /// class derived from a test Action. All you must do is construct a CustomTestAction
    /// with a preAction and Action delegate (either can be null if you want)
    /// and PerfActionHelper will take it from there.
    /// </summary>
    public class CustomTestAction : TestAction
    {

        #region PrivateMembers
        private ActionDelegate _preAction;
        private ActionDelegate _action;
        #endregion PrivateMembers

      

        /// <summary>
        /// Constructs a CustomTestAction for consumption by a PerfActionHelper
        /// </summary>
        /// <param name="name">The name that this operation will assume. This is the name that perfaction helper will
        /// key off of on the command line to select this action.</param>
        /// <param name="preAction">PreAction delegate that will be called to set up the scenario, prior to measurments. Provide null to specify no PreAction.</param>
        /// <param name="action">Action delegate that will be called to begin measured action operations.</param>
        public CustomTestAction(string name, ActionDelegate preAction, ActionDelegate action) : base(name)
        {
            if (action == null)
            {
                throw new ArgumentException("A custom action delegate is required.", "action");
            }

            this._preAction = preAction;
            this._action = action;
        }

        /// <summary>
        /// Executes PreAction delegate if it was supplied.
        /// </summary>
        public override ActionBehavior PreAction()
        {
            if (_preAction == null)
            {
                return ActionBehavior.ActionIsComplete;
            }
            else
            {
                return _preAction();
            }
        }

        /// <summary>
        /// Executes the provided Action delegate.
        /// </summary>
        public override ActionBehavior Action()
        {
            return _action();
        }
    }
}
