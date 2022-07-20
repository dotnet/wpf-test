// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using DRT;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Media.Animation;

namespace DRTFreezable
{
    public class DRTFreezable : DrtBase
    {
        private static int s_repeatCount = 1;
        private const int c_perfRepeatCount = 100000;

        [STAThread]
        public static int Main(string[] args)
        {
            DrtBase drt = new DRTFreezable();

            return drt.Run(args);
        }

        private DRTFreezable()
        {
            DrtName = "DRTFreezable";
            TeamContact = "WPF";
            Contact = "Microsoft";
            Suites = new DrtTestSuite[] {
                new FreezableTestSuite()
            };
        }

        protected override bool HandleCommandLineArgument(string arg, bool option, string[] args, ref int k)
        {
            // start by giving the base class the first chance
            if (base.HandleCommandLineArgument(arg, option, args, ref k))
            {
                return true;
            }

            // process your own arguments here, using these parameters:
            //      arg     - current argument
            //      option  - true if there was a leading - or /.
            //      args    - all arguments
            //      k       - current index into args
            // Here's a typical sketch:

            if (option)
            {
                switch (arg)    // arg is lower-case, no leading - or /
                {
                    case "perf":
                        s_repeatCount = c_perfRepeatCount;
                        break;

                    default:                // unknown option.  don't handle it
                        return false;
                }
                return true;
            }

            return false;
        }

        protected override void PrintOptions()
        {
            Console.WriteLine("Options:");
            Console.WriteLine("  -perf  run test in a loop and display perf data.");
            base.PrintOptions();
        }

        public int RepeatCount
        {
            get
            {
                return s_repeatCount;
            }
        }
    }

    public sealed partial class FreezableTestSuite : DrtTestSuite
    {
        public FreezableTestSuite() : base("FreezableTestSuite") {}

        public override DrtTest[] PrepareTests()
        {
            return new DrtTest[]
            {
                new DrtTest(FreezablesTest),
                new DrtTest(MutableDefaultTest),
                new DrtTest(FreezingUnfreezableDPsTest),
                new DrtTest(CodegenedFreezablesTest),
                new DrtTest(AttachedAndExpressionTest),
                new DrtTest(CloneTest),
                new DrtTest(FreezableCollection)
            };
        }

        private void FreezablesTest()
        {
            TestFreezable testValue1 = new TestFreezable();
            testValue1.Freeze();

            TestFreezable testValue2 = new TestFreezable();
            testValue2.Freeze();

            PropertyDescriptor simpleProperty = TypeDescriptor.GetProperties(typeof(TestFreezable))["SimpleProperty"];
            PropertyDescriptor nestedProperty = TypeDescriptor.GetProperties(typeof(TestFreezable))["NestedProperty"];
            PropertyDescriptor simpleDP = TypeDescriptor.GetProperties(typeof(TestFreezable))["SimpleDP"];
            PropertyDescriptor nestedDP = TypeDescriptor.GetProperties(typeof(TestFreezable))["NestedDP"];

            long startTime = Environment.TickCount;

            for (int i = 0; i < DRT.RepeatCount; i++)
            {
                TestProperty(simpleProperty, 1, 2);
                TestProperty(nestedProperty, testValue1, testValue2);
                TestProperty(nestedProperty, testValue1.Clone(), testValue2.Clone());

/*
                TestProperty(simpleDP, true, false);
                TestProperty(nestedDP, testValue1, testValue2);
                TestProperty(nestedDP, testValue1.Clone(), testValue2.Clone());
*/
            }

            // Verify that the OnChanged virtual is called for direct and sub property changes.

            TestFreezable tf = new TestFreezable();

            tf.NestedProperty = new TestFreezable();
            DRT.Assert(tf.ChangedCount == 1, "OnChanged not called during direct property change.");

            tf.NestedProperty.SimpleProperty = 17;
            DRT.Assert(tf.ChangedCount == 2, "OnChanged not called during sub property change.");

            long endTime = Environment.TickCount;

            if (DRT.RepeatCount > 1)
            {
                Console.WriteLine("Tick count: {0}", endTime - startTime);
            }
        }

        private void TestProperty(PropertyDescriptor property, object value1, object value2)
        {
            TestFreezable orig = new TestFreezable();

            // We will use this changed handler to verify that modifying the copy
            // does not change the original.
            bool changed = false;

            // Hook up a changed handler
            orig.Changed += delegate
            {
                changed = true;
            };

            TestProperty_Helper(orig, property, value1, value2);

            DRT.Assert(changed, "TestProperty_Helper should have caused a change.");
            changed = false;

            TestFreezable copy = orig.Clone();

            DRT.AssertEqual(value1, property.GetValue(copy), "Value of " + property.Name + " was not copied.");

            TestProperty_Helper(copy, property, value1, value2);

            DRT.Assert(!changed, "Modifying the copy should not have changed the original.");
        }

        private void TestProperty_Helper(TestFreezable tf, PropertyDescriptor property, object value1, object value2)
        {
            // Verify initial state
            DRT.Assert(!tf.IsFrozen, "TestFreezable should not be created frozen.");
            DRT.Assert(tf.CanFreeze, "TestFreezable should be able to be frozen on creation.");

            bool changed = false;
            TestFreezable nestedValue = value1 as TestFreezable;

            // Hook up a changed handler
            tf.Changed += delegate
            {
                changed = true;
            };

            // Set property
            DRT.Assert(!changed, "changed should be false, test flawed.");
            property.SetValue(tf, value1);

            // Verify Changed was raised
            DRT.Assert(changed, "Setting property should have raised changed.");
            changed = false;

            DRT.AssertEqual(value1, property.GetValue(tf), property.Name + " did not return the value set.");

            // Verify Changed was not raised
            DRT.Assert(!changed, "Reading simple property should not have raised changed.");

            // If property contains a nested TestFreezable, verify that change
            // notifications propagate.
            if (nestedValue != null && !nestedValue.IsFrozen)
            {
                nestedValue.SimpleProperty = 42;
                DRT.Assert(changed, "Setting simple property of nested freezable should have raised changed.");
                changed = false;

                nestedValue.NestedProperty = new TestFreezable();
                DRT.Assert(changed, "Setting simple property of nested freezable should have raised changed.");
                changed = false;
            }

            // Freeze the Freezable and verify its immutable status
            tf.Freeze();
            DRT.Assert(tf.IsFrozen, "TestFreezable should be frozen.");

            DRT.AssertEqual(value1, property.GetValue(tf), property.Name + " did not return the value set after being frozen.");

            // If the property containes a nested TestFreezable, verify that
            // that the freezing propagated.
            if (nestedValue != null)
            {
                DRT.Assert(nestedValue.IsFrozen, "Freeze did not propogate to nested property " + property.Name);
            }

            // Verify Changed was raised
            DRT.Assert(changed, "Freezing should have raised changed.");
            changed = false;

            // Test attempt to write to
            DRT.Assert(CheckForException(
                delegate
                {
                    property.SetValue(tf, value2);
                }
                ), "Expected InvalidOperationException for modifying a frozen Freezable.");

            // Test changed handler
            DRT.Assert(CheckForException(
                delegate
                {
                    tf.Changed += delegate
                    {
                        DRT.Assert(false, "This delegate should never be invoked.");
                    };
                }
                ), "Expected InvalidOperationException for adding a changed handler to a frozen Freezable.");

            DRT.Assert(!changed, "The frozen Freezable should not have changed.");
        }

        private void MutableDefaultTest()
        {
            DrawingGroup group = new DrawingGroup();

            // The metadata default value should be Frozen
            DRT.Assert(((Freezable)DrawingGroup.ChildrenProperty.GetMetadata(group).DefaultValue).IsFrozen);

            // Verify that it is default
            DRT.Assert(group.ReadLocalValue(DrawingGroup.ChildrenProperty) == DependencyProperty.UnsetValue);

            DrawingCollection mutableCollection = group.Children;

            // Reading the value through GetValue should give an unfrozen copy
            DRT.Assert(!mutableCollection.IsFrozen);

            // After reading it once, it still should be default
            DRT.Assert(group.ReadLocalValue(DrawingGroup.ChildrenProperty) == DependencyProperty.UnsetValue);

            DrawingCollection mutableCollection2 = group.Children;

            // Two GetValues should return the same reference
            DRT.Assert(mutableCollection == mutableCollection2);

            mutableCollection.Add(new GeometryDrawing());

            // Adding to the mutable default should have promoted it to local
            DRT.Assert(group.ReadLocalValue(DrawingGroup.ChildrenProperty) == mutableCollection);

            // Local promotion should have cleared the default value cache. Let's
            // reset the value to default and see that we get a different mutable default
            // value back
            group.ClearValue(DrawingGroup.ChildrenProperty);
            DrawingCollection mutableCollection3 = group.Children;
            DRT.Assert(mutableCollection3 != mutableCollection);

            // Changing the property to something else and then changing the mutable
            // default reference should not promote it since a new local value
            // has been set
            group.Children = new DrawingCollection();
            mutableCollection3.Add(new GeometryDrawing());
            DRT.Assert(group.Children != mutableCollection3);

            // Databind to the mutable default
            group.ClearValue(DrawingGroup.ChildrenProperty);
            Binding binding = new Binding("Children");
            binding.Source = group;
            DrawingGroup group2 = new DrawingGroup();
            BindingOperations.SetBinding(group2, DrawingGroup.ChildrenProperty, binding);
            DRT.Assert(group2.Children == group.Children);
            DRT.Assert(group.ReadLocalValue(DrawingGroup.ChildrenProperty) == DependencyProperty.UnsetValue);

            // Does the binding still work through promotion?
            group.Children.Add(new GeometryDrawing());
            DRT.Assert(group2.Children == group.Children);
            DRT.Assert(group2.Children.Count == group.Children.Count);
            DRT.Assert(group.ReadLocalValue(DrawingGroup.ChildrenProperty) != DependencyProperty.UnsetValue);

            // Verify that after an object with a cached default value is frozen, reading from the
            // cache will return a frozen object.  We explicitly orphan the cache, so also check that
            // the default value of the newly frozen object is not the same as the one we originally
            // handed out.
            DrawingCollection cachedDefault;
            DrawingCollection frozenDefault;
            DrawingGroup group3 = new DrawingGroup();
            cachedDefault = group3.Children;  // read the value so the default is cached
            DRT.Assert(!cachedDefault.IsFrozen);
            group3.Freeze();
            frozenDefault = group3.Children;  // grab a frozen default value
            DRT.Assert(frozenDefault.IsFrozen);
            DRT.Assert(frozenDefault != cachedDefault);

            // We should have frozen the cached default so that users get an exception if they
            // attempt to modify it.
            DRT.Assert(cachedDefault.IsFrozen);

            // Make sure that frozen objects, if they don't already have cached values,
            // return the original frozen default value passed into the FreezableDefaultValueFactory.
            DrawingGroup group4 = new DrawingGroup();
            group4.Freeze();
            mutableCollection = group4.Children;  // read the children - this shouldn't clone and cache the result

            DrawingGroup group5 = new DrawingGroup();
            group5.Freeze();
            DrawingCollection defaultValue = group5.Children;
            DRT.Assert(mutableCollection == defaultValue);
        }

        private void FreezingUnfreezableDPsTest()
        {
            // Cannot freeze freezables with expressions or animations.
            Pen pen;

            //Create an empty expression
            Type type = typeof(System.Windows.DependencyProperty).Assembly.GetType("System.Windows.Expression");
            Object expressionObj = type.InvokeMember(null, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.CreateInstance, null, null, null);

            // Expression on value-type property
            pen = new Pen();
            pen.SetValue(Pen.ThicknessProperty, expressionObj);
            DRT.Assert(!pen.CanFreeze);

            // Nothing exciting here
            pen = new Pen();
            pen.SetValue(Pen.ThicknessProperty, 10.0);
            DRT.Assert(pen.CanFreeze);

            // Expression on freezable-type property
            pen = new Pen();
            pen.SetValue(Pen.BrushProperty, expressionObj);
            DRT.Assert(!pen.CanFreeze);

            // Animation on value-type
            pen = new Pen();
            DoubleAnimation thicknessAnimation = new DoubleAnimation(0, 100, new TimeSpan(0, 0, 0, 0, 2000));
            thicknessAnimation.RepeatBehavior = RepeatBehavior.Forever;
            thicknessAnimation.AutoReverse = true;
            pen.BeginAnimation(Pen.ThicknessProperty, thicknessAnimation);
            DRT.Assert(!pen.CanFreeze);
        }

        private void CodegenedFreezablesTest()
        {
            Model3DGroup group = new Model3DGroup();
            int groupChanges = 0;
            group.Changed += delegate
            {
                ++groupChanges;
            };
            Model3DCollection collection = group.Children;
            int collectionChanges = 0;
            collection.Changed += delegate
            {
                ++collectionChanges;
            };
            // Changing the child.  Should fire both delegates
            collection.Add(new GeometryModel3D());
            DRT.Assert(groupChanges == 1);
            DRT.Assert(collectionChanges == 1);
        }

        private void AttachedAndExpressionTest()
        {
            Brush brush = new LinearGradientBrush();

            DependencyProperty BoolAP
                = DependencyProperty.RegisterAttached(
                    "Bool",
                    typeof(bool),
                    typeof(FreezableTestSuite)
                    );

            DependencyProperty ReferenceAP
                = DependencyProperty.RegisterAttached(
                    "Reference",
                    typeof(object),
                    typeof(FreezableTestSuite)
                    );

            DependencyProperty FreezableAP
                = DependencyProperty.RegisterAttached(
                    "Freezable",
                    typeof(Pen),
                    typeof(FreezableTestSuite)
                    );

            brush.SetValue(BoolAP, true);
            brush.SetValue(ReferenceAP, new object());
            Pen pen = new Pen();
            pen.Brush = new LinearGradientBrush();
            brush.SetValue(FreezableAP, pen);

            //
            // 1. General Copy test
            //

            Brush brushCopy = brush.Clone();
            // Value properties should be the same value after a copy
            DRT.Assert(brushCopy.GetValue(BoolAP) == brush.GetValue(BoolAP));
            // Reference properties should be the same reference after a copy
            DRT.Assert(ReferenceEquals(brushCopy.GetValue(ReferenceAP), brush.GetValue(ReferenceAP)));
            // Freezable properties should be different instances after a copy
            DRT.Assert(!ReferenceEquals(brushCopy.GetValue(FreezableAP), brush.GetValue(FreezableAP)));
            // Check a sub property to verify the deep copy
            DRT.Assert(!ReferenceEquals(
                ((Pen)brushCopy.GetValue(FreezableAP)).Brush,
                ((Pen)brush.GetValue(FreezableAP)).Brush)
                );

            //
            // 2. Expression deep Copy test
            //

            // Bind brushCopy's opacity to brush's
            brush.Opacity = 5.0;
            Binding binding = new Binding("Opacity");
            binding.Source = brush;
            BindingOperations.SetBinding(brushCopy, Brush.OpacityProperty, binding);
            DRT.Assert(brush.Opacity == brushCopy.Opacity);

            Brush brushCopyCopy = brushCopy.Clone();

            BindingExpression brushCopysExpression
                = BindingOperations.GetBindingExpression(brushCopy, Brush.OpacityProperty);
            BindingExpression brushCopyCopysExpression
                = BindingOperations.GetBindingExpression(brushCopyCopy, Brush.OpacityProperty);
            // The expression objects should be different but their evaluated values should
            // be the same
            DRT.Assert(!ReferenceEquals(brushCopysExpression, brushCopyCopysExpression));
            DRT.Assert(brushCopy.Opacity == brushCopyCopy.Opacity);

            //
            // 3. PCH test
            //

            int count = 0;
            brush.Changed += delegate { ++count; };
            ((Pen)brush.GetValue(FreezableAP)).Thickness = 5.0;
            DRT.Assert(count == 1);

            //
            // 4. Freeze test
            //

            brush.Freeze();
            DRT.Assert(((Freezable)brush.GetValue(FreezableAP)).IsFrozen);

            //
            // 5. CloneCurrentValue test
            //

            Brush brushCopyCurrentValue = brushCopy.CloneCurrentValue();
            // Value properties should be the same value after a copy
            DRT.Assert(brushCopyCurrentValue.GetValue(BoolAP) == brushCopy.GetValue(BoolAP));
            // Reference properties should be the same reference after a copy
            DRT.Assert(ReferenceEquals(brushCopyCurrentValue.GetValue(ReferenceAP), brushCopy.GetValue(ReferenceAP)));
            // Freezable properties should be different instances after a copy
            DRT.Assert(!ReferenceEquals(brushCopyCurrentValue.GetValue(FreezableAP), brushCopy.GetValue(FreezableAP)));
            // Check a sub property to verify the deep copy
            DRT.Assert(!ReferenceEquals(
                ((Pen)brushCopyCurrentValue.GetValue(FreezableAP)).Brush,
                ((Pen)brushCopy.GetValue(FreezableAP)).Brush)
                );

            // We should have copied the value of the expression but ditched the expression object
            DRT.Assert(brushCopyCurrentValue.Opacity == brushCopy.Opacity);
            DRT.Assert(
                BindingOperations.GetBindingExpression(brushCopyCurrentValue, Brush.OpacityProperty) == null);
        }


        private void CloneTest()
        {
            ParallelTimeline grandParent = new ParallelTimeline();
            ParallelTimeline parent = new ParallelTimeline();
            DoubleAnimation child = new DoubleAnimation();

            grandParent.Children.Add(parent);
            parent.Children.Add(child);

            parent.Freeze();

            ParallelTimeline frozenClone = (ParallelTimeline)grandParent.GetCurrentValueAsFrozen();

            // The clone must be frozen.
            DRT.Assert(frozenClone.IsFrozen);

            // The clone should not be the same instance since grandParent wasn't frozen
            DRT.Assert(frozenClone != grandParent);

            // GetCurrentValueAsFrozen should not have made a clone of the subtree
            DRT.Assert(frozenClone.Children[0] == grandParent.Children[0]);

            // Get an unfrozen clone to start over
            grandParent = grandParent.Clone();
            parent = (ParallelTimeline)grandParent.Children[0];

            // Now put a binding expression on the parent.  It now can't be frozen, but
            // we can call GetCurrentValueAsFrozen to get a clone that can be, since
            // it copies over the value of the binding, not the binding itself.
            Binding binding = new Binding("Duration");
            BindingOperations.SetBinding(parent, Timeline.DurationProperty, binding);

            DRT.Assert(!grandParent.CanFreeze);

            // GetAsFrozen should throw an assertion
            try
            {
                frozenClone = (ParallelTimeline)grandParent.GetAsFrozen();
            }
            catch (InvalidOperationException)
            {
                // expected exception
            }

            frozenClone = (ParallelTimeline)grandParent.GetCurrentValueAsFrozen();
            DRT.Assert(frozenClone.IsFrozen);
        }

        private class TestCollectionMethods<T> where T: DependencyObject
        {
            public static void DoTest(DrtBase DRT)
            {
                T item;

                // Test constructors
                FreezableCollection<T> brushes = new FreezableCollection<T>();
                DRT.Assert(brushes.Count == 0);

                FreezableCollection<T> brushesWithCapacity = new FreezableCollection<T>(112);
                DRT.Assert(brushesWithCapacity.Count == 0);

                brushesWithCapacity.Add(Brushes.Cyan as T);
                brushesWithCapacity.Add(Brushes.White as T);
                brushesWithCapacity.Add(Brushes.Black as T);

                FreezableCollection<T> brushesFromCopy = new FreezableCollection<T>(brushesWithCapacity);

                DRT.Assert(brushesWithCapacity.Count == 3);
                DRT.Assert(brushesWithCapacity.Count == brushesFromCopy.Count);

                for (int i = 0; i < brushesWithCapacity.Count; i++)
                {
                    DRT.Assert(Object.ReferenceEquals(brushesWithCapacity[i], brushesFromCopy[i]));
                }

                // Test change notifications
                int changesDetected = 0;
                int changesExpected = 0;
                ChangeChecker<T> changeChecker = new ChangeChecker<T>(brushes, DRT);

                brushes.Changed +=
                    delegate
                    {
                        changesDetected++;
                    };

                changeChecker.Expect(NotifyCollectionChangedAction.Add, brushes.Count, Brushes.Orange as T);
                brushes.Add(Brushes.Orange as T);
                changesExpected++;
                DRT.Assert(changesDetected == changesExpected);
                changeChecker.Check();

                changeChecker.Expect(NotifyCollectionChangedAction.Add, brushes.Count, Brushes.Purple as T);
                brushes.Add(Brushes.Purple as T);
                changesExpected++;
                DRT.Assert(changesDetected == changesExpected);
                changeChecker.Check();

                // Test Clear
                changeChecker.Expect(NotifyCollectionChangedAction.Reset, 0, null);
                brushes.Clear();
                changesExpected++;
                DRT.Assert(changesDetected == changesExpected);
                changeChecker.Check();

                DRT.Assert(brushes.Count == 0);

                changeChecker.Expect(NotifyCollectionChangedAction.Add, brushes.Count, Brushes.Blue as T);
                brushes.Add(Brushes.Blue as T);
                changesExpected++;
                changeChecker.Check();

                changeChecker.Expect(NotifyCollectionChangedAction.Add, brushes.Count, Brushes.Green as T);
                brushes.Add(Brushes.Green as T);
                changesExpected++;
                changeChecker.Check();

                // Test sub-changes
                item = new SolidColorBrush(Colors.Silver) as T;
                changeChecker.Expect(NotifyCollectionChangedAction.Add, brushes.Count, item);
                brushes.Add(item);
                changesExpected++;
                changeChecker.Check();

                DRT.Assert(Object.ReferenceEquals(brushes[0], Brushes.Blue));
                DRT.Assert(Object.ReferenceEquals(brushes[1], Brushes.Green));
                DRT.Assert((brushes[2] as SolidColorBrush).Color == Colors.Silver);

                (brushes[2] as SolidColorBrush).Color = Colors.LightGreen;
                changesExpected++;
                DRT.Assert(changesDetected == changesExpected);

                // Test Remove/Insert/etc
                changeChecker.Expect(NotifyCollectionChangedAction.Remove, 1, brushes[1]);
                brushes.RemoveAt(1);
                changesExpected++;
                DRT.Assert((brushes[1] as SolidColorBrush).Color == Colors.LightGreen);
                changeChecker.Check();

                DRT.Assert(brushes.Contains(Brushes.Blue as T));
                DRT.Assert(brushes.IndexOf(Brushes.Blue as T) == 0);

                item = new SolidColorBrush(Colors.Gray) as T;
                changeChecker.Expect(NotifyCollectionChangedAction.Add, 1, item);
                brushes.Insert(1, item);
                changesExpected++;
                DRT.Assert(changesDetected == changesExpected);
                changeChecker.Check();

                DRT.Assert((brushes[1] as SolidColorBrush).Color == Colors.Gray);

                item = Brushes.Blue as T;
                changeChecker.Expect(NotifyCollectionChangedAction.Remove, 0, item);
                brushes.Remove(item);
                changesExpected++;
                DRT.Assert(brushes.Count == 2);
                DRT.Assert(changesDetected == changesExpected);
                changeChecker.Check();

                T[] brushArray = new T[2];

                brushes.CopyTo(brushArray, 0);

                DRT.Assert((brushes[0] as SolidColorBrush).Color == Colors.Gray);
                DRT.Assert((brushes[1] as SolidColorBrush).Color == Colors.LightGreen);

                int arrayIndex = 0;

                // IEnumerable
                foreach (T brush in brushes)
                {
                    DRT.Assert(Object.ReferenceEquals(brush, (brushes[arrayIndex++])));
                }

                // IsReadOnly
                DRT.Assert(!((ICollection<T>)brushes).IsReadOnly);

                FreezableCollection<T> clone = brushes.Clone();

                DRT.Assert((clone[0] as SolidColorBrush).Color == Colors.Gray);
                DRT.Assert((clone[1] as SolidColorBrush).Color == Colors.LightGreen);

                // Test IsFreezable
                DRT.Assert(clone.CanFreeze);

                VisualBrush vb = new VisualBrush();
                vb.Visual = new Button();

                clone.Add(vb as T);
                DRT.Assert(!clone.CanFreeze);

                brushes.Freeze();

                // Test adding to a Frozen collection
                bool threwInvalidOperationException = false;

                try
                {
                    brushes.Add(Brushes.Green as T);
                }
                catch (InvalidOperationException)
                {
                    threwInvalidOperationException = true;
                }

                DRT.Assert(threwInvalidOperationException);

                T brushFromOtherThread = default(T);

                // Test adding to a collection off thread
                Thread thread = new Thread((ThreadStart)
                    delegate
                    {
                        threwInvalidOperationException = false;

                        try
                        {
                            clone.Add(Brushes.Green as T);
                        }
                        catch (InvalidOperationException)
                        {
                            threwInvalidOperationException = true;
                        }
                        DRT.Assert(threwInvalidOperationException);

                        brushFromOtherThread = new SolidColorBrush(Colors.Olive) as T;
                    }
                );

                thread.Start();
                thread.Join();

                DRT.Assert(brushFromOtherThread != null);

                threwInvalidOperationException = false;

                try
                {
                    clone.Add(brushFromOtherThread);
                }
                catch (InvalidOperationException)
                {
                    threwInvalidOperationException = true;
                }
                DRT.Assert(threwInvalidOperationException);
            }
        }

        private void FreezableCollection()
        {
            TestCollectionMethods<Brush>.DoTest(DRT);
            TestCollectionMethods<DependencyObject>.DoTest(DRT);

            Binding b = new Binding();
            b.Path = new PropertyPath("Width");

            // Test basic binding - first, outisde of a collection
            Button button = new Button();
            button.Width = 100;
            button.DataContext = button;

            SolidColorBrush scb = new SolidColorBrush();

            button.Tag = scb;
            DRT.Assert(scb.Opacity == 1);

            BindingOperations.SetBinding(scb, Brush.OpacityProperty, b);

            double opacity = scb.Opacity;
            DRT.Assert(scb.Opacity == 100);

            FreezableCollection<Brush> brushes = new FreezableCollection<Brush>();
            button.Tag = brushes;

            SolidColorBrush scb2 = new SolidColorBrush();
            brushes.Add(scb2);
            DRT.Assert(scb2.Opacity == 1);

            BindingOperations.SetBinding(scb2, Brush.OpacityProperty, b);

            DRT.Assert(scb2.Opacity == 100);
        }

        private bool CheckForException(EventHandler handler)
        {
            // Skip exception testing if we are timing pref.
            if (DRT.RepeatCount > 1) return true;

            Exception thrown = null;

            try
            {
                handler(null, EventArgs.Empty);
            }
            catch(Exception e)
            {
                thrown = e;
            }

            return thrown != null;
        }

        private new DRTFreezable DRT
        {
            get
            {
                return (DRTFreezable) base.DRT;
            }
        }

        private static readonly TestFreezable _testValue = new TestFreezable();

        private class ChangeChecker<T> where T: DependencyObject
        {
            public ChangeChecker(FreezableCollection<T> target, DrtBase drt)
            {
                _target = target;
                _drt = drt;

                INotifyCollectionChanged incc = target as INotifyCollectionChanged;

                if (incc != null)
                {
                    _isEnabled = true;
                    incc.CollectionChanged += new NotifyCollectionChangedEventHandler(OnCollectionChanged);

                    INotifyPropertyChanged inpc = target as INotifyPropertyChanged;
                    if (inpc != null)
                        inpc.PropertyChanged += new PropertyChangedEventHandler(OnPropertyChanged);
                }
            }

            public void Expect(NotifyCollectionChangedAction action, int index, object item)
            {
                _action = action;
                _index = index;
                _item = item;
                _ccArgs = null;
                _countChanged = false;
            }

            public void Check()
            {
                if (!_isEnabled)
                    return;

                _drt.Assert(_ccArgs != null, "Failed to receive CollectionChanged event");
                _drt.AssertEqual(_action, _ccArgs.Action, "Received wrong CollectionChanged event");

                switch (_action)
                {
                    case NotifyCollectionChangedAction.Add:
                        _drt.AssertEqual(_index, _ccArgs.NewStartingIndex, "Add event has wrong index");
                        _drt.AssertEqual(_item, _ccArgs.NewItems[0], "Add event has wrong item");
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        _drt.AssertEqual(_index, _ccArgs.OldStartingIndex, "Remove event has wrong index");
                        _drt.AssertEqual(_item, _ccArgs.OldItems[0], "Remove event has wrong item");
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        break;
                }

                _drt.Assert(_countChanged, "Failed to receive PropertyChanged event for Count property");
            }

            private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                _ccArgs = e;
            }

            private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (String.Equals(e.PropertyName, "Count"))
                {
                    _countChanged = true;
                }
            }

            FreezableCollection<T> _target;
            DrtBase _drt;
            bool _isEnabled;

            NotifyCollectionChangedAction _action;
            int _index;
            object _item;
            NotifyCollectionChangedEventArgs _ccArgs;
            bool _countChanged;
        }
    }
}

