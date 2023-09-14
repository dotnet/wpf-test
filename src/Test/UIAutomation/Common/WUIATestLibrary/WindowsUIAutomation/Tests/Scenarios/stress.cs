// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
 * UIAutomation Stress tests cases
 * 
 * This file is used by the Avalon Stress UIAutomationStress project, and the 
 * UIVerify test framework.  Any changes to this file needs to be build in 
 * Client/AccessibleTech, and Client/WcpTests/Stress/Fremwork
 
********************************************************************/

using System;
using System.Windows.Automation;
using System.Windows;
using System.Collections;
using System.Reflection;
using System.Threading;
using System.Text;
using System.Diagnostics;

namespace Microsoft.Test.WindowsUIAutomation.Tests.Scenarios
{
    using InternalHelper;
    using InternalHelper.Tests;
    using InternalHelper.Enumerations;
    using Microsoft.Test.WindowsUIAutomation;
    using Microsoft.Test.WindowsUIAutomation.Core;
    using Microsoft.Test.WindowsUIAutomation.Interfaces;
    using Microsoft.Test.WindowsUIAutomation.TestManager;
    using System.Windows.Automation.Text;

    /// -----------------------------------------------------------------------
    /// <summary>
    /// Class that has the actual tests.  Call RunStressTests to start the diver that 
    /// drives the tests.
    /// </summary>
    /// -----------------------------------------------------------------------
    internal class StressTests
    {
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        const string THIS = "StressTests";

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public const string TestSuite = "Microsoft.Test.WindowsUIAutomation.Tests.Scenarios" + "." + THIS;

        #region Variables

        /// -------------------------------------------------------------------
        /// <summary>
        /// Random generator
        /// </summary>
        /// -------------------------------------------------------------------
        static Random s_rnd = new Random();

        Hashtable _methods = new Hashtable();

        #endregion Variables

        public static void Initialize(AutomationElement element)
        {
            //PopulateAutomationElementProperties(ref _automationProperties, ref _automationPatterns);           
        }

        #region Tests

        #region AutomationElement - DONE

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("AutomationElement", "AcceleratorKey", TestLab.PullData, TestWorks.Stable, Weight = 1)]
        public static void AutomationElementAcceleratorKey(AutomationElement element)
        {
            Dump("Current.AcceleratorKey", true, element);
            try
            {
                object obj = element.Current.AcceleratorKey;
            }
            catch (Exception exception)
            {
                VerifyException(element, exception, typeof(ElementNotAvailableException));
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("AutomationElement", "FindFirst", TestLab.PullData, TestWorks.Stable, Scenario = Scenario.General | Scenario.Narrator)]
        public static void AutomationElementFindFirst(AutomationElement element)
        {
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("AutomationElement", "GetClickablePoint", TestLab.PullData, TestWorks.NoWorking)]
        public static void AutomationElementGetClickablePoint(AutomationElement element)
        {
            Dump("GetClickablePoint()", true, element);
            try
            {
                Point obj = element.GetClickablePoint();
            }
            catch (Exception exception)
            {
                VerifyException(element, exception, 
                    typeof(ElementNotAvailableException),
                    typeof(NoClickablePointException)
                );
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("AutomationElement", "GetCurrentPattern", TestLab.PullData, TestWorks.Stable)]
        public static void AutomationElementGetCurrentPattern(AutomationElement element)
        {
            Dump("GetCurrentPattern(InvokePattern.Pattern)", true, element);
            try
            {
                object obj = element.GetCurrentPattern(InvokePattern.Pattern);
            }
            catch (Exception exception)
            {
                VerifyException(element, exception, 
                    typeof(ElementNotAvailableException),
                    typeof(InvalidOperationException) /*{"Unsupported Pattern"} */
                    );
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("AutomationElement", "GetCurrentPropertyValue", TestLab.PullData, TestWorks.Stable)]
        public static void AutomationElementGetCurrentPropertyValue(AutomationElement element)
        {
            ArrayList automationProperties = null;
            ArrayList automationPatterns = null;
            Random rnd = new Random((int)DateTime.Now.Ticks);
            PopulateAutomationElementProperties(ref automationProperties, ref automationPatterns);
            AutomationProperty property = (AutomationProperty)automationProperties[rnd.Next(automationProperties.Count - 1)];

            Dump("GetCurrentPropertyValue(" + property + ")", true, element);
            try
            {
                object obj = element.GetCurrentPropertyValue(property);
            }
            catch (Exception exception)
            {
                VerifyException(element, exception,
                  typeof(ElementNotAvailableException),
                  typeof(InvalidOperationException) /* {"Operation is not valid due to the current state of the object."}*/
                  );
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("AutomationElement", "GetHashCode", TestLab.PullData, TestWorks.Stable)]
        public static void AutomationElementGetHashCode(AutomationElement element)
        {
            Dump("GetHashCode()", true, element);
            try
            {
                object obj = element.GetHashCode();
            }
            catch (Exception exception)
            {
                VerifyException(element, exception, typeof(ElementNotAvailableException));
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("AutomationElement", "GetRuntimeId", TestLab.PullData, TestWorks.Stable)]
        public static void AutomationElementGetRuntimeId(AutomationElement element)
        {
            Dump("GetRuntimeId()", true, element);
            try
            {
                object obj = element.GetRuntimeId();
            }
            catch (Exception exception)
            {
                VerifyException(element, exception, typeof(ElementNotAvailableException));
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("AutomationElement", "GetSupportedPatterns", TestLab.PullData, TestWorks.Stable)]
        public static void AutomationElementGetSupportedPatterns(AutomationElement element)
        {
            Dump("GetSupportedPatterns()", true, element);
            try
            {
                AutomationPattern[] obj = element.GetSupportedPatterns();
            }
            catch (Exception exception)
            {
                VerifyException(element, exception, typeof(ElementNotAvailableException));
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("AutomationElement", "GetSupportedProperties", TestLab.PullData, TestWorks.Stable)]
        public static void AutomationElementGetSupportedProperties(AutomationElement element)
        {
            Dump("GetSupportedProperties()", true, element);
            try
            {
                AutomationProperty[] obj = element.GetSupportedProperties();
            }
            catch (Exception exception)
            {
                VerifyException(element, exception, typeof(ElementNotAvailableException));
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("AutomationElement", "GetType", TestLab.PullData, TestWorks.Stable)]
        public static void AutomationElementGetType(AutomationElement element)
        {
            Dump("GetType()", true, element);
            try
            {
                Type obj = element.GetType();
            }
            catch (Exception exception)
            {
                VerifyException(element, exception, typeof(ElementNotAvailableException));
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("AutomationElement", "SetFocus", TestLab.PushData, TestWorks.Stable)]
        public static void AutomationElementSetFocus(AutomationElement element)
        {
            Dump("SetFocus()", true, element);
            try
            {
                element.SetFocus();
            }
            catch (Exception exception)
            {
                VerifyException(element, exception,
                    typeof(ElementNotAvailableException),
                    typeof(ElementNotEnabledException),
                    typeof(InvalidOperationException) /* {"The target element could not receive focus."}*/
                    );
                    
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("AutomationElement", "ToString", TestLab.PullData, TestWorks.Stable)]
        public static void AutomationElementToString(AutomationElement element)
        {
            Dump("ToString()", true, element);
            try
            {
                object obj = element.ToString();
            }
            catch (Exception exception)
            {
                VerifyException(element, exception, typeof(ElementNotAvailableException));
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("AutomationElement", "TryGetClickablePoint", TestLab.PullData, TestWorks.NoWorking)]
        public static void AutomationElementTryGetClickablePoint(AutomationElement element)
        {
            Dump("TryGetClickablePoint()", true, element);
            Point pt = new Point();
            try
            {
                object obj = element.TryGetClickablePoint(out pt);
            }
            catch (Exception exception)
            {
                VerifyException(element, exception, typeof(ElementNotAvailableException));
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("AutomationElement", "TryGetCurrentPattern", TestLab.PullData, TestWorks.Stable)]
        public static void AutomationElementTryGetCurrentPattern(AutomationElement element)
        {
            ArrayList automationProperties = null;
            ArrayList automationPatterns = null;
            Random rnd = new Random((int)DateTime.Now.Ticks);
            PopulateAutomationElementProperties(ref automationProperties, ref automationPatterns);
            AutomationPattern pattern = (AutomationPattern)automationPatterns[rnd.Next(automationPatterns.Count)];
            object retPattern = null;

            Dump("TryGetCurrentPattern(" + pattern + ")", true, element);
            try
            {
                object val = element.TryGetCurrentPattern(pattern, out retPattern);
            }
            catch (Exception exception)
            {
                VerifyException(element, exception, typeof(ElementNotAvailableException));
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("AutomationElement", "FocusedElement", TestLab.PullData, TestWorks.NoWorking, 35)]
        public static void AutomationElementFocusedElement(AutomationElement element)
        {
            Dump("FocusedElement()", true, element);
            try
            {
                AutomationElement obj = AutomationElement.FocusedElement;
            }
            catch (Exception exception)
            {
                VerifyException(element, exception, typeof(ElementNotAvailableException));
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("AutomationElement", "FromHandle", TestLab.PullData, TestWorks.Stable)]
        public static void AutomationElementFromHandle(AutomationElement element)
        {
            AutomationElement obj;
            object handle = element.GetCurrentPropertyValue(AutomationElement.NativeWindowHandleProperty);

            Dump("FromHandle(" + handle + ")", true, element);
            if ((int)handle == 0)
                try
                {
                    obj = AutomationElement.FromHandle(IntPtr.Zero);
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception, typeof(ArgumentException));
                }
            else
                try
                {
                    obj = AutomationElement.FromHandle(new IntPtr((int)handle));
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception, typeof(ElementNotAvailableException));
                }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("AutomationElement", "FromPoint", TestLab.PullData, TestWorks.Stable)]
        public static void AutomationElementFromPoint(AutomationElement element)
        {
            Random rnd = new Random();
            Point pt = new Point(rnd.Next(0, 3000), rnd.Next(0, 3000));

            Dump("FromPoint(" + pt + ")", true, element);
            try
            {
                AutomationElement obj = AutomationElement.FromPoint(pt);
            }
            catch (Exception exception)
            {
                VerifyException(element, exception, typeof(NoClickablePointException), typeof(ElementNotAvailableException));
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("AutomationElement", "RootElement", TestLab.PullData, TestWorks.Stable)]
        public static void AutomationElementRootElement(AutomationElement element)
        {
            Dump("RootElement", true, element);
            AutomationElement obj = AutomationElement.RootElement;
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("AutomationElement", "ReferenceEquals", TestLab.PullData, TestWorks.Stable)]
        public static void AutomationElementReferenceEquals(AutomationElement element)
        {
            Random rnd = new Random();

            Dump("ReferenceEquals()", true, element);
            try
            {
                bool obj = AutomationElement.ReferenceEquals(element, AutomationElement.FromPoint(new Point(rnd.Next(100, 100), rnd.Next(100, 100))));
            }
            catch (Exception exception)
            {
                VerifyException(element, exception, typeof(ElementNotAvailableException));
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("AutomationElement", "Equals", TestLab.PullData, TestWorks.Stable)]
        public static void AutomationElementEquals(AutomationElement element)
        {
            Random rnd = new Random();

            Dump("Equals()", true, element);
            try
            {
                bool obj = AutomationElement.Equals(element, AutomationElement.FromPoint(new Point(rnd.Next(100, 100), rnd.Next(100, 100))));
            }
            catch (Exception exception)
            {
                VerifyException(element, exception, typeof(ElementNotAvailableException));
            }
        }

        #region TreeWalker

        #region TreeWalker.RawViewWalker

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("AutomationElement", "TreeWalkerRawViewWalkerGetFirstChild", TestLab.PullData, TestWorks.Stable)]
        public static void AutomationElementTreeWalkerRawViewWalkerGetFirstChild(AutomationElement element)
        {
            Dump("TreeWalker.RawViewWalker.GetFirstChild", true, element);
            try
            {
                AutomationElement tempElement = TreeWalker.RawViewWalker.GetFirstChild(element);
            }
            catch (Exception exception)
            {
                VerifyException(element, exception, typeof(ElementNotAvailableException));
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("AutomationElement", "TreeWalkerRawViewWalkerGetFirstChildCache", TestLab.PullData, TestWorks.Stable)]
        public static void AutomationElementTreeWalkerRawViewWalkerGetFirstChildCache(AutomationElement element)
        {
            Dump("TreeWalker.RawViewWalker.GetFirstChild(cache)", true, element);
            try
            {
                AutomationElement tempElement = TreeWalker.RawViewWalker.GetFirstChild(element, RandomCache());
            }
            catch (Exception exception)
            {
                VerifyException(element, exception, typeof(ElementNotAvailableException));
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("AutomationElement", "TreeWalkerRawViewWalkerGetHashCode", TestLab.PullData, TestWorks.Stable)]
        public static void AutomationElementTreeWalkerRawViewWalkerGetHashCode(AutomationElement element)
        {
            Dump("TreeWalker.RawViewWalker.GetHashCode()", true, element);
            try
            {
                int tempElement = TreeWalker.RawViewWalker.GetHashCode();
            }
            catch (Exception exception)
            {
                VerifyException(element, exception, typeof(ElementNotAvailableException));
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("AutomationElement", "TreeWalkerRawViewWalkerGetLastChild", TestLab.PullData, TestWorks.Stable)]
        public static void AutomationElementTreeWalkerRawViewWalkerGetLastChild(AutomationElement element)
        {
            Dump("TreeWalker.RawViewWalker.GetLastChild()", true, element);
            try
            {
                AutomationElement tempElement = TreeWalker.RawViewWalker.GetLastChild(element);
            }
            catch (Exception exception)
            {
                VerifyException(element, exception, typeof(ElementNotAvailableException));
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("AutomationElement", "TreeWalkerRawViewWalkerGetLastChildCache", TestLab.PullData, TestWorks.Stable)]
        public static void AutomationElementTreeWalkerRawViewWalkerGetLastChildCache(AutomationElement element)
        {
            Dump("TreeWalker.RawViewWalker.GetLastChild(cache)", true, element);
            try
            {
                AutomationElement tempElement = TreeWalker.RawViewWalker.GetLastChild(element, RandomCache());
            }
            catch (Exception exception)
            {
                VerifyException(element, exception, typeof(ElementNotAvailableException));
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("AutomationElement", "TreeWalkerRawViewWalkerGetNextSibling", TestLab.PullData, TestWorks.Stable)]
        public static void AutomationElementTreeWalkerRawViewWalkerGetNextSibling(AutomationElement element)
        {
            Dump("TreeWalker.RawViewWalker.GetNextSibling()", true, element);
            try
            {
                AutomationElement tempElement = TreeWalker.RawViewWalker.GetNextSibling(element);
            }
            catch (Exception exception)
            {
                VerifyException(element, exception, typeof(ElementNotAvailableException));
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("AutomationElement", "TreeWalkerRawViewWalkerGetNextSiblingCache", TestLab.PullData, TestWorks.Stable)]
        public static void AutomationElementTreeWalkerRawViewWalkerGetNextSiblingCache(AutomationElement element)
        {
            Dump("TreeWalker.RawViewWalker.GetNextSibling(cache)", true, element);
            try
            {
                AutomationElement tempElement = TreeWalker.RawViewWalker.GetNextSibling(element, RandomCache());
            }
            catch (Exception exception)
            {
                VerifyException(element, exception, typeof(ElementNotAvailableException));
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("AutomationElement", "TreeWalkerRawViewWalkerGetParent", TestLab.PullData, TestWorks.Stable)]
        public static void AutomationElementTreeWalkerRawViewWalkerGetParent(AutomationElement element)
        {
            Dump("TreeWalker.RawViewWalker.GetParent()", true, element);
            try
            {
                AutomationElement tempElement = TreeWalker.RawViewWalker.GetParent(element);
            }
            catch (Exception exception)
            {
                VerifyException(element, exception, typeof(ElementNotAvailableException));
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("AutomationElement", "TreeWalkerRawViewWalkerGetParentCache", TestLab.PullData, TestWorks.Stable)]
        public static void AutomationElementTreeWalkerRawViewWalkerGetParentCache(AutomationElement element)
        {
            Dump("TreeWalker.RawViewWalker.GetParent(cache)", true, element);
            try
            {
                AutomationElement tempElement = TreeWalker.RawViewWalker.GetParent(element, RandomCache());
            }
            catch (Exception exception)
            {
                VerifyException(element, exception, typeof(ElementNotAvailableException));
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("AutomationElement", "TreeWalkerRawViewWalkerGetPreviousSibling", TestLab.PullData, TestWorks.Stable)]
        public static void AutomationElementTreeWalkerRawViewWalkerGetPreviousSibling(AutomationElement element)
        {
            Dump("TreeWalker.RawViewWalker.GetPreviousSibling()", true, element);
            try
            {
                AutomationElement tempElement = TreeWalker.RawViewWalker.GetPreviousSibling(element);
            }
            catch (Exception exception)
            {
                VerifyException(element, exception, typeof(ElementNotAvailableException));
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("AutomationElement", "TreeWalkerRawViewWalkerGetPreviousSiblingCache", TestLab.PullData, TestWorks.Stable)]
        public static void AutomationElementTreeWalkerRawViewWalkerGetPreviousSiblingCache(AutomationElement element)
        {
            Dump("TreeWalker.RawViewWalker.GetPreviousSibling(cache)", true, element);
            try
            {
                AutomationElement tempElement = TreeWalker.RawViewWalker.GetPreviousSibling(element, RandomCache());
            }
            catch (Exception exception)
            {
                VerifyException(element, exception, typeof(ElementNotAvailableException));
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("AutomationElement", "TreeWalkerRawViewWalkerGetType", TestLab.PullData, TestWorks.Stable)]
        public static void AutomationElementTreeWalkerRawViewWalkerGetType(AutomationElement element)
        {
            Dump("TreeWalker.RawViewWalker.GetType()", true, element);
            try
            {
                object tempElement = TreeWalker.RawViewWalker.GetType();
            }
            catch (Exception exception)
            {
                VerifyException(element, exception, typeof(ElementNotAvailableException));
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("AutomationElement", "TreeWalkerRawViewWalkerNormalize", TestLab.PullData, TestWorks.Stable)]
        public static void AutomationElementTreeWalkerRawViewWalkerNormalize(AutomationElement element)
        {
            Dump("TreeWalker.RawViewWalker.Normalize()", true, element);
            try
            {
                AutomationElement tempElement = TreeWalker.RawViewWalker.Normalize(element);
            }
            catch (Exception exception)
            {
                VerifyException(element, exception, typeof(ElementNotAvailableException));
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("AutomationElement", "TreeWalkerRawViewWalkerToString",  TestLab.PullData, TestWorks.Stable)]
        public static void AutomationElementTreeWalkerRawViewWalkerToString(AutomationElement element)
        {
            Dump("TreeWalker.RawViewWalker.ToString()", true, element);
            try
            {
                object tempElement = TreeWalker.RawViewWalker.ToString();
            }
            catch (Exception exception)
            {
                VerifyException(element, exception, typeof(ElementNotAvailableException));
            }
        }

        #endregion TreeWalker.RawViewWalker

        #region TreeWalker.ContentViewWalker

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("AutomationElement", "TreeWalkerContentViewWalkerGetFirstChild", TestLab.PullData, TestWorks.Stable)]
        public static void AutomationElementTreeWalkerContentViewWalkerGetFirstChild(AutomationElement element)
        {
            Dump("TreeWalker.ContentViewWalker.GetFirstChild", true, element);
            try
            {
                AutomationElement tempElement = TreeWalker.ContentViewWalker.GetFirstChild(element);
            }
            catch (Exception exception)
            {
                VerifyException(element, exception, typeof(ElementNotAvailableException));
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("AutomationElement", "TreeWalkerContentViewWalkerGetFirstChildCache", TestLab.PullData, TestWorks.Stable)]
        public static void AutomationElementTreeWalkerContentViewWalkerGetFirstChildCache(AutomationElement element)
        {
            Dump("TreeWalker.ContentViewWalker.GetFirstChild(cache)", true, element);
            try
            {
                AutomationElement tempElement = TreeWalker.ContentViewWalker.GetFirstChild(element, RandomCache());
            }
            catch (Exception exception)
            {
                VerifyException(element, exception, typeof(ElementNotAvailableException));
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("AutomationElement", "TreeWalkerContentViewWalkerGetHashCode", TestLab.PullData, TestWorks.Stable)]
        public static void AutomationElementTreeWalkerContentViewWalkerGetHashCode(AutomationElement element)
        {
            Dump("TreeWalker.ContentViewWalker.GetHashCode()", true, element);
            try
            {
                int tempElement = TreeWalker.ContentViewWalker.GetHashCode();
            }
            catch (Exception exception)
            {
                VerifyException(element, exception, typeof(ElementNotAvailableException));
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("AutomationElement", "TreeWalkerContentViewWalkerGetLastChild", TestLab.PullData, TestWorks.Stable)]
        public static void AutomationElementTreeWalkerContentViewWalkerGetLastChild(AutomationElement element)
        {
            Dump("TreeWalker.ContentViewWalker.GetLastChild()", true, element);
            try
            {
                AutomationElement tempElement = TreeWalker.ContentViewWalker.GetLastChild(element);
            }
            catch (Exception exception)
            {
                VerifyException(element, exception, typeof(ElementNotAvailableException));
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("AutomationElement", "TreeWalkerContentViewWalkerGetLastChildCache", TestLab.PullData, TestWorks.Stable)]
        public static void AutomationElementTreeWalkerContentViewWalkerGetLastChildCache(AutomationElement element)
        {
            Dump("TreeWalker.ContentViewWalker.GetLastChild(cache)", true, element);
            try
            {
                AutomationElement tempElement = TreeWalker.ContentViewWalker.GetLastChild(element, RandomCache());
            }
            catch (Exception exception)
            {
                VerifyException(element, exception, typeof(ElementNotAvailableException));
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("AutomationElement", "TreeWalkerContentViewWalkerGetNextSibling", TestLab.PullData, TestWorks.Stable)]
        public static void AutomationElementTreeWalkerContentViewWalkerGetNextSibling(AutomationElement element)
        {
            Dump("TreeWalker.ContentViewWalker.GetNextSibling()", true, element);
            try
            {
                AutomationElement tempElement = TreeWalker.ContentViewWalker.GetNextSibling(element);
            }
            catch (Exception exception)
            {
                VerifyException(element, exception, typeof(ElementNotAvailableException));
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("AutomationElement", "TreeWalkerContentViewWalkerGetNextSiblingCache", TestLab.PullData, TestWorks.Stable)]
        public static void AutomationElementTreeWalkerContentViewWalkerGetNextSiblingCache(AutomationElement element)
        {
            Dump("TreeWalker.ContentViewWalker.GetNextSibling(cache)", true, element);
            try
            {
                AutomationElement tempElement = TreeWalker.ContentViewWalker.GetNextSibling(element, RandomCache());
            }
            catch (Exception exception)
            {
                VerifyException(element, exception, typeof(ElementNotAvailableException));
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("AutomationElement", "TreeWalkerContentViewWalkerGetParent", TestLab.PullData, TestWorks.Stable)]
        public static void AutomationElementTreeWalkerContentViewWalkerGetParent(AutomationElement element)
        {
            Dump("TreeWalker.ContentViewWalker.GetParent()", true, element);
            try
            {
                AutomationElement tempElement = TreeWalker.ContentViewWalker.GetParent(element);
            }
            catch (Exception exception)
            {
                VerifyException(element, exception, typeof(ElementNotAvailableException));
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("AutomationElement", "TreeWalkerContentViewWalkerGetParentCache", TestLab.PullData, TestWorks.Stable)]
        public static void AutomationElementTreeWalkerContentViewWalkerGetParentCache(AutomationElement element)
        {
            Dump("TreeWalker.ContentViewWalker.GetParent(cache)", true, element);
            try
            {
                AutomationElement tempElement = TreeWalker.ContentViewWalker.GetParent(element, RandomCache());
            }
            catch (Exception exception)
            {
                VerifyException(element, exception, typeof(ElementNotAvailableException));
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("AutomationElement", "TreeWalkerContentViewWalkerGetPreviousSibling", TestLab.PullData, TestWorks.Stable)]
        public static void AutomationElementTreeWalkerContentViewWalkerGetPreviousSibling(AutomationElement element)
        {
            Dump("TreeWalker.ContentViewWalker.GetPreviousSibling()", true, element);
            try
            {
                AutomationElement tempElement = TreeWalker.ContentViewWalker.GetPreviousSibling(element);
            }
            catch (Exception exception)
            {
                VerifyException(element, exception, typeof(ElementNotAvailableException));
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("AutomationElement", "TreeWalkerContentViewWalkerGetPreviousSiblingCache", TestLab.PullData, TestWorks.Stable)]
        public static void AutomationElementTreeWalkerContentViewWalkerGetPreviousSiblingCache(AutomationElement element)
        {
            Dump("TreeWalker.ContentViewWalker.GetPreviousSibling(cache)", true, element);
            try
            {
                AutomationElement tempElement = TreeWalker.ContentViewWalker.GetPreviousSibling(element, RandomCache());
            }
            catch (Exception exception)
            {
                VerifyException(element, exception, typeof(ElementNotAvailableException));
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("AutomationElement", "TreeWalkerContentViewWalkerGetType", TestLab.PullData, TestWorks.Stable)]
        public static void AutomationElementTreeWalkerContentViewWalkerGetType(AutomationElement element)
        {
            Dump("TreeWalker.ContentViewWalker.GetType()", true, element);
            try
            {
                object tempElement = TreeWalker.ContentViewWalker.GetType();
            }
            catch (Exception exception)
            {
                VerifyException(element, exception, typeof(ElementNotAvailableException));
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("AutomationElement", "TreeWalkerContentViewWalkerNormalize", TestLab.PullData, TestWorks.Stable)]
        public static void AutomationElementTreeWalkerContentViewWalkerNormalize(AutomationElement element)
        {
            Dump("TreeWalker.ContentViewWalker.Normalize()", true, element);
            try
            {
                AutomationElement tempElement = TreeWalker.ContentViewWalker.Normalize(element);
            }
            catch (Exception exception)
            {
                VerifyException(element, exception, typeof(ElementNotAvailableException));
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("AutomationElement", "TreeWalkerContentViewWalkerToString", TestLab.PullData, TestWorks.Stable)]
        public static void AutomationElementTreeWalkerContentViewWalkerToString(AutomationElement element)
        {
            Dump("TreeWalker.ContentViewWalker.ToString()", true, element);
            try
            {
                object tempElement = TreeWalker.ContentViewWalker.ToString();
            }
            catch (Exception exception)
            {
                VerifyException(element, exception, typeof(ElementNotAvailableException));
            }
        }

        #endregion TreeWalker.ControlView

        #region TreeWalker.ControlViewWalker

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("AutomationElement", "TreeWalkerControlViewWalkerGetFirstChild", TestLab.PullData, TestWorks.Stable)]
        public static void AutomationElementTreeWalkerControlViewWalkerGetFirstChild(AutomationElement element)
        {
            Dump("TreeWalker.ControlViewWalker.GetFirstChild", true, element);
            try
            {
                AutomationElement tempElement = TreeWalker.ControlViewWalker.GetFirstChild(element);
            }
            catch (Exception exception)
            {
                VerifyException(element, exception, typeof(ElementNotAvailableException));
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("AutomationElement", "TreeWalkerControlViewWalkerGetFirstChildCache", TestLab.PullData, TestWorks.Stable)]
        public static void AutomationElementTreeWalkerControlViewWalkerGetFirstChildCache(AutomationElement element)
        {
            Dump("TreeWalker.ControlViewWalker.GetFirstChild(cache)", true, element);
            try
            {
                AutomationElement tempElement = TreeWalker.ControlViewWalker.GetFirstChild(element, RandomCache());
            }
            catch (Exception exception)
            {
                VerifyException(element, exception, typeof(ElementNotAvailableException));
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("AutomationElement", "TreeWalkerControlViewWalkerGetHashCode", TestLab.PullData, TestWorks.Stable)]
        public static void AutomationElementTreeWalkerControlViewWalkerGetHashCode(AutomationElement element)
        {
            Dump("TreeWalker.ControlViewWalker.GetHashCode()", true, element);
            try
            {
                int tempElement = TreeWalker.ControlViewWalker.GetHashCode();
            }
            catch (Exception exception)
            {
                VerifyException(element, exception, typeof(ElementNotAvailableException));
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("AutomationElement", "TreeWalkerControlViewWalkerGetLastChild", TestLab.PullData, TestWorks.Stable)]
        public static void AutomationElementTreeWalkerControlViewWalkerGetLastChild(AutomationElement element)
        {
            Dump("TreeWalker.ControlViewWalker.GetLastChild()", true, element);
            try
            {
                AutomationElement tempElement = TreeWalker.ControlViewWalker.GetLastChild(element);
            }
            catch (Exception exception)
            {
                VerifyException(element, exception, typeof(ElementNotAvailableException));
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("AutomationElement", "TreeWalkerControlViewWalkerGetLastChildCache", TestLab.PullData, TestWorks.Stable)]
        public static void AutomationElementTreeWalkerControlViewWalkerGetLastChildCache(AutomationElement element)
        {
            Dump("TreeWalker.ControlViewWalker.GetLastChild(cache)", true, element);
            try
            {
                AutomationElement tempElement = TreeWalker.ControlViewWalker.GetLastChild(element, RandomCache());
            }
            catch (Exception exception)
            {
                VerifyException(element, exception, typeof(ElementNotAvailableException));
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("AutomationElement", "TreeWalkerControlViewWalkerGetNextSibling", TestLab.PullData, TestWorks.Stable)]
        public static void AutomationElementTreeWalkerControlViewWalkerGetNextSibling(AutomationElement element)
        {
            Dump("TreeWalker.ControlViewWalker.GetNextSibling()", true, element);
            try
            {
                AutomationElement tempElement = TreeWalker.ControlViewWalker.GetNextSibling(element);
            }
            catch (Exception exception)
            {
                VerifyException(element, exception, typeof(ElementNotAvailableException));
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("AutomationElement", "TreeWalkerControlViewWalkerGetNextSiblingCache", TestLab.PullData, TestWorks.Stable)]
        public static void AutomationElementTreeWalkerControlViewWalkerGetNextSiblingCache(AutomationElement element)
        {
            Dump("TreeWalker.ControlViewWalker.GetNextSibling(cache)", true, element);
            try
            {
                AutomationElement tempElement = TreeWalker.ControlViewWalker.GetNextSibling(element, RandomCache());
            }
            catch (Exception exception)
            {
                VerifyException(element, exception, typeof(ElementNotAvailableException));
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("AutomationElement", "TreeWalkerControlViewWalkerGetParent", TestLab.PullData, TestWorks.Stable)]
        public static void AutomationElementTreeWalkerControlViewWalkerGetParent(AutomationElement element)
        {
            Dump("TreeWalker.ControlViewWalker.GetParent()", true, element);
            try
            {
                AutomationElement tempElement = TreeWalker.ControlViewWalker.GetParent(element);
            }
            catch (Exception exception)
            {
                VerifyException(element, exception, typeof(ElementNotAvailableException));
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("AutomationElement", "TreeWalkerControlViewWalkerGetParentCache", TestLab.PullData, TestWorks.Stable)]
        public static void AutomationElementTreeWalkerControlViewWalkerGetParentCache(AutomationElement element)
        {
            Dump("TreeWalker.ControlViewWalker.GetParent(cache)", true, element);
            try
            {
                AutomationElement tempElement = TreeWalker.ControlViewWalker.GetParent(element, RandomCache());
            }
            catch (Exception exception)
            {
                VerifyException(element, exception, typeof(ElementNotAvailableException));
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("AutomationElement", "TreeWalkerControlViewWalkerGetPreviousSibling", TestLab.PullData, TestWorks.Stable)]
        public static void AutomationElementTreeWalkerControlViewWalkerGetPreviousSibling(AutomationElement element)
        {
            Dump("TreeWalker.ControlViewWalker.GetPreviousSibling()", true, element);
            try
            {
                AutomationElement tempElement = TreeWalker.ControlViewWalker.GetPreviousSibling(element);
            }
            catch (Exception exception)
            {
                VerifyException(element, exception, typeof(ElementNotAvailableException));
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("AutomationElement", "TreeWalkerControlViewWalkerGetPreviousSiblingCache", TestLab.PullData, TestWorks.Stable)]
        public static void AutomationElementTreeWalkerControlViewWalkerGetPreviousSiblingCache(AutomationElement element)
        {
            Dump("TreeWalker.ControlViewWalker.GetPreviousSibling(cache)", true, element);
            try
            {
                AutomationElement tempElement = TreeWalker.ControlViewWalker.GetPreviousSibling(element, RandomCache());
            }
            catch (Exception exception)
            {
                VerifyException(element, exception, typeof(ElementNotAvailableException));
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("AutomationElement", "TreeWalkerControlViewWalkerGetType", TestLab.PullData, TestWorks.Stable)]
        public static void AutomationElementTreeWalkerControlViewWalkerGetType(AutomationElement element)
        {
            Dump("TreeWalker.ControlViewWalker.GetType()", true, element);
            try
            {
                object tempElement = TreeWalker.ControlViewWalker.GetType();
            }
            catch (Exception exception)
            {
                VerifyException(element, exception, typeof(ElementNotAvailableException));
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("AutomationElement", "TreeWalkerControlViewWalkerNormalize", TestLab.PullData, TestWorks.Stable)]
        public static void AutomationElementTreeWalkerControlViewWalkerNormalize(AutomationElement element)
        {
            Dump("TreeWalker.ControlViewWalker.Normalize()", true, element);
            try
            {
                AutomationElement tempElement = TreeWalker.ControlViewWalker.Normalize(element);
            }
            catch (Exception exception)
            {
                VerifyException(element, exception, typeof(ElementNotAvailableException));
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("AutomationElement", "TreeWalkerControlViewWalkerToString", TestLab.PullData, TestWorks.Stable)]
        public static void AutomationElementTreeWalkerControlViewWalkerToString(AutomationElement element)
        {
            Dump("TreeWalker.ControlViewWalker.ToString()", true, element);
            try
            {
                object tempElement = TreeWalker.ControlViewWalker.ToString();
            }
            catch (Exception exception)
            {
                VerifyException(element, exception, typeof(ElementNotAvailableException));
            }
        }

        #endregion TreeWalker.ContentView

        #endregion TreeWalker

        #endregion AutomationElement

        #region DockTests DONE

        #region PushData

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("DockPattern", "SetDockPosition", TestLab.PushData, TestWorks.Stable)]
        public static void DockPatternSetDockPosition(AutomationElement element)
        {
            Random rnd = new Random((int)DateTime.Now.Ticks);
            object patternObj = null;
            DockPosition position = DockPosition.Bottom;

            if (GetPatternObject(element, DockPattern.Pattern, ref patternObj))
            {
                DockPattern pattern = (DockPattern)patternObj;
                Dump("DockPattern.SetDockPosition", true, element);
                try
                {
                    switch (rnd.Next(6))
                    {
                        case 0:
                            position = DockPosition.Bottom;
                            break;
                        case 1:
                            position = DockPosition.Fill;
                            break;
                        case 2:
                            position = DockPosition.Left;
                            break;
                        case 3:
                            position = DockPosition.None;
                            break;
                        case 4:
                            position = DockPosition.Right;
                            break;
                        case 5:
                            position = DockPosition.Top;
                            break;
                    }

                    pattern.SetDockPosition(position);
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception,
                        typeof(ElementNotAvailableException),
                        typeof(InvalidOperationException));
                }
            }
        }

        #endregion PushData

        #region PullData

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("DockPattern", "Equals", TestLab.PullData, TestWorks.Stable)]
        public static void DockPatternEquals(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, DockPattern.Pattern, ref patternObj))
            {
                DockPattern pattern = (DockPattern)patternObj;
                Dump("DockPattern.Equals", true, element);
                try
                {
                    object property = pattern.Equals(AutomationElement.RootElement);
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception, typeof(ElementNotAvailableException));
                }
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("DockPattern", "GetHashCode", TestLab.PullData, TestWorks.Stable)]
        public static void DockPatternGetHashCode(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, DockPattern.Pattern, ref patternObj))
            {
                DockPattern pattern = (DockPattern)patternObj;
                Dump("DockPattern.GetHashCode", true, element);
                try
                {
                    object property = pattern.GetHashCode();
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception, typeof(ElementNotAvailableException));
                }
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("DockPattern", "GetType", TestLab.PullData, TestWorks.Stable)]
        public static void DockPatternGetType(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, DockPattern.Pattern, ref patternObj))
            {
                DockPattern pattern = (DockPattern)patternObj;
                Dump("DockPattern.GetType", true, element);
                try
                {
                    object property = pattern.GetType();
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception, typeof(ElementNotAvailableException));
                }
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("DockPattern", "ToString", TestLab.PullData, TestWorks.Stable)]
        public static void DockPatternToString(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, DockPattern.Pattern, ref patternObj))
            {
                DockPattern pattern = (DockPattern)patternObj;
                Dump("DockPattern.ToString", true, element);
                try
                {
                    object property = pattern.ToString();
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception, typeof(ElementNotAvailableException));
                }
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("DockPattern", "CurrentDockPosition", TestLab.PullData, TestWorks.Stable)]
        public static void DockPatternCurrentDockPosition(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, DockPattern.Pattern, ref patternObj))
            {
                DockPattern pattern = (DockPattern)patternObj;
                Dump("DockPattern.Current.DockPosition", true, element);
                try
                {
                    object property = pattern.Current.DockPosition;
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception, typeof(ElementNotAvailableException));
                }
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("DockPattern", "CurrentEquals", TestLab.PullData, TestWorks.Stable)]
        public static void DockPatternCurrentEquals(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, DockPattern.Pattern, ref patternObj))
            {
                DockPattern pattern = (DockPattern)patternObj;
                Dump("DockPattern.Current.Equals", true, element);
                try
                {
                    object property = pattern.Current.Equals(AutomationElement.RootElement);
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception, typeof(ElementNotAvailableException));
                }
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("DockPattern", "CurrentGetHashCode", TestLab.PullData, TestWorks.Stable)]
        public static void DockPatternCurrentGetHashCode(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, DockPattern.Pattern, ref patternObj))
            {
                DockPattern pattern = (DockPattern)patternObj;
                Dump("DockPattern.Current.GetHashCode", true, element);
                try
                {
                    object property = pattern.Current.GetHashCode();
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception, typeof(ElementNotAvailableException));
                }
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("DockPattern", "CurrentGetType", TestLab.PullData, TestWorks.Stable)]
        public static void DockPatternCurrentGetType(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, DockPattern.Pattern, ref patternObj))
            {
                DockPattern pattern = (DockPattern)patternObj;
                Dump("DockPattern.Current.GetType", true, element);
                try
                {
                    object property = pattern.Current.GetType();
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception, typeof(ElementNotAvailableException));
                }
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("DockPattern", "CurrentToString", TestLab.PullData, TestWorks.Stable)]
        public static void DockPatternCurrentToString(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, DockPattern.Pattern, ref patternObj))
            {
                DockPattern pattern = (DockPattern)patternObj;
                Dump("DockPattern.Current.ToString", true, element);
                try
                {
                    object property = pattern.Current.ToString();
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception, typeof(ElementNotAvailableException));
                }
            }
        }

        #endregion PullData

        #endregion DockTests

        #region ExpandCollapse DONE

        #region PushData

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("ExpandCollapsePattern", "Expand", TestLab.PushData, TestWorks.Stable)]
        public static void ExpandCollapsePatternExpand(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, ExpandCollapsePattern.Pattern, ref patternObj))
            {
                ExpandCollapsePattern pattern = (ExpandCollapsePattern)patternObj;
                Dump("ExpandCollapsePattern.Expand()", true, element);
                try
                {
                    pattern.Expand();
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception,
                        typeof(ElementNotAvailableException),
                        typeof(ElementNotEnabledException),
                        typeof(InvalidOperationException) /*{"Operation cannot be performed."} */);
                }
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("ExpandCollapsePattern", "Collapse", TestLab.PushData, TestWorks.Stable)]
        public static void ExpandCollapsePatternCollapse(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, ExpandCollapsePattern.Pattern, ref patternObj))
            {
                ExpandCollapsePattern pattern = (ExpandCollapsePattern)patternObj;
                Dump("ExpandCollapsePattern.Collapse()", true, element);
                try
                {
                    pattern.Collapse();
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception,
                        typeof(ElementNotAvailableException),
                        typeof(ElementNotEnabledException),
                        typeof(InvalidOperationException) /* {"Operation cannot be performed."} */);
                }
            }
        }

        #endregion PushData

        #region PullData

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("ExpandCollapsePattern", "Equals", TestLab.PullData, TestWorks.Stable)]
        public static void ExpandCollapsePatternEquals(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, ExpandCollapsePattern.Pattern, ref patternObj))
            {
                ExpandCollapsePattern pattern = (ExpandCollapsePattern)patternObj;
                Dump("ExpandCollapsePattern.Equals()", true, element);
                try
                {
                    pattern.Equals(AutomationElement.RootElement);
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception,
                        typeof(ElementNotAvailableException));
                }
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("ExpandCollapsePattern", "GetHashCode", TestLab.PullData, TestWorks.Stable)]
        public static void ExpandCollapsePatternGetHashCode(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, ExpandCollapsePattern.Pattern, ref patternObj))
            {
                ExpandCollapsePattern pattern = (ExpandCollapsePattern)patternObj;
                Dump("ExpandCollapsePattern.GetHashCode()", true, element);
                try
                {
                    pattern.GetHashCode();
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception,
                        typeof(ElementNotAvailableException));
                }
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("ExpandCollapsePattern", "GetType", TestLab.PullData, TestWorks.Stable)]
        public static void ExpandCollapsePatternGetType(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, ExpandCollapsePattern.Pattern, ref patternObj))
            {
                ExpandCollapsePattern pattern = (ExpandCollapsePattern)patternObj;
                Dump("ExpandCollapsePattern.GetType()", true, element);
                try
                {
                    pattern.GetType();
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception,
                        typeof(ElementNotAvailableException));
                }
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("ExpandCollapsePattern", "ToString", TestLab.PullData, TestWorks.Stable)]
        public static void ExpandCollapsePatternToString(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, ExpandCollapsePattern.Pattern, ref patternObj))
            {
                ExpandCollapsePattern pattern = (ExpandCollapsePattern)patternObj;
                Dump("ExpandCollapsePattern.ToString()", true, element);
                try
                {
                    pattern.ToString();
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception,
                        typeof(ElementNotAvailableException),
                        typeof(ElementNotEnabledException));
                }
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("ExpandCollapsePattern", "CurrentEquals", TestLab.PullData, TestWorks.Stable)]
        public static void ExpandCollapsePatternCurrentEquals(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, ExpandCollapsePattern.Pattern, ref patternObj))
            {
                ExpandCollapsePattern pattern = (ExpandCollapsePattern)patternObj;
                Dump("ExpandCollapsePattern.Current.Equals()", true, element);
                try
                {
                    pattern.Equals(AutomationElement.RootElement); 
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception,
                        typeof(ElementNotAvailableException));
                }
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("ExpandCollapsePattern", "CurrentExpandCollapseState", TestLab.PullData, TestWorks.Stable)]
        public static void ExpandCollapsePatternCurrentExpandCollapseState(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, ExpandCollapsePattern.Pattern, ref patternObj))
            {
                ExpandCollapsePattern pattern = (ExpandCollapsePattern)patternObj;
                Dump("ExpandCollapsePattern.Current.ExpandCollapseState", true, element);
                try
                {
                    object obj = pattern.Current.ExpandCollapseState;
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception,
                        typeof(ElementNotAvailableException));
                }
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("ExpandCollapsePattern", "CurrentGetHashCode", TestLab.PullData, TestWorks.Stable)]
        public static void ExpandCollapsePatternCurrentGetHashCode(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, ExpandCollapsePattern.Pattern, ref patternObj))
            {
                ExpandCollapsePattern pattern = (ExpandCollapsePattern)patternObj;
                Dump("ExpandCollapsePattern.Current.GetHashCode()", true, element);
                try
                {
                    object obj = pattern.Current.GetHashCode();
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception,
                        typeof(ElementNotAvailableException));
                }
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("ExpandCollapsePattern", "CurrentToString", TestLab.PullData, TestWorks.Stable)]
        public static void ExpandCollapsePatternCurrentToString(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, ExpandCollapsePattern.Pattern, ref patternObj))
            {
                ExpandCollapsePattern pattern = (ExpandCollapsePattern)patternObj;
                Dump("ExpandCollapsePattern.Current.ToString()", true, element);
                try
                {
                    object obj = pattern.Current.ToString();
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception,
                        typeof(ElementNotAvailableException));
                }
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("ExpandCollapsePattern", "CurrentGetType", TestLab.PullData, TestWorks.Stable)]
        public static void ExpandCollapsePatternCurrentGetType(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, ExpandCollapsePattern.Pattern, ref patternObj))
            {
                ExpandCollapsePattern pattern = (ExpandCollapsePattern)patternObj;
                Dump("ExpandCollapsePattern.Current.GetType()", true, element);
                try
                {
                    object obj = pattern.Current.GetType();
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception,
                        typeof(ElementNotAvailableException));
                }
            }
        }

        #endregion PullData

        #endregion ExpandCollapse

        #region InvokePattern DONE

        static string[] s_badInvokes = new string[] { "close" };

        #region PushData

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("InvokePattern", "Invoke", TestLab.PushData, TestWorks.NoWorking, 1447959)]
        public static void InvokePatternInvoke(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, InvokePattern.Pattern, ref patternObj))
            {
                InvokePattern pattern = (InvokePattern)patternObj;
                Dump("InvokePattern.Invoke()", true, element);
                try
                {
                    if (Array.IndexOf(s_badInvokes, element.Current.Name.ToLower()) == -1)
                    {
                        pattern.Invoke();
                    }
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception,
                        typeof(ElementNotAvailableException),
                        typeof(ElementNotEnabledException));
                }
            }
        }

        #endregion PushData

        #endregion InvokePattern

        #region ScrollItem DONE

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("ScrollItemPattern", "Equals", TestLab.PushData, TestWorks.Stable)]
        public static void ScrollItemPatternEquals(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, ScrollItemPattern.Pattern, ref patternObj))
            {
                ScrollItemPattern pattern = (ScrollItemPattern)patternObj;
                Dump("ScrollItemPattern.Equals", true, element);
                try
                {
                    object property = pattern.Equals(AutomationElement.RootElement);
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception, typeof(ElementNotAvailableException));
                }
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("ScrollItemPattern", "GetHashCode", TestLab.PushData |TestLab.PullData, TestWorks.Stable)]
        public static void ScrollItemPatternGetHashCode(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, ScrollItemPattern.Pattern, ref patternObj))
            {
                ScrollItemPattern pattern = (ScrollItemPattern)patternObj;
                Dump("ScrollItemPattern.GetHashCode", true, element);
                try
                {
                    object property = pattern.GetHashCode();
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception, typeof(ElementNotAvailableException));
                }
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("ScrollItemPattern", "GetType", TestLab.PushData |TestLab.PullData, TestWorks.Stable)]
        public static void ScrollItemPatternGetType(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, ScrollItemPattern.Pattern, ref patternObj))
            {
                ScrollItemPattern pattern = (ScrollItemPattern)patternObj;
                Dump("ScrollItemPattern.GetType", true, element);
                try
                {
                    object property = pattern.GetType();
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception, typeof(ElementNotAvailableException));
                }
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("ScrollItemPattern", "ScrollIntoView", TestLab.PushData, TestWorks.Stable)]
        public static void ScrollItemPatternScrollIntoView(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, ScrollItemPattern.Pattern, ref patternObj))
            {
                ScrollItemPattern pattern = (ScrollItemPattern)patternObj;
                Dump("ScrollItemPattern.ScrollIntoView", true, element);
                try
                {
                    pattern.ScrollIntoView();
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception,
                        typeof(InvalidOperationException),      /* {"Operation cannot be performed."} ... modal dialog is displayed*/
                        typeof(ElementNotAvailableException),
                        typeof(ElementNotEnabledException)      /*{"The operation is not allowed on a non-enabled element"} */
                        );
                }
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("ScrollItemPattern", "ToString", TestLab.PushData |TestLab.PullData, TestWorks.Stable)]
        public static void ScrollItemPatternToString(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, ScrollItemPattern.Pattern, ref patternObj))
            {
                ScrollItemPattern pattern = (ScrollItemPattern)patternObj;
                Dump("ScrollItemPattern.ToString", true, element);
                try
                {
                    object property = pattern.ToString();
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception, typeof(ElementNotAvailableException));
                }
            }
        }

        #endregion ScrollItem

        #region Scroll DONE

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("ScrollPattern", "Equals", TestLab.PushData |TestLab.PullData, TestWorks.Stable)]
        public static void ScrollPatternEquals(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, ScrollPattern.Pattern, ref patternObj))
            {
                ScrollPattern pattern = (ScrollPattern)patternObj;
                Dump("ScrollPattern.Equals", true, element);
                try
                {
                    object property = pattern.Equals(AutomationElement.RootElement);
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception, typeof(ElementNotAvailableException));
                }
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("ScrollPattern", "GetHashCode", TestLab.PushData |TestLab.PullData, TestWorks.Stable)]
        public static void ScrollPatternGetHashCode(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, ScrollPattern.Pattern, ref patternObj))
            {
                ScrollPattern pattern = (ScrollPattern)patternObj;
                Dump("ScrollPattern.GetHashCode", true, element);
                try
                {
                    object property = pattern.GetHashCode();
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception, typeof(ElementNotAvailableException));
                }
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("ScrollPattern", "GetType", TestLab.PushData |TestLab.PullData, TestWorks.Stable)]
        public static void ScrollPatternGetType(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, ScrollPattern.Pattern, ref patternObj))
            {
                ScrollPattern pattern = (ScrollPattern)patternObj;
                Dump("ScrollPattern.GetType", true, element);
                try
                {
                    object property = pattern.GetType();
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception, typeof(ElementNotAvailableException));
                }
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("ScrollPattern", "Scroll", TestLab.PushData, TestWorks.Stable)]
        public static void ScrollPatternScroll(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, ScrollPattern.Pattern, ref patternObj))
            {

                ScrollPattern pattern = (ScrollPattern)patternObj;

                Dump("ScrollPattern.Scroll", true, element);
                try
                {
                    pattern.Scroll(GetRandomScrollAmount(), GetRandomScrollAmount());
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception,
                        typeof(ElementNotAvailableException),
                        typeof(ElementNotEnabledException), /*  {"The operation is not allowed on a non-enabled element"} */
                        typeof(InvalidOperationException)   /*  {"Operation cannot be performed."} */
                        );
                }
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("ScrollPattern", "ScrollHorizontal", TestLab.PushData, TestWorks.Stable)]
        public static void ScrollPatternScrollHorizontal(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, ScrollPattern.Pattern, ref patternObj))
            {
                ScrollPattern pattern = (ScrollPattern)patternObj;
                Dump("ScrollPattern.ScrollHorizontal", true, element);
                try
                {
                    pattern.ScrollHorizontal(GetRandomScrollAmount());
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception,
                        typeof(ElementNotAvailableException),
                        typeof(ElementNotEnabledException), /*  {"The operation is not allowed on a non-enabled element"} */
                        typeof(InvalidOperationException)   /*  {"Operation cannot be performed."} */
                        );
                }
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("ScrollPattern", "ScrollVertical", TestLab.PushData, TestWorks.Stable)]
        public static void ScrollPatternScrollVertical(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, ScrollPattern.Pattern, ref patternObj))
            {
                ScrollPattern pattern = (ScrollPattern)patternObj;
                Dump("ScrollPattern.ScrollVertical", true, element);
                try
                {
                    pattern.ScrollVertical(GetRandomScrollAmount());
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception,
                        typeof(ElementNotAvailableException),
                        typeof(ElementNotEnabledException), /*  {"The operation is not allowed on a non-enabled element"} */
                        typeof(InvalidOperationException)   /*  {"Operation cannot be performed."} */
                        );
                }
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("ScrollPattern", "SetScrollPercent", TestLab.PushData, TestWorks.Stable)]
        public static void ScrollPatternSetScrollPercent(AutomationElement element)
        {
            object patternObj = null;
            Random rnd = new Random((int)DateTime.Now.Ticks);
            if (GetPatternObject(element, ScrollPattern.Pattern, ref patternObj))
            {
                ScrollPattern pattern = (ScrollPattern)patternObj;
                Dump("ScrollPattern.SetScrollPercent", true, element);
                try
                {
                    // Postion between -50..150
                    pattern.SetScrollPercent((200 * rnd.NextDouble()) - 50, (200 * rnd.NextDouble()) - 50);
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception,
                        typeof(ElementNotAvailableException),
                        typeof(ElementNotEnabledException), /*  {"The operation is not allowed on a non-enabled element"} */
                        typeof(InvalidOperationException),  /*  {"Operation cannot be performed."} */
                        typeof(ArgumentOutOfRangeException) /*  {"ScrollBar Percent must be set between 0 and 100.\r\nParameter name: verticalPercent"} */
                        );
                }
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("ScrollPattern", "ToString", TestLab.PushData |TestLab.PullData, TestWorks.Stable)]
        public static void ScrollPatternToString(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, ScrollPattern.Pattern, ref patternObj))
            {
                ScrollPattern pattern = (ScrollPattern)patternObj;
                Dump("ScrollPattern.ToString()", true, element);
                try
                {
                    object property = pattern.ToString();
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception, typeof(ElementNotAvailableException));
                }
            }
        }


        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("ScrollPattern", "CurrentEquals", TestLab.PushData |TestLab.PullData, TestWorks.Stable)]
        public static void ScrollPatternCurrentEquals(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, ScrollPattern.Pattern, ref patternObj))
            {
                ScrollPattern pattern = (ScrollPattern)patternObj;
                Dump("ScrollPattern.Current.Equals()", true, element);
                try
                {
                    object property = pattern.Current.Equals(AutomationElement.RootElement);
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception, typeof(ElementNotAvailableException));
                }
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("ScrollPattern", "CurrentGetHashCode", TestLab.PushData |TestLab.PullData, TestWorks.Stable)]
        public static void ScrollPatternCurrentGetHashCode(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, ScrollPattern.Pattern, ref patternObj))
            {
                ScrollPattern pattern = (ScrollPattern)patternObj;
                Dump("ScrollPattern.Current.GetHashCode()", true, element);
                try
                {
                    object property = pattern.Current.GetHashCode();
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception, typeof(ElementNotAvailableException));
                }
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("ScrollPattern", "CurrentGetType", TestLab.PushData |TestLab.PullData, TestWorks.Stable)]
        public static void ScrollPatternCurrentGetType(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, ScrollPattern.Pattern, ref patternObj))
            {
                ScrollPattern pattern = (ScrollPattern)patternObj;
                Dump("ScrollPattern.Current.GetType()", true, element);
                try
                {
                    object property = pattern.Current.GetType();
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception, typeof(ElementNotAvailableException));
                }
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("ScrollPattern", "CurrentHorizontallyScrollable", TestLab.PushData |TestLab.PullData, TestWorks.Stable)]
        public static void ScrollPatternCurrentHorizontallyScrollable(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, ScrollPattern.Pattern, ref patternObj))
            {
                ScrollPattern pattern = (ScrollPattern)patternObj;
                Dump("ScrollPattern.Current.HorizontallyScrollable", true, element);
                try
                {
                    object property = pattern.Current.HorizontallyScrollable;
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception, typeof(ElementNotAvailableException));
                }
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("ScrollPattern", "CurrentHorizontalScrollPercent", TestLab.PushData |TestLab.PullData, TestWorks.Stable)]
        public static void ScrollPatternCurrentHorizontalScrollPercent(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, ScrollPattern.Pattern, ref patternObj))
            {
                ScrollPattern pattern = (ScrollPattern)patternObj;
                Dump("ScrollPattern.Current.HorizontalScrollPercent", true, element);
                try
                {
                    object property = pattern.Current.HorizontalScrollPercent;
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception, typeof(ElementNotAvailableException));
                }
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("ScrollPattern", "CurrentHorizontalViewSize", TestLab.PushData |TestLab.PullData, TestWorks.Stable)]
        public static void ScrollPatternCurrentHorizontalViewSize(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, ScrollPattern.Pattern, ref patternObj))
            {
                ScrollPattern pattern = (ScrollPattern)patternObj;
                Dump("ScrollPattern.Current.HorizontalViewSize", true, element);
                try
                {
                    object property = pattern.Current.HorizontalViewSize;
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception, typeof(ElementNotAvailableException));
                }
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("ScrollPattern", "CurrentToString", TestLab.PushData |TestLab.PullData, TestWorks.Stable)]
        public static void ScrollPatternCurrentToString(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, ScrollPattern.Pattern, ref patternObj))
            {
                ScrollPattern pattern = (ScrollPattern)patternObj;
                Dump("ScrollPattern.Current.ToString", true, element);
                try
                {
                    object property = pattern.Current.ToString();
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception, typeof(ElementNotAvailableException));
                }
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("ScrollPattern", "CurrentVerticallyScrollable", TestLab.PushData |TestLab.PullData, TestWorks.Stable)]
        public static void ScrollPatternCurrentVerticallyScrollable(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, ScrollPattern.Pattern, ref patternObj))
            {
                ScrollPattern pattern = (ScrollPattern)patternObj;
                Dump("ScrollPattern.Current.VerticallyScrollable", true, element);
                try
                {
                    object property = pattern.Current.VerticallyScrollable;
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception, typeof(ElementNotAvailableException));
                }
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("ScrollPattern", "CurrentVerticalScrollPercent", TestLab.PushData |TestLab.PullData, TestWorks.Stable)]
        public static void ScrollPatternCurrentVerticalScrollPercent(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, ScrollPattern.Pattern, ref patternObj))
            {
                ScrollPattern pattern = (ScrollPattern)patternObj;
                Dump("ScrollPattern.Current.VerticalScrollPercent", true, element);
                try
                {
                    object property = pattern.Current.VerticalScrollPercent;
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception, typeof(ElementNotAvailableException));
                }
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("ScrollPattern", "CurrentVerticalViewSize", TestLab.PushData |TestLab.PullData, TestWorks.Stable)]
        public static void ScrollPatternCurrentVerticalViewSize(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, ScrollPattern.Pattern, ref patternObj))
            {
                ScrollPattern pattern = (ScrollPattern)patternObj;
                Dump("ScrollPattern.Current.VerticalViewSize", true, element);
                try
                {
                    object property = pattern.Current.VerticalViewSize;
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception, typeof(ElementNotAvailableException));
                }
            }
        }

        #endregion Scroll

        #region SelectionItemPattern DONE

        #region PushData

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("SelectionItemPattern", "AddToSelection", TestLab.PushData, TestWorks.Stable)]
        public static void SelectionItemPatternAddToSelection(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, SelectionItemPattern.Pattern, ref patternObj))
            {
                SelectionItemPattern pattern = (SelectionItemPattern)patternObj;
                Dump("SelectionPattern.AddToSelection()", true, element);
                try
                {
                    pattern.AddToSelection();
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception,
                        typeof(ElementNotAvailableException),
                        typeof(ElementNotEnabledException),
                        typeof(InvalidOperationException) /* {"This Selection Container does not support multiple selection."}*/);
                }
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("SelectionItemPattern", "Select", TestLab.PushData, TestWorks.Stable)]
        public static void SelectionItemPatternSelect(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, SelectionItemPattern.Pattern, ref patternObj))
            {
                SelectionItemPattern pattern = (SelectionItemPattern)patternObj;
                Dump("SelectionPattern.Select()", true, element);
                try
                {
                    pattern.Select();
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception,
                        typeof(ElementNotAvailableException),
                        typeof(ElementNotEnabledException),
                        typeof(InvalidOperationException)       /* {"Operation cannot be performed."} */);
                }
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("SelectionItemPattern", "RemoveFromSelection", TestLab.PushData, TestWorks.Stable)]
        public static void SelectionItemPatternRemoveFromSelection(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, SelectionItemPattern.Pattern, ref patternObj))
            {
                SelectionItemPattern pattern = (SelectionItemPattern)patternObj;
                Dump("SelectionPattern.RemoveFromSelection()", true, element);
                try
                {
                    pattern.RemoveFromSelection();
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception,
                        typeof(ElementNotAvailableException),
                        typeof(ElementNotEnabledException),
                        typeof(InvalidOperationException) /* {"This Selection Container requires one selection."}*/
                    );
                }
            }
        }

        #endregion PushData

        #region PullData

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("SelectionItemPattern", "CurrentIsSelected", TestLab.PullData, TestWorks.Stable)]
        public static void SelectionItemPatternCurrentIsSelected(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, SelectionItemPattern.Pattern, ref patternObj))
            {
                SelectionItemPattern pattern = (SelectionItemPattern)patternObj;
                Dump("SelectionPattern.Current.IsSelected", true, element);
                try
                {
                    object property = pattern.Current.IsSelected;
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception, typeof(ElementNotAvailableException));
                }
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("SelectionItemPattern", "CurrentSelectionContainer", TestLab.PullData, TestWorks.Stable)]
        public static void SelectionItemPatternCurrentSelectionContainer(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, SelectionItemPattern.Pattern, ref patternObj))
            {
                SelectionItemPattern pattern = (SelectionItemPattern)patternObj;
                Dump("SelectionPattern.Current.SelectionContainer", true, element);
                try
                {
                    object property = pattern.Current.SelectionContainer;
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception, typeof(ElementNotAvailableException));
                }
            }
        }

        #endregion PushData


        #endregion SelectionItemPattern

        #region SelectionPattern DONE

        #region PushData
        #endregion PushData

        #region PullData

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("SelectionPattern", "CurrentCanSelectMultiple", TestLab.PullData, TestWorks.Stable)]
        public static void SelectionPatternCurrentCanSelectMultiple(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, SelectionPattern.Pattern, ref patternObj))
            {
                SelectionPattern pattern = (SelectionPattern)patternObj;
                Dump("SelectionPattern.Current.CanSelectMultiple", true, element);
                try
                {
                    bool property = pattern.Current.CanSelectMultiple;
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception,
                        typeof(ElementNotAvailableException),
                        typeof(ElementNotEnabledException));
                }
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("SelectionPattern", "CurrentIsSelectionRequired", TestLab.PullData, TestWorks.Stable)]
        public static void SelectionPatternCurrentIsSelectionRequired(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, SelectionPattern.Pattern, ref patternObj))
            {
                SelectionPattern pattern = (SelectionPattern)patternObj;
                Dump("SelectionPattern.Current.IsSelectionRequired", true, element);
                try
                {
                    bool property = pattern.Current.IsSelectionRequired;
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception,
                        typeof(ElementNotAvailableException),
                        typeof(ElementNotEnabledException));
                }
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("SelectionPattern", "CurrentGetSelection", TestLab.PullData, TestWorks.Stable)]
        public static void SelectionPatternCurrentGetSelection(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, SelectionPattern.Pattern, ref patternObj))
            {
                SelectionPattern pattern = (SelectionPattern)patternObj;
                Dump("SelectionPattern.Current.GetSelection()", true, element);
                try
                {
                    AutomationElement[] elements = pattern.Current.GetSelection();
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception,
                        typeof(ElementNotAvailableException),
                        typeof(ElementNotEnabledException));
                }
            }
        }

        #endregion PullData

        #endregion SelectionPattern

        #region TogglePattern DONE

        #region PushData

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("TogglePattern", "Toggle", TestLab.PushData, TestWorks.Stable)]
        public static void TogglePatternToggle(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, TogglePattern.Pattern, ref patternObj))
            {
                TogglePattern pattern = (TogglePattern)patternObj;
                Dump("TogglePattern.Toggle()", true, element);
                try
                {
                    if (TreeWalker.ControlViewWalker.GetParent(TreeWalker.ControlViewWalker.GetParent(element)).Current.AutomationId != "1020" &&
                        TreeWalker.ControlViewWalker.GetParent(element).Current.AutomationId != "1020" &&
                        TreeWalker.ControlViewWalker.GetParent(TreeWalker.ControlViewWalker.GetParent(element)).Current.AutomationId != "1013" &&
                        TreeWalker.ControlViewWalker.GetParent(element).Current.AutomationId != "1013")
                    {
                        pattern.Toggle();
                    }
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception,
                        typeof(ElementNotAvailableException),
                        typeof(ElementNotEnabledException)//,
                        //typeof(TimeoutException)    /* Special case for ListView */
                        );
                }
            }
        }

        #endregion PushData

        #region PullData

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("TogglePattern", "CurrentToggleState", TestLab.PullData, TestWorks.Stable)]
        public static void TogglePatternCurrentToggleState(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, TogglePattern.Pattern, ref patternObj))
            {
                TogglePattern pattern = (TogglePattern)patternObj;
                Dump("TogglePattern.Current.ToggleState", true, element);
                try
                {
                    ToggleState property = pattern.Current.ToggleState;
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception,
                        typeof(ElementNotAvailableException),
                        typeof(ElementNotEnabledException));
                }
            }
        }

        #endregion PullData

        #endregion TogglePattern

        #region Transform DONE

        #region PushData

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("TransformPattern", "Move", TestLab.PushData, TestWorks.Stable)]
        public static void TransformPatternMove(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, TransformPattern.Pattern, ref patternObj))
            {
                TransformPattern pattern = (TransformPattern)patternObj;
                Dump("ScrollPattern.Move", true, element);
                try
                {
                    pattern.Move(s_rnd.Next(-2000, 2000), s_rnd.Next(-2000, 2000));
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception, 
                        typeof(ElementNotAvailableException),
                        typeof(InvalidOperationException) /* {"Operation cannot be performed."} */
                        );
                }
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("TransformPattern", "Resize", TestLab.PushData, TestWorks.Stable)]
        public static void TransformPatternResize(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, TransformPattern.Pattern, ref patternObj))
            {
                TransformPattern pattern = (TransformPattern)patternObj;
                Dump("ScrollPattern.Resize", true, element);
                try
                {
                    pattern.Resize(s_rnd.Next(-10, 2000), s_rnd.Next(-10, 2000));
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception,
                        typeof(ElementNotAvailableException),
                        typeof(ArgumentException),
                        typeof(InvalidOperationException));
                }
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("TransformPattern", "Rotate", TestLab.PushData, TestWorks.Stable)]
        public static void TransformPatternRotate(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, TransformPattern.Pattern, ref patternObj))
            {
                TransformPattern pattern = (TransformPattern)patternObj;
                Dump("ScrollPattern.Rotate", true, element);
                try
                {
                    pattern.Rotate(s_rnd.Next(-720, 720));
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception,
                        typeof(ElementNotAvailableException),
                        typeof(ArgumentException),
                        typeof(InvalidOperationException));
                }
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("TransformPattern", "ToString", TestLab.PushData, TestWorks.Stable)]
        public static void TransformPatternToString(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, TransformPattern.Pattern, ref patternObj))
            {
                TransformPattern pattern = (TransformPattern)patternObj;
                Dump("ScrollPattern.ToString", true, element);
                try
                {
                    object property = pattern.ToString();
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception,
                        typeof(ElementNotAvailableException),
                        typeof(ArgumentException),
                        typeof(InvalidOperationException));
                }
            }
        }

        #endregion PushData

        #region PullData

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("TransformPattern", "Equals", TestLab.PullData, TestWorks.Stable)]
        public static void TransformPatternEquals(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, TransformPattern.Pattern, ref patternObj))
            {
                TransformPattern pattern = (TransformPattern)patternObj;
                Dump("ScrollPattern.Equals", true, element);
                try
                {
                    object property = pattern.Equals(AutomationElement.RootElement);
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception, typeof(ElementNotAvailableException));
                }
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("TransformPattern", "GetHashCode", TestLab.PullData, TestWorks.Stable)]
        public static void TransformPatternGetHashCode(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, TransformPattern.Pattern, ref patternObj))
            {
                TransformPattern pattern = (TransformPattern)patternObj;
                Dump("ScrollPattern.GetHashCode", true, element);
                try
                {
                    object property = pattern.GetHashCode();
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception, typeof(ElementNotAvailableException));
                }
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("TransformPattern", "GetType", TestLab.PullData, TestWorks.Stable)]
        public static void TransformPatternGetType(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, TransformPattern.Pattern, ref patternObj))
            {
                TransformPattern pattern = (TransformPattern)patternObj;
                Dump("ScrollPattern.GetType", true, element);
                try
                {
                    object property = pattern.GetType();
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception, typeof(ElementNotAvailableException));
                }
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("TransformPattern", "CanMove", TestLab.PullData, TestWorks.Stable)]
        public static void TransformPatternCanMove(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, TransformPattern.Pattern, ref patternObj))
            {
                TransformPattern pattern = (TransformPattern)patternObj;
                Dump("ScrollPattern.CanMove", true, element);
                try
                {
                    object property = pattern.Current.CanMove;
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception, typeof(ElementNotAvailableException));
                }
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("TransformPattern", "CanResize", TestLab.PullData, TestWorks.Stable)]
        public static void TransformPatternCanResize(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, TransformPattern.Pattern, ref patternObj))
            {
                TransformPattern pattern = (TransformPattern)patternObj;
                Dump("ScrollPattern.CanResize", true, element);
                try
                {
                    object property = pattern.Current.CanResize;
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception, typeof(ElementNotAvailableException));
                }
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("TransformPattern", "CanRotate", TestLab.PullData, TestWorks.Stable)]
        public static void TransformPatternCanRotate(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, TransformPattern.Pattern, ref patternObj))
            {
                TransformPattern pattern = (TransformPattern)patternObj;
                Dump("ScrollPattern.CanRotate", true, element);
                try
                {
                    object property = pattern.Current.CanRotate;
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception, typeof(ElementNotAvailableException));
                }
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("TransformPattern", "CurrentEquals", TestLab.PullData, TestWorks.Stable)]
        public static void TransformPatternCurrentEquals(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, TransformPattern.Pattern, ref patternObj))
            {
                TransformPattern pattern = (TransformPattern)patternObj;
                Dump("ScrollPattern.Current.Equals", true, element);
                try
                {
                    object property = pattern.Current.Equals(AutomationElement.RootElement); ;
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception, typeof(ElementNotAvailableException));
                }
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("TransformPattern", "CurrentGetHashCode", TestLab.PullData, TestWorks.Stable)]
        public static void TransformPatternCurrentGetHashCode(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, TransformPattern.Pattern, ref patternObj))
            {
                TransformPattern pattern = (TransformPattern)patternObj;
                Dump("ScrollPattern.Current.GetHashCode", true, element);
                try
                {
                    object property = pattern.Current.GetHashCode();
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception, typeof(ElementNotAvailableException));
                }
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("TransformPattern", "CurrentGetType", TestLab.PullData, TestWorks.Stable)]
        public static void TransformPatternCurrentGetType(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, TransformPattern.Pattern, ref patternObj))
            {
                TransformPattern pattern = (TransformPattern)patternObj;
                Dump("ScrollPattern.Current.GetType", true, element);
                try
                {
                    object property = pattern.Current.GetType();
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception, typeof(ElementNotAvailableException));
                }
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("TransformPattern", "CurrentToString", TestLab.PullData, TestWorks.Stable)]
        public static void TransformPatternCurrentToString(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, TransformPattern.Pattern, ref patternObj))
            {
                TransformPattern pattern = (TransformPattern)patternObj;
                Dump("ScrollPattern.Current.ToString", true, element);
                try
                {
                    object property = pattern.Current.ToString();
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception, typeof(ElementNotAvailableException));
                }
            }
        }

        #endregion PullData

        #endregion Transform

        #region RangePattern DONE

        #region PullData

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("RangeValuePattern", "SetValue", TestLab.PushData, TestWorks.Stable)]
        public static void RangeValuePatternSetValue(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, RangeValuePattern.Pattern, ref patternObj))
            {
                RangeValuePattern pattern = (RangeValuePattern)patternObj;
                double val = (double)Helpers.RandomValue(Convert.ToDouble(pattern.Current.Minimum), Convert.ToDouble(pattern.Current.Maximum));
                Dump("RangeValue.SetValue(" + val + ") with Minimum(" + pattern.Current.Minimum + ") and pattern.Current.Maximum(" + pattern.Current.Maximum + ")", false, element);
                try
                {
                    pattern.SetValue(val);
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception,
                        typeof(ElementNotAvailableException),
                        typeof(ElementNotEnabledException),
                        typeof(InvalidOperationException) /* {"This value is readonly."} */
                    );
                }
            }
        }

        #endregion PullData

        #region PullData

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("RangeValuePattern", "CurrentIsReadOnly", TestLab.PullData, TestWorks.Stable)]
        public static void RangeValuePatternCurrentIsReadOnly(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, RangeValuePattern.Pattern, ref patternObj))
            {
                RangeValuePattern pattern = (RangeValuePattern)patternObj;
                Dump("RangeValuePattern.Current.IsReadOnly", true, element);
                try
                {
                    object property = pattern.Current.IsReadOnly;
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception, typeof(ElementNotAvailableException));
                }
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("RangeValuePattern", "CurrentLargeChange", TestLab.PullData, TestWorks.Stable)]
        public static void RangeValuePatternCurrentLargeChange(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, RangeValuePattern.Pattern, ref patternObj))
            {
                RangeValuePattern pattern = (RangeValuePattern)patternObj;
                Dump("RangeValuePattern.Current.LargeChange", true, element);
                try
                {
                    object property = pattern.Current.LargeChange;
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception, typeof(ElementNotAvailableException));
                }
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("RangeValuePattern", "CurrentMaximum", TestLab.PullData, TestWorks.Stable)]
        public static void RangeValuePatternCurrentMaximum(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, RangeValuePattern.Pattern, ref patternObj))
            {
                RangeValuePattern pattern = (RangeValuePattern)patternObj;
                Dump("RangeValuePattern.Current.Maximum", true, element);
                try
                {
                    object property = pattern.Current.Maximum;
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception, typeof(ElementNotAvailableException));
                }
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("RangeValuePattern", "CurrentMinimum", TestLab.PullData, TestWorks.Stable)]
        public static void RangeValuePatternCurrentMinimum(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, RangeValuePattern.Pattern, ref patternObj))
            {
                RangeValuePattern pattern = (RangeValuePattern)patternObj;
                Dump("RangeValuePattern.Current.Minimum", true, element);
                try
                {
                    object property = pattern.Current.Minimum;
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception, typeof(ElementNotAvailableException));
                }
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("RangeValuePattern", "CurrentSmallChange", TestLab.PullData, TestWorks.Stable)]
        public static void RangeValuePatternCurrentSmallChange(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, RangeValuePattern.Pattern, ref patternObj))
            {
                RangeValuePattern pattern = (RangeValuePattern)patternObj;
                Dump("RangeValuePattern.Current.SmallChange", true, element);
                try
                {
                    object property = pattern.Current.SmallChange;
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception, typeof(ElementNotAvailableException));
                }
            }
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("RangeValuePattern", "CurrentValue", TestLab.PullData, TestWorks.Stable)]
        public static void RangeValuePatternCurrentValue(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, RangeValuePattern.Pattern, ref patternObj))
            {
                RangeValuePattern pattern = (RangeValuePattern)patternObj;
                Dump("RangeValuePattern.Current.Value", true, element);
                try
                {
                    object property = pattern.Current.Value;
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception, typeof(ElementNotAvailableException));
                }
            }
        }

        #endregion PullData

        #endregion RangeValuePattern

        #region Value DONE

        #region PushData

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("ValuePattern", "SetValue", TestLab.PushData, TestWorks.Stable)]
        public static void ValuePatternSetValue(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, ValuePattern.Pattern, ref patternObj))
            {
                ValuePattern pattern = (ValuePattern)patternObj;
                Dump("ValuePattern.SetValue", true, element);
                try
                {
                    pattern.SetValue(RandomString());
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception,
                        typeof(ElementNotAvailableException),
                        typeof(ElementNotEnabledException),
                        typeof(InvalidOperationException),      /* {"This value is readonly."} */
                        typeof(FormatException)                 /* {"The string was not recognized as a valid DateTime. There is a unknown word starting at index 0."}*/
                        );
                }
            }
        }

        #endregion PushData

        #region PullData

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("ValuePattern", "Equals", TestLab.PullData, TestWorks.Stable)]
        public static void ValuePatternEquals(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, ValuePattern.Pattern, ref patternObj))
            {
                ValuePattern pattern = (ValuePattern)patternObj;
                Dump("ValuePattern.Equals", true, element);
                try
                {
                    object property = pattern.Equals(AutomationElement.RootElement);
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception, typeof(ElementNotAvailableException));
                }
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("ValuePattern", "GetHashCode", TestLab.PullData, TestWorks.Stable)]
        public static void ValuePatternGetHashCode(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, ValuePattern.Pattern, ref patternObj))
            {
                ValuePattern pattern = (ValuePattern)patternObj;
                Dump("ValuePattern.GetHashCode", true, element);
                try
                {
                    object property = pattern.GetHashCode();
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception, typeof(ElementNotAvailableException));
                }
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("ValuePattern", "GetType", TestLab.PullData, TestWorks.Stable)]
        public static void ValuePatternGetType(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, ValuePattern.Pattern, ref patternObj))
            {
                ValuePattern pattern = (ValuePattern)patternObj;
                Dump("ValuePattern.GetType", true, element);
                try
                {
                    object property = pattern.GetType();
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception, typeof(ElementNotAvailableException));
                }
            }
        }

        #endregion PullData

        #endregion Value

        #region ############### GridItemPattern Needs added
        #endregion GridItemPattern

        #region ############### GridPattern Needs added
        #endregion GridPattern

        #region ############### MultiViewPattern Needs added
        #endregion MultiViewPattern

        #region ############### TextPattern Needs added

        #region PullData

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("TextPattern", "DocumentRange", TestLab.PullData, TestWorks.Stable)]
        public static void TextPatternDocumentRange(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, TextPattern.Pattern, ref patternObj))
            {
                TextPattern pattern = (TextPattern)patternObj;
                Dump("TextPattern.DocumentRange", true, element);
                try
                {
                    object property = pattern.DocumentRange;
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception, typeof(ElementNotAvailableException));
                }
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("TextPattern", "GetSelection", TestLab.PullData, TestWorks.Stable)]
        public static void TextPatternGetSelection(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, TextPattern.Pattern, ref patternObj))
            {
                TextPattern pattern = (TextPattern)patternObj;
                Dump("TextPattern.GetSelection", true, element);
                try
                {
                    object property = pattern.GetSelection();
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception, 
                        typeof(ElementNotAvailableException),
                        typeof(InvalidOperationException));
                }
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("TextPattern", "GetVisibleRanges", TestLab.PullData, TestWorks.Stable)]
        public static void TextPatternGetVisibleRanges(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, TextPattern.Pattern, ref patternObj))
            {
                TextPattern pattern = (TextPattern)patternObj;
                Dump("TextPattern.GetVisibleRanges", true, element);
                try
                {
                    object property = pattern.GetVisibleRanges();
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception, typeof(ElementNotAvailableException));
                }
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("TextPattern", "RangeFromChildGood", TestLab.PullData, TestWorks.Stable)]
        public static void TextPatternRangeFromChildGood(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, TextPattern.Pattern, ref patternObj))
            {
                TextPattern pattern = (TextPattern)patternObj;
                Dump("TextPattern.GetVisibleRanges", true, element);
                try
                {
                    AutomationElementCollection collection = element.FindAll(TreeScope.Subtree, System.Windows.Automation.Condition.TrueCondition);
                    object property = pattern.RangeFromChild(collection[s_rnd.Next(collection.Count)]);
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception,
                        typeof(ElementNotAvailableException),
                        typeof(InvalidOperationException));
                }
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("TextPattern", "RangeFromChildBad", TestLab.PullData, TestWorks.Stable)]
        public static void TextPatternRangeFromChildBad(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, TextPattern.Pattern, ref patternObj))
            {
                TextPattern pattern = (TextPattern)patternObj;
                Dump("TextPattern.RangeFromChild(bad)", true, element);
                try
                {
                    AutomationElementCollection collection = element.FindAll(TreeScope.Subtree, System.Windows.Automation.Condition.TrueCondition);
                    object property = pattern.RangeFromChild(TreeWalker.RawViewWalker.GetParent(element));
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception,
                        typeof(ElementNotAvailableException),
                        typeof(InvalidOperationException),      /* {"Win32 Edit controls don't have children."} */
                        typeof(ArgumentException)               /* {"Value does not fall within the expected range."} */
                        );
                }
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("TextPattern", "RangeFromPointRandom", TestLab.PullData, TestWorks.Stable)]
        public static void TextPatternRangeFromPointRandom(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, TextPattern.Pattern, ref patternObj))
            {
                TextPattern pattern = (TextPattern)patternObj;
                Dump("TextPattern.RangeFromPoint(random)", true, element);
                try
                {
                    object property = pattern.RangeFromPoint(new Point(s_rnd.Next(-2000, 2000), s_rnd.Next(-2000, 2000)));
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception,
                        typeof(ArgumentException),              /* {"Screen coordinate is outside the bounding rectangle"} */
                        typeof(ElementNotAvailableException),
                        typeof(InvalidOperationException));
                }
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("TextPattern", "SupportedTextSelection", TestLab.PullData, TestWorks.Stable)]
        public static void TextPatternSupportedTextSelection(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, TextPattern.Pattern, ref patternObj))
            {
                TextPattern pattern = (TextPattern)patternObj;
                Dump("TextPattern.SupportedTextSelection", true, element);
                try
                {
                    object property = pattern.SupportedTextSelection;
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception, typeof(ElementNotAvailableException));
                }
            }
        }

        #endregion PullData
        
        #region PushData

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("TextPattern", "DocumentRangeSelect", TestLab.PushData, TestWorks.Stable)]
        public static void TextPatternDocumentRangeSelect(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, TextPattern.Pattern, ref patternObj))
            {
                TextPattern pattern = (TextPattern)patternObj;
                Dump("TextPattern.DocumentRange.Select", true, element);
                try
                {
                    GetRandomTestPatternRange(pattern).Select();
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception, typeof(ElementNotAvailableException));
                }
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("TextPattern", "DocumentRangeAddToSelection", TestLab.PullData, TestWorks.NoWorking)]
        public static void TextPatternDocumentRangeAddToSelection(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, TextPattern.Pattern, ref patternObj))
            {
                System.Diagnostics.Debug.Assert(false);
                TextPattern pattern = (TextPattern)patternObj;
                Dump("TextPattern.DocumentRange.AddToSelection", true, element);
                try
                {
                    pattern.DocumentRange.AddToSelection();
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception, 
                        typeof(ElementNotAvailableException),
                        typeof(InvalidOperationException)       /* {"Operation is not valid due to the current state of the object."} */);
                }
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("TextPattern", "DocumentRangeClone", TestLab.PullData, TestWorks.Stable)]
        public static void TextPatternDocumentRangeClone(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, TextPattern.Pattern, ref patternObj))
            {
                TextPattern pattern = (TextPattern)patternObj;
                Dump("TextPattern.DocumentRange.Clone", true, element);
                try
                {
                    pattern.DocumentRange.Clone();

                }
                catch (Exception exception)
                {
                    VerifyException(element, exception, typeof(ElementNotAvailableException));
                }
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("TextPattern", "DocumentRangeCompare", TestLab.PullData, TestWorks.Stable)]
        public static void TextPatternDocumentRangeCompare(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, TextPattern.Pattern, ref patternObj))
            {
                TextPattern pattern = (TextPattern)patternObj;
                Dump("TextPattern.DocumentRange.Compare", true, element);
                try
                {
                    pattern.DocumentRange.Compare(GetRandomTestPatternRange());

                }
                catch (Exception exception)
                {
                    VerifyException(element, exception, 
                        typeof(ElementNotAvailableException), 
                        typeof(ArgumentNullException)          /* {"Value cannot be null.\r\nParameter name: range"} */
                        );
                }
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("TextPattern", "DocumentRangeCompareEndpoints", TestLab.PullData, TestWorks.Stable)]
        public static void TextPatternDocumentRangeCompareEndpoints(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, TextPattern.Pattern, ref patternObj))
            {
                TextPattern pattern = (TextPattern)patternObj;
                Dump("TextPattern.DocumentRange.CompareEndpoints", true, element);
                try
                {
                    pattern.DocumentRange.CompareEndpoints(TextPatternRangeEndpoint.Start, GetRandomTestPatternRange(), TextPatternRangeEndpoint.Start);
                    pattern.DocumentRange.CompareEndpoints(TextPatternRangeEndpoint.Start, GetRandomTestPatternRange(), TextPatternRangeEndpoint.End);
                    pattern.DocumentRange.CompareEndpoints(TextPatternRangeEndpoint.End, GetRandomTestPatternRange(), TextPatternRangeEndpoint.Start);
                    pattern.DocumentRange.CompareEndpoints(TextPatternRangeEndpoint.End, GetRandomTestPatternRange(), TextPatternRangeEndpoint.End);
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception,
                        typeof(ElementNotAvailableException),
                        typeof(ArgumentNullException)           /* typeof(ElementNotAvailableException) */ );
                }
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [StressBuckets("TextPattern", "Test", TestLab.PullData, TestWorks.Stable)]
        public static void TextPatternTest(AutomationElement element)
        {
            object patternObj = null;
            if (GetPatternObject(element, TextPattern.Pattern, ref patternObj))
            {
                TextPattern pattern = (TextPattern)patternObj;
                Dump("TextPattern.DocumentRange.CompareEndpoints", true, element);
                try
                {
                    pattern.DocumentRange.CompareEndpoints(TextPatternRangeEndpoint.Start, GetRandomTestPatternRange(pattern), TextPatternRangeEndpoint.Start);
                    pattern.DocumentRange.CompareEndpoints(TextPatternRangeEndpoint.Start, GetRandomTestPatternRange(pattern), TextPatternRangeEndpoint.End);
                    pattern.DocumentRange.CompareEndpoints(TextPatternRangeEndpoint.End, GetRandomTestPatternRange(pattern), TextPatternRangeEndpoint.Start);
                    pattern.DocumentRange.CompareEndpoints(TextPatternRangeEndpoint.End, GetRandomTestPatternRange(pattern), TextPatternRangeEndpoint.End);
                }
                catch (Exception exception)
                {
                    VerifyException(element, exception,
                        typeof(ElementNotAvailableException),
                        typeof(ArgumentNullException)           /* typeof(ElementNotAvailableException) */ );
                }
            }
        }

        private static TextPatternRange GetRandomTestPatternRange(TextPattern pattern)
        {
            int count = s_rnd.Next(-10, 10);
            
            TextUnit textUnit = GetRandomTextUnit();
            pattern.DocumentRange.Move(textUnit, count);

            count = s_rnd.Next(-10, 10);
            textUnit = GetRandomTextUnit();
            pattern.DocumentRange.MoveEndpointByUnit(TextPatternRangeEndpoint.End, textUnit, count);
            
            return pattern.DocumentRange;
        }


        private static TextUnit GetRandomTextUnit()
        {
            switch (s_rnd.Next(7))
            {
                case 0: return TextUnit.Character;
                case 1: return TextUnit.Document;
                case 2: return TextUnit.Format;
                case 3: return TextUnit.Line;
                case 4: return TextUnit.Page;
                case 5: return TextUnit.Paragraph;
                case 6: return TextUnit.Word;
                default : return TextUnit.Character;
            }
        }


        private static System.Windows.Automation.Text.TextPatternRange GetRandomTestPatternRange()
        {
            
            return null;
        }


        #endregion PushData

        #endregion TextPattern Needs added

        #endregion Tests

        #region Helpers

        private static string RandomString()
        {
            StringBuilder strTemp = new StringBuilder();
            Random rnd = new Random((int)DateTime.Now.Ticks);
            int size = rnd.Next(92);

            if (size < 25)          //
                size = 0;           // weight: 25
            else if (size < 50)     
                size = 1;           // weight: 25
            else if (size < 60)     
                size = 2;           // weight: 10
            else if (size < 70)     
                size = 10;          // weight: 10
            else if (size < 80)     
                size = 50;          // weight: 10
            else if (size < 90)     
                size = 5000;        // weight: 10
            else if (size < 91)     
                size = 50000;       // weight: 1


            int iNumber;


            switch (rnd.Next(5))
            {
                case 0: // anything
                    for (int i = 0; i < size; i++)
                    {
                        //strTemp.Append((char)rnd.Next());
                    }
                    break;
                case 1: // Alphanumeric
                    for (int i = 0; i < size; i++)
                    {
                        iNumber = rnd.Next(122);
                        if (48 > iNumber)
                            iNumber += 48;
                        if ((57 < iNumber) &&
                            (65 > iNumber))
                            iNumber += 7;
                        if ((90 < iNumber) &&
                            (97 > iNumber))
                            iNumber += 6;
                        strTemp.Append((char)iNumber);
                    }
                    break;
                case 2:// Alpha
                    for (int i = 0; i < size; i++)
                    {
                        iNumber = rnd.Next(122);
                        if (65 > iNumber)
                            iNumber = 65 + iNumber % 56;
                        if ((90 < iNumber) &&
                            (97 > iNumber))
                            iNumber += 6;
                        strTemp.Append((char)iNumber);
                    }
                    break;
                case 3: // numeric
                    for (int i = 0; i < size; i++)
                    {
                        strTemp.Append(string.Format("{0}", rnd.Next(10)));
                    }
                    break;
                case 4:
                    //strTemp.Append(DateTime.Now);
                    break;
            }

            return strTemp.ToString();
        }

        static ScrollAmount GetRandomScrollAmount()
        {
            Random rnd = new Random((int)DateTime.Today.Ticks);

            switch (rnd.Next(5))
            {
                case 0:
                    return ScrollAmount.LargeDecrement;
                case 1:
                    return ScrollAmount.LargeIncrement;
                case 2:
                    return ScrollAmount.NoAmount;
                case 3:
                    return ScrollAmount.SmallDecrement;
                case 4:
                    return ScrollAmount.SmallIncrement;
                default:
                    throw new ArgumentException("Bad");
            }


        }

        private static void VerifyException(AutomationElement element, Exception exception, params object[] p)
        {
            if (Array.IndexOf(p, exception.GetType()) == -1)
                throw new Exception("Test Error(" + Helpers.GetXmlPathFromAutomationElement(element), exception);
        }

        /// -------------------------------------------------------------------
        /// <summary>HELPER: Obtains the pattern object and returns true or false depending on success</summary>
        /// -------------------------------------------------------------------
        private static bool GetPatternObject(AutomationElement element, AutomationPattern automationPattern, ref object patternObj)
        {
            bool succeeded = true;
            try
            {
                patternObj = element.GetCurrentPattern(automationPattern);
            }
            catch (InvalidOperationException exception)
            {
                Dump(exception.ToString(), true, element);
                succeeded = false;
            }
            return succeeded;

        }

        /// -------------------------------------------------------------------
        /// <summary>HELPER: Generates a random cache</summary>
        /// -------------------------------------------------------------------
        static CacheRequest RandomCache()
        {
            Random rnd = new Random((int)DateTime.Now.Ticks);
            StringBuilder sb = new StringBuilder();
            CacheRequest cache = new CacheRequest();
            AutomationPattern p1;
            AutomationProperty p2;
            ArrayList propertyList = null;
            ArrayList patternList = null;

            PopulateAutomationElementProperties(ref propertyList, ref patternList);

            try
            {
                // Decide whether to add patterns
                if (rnd.Next(2) == 0)
                {
                    // Add up to two patterns
                    int maxIndex = rnd.Next(2);
                    for (int index = 0; index < maxIndex; index++)
                    {
                        try
                        {

                            if (null != (p1 = (AutomationPattern)patternList[rnd.Next(patternList.Count)]))
                            {
                                sb.Append(p1.ProgrammaticName + ":");
                                cache.Add(p1);
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
                // Decide whether to add properties
                if (rnd.Next(2) == 0)
                {
                    // Add up to three AutomationProperty
                    int maxIndex = rnd.Next(3);
                    for (int index = 0; index < maxIndex; index++)
                    {
                        try
                        {
                            if (null != (p2 = (AutomationProperty)propertyList[(rnd.Next(propertyList.Count))]))
                            {
                                sb.Append(p2.ProgrammaticName + ":");
                                cache.Add(p2);
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                }

                switch (rnd.Next(2))
                {
                    case 0:
                        cache.AutomationElementMode = AutomationElementMode.Full;
                        sb.Append(AutomationElementMode.Full + ":");
                        break;
                    case 1:
                        cache.AutomationElementMode = AutomationElementMode.None;
                        sb.Append(AutomationElementMode.None + ":");
                        break;
                    default:
                        throw new Exception("Bad test code(AutomationElementMode)");
                }

                switch (rnd.Next(6))
                {
                    case 0:
                        cache.TreeScope = TreeScope.Ancestors;
                        sb.Append(TreeScope.Ancestors + ":");
                        break;
                    case 1:
                        cache.TreeScope = TreeScope.Children;
                        sb.Append(TreeScope.Children + ":");
                        break;
                    case 2:
                        cache.TreeScope = TreeScope.Descendants;
                        sb.Append(TreeScope.Descendants + ":");
                        break;
                    case 3:
                        cache.TreeScope = TreeScope.Element;
                        sb.Append(TreeScope.Element + ":");
                        break;
                    case 4:
                        cache.TreeScope = TreeScope.Parent;
                        sb.Append(TreeScope.Parent + ":");
                        break;
                    case 5:
                        cache.TreeScope = TreeScope.Subtree;
                        sb.Append(TreeScope.Subtree + ":");
                        break;
                    default:
                        throw new Exception("Bad test code(treescope)");
                }
                //switch (rnd.Next(3))
                //{
                //    case 0:
                //        cache.TreeFilter = NotCondition.;
                //        break;
                //    case 1:
                //        cache.TreeFilter = AndCondition;
                //        break;
                //    case 2:
                //        cache.TreeFilter = OrCondition;
                //        break;

                //}
            }
            catch (ArgumentException)
            {
                // Some TreeScopes are invalid
            }

            Dump(sb.ToString(), false, null);
            return cache;
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// HELPERS: General purpose dump information routine.  If the _appCommands has 
        /// been defined, then this same infomration will be passed back to the 
        /// calling application. If infois an Exception, then display the 
        /// message and re-throw the exception.
        /// </summary>
        /// <param name="info">Information that needs to be printed out</param>
        /// <param name="displayElement">Flag to determine whether to call
        /// Library.GetUISpyLook and display the results</param>
        /// -------------------------------------------------------------------
        static public void Dump(object info, bool displayElement, AutomationElement element)
        {
            try
            {
                StringBuilder str = new StringBuilder("<" + Thread.CurrentThread.ManagedThreadId + ">");

                // Custom attribute needs to be printed out
                if (info is StressBuckets)
                {
                    StressBuckets sb = (StressBuckets)info;

                    if (element == null)
                        str.Append("[" + Library.GetUISpyLook(element) + "]");
                    else
                        str.Append("[null]");

                    str.Append("Calling: " + sb.PatternName + "." + sb.MethodOrPropertyName);
                }
                // Normal string that needs printed
                else if (info is string)
                {
                    str.Append(info.ToString());
                }
                // Did I miss something?
                else
                {
                    Debug.Assert(false);
                }

                if (displayElement)
                {
                    if (element != null)
                        str.Append("[" + InternalHelper.Helpers.GetXmlPathFromAutomationElement(element) + "]");
                    else
                        str.Append("[null]");
                }

                // Lock the error and print
                lock (str)
                {
                    //System.Diagnostics.Trace.WriteLine(str.ToString());
                    Console.WriteLine(str.ToString());
                }
            }
            catch (Exception)
            {
                // Searves no purpose to error here in this code, such 
                // as EncoderFallbackException, so just eat them
            }

        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// Reset the _curElement to the _originalElement.  This may be needed when 
        /// elements go away and the _curElement no longer points to a valid object
        /// </summary>
        /// -------------------------------------------------------------------
        static private void ResetElementToOriginalElement()
        {
            Dump("############# Reset", false, null);
            //_curElement = _originalElement;
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// HELPER: Adds all the avaliable AutomationElement properties to collection
        /// </summary>
        /// -------------------------------------------------------------------
        static void PopulateAutomationElementProperties(ref ArrayList propertyList, ref ArrayList patternList)
        {
            // Only puolate it if we have not already initialized it
            if (propertyList == null)
            {
                propertyList = new ArrayList();
                patternList = new ArrayList();


                Assembly assembly = Assembly.GetAssembly(typeof(AutomationElement));
                AutomationProperty property = null;

                // ----------------------------------------------------------
                // Do the AutomationProperties off of the AutomationElement
                // ----------------------------------------------------------
                foreach (FieldInfo fi in assembly.GetType("System.Windows.Automation.AutomationElement").GetFields())
                {
                    if (fi.FieldType == typeof(AutomationProperty))
                    {
                        // Get the AutomationProperty using Reflection
                        property = (AutomationProperty)fi.GetValue(AutomationElement.RootElement);
                        propertyList.Add(property);

                    }
                }
                // ----------------------------------------------------------"
                // Do the AutomationProperties off of the Pattern objects"
                // ----------------------------------------------------------"
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.BaseType == typeof(BasePattern))
                    {
                        AutomationIdentifier id = (AutomationIdentifier)type.GetField("Pattern").GetValue(AutomationElement.RootElement);
                        if (patternList.IndexOf(id) == -1)
                            patternList.Add(id);

                        foreach (FieldInfo fi in type.GetFields())
                        {
                            if (fi.FieldType == typeof(AutomationProperty))
                            {
                                property = (AutomationProperty)fi.GetValue(AutomationElement.RootElement);
                                propertyList.Add(property);
                            }
                        }
                    }
                }
            }
        }

        #endregion Helpers
    }

    /// -----------------------------------------------------------------------
    /// <summary>
    /// Enumeration of where the test can be ran from
    /// </summary>
    /// -----------------------------------------------------------------------
    public enum TestLab
    {
        PushData,
        PullData,
        PushPullData
    }

    /// -----------------------------------------------------------------------
    /// <summary>
    /// Enumeration to flag the actual stability of the test case
    /// </summary>
    /// -----------------------------------------------------------------------
    public enum TestWorks
    {
        Stable = 1,
        NoWorking = 2
    }

    /// -----------------------------------------------------------------------
    /// <summary>
    /// Flag to filter on what type of stress scenario to run.  Mostly for UIV
    /// tests so we can fine tune our tests for specific clients
    /// </summary>
    /// -----------------------------------------------------------------------
    public enum Scenario
    {
        General = 1,
        Narrator = 2
    }

    /// -----------------------------------------------------------------------
    /// <summary>
    /// This is the custom attribues on each test cases.  The information
    /// within the attribute will help the driver determine which methods
    /// can be called given the characteristics of the given 
    /// AutomationElement.
    /// </summary>
    /// -----------------------------------------------------------------------
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Struct, AllowMultiple = true)]
    public sealed class StressBuckets : Attribute
    {
        #region Variables

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        string _patternName;
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        string _methodOrPropertyName;
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        TestLab _testLab;
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        TestWorks _testWorks;
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        int _bugNumber;
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        Scenario _scenario;
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        int _weight = 5;


        #endregion Variables

        #region Properties

        /// -------------------------------------------------------------------
        /// <summary>Return the client that this test is designed for</summary>
        /// -------------------------------------------------------------------
        public Scenario Scenario { get { return _scenario; } set { _scenario = value; } }
        /// -------------------------------------------------------------------
        /// <summary>Bug number assigned</summary>
        /// -------------------------------------------------------------------
        public int BugNumber { get { return _bugNumber; } }
        /// -------------------------------------------------------------------
        /// <summary>The UIAutomation pattern name test will work with</summary>
        /// -------------------------------------------------------------------
        public string PatternName { get { return _patternName; } }
        /// -------------------------------------------------------------------
        /// <summary>The UIAutomation method or property(set/get) that is 
        /// associated with the pattern.</summary>
        /// -------------------------------------------------------------------
        public string MethodOrPropertyName { get { return _methodOrPropertyName; } }
        /// -------------------------------------------------------------------
        /// <summary>Enumeration to determine where it's valid to run the test</summary>
        /// -------------------------------------------------------------------
        public TestLab TestLab { get { return _testLab; } }
        /// -------------------------------------------------------------------
        /// <summary>Enumeration to determine if the test is stable</summary>
        /// -------------------------------------------------------------------
        public TestWorks TestWorks { get { return _testWorks; } }
        /// -------------------------------------------------------------------
        /// <summary>Assigned weight of the test case</summary>
        /// -------------------------------------------------------------------
        public int Weight { get { return _weight; } set { _weight = value; } }

        #endregion Properties

        /// -------------------------------------------------------------------
        /// <summary>Constructor</summary>
        /// -------------------------------------------------------------------
        public StressBuckets()
        {
        }
        /// -------------------------------------------------------------------
        /// <summary>Constructor for custom attribute associated with the 
        /// UIAutomation test</summary>
        /// -------------------------------------------------------------------
        public StressBuckets(string patternName, string methodOrPropertyName, TestLab testLab, TestWorks testWorks)
        {
            _patternName = patternName;
            _methodOrPropertyName = methodOrPropertyName;
            _testLab = testLab;
            _testWorks = testWorks;
            _scenario = Scenario.General;
        }
        /// -------------------------------------------------------------------
        /// <summary>Constructor for custom attribute associated with the 
        /// UIAutomation test</summary>
        /// -------------------------------------------------------------------
        public StressBuckets(string patternName, string methodOrPropertyName, TestLab testLab, TestWorks testWorks, Scenario scenario)
        {
            _patternName = patternName;
            _methodOrPropertyName = methodOrPropertyName;
            _testLab = testLab;
            _testWorks = testWorks;
            _scenario = scenario;
        }
        /// -------------------------------------------------------------------
        /// <summary>Constructor for custom attribute associated with the 
        /// UIAutomation test</summary>
        /// -------------------------------------------------------------------
        public StressBuckets(string patternName, string methodOrPropertyName, TestLab testLab, TestWorks testWorks, int bugNumber)
        {
            _bugNumber = bugNumber;
            _patternName = patternName;
            _methodOrPropertyName = methodOrPropertyName;
            _testLab = testLab;
            _testWorks = TestWorks.NoWorking; // Override and set this
        }
    }
}
