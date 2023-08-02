using System;
using System.Windows;
using System.Windows.Controls;

using Microsoft.Test.Logging;

namespace Avalon.Test.ComponentModel.Validations
{
    /// <summary>
    /// Validate the event args in ScrollChangedEvent
    /// </summary>
    public class ScrollChangedEventArgsValidation : IValidation
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="validationParams"></param>
        /// <remarks validationParams[0]>Control</remarks>
        /// <remarks validationParams[1]>Expected values</remarks>
        /// <remarks validationParams[2]>EventArgs</remarks>
        /// <returns></returns>
        public bool Validate(params object[] validationParams)
        {

            ScrollChangedEventArgs eventArgs = validationParams[3] as ScrollChangedEventArgs;

            if (eventArgs == null)
            {
                TestLog.Current.LogStatus("FAIL - ScrollChangedEventArgs is null");
                return false;
            }

            object[] valiaditonParams = validationParams[1] as object[];

            if (valiaditonParams == null)
            {
                TestLog.Current.LogStatus("FAIL - Expected values, validationParams[1] is null");
                return false;
            }

            if (valiaditonParams.Length  != 6)
            {
                TestLog.Current.LogStatus("FAIL - Expected 6 values in validationParams[1], but actually there is only " + valiaditonParams.Length);
                return false;
            }

            double extentHeight = Double.Parse(valiaditonParams[0].ToString());
            double extentHeightChange = Double.Parse(valiaditonParams[1].ToString());
            double extentWidth = Double.Parse(valiaditonParams[2].ToString());
            double extentWidthChange = Double.Parse(valiaditonParams[3].ToString());
            double viewportWidth = Double.Parse(valiaditonParams[4].ToString());
            double viewportWidthChange = Double.Parse(valiaditonParams[5].ToString());


            if ((extentHeight != eventArgs.ExtentHeight) ||
                    (extentHeightChange != eventArgs.ExtentHeightChange) ||
                    (extentWidth != eventArgs.ExtentWidth) ||
                    (extentWidthChange != eventArgs.ExtentWidthChange) ||
                    // (viewportWidth != eventArgs.ViewportWidth) || // Don't validate viewportWidth because it is theme dependent
                    (viewportWidthChange != eventArgs.ViewportWidthChange))
            {

                TestLog.Current.LogStatus("FAIL - Expected and actual values differ ");

                TestLog.Current.LogStatus("ExtentHeight " + " Actual: " + eventArgs.ExtentHeight.ToString() + " Expected : " + extentHeight);
                TestLog.Current.LogStatus("ExtentHeightChange " + " Actual: " + eventArgs.ExtentHeightChange.ToString() + " Expected : " + extentHeightChange);
                TestLog.Current.LogStatus("ExtentWidth " + " Actual: " + eventArgs.ExtentWidth.ToString() + " Expected : " + extentWidth);
                TestLog.Current.LogStatus("ExtentWidthChange " + " Actual: " + eventArgs.ExtentWidthChange.ToString() + " Expected : " + extentWidthChange);
                TestLog.Current.LogStatus("ViewportWidth " + " Actual: " + eventArgs.ViewportWidth.ToString() + " Expected : " + viewportWidth);
                TestLog.Current.LogStatus("ViewportWidthChange " + " Actual: " + eventArgs.ViewportWidthChange.ToString() + " Expected : " + viewportWidthChange);

                return false;
            }

            return true;

        }
    }



}
