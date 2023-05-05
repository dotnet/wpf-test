// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.DataServices
{
    using System;
    using System.Threading;
    using System.Windows.Threading;
    using System.Collections;
    using System.Reflection;
    using System.Security.Permissions;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Media;
    using System.Windows.Data;
    using System.Xml;
    using Codeplex;
    using System.Xml.Linq;
    using Microsoft.Test.Threading;

    /// <summary>
    /// Contains utility functions for testing Avalon
    /// </summary>
    public static class Util
    {
        static Util()
        {
            s_cleanupMethodInfo = typeof(BindingOperations).GetMethod("Cleanup", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.InvokeMethod);
        }

        public static void WaitForItemsControlPopulation(ItemsControl control, int timeout)
        {
            DispatcherHelper.DoEvents(0, DispatcherPriority.Background);
            while ((control.Items.Count <= 0) && (timeout-- > 0))
            {
                Thread.Sleep(1000);
                DispatcherHelper.DoEvents(0, DispatcherPriority.Background);
            }
        }

        public static bool WaitForXLinqDataProviderReady(string resourceName, FrameworkElement element, int secondsToWait)
        {
            int repeatCount = 0;
            object data = null;
            XDocument xDoc = null;

            XLinqDataProvider dataSource = (XLinqDataProvider)element.FindResource(resourceName);

            data = dataSource.Data;
            xDoc = dataSource.Document;

            while (data == null || xDoc == null || xDoc.Document == null)
            {
                // the document hasn't loaded yet.  Try again later.
                if (repeatCount < secondsToWait)
                {
                    if ((xDoc == null || xDoc.Document == null) || (data == null))
                    {
                        repeatCount++;
                        Thread.Sleep(1000);
                    }
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        public static bool WaitForXmlDataProviderReady(string resourceName, FrameworkElement element, int secondsToWait)
        {
            int repeatCount = 0;
            object data = null;
            XmlDocument doc = null;

            XmlDataProvider dataSource = (XmlDataProvider)element.FindResource(resourceName);
            data = dataSource.Data;
            doc = dataSource.Document;

            while (data == null || doc == null || doc.DocumentElement == null)
            {
                // the document hasn't loaded yet.  Try again later.
                if (repeatCount < secondsToWait)
                {
                    repeatCount++;
                    if ((doc == null || doc.DocumentElement == null) || (data == null))
                    {
                        Thread.Sleep(1000);
                    }
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Compares 2 objects to determine if they have the same value
        /// </summary>
        /// <param name="a">first object</param>
        /// <param name="b">second object</param>
        /// <returns></returns>
        public static bool CompareObjects(object a, object b)
        {
            // Are they the same object?
            if (a == b)
                return true;

            // Are either null?
            if (a == null || b == null)
                return false;

            // Are they of the same type?
            if (a.GetType() == b.GetType())
            {
                // Check the comparable interface to determine a match
                if (a is IComparable)
                {
                    return Comparer.Default.Compare(a, b) == 0;
                }
                else
                {
                    // Check the object equals to determine a match
                    return a.Equals(b);
                }
            }
            // No match, default to false
            return false;
        }

        /// <summary>
        ///  Throws if the string passed as parameters are not equal
        /// </summary>
        public static void AssertEquals(object obj1, object obj2)
        {
            if (!(Object.Equals(obj1, obj2)))
            {
                throw new TestException("Error in test case. " + obj1.ToString() + " does not equal " + obj2.ToString());
            }
        }

        /// <summary>
        /// Assert that objects are equal. The phrase "Expected: x  Got: y" is automatically added to the message.
        /// </summary>
        /// <param name="expected">expected value</param>
        /// <param name="actual">actual value</param>
        /// <param name="message">message to display if assert fails</param>
        /// <param name="arg">args for format tags in message</param>
        public static void AssertEquals(object actual, object expected, string message)
        {
            if (!Object.Equals(expected, actual))
            {
                if (expected == null) expected = "NULL";
                if (actual == null) actual = "NULL";
                message += String.Format(" Expected: {0}  Got: {1}", expected, actual);
                System.Diagnostics.Debug.Assert(false, message);
            }
        }

        public static long GetMemory()
        {
            long result = GC.GetTotalMemory(true);
            GC.WaitForPendingFinalizers();
            while ((bool)s_cleanupMethodInfo.Invoke(null, null))
            {
                result = GC.GetTotalMemory(true);
                GC.WaitForPendingFinalizers();
            }
            return result;
        }

        #region Tree-walking helpers

        /// <summary>
        /// Finds the first FrameworkElement in the VisualTree that has the specified Name
        /// </summary>
        /// <param name="element">Visual Element to search from</param>
        /// <param name="id">Name of the element to look for</param>
        /// <returns>The found element</returns>
        public static FrameworkElement FindElement(FrameworkElement element, string id)
        {
            FrameworkElement[] elements = FindElements(element, id);

            if (elements.Length > 0)
                return elements[0];

            return null;
        }

        /// <summary>
        /// Finds all the FrameworkElements in the VisualTree that has the specified Name
        /// </summary>
        /// <param name="element">Visual Element to search from</param>
        /// <param name="id">Name of the elements to look for</param>
        /// <returns>array of the elements found</returns>
        public static FrameworkElement[] FindElements(FrameworkElement element, string id)
        {
            ArrayList list = new ArrayList();
            FindElements(element, id, list);
            return (FrameworkElement[])list.ToArray(typeof(FrameworkElement));
        }

        static void FindElements(FrameworkElement element, string id, ArrayList list)
        {
            if (element.Name == id)
                list.Add(element);

            //make sure that the visual tree is avalible
            element.ApplyTemplate();

            //search all the children
            int count = VisualTreeHelper.GetChildrenCount(element);
            for (int i = 0; i < count; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(element, i);
                if (child is FrameworkElement)
                    FindElements((FrameworkElement)child, id, list);
            }
            //Connect the Popup visual tree
            if (element is Popup)
                FindElements(((Popup)element).Child as FrameworkElement, id, list);
        }

        /// <summary>
        /// Finds all the FrameworkElements in the VisualTree with the specified type
        /// </summary>
        /// <param name="element">Visual Element to search from</param>
        /// <param name="type">Type of the elements to look for</param>
        /// <returns>array of the elements found</returns>
        public static FrameworkElement[] FindElementsWithType(FrameworkElement element, Type type)
        {
            ArrayList list = new ArrayList();
            FindElementsWithType(element, type, list);
            return (FrameworkElement[])list.ToArray(typeof(FrameworkElement));
        }

        static void FindElementsWithType(FrameworkElement element, Type type, ArrayList list)
        {
            if (element.GetType().Equals(type))
                list.Add(element);

            //make sure that the visual tree is avalible
            element.ApplyTemplate();

            //search all the children
            int count = VisualTreeHelper.GetChildrenCount(element);
            for (int i = 0; i < count; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(element, i);
                if (child is FrameworkElement)
                    FindElementsWithType((FrameworkElement)child, type, list);
            }
            //Connect the Popup visual tree
            if (element is Popup)
                FindElementsWithType(((Popup)element).Child as FrameworkElement, type, list);
        }

        /// <summary>
        /// Finds the first FrameworkElement in the VisualTree that has the specified Name
        /// </summary>
        /// <param name="element">Visual Element to search from</param>
        /// <param name="id">Name of the element to look for</param>
        /// <returns>The found element</returns>
        public static FrameworkElement FindDataVisual(FrameworkElement element, object data)
        {
            FrameworkElement[] elements = FindDataVisuals(element, new object[] { data });

            if (elements.Length > 0)
                return elements[0];

            return null;
        }

        /// <summary>
        /// Finds all the FrameworkElements in the VisualTree that has the specified Name
        /// </summary>
        /// <param name="element">Visual Element to search from</param>
        /// <param name="id">Name of the elements to look for</param>
        /// <returns>array of the elements found</returns>
        public static FrameworkElement[] FindDataVisuals(FrameworkElement element, IEnumerable dataCollection)
        {
            ArrayList list = new ArrayList();

            FindDataVisuals(element, dataCollection, list);
            return (FrameworkElement[])list.ToArray(typeof(FrameworkElement));
        }

        static void FindDataVisuals(FrameworkElement element, IEnumerable dataCollection, ArrayList list)
        {
            //make sure that the visual tree is avalible
            element.ApplyTemplate();
            int count;
            if (element is ContentPresenter)
            {
                ContentPresenter cp = (ContentPresenter)element;

                foreach (object item in dataCollection)
                {
                    count = VisualTreeHelper.GetChildrenCount(element);

                    if (cp.Content == item && count > 0)
                    {
                        list.Add(VisualTreeHelper.GetChild(element, 0));
                        break;
                    }
                }
            }

            //search all the children
            count = VisualTreeHelper.GetChildrenCount(element);
            for (int i = 0; i < count; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(element, i);
                if (child is FrameworkElement)
                    FindDataVisuals((FrameworkElement)child, dataCollection, list);
            }

            //Connect the Popup visual tree
            if (element is Popup)
                FindDataVisuals(((Popup)element).Child as FrameworkElement, dataCollection, list);
        }

        /// <summary>
        /// Do a depth-first search of the visual tree looking for a node with
        /// a given property value.
        /// </summary>
        /// <param name="dp">property to query</param>
        /// <param name="value">desired value</param>
        /// <param name="node">starting node for the search</param>
        /// <param name="includeNode">if false, do not test the node itself</param>
        /// <returns></returns>
        public static DependencyObject FindVisualByPropertyValue(DependencyProperty dp, object value, DependencyObject node, bool includeNode)
        {
            // see if the node itself has the right value
            if (includeNode)
            {
                object nodeValue = node.GetValue(dp);
                if (Object.Equals(value, nodeValue))
                    return node;
            }

            // if not, recursively look at the visual children
            int count = VisualTreeHelper.GetChildrenCount(node);
            for(int i = 0; i < count; i++)
            {
                // Common base class for Visual and Visual3D is DependencyObject
                DependencyObject result = FindVisualByPropertyValue(dp, value, VisualTreeHelper.GetChild(node,i), true);
                if (result != null)
                    return result;
            }

            // not found
            return null;
        }

        /// <summary>
        /// Do a depth-first search of the visual tree looking for a node with
        /// a given property value.
        /// </summary>
        /// <param name="dp">property to query</param>
        /// <param name="value">desired value</param>
        /// <param name="node">starting node for the search</param>
        /// <returns></returns>
        public static DependencyObject FindVisualByPropertyValue(DependencyProperty dp, object value, DependencyObject node)
        {
            return FindVisualByPropertyValue(dp, value, node, true);
        }

        /// <summary>
        /// Do a depth-first search of the visual tree (starting at the root)
        /// looking for a node with a given property value.
        /// </summary>
        /// <param name="dp">property to query</param>
        /// <param name="value">desired value</param>
        /// <returns></returns>
        /// <example>
        /// For example, to find the element with ID "foo", call
        ///  DRT.FindVisualByPropertyValue(IDProperty, "foo");
        /// </example>
        public static DependencyObject FindVisualByPropertyValue(DependencyProperty dp, object value, FrameworkElement RootElement)
        {
            return FindVisualByPropertyValue(dp, value, RootElement);
        }

        /// <summary>
        /// Do a depth-first search of the visual tree looking for a node with
        /// a given type.
        /// </summary>
        /// <param name="type">type of desired node</param>
        /// <param name="node">starting node for the search</param>
        /// <param name="includeNode">if false, do not test the node itself</param>
        public static DependencyObject FindVisualByType(Type type, DependencyObject node, bool includeNode)
        {
            // see if the node itself has the right type
            if (includeNode)
            {
                if (type == node.GetType())
                    return node;
            }

            // if not, recursively look at the visual children
            int count = VisualTreeHelper.GetChildrenCount(node);
            for(int i = 0; i < count; i++)
            {
                // Common base class for Visual and Visual3D is DependencyObject
                DependencyObject result = FindVisualByType(type,VisualTreeHelper.GetChild(node,i), true);
                if (result != null)
                    return result;
            }

            // not found
            return null;
        }

        /// <summary>
        /// Do a depth-first search of the visual tree looking for a node with
        /// a given type.
        /// </summary>
        /// <param name="type">type of desired node</param>
        /// <param name="node">starting node for the search</param>
        public static DependencyObject FindVisualByType(Type type, DependencyObject node)
        {
            return FindVisualByType(type, node, true);
        }

        /// <summary>
        /// Do a depth-first search of the visual tree looking for a node with
        /// a given type.
        /// </summary>
        /// <param name="type">type of desired node</param>
        /// <param name="node">starting node for the search</param>
        /// <param name="includeNode">if false, do not test the node itself</param>
        /// <param name="skip">Skip this number of matches.</param>
        public static DependencyObject FindVisualByType(Type type, DependencyObject node, bool includeNode, int skip)
        {
            return FindVisualByType(type, node, includeNode, ref skip);
        }
        static DependencyObject FindVisualByType(Type type, DependencyObject node, bool includeNode, ref int skip)
        {
            // see if the node itself has the right type
            if (includeNode)
            {
                if (type == node.GetType())
                {
                    if (skip == 0)
                    {
                        return node;
                    }
                    else
                    {
                        skip--;
                    }
                }
            }

            // if not, recursively look at the visual children
            int count = VisualTreeHelper.GetChildrenCount(node);
            for (int i = 0; i < count; i++)
            {
                DependencyObject result = FindVisualByType(type, VisualTreeHelper.GetChild(node, i), true, ref skip);
                if (result != null)
                    return result;
            }

            // not found
            return null;
        }

        /// <summary>
        /// Do a depth-first search of the visual tree looking for a node with
        /// a given ID.
        /// </summary>
        /// <param name="id">id of desired node</param>
        /// <param name="node">starting node for the search</param>
        public static DependencyObject FindVisualByID(string id, DependencyObject node)
        {
            return FindVisualByPropertyValue(FrameworkElement.NameProperty, id, node);
        }

        /// <summary>
        /// Do a depth-first search of the visual tree looking for a node with
        /// a given ID.
        /// </summary>
        /// <param name="id">id of desired node</param>
        public static DependencyObject FindVisualByID(string id, FrameworkElement RootElement)
        {
            return FindVisualByID(id, RootElement);
        }

        /// <summary>
        /// Find the childitem withint a given visual tree
        /// </summary>
        /// <typeparam name="childItem">child item type</typeparam>
        /// <param name="obj">DO to start out</param>
        /// <returns></returns>
        public static childItem FindVisualChild<childItem>(DependencyObject obj) where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);

                if (child != null && child is childItem)
                    return (childItem)child;
                else
                {
                    childItem childOfChild = FindVisualChild<childItem>(child);

                    if (childOfChild != null)
                        return childOfChild;
                }
            }

            return null;
        }

        /// <summary>
        /// For ComboBox tests - make sure the parent of the CP returned is the ComboBox, not something else
        /// </summary>
        /// <param name="cb">the combobox to work with</param>
        /// <returns>CP that has the cb as its TempalatedParent</returns>
        public static ContentPresenter GetSelectionBox(ComboBox cb)
        {
            ContentPresenter cp = Util.FindVisualByType(typeof(ContentPresenter), cb) as ContentPresenter;
            while (cp != null)
            {
                if (cp.TemplatedParent == cb)
                    return cp;

                cp = Util.FindVisualByType(typeof(ContentPresenter), cb) as ContentPresenter;
            }

            return null;
        }

        /// <summary>
        /// Find the control in the data template for ItemsControl
        /// </summary>
        /// <typeparam name="childControl">the child control to find, i.e. TextBlock</typeparam>
        /// <param name="lb">the listbox control</param>
        /// <returns>the child control in template</returns>
        public static childControl FindControlByTypeInTemplate<childControl>(DependencyObject lbi) where childControl : DependencyObject
        {
            ContentPresenter contentPresenter;
            DataTemplate dataTemplate;

            contentPresenter = FindVisualChild<ContentPresenter>(lbi);
            dataTemplate = contentPresenter.ContentTemplate;
            return (childControl)Util.FindVisualByType(typeof(childControl), contentPresenter);
        }

        #endregion

        private static MethodInfo s_cleanupMethodInfo;
    }

    public class TestException : Exception
    {
        public TestException(string str)
            : base(str)
        {
        }

    }

}

