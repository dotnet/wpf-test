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
    public class ItemContainerPatternWrapper : PatternObject
    {

        #region Variables

        /// -----------------------------------------------------------------------
        /// <summary></summary>
        /// -----------------------------------------------------------------------
        ItemContainerPattern _pattern;

       


        #endregion Variables

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal ItemContainerPatternWrapper(AutomationElement element, string testSuite, TestPriorities priority, TypeOfControl typeOfControl, TypeOfPattern typeOfPattern, string dirResults, bool testEvents, IApplicationCommands commands)
            :
            base(element, testSuite, priority, typeOfControl, typeOfPattern, dirResults, testEvents, commands)
        {
            Comment("Creating ItemContainerTests");

            _pattern = (ItemContainerPattern)GetPattern(m_le, m_useCurrent, ItemContainerPattern.Pattern);
            if (_pattern == null)
                ThrowMe(CheckType.IncorrectElementConfiguration, Helpers.PatternNotSupported + ": ItemContainerPattern");
                       
        }

        #region Properties

        #endregion Properties

        #region Methods

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal void pattern_FindFirstItemAndRealize(Type expectedException, CheckType checkType)
        {
            string call = "Realize()";
            AutomationElement testElement = null;
            try
            {
                testElement = _pattern.FindItemByProperty(null, null, "hello");
            }
            catch (Exception actualException)
            {
                if (Library.IsCriticalException(actualException))
                    throw;

                TestException(expectedException.GetType(), actualException.GetType(), call, checkType);
                return;
            }

            VirtualizedItemPattern _virtualizedItemPattern = null;
            Comment("Trying to se if VirtualizedItemPattern is available and if it is - we will de-virtualize found item");
            try
            {
                _virtualizedItemPattern = (VirtualizedItemPattern)testElement.GetCurrentPattern(VirtualizedItemPattern.Pattern) as VirtualizedItemPattern;
            }
            catch
            {
                Comment("VirtualizedItem pattern is not available. We'll skip de-virtualizing item");
            }
            if (_virtualizedItemPattern != null)
            {
                //Comment("Before " + call + ", BoundingRectagle = '" + testElement.Current.BoundingRectangle + "'");
                try
                {
                _virtualizedItemPattern.Realize();
                }
                catch (Exception actualException)
                {
                    if (Library.IsCriticalException(actualException))
                        throw;

                    TestException(expectedException.GetType(), actualException.GetType(), call, checkType);
                    return;
                }
                Comment("After " + call + ", BoundingRectagle = '" + testElement.Current.BoundingRectangle + "'");
            }
            
            TestNoException(expectedException, call, checkType);
        }


        internal void pattern_FindAllItemsAndRealize(Type expectedException, CheckType checkType)
        {
            string call = "Realize()";
            int count = 0;
            AutomationElement testElement = null;

            try
            {
                testElement = _pattern.FindItemByProperty(null, null, "@(*&#(#@!~");
            }
            catch (Exception actualException)
            {
                if (Library.IsCriticalException(actualException))
                    throw;

                TestException(expectedException.GetType(), actualException.GetType(), call, checkType);
                return;
            }

            VirtualizedItemPattern _virtualizedItemPattern = null;

            while ((testElement != null) && (count < 150))
            {
                Comment("Trying to see if VirtualizedItemPattern is available and if it is - we will de-virtualize found item");
                try
                {
                    _virtualizedItemPattern = (VirtualizedItemPattern)testElement.GetCurrentPattern(VirtualizedItemPattern.Pattern) as VirtualizedItemPattern;
                }
                catch
                {
                    Comment("VirtualizedItem pattern is not available. We'll skip de-virtualizing item");
                }
                if (_virtualizedItemPattern != null)
                {
                    try
                    {
                        _virtualizedItemPattern.Realize();
                    }
                    catch (Exception actualException)
                    {
                        if (Library.IsCriticalException(actualException))
                        {
                            throw;
                        }

                        TestException(expectedException.GetType(), actualException.GetType(), call, checkType);
                        return;
                    }
                    Comment("After " + call + ", BoundingRectagle = '" + testElement.Current.BoundingRectangle + "'");
                }
                try
                {
                    testElement = _pattern.FindItemByProperty(testElement, null, "@#$@%");
                }
                catch (Exception actualException)
                {
                    if (Library.IsCriticalException(actualException))
                    {
                        throw;
                    }

                    TestException(expectedException.GetType(), actualException.GetType(), call, checkType);
                    return;
                }
                count++;
            }
            Comment("This was the last item in the container or the test went through 150 items");

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
    public sealed class ItemContainerTests : ItemContainerPatternWrapper
    {
        #region Member variables

        /// -------------------------------------------------------------------
        /// <summary>
        /// Used to identify the assembly name
        /// </summary>
        /// -------------------------------------------------------------------
        const string THIS = "ItemContainerTests";

        /// <summary></summary>
        public const string TestSuite = NAMESPACE + "." + THIS;

        /// <summary>Defines which UIAutomation Pattern this tests</summary>
        public static readonly string TestWhichPattern = ItemContainerPattern.Pattern != null? Automation.PatternName(ItemContainerPattern.Pattern):null;

        #endregion Member variables

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public ItemContainerTests(AutomationElement element, TestPriorities priority, string dirResults, bool testEvents, TypeOfControl typeOfControl, IApplicationCommands commands)
            :
            base(element, TestSuite, priority, typeOfControl, TypeOfPattern.ItemContainer, dirResults, testEvents, commands)
        {
        }

        #region Tests

        /// -------------------------------------------------------------------
        ///<summary>Simple test to find first child and de-virtuialize it</summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("FindItemByProperty.1",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Generic,
            Description = new string[] 
            {
                "Precondition: Container has at least one child",
                "Verification: Call FindItemByPropety on first child",
                "Verification: Call Realize() on item"
            })]
        public void FindItemByProperty1(TestCaseAttribute testCase)
        {
            this.HeaderComment(testCase);

            pattern_FindFirstItemAndRealize(null, CheckType.Verification);

            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        ///<summary>Test to find all children in the container and de-virtuialize</summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("FindItemByPropertyAllChildren.1",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Generic,
            Description = new string[] 
            {
                "Precondition: Container has at least one child",
                "Verification: Call FindItemByPropety on first child",
                "Verification: Call Realize() on item",
                "Verification: Iterated through all children using the pattern"
            })]
        public void FindItemByPropertyAllChildren1(TestCaseAttribute testCase)
        {
            this.HeaderComment(testCase);

            pattern_FindAllItemsAndRealize(null, CheckType.Verification);

            m_TestStep++;
        }
        #endregion
    }
}
