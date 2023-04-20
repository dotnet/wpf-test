// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Test.Xaml.Utilities;

namespace Microsoft.Test.Xaml.Parser.Verifiers.ComponentModel
{
    /// <summary/>
    public static class RadioButtonListBVTComponentModel_Verify
    {
        /// <summary>
        /// RadioButtonListBVTComponentModel_Verify
        /// </summary>
        /// <param name="rootElement"></param>
        /// <returns></returns>
        public static bool Verify(UIElement rootElement)
        {
            bool             result = true;
            FrameworkElement fe     = rootElement as FrameworkElement;
            RadioButton      item   = fe.FindName("RADIOBUTTON_1") as RadioButton;
            VerifyElement.VerifyBool(null == item, false, ref result);
            VerifyElement.VerifyString((string) item.Content, "RadioButton1", ref result);


            item = fe.FindName("RADIOBUTTON_2") as RadioButton;
            VerifyElement.VerifyBool(null == item, false, ref result);
            VerifyElement.VerifyString((string) item.Content, "RadioButton2", ref result);

            item = fe.FindName("RADIOBUTTON_3") as RadioButton;
            VerifyElement.VerifyBool(null == item, false, ref result);
            VerifyElement.VerifyString((string) item.Content, "RadioButton3", ref result);

            return result;
        }
    }
}
