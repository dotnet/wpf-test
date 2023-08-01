// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.Test.Modeling;
using System.Windows.Threading;
using Avalon.Test.CoreUI.Trusted;
using Microsoft.Test.Discovery;
using Microsoft.Test;

namespace Avalon.Test.CoreUI.Threading
{
    /****************  DispatcherOperationCompleteModel Model *****************
     *  Description: 
     *  Area: 
      *  Dependencies: ClientTestLibrary.dll
     *  Revision History:
     **********************************************************/

    /// <summary>
    /// DispatcherOperationCompleteModel Model class
    /// </summary>
    [Model(@"FeatureTests\ElementServices\OperationComplete_Stateless.xtc", 1, @"Threading\Operation", TestCaseSecurityLevel.FullTrust, "DispatcherOperationCompleteModel", Description = "Staless Model for using 1 Operation.Complete doing multiple Complete (Single thread - multiple thread)", ExpandModelCases = true, Area = "ElementServices")]    
    public class DispatcherOperationCompleteModel : CoreModel 
    {
        /// <summary>
        /// Creates a DispatcherOperationCompleteModel Model instance
        /// </summary>
        public DispatcherOperationCompleteModel(): base()
        {
            Name = "DispatcherOperationCompleteModel";
            Description = "DispatcherOperationCompleteModel Model";
            ModelPath = "MODEL_PATH_TOKEN";

            //Attach Event Handlers
            OnInitialize += new EventHandler(OnInitialize_Handler);
            OnCleanUp += new EventHandler(OnCleanUp_Handler);
            OnGetCurrentState += new StateEventHandler(OnGetCurrentState_Handler);
            OnBeginCase += new StateEventHandler(OnBeginCase_Handler);
            OnEndCase += new StateEventHandler(OnEndCase_Handler);
            base.OnSetAsyncHelper += new EventHandler(OnAsyncHelper_Handler);
            base.OnEndCaseOnNestedPump += new EventHandler(OnEndCaseOnNestedPump_Handler);

            //Add Action Handlers
            AddAction("CompleteOperation", new ActionHandler(CompleteOperation));
            AddAction("ChooseDispatcher", new ActionHandler(ChooseDispatcher));
            AddAction("Validate", new ActionHandler(Validate));
        }

        /// <summary>
        /// Initializes the Model
        /// </summary>
        /// <remarks>
        /// Attached to OnInitialize event which is fired when your model is
        /// created and added to a Traversal
        /// </remarks>
        /// <param name="sender">The model that fired the event</param>
        /// <param name="e">Contains no information (EventArgs.Empty)</param>
        private void OnInitialize_Handler(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Cleans up the Model
        /// </summary>
        /// <remarks>
        /// Attached to OnCleanUp event which is fired when your model is
        /// removed from the Traversal
        /// </remarks>
        /// <param name="sender">The model that fired the event</param>
        /// <param name="e">Contains no information (EventArgs.Empty)</param>
        private void OnCleanUp_Handler(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Gets the current State of the Model
        /// </summary>
        /// <remarks>
        /// Attached to OnGetCurrentState event which is fired after
        /// each action to validate
        /// </remarks>
        /// <param name="sender">Model that fired the event</param>
        /// <param name="e">The current State in a StateEventArgs</param>
        private void OnGetCurrentState_Handler(object sender, StateEventArgs e)
        {
            // The state values set here will be compared to the expected state by the Model's default ValidateState function.
            // Only put code here that sets the State object to represent the current State of the Model.
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
            // The state values passed in here are what the traversal expects the current/begin state to be
            // Put code here that sets the system state to match the current State object
            // Any code that needs to be run at the start of each case can also go here
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
            // The state values passed in here are what the traversal expects the current/end state to be
            // Put code here that sets the system state to match the current State object
            // Any code that needs to be run at the end of each case can also go here

            _automation.Dispose();
        }

        /// <summary>
        /// Callback after the AsyncActionManager is set
        /// </summary>
        private void OnAsyncHelper_Handler(object actionsManager, EventArgs e)
        {
            // If you need the AsyncActionManager, you can get it
            // from Model.ActionManager
        }

        /// <summary>
        /// Callback that is called when alls transitions for the current test case on the 
        /// XTC are completed. For example you may want to exited the Dispatcher or
        /// or close the nested pump.
        /// </summary>
        private void OnEndCaseOnNestedPump_Handler(object o,EventArgs args)
        {
            _automation.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Handler for CompleteOperation</remarks>
        /// <param name="endState">Expected end State object</param>
        /// <param name="inParams">Input action parameters object</param>
        /// <param name="outParams">Output action parameters object</param>
        /// <returns>false if errors</returns>
        private bool CompleteOperation(State endState, State inParams, State outParams)
        {
            //Action Params (listed here for convienence during coding)
            //inParams["Timeout"] - 

            /* You should perform your action based on the Action Parameters passed to you in the Params Object;
             * Use the ParamsOut object to validate the result of the action
             * If the action fails you should call RaiseError and return false;
             */

            _automation.CompleteOperation(
                inParams["EnqueuedOriginThread"], 
                inParams["EnqueuedPriority"],
                inParams["KeepBusy"],
                inParams["CompleteOriginThread"], 
                inParams["CompleteTimeout"]);
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Handler for ChooseDispatcher</remarks>
        /// <param name="endState">Expected end State object</param>
        /// <param name="inParams">Input action parameters object</param>
        /// <param name="outParams">Output action parameters object</param>
        /// <returns>false if errors</returns>
        private bool ChooseDispatcher(State endState, State inParams, State outParams)
        {
            //Action Params (listed here for convienence during coding)
            //inParams["DispatcherType"] - 

            /* You should perform your action based on the Action Parameters passed to you in the Params Object;
             * Use the ParamsOut object to validate the result of the action
             * If the action fails you should call RaiseError and return false;
             */
            if (_automation != null)
            {
                _automation.Dispose();
            }

            _automation = new DispatcherOperationModelingAutomation(this.AsyncActions);
            _automation.ChooseDispatcher(inParams["DispatcherType"]);
            
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Handler for ChooseDispatcher</remarks>
        /// <param name="endState">Expected end State object</param>
        /// <param name="inParams">Input action parameters object</param>
        /// <param name="outParams">Output action parameters object</param>
        /// <returns>false if errors</returns>
        private bool Validate(State endState, State inParams, State outParams)
        {
            //Action Params (listed here for convienence during coding)
            //inParams["DispatcherType"] - 

            /* You should perform your action based on the Action Parameters passed to you in the Params Object;
             * Use the ParamsOut object to validate the result of the action
             * If the action fails you should call RaiseError and return false;
             */

            _automation.Validate();
            return true;
        }

        DispatcherOperationModelingAutomation _automation = null;
    }

}

//This file was generated using MDE on: Tuesday, December 14, 2004 9:37:42 AM

