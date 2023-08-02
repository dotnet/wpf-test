// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Test;
using Microsoft.Test.Threading;
using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Serialization;
using Avalon.Test.CoreUI.Threading;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.CoreInput.Common;
using Microsoft.Test.Logging;

namespace Avalon.Test.CoreUI.PropertyEngine
{
    /// <summary>
    /// All property engine verifiers in this class
    /// </summary>
    public static partial class Verifiers
    {
        private static DateTime s_dtFirstCalled = DateTime.MinValue;
        private static int s_timeElapsed = 0;

        /// <summary>
        /// This Verify will keep XAML on screen for 5 seconds. 
        /// Useful for development and debugging.
        /// </summary>
        /// <param name="root">Root element of the visual tree</param>
        /// <returns>true when verification completes, false otherwise.</returns>
        public static bool Keep5Seconds(UIElement root)
        {
            return KeepAnySeconds(5);
        }

        /// <summary>
        /// This Verify will keep XAML on screen for 10 seconds. 
        /// Useful for development and debugging.
        /// </summary>
        /// <param name="root">Root element of the visual tree</param>
        /// <returns>true when verification completes, false otherwise.</returns>
        public static bool Keep10Seconds(UIElement root)
        {
            return KeepAnySeconds(10);
        }

        /// <summary>
        /// This Verify will keep XAML on screen for 30 seconds. 
        /// Useful for development and debugging.
        /// </summary>
        /// <param name="root">Root element of the visual tree</param>
        /// <returns>true when verification completes, false otherwise.</returns>
        public static bool Keep30Seconds(UIElement root)
        {
            return KeepAnySeconds(30);
        }

        /// <summary>
        /// Implementation detail for various KeepNSeconds Verifiers
        /// </summary>
        /// <param name="seconds">seconds to keep</param>
        /// <returns>true when verification completes, false otherwise.</returns>
        private static bool KeepAnySeconds(int seconds)
        {
            if (s_dtFirstCalled.Equals(DateTime.MinValue))
            {
                s_dtFirstCalled = DateTime.Now;
                return false;
            }
            else
            {
                TimeSpan ts = DateTime.Now - s_dtFirstCalled;
                if (ts.Seconds <= seconds)
                {
                    //In reality, it can only be <
                    if (s_timeElapsed != ts.Seconds)
                    {
                        s_timeElapsed = ts.Seconds;
                        CoreLogger.LogStatus("Currently at second : " + s_timeElapsed.ToString());
                    }
                    return false;
                }
                else
                {
                    //rest dtFirstCalled value
                    s_dtFirstCalled = DateTime.MinValue;
                    s_timeElapsed = 0;
                    return true;
                }
            }
        }

        /// <summary>
        /// Verification for BugRepro3.xaml
        /// </summary>
        /// <param name="root"></param>
        public static void BugRepro3Verifier(UIElement root)
        {
            FrameworkElement fe = root as FrameworkElement;
            VerifyElement.VerifyBool(null == fe, false);

            Button button1 = fe.FindName("MyButton1") as Button;
            VerifyElement.VerifyBool(null == button1, false);
            int childrenCount = VisualTreeHelper.GetChildrenCount(button1);
            VerifyElement.VerifyBool(childrenCount != 0, true);

            Button button2 = fe.FindName("MyButton2") as Button;
            VerifyElement.VerifyBool(null == button2, false);
            childrenCount = VisualTreeHelper.GetChildrenCount(button2);
            VerifyElement.VerifyInt(childrenCount, 0);

            Button button3 = fe.FindName("MyButton3") as Button;
            VerifyElement.VerifyBool(null == button3, false);
            childrenCount = VisualTreeHelper.GetChildrenCount(button3);
            VerifyElement.VerifyBool(childrenCount != 0, true);

        }

        /// <summary>
        /// Verification for BugRepro4.xaml
        /// </summary>
        /// <param name="root"></param>
        public static void BugRepro4Verifier(UIElement root)
        {
            TreeHelper.WaitForTimeManager();
            FrameworkElement fe = root as FrameworkElement;
            VerifyElement.VerifyBool(null == fe, false);

            Button button1 = fe.FindName("MyButton1") as Button;
            VerifyElement.VerifyBool(null == button1, false);

            Button button2 = (Button) button1.Template.FindName("tempButton", button1);
            VerifyElement.VerifyBool(null == button2, false);

            SolidColorBrush background = (SolidColorBrush)button2.Background;
            VerifyElement.VerifyBool(null == background, false);
            VerifyElement.VerifyColor(background.Color, Colors.LightBlue);            
        }

        /// <summary>
        /// Verification for ValueSource.
        /// </summary>
        /// <param name="root"></param>
        public static void ValueSourceTestVerifier(UIElement root)
        {
            Panel panel = root as Panel;

            // The xaml contains multiple test elements that have Background set
            // in different ways.

            foreach (UIElement child in panel.Children)
            {
                ValueSource valueSource = DependencyPropertyHelper.GetValueSource(child, Control.BackgroundProperty);
                bool isAnimated = valueSource.IsAnimated;
                bool isCoerced = valueSource.IsCoerced;
                bool isExpression = valueSource.IsExpression;

                // Verify IsAnimated, IsCoerced, and IsExpression.
                string sourceString = TreeHelper.GetNodeId(child);

                if (sourceString.IndexOf("IsAnimated") > -1 && !isAnimated)
                {
                    throw new Microsoft.Test.TestValidationException("ValueSource.IsAnimated=false, should be true.");
                }

                if (sourceString.IndexOf("IsCoerced") > -1 && !isCoerced)
                {
                    throw new Microsoft.Test.TestValidationException("ValueSource.IsCoerced=false, should be true.");
                }

                if (sourceString.IndexOf("IsExpression") > -1 && !isExpression)
                {
                    throw new Microsoft.Test.TestValidationException("ValueSource.IsExpression=false, should be true.");
                }

                // Verify BaseValueSource.
                sourceString = sourceString.Substring(0, sourceString.IndexOf("Set"));

                BaseValueSource expectedValueSource =
                    (BaseValueSource)Enum.Parse(typeof(BaseValueSource), sourceString);

                BaseValueSource actualValueSource = valueSource.BaseValueSource;

                if (expectedValueSource != actualValueSource)
                {
                    throw new Microsoft.Test.TestValidationException("BaseValueSource is not the expected value. Expected:" + expectedValueSource + ", Actual:" + actualValueSource);
                }

                // Verify equivalency.
                ValueSource valueSource2 = DependencyPropertyHelper.GetValueSource(child, Control.BackgroundProperty);

                if (!valueSource2.Equals(valueSource) || valueSource2 != valueSource || !(valueSource2 == valueSource))
                {
                    throw new Microsoft.Test.TestValidationException("Two equalivalent ValueSource instances are not equal.");
                }
            }
        }
    }
}
