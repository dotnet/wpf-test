// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Collections;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows.Controls;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Input;
using Microsoft.Test.DataServices;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.Verification;
using System.Windows.Markup;
using System.IO;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// </summary>
    public abstract class RegressionTest : StepsTest
    {
        public RegressionTest()
        {
            this.InitializeSteps += IdentifyFix;
            this.RunSteps += RunTest;
        }

        public abstract TestResult IdentifyFix();
        public abstract TestResult RunTest();

        public UIElement LoadContent(string xaml)
        {
            if (!File.Exists(xaml))
                throw new FileNotFoundException(string.Format("Could not find file '{0}'", xaml));

            object content = null;

            using (Stream stream = File.OpenRead(xaml))
            {
                content = XamlReader.Load(stream);
            }

            if (content != null && content is UIElement)
            {
                return content as UIElement;
            }

            throw new Exception("Cannot load null content.");
        }
    }

    public static class Helpers
    {
        public static System.Drawing.Point ToWinFormsPoint(this Point point)
        {
            return new System.Drawing.Point(Convert.ToInt32(point.X), Convert.ToInt32(point.Y));
        }
    }
}
