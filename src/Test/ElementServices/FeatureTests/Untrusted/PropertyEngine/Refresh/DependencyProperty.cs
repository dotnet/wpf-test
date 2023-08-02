// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Microsoft.Test;
using Microsoft.Test.Discovery;
using Avalon.Test.CoreUI;
using System.Threading;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.PropertyEngine;
using Microsoft.Test.Discovery;
using Microsoft.Test;
using Avalon.Test.CoreUI.UtilityHelper;

namespace Avalon.Test.CoreUI.PropertyEngine.RefreshDependencyProperty
{
    /// <summary>
    /// Use the command below to run this test with CoreTests.exe:
    /// coretests.exe /Class=Avalon.Test.CoreUI.PropertyEngine.RefreshDependencyProperty.DPRegTestCase  /Method=LabRunAll
    /// </summary>
    [TestDefaults]
    public class DPRegTestCase : DPTestCase
    {
        /// <summary>
        /// Used by the Test Engine
        /// </summary>
        [Test(0, @"PropertyEngine\DependencyProperty", TestCaseSecurityLevel.FullTrust, "DependencyProperty")]
        public void LabRunAll()
        {
            Utilities.StartRunAllTests("DependencyProperty");

            TestDP("DP1", typeof(double), typeof(DPRegTestCase), 3.14, 3.14m);                                  // builtin value type
            TestDP("dp1", typeof(Vector), typeof(DPRegTestCase), new Vector(0, 1), 0);                          // user value type, name is case sensitive
            TestDP("dp2", typeof(Dock), typeof(DPRegTestCase), Dock.Right, new Vector(0, 1));                   // enum
            TestDP("dp3", typeof(Dock), typeof(DPRegTestCase), (Dock)int.MaxValue, 0);                          // out-of-range enum special case
            TestDP("dp4", typeof(MenuItem), typeof(DPRegTestCase), new MenuItem(), new FrameworkElement());     // reference type
            TestDP("dp7", typeof(A), typeof(DPRegTestCase), new C(), new FrameworkElement());                   // inheritance

            TestBadDP(null, typeof(int), typeof(DPRegTestCase), typeof(ArgumentNullException));
            TestBadDP("dp5", null, typeof(DPRegTestCase), typeof(ArgumentNullException));
            TestBadDP("dp6", typeof(int), null, typeof(ArgumentNullException));
            TestBadDP("dp1", typeof(double), typeof(DPRegTestCase), typeof(ArgumentException));                 // same name

            RegressionTestBug52();

            Utilities.StopRunAllTests();
        }

        /// <summary>
        /// 



        private void RegressionTestBug52()
        {
            Utilities.PrintStatus("Regression test for bug 52");
            DependencyProperty Test1Property = DependencyProperty.RegisterAttached(
                "Test1", typeof(int), typeof(DPRegTestCase), new PropertyMetadata());
            Utilities.Assert((int)Test1Property.DefaultMetadata.DefaultValue == 0, "DefaltValue is auto-generated.");
        }

        private class A {}
        private class B : A {}
        private class C : B {}

    }

    /// <summary>
    /// Abstract class providing basic functionality for DP testing.
    /// </summary>
    public abstract class DPTestCase : TestCase
    {
        /// <summary>
        /// This method tests correct DP registration
        /// </summary>
        protected void TestDP(string name, Type propertyType, Type ownerType, object validValue, object invalidValue)
        {
            DependencyProperty dp = DependencyProperty.RegisterAttached(name, propertyType, ownerType);
            Utilities.PrintDependencyProperty(dp);

            Utilities.Assert(dp.GlobalIndex > _index, "dp.GlobalIndex increasing");

            if (propertyType.IsValueType)
            {
                Utilities.Assert(!dp.IsValidType(null), "For ValueType, dp.IsValidType(null) is false");
            }
            else
            {
                Utilities.Assert(dp.IsValidType(null), "For ReferenceType, dp.IsValidType(null) is true");
            }
            Utilities.Assert(!dp.IsValidType(DependencyProperty.UnsetValue), "dp.IsValidType(DependencyProperty.UnsetValue) is false");

            Utilities.Assert(dp.IsValidType(validValue), "dp.IsValidType(validValue) is true");
            Utilities.Assert(!dp.IsValidType(invalidValue), "dp.IsValidType(invalidValue) is false");

            _index = dp.GlobalIndex;
        }

        /// <summary>
        /// This method tests incorrect DP registration
        /// </summary>
        protected void TestBadDP(string name, Type propertyType, Type ownerType, Type expectedExceptionType)
        {
            try
            {
                DependencyProperty.RegisterAttached(name, propertyType, ownerType);
                Utilities.ExpectedExceptionNotReceived();
            }
            catch (Exception ex)
            {
                if (ex.GetType() != expectedExceptionType)
                {
                    throw;
                }
                Utilities.ExpectedExceptionReceived(ex);
            }
        }

        private int _index = -1;
    }
}
