// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Xaml;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Xaml.Utilities
{
    /// <summary>
    /// Helper method for the XamlTest suite
    /// </summary>
    public static class XamlTestHelper
    {
        #region Static Methods

        /// <summary>
        /// Gets the correct numerical separator for the culture
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <returns>char value</returns>
        public static char GetNumericListSeparatorByCulture(CultureInfo culture)
        {
            char ch = ',';
            NumberFormatInfo instance = NumberFormatInfo.GetInstance(culture);
            if ((instance.NumberDecimalSeparator.Length > 0) && (ch == instance.NumberDecimalSeparator[0]))
            {
                ch = ';';
            }

            return ch;
        }

        /// <summary>
        /// Verify that the assembly name and full type name of 'n'th logical child of
        /// element match the assembly name and full type name provided.
        /// NOTE: 'n' starts with 1, and not with 0.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="n">The int n.</param>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <param name="typeFullName">Full name of the type.</param>
        /// <param name="result">if set to <c>true</c> [result].</param>
        /// <returns>FrameworkElement value</returns>
        public static FrameworkElement VerifyChildType(FrameworkElement element, int n, string assemblyName, string typeFullName, ref bool result)
        {
            IEnumerator children = LogicalTreeHelper.GetChildren(element).GetEnumerator();
            for (int i = 0; i < n; i++)
            {
                children.MoveNext();
            }

            object child = children.Current;
            Type type = child.GetType();

            // Verify the type name;
            if (typeFullName != type.FullName)
            {
                GlobalLog.LogEvidence("Child type name did not match: {0} vs. {1}", typeFullName, type.FullName);
                result = false;
            }

            // Verify the assembly
            if (assemblyName != (type.Assembly).GetName().Name)
            {
                GlobalLog.LogEvidence("Assembly name did not match: {0} vs. {1}", assemblyName, (type.Assembly).GetName().Name);
                result = false;
            }

            return child as FrameworkElement;
        }

        /// <summary>
        /// Verify that given element doesn't have any logical children
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="result">if set to <c>true</c> [result].</param>
        public static void VerifyNoChildren(FrameworkElement element, ref bool result)
        {
            VerifyNoChild(element, 1, ref result);
        }

        /// <summary>
        /// Verify that 'n'th logical child doesn't exist for given element
        /// i.e. given element has less than 'n' logical children
        /// NOTE: 'n' starts with 1, and not with 0.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="n">The int n.</param>
        /// <param name="result">if set to <c>true</c> [result].</param>
        public static void VerifyNoChild(FrameworkElement element, int n, ref bool result)
        {
            string errorMesg = "VerifyNoChild failed. " + n + "th child exists for " + element.GetType().Name;
            IEnumerator children = LogicalTreeHelper.GetChildren(element).GetEnumerator();
            int i = 0;
            for (i = 0; i < n; i++)
            {
                if (!children.MoveNext())
                {
                    break;
                }
            }

            // We want to verify that element doesn't have n logical children.
            // If that's true, MoveNext() would return false somewhere in the above loop,
            // hence we would break out of the loop, so i wouldn't reach the value of n
            if (i >= n)
            {
                GlobalLog.LogEvidence("i was >= n, therefore children were found");
                result = false;
            }
        }

        /// <summary>
        /// ContentPropertyHasAttribute ensures that the element's contentProperty's type has the
        /// attribute given by attributeName
        /// </summary>
        /// <param name="element">element whose contentProperty is to be inspected.</param>
        /// <param name="attributeName">string that represents the Attribute contentProperty's type should have</param>
        /// <returns>bool value</returns>
        public static bool ContentPropertyHasAttribute(object element, string attributeName)
        {
            object[] elementAttributes = element.GetType().GetCustomAttributes(true);
            string contentPropertyName = null;

            foreach (object attrib in elementAttributes)
            {
                if (attrib.GetType().ToString().Equals("System.Windows.Markup.ContentPropertyAttribute"))
                {
                    contentPropertyName = ((System.Windows.Markup.ContentPropertyAttribute) attrib).Name;
                }
            }

            if (null != contentPropertyName)
            {
                PropertyInfo propertyInfo = element.GetType().GetProperty(contentPropertyName);
                object[] contentPropertyAttibutes = propertyInfo.PropertyType.GetCustomAttributes(true);
                foreach (object contentAttrib in contentPropertyAttibutes)
                {
                    if (contentAttrib.GetType().ToString().Equals(attributeName))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// GetIEnumeratorForObject tries to return an enumerator for the argument element's children
        /// </summary>
        /// <param name="element">element that will be inspected for children that can be enumerated.</param>
        /// <returns>IEnumerator value</returns>
        public static IEnumerator GetIEnumeratorForObject(object element)
        {
            IEnumerable logicalChildrenEnumerable = null;
            IEnumerator logicalChildrenEnumerator = null;
            if (element is DependencyObject)
            {
                logicalChildrenEnumerable = LogicalTreeHelper.GetChildren((DependencyObject) element);
                logicalChildrenEnumerator = logicalChildrenEnumerable.GetEnumerator();
            }

            return logicalChildrenEnumerator;
        }

        /// <summary>
        /// HasAttribute ensures that the element's type has the
        /// attribute given by attributeName
        /// </summary>
        /// <param name="element">element that will have its type inspected.</param>
        /// <param name="attributeName">string that represents the Attribute the element's type should have</param>
        /// <returns>
        /// <c>true</c> if the specified element has attribute; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasAttribute(object element, string attributeName)
        {
            object[] elementAttributes = element.GetType().GetCustomAttributes(true);
            foreach (object attrib in elementAttributes)
            {
                if (attrib.GetType().ToString().Equals(attributeName))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// GetStringFromControl returns string content in a Control if possible.  Otherwise null.
        /// </summary>
        /// <param name="obj">control to return string from</param>
        /// <returns>string value</returns>
        public static string GetStringFromControl(object obj)
        {
            ContentControl objAsContentControl = obj as ContentControl;
            if (null != objAsContentControl)
            {
                return objAsContentControl.Content as string;
            }

            Run run = obj as Run;
            if (null != run)
            {
                return run.Text;
            }

            InlineUIContainer container = obj as InlineUIContainer;
            if (null != container)
            {
                return GetStringFromControl(container.Child);
            }

            return null;
        }

        /// <summary>
        /// EnsureWhitespaceCollapsedMiddleSiblings inspects the strCollapsed string
        /// to see if it meets xmlSpace=default Whitespace collapsing criteria.
        /// </summary>
        /// <param name="strCollapsed">String to be inspected for meeting criteria.</param>
        /// <param name="prevSiblingHadTrimWS">Boolean that tells if previous sibling wants to trim this string.</param>
        /// <param name="sigCollectionAttrib">Boolean that tells if whitespace is significant in strCollapsed's parent collection.</param>
        /// <param name="whitespaceTrimChars">Char array that contains all acceptable characters to trim as whitespace.</param>
        /// <param name="result">ref result</param>
        public static void EnsureWhitespaceCollapsedMiddleSiblings(string strCollapsed, bool prevSiblingHadTrimWS, bool sigCollectionAttrib, char[] whitespaceTrimChars, ref bool result)
        {
            // ensure that childUieString doesn't have more than a single whitespace on either side
            int strCollapsedLength = strCollapsed.Length;
            string strCollapsedTrimStart = strCollapsed.TrimStart(whitespaceTrimChars);
            string strCollapsedTrimEnd = strCollapsed.TrimEnd(whitespaceTrimChars);
            int strCollapsedTrimStartLength = strCollapsedTrimStart.Length;
            int strCollapsedTrimEndLength = strCollapsedTrimEnd.Length;

            // also takes care of condition: all purewhitespacestring.length == 1
            // how can we be sure we didn't trim too much?  can't ...
            // don't know if original string had 0 length string or not ...
            int tolerance = 1;
            if (!sigCollectionAttrib)
            {
                tolerance = 0; // whenever text abuts non-text in the collection, get trimming
            }

            if (!((strCollapsedLength - strCollapsedTrimStartLength) <= tolerance))
            {
                GlobalLog.LogEvidence("Too much whitespace at start of string! :" + strCollapsed);
                result = false;
            }

            if (!((strCollapsedLength - strCollapsedTrimEndLength) <= tolerance))
            {
                GlobalLog.LogEvidence("Too much whitespace at end of string! :" + strCollapsed);
                result = false;
            }

            if (prevSiblingHadTrimWS) // Add TrimWS checks
            {
                if (0 != (strCollapsedLength - strCollapsedTrimStartLength))
                {
                    GlobalLog.LogEvidence("Too much whitespace at start of first child! :" + strCollapsed);
                    result = false;
                }
            }
        }

        /// <summary>
        /// WSCollapseVerify ensures that all string-content-capable uielements have text that equals testTargetString
        /// Only Checks one deep (really only set up for buttons in a CustomDockPanel for now).
        /// </summary>
        /// <param name="uie">logical tree to inspect</param>
        /// <param name="testTargetString">string that uielements with text-content should match</param>
        /// <param name="result">ref result \</param>
        public static void WSCollapseVerify(UIElement uie, string testTargetString, ref bool result)
        {
            IEnumerable logicalChildrenEnumerable = LogicalTreeHelper.GetChildren(uie);
            IEnumerator logicalChildrenEnumerator = logicalChildrenEnumerable.GetEnumerator();
            while (logicalChildrenEnumerator.MoveNext())
            {
                object child = logicalChildrenEnumerator.Current;
                string childString = GetStringFromControl(child);

                if (((null != childString) || (null != testTargetString)) && !childString.Equals(testTargetString))
                {
                    GlobalLog.LogEvidence(childString + " not equals " + testTargetString);
                    result = false;
                }
            }
        }

        /// <summary>
        /// Searches the currently loaded assemblies for one with the name passed in.
        /// If found, it will return the assembly
        /// </summary>
        /// <param name="asmName">Name of the asm.</param>
        /// <returns>Assembly value</returns>
        public static Assembly FindLoadedAssembly(string asmName)
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly asm in assemblies)
            {
                if (asm.FullName.Contains(asmName))
                {
                    return asm;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns whether a given property is a directive that is omitted from the XAML textual rep
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>bool value</returns>
        public static bool IsImplicit(XamlMember property)
        {
            return DiagnosticWriter.IsImplicit(property);
        }

        #endregion
    }
}
