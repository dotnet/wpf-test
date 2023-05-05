// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Windows.Controls;
using System.Windows.Markup;
using Microsoft.Test.Logging;
using Microsoft.Test.Xaml.Utilities;

namespace Microsoft.Test.Xaml.MarkupCompiler
{
    public class RegressionIssue6
    {
        /// <summary>
        /// Verifies correct BAML behavior for 3.5 and 4.0 assemblies regarding
        /// MarkupExtension.ProvideValue in top-level deferrable content resources. 
        /// </summary>
        public void DeferredContentMarkupExtensionProvideValue()
        {
            object value;

            // 3.5
            Assembly bamlTestClasses35 = Assembly.Load("BamlTestClasses35");
            Assert.IsTrue(bamlTestClasses35.ImageRuntimeVersion.StartsWith("v2"), "bamlTestClasses35.ImageRuntimeVersion.StartsWith(v2)");

            // Test 1
            StackPanel stackPanel35 = (StackPanel)Activator.CreateInstance(bamlTestClasses35.GetType("BamlTestClasses35.DeferContMEPV"));

            // Test 2
            value = ((StackPanel)stackPanel35.FindName("Test2")).Resources["markupextension"];
            Assert.IsTrue(value == null, "stackPanel35.Test2.Resource[markupextension]==null");

            // Test 3
            value = ((StackPanel)stackPanel35.FindName("Test3")).Resources["markupextension1"];
            Assert.IsTrue(value == null, "stackPanel35.Test3.Resource[markupextension1]==null");
            value = ((StackPanel)stackPanel35.FindName("Test3")).Resources["markupextension2"];
            Assert.IsTrue(value == null, "stackPanel35.Test3.Resource[markupextension2]==null");

            // Test 4
            value = ((StackPanel)stackPanel35.FindName("Test4")).Resources["markupextension1"];
            Assert.IsTrue(value is NullExtension, "stackPanel35.Test4.Resource[markupextension1] is NullExtension");
            value = ((StackPanel)stackPanel35.FindName("Test4")).Resources["markupextension2"];
            Assert.IsTrue(value is NullExtension, "stackPanel35.Test4.Resource[markupextension2] is NullExtension");
            
            // Test 5 - Nested deferrable content (resources)
            value = ((StackPanel)stackPanel35.FindName("Test5")).Resources["button1"];
            value = (value as Button).Resources["markupextension1"];
            Assert.IsTrue(value == null, "inner markupextension == null");

            // 4.0
            Assembly bamlTestClasses40 = Assembly.Load("BamlTestClasses40");
            Assert.IsTrue(bamlTestClasses40.ImageRuntimeVersion.StartsWith("v4"));
            
            // Test 1
            StackPanel stackPanel40 = (StackPanel)Activator.CreateInstance(bamlTestClasses40.GetType("BamlTestClasses40.DeferContMEPV"));
            
            // Test 2
            value = ((StackPanel)stackPanel40.FindName("Test2")).Resources["markupextension"];
            Assert.IsTrue(value == null, "stackPanel40.Test2.Resource[markupextension]==null");

            // Test 3
            value = ((StackPanel)stackPanel40.FindName("Test3")).Resources["markupextension1"];
            Assert.IsTrue(value == null, "stackPanel40.Test3.Resource[markupextension1]==null");
            value = ((StackPanel)stackPanel40.FindName("Test3")).Resources["markupextension2"];
            Assert.IsTrue(value == null, "stackPanel40.Test3.Resource[markupextension2]==null");

            // Test 4
            value = ((StackPanel)stackPanel40.FindName("Test4")).Resources["markupextension1"];
            Assert.IsTrue(value == null, "stackPanel40.Test4.Resource[markupextension1]==null");
            value = ((StackPanel)stackPanel40.FindName("Test4")).Resources["markupextension2"];
            Assert.IsTrue(value == null, "stackPanel40.Test4.Resource[markupextension2]==null");

            // Test 5 - Nested deferrable content (resources)
            value = ((StackPanel)stackPanel40.FindName("Test5")).Resources["button1"];
            value = (value as Button).Resources["markupextension1"];
            Assert.IsTrue(value == null, "inner markupextension == null");
        }
    }
}
