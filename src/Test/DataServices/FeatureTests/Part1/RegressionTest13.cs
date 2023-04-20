// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    ///Regression Test - net 4.5 breaks static bindings
    ///If not specify a source for a binding that uses a static property,
    ///WPF will throw InvalidOperationException when app targets to 4.5+,
    ///but nothing happend when app targets to 4.0.
    /// </description>
    /// </summary>
    // [DISABLED_WHILE_PORTING]
    // [Test(0, "Binding", "RegressionTest13", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
    public class RegressionTest13 : AvalonTest
    {
        #region Constructor

        public RegressionTest13()
        {
            InitializeSteps += new TestStep(Repro);
        }

        #endregion

        #region Test Steps

        private TestResult Repro()
        {
            Status("Repro");

            TextBox textbox = new TextBox();
            try
            {
                Binding binding = new Binding
                {
                    Source = typeof(MyItem),
                    Path = new PropertyPath(typeof(MyItem).GetProperty("Prop")),
                    Mode = BindingMode.OneWayToSource,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                };

                BindingOperations.SetBinding(textbox, TextBox.TextProperty, binding);
            }
            catch (InvalidOperationException ex)
            {
#if TESTBUILD_NET_ATLEAST_45  //InvalidOperationException is expected to be caught at .NET4.5+
                 LogComment(string.Format("Expected Exception is caught:\nDetails:{0} ", ex));
                 return TestResult.Pass;
#else                //InvalidOperationException is not expected to be caught bellow .NET4.0
                LogComment(string.Format("Unexpected Exception is caught:\nThis InvalidOperationException should just throw on .net 4.5 or .net4.5+/nDetails:{0} ", ex));
                return TestResult.Fail;
#endif
            }
            catch (Exception ex)
            {
                LogComment(string.Format("Unexpected Exception is caught:\nDetails:{0} ", ex));
                return TestResult.Fail;
            }

#if TESTBUILD_NET_ATLEAST_45
            LogComment("Binding.StaticSource cannot be set while using Binding.Source on .NET 4.5+");
            return TestResult.Fail;
#else
            return TestResult.Pass;
#endif
        }

        #endregion

        #region Helper Class

        public class MyItem
        {
            public static string Prop { get; set; }
        }

        #endregion
    }
}
