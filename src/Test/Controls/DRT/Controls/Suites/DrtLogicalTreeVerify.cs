// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Xml;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Markup;
using System.Windows.Threading;

namespace DRT
{
    public class DrtLogicalTreeVerifySuite : DrtTestSuite
    {
        public DrtLogicalTreeVerifySuite() : base("LogicalTreeVerify")
        {
            Contact = "Microsoft";
        }

        public override DrtTest[] PrepareTests()
        {
            Border border = new Border();
            border.Background = Brushes.White;
            DRT.RootElement = border;

            StackPanel panel = new StackPanel();
            panel.Orientation = System.Windows.Controls.Orientation.Horizontal;
            border.Child = panel;

            TextBlock title = new TextBlock();
            title.Text = "Verifying the Logical Tree...";
            panel.Children.Add(title);

            DRT.ShowRoot();

            return new DrtTest[]
            {
                new DrtTest(LoadTypes),
                new DrtTest(BuildTree),
                new DrtTest(VerifyTree),
            };
        }


        /// <summary>
        ///     Loads all the type data for the Avalon assemblies.
        ///     Excludes types listed in an exclude file.
        /// </summary>
        private void LoadTypes()
        {
            Console.WriteLine();
            Console.WriteLine("---------- Loading Types ----------");

            // Read the excludes file, if it exists and store the names in a list
            if (File.Exists(ExcludesFileName))
            {
                FileStream excludesFile = File.Open(ExcludesFileName, FileMode.Open, FileAccess.Read);
                if (excludesFile != null)
                {
                    try
                    {
                        XmlTextReader reader = new XmlTextReader(excludesFile);
                        while (reader.Read())
                        {
                            if (reader.Name == "Type")
                            {
                                if (reader.HasAttributes)
                                {
                                    if (reader.MoveToFirstAttribute())
                                    {
                                        if (reader.Name == "Name")
                                        {
                                            // This a name of a type to exclude
                                            string name = reader.Value;
                                            _excludes.Add(name);

                                            Console.WriteLine(String.Format("Exclude requested for {0}", name));
                                        }
                                        reader.MoveToElement();
                                    }
                                }
                            }
                        }
                    }
                    finally
                    {
                        excludesFile.Close();
                    }
                }
            }

            // These are known types to make loading the assembly data easier
            Type[] knownTypes = {
                typeof(Dispatcher),         // WindowsBase
                typeof(Visual),             // PresentationCore
                typeof(FrameworkElement),   // PresentationFramework
                typeof(AutomationElement),  // WindowsUIAutomation
            };

            // Go through the assemblies and pull out their types
            for (int i = 0; i < knownTypes.Length; i++)
            {
                Type[] types = knownTypes[i].Assembly.GetTypes();

                for (int x = 0; x < types.Length; x++)
                {
                    if (!_excludes.Contains(types[x].FullName))
                    {
                        _types.Add(types[x]);
                    }
                    else
                    {
                        Console.WriteLine("Excluded " + types[x].FullName);
                    }
                }
            }
        }

        private void PreCheck()
        {
            // We are going to test the logical tree enumerators for
            // ContentControl, ItemsControl, HeaderedItemsControl, and HeaderedContentControl.
            // Each one will first check the empty state then check with FrameworkElement(s) added.

            // --- ContentControl ---

            ContentControl cc = new ContentControl();
            IEnumerator e = LogicalTreeHelper.GetChildren(cc).GetEnumerator();
            DRT.Assert(!e.MoveNext(), "Empty ContentControl's logical tree enumerator responded to MoveNext()");

            cc.Content = new FrameworkElement();

            e = LogicalTreeHelper.GetChildren(cc).GetEnumerator();
            DRT.Assert(e.MoveNext(), "Content not reachable in ContentControl");

            FrameworkElement child = e.Current as FrameworkElement;
            DRT.Assert(child != null, "ContentControl child not round-tripped");

            DRT.Assert(LogicalTreeHelper.GetParent(child) == cc, "ContentControl child's parent is not the ContentControl");

            DRT.Assert(!e.MoveNext(), "Went past end of ContentControl's children");

            // --- ItemsControl --

            ItemsControl ic = new ItemsControl();
            e = LogicalTreeHelper.GetChildren(ic).GetEnumerator();
            DRT.Assert(!e.MoveNext(), "Empty ItemsControl responded to MoveNext");

            ic.Items.Add(new FrameworkElement());
            ic.Items.Add(new FrameworkElement());

            e = LogicalTreeHelper.GetChildren(ic).GetEnumerator();
            for (int count = 0; count < 2; count++)
            {
                DRT.Assert(e.MoveNext(), "ItemsControl didn't enumerate all children");

                child = e.Current as FrameworkElement;
                DRT.Assert(child != null, "ItemsControl child didn't round-trip");

                DRT.Assert(LogicalTreeHelper.GetParent(child) == ic, "ItemsControl child's parent not set to ItemsControl");
            }

            DRT.Assert(!e.MoveNext(), "Moved past end of ItemsControl");

            // --- HeaderedItemsControl ---

            HeaderedItemsControl hic = new HeaderedItemsControl();
            e = LogicalTreeHelper.GetChildren(hic).GetEnumerator();
            DRT.Assert(!e.MoveNext(), "Empty HeaderedItemsControl responded to MoveNext");

            hic.Header = new FrameworkElement();
            hic.Items.Add(new FrameworkElement());
            hic.Items.Add(new FrameworkElement());

            e = LogicalTreeHelper.GetChildren(hic).GetEnumerator();
            for (int count = 0; count < 3; count++)
            {
                DRT.Assert(e.MoveNext(), "HeaderedItemsControl didn't enumerate all children");

                child = e.Current as FrameworkElement;
                DRT.Assert(child != null, "HeaderedItemsControl child didn't round-trip");

                DRT.Assert(LogicalTreeHelper.GetParent(child) == hic, "HeaderedItemsControl child's parent not set to ItemsControl");

                DRT.Assert((count != 0) || (child == hic.Header), "HeaderedItemsControl's Header in the wrong location");
            }
            DRT.Assert(!e.MoveNext(), "Moved past end of HeaderedItemsControl");

            // --- HeaderedItemsControl ---

            HeaderedContentControl hcc = new HeaderedContentControl();
            e = LogicalTreeHelper.GetChildren(hcc).GetEnumerator();
            DRT.Assert(!e.MoveNext(), "Empty HeaderedContentControl responded to MoveNext");

            hcc.Header = new FrameworkElement();
            hcc.Content = new FrameworkElement();

            e = LogicalTreeHelper.GetChildren(hcc).GetEnumerator();
            for (int count = 0; count < 2; count++)
            {
                DRT.Assert(e.MoveNext(), "HeaderedContentControl didn't enumerate all children");

                child = e.Current as FrameworkElement;
                DRT.Assert(child != null, "HeaderedContentControl child didn't round-trip");

                DRT.Assert(LogicalTreeHelper.GetParent(child) == hcc, "HeaderedContentControl child's parent not set to ItemsControl");

                DRT.Assert((count != 0) || (child == hcc.Header), "HeaderedContentControl's Header in the wrong location");
            }
            DRT.Assert(!e.MoveNext(), "Moved past end of HeaderedContentControl");
        }

        /// <summary>
        ///     Builds a visual tree of all the types that are tree elements that could contain logical children
        /// </summary>
        private void BuildTree()
        {
            Console.WriteLine();
            Console.WriteLine("------- Performing Pre-Check ------");

            PreCheck();

            Console.WriteLine();
            Console.WriteLine("---------- Building Tree ----------");

            TestPanel panel = new TestPanel();

            Type elementType = typeof(UIElement);

            for (int i = 0; i < _types.Count; i++)
            {
                Type type = _types[i];

                if (elementType.IsAssignableFrom(type))
                {
                    // We have a UIElement

                    Console.WriteLine(type.ToString());

                    UIElement e = null;
                    try
                    {
                        // Create an instance of the element
                        e = (UIElement)Activator.CreateInstance(type);
                        Console.WriteLine(" - Created");
                    }
                    catch (Exception)
                    {
                        Console.WriteLine(" - Unable to create (Exception thrown)");
                    }

                    if (e != null)
                    {
                        // We have an instance of a UIElement

                        IAddChild addChild = e as IAddChild;
                        if (addChild != null)
                        {
                            // It is both FE/FCE and IAddChild

                            VerifyChild child = new VerifyChild();
                            bool verifyChild = false;

                            try
                            {
                                // Try to add a child

                                addChild.AddChild(child);


                                // Make sure that we can get to that logical child
                                // and that it isn't just "lost"
                                IEnumerator reader = LogicalTreeHelper.GetChildren(e).GetEnumerator();
                                if (reader != null)
                                {
                                    while (reader.MoveNext())
                                    {
                                        object o = reader.Current;
                                        if (o == child)
                                        {
                                            verifyChild = true;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    Console.WriteLine(" - Null Logical Children found");
                                }
                            }
                            catch (Exception)
                            {
                                VerifyChild.ReturnOne();
                                child = null;
                                Console.WriteLine(" - Rejected, unable to add child");
                            }

                            if (child != null)
                            {
                                if (verifyChild)
                                {
                                    // The child was added and we were able to access it

                                    try
                                    {
                                        // Add the subtree to the panel

                                        panel.Children.Add(e);
                                        Console.WriteLine(" - Added");
                                    }
                                    catch (Exception)
                                    {
                                        Console.WriteLine(" - Rejected, unable to add to panel");
                                        if (panel.Children.Contains(e))
                                        {
                                            panel.Children.Remove(e);
                                        }
                                    }
                                }
                                else
                                {
                                    VerifyChild.ReturnOne();
                                    child = null;

                                    Console.WriteLine(" - Rejected, unable to get to added child through Logical Children");
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine(" - Rejected, not IAddChild");
                        }
                    }
                }
            }

            // Hook up the panel to the window
            DRT.Show(panel);

            Console.WriteLine(String.Format("{0} Children Created", VerifyChild.GlobalID));
        }

        // The name of the type that we're currently testing
        private string CurrentTypeName
        {
            get { return _currentTypeName; }
            set { _currentTypeName = value; }
        }

        /// <summary>
        ///     Verify that the logical tree doesn't hit a child twice
        /// </summary>
        private void VerifyTree()
        {
            Console.WriteLine();
            Console.WriteLine("---------- Verifying Tree ----------");

            int[] visited = new int[VerifyChild.GlobalID];
            TestPanel root = (TestPanel)DRT.RootElement;

            for (int i = 0; i < root.Children.Count; i++)
            {
                Visual child = root.Children[i];
                CurrentTypeName = child.GetType().ToString();
                TraverseVisual(child, ref visited, 0);
            }
        }

        private void TraverseVisual(DependencyObject e, ref int[] visited, int depth)
        {
            string indent = new string(' ', depth);
            Console.WriteLine(indent + e.GetType().ToString());

            if (e != null)
            {
                // Go down the logical tree
                TraverseLogical(e, ref visited);
            }

            // Go down the visual tree
            int count = VisualTreeHelper.GetChildrenCount(e);
            for(int i = 0; i < count; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(e,i);
                TraverseVisual(child, ref visited, depth + 1);
            }
        }

        private void TraverseLogical(DependencyObject node, ref int[] visited)
        {
            IEnumerator reader = LogicalTreeHelper.GetChildren(node).GetEnumerator();
            while ((reader != null) && reader.MoveNext())
            {
                object o = reader.Current;

                VerifyChild verifyChild = o as VerifyChild;
                if (verifyChild != null)
                {
                    // We found one of the children we added.
                    // Make sure that this is the first time we found it.

                    int index = verifyChild.ChildID;
                    visited[index]++;
                    if (visited[index] > 1)
                    {
                        string msg = String.Format("Child #{0} encountered {4} times. logical parent = {1}; visual parent = {2}; current node = {3}, current context = {5}", new object[] { verifyChild.ChildID, verifyChild.Parent.GetType().ToString(), VisualTreeHelper.GetParent(verifyChild).GetType().ToString(), node.GetType().ToString(), visited[index], CurrentTypeName });

                        if (!_excludes.Contains(CurrentTypeName))
                        {
                            DRT.Assert(false, msg);
                        }
                        else
                        {
                            Console.WriteLine("IGNORED: " + msg);
                        }
                    }
                }

                DependencyObject logicalChild = o as DependencyObject;
                if (logicalChild != null)
                {
                    // We found a logical child that should have a parent pointer, verify it

                    DependencyObject parent = LogicalTreeHelper.GetParent(logicalChild);
                    if (parent != node)
                    {
                        string msg = String.Format("Child's ({0}) logical parent ({1}) does not match a parent's ({2}) logical children. current context = {3}", new object[] { logicalChild.GetType().ToString(), parent != null ? parent.GetType().ToString() : "null", node.GetType().ToString(), CurrentTypeName });
                        if (!_excludes.Contains(CurrentTypeName))
                        {
                            DRT.Assert(false, msg);
                        }
                        else
                        {
                            Console.WriteLine("IGNORED: " + msg);
                        }
                    }
                }
            }
        }

        private const string ExcludesFileName = @"DrtFiles\Controls\DrtLogicalTreeVerify.xml";

        private string _currentTypeName = String.Empty;
        private List<Type> _types = new List<Type>(200);
        private List<string> _excludes = new List<string>(10);

    }

    /// <summary>
    ///     This panel will let all children display at 0,0 with the available room.
    /// </summary>
    public class TestPanel : Panel
    {
        public TestPanel() : base()
        {
        }

        protected override Size MeasureOverride(Size constraint)
        {
            foreach (UIElement child in Children)
            {
                child.Measure(constraint);
            }

            if (double.IsPositiveInfinity(constraint.Width) || double.IsPositiveInfinity(constraint.Height)
                || double.IsNaN(constraint.Width) || double.IsNaN(constraint.Height))
            {
                return new Size();
            }

            return constraint;
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            foreach (UIElement child in Children)
            {
                child.Arrange(new Rect(child.DesiredSize));
            }
            return arrangeSize;
        }
    }

    /// <summary>
    ///     Child class used in verifying the logical tree.
    /// </summary>
    public class VerifyChild : FrameworkElement
    {
        public VerifyChild() : base()
        {
            _id = NextID();
        }

        /// <summary>
        ///     This is the unique ID of this child
        /// </summary>
        public int ChildID
        {
            get
            {
                return _id;
            }
        }

        /// <summary>
        ///     This is the ID to be used on the next instance of this class.
        ///     It is also the count of the number of instances of this class.
        /// </summary>
        public static int GlobalID
        {
            get
            {
                return s_globalID;
            }
        }

        // Something happened to one of the children, decrement the count
        public static void ReturnOne()
        {
            s_globalID--;
        }

        private static int NextID()
        {
            // We're not worrying about thread safety here because it's only
            // in a test that is single threaded.
            return s_globalID++;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            return new Size(2.0, 2.0);
        }

        private static int s_globalID = 0;
        private int _id;
    }
}

