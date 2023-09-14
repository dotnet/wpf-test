// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// This source is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
// All other rights reserved.

using System;
using System.CodeDom;
using System.Collections;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Automation;

namespace InternalHelper.Tests.Patterns
{
    using InternalHelper;
    using InternalHelper.Tests;
    using InternalHelper.Enumerations;
    using Microsoft.Test.WindowsUIAutomation;
    using Microsoft.Test.WindowsUIAutomation.Core;
    using Microsoft.Test.WindowsUIAutomation.TestManager;
    using Microsoft.Test.WindowsUIAutomation.Interfaces;

    /// -----------------------------------------------------------------------
    /// <summary></summary>
    /// -----------------------------------------------------------------------
    public class VirtualizedItemPatternWrapper : PatternObject
    {

        #region Variables

        /// -----------------------------------------------------------------------
        /// <summary></summary>
        /// -----------------------------------------------------------------------
        VirtualizedItemPattern _pattern;

        

        #endregion Variables

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal VirtualizedItemPatternWrapper(AutomationElement element, string testSuite, TestPriorities priority, TypeOfControl typeOfControl, TypeOfPattern typeOfPattern, string dirResults, bool testEvents, IApplicationCommands commands)
            :
            base(element, testSuite, priority, typeOfControl, typeOfPattern, dirResults, testEvents, commands)
        {
            Comment("Creating VirtualizedItemTests");

            _pattern = (VirtualizedItemPattern)GetPattern(m_le, m_useCurrent, VirtualizedItemPattern.Pattern);
            if (_pattern == null)
                ThrowMe(CheckType.IncorrectElementConfiguration, Helpers.PatternNotSupported + ": VirtualizedItemPattern");

        }

        #region Properties

        #endregion Properties

        #region Methods

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal void pattern_Realize(Type expectedException, CheckType checkType)
        {
            string call = "Realize()";

            try
            {
                Comment("Before " + call + ", BoundingRectagle = '" + m_le.Current.BoundingRectangle + "'");
                _pattern.Realize();
                Comment("After " + call + ", BoundingRectagle = '" + m_le.Current.BoundingRectangle + "'");
            }
            catch (Exception actualException)
            {
                if (Library.IsCriticalException(actualException))
                    throw;

                TestException(expectedException.GetType(), actualException.GetType(), call, checkType);
                return;
            }

            TestNoException(expectedException, call, checkType);
        }

        
        #endregion Methods

    }
}

namespace Microsoft.Test.WindowsUIAutomation.Tests.Patterns
{
    using InternalHelper;
    using InternalHelper.Tests;
    using InternalHelper.Tests.Patterns;
    using InternalHelper.Enumerations;
    using Microsoft.Test.WindowsUIAutomation;
    using Microsoft.Test.WindowsUIAutomation.Core;
    using Microsoft.Test.WindowsUIAutomation.TestManager;
    using Microsoft.Test.WindowsUIAutomation.Interfaces;

    /// -----------------------------------------------------------------------
    /// <summary></summary>
    /// -----------------------------------------------------------------------
    public sealed class VirtualizedItemTests : VirtualizedItemPatternWrapper
    {
        #region Member variables

        /// -------------------------------------------------------------------
        /// <summary>
        /// Used to identify the assembly name
        /// </summary>
        /// -------------------------------------------------------------------
        const string THIS = "VirtualizedItemTests";

        /// <summary></summary>
        public const string TestSuite = NAMESPACE + "." + THIS;

        /// <summary>Defines which UIAutomation Pattern this tests</summary>
        public static readonly string TestWhichPattern = VirtualizedItemPattern.Pattern != null ? Automation.PatternName(VirtualizedItemPattern.Pattern) : null;

        #endregion Member variables

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public VirtualizedItemTests(AutomationElement element, TestPriorities priority, string dirResults, bool testEvents, TypeOfControl typeOfControl, IApplicationCommands commands)
            :
            base(element, TestSuite, priority, typeOfControl, TypeOfPattern.VirtualizedItem, dirResults, testEvents, commands)
        {
        }

        #region Tests

        /// -------------------------------------------------------------------
        ///<summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("Realize.1",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,
            Author = "Microsoft Corp.",
            TestCaseType = TestCaseType.Generic,
            Description = new string[] 
            {
                "Verification: Call Realize"
            })]
        public void TestVirtualizedItem1(TestCaseAttribute testCase)
        {
            this.HeaderComment(testCase);

            // "Verification: Call Realize"
            pattern_Realize(null, CheckType.Verification);

            m_TestStep++;
        }
        #endregion
    }
}
