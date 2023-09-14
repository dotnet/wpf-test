//---------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All rights reserved.
//
// All classes in this file are some helper ones, which used for do some common 
// work for many test cases.
//---------------------------------------------------------------------------

#region Using directives
using System.Windows;
using System.Windows.Controls;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Logging;
using System;
using System.Xml;
using System.Windows.Documents;
using System.Windows.Data;
using System.Windows.Automation;
using Avalon.Test.ComponentModel.UnitTests;
using Microsoft.Test.Input;

#endregion



namespace Avalon.Test.ComponentModel.Actions
{


    /// <summary>
    /// class to press left mouse button on Expander.
    /// </summary>
    [TargetType(typeof(Expander))]
    public class ExpanderLeftMouseButtonAction : IAction
    {
        #region IAction Members

        /// <summary>
        /// Press Left mouse button on Expander
        /// </summary>
        /// <param name="frmElement"></param>
        /// <param name="actionParams"></param>
        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {
            if (frmElement == null)
            {
                GlobalLog.LogStatus(" null object in ExpanderLeftMouseButtonAction");
                return;
            }
            UserInput.MouseLeftDown(frmElement, 40, 10);
            UserInput.MouseLeftUp(frmElement, 40, 10);
        }

        #endregion
    }



    /// <summary>
    /// class to press left mouse button on Expander.
    /// </summary>
    [TargetType(typeof(Expander))]
    public class ExpanderKeyAction : IAction
    {
        #region IAction Members

        /// <summary>
        /// Press key Expander bu automation.
        /// </summary>
        /// <param name="frmElement"></param>
        /// <param name="actionParams"></param>
        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {
            if (frmElement == null)
            {
                GlobalLog.LogStatus(" null Expander in ExpanderKeyAction");
                return;
            }
            string action = actionParams[1] as string;
            if (action != string.Empty)
            {
                UserInput.KeyPress("Tab");
                QueueHelper.WaitTillQueueItemsProcessed();
                UserInput.KeyPress(action);
                QueueHelper.WaitTillQueueItemsProcessed();
            }
        }

        #endregion IAction Members
    }



    /// <summary>
    /// class to change Expander's ExpandDirection.
    /// </summary>
    [TargetType(typeof(Expander))]
    public class ExpanderChangeDirectionAction : IAction
    {
        #region IAction Members

        /// <summary>
        /// Change Expander's ExpandDirection from Up to Left,Left to Down,Down to Right,Right to Up.
        /// </summary>
        /// <param name="frmElement"></param>
        /// <param name="actionParams"></param>
        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {
            Expander expander = LogicalTreeHelper.FindLogicalNode(frmElement, (string)actionParams[0]) as Expander;
            if (expander == null)
            {
                GlobalLog.LogStatus("Can not change direction for a null Expander");
                return;
            }

            switch (expander.ExpandDirection)
            {
                case ExpandDirection.Up:
                    {
                        GlobalLog.LogStatus("Change Direction from " + ExpandDirection.Up.ToString() + " to " + ExpandDirection.Left.ToString());
                        expander.ExpandDirection = ExpandDirection.Left;
                        break;
                    }

                case ExpandDirection.Left:
                    {
                        GlobalLog.LogStatus("Change Direction from " + ExpandDirection.Left.ToString() + " to " + ExpandDirection.Down.ToString());
                        expander.ExpandDirection = ExpandDirection.Down;
                        break;
                    }

                case ExpandDirection.Down:
                    {
                        GlobalLog.LogStatus("Change Direction from " + ExpandDirection.Down.ToString() + " to " + ExpandDirection.Right.ToString());
                        expander.ExpandDirection = ExpandDirection.Right;
                        break;
                    }

                case ExpandDirection.Right:
                    {
                        GlobalLog.LogStatus("Change Direction from " + ExpandDirection.Down.ToString() + " to " + ExpandDirection.Up.ToString());
                        expander.ExpandDirection = ExpandDirection.Up;
                        break;
                    }

                default:
                    break;
            }
        }

        #endregion IAction Members
    }



    /// <summary>
    /// class to set binding between Expanders.
    /// </summary>
    public class ExpanderBindingAction : IAction
    {
        #region IAction Members

        /// <summary>
        /// Change Expander's Header by hard code.
        /// </summary>
        /// <param name="frmElement"></param>
        /// <param name="actionParams"></param>
        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {
            object[] args = (object[])actionParams;
            Expander source = LogicalTreeHelper.FindLogicalNode(frmElement, (string)args[0]) as Expander;
            FrameworkElement destination = LogicalTreeHelper.FindLogicalNode(frmElement, (string)args[1]) as Expander;
            if (destination == null || source == null)
            {
                GlobalLog.LogStatus("Can not change header for a null Expander");
                return;
            }
            ControlBindingAction action = new ControlBindingAction();
            action.Do(destination, source, "IsExpanded", Expander.IsExpandedProperty);
            action.Do(destination, source, "ExpandDirection", Expander.ExpandDirectionProperty);
        }

        #endregion IAction Members
    }



    /// <summary>
    /// class to bind Content to other control.
    /// </summary>
    public class ExpanderContentBindingAction : IAction
    {
        /// <summary>
        /// bind Content to other control
        /// </summary>
        /// <param name="frmElement"></param>
        /// <param name="actionParams"></param>
        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {
            Panel panel = frmElement as Panel;
            string[] args = (string[])actionParams;
            Expander expander = LogicalTreeHelper.FindLogicalNode(panel, args[0]) as Expander;
            Control source = LogicalTreeHelper.FindLogicalNode(panel, args[1]) as Control;
            ControlBindingAction action = new ControlBindingAction();
            action.Do(expander, source, args[2], Expander.ContentProperty);
        }
    }



    /// <summary>
    /// class to set binding between Controls.
    /// </summary>
    public class ControlBindingAction : IAction
    {
        #region IAction Members

        /// <summary>
        /// Set bining between Controls
        /// </summary>
        /// <param name="frmElement"></param>
        /// <param name="actionParams"></param>
        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {
            FrameworkElement source = actionParams[0] as FrameworkElement;
            if (frmElement == null || source == null)
            {
                GlobalLog.LogStatus("Can not change header for a null Expander");
                return;
            }
            Binding bind = new Binding((string)actionParams[1]);
            bind.Source = source;
            frmElement.SetBinding((DependencyProperty)actionParams[2], bind);
            QueueHelper.WaitTillQueueItemsProcessed();
        }

        #endregion IAction Members
    }


    /// <summary>
    /// Make a FrameworkElement to get focus
    /// </summary>
    public class ExpanderFocusAction : IAction
    {
        #region IAction Members

        /// <summary>
        /// Invoke focus on every control.
        /// </summary>
        /// <param name="frmElement"></param>
        /// <param name="actionParams"></param>
        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {
            Expander expander = frmElement as Expander;
            expander.IsExpanded = !expander.IsExpanded;
            foreach (string objectName in actionParams)
            {
                FrameworkElement element = LogicalTreeHelper.FindLogicalNode(frmElement, objectName) as FrameworkElement;
                element.Focus();
            }
        }

        #endregion IAction Members
    }
}
