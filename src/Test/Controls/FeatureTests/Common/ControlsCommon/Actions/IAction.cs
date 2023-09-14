// IAction.cs
// Defines IAction/IAsyncAction interface
// 
// Test Cases call actions to do something. Actions implements IAction interface.
// Actions with more than one step which executes asycnhronously should implement IAsyncAction
// interface                     

using System;
using System.Windows;

namespace Avalon.Test.ComponentModel
{
    /// <summary>
    /// IAction interface
    /// </summary>
    public interface IAction
    {                
        void Do(FrameworkElement frmElement, params object[] actionParams);
    }

    public interface IAsyncAction : IAction
    {
        /// <summary>
        /// Fires when action completes
        /// </summary>
        event ActionCompletedEventHandler OnActionCompleted;
    }

    /// <summary>
    /// Delegate for ActionCompletedEvent
    /// </summary>
    public delegate void ActionCompletedEventHandler (object sender, ActionCompletedEventArgs args);

    
    /// <summary>
    /// ActionCompletedEventArgs
    /// </summary>
    public class ActionCompletedEventArgs : EventArgs
    {

        public bool Status
        {
            get
            { 
                return status;
            } 

            set
            { 
                status = value;
            }
        }         

        private bool status;

    }

}
