// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.PropertyEngine;
using Avalon.Test.CoreUI.UtilityHelper;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Avalon.Test.CoreUI.PropertyEngine.RefreshDependencyProperty
{
    /******************************************************************************
    * CLASS:          DependencyPropertyWithIntlName
    ******************************************************************************/
    /// <summary>
    /// Test: 9429 -- International Support for Property Names
    /// </summary>
    [Test(0, "PropertyEngine.DependencyProperty", TestCaseSecurityLevel.PartialTrust, "DependencyPropertyWithIntlName")]
    public class DependencyPropertyWithIntlName : RefreshDPBase
    {
       #region Constructor
        /******************************************************************************
        * Function:          DependencyPropertyWithIntlName Constructor
        ******************************************************************************/
        public DependencyPropertyWithIntlName()
        {
            RunSteps += new TestStep(StartTest);
        }
        #endregion


        #region Test Steps
        /******************************************************************************
        * Function:          StartTest
        ******************************************************************************/
        TestResult StartTest()
         {
            TestDP("Добро пожаловать", typeof(double), typeof(DependencyPropertyWithIntlName), 3.14, 3.14m);                   // Russian
            TestDP("ようこそ", typeof(Vector), typeof(DependencyPropertyWithIntlName), new Vector(0, 1), 0);                       // Japanese
            TestDP("어서 오십시오", typeof(Dock), typeof(DependencyPropertyWithIntlName), Dock.Right, new Vector(0, 1));             // Korean
            TestDP("Υποδοχή", typeof(Dock), typeof(DependencyPropertyWithIntlName), (Dock)int.MaxValue, 0);                    // Greek
            TestDP("欢迎", typeof(MenuItem), typeof(DependencyPropertyWithIntlName), new MenuItem(), new FrameworkElement());    // Simplified Chinese

           //Any failures are captured by Asserts in TestDP.
            return TestResult.Pass;
         }
         #endregion
    }
}
