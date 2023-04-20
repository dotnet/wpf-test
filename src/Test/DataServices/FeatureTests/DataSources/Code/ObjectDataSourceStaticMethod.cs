// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Xml;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.ComponentModel;
using Microsoft.Test;
using System.Windows.Markup;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
	/// <summary>
	/// <description>
    /// Verifies that you can bind to a static method on a static class with ObjectDataProvider. Provides coverage for regression bug.
	/// </description>
	/// <relatedBugs>

    /// </relatedBugs>
	/// </summary>
    [Test(2, "DataSources", "ObjectDataSourceStaticMethod")]
    public class ObjectDataSourceStaticMethod : XamlTest
    {
        public ObjectDataSourceStaticMethod()
            : base(@"ObjectDataSourceStaticMethod.xaml")
        {
            RunSteps += new TestStep(TestStaticMethod);
        }

 
        TestResult TestStaticMethod()
        {
            WaitForPriority(DispatcherPriority.SystemIdle);
            TextBlock methodBoundTextBlock = (TextBlock)((DockPanel)this.RootElement).Children[0];
            if (methodBoundTextBlock.Text != "Static Method")
            {
                LogComment("TextBlock text was expected to be: Static Method, actual: " + methodBoundTextBlock.Text);
                return TestResult.Fail;
            }
            return TestResult.Pass;
        }
    }

    public static class MyStaticClass
    {
        static MyStaticClass()
        {
        }

        public static string MyStaticMethod()
        {
            return "Static Method";
        }
    }
}
