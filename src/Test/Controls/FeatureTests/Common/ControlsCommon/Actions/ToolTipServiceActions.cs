using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Test.Logging;
using Avalon.Test.ComponentModel.Utilities;
using System.Reflection;
using System.Windows.Controls.Primitives;
using Microsoft.Test.Input;
using System.Windows.Input;
using System.Collections.Generic;
using Microsoft.Test;
using System.Diagnostics;

namespace Avalon.Test.ComponentModel.Actions
{
    public enum OpeningToolTipServiceScenario
    {
        Default,
        ShowDuration
    }

    public static class ToolTipServiceActions
    {
        #region public methods

        public static void OpeningAndClosingToolTip(FrameworkElement frameworkElement, OpeningToolTipServiceScenario openingToolTipServiceScenario)
        {
            // set tolerance to 20 Milliseconds.
            long tolerance = 20;
            long showDurationTime = 0;
            long actualTime;
            Stopwatch watch = null;

            if (openingToolTipServiceScenario == OpeningToolTipServiceScenario.ShowDuration)
            {
                showDurationTime = Convert.ToInt64(frameworkElement.GetValue(ToolTipService.ShowDurationProperty));
            }

            UserInput.MouseMove(frameworkElement, Convert.ToInt32(frameworkElement.ActualWidth / 2), Convert.ToInt32(frameworkElement.ActualHeight / 2));
            QueueHelper.WaitTillQueueItemsProcessed();

            ToolTipHelper.WaitForToolTipOpening(frameworkElement);

            if (openingToolTipServiceScenario == OpeningToolTipServiceScenario.ShowDuration)
            {
                watch = new Stopwatch();
                watch.Start();
            }

            ToolTip tooltip = ToolTipHelper.FindToolTip();
            if (tooltip == null)
            {
                throw new TestValidationException("ToolTip is closed after mouse over tooltip.");
            }

            switch (openingToolTipServiceScenario)
            {
                case OpeningToolTipServiceScenario.Default:
                    // tooltip should not close until explicitly instructed.
                    // The test can't wait forever, but 6 seconds is enough to simulate it
                    QueueHelper.WaitTillTimeout(TimeSpan.FromMilliseconds(6000));
                    tooltip = ToolTipHelper.FindToolTip();
                    if (tooltip == null)
                    {
                        throw new TestValidationException("ToolTip closed without explicit reason.");
                    }

                    // move the mouse cursor away from the control, and give the tooltip time
                    // to close so the next tooltip doesn't open immediately.
                    UserInput.MouseMove(300, 300);
                    QueueHelper.WaitTillTimeout(TimeSpan.FromMilliseconds(1000));
                    break;

                case OpeningToolTipServiceScenario.ShowDuration:
                    // tooltip should close when its ShowDuration expires
                    ToolTipHelper.WaitForToolTipClosing(frameworkElement);
                    actualTime = watch.ElapsedMilliseconds;
                    if ((actualTime - tolerance) > showDurationTime || (actualTime + tolerance) < showDurationTime)
                    {
                        string errorMessage = "Actual time " + actualTime.ToString() + " not equal to ShowDuration time " + showDurationTime.ToString() + " when tolerance is " + tolerance.ToString();
                        throw new TestValidationException(errorMessage);
                    }
                    break;
            }

            tooltip = ToolTipHelper.FindToolTip();
            if (tooltip != null)
            {
                throw new TestValidationException("ToolTip is opened after show duration time expired.");
            }

            // move the mouse cursor away from the control.
            UserInput.MouseMove(0, 0);
            QueueHelper.WaitTillQueueItemsProcessed();
        }

        public static void TestExceptions(FrameworkElement frameworkElement)
        {
            int invalidValue = -1;
            string initialShowDelayExceptionMessage = "'" + invalidValue.ToString() + "' is not a valid value for property 'InitialShowDelay'.";
            string betweenShowDelayExceptionMessage = "'" + invalidValue.ToString() + "' is not a valid value for property 'BetweenShowDelay'.";
            string showDurationExceptionMessage = "'" + invalidValue.ToString() + "' is not a valid value for property 'ShowDuration'.";

            ExceptionHelper.ExpectException(delegate()
            {
                ToolTipService.SetInitialShowDelay(frameworkElement, invalidValue);
            }, new ArgumentException(initialShowDelayExceptionMessage, new Exception()));

            ExceptionHelper.ExpectException(delegate()
            {
                ToolTipService.SetBetweenShowDelay(frameworkElement, invalidValue);
            }, new ArgumentException(betweenShowDelayExceptionMessage, new Exception()));

            ExceptionHelper.ExpectException(delegate()
            {
                ToolTipService.SetShowDuration(frameworkElement, invalidValue);
            }, new ArgumentException(showDurationExceptionMessage, new Exception()));
        }

        #endregion
    }
}


