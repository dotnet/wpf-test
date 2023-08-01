// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;

using System.IO;
using System.Threading;
using System.Windows.Threading;
using System.Windows;

using Avalon.Test.CoreUI.Common;

using Avalon.Test.CoreUI.Parser;
using System.Windows.Markup;
using System.Reflection;
using System.Collections;
using System.Xml;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Media;

using System.Runtime.InteropServices;
using Avalon.Test.CoreUI.Source;
using System.ComponentModel;
using System.Windows.Data;

using Microsoft.Test.Win32;
using Microsoft.Test.Serialization;

namespace Avalon.Test.CoreUI.Serialization
{
    /// <summary>
    /// A class to repro 

    public class BugRepro11
    {
        /// <summary>
        ///  Verifier for BugRepro11.xaml
        /// </summary>
        public static void Verify1(UIElement uie)
        {
            CoreLogger.LogStatus("Inside BugRepro11.Verify1()...");

            DockPanel myPanel = uie as DockPanel;

            if (null == myPanel)
            {
                throw new Microsoft.Test.TestValidationException("Should be DockPanel");
            }
        }

        private static void CheckProperty(string sourcestr, string propertystr, bool serialized)
        {
            if (sourcestr.Contains(propertystr) != serialized)
            {
                throw new Microsoft.Test.TestValidationException("Should " + (serialized?"" :"not ") + " serialize property: " + propertystr);
            }
        }
    }
}
