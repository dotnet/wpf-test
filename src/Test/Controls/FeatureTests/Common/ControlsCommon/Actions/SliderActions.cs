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
using Microsoft.Test.RenderingVerification;
using System.Windows.Threading;

namespace Avalon.Test.ComponentModel.Actions
{
    public static class SliderActions
    {
        #region public methods

        /// <summary>
        /// Test Slider exceptions.
        /// </summary>
        /// <param name="slider"></param>
        public static void TestExceptions(Slider slider)
        {
            int delayValue = -1;
            string assignValueToDelayExceptionMessage = "'" + delayValue.ToString() + "'" + " is not a valid value for property 'Delay'.";
            string assignValueToIntervalExceptionMessage = "'" + delayValue.ToString() + "'" + " is not a valid value for property 'Interval'.";
            string assignValueToAutoToolTipPrecisionExceptionMessage = "'" + delayValue.ToString() + "'" + " is not a valid value for property 'AutoToolTipPrecision'.";

            // So we use new Exception() for the second parameter for now.
            // I confirmed that the property engine uses the ArgumentException constructor overload that 
            // takes only the localized message, which would keep ParamName as null. In the normal usage 
            // of ArgumentException within a method, the ParamName would be the non-localized name of a 
            // local method parameter, but in the GetValue case there is no context for that variable since 
            // GetValue doesn’t know where or how it was used. It shouldn’t be the DP name since that is 
            // essentially the method name (i.e. set_Interval), and always using “value” could be confusing 
            // in situations where someone called GetValue(myOtherLocalVariableName). So, not specifying the 
            // ParamName seems appropriate.
            ExceptionHelper.ExpectException(delegate()
            {
                slider.Delay = -1;
            }, new ArgumentException(assignValueToDelayExceptionMessage, new Exception()));

            ExceptionHelper.ExpectException(delegate()
            {
                slider.Interval = -1;
            }, new ArgumentException(assignValueToIntervalExceptionMessage, new Exception()));

            ExceptionHelper.ExpectException(delegate()
            {
                slider.AutoToolTipPrecision = -1;
            }, new ArgumentException(assignValueToAutoToolTipPrecisionExceptionMessage, new Exception()));
        }

        #endregion
    }
}


