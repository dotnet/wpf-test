// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Test.Logging;
using System.Windows;
using Microsoft.Test.Xaml.Utilities;
using System.Windows.Controls;
using Microsoft.Test.Xaml.Types;

namespace Microsoft.Test.Xaml.Parser.Verifiers.Immutable
{
    /// <summary/>
    public static class ImmutableTypeSerialization_Verify
    {
        /// <summary>
        /// Verify xaml ImmutableTypeVerification.xaml.
        /// </summary>
        public static bool Verify(UIElement rootElement)
        {
            bool result = true;
            GlobalLog.LogStatus("Inside ParserVerifier.DoubleConverterVerify()...");
            GlobalLog.LogStatus("Verify that FrameworkElement can be found.");
            FrameworkElement fe = (FrameworkElement) LogicalTreeHelper.FindLogicalNode(rootElement, "frameworkElement");
            VerifyElement.VerifyBool(null != fe, true, ref result);
            //Verify Double.
            VerifyElement.VerifyBool(Double.NaN.Equals(fe.Width), true, ref result);
            VerifyElement.VerifyBool(Double.NaN.Equals(fe.Height), true, ref result);

            Button button1 = (FrameworkElement) LogicalTreeHelper.FindLogicalNode(rootElement, "button1") as Button;
            VerifyElement.VerifyBool(null != button1, true, ref result);
            VerifyElement.VerifyBool(Double.NaN.Equals(button1.Width), true, ref result);
            VerifyElement.VerifyBool(Double.NaN.Equals(button1.Height), true, ref result);

            Button button2 = (FrameworkElement) LogicalTreeHelper.FindLogicalNode(rootElement, "button2") as Button;
            VerifyElement.VerifyBool(null != button2, true, ref result);
            VerifyElement.VerifyDouble(button2.Width, 5.0, ref result);
            VerifyElement.VerifyDouble(button2.Height, 6.0, ref result);

            //Verify string.
            string stringVal = (string) fe.GetValue(FrameworkElementWithimmutableProperties.StringDPProperty);
            VerifyElement.VerifyString(stringVal, "String value", ref result);
            stringVal = ((FrameworkElementWithimmutableProperties) fe).ClrString;
            VerifyElement.VerifyString(stringVal, "clr String value", ref result);

            //Verify Int32
            int intVal = (int) fe.GetValue(FrameworkElementWithimmutableProperties.Int32DPProperty);
            VerifyElement.VerifyInt(intVal, 32, ref result);
            intVal = ((FrameworkElementWithimmutableProperties) fe).ClrInt32;
            VerifyElement.VerifyInt(intVal, 322, ref result);

            return result;
        }
    }
}
