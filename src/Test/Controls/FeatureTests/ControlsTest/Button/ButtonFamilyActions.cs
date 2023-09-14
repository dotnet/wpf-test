using System;
using System.Windows;
using System.Windows.Controls;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Logging;
using Microsoft.Test.Input;

namespace Avalon.Test.ComponentModel.Actions
{
    #region Mouse Action Members
    [TargetType(typeof(Control))]
    public class ControlMouseLeftClickAction : IAction
    {
        /// <summary>
        /// Mouse left click on a control.
        /// </summary>
        /// <param name="control"></param>
        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {
            QueueHelper.WaitTillQueueItemsProcessed();
            Control control = frmElement as Control;
            if (control is Button)
            {
                Button button = control as Button;
                if (button.ClickMode == ClickMode.Hover)
                {
                    UserInput.MouseMove(button, (int)(button.ActualWidth / 2 - 5), (int)(button.ActualHeight / 2 - 5));

                }
                else if (button.ClickMode == ClickMode.Release)
                {
                    if (button.ContextMenu is ContextMenu)
                    {
                        UserInput.MouseLeftDown(button, (int)(button.ActualWidth / 2 - 5), (int)(button.ActualHeight / 2 - 5));
                        QueueHelper.WaitTillQueueItemsProcessed();
                        UserInput.MouseLeftUp(button, (int)(button.ActualWidth / 2 - 5), (int)(button.ActualHeight / 2 - 5));
                    }
                    else
                    {
                        UserInput.MouseLeftClickCenter(button);
                    }
                }
                else if (button.ClickMode == ClickMode.Press)
                {
                    UserInput.MouseLeftDown(button, (int)(button.ActualWidth / 2 - 5), (int)(button.ActualHeight / 2 - 5));
                }
            }
            else
            {
                if (control is ComboBox)
                {
                    ComboBox cb = control as ComboBox;
                    if (cb.FlowDirection == FlowDirection.LeftToRight)
                    {
                        UserInput.MouseLeftDown(cb, (int)(cb.ActualWidth - 5), (int)(cb.ActualHeight / 2 - 5));
                        QueueHelper.WaitTillQueueItemsProcessed();
                        UserInput.MouseLeftUp(cb, (int)(cb.ActualWidth - 5), (int)(cb.ActualHeight / 2 - 5));
                        QueueHelper.WaitTillQueueItemsProcessed();
                        UserInput.MouseLeftDown(cb, (int)(cb.ActualWidth - 5), (int)(cb.ActualHeight / 2 - 5));
                        QueueHelper.WaitTillQueueItemsProcessed();
                        UserInput.MouseLeftUp(cb, (int)(cb.ActualWidth - 5), (int)(cb.ActualHeight / 2 - 5));
                    }
                    else
                    {
                        UserInput.MouseLeftDown(cb, 5, (int)(cb.ActualHeight / 2 - 5));
                        QueueHelper.WaitTillQueueItemsProcessed();
                        UserInput.MouseLeftUp(cb, 5, (int)(cb.ActualHeight / 2 - 5));
                        QueueHelper.WaitTillQueueItemsProcessed();
                        UserInput.MouseLeftDown(cb, 5, (int)(cb.ActualHeight / 2 - 5));
                        QueueHelper.WaitTillQueueItemsProcessed();
                        UserInput.MouseLeftUp(cb, 5, (int)(cb.ActualHeight / 2 - 5));
                    }
                }
                else
                {
                    UserInput.MouseLeftClickCenter(control);
                }
            }
        }
    }
    [TargetType(typeof(Control))]
    public class ControlMouseRightClickAction : IAction
    {
        /// <summary>
        /// Mouse right click on a control.
        /// </summary>
        /// <param name="control"></param>
        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {
            Control control = frmElement as Control;
            UserInput.MouseRightClickCenter(control);
        }
    }
    [TargetType(typeof(Control))]
    public class ControlMouseLeftDownAction : IAction
    {
        /// <summary>
        /// Mouse down on a control.
        /// </summary>
        /// <param name="control"></param>
        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {
            Control control = frmElement as Control;
            UserInput.MouseLeftDown(control);
        }
    }
    [TargetType(typeof(Control))]
    public class ControlMouseUpAction : IAction
    {
        /// <summary>
        /// Mouse up on a control.
        /// </summary>
        /// <param name="control"></param>
        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {
            Control control = frmElement as Control;
            UserInput.MouseLeftUp(control);
        }
    }

    [TargetType(typeof(Control))]
    public class ControlMouseLeftDownCenterAction : IAction
    {
        /// <summary>
        /// Mouse down on the center of the control.
        /// </summary>
        /// <param name="control"></param>
        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {
            Control control = frmElement as Control;
            UserInput.MouseLeftDownCenter(control);
        }
    }
    [TargetType(typeof(Control))]
    public class ControlMouseLeftUpCenterAction : IAction
    {
        /// <summary>
        /// Mouse down on the center of the control.
        /// </summary>
        /// <param name="control"></param>
        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {
            Control control = frmElement as Control;
            UserInput.MouseLeftUpCenter(control);
        }
    }
    [TargetType(typeof(Control))]
    public class ControlMouseOverAction : IAction
    {
        /// <summary>
        /// Mouse over to a control.
        /// </summary>
        /// <param name="control"></param>
        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {
            UserInput.MouseMove(frmElement, (int)frmElement.ActualWidth / 2, (int)frmElement.ActualHeight / 2);
        }
    }
    [TargetType(typeof(Control))]
    public class ControlMouseAwayAction : IAction
    {
        /// <summary>
        /// Mouse away from a control.
        /// </summary>
        /// <param name="control"></param>
        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {
            UserInput.MouseMove(1, 1);
        }
    }
    [TargetType(typeof(Control))]
    public class MouseMoveAction : IAction
    {
        /// <summary>
        /// Mouse move to a given x, y coordinates.
        /// </summary>
        /// <param name="actionParams"">[0]=x, [2]=y </param>
        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {
            if (actionParams != null && actionParams.Length >= 1 && frmElement != null)
            {
                int x = Int32.Parse(actionParams[0].ToString());
                int y = Int32.Parse(actionParams[1].ToString());
                if (actionParams[0] != null)
                {
                    //Mouse move to on a framework element.
                    UserInput.MouseMove(frmElement, x, y);
                    QueueHelper.WaitTillQueueItemsProcessed();
                }
            }
        }
    }
    #endregion

    #region Keystroke Action Members
    [TargetType(typeof(Control))]
    public class ControlPressEscapeAction : IAction
    {
        /// <summary>
        /// Press escape key on a control to test control IsCancel property.
        /// </summary>
        /// <param name="control"></param>
        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {
            UserInput.KeyPress("Escape");
        }
    }
    [TargetType(typeof(Control))]
    public class ControlPressTabAction : IAction
    {
        /// <summary>
        /// Press tab key.
        /// </summary>
        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {
            UserInput.KeyPress("Tab");
        }
    }
    [TargetType(typeof(Control))]
    public class ControlPressRightArrowAction : IAction
    {
        /// <summary>
        /// Press right arrow key.
        /// </summary>
        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {
            UserInput.KeyPress("Right");
        }
    }
    [TargetType(typeof(Control))]
    public class ControlPressDownArrowAction : IAction
    {
        /// <summary>
        /// Press down arrow key.
        /// </summary>
        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {

            if (!frmElement.IsKeyboardFocusWithin)
            {
                frmElement.Focus();
            }

            UserInput.KeyPress("Down");
        }
    }
    [TargetType(typeof(Control))]
    public class ControlPressUpArrowAction : IAction
    {
        /// <summary>
        /// Press up arrow key.
        /// </summary>
        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {
            if (!frmElement.IsKeyboardFocusWithin)
            {
                frmElement.Focus();
            }

            UserInput.KeyPress("Up");
        }
    }
    [TargetType(typeof(Control))]
    public class ControlSpaceKeyDownAction : IAction
    {
        /// <summary>
        /// Set the focus on a control. Then, space key down.
        /// </summary>
        /// <param name="control"></param>
        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {
            Control control = frmElement as Control;
            control.Focus();
            UserInput.KeyDown("Space");
        }
    }
    [TargetType(typeof(Control))]
    public class ControlPressEnterAction : IAction
    {
        /// <summary>
        /// Set the focus on a control. Then, press enter key.
        /// </summary>
        /// <param name="control"></param>
        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {
            Control control = frmElement as Control;
            control.Focus();
            UserInput.KeyPress("Enter");
        }
    }
    [TargetType(typeof(Control))]
    public class ControlPressSpaceAction : IAction
    {
        /// <summary>
        /// Set the focus on a control. Then, press space key.
        /// </summary>
        /// <param name="control"></param>
        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {
            Control control = frmElement as Control;
            if (control is Button)
            {
                Button button = control as Button;
                if (button.ClickMode == ClickMode.Release)
                {
                    UserInput.KeyPress("Space");
                }
                else if (button.ClickMode == ClickMode.Press)
                {
                    UserInput.KeyDown("Space");
                }
            }
            else
            {
                UserInput.KeyPress("Space");
            }
        }
    }
    [TargetType(typeof(Control))]
    public class ControlPressMinusAction : IAction
    {
        /// <summary>
        /// Press minus key on a control.
        /// </summary>
        /// <param name="control"></param>
        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {
            Control control = frmElement as Control;
            control.Focus();
            UserInput.KeyPress("Subtract");
        }
    }
    [TargetType(typeof(Control))]
    public class ControlPressPlusAction : IAction
    {
        /// <summary>
        /// Press plus key on a control.
        /// </summary>
        /// <param name="control"></param>
        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {
            Control control = frmElement as Control;
            control.Focus();
            UserInput.KeyPress("Add");
        }
    }
    [TargetType(typeof(Control))]
    public class ControlShiftF10Action : IAction
    {
        /// <summary>
        /// Pressing Shift+F10 keys on a control to open a ContextMenu.
        /// </summary>
        /// <param name="control"></param>
        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {
            UserInput.KeyDown("RightShift");
            UserInput.KeyPress("F10");
            UserInput.KeyUp("RightShift");
        }
    }
    [TargetType(typeof(Control))]
    public class ControlCtrlShiftF10Action : IAction
    {
        /// <summary>
        /// Pressing Shift+F10 keys on a control to close a ContextMenu.
        /// </summary>
        /// <param name="control"></param>
        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {
            UserInput.KeyDown("RightCtrl");
            UserInput.KeyDown("RightShift");
            UserInput.KeyPress("F10");
            UserInput.KeyUp("RightShift");
            UserInput.KeyUp("RightCtrl");
        }
    }
    [TargetType(typeof(Control))]
    public class Shift_Click_Action : IAction
    {
        /// <summary>
        /// Perform a Shift+Mouse click on the center of the slider to to move thumb to mouse pointer.
        /// </summary>
        /// <param name="frmElement"></param>
        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {
            UserInput.KeyDown("LeftShift");
            UserInput.MouseLeftClickCenter(frmElement as FrameworkElement);
            UserInput.KeyUp("LeftShift");
        }
    }
    [TargetType(typeof(Control))]
    public class ControlPressPageUpAction : IAction
    {
        /// <summary>
        /// Press PageUp key on a control.
        /// </summary>
        /// <param name="control"></param>
        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {
            Control control = frmElement as Control;
            control.Focus();
            UserInput.KeyPress("PageUp");
        }
    }
    [TargetType(typeof(Control))]
    public class ControlPressPageDownAction : IAction
    {
        /// <summary>
        /// Press PageDown key on a control.
        /// </summary>
        /// <param name="control"></param>
        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {
            Control control = frmElement as Control;
            control.Focus();
            UserInput.KeyPress("PageDown");
        }
    }
    [TargetType(typeof(Control))]
    public class ControlPressHomeAction : IAction
    {
        /// <summary>
        /// Press Home key on a control.
        /// </summary>
        /// <param name="control"></param>
        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {
            Control control = frmElement as Control;
            control.Focus();
            UserInput.KeyPress("Home");
        }
    }
    [TargetType(typeof(Control))]
    public class ControlPressEndAction : IAction
    {
        /// <summary>
        /// Press Home key on a control.
        /// </summary>
        /// <param name="control"></param>
        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {
            Control control = frmElement as Control;
            control.Focus();
            UserInput.KeyPress("End");
        }
    }
    [TargetType(typeof(Control))]
    public class ControlPressLeftArrowAction : IAction
    {
        /// <summary>
        /// Press Left arrow key.
        /// </summary>
        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {
            UserInput.KeyPress("Left");
        }
    }
    [TargetType(typeof(Control))]
    public class ControlKeyShiftDownAction : IAction
    {
        /// <summary>
        /// Perform a Shift Down action on any Control.
        /// </summary>
        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {
            UserInput.KeyDown("LeftShift");
        }
    }
    [TargetType(typeof(Control))]
    public class ControlKeyShiftUpAction : IAction
    {
        /// <summary>
        /// /// Perform a Shift Up action on any Control.
        /// </summary>
        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {
            UserInput.KeyUp("LeftShift");
        }
    }

    [TargetType(typeof(Control))]
    public class ControlPressRightAltDownAction : IAction
    {	
        /// <summary>
        /// /// Perform a press right Alt key down action on any Control.
        /// </summary>
            public void Do(FrameworkElement frmElement, params object[] actionParams)
        {
            UserInput.KeyDown("RightAlt");			
        }
    }

    [TargetType(typeof(Control))]
    public class ControlPressRightAltUpAction : IAction
    {
        /// <summary>
        /// /// Perform a press right Alt key up action on any Control.
        /// </summary>
        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {
            UserInput.KeyUp("RightAlt");
        }
    }
    [TargetType(typeof(Control))]
    public class ControlFocusAction : IAction
    {
        /// <summary>
        /// Focus action
        /// </summary>
        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {
            frmElement.Focus();
        }
    }

    [TargetType(typeof(Control))]
    public class ControlMouseWheelAction : IAction
    {
        /// <summary>
        /// MouseWheel action
        /// </summary>
        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {
            int x = 10;
            int y = 10;
            int scrollDistance = 20;

            if (actionParams != null)
            {
                if (actionParams.Length > 0)
                    x = Int32.Parse(actionParams[0].ToString());

                if (actionParams.Length > 1)
                    y = Int32.Parse(actionParams[1].ToString());

                if (actionParams.Length > 2)
                    scrollDistance = Int32.Parse(actionParams[2].ToString());
            }

            UserInput.MouseWheel(frmElement, x, y, scrollDistance);
        }
    }

    #endregion
}


