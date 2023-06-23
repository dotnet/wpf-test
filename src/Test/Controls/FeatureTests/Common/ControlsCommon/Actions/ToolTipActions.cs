using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using Microsoft.Test;
using Microsoft.Test.Input;
using Microsoft.Test.RenderingVerification;

namespace Avalon.Test.ComponentModel.Actions
{
    public static class ToolTipActions
    {
        #region public methods

        public enum TriggerAction
        {
            Mouse,
            KeyboardFocus,
            KeyboardShortcut
        }

        public static void OpenAndCloseToolTip(FrameworkElement frameworkElement, System.Windows.Point offsetPoint, TriggerAction action = TriggerAction.Mouse)
        {
            ToolTip tooltip = frameworkElement.ToolTip as ToolTip;

            // open tooltip.
            RoutedEventArgs openedEventArgs = new RoutedEventArgs(System.Windows.Controls.ToolTip.OpenedEvent);
            openedEventArgs.Source = tooltip;
            EventTriggerCallback openToolTip = delegate
            {
                switch(action)
                {
                    case TriggerAction.Mouse:
                        UserInput.MouseMove(frameworkElement, Convert.ToInt32(frameworkElement.ActualWidth / 2), Convert.ToInt32(frameworkElement.ActualHeight / 2));
                        break;

                    case TriggerAction.KeyboardFocus:
                        System.Windows.Input.Keyboard.Focus(null);
                        QueueHelper.WaitTillQueueItemsProcessed();
                        while (System.Windows.Input.Keyboard.FocusedElement != frameworkElement)
                        {
                            UserInput.KeyPress(System.Windows.Input.Key.Tab.ToString());
                            QueueHelper.WaitTillQueueItemsProcessed();
                        }
                        break;

                    case TriggerAction.KeyboardShortcut:
                        UserInput.KeyDown(System.Windows.Input.Key.LeftCtrl.ToString());
                        UserInput.KeyDown(System.Windows.Input.Key.LeftShift.ToString());

                        UserInput.KeyPress(System.Windows.Input.Key.F10.ToString());

                        UserInput.KeyUp(System.Windows.Input.Key.LeftShift.ToString());
                        UserInput.KeyUp(System.Windows.Input.Key.LeftCtrl.ToString());
                        break;
                }

                WaitForToolTipOpened(tooltip);
            };

            EventHelper.ExpectEvent<RoutedEventArgs>(openToolTip, tooltip, "Opened", openedEventArgs);

            if (!tooltip.IsOpen)
            {
                throw new TestValidationException("ToolTip is closed.");
            }

            // validate offset.
            System.Drawing.Rectangle buttonRectangle = ImageUtility.GetScreenBoundingRectangle(frameworkElement);
            System.Drawing.Rectangle tooltipRectangle = ImageUtility.GetScreenBoundingRectangle(tooltip);
            System.Windows.Point actualOffsetPoint = new System.Windows.Point(tooltipRectangle.X - buttonRectangle.X, tooltipRectangle.Y - buttonRectangle.Y);
            
            if (!actualOffsetPoint.X.Equals(offsetPoint.X))
            {
                throw new TestValidationException(string.Format("The acutal X offset {0} is not equal to input offset{1}.", actualOffsetPoint.X, offsetPoint.X));
            }
            if (!actualOffsetPoint.Y.Equals(offsetPoint.Y))
            {
                throw new TestValidationException(string.Format("The acutal Y offset {0} is not equal to input offset {1}.", actualOffsetPoint.Y, offsetPoint.Y));
            }

            // close tooltip.
            RoutedEventArgs closedEventArgs = new RoutedEventArgs(System.Windows.Controls.ToolTip.ClosedEvent);
            closedEventArgs.Source = tooltip;
            EventTriggerCallback closeToolTip = delegate
            {
                switch (action)
                {
                    case TriggerAction.Mouse:
                        UserInput.MouseMove(0,0);
                        break;
                    case TriggerAction.KeyboardFocus:
                    case TriggerAction.KeyboardShortcut:
                        UserInput.KeyDown(System.Windows.Input.Key.LeftCtrl.ToString());
                        UserInput.KeyDown(System.Windows.Input.Key.LeftShift.ToString());

                        UserInput.KeyPress(System.Windows.Input.Key.F10.ToString());

                        UserInput.KeyUp(System.Windows.Input.Key.LeftShift.ToString());
                        UserInput.KeyUp(System.Windows.Input.Key.LeftCtrl.ToString());
                        break;
                }
                WaitForToolTipClosed(tooltip);
            };

            EventHelper.ExpectEvent<RoutedEventArgs>(closeToolTip, tooltip, "Closed", closedEventArgs);

            if (tooltip.IsOpen)
            {
                throw new TestValidationException("ToolTip is opened.");
            }
        }

        public static void TestExceptions(ToolTip tooltip)
        {
            string assignValueToPopupChildExceptionMessage = "'ToolTip' cannot have a logical or visual parent.";
            tooltip.IsOpen = true;
            QueueHelper.WaitTillQueueItemsProcessed();

            Popup popup = tooltip.Parent as Popup;
            if (popup == null)
            {
                throw new TestValidationException("Popup is null.");
            }

            ExceptionHelper.ExpectException(delegate()
            {
                popup.Child = null;
            }, new InvalidOperationException(assignValueToPopupChildExceptionMessage, new Exception()));

            ToolTip newToolTip = new ToolTip();
            newToolTip.Content = "New ToolTip.";

            ExceptionHelper.ExpectException(delegate()
            {
                popup.Child = newToolTip;
            }, new InvalidOperationException(assignValueToPopupChildExceptionMessage, new Exception()));
        }

        public static void WaitForToolTipOpened(ToolTip tooltip)
        {
            DispatcherFrame frame = new DispatcherFrame();
            tooltip.Opened += delegate(Object s, RoutedEventArgs e){ frame.Continue = false; };
            Dispatcher.PushFrame(frame);
        }
        
        public static void WaitForToolTipClosed(ToolTip tooltip)
        {
            DispatcherFrame frame = new DispatcherFrame();
            tooltip.Closed += delegate(Object s, RoutedEventArgs e){ frame.Continue = false; };
            Dispatcher.PushFrame(frame);
        }

        #endregion
    }
}


