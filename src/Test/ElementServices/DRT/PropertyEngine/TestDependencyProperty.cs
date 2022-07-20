// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
//
// Description: DRT test suite for Inheritance context
//

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;  // ISupportInitialize
using System.IO; // StreamWriter for logging output
using System.Reflection;
using System.Threading;

using System.Windows;
using System.Windows.Controls; // Button as test object
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation; // Storyboard support
using System.Windows.Markup;
using System.Windows.Shapes;


namespace DRT
{
    public sealed class TestDependencyProperty : DrtTestSuite
    {
        public TestDependencyProperty() : base("DependencyProperty")
        {
            Contact = "Microsoft";
        }

        #region Setup

        public override DrtTest[] PrepareTests()
        {
            return new DrtTest[]
            {
                // Note: This test has been disabled because changing the static constructor order causes issues.
                // This test depends on static constructor ordering, so it must run first!
                //new DrtTest(PropertyChangeTests),

                new DrtTest(BaseLocalValueAndExpressionTests),
                new DrtTest(FrameworkInheritanceTests),

                // Styling tests
                new DrtTest(StyleValueAndPropertyTriggerOnSelf),
                new DrtTest(DataBoundStyleValueAndPropertyTriggerOnSelf),
                new DrtTest(StyleResourcesPropertyOnSelf),

                new DrtTest(MiscellaneousBugRegressions),

                // Templating tests
                new DrtTest(TemplateVisualTreeNodesAndAlias),
                new DrtTest(TemplateResourceReference),
                new DrtTest(TemplatedParentDRT),
                new DrtTest(DataBoundTemplateVisualTreeNodesAndAlias),

                // DesignerProperties tests
                new DrtTest(TestDesignerProperties),

                // DesignerCoerceValueCallback tests
                new DrtTest(TestDesignerCoerceValueCallback),

                // DependencyPropertyDescriptor tests
                new DrtTest(TestDependencyPropertyDescriptor),
            };
        }

        #endregion Setup

        public void Check(bool condition, string description)
        {
            CheckCount++;
            DRT.Assert(condition, "Verification #" + CheckCount + " Failed.  Diagnostic message:" + description);
        }

        private int CheckCount = 0;


        //
        // Note: This test has been disabled because changing the static constructor order causes issues.
        // Test that we dont miss OverrideMetadata due to the order of static initilizers running.
        // This test must be the first test run so that VeryGoodMercury.cctor runs before GoodMercury.cctor.
        //
        /*
        void PropertyChangeTests()
        {
            VeryGoodMercury veryGoodMercury = new VeryGoodMercury();
            DRT.Assert(veryGoodMercury._bestValueEver == 0, "Property Changes shouldn't fire before the value has changed.");

            veryGoodMercury.SetValue(VeryGoodMercury.OverrideMeProperty, true);
            DRT.Assert(veryGoodMercury._bestValueEver == (0x1 | 0x2 | 0x4), "Property Changes should fire for all base classes that have a PropertyChangeCallback ({0}).", veryGoodMercury._bestValueEver);
        }
        */

        //
        // Local value/Value Expression tests
        //
        //  This tests the basic DependencyProperty functionality in WindowsBase.dll.
        // and all test objects are defined in this file.  Any breaks in this
        // code is caused by changes in WindowsBase.dll DependencyProperty behavior.
        //
        void BaseLocalValueAndExpressionTests()
        {
            Console.WriteLine("1100 - Create test object");

            Mercury m = new Mercury();

            Console.WriteLine("1200 - Default value");

            // Simple straightforward test for default value
            int a = m.Alpha;
            Check(a == 124,
                "Value for the Alpha property on test object 'Mercury' was not its default value even though there was nothing else to override it.  Did you add code that interfered with the default value?");

            int b = m.Beta;
            Check(b == 200,
                "Value for the Beta property on test object 'Mercury' was not its default value even though there was nothing else to override it.  Did you add code that interfered with the default value?");

            /* Expressions removed from public use

                        Console.WriteLine("1300 - Expression");

                        // Beta gets HalfExpression, so it should be half of Alpha.
                        HalfExpression half = new HalfExpression(m, GreekStandard.AlphaProperty);
                        m.SetValue(GreekStandard.BetaProperty, half);

                        half.ChangeSources(m, GreekStandard.BetaProperty, half.GetSources());

                        Check((int)m.GetValue(GreekStandard.BetaProperty) == 62,
                            "Beta property should be half of the Alpha value but wasn't.  Check for interference to code path within GetValue to evaluate Expression objects (calling Expression.GetValue).");

                        m.Alpha = 888;
                        Check((int)m.GetValue(GreekStandard.BetaProperty) == 444,
                            "Beta property should have returned an updated value in response to Alpha's change, because Beta is an expression depending on Alpha.  See if the Expression object on Beta was altered, or perhaps the invalidation in response to Alpha change didn't make it to the Beta expression.");

                        // Check local overrides with Expressions
                        m.SetValue(GreekStandard.AlphaProperty, new Expression());
            */
            m.Alpha = 888;

            b = m.Beta;
            Check(b == 888,
                "Value for the Beta property was not coerced to be >= Alpha value property");

            m.SetValue(GreekStandard.BetaProperty, 2004);

            Console.WriteLine("1300 - LocalValueEnumerator");

            // Now test the LocalValueEnumerator functionality
            LocalValueEnumerator locals = m.GetLocalValueEnumerator();

            locals.MoveNext();
            Check(locals.Current.Property == GreekStandard.AlphaProperty,
                "Alpha property is expected to be the first property in the enumerator because it was registered first.");
            Check((int)locals.Current.Value == 888,
                "Value of the Alpha property is supposed to be 888.  It was set as the local value on the Alpha property earlier in this DRT and nothing should have overridden or overwritten it.");

            locals.MoveNext();
            Check(locals.Current.Property == GreekStandard.BetaProperty,
                "Beta property is expected to be the second property in the enumerator because it was registered after Alpha.");
            Check(((int)locals.Current.Value) == 2004,
                "Value of the Beta property is supposed to be 2004.  It was set as the local value on the Beta property earlier in this DRT and nothing should have overridden or overwritten it.");

            Check(locals.MoveNext() == false,
                "No other values were set on the test Mercury object in this DRT - the LocalValueEnumerator is supposed to stop now.");

            Console.WriteLine("1350 - LocalValueEnumerator when notified of object change should halt");

            locals.Reset();     // Testing LocalValueEnumerator invalidation (1)
            locals.MoveNext();  // Testing LocalValueEnumerator invalidation (2) -- advance to Alpha property
            locals.MoveNext();  // Testing LocalValueEnumerator invalidation (2) -- advance to Beta property

            m.ClearValue(GreekStandard.BetaProperty);

            b = m.Beta;

            Check(b == (int)GreekStandard.BetaProperty.GetMetadata(typeof(Mercury)).DefaultValue,
                "The Beta property was just cleared, as a result we should have received the default value in return.  Find where the bad value came from and fix the code that gave it.");

            m.ClearValue(GreekStandard.AlphaProperty);

            Check(m.GetValue(GreekStandard.BetaProperty) == GreekStandard.BetaProperty.GetMetadata(typeof(Mercury)).DefaultValue,
                "The Beta property was just cleared, as a result we should have received the default value in return.  Find where the bad value came from and fix the code that gave it.");

            Check(locals.Current.Property == GreekStandard.BetaProperty,
                "This check was already done earlier in this DRT - if we fail here somebody has messed with the LocalValueEnumerator");
            Check(((int)locals.Current.Value) == 2004,
                "This check should match the one earlier in this DRT - the LocalValueEnumerator is a snapshot and does not stay in sync with the real value (which just got cleared.)");

            try
            {
                locals.MoveNext();
            }
            catch (InvalidOperationException)
            {
                Check(false, "LocalValueEnumerator should be valid even after ClearValue. Since we take a snap shot of the underlying store we should be fine");
            }

            Console.WriteLine("1400 - DependencyProperty caching pattern");

            double d = m.Delta;
            Check(d == 3.14,
                "First call to Delta - should have triggered a call to GetValueBase and retrieved the default value of the Property as per the property metadata passed in via OverrideMetadata");

            d = m.Delta;
            Check(d == 3.14,
                "Second call to Delta - this should have retrieved the cached value");

            m.Delta = 11.22;
            d = m.Delta;
            Check(d == 11.22,
                "Third call to Delta - this should have triggered a new call to GetValueBase because an earlier call to set the property should have caused an invalidation to dirty the cache valid flag.");

            Console.WriteLine("1500 - Read-only DependencyProperty");

            try
            {
                m.SetValue(Mercury.MercuryReadOnlyProperty, 25);
                Check(false, "We should not have been allowed to set a read-only property without the key.");
            }
            catch (InvalidOperationException)
            {
                // Expected InvalidOperationException because we're writing a "Read-Only" property
                //  without using the magic token.
            }

            m.TestSettingUsingDependencyPropertyKey(25);

            Console.WriteLine("1510 - Read-only DependencyProperty and OverrideMetadata interaction.");

            try
            {
                // Should trip an exception in static constructor's call to OverrideMetadata since it doesn't have key
                BadMercury badBoy = new BadMercury();

                Check(false, "We should not have been allowed to override the metadata of a read-only property without using the key.");
            }
            catch (TypeInitializationException)
            {
                // Expected.
            }

            // The static constructor of this class, in contrast, passes in the correct authorization key for OverrideMetadata.
            GoodMercury goodBoy = new GoodMercury();
        }

        //
        // Inheritance tests
        //

        void FrameworkInheritanceTests()
        {
            Console.WriteLine("2100 - Create test objects");
            MyFrameworkContentElement ce = new MyFrameworkContentElement();
            MyFrameworkElement fe = new MyFrameworkElement();

            // Basic value inheritance tests
            Check((int)fe.GetValue(GreekStandard.AlphaProperty) == 458,
                "Object 'fe' should have no values on the Alpha property, so it should have returned the default value as registered in the property metadata passed in to OverrideMetadata.");

            Console.WriteLine("2200 - Inheritance tests using a single parent/single child relationship");

            ce.SetValue(GreekStandard.AlphaProperty, 123);
            ce.AppendModelChild(fe);
            Check(fe.Alpha == 123,
                "Object 'fe' is now a child of object 'ce', and should have picked up a value inherited from the parent.");

            fe.Alpha = 999;
            Check(fe.Alpha == 999,
                "Object 'fe' was just given a local value, which should take precedence over inheritance");

            fe.ClearValue(GreekStandard.AlphaProperty);
            ce.RemoveModelChild(fe);
            Check(fe.Alpha == 458,
                "After the local value was cleared from object 'fe', and it was removed as parent of 'ce', we should return to the default value.");

            ce.AppendModelChild(fe);
            Check(fe.Alpha == 123,
                "After object 'ce' was added as parent of 'fe', we should return to the inherited value.");

            ce.ClearValue(GreekStandard.AlphaProperty);
            Check(fe.Alpha == 458,
                "After the local value was cleared from object 'ce', we should return to the default value.");

            Console.WriteLine("2300 - Inheritance tests using a slightly more complex tree");

            // Prepare to test InvalidateTree

            Application app = new Application();  // You need to create the App before creating the Window,
            Window window = new Window();  //    in order to get the Window registered.

            MyFrameworkElement feParent = new MyFrameworkElement();
            MyFrameworkElement feChild1 = new MyFrameworkElement();
            MyFrameworkElement feChild2 = new MyFrameworkElement();

            // Create a parent with two children.  The children are *not*
            // in the same order wrt visual & logical trees (this is necessar
            // to test a code path in FrameworkElement.InvalidateTree).

            feParent.AppendChild(feChild1);
            feParent.AppendChild(feChild2);

            Console.WriteLine("2400 - The tree will also include resource references");

            // Set resource references on the children that won't
            // resolve yet (defaults will be returned).

            feChild1.SetResourceReference(GreekStandard.AlphaProperty, "Child1");
            feChild2.SetResourceReference(GreekStandard.AlphaProperty, "Child2");

            Console.WriteLine("2500 - Build ResourceDictionary");

            // Set a resource dictionary on the grandparent (though
            // the grandparent isn't connected yet).

            window.Resources = new ResourceDictionary();
            window.Resources["Child1"] = 999;
            window.Resources["Child2"] = 1000;

            // Verify that the children resolve to defaults (and
            // cause the caches to be filled).

            Console.WriteLine("2600 - Check resource references before dictionary is available");

            Check(feChild1.Alpha == 458,
                "The Alpha property should be the default value because the resource reference should fail.  Find why the lookup erroneously indicated success. (1 of 2)");
            Check(feChild2.Alpha == 458,
                "The Alpha property should be the default value because the resource reference should fail.  Find why the lookup erroneously indicated success. (2 of 2)");

            // Now connect the parent to the grandparent, which
            // should invalidate the resource references.

            Console.WriteLine("2700 - Add node with dictionary into tree");

            window.Content = feParent; //.AppendModelChild(feParent);

            // Verify that the children resolve to the resource references.

            Console.WriteLine("2800 - Check resource references after dictionary is in place");

            Check(feChild1.Alpha == 999,
                "The Alpha property should have been successfully retrieved from the ResourceDictionary set a few lines earlier.  This is most likely a break in ResourceDictionary or the FrameworkElement parent walk to get there. (1 of 2)");
            Check(feChild2.Alpha == 1000,
                "The Alpha property should have been successfully retrieved from the ResourceDictionary set a few lines earlier.  This is most likely a break in ResourceDictionary or the FrameworkElement parent walk to get there. (2 of 2)");


            // Also verify that unresolved resource
            // references throw an exception from the public API.

            Console.WriteLine("2900 - Check resource references after dictionary is in place");

            bool didThrow = false;
            try
            {
                // This should throw; the resource won't be found.
                window.FindResource("Child3");
            }
            catch (ResourceReferenceKeyNotFoundException e)
            {
                Check (String.Compare( (string)e.Key, "Child3") == 0, "ResourceReferenceKeyNotFoundException had the wrong key" );
                didThrow = true;
            }

            Check (didThrow == true, "Looking up a missing resource using FindResource should have thrown an exception" );

            // TryFindResource should not throw an exception
            Check(window.TryFindResource("Child3") == null, "TryFindResource should return null when not found");

            //---------------------------------------------------------------------------
            //---------------------------------------------------------------------------
            // BaseValue tests
            //---------------------------------------------------------------------------
            //---------------------------------------------------------------------------

            MyFrameworkElement feGrandParent = new MyFrameworkElement();
            feParent = new MyFrameworkElement();
            MyFrameworkElement feChild = new MyFrameworkElement();
            MyFrameworkElement feGrandChild = new MyFrameworkElement();

            feChild.AppendModelChild(feGrandChild);
            feParent.AppendModelChild(feChild);
            feGrandParent.Alpha = 100;

            //---------------------------------------------------------------------------
            // No IsSelfInheritanceParent(ISIP) baseValue inheritance
            //---------------------------------------------------------------------------

            // Add to tree
            feGrandParent.AppendModelChild(feParent);
            Check(feParent.Alpha == 100 && feChild.Alpha == 100 && feGrandChild.Alpha == 100, "Value 100 for inheritable property Alpha should have been propagated to the children on a tree add operation");

            // Remove from tree
            feGrandParent.RemoveModelChild(feParent);
            Check(feParent.Alpha == 458 && feChild.Alpha == 458 && feGrandChild.Alpha == 458, "All the inherited property value should have been rolled back to the default value on a tree remove operation");

            //---------------------------------------------------------------------------
            // ISIP set on root baseValue inheritance
            //---------------------------------------------------------------------------
            feParent.Gamma = 200;

            // Add to tree
            feGrandParent.AppendModelChild(feParent);
            Check(feParent.Alpha == 100 && feChild.Alpha == 100 && feGrandChild.Alpha == 100, "Value 100 for inheritable property Alpha should have been propagated to the children on a tree add operation");
            Check(feParent.Gamma == 200 && feChild.Gamma == 200 && feGrandChild.Gamma == 200, "Inheritable property change to value 200 for inheritable property Gamma should have been propagated to the children");

            // Remove from tree
            feGrandParent.RemoveModelChild(feParent);
            Check(feParent.Alpha == 458 && feChild.Alpha == 458 && feGrandChild.Alpha == 458, "All the inherited property value should have been rolled back to the default value on a tree remove operation");
            Check(feParent.Gamma == 200 && feChild.Gamma == 200 && feGrandChild.Gamma == 200, "Inheritable property change to value 200 for inheritable property Gamma should have been propagated to the children");

            //---------------------------------------------------------------------------
            // IsSelfInheritanceParent(ISIP) set on non-root node baseValue inheritance
            //---------------------------------------------------------------------------
            feParent.RemoveModelChild(feChild);
            feParent = new MyFrameworkElement();
            feParent.AppendModelChild(feChild);
            feChild.Gamma = 300;

            // Add to tree
            feGrandParent.AppendModelChild(feParent);
            Check(feParent.Alpha == 100 && feChild.Alpha == 100 && feGrandChild.Alpha == 100, "Value 100 for inheritable property Alpha should have been propagated to the children on a tree add operation");
            Check(feParent.Gamma == 888 && feChild.Gamma == 300 && feGrandChild.Gamma == 300, "Inheritable property change to value 200 for inheritable property Gamma should have been propagated to the children");

            // Remove from tree
            feGrandParent.RemoveModelChild(feParent);
            Check(feParent.Alpha == 458 && feChild.Alpha == 458 && feGrandChild.Alpha == 458, "All the inherited property value should have been rolled back to the default value on a tree remove operation");
            Check(feParent.Gamma == 888 && feChild.Gamma == 300 && feGrandChild.Gamma == 300, "Inheritable property change to value 200 for inheritable property Gamma should have been propagated to the children");

            //---------------------------------------------------------------------------
            //---------------------------------------------------------------------------
            // Coersion tests
            //---------------------------------------------------------------------------
            //---------------------------------------------------------------------------
            feGrandParent = new MyFrameworkElement();
            feParent = new MyFrameworkElement();
            feChild = new MyFrameworkElement();
            feGrandChild = new MyFrameworkElement();

            feChild.AppendModelChild(feGrandChild);
            feParent.AppendModelChild(feChild);
            feGrandParent.CoersionValue = 400;
            feGrandParent.CoerceValue(GreekStandard.AlphaProperty);
            Check(feGrandParent.Alpha == 400, "DefaultValue must be coerced to value 400 on the grand parent node");

            //---------------------------------------------------------------------------
            // No IsSelfInheritanceParent(ISIP) coerced default inheritance
            //---------------------------------------------------------------------------

            // Add to tree
            feGrandParent.AppendModelChild(feParent);
            Check(feParent.Alpha == 400 && feChild.Alpha == 400 && feGrandChild.Alpha == 400, "Value 400 for inheritable property Alpha should have been propagated to the children on a tree add operation");

            // Remove from tree
            feGrandParent.RemoveModelChild(feParent);
            Check(feParent.Alpha == 458 && feChild.Alpha == 458 && feGrandChild.Alpha == 458, "All the inherited property value should have been rolled back to the default value on a tree remove operation");

            // Change inherited property - coercion interrupts inheritance path
            feGrandParent.AppendModelChild(feParent);
            feChild.ZetaCoercionValue = 500;
            feGrandParent.Zeta = 300;
            Check(feGrandParent.Zeta == 300 && feParent.Zeta == 300, "Value 300 should have inherited into tree");
            Check(feChild.Zeta == 500 && feGrandChild.Zeta == 500, "Coerced value 500 should have supplanted 300 during inheritance");

            // (previous test sets ISIP on feChild.  Reassemble a fresh tree)
            feGrandParent.RemoveModelChild(feParent);
            feParent.RemoveModelChild(feChild);
            feChild = new MyFrameworkElement();
            feGrandChild = new MyFrameworkElement();
            feChild.AppendModelChild(feGrandChild);
            feParent.AppendModelChild(feChild);

            // Add to tree, changing inherited property - coercion interrupts inheritance path
            feGrandParent.Zeta = 300;
            feGrandParent.Gamma = 200;
            feChild.ZetaCoercionValue = 500;
            feGrandParent.AppendModelChild(feParent);
            Check(feGrandParent.Zeta == 300 && feParent.Zeta == 300, "Value 300 should have inherited into tree");
            Check(feChild.Zeta == 500 && feGrandChild.Zeta == 500, "Coerced value 500 should have supplanted 300 during inheritance");
            Check(feParent.Gamma == 200 && feChild.Gamma == 200 && feGrandChild.Gamma == 200, "Inheritable property change to value 200 for inheritable property Gamma should have been propagated to the children");

            // Remove from tree
            feChild.ZetaCoercionValue = 0;
            feGrandParent.RemoveModelChild(feParent);
            Check(feParent.Zeta == 267 && feChild.Zeta == 267 && feGrandChild.Zeta == 267, "All the inherited property value should have been rolled back to the default value on a tree remove operation");

            // (previous test sets ISIP on feChild.  Reassemble a fresh tree)
            feGrandParent.RemoveModelChild(feParent);
            feParent.RemoveModelChild(feChild);
            feChild = new MyFrameworkElement();
            feGrandChild = new MyFrameworkElement();
            feChild.AppendModelChild(feGrandChild);
            feParent.AppendModelChild(feChild);

            //---------------------------------------------------------------------------
            // IsSelfInheritanceParent(ISIP) set on any node coerced default inheritance
            //---------------------------------------------------------------------------
            feParent.Gamma = 500;
            feChild.CoersionValue = 600;
            feChild.CoerceValue(GreekStandard.AlphaProperty);

            // Add to tree
            feGrandParent.AppendModelChild(feParent);
            Check(feParent.Alpha == 400 && feChild.Alpha == 400 && feGrandChild.Alpha == 400, "Value 400 for inheritable property Alpha should have been propagated to the children on a tree add operation");
            Check(feParent.Gamma == 500 && feChild.Gamma == 500 && feGrandChild.Gamma == 500, "Inheritable property change to value 500 for inheritable property Gamma should have been propagated to the children");

            // Coercion interrupts inheritance path
            feParent.ZetaCoercionValue = 100;
            feGrandParent.Zeta = 700;
            Check(feGrandParent.Zeta == 700, "Value 700 should have inherited into tree");
            Check(feParent.Zeta == 100 && feChild.Zeta == 100 && feGrandChild.Zeta == 100, "Coerced value 100 should have supplanted 700 during inheritance");

            // Remove from tree
            feGrandParent.RemoveModelChild(feParent);
            Check(feParent.Alpha == 458 && feChild.Alpha == 600 && feGrandChild.Alpha == 600, "All the inherited property value should have been rolled back to the default/coerced value on a tree remove operation");
            Check(feParent.Gamma == 500 && feChild.Gamma == 500 && feGrandChild.Gamma == 500, "Inheritable property change to value 200 for inheritable property Gamma should have been propagated to the children");

            //---------------------------------------------------------------------------
            //---------------------------------------------------------------------------
            // Inheritable property chaining
            //---------------------------------------------------------------------------
            //---------------------------------------------------------------------------
            feGrandParent = new MyFrameworkElement();
            feParent = new MyFrameworkElement();
            feChild = new MyFrameworkElement();
            feGrandChild = new MyFrameworkElement();

            feChild.AppendModelChild(feGrandChild);
            feParent.AppendModelChild(feChild);
            feGrandParent.Alpha = 700;

            //---------------------------------------------------------------------------
            // Chaining within the same node
            //---------------------------------------------------------------------------
            feParent.AlphaDependentValue = 800;

            // Add to tree
            feGrandParent.AppendModelChild(feParent);
            Check(feParent.Alpha == 700 && feChild.Alpha == 700 && feGrandChild.Alpha == 700, "Value 700 for inheritable property Alpha should have been propagated to the children on a tree add operation");
            Check(feParent.Gamma == 800 && feChild.Gamma == 800 && feGrandChild.Gamma == 800, "Value 800 for inheritable property Gamma should have been propagated to the children independent of the tree add operation");

            // Remove from tree
            feGrandParent.RemoveModelChild(feParent);
            Check(feParent.Alpha == 458 && feChild.Alpha == 458 && feGrandChild.Alpha == 458, "All the inherited property value should have been rolled back to the default value on a tree remove operation");
            Check(feParent.Gamma == 800 && feChild.Gamma == 800 && feGrandChild.Gamma == 800, "Value 800 for inheritable property Gamma should have been propagated to the children independent of the tree add operation");

            //---------------------------------------------------------------------------
            // Chaining into unvisited child node
            //---------------------------------------------------------------------------
            feParent.AlphaDependentValue = 900;
            feChild.GammaDependentValue = 950;
            feGrandChild.Sigma = 1000;

            // Add to tree
            // Note the existence of a implementation issue here. feChild misses an Alpha PropertyChanged notification and
            // as a result the change does not propagate to feGrandChild and that one has a stale
            // value. We need to fix this problem. This test is simply capturing the error functionality.
            feGrandParent.AppendModelChild(feParent);
            Check(feParent.Alpha == 700 && feChild.Alpha == 700 && feGrandChild.Alpha == 458, "Value 700 for inheritable property Alpha should have been propagated to the children on a tree add operation");
            Check(feParent.Gamma == 900 && feChild.Gamma == 900 && feGrandChild.Gamma == 900, "Value 900 for inheritable property Gamma should have been propagated to the children independent of the tree add operation");
            Check(feParent.Sigma == 918 && feChild.Sigma == 950 && feGrandChild.Sigma == 1000, "Value 950 for inheritable property Sigma should have been propagated to the children independent of the tree add operation");

            // Remove from tree
            feGrandParent.RemoveModelChild(feParent);
            Check(feParent.Alpha == 458 && feChild.Alpha == 458 && feGrandChild.Alpha == 458, "All the inherited property value should have been rolled back to the default value on a tree remove operation");
            Check(feParent.Gamma == 900 && feChild.Gamma == 900 && feGrandChild.Gamma == 900, "Value 900 for inheritable property Gamma should have been propagated to the children independent of the tree add operation");
            Check(feParent.Sigma == 918 && feChild.Sigma == 950 && feGrandChild.Sigma == 1000, "Value 950 for inheritable property Sigma should have been propagated to the children independent of the tree add operation");

            // InheritanceBehavior tests

            feParent = new MyFrameworkElement(InheritanceBehavior.SkipToAppNow);
            feChild = new MyFrameworkElement();

            feParent.Alpha = 100;
            feParent.Yabba = 200;

            feParent.AppendModelChild(feChild);

            Check(feChild.Alpha == 458, "Child should not inherit Alpha value");
            Check(feChild.Yabba == 200, "Child should inherit Yabba value because it overrides inheritance behavior");

            // Try the tree change first

            feParent = new MyFrameworkElement(InheritanceBehavior.SkipToAppNow);
            feChild = new MyFrameworkElement();

            feParent.AppendModelChild(feChild);

            feParent.Alpha = 100;
            feParent.Yabba = 200;

            Check(feChild.Alpha == 458, "Child should not inherit Alpha value");
            Check(feChild.Yabba == 200, "Child should inherit Yabba value because it overrides inheritance behavior");

            // Try with the IsSelfInheritanceParent flag set

            feParent = new MyFrameworkElement();
            feChild = new MyFrameworkElement(InheritanceBehavior.SkipToAppNext);

            feParent.Alpha = 100;
            feParent.Yabba = 200;
            feChild.Gamma = 10;

            feParent.AppendModelChild(feChild);

            Check(feChild.Alpha == 458, "Child should not inherit Alpha value");
            Check(feChild.Yabba == 200, "Child should inherit Yabba value because it overrides inheritance behavior");

            // Try the tree change first

            feParent = new MyFrameworkElement();
            feChild = new MyFrameworkElement(InheritanceBehavior.SkipToAppNext);

            feParent.AppendModelChild(feChild);

            feParent.Alpha = 100;
            feParent.Yabba = 200;
            feChild.Gamma = 10;

            Check(feChild.Alpha == 458, "Child should not inherit Alpha value");
            Check(feChild.Yabba == 200, "Child should inherit Yabba value because it overrides inheritance behavior");

        }


        //
        // Styling tests
        //

        // Check Simple and PropertyTriggers on "self"
        void StyleValueAndPropertyTriggerOnSelf()
        {
            Console.WriteLine("3100 - Value and Trigger on the styled element itself");
            Console.WriteLine("3110 - Create test object and test Style");

            MyFrameworkElement fe = new MyFrameworkElement();

            Style s = new Style(typeof(MyFrameworkElement));

            // This specifies: Alpha = 333 for element with this Style.
            s.Setters.Add(new Setter(GreekStandard.AlphaProperty, 333));

            // Trigger specifies: if( Beta == 999 ) { Delta = 444.555 }
            Trigger trigger = new Trigger();
            trigger.Property = GreekStandard.BetaProperty;
            trigger.Value = 999;
            Setter setter = new Setter();
            setter.Property = GreekStandard.DeltaProperty;
            setter.Value = 444.555;
            trigger.Setters.Add(setter);
            s.Triggers.Add(trigger);

            Console.WriteLine("3120 - Apply test Style to test object");

            // Apply Style
            fe.Style = s;

            Console.WriteLine("3130 - Verify test object state properly reflects Style");

            Check(fe.Alpha == 333,
                "Alpha property value should have been picked up from the just-applied Style.  Check why this Style value lookup failed in FrameworkElement.GetValueCore.");

            // Verify that the Trigger target property is the default.
            Check(fe.Delta == 0,
                "GetValue should have returned the default value as last resort because nothing else is available.  Find out what is erroneously causing a value to get picked up, overriding the defualt.");

            Console.WriteLine("3140 - Meet Trigger condition and verify it is active");

            // Make the Trigger active
            fe.Beta = 999;
            Check(fe.Delta == 444.555,
                "Trigger should be active at this point.  Step into FrameworkElement.GetValueCore to see why it's not getting picked up from the Style.");

            Console.WriteLine("3150 - Un-do meeting Trigger condition and verify it is inactive");

            // Deactivate the Trigger
            fe.Beta = 997;
            Check(fe.Delta == 0,
                "Trigger has become inactive again, and hence we should have fallen through to the default.  Find out what's erroneously returning a value that overrode the default.");
        }

        // Check data-bound Simple and PropertyTriggers on "self"
        void DataBoundStyleValueAndPropertyTriggerOnSelf()
        {
            Console.WriteLine("3600 - Data-bound Value and Trigger on the styled element itself");
            Binding binding;
            Console.WriteLine("3610 - Create test object and test Style");

            MyFrameworkElement fe = new MyFrameworkElement();

            MyDataItem item = new MyDataItem(333, 999, 444.555);

            Style s = new Style(typeof(MyFrameworkElement));

            // This specifies: Alpha = 333 for element with this Style.
            binding = new Binding("Alpha");
            binding.Source = item;
            s.Setters.Add(new Setter(GreekStandard.AlphaProperty, binding));

            // Trigger specifies: if( Beta == 999 ) { Delta = 444.555 }
            Trigger trigger = new Trigger();
            trigger.Property = GreekStandard.BetaProperty;
            trigger.Value = 999;
            binding = new Binding("Delta");
            binding.Source = item;
            Setter setter = new Setter();
            setter.Property = GreekStandard.DeltaProperty;
            setter.Value = binding;
            trigger.Setters.Add(setter);
            s.Triggers.Add(trigger);

            Console.WriteLine("3620 - Apply test Style to test object");

            // Apply Style
            fe.Style = s;

            Console.WriteLine("3630 - Verify test object state properly reflects Style");

            Check(fe.Alpha == 333,
                "Alpha property value should have been picked up from the just-applied Style.  Check why this Style value lookup failed in FrameworkElement.GetValueCore.");

            // Verify that the Trigger target property is the default.
            Check(fe.Delta == 0,
                "GetValue should have returned the default value as last resort because nothing else is available.  Find out what is erroneously causing a value to get picked up, overriding the defualt.");

            Console.WriteLine("3640 - Meet Trigger condition and verify it is active");

            // Make the Trigger active
            fe.Beta = 999;
            Check(fe.Delta == 444.555,
                "Trigger should be active at this point.  Step into FrameworkElement.GetValueCore to see why it's not getting picked up from the Style.");

            Console.WriteLine("3650 - Un-do meeting Trigger condition and verify it is inactive");

            // Deactivate the Trigger
            fe.Beta = 997;
            Check(fe.Delta == 0,
                "Trigger has become inactive again, and hence we should have fallen through to the default.  Find out what's erroneously returning a value that overrode the default.");
        }

        // Check the working of the Style.ResourcesProperty
        void StyleResourcesPropertyOnSelf()
        {
            Console.WriteLine("3800 - Style.Resources property tests");
            Console.WriteLine("3810 - Create test object and test Style");

            MyFrameworkElement fe = new MyFrameworkElement();

            Style s = new Style(typeof(MyFrameworkElement));
            Style bs = new Style(typeof(MyFrameworkElement));
            s.BasedOn = bs;

            // Populate the style Resource Dictionary
            bs.Resources.Add("MyInteger", 333);
            s.Resources["MyDouble"] = 444.555;

            // This specifies: Alpha = "{DynamicResource MyInteger}" for element with this Style.
            Setter setter = new Setter(GreekStandard.AlphaProperty, new DynamicResourceExtension("MyInteger"));
            s.Setters.Add(setter);

            // Trigger specifies: if( Beta == 999 ) { Delta = "{DynamicResource MyDouble}" }
            Trigger trigger = new Trigger();
            trigger.Property = GreekStandard.BetaProperty;
            trigger.Value = 999;
            trigger.Setters.Add(new Setter(GreekStandard.DeltaProperty, new DynamicResourceExtension("MyDouble")));
            s.Triggers.Add(trigger);

            Console.WriteLine("3820 - Apply test Style to test object");

            // Apply Style
            fe.Style = s;

            Console.WriteLine("3830 - Verify test object state properly reflects Style");

            Check(fe.Alpha == 333,
                "Alpha property value should have been picked up from the just-applied Style's ResourceDictionary.  Check why this Style value lookup failed in FrameworkElement.GetValueCore.");

            // Verify that the Trigger target property is the default.
            Check(fe.Delta == 0,
                "GetValue should have returned the default value as last resort because nothing else is available.  Find out what is erroneously causing a value to get picked up, overriding the defualt.");

            Console.WriteLine("3840 - Meet Trigger condition and verify it is active");

            // Make the Trigger active
            fe.Beta = 999;
            Check(fe.Delta == 444.555,
                "Trigger should be active at this point and should pickup the value from the Style's ResourceDictionary.  Step into FrameworkElement.GetValueCore to see why it's not getting picked up from the Style.");

            Console.WriteLine("3850 - Un-do meeting Trigger condition and verify it is inactive");

            // Deactivate the Trigger
            fe.Beta = 997;
            Check(fe.Delta == 0,
                "Trigger has become inactive again, and hence we should have fallen through to the default.  Find out what's erroneously returning a value that overrode the default.");

            // Set a local ResourceReference and see if it picks up the
            // resource from the Style's ResourceDictionary
            MyFrameworkElement childFE = new MyFrameworkElement();
            fe.AppendModelChild(childFE);

            childFE.SetResourceReference(GreekStandard.AlphaProperty, "MyInteger");

            Console.WriteLine("3860 - Verify test child object state properly reflects Style.Resources");

            Check(childFE.Alpha == 333,
                "Alpha property value should have been picked up from the parent Style's ResourceDictionary.");

            // Test that changing the style does reflect the changes expected
            s = new Style(typeof(MyFrameworkElement));

            // Populate the style Resource Dictionary
            s.Resources.Add("MyInteger", 666);
            s.Resources["MyDouble"] = 777.888;

            // This specifies: Alpha = "{DynamicResource MyInteger}" for element with this Style.
            setter = new Setter(GreekStandard.AlphaProperty, new DynamicResourceExtension("MyInteger"));
            s.Setters.Add(setter);

            // Apply Style
            fe.Style = s;

            Console.WriteLine("3870 - Verify that changing a style with Resource section does all the right invalidations to reflect the change.");

            Check(fe.Alpha == 666,
                "Alpha property value should have been picked up from the just-applied Style's ResourceDictionary.  Check why this Style value lookup failed in FrameworkElement.GetValueCore.");

            Check(fe.Delta == 0,
                "Trigger has become inactive again, and hence we should have fallen through to the default.  Find out what's erroneously returning a value that overrode the default.");

            Check(childFE.Alpha == 666,
                "Alpha property value should have been picked up from the parent Style's ResourceDictionary.");

            // Last of all try to change the ResourceDictionary on a Style after it has been sealed.

            Console.WriteLine("3880 - Verify that changing the Style.Resources after the style has been sealed is disallowed.");

            try
            {
                s.Resources.Add("MyString", "Hi There");
                Check(false, "Changing Style.Resources after the style has been sealed must be disallowed");
            }
            catch (InvalidOperationException)
            {
            }

            // Test that style triggers have higher priority that template triggers
            Console.WriteLine("3890 - Create test Style and Template");

            fe = new MyFrameworkElement();

            s = new Style(typeof(MyFrameworkElement));

            ControlTemplate t = new ControlTemplate(typeof(MyFrameworkElement));

            // Trigger specifies: if( Beta == 999 ) { Delta = 444.555 }
            trigger = new Trigger();
            trigger.Property = GreekStandard.BetaProperty;
            trigger.Value = 999;
            trigger.Setters.Add(new Setter(GreekStandard.DeltaProperty, 555.666));
            t.Triggers.Add(trigger);

            // This specifies: Template = t for element with this Style.
            setter = new Setter(Control.TemplateProperty, t);
            s.Setters.Add(setter);

            // This specifies: Delta = 333 for element with this Style.
            setter = new Setter(GreekStandard.DeltaProperty, 333.444);
            s.Setters.Add(setter);

            // Trigger specifies: if( Beta == 999 ) { Delta = 444.555 }
            trigger = new Trigger();
            trigger.Property = GreekStandard.BetaProperty;
            trigger.Value = 999;
            trigger.Setters.Add(new Setter(GreekStandard.DeltaProperty, 444.555));
            s.Triggers.Add(trigger);

            // Apply Style
            fe.Style = s;

            Console.WriteLine("3891 - Verify test object state properly reflects Style");

            // Verify that the style setters are applied.
            Check(fe.Template == t,
                "GetValue should have returned the style setter value.");

            Check(fe.Delta == 333.444,
                "GetValue should have returned the style setter value because the trigger condition isn't satisfied.");

            Console.WriteLine("3892 - Meet Trigger condition and verify it is active");

            // Make the Trigger active
            fe.Beta = 999;
            Check(fe.Delta == 444.555,
                "Trigger should be active at this point. Style trigger should have precedence over template triggers.");

            Console.WriteLine("3893 - Un-do meeting Trigger condition and verify it is inactive");

            // Deactivate the Trigger
            fe.Beta = 997;
            Check(fe.Delta == 333.444,
                "Trigger has become inactive again, and hence we should have fallen through to the style setter value.");

        }


        /// <summary>
        ///     Test regressions for various bugs.  Sometimes it's easy to stick it
        /// elsewhere in the DRT, but if not, have a little repro here.
        /// </summary>
        void MiscellaneousBugRegressions()
        {
            Console.WriteLine("4100 - 957809: Template.set_VisualTree should not crash when given null value");
            ControlTemplate buttonTemplate = new ControlTemplate(typeof(System.Windows.Controls.Button));
            buttonTemplate.VisualTree = null;

            Console.WriteLine("4200 - 953132: Property registration should not allow empty string as name");
            try
            {
                DependencyProperty nullNameProperty = DependencyProperty.Register(
                    String.Empty, typeof(int), typeof(TestDependencyProperty));

                Check(false, "Property registration shouldn't take empty string as name");
            }
            catch (ArgumentException)
            {
                // Expected
            }

            Console.WriteLine("4300 - 915911: RegisterAttached doesn't use the given ValidateCallback to check the given default value");
            try
            {
                DependencyProperty badValueProperty = DependencyProperty.RegisterAttached(
                    "BadValue", typeof(int), typeof(TestDependencyProperty),
                    new PropertyMetadata(3),
                    new ValidateValueCallback(IsEvenNumber));

                Check(false, "Property registration shouldn't allow default value that fails to pass the given ValidateValueCallback");
            }
            catch (ArgumentException)
            {
                // Expected
            }
        }

        public static bool IsEvenNumber(object value)
        {
            int number = (int)value;
            return (number % 2) == 0;
        }

        //
        // Templating tests
        //

        // Check PropertyTriggers and Optimized data binding on template child
        void TemplateVisualTreeNodesAndAlias()
        {
            Console.WriteLine("5100 - FrameworkTemplate.VisualTree nodes and property Alias-ing");
            Console.WriteLine("5110 - Create test object and test Template");

            MyFrameworkElement fe = new MyFrameworkElement();

            ControlTemplate t = new ControlTemplate(typeof(MyFrameworkElement));

            // Pseudocode of MultiTrigger:
            //      if( Alpha == 777 )
            //      {
            //          "Chrome".Beta = 20202;
            //          "Content".Delta = 20.202;
            //      }
            // (MultiTrigger can key off of multiple conditions, but we only have one here.)
            MultiTrigger p = new MultiTrigger();
            p.Conditions.Add(new Condition(GreekStandard.AlphaProperty, 777));
            p.Setters.Add(new Setter(GreekStandard.BetaProperty, 20202, "Chrome"));
            p.Setters.Add(new Setter(GreekStandard.DeltaProperty, 20.202, "Content"));
            t.Triggers.Add(p);

            // Pseudocode of MultiTrigger:
            //      if( Beta == 30303 )
            //      {
            //          "Content".Delta = 30.303;
            //      }
            // (MultiTrigger can key off of multiple conditions, but we only have one here.)
            p = new MultiTrigger();
            p.Conditions.Add(new Condition(GreekStandard.BetaProperty, 30303, "Chrome"));
            p.Setters.Add(new Setter(GreekStandard.DeltaProperty, 30.303, "Content"));
            t.Triggers.Add(p);

            // FrameworkTemplate.VisualTree specifies:
            //   [Root object "Chrome"]
            //          |
            //          +-----------------------+
            //          |                       |
            //   [Child object "Content"]   [Child object "ContentElement"]

            FrameworkElementFactory root = new FrameworkElementFactory(typeof(MyFrameworkElement), "Chrome");
            root.SetValue(GreekStandard.BetaProperty, 11011); // Check #1 below

            FrameworkElementFactory child = new FrameworkElementFactory(typeof(MyFrameworkElement), "Content");
            child.SetValue(GreekStandard.DeltaProperty, new TemplateBindingExtension(GreekStandard.DeltaProperty)); // Check #2 below

            root.AppendChild(child);

            // Test having a FrameworkContentElement in a Style VisualTree.
            FrameworkElementFactory childFCE = new FrameworkElementFactory(typeof(MyFrameworkContentElement), "ContentElement");
            childFCE.SetValue(MyFrameworkContentElement.ZuluProperty, new TemplateBindingExtension(GreekStandard.DeltaProperty));

            root.AppendChild(childFCE);

            Console.WriteLine("5120 - Apply test Template to test object");

            t.VisualTree = root;

            fe.Template = t;

            // Fault in VisualTree
            fe.ApplyTemplate();

            Console.WriteLine("5130 - Get FrameworkTemplate.VisualTree child nodes");

            // Grab the tree elements - this needs to be in sync with the
            // FrameworkElementFactory structure above or we won't get the
            // objects we expected to get.
            MyFrameworkElement chrome = (MyFrameworkElement)VisualTreeHelper.GetChild(fe, 0);
            MyFrameworkElement content = (MyFrameworkElement)chrome.Children[0];
            MyFrameworkContentElement contentElement = (MyFrameworkContentElement)chrome.Children[1];

            Console.WriteLine("5140 - Verify state of child objects is as expected");

            // Check #1
            Check(chrome.Beta == 11011,
                "Property value specified on the FrameworkElementFactory object didn't get picked up. Step through FrameworkElement.GetValueCore and see why Template couldn't find the value.");

            fe.Delta = 99.99; // Trigger the alias for this check #2
            Check(content.Delta == 99.99,
                "Child object had Alias to TemplatedParent node, whose value was just set. Child object should have picked up that value through the Alias mechanism. (1 of 2 : FrameworkElement)");

            // Another check for Alias - this time on the FrameworkContentElement
            Check(((double)contentElement.GetValue(MyFrameworkContentElement.ZuluProperty)) == 99.99,
                "Child object had Alias to TemplatedParent node, whose value was just set. Child object should have picked up that value through the Alias mechanism. (2 of 2 : FrameworkContentElement)");

            Console.WriteLine("5150 - Activate MultiTrigger and verify it's active.");

            // Set the conditions to activate the MultiTrigger created above
            fe.Alpha = 777;
            Check(chrome.Beta == 20202,
                "Child object should have picked up value from an active MultiTrigger. Did the trigger not go active?  Or did the child pick up a value from elsewhere? (1 of 2)");
            Check(content.Delta == 20.202,
                "Child object should have picked up value from an active MultiTrigger. Did the trigger not go active?  Or did the child pick up a value from elsewhere? (2 of 2)");

            Console.WriteLine("5160 - Activate MultiTrigger that sources off of a non-container node and verify it's active.");

            // Set the conditions to activate the MultiTrigger created above
            chrome.Beta = 30303;
            Check(content.Delta == 30.303,
                "Child object should have picked up value from an active MultiTrigger. Did the trigger not go active?  Or did the child pick up a value from elsewhere? (2 of 2)");

            Console.WriteLine("5170 - Remove test Template from test object.");

            // Simple smoke test of Template invalidation code path.
            fe.Template = null;
        }

        void TemplateResourceReference()
        {
            Console.WriteLine("5200 - Dynamic resource references in Templates");
            Console.WriteLine("5210 - Create test element");

            MyFrameworkElement fe = new MyFrameworkElement();

            Console.WriteLine("5220 - Create test Template that uses resource references");

            ControlTemplate t = new ControlTemplate(typeof(MyFrameworkElement));

            FrameworkElementFactory root = new FrameworkElementFactory(typeof(MyFrameworkElement), "Chrome");
            root.SetResourceReference(GreekStandard.AlphaProperty, "bar");

            FrameworkElementFactory child = new FrameworkElementFactory(typeof(MyFrameworkElement), "Content");

            root.AppendChild(child);

            t.VisualTree = root;

            Console.WriteLine("5230 - Apply the test Template");

            fe.Template = t;

            // Fault in VisualTree
            fe.ApplyTemplate();

            Console.WriteLine("5240 - Get references to VisualTree child nodes");

            // This needs to match the FrameworkElementFactory stuff
            // above or we'll get wrong answers.
            MyFrameworkElement chrome = (MyFrameworkElement)VisualTreeHelper.GetChild(fe, 0);
            MyFrameworkElement content = (MyFrameworkElement)chrome.Children[0];

            Console.WriteLine("5250 - Verify that resource lookup fails (since there's no ResourceDictionary)");

            // Stage 1: We should have default values.
            Check(chrome.Alpha == 458,
                "Default value expected because there should be nothing else.  Find out why we didn't fall through to the default value case. (2 of 2)");

            Console.WriteLine("5260 - Create a ResourceDictionary and put it on the test element");

            // Build and attach the ResourceDictionary
            ResourceDictionary resources = new ResourceDictionary();
            resources["bar"] = 99999;

            fe.Resources = resources;

            Console.WriteLine("5270 - Verify that resource lookup is successful");

            // Stage 2: We should be able to pick up the resource dictionary values.
            Check(chrome.Alpha == 99999,
                "Resource lookup failed - see if we actually went in to evaluate the ResourceReferenceExpression and if we did, why it couldn't find the dictionary.  (2 of 2)");

            Console.WriteLine("5280 - Remove the ResourceDictionary");

            // Remove the ResourceDictionary
            fe.Resources = null;

            Console.WriteLine("5290 - Verify that resource lookup fails (since there's no ResourceDictionary, again.)");

            // Stage 3: Back at the default values.
            Check(chrome.Alpha == 458,
                "Default value expected because we just removed the ResourceDictionary and there should be nothing else.  Find out why we didn't fall through to the default value case. (2 of 2)");
        }

        // Create a small template to test TemplatedParent-related functionality.
        void TemplatedParentDRT()
        {
            Console.WriteLine("5300 - FrameworkElement.TemplatedParent");
            Console.WriteLine("5310 - Create test object and test Template");

            Button testButton = new Button();
            ControlTemplate contrivedButtonTemplate = ContrivedButtonTemplate();

            Console.WriteLine("5320 - Apply test Template and call ApplyTemplate");

            // Set the contrived button template, and activate it.
            testButton.Style = null;
            testButton.Template = contrivedButtonTemplate;
            testButton.ApplyTemplate();

            Console.WriteLine("5330 - Get references to VisualTree child objects");

            // This section needs to be in sync with the structure in ContrivedButtonTemplate().
            FrameworkElement decorator = (FrameworkElement)VisualTreeHelper.GetChild(testButton, 0);
            FrameworkElement dockPanel = (FrameworkElement)VisualTreeHelper.GetChild(decorator, 0);
            FrameworkElement canvas = (FrameworkElement)VisualTreeHelper.GetChild(dockPanel, 0);

            Console.WriteLine("5340 - Verify that TemplatedParent property is correctly set");

            // OK, everybody should be pointing to testButton as TemplatedButton
            Check(decorator.TemplatedParent == testButton,
                "TemplateTreeRoot of testButton should have TemplatedParent pointing to testButton");
            Check(dockPanel.TemplatedParent == testButton,
                "Indexed child of testButton should have TemplatedParent pointing to testButton");
            Check(canvas.TemplatedParent == testButton,
                "Non-indexed child of testButton should have TemplatedParent pointing to testButton");

            // Consider - checking internals.
            //  decorator.IsTemplatedTreeRoot should be true
            //  Other two's IsTemplatedTreeRoot should be false
            //  decorator._childIndex should be 1
            //  dockPanel._childIndex should be 2
            //  canvas._childIndex should be -1

            Console.WriteLine("5350 - Replace test Template with standard theme's Button Style");

            // Turn off contrived template and that should kick in the theme Style.VisualTree for the button
            testButton.ClearValue(Control.TemplateProperty);
            testButton.ClearValue(FrameworkElement.StyleProperty);
            testButton.ApplyTemplate();

            Console.WriteLine("5360 - Old Template's VisualTree child objects' TemplatedParent property should be cleaned up");

            // Verify that TemplatedParent references are all cleaned up
            Check(decorator.TemplatedParent == null,
                "TemplateTreeRoot of testButton should have TemplatedParent of null after a new style is applied.");
            Check(dockPanel.TemplatedParent == null,
                "Indexed child of testButton should have TemplatedParent of null after a new style is applied.");
            Check(canvas.TemplatedParent == null,
                "Non-indexed child of testButton should have TemplatedParent of null after a new style is applied.");
        }

        // Used for TemplatedParentDRT.
        private static ControlTemplate ContrivedButtonTemplate()
        {
            ControlTemplate t = new ControlTemplate(typeof(Button));

            // This guys should be the "_childFirst", also with IsTemplatedTreeRoot == true.
            FrameworkElementFactory mainTransform = new FrameworkElementFactory(typeof(Decorator));
            t.VisualTree = mainTransform;

            // Main DockPanel - this is the _childNext in the chain.
            FrameworkElementFactory mainDockPanel = new FrameworkElementFactory(typeof(DockPanel));
            mainTransform.AppendChild(mainDockPanel);

            // Button body Canvas - this guy should be the childNoChildIndex chain.
            FrameworkElementFactory buttonBodyCanvas = new FrameworkElementFactory(typeof(Canvas));
            mainDockPanel.AppendChild(buttonBodyCanvas);

            // Is there a FrameworkContentElement I can add here?

            return t;
        }

        // Check data-bound PropertyTriggers and Optimized data binding on child
        void DataBoundTemplateVisualTreeNodesAndAlias()
        {
            Console.WriteLine("5400 - Data-bound Style.VisualTree nodes and property Alias-ing");
            Binding binding;
            Console.WriteLine("5410 - Create test object and test Template");

            MyFrameworkElement fe = new MyFrameworkElement();

            MyDataItem item1 = new MyDataItem(777, 20202, 20.202);
            MyDataItem item2 = new MyDataItem(888, 11011, 30.303);

            ControlTemplate t = new ControlTemplate(typeof(MyFrameworkElement));

            // Pseudocode of MultiTrigger:
            //      if( Alpha == 777 )
            //      {
            //          "Chrome".Beta = 20202;
            //          "Content".Delta = 20.202;
            //      }
            // (MultiTrigger can key off of multiple conditions, but we only have one here.)
            MultiTrigger p = new MultiTrigger();
            p.Conditions.Add(new Condition(GreekStandard.AlphaProperty, 777));
            binding = new Binding("Beta");
            binding.Source = item1;
            p.Setters.Add(new Setter(GreekStandard.BetaProperty, binding, "Chrome"));
            binding = new Binding("Delta");
            binding.Source = item1;
            p.Setters.Add(new Setter(GreekStandard.DeltaProperty, binding, "Content"));
            t.Triggers.Add(p);

            // FrameworkTemplate.VisualTree specifies:
            //   [Root object "Chrome"]
            //          |
            //          +-----------------------+
            //          |                       |
            //   [Child object "Content"]   [Child object "ContentElement"]

            FrameworkElementFactory root = new FrameworkElementFactory(typeof(MyFrameworkElement), "Chrome");
            binding = new Binding("Beta");
            binding.Source = item2;
            root.SetValue(GreekStandard.BetaProperty, binding); // Check #1 below

            FrameworkElementFactory child = new FrameworkElementFactory(typeof(MyFrameworkElement), "Content");
            child.SetValue(GreekStandard.DeltaProperty, new TemplateBindingExtension(GreekStandard.DeltaProperty)); // Check #2 below

            root.AppendChild(child);

            // Test having a FrameworkContentElement in a Style VisualTree.
            FrameworkElementFactory childFCE = new FrameworkElementFactory(typeof(MyFrameworkContentElement), "ContentElement");
            childFCE.SetValue(MyFrameworkContentElement.ZuluProperty, new TemplateBindingExtension(GreekStandard.DeltaProperty));

            root.AppendChild(childFCE);

            Console.WriteLine("5420 - Apply test Template to test object");

            t.VisualTree = root;

            fe.Template = t;

            // Fault in VisualTree
            fe.ApplyTemplate();

            Console.WriteLine("5430 - Get FrameworkTemplate.VisualTree child nodes");

            // Grab the tree elements - this needs to be in sync with the
            // FrameworkElementFactory structure above or we won't get the
            // objects we expected to get.
            MyFrameworkElement chrome = (MyFrameworkElement)VisualTreeHelper.GetChild(fe, 0);
            MyFrameworkElement content = (MyFrameworkElement)chrome.Children[0];
            MyFrameworkContentElement contentElement = (MyFrameworkContentElement)chrome.Children[1];

            Console.WriteLine("5440 - Verify state of child objects is as expected");

            // Check #1
            Check(chrome.Beta == 11011,
                "Property value specified on the FrameworkElementFactory object didn't get picked up.  Step through FrameworkElement.GetValueCore and see why Template couldn't find the value.");

            fe.Delta = 99.99; // Trigger the alias for this check #2
            Check(content.Delta == 99.99,
                "Child object had Alias to TemplatedParent node, whose value was just set.  Child object should have picked up that value through the Alias mechanism. (1 of 2 : FrameworkElement)");

            // Another check for Alias - this time on the FrameworkContentElement
            Check(((double)contentElement.GetValue(MyFrameworkContentElement.ZuluProperty)) == 99.99,
                "Child object had Alias to TemplatedParent node, whose value was just set.  Child object should have picked up that value through the Alias mechanism. (2 of 2 : FrameworkContentElement)");

            Console.WriteLine("5450 - Activate MultiTrigger and verify it's active.");

            // Set the conditions to activate the MultiTrigger created above
            fe.Alpha = 777;
            Check(chrome.Beta == 20202,
                "Child object should have picked up value from an active MultiTrigger.  Did the trigger not go active?  Or did the child pick up a value from elsewhere? (1 of 2)");
            Check(content.Delta == 20.202,
                "Child object should have picked up value from an active MultiTrigger.  Did the trigger not go active?  Or did the child pick up a value from elsewhere? (2 of 2)");

            Console.WriteLine("5460 - Remove test Template from test object.");

            // Simple smoke test of Template invalidation code path.
            fe.Template = null;
        }

        // Test DesignerProperties
        void TestDesignerProperties()
        {
            // These are elements belonging to the designer app

            _root = new DockPanel();
            _frame = new Frame();
            _frameContent = new Button();
            _root.Children.Add(_frame);
            _frame.Content = _frameContent;

            // These are elements belonging to the design surface

            _designSurface = new StackPanel();
            _frameInDesignSurface = new Frame();
            _frameContentInDesignSurface = new Button();
            _root.Children.Add(_designSurface);
            _designSurface.Children.Add(_frameInDesignSurface);
            _frameInDesignSurface.Content = _frameContentInDesignSurface;

            // Set the IsInDesignModeProperty

            DesignerProperties.SetIsInDesignMode(_designSurface, true);

            // Layout this tree

            DRT.RootElement = _root;
            DRT.ShowRoot();

            // Verify the IsInDesignMode flag

            DRT.ResumeAt(new DrtTest(VerifyIsInDesignMode));
        }

        // Verify the IsInDesignMode flag
        private void VerifyIsInDesignMode()
        {
            // Verify the IsInDesignModeProperty value

            Check(DesignerProperties.GetIsInDesignMode(_root) == false, "IsInDesignMode must be false");
            Check(DesignerProperties.GetIsInDesignMode(_frame) == false, "IsInDesignMode must be false");
            Check(DesignerProperties.GetIsInDesignMode(_frameContent) == false, "IsInDesignMode must be false");
            Check(DesignerProperties.GetIsInDesignMode(_designSurface) == true, "IsInDesignMode must be true");
            Check(DesignerProperties.GetIsInDesignMode(_frameInDesignSurface) == true, "IsInDesignMode must be true");
            Check(DesignerProperties.GetIsInDesignMode(_frameContentInDesignSurface) == true, "IsInDesignMode must be true");

        }

        void TestDesignerCoerceValueCallback()
        {
            DependencyPropertyDescriptor dpd = DependencyPropertyDescriptor.FromProperty(MyFrameworkElement.ThetaProperty, typeof(MyFrameworkElement));
            dpd.DesignerCoerceValueCallback = (CoerceValueCallback) delegate(DependencyObject d, object baseValue)
                {
                    int theta = (int)baseValue;
                    return (theta + 2) * 5;
                };

            MyFrameworkElement myFE = new MyFrameworkElement();

            // LOCAL VALUE

            myFE.Theta = 10;
            Check(myFE.Theta == 65, "Actual: " + myFE.Theta + " Expected: 65\n" +
                "The value of Theta should reflect the result of normal coersion as well as designer coersion i.e. (10+1 + 2)*5 = 65");

            myFE.ClearValue(MyFrameworkElement.ThetaProperty);
            Check(myFE.ReadLocalValue(MyFrameworkElement.ThetaProperty) == DependencyProperty.UnsetValue, "Actual: " + myFE.ReadLocalValue(MyFrameworkElement.ThetaProperty) + " Expected: DependencyProperty.UnsetValue\n" +
                "The local value of Theta should should have been cleared");
            Check(myFE.Theta == 10, "Actual: " + myFE.Theta + " Expected: 10\n" +
                "The value of Theta should reflect the result of designer coersion on the default value i.e. (0+2)*5 = 10");

            // STYLE VALUE

            Style style = new Style();
            style.Setters.Add(new Setter(MyFrameworkElement.ThetaProperty, 20));

            myFE.Style = style;
            Check(myFE.Theta == 115, "Actual: " + myFE.Theta + " Expected: 115\n" +
                "The value of Theta should reflect the result of normal coersion as well as designer coersion on the style value i.e. (20+1+2)*5 = 115");

            myFE.ClearValue(FrameworkElement.StyleProperty);

            // INHERITED VALUE

            MyFrameworkElement myParentFE = new MyFrameworkElement();
            myParentFE.AppendChild(myFE);

            myParentFE.Theta = 30;
            Check(myFE.Theta == 840, "Actual: " + myFE.Theta + " Expected: 840\n" +
                "The value of Theta should reflect the result of normal coersion as well as designer coersion on the inherited value i.e. (((30+1+2)*5)+1+2)*5 = 840");

            myParentFE.RemoveModelChild(myFE);

            // CANCEL VALUE

            dpd.DesignerCoerceValueCallback = (CoerceValueCallback) delegate(DependencyObject d, object baseValue)
                {
                    return DependencyProperty.UnsetValue;
                };

            int oldValue = myFE.Theta;
            myFE.Theta = 10;
            Check(myFE.Theta == oldValue, "Actual: " + myFE.Theta + " Expected: " + oldValue + "\n" +
                "The value of Theta should reflect the oldValue of the property since the operation was cancelled by returning UnsetValue");

            dpd.DesignerCoerceValueCallback = null;

            // DEFAULT VALUE

            myFE.ClearValue(MyFrameworkElement.ThetaProperty);
            Check(myFE.Theta == 0, "Actual: " + myFE.Theta + " Expected: 0\n" +
                "The value of Theta should be the default since the DesignerCoerceValueCallback has been removed");

            myFE.InvalidateProperty(MyFrameworkElement.ThetaProperty);
            Check(myFE.Theta == 1, "Actual: " + myFE.Theta + " Expected: 1\n" +
                "The value of Theta should be the default in this case but it turns out that in the InvalidateProperty case we still call the coersion callback");
        }

        void TestDependencyPropertyDescriptor()
        {
            DependencyPropertyDescriptor dpd = DependencyPropertyDescriptor.FromProperty(MyFrameworkElement.TestTypeConverterProperty, typeof(MyFrameworkElement));
            TypeConverterAttribute tca = (TypeConverterAttribute)dpd.Attributes[typeof(TypeConverterAttribute)];
            Check(tca.ConverterTypeName.Contains("TestTypeConverter"), "Did not retrieve the TypeConverterAttribute on the Property Type");

            dpd = DependencyPropertyDescriptor.FromProperty(MyFrameworkElement.TestPropertyConverterProperty, typeof(MyFrameworkElement));
            tca = (TypeConverterAttribute)dpd.Attributes[typeof(TypeConverterAttribute)];
            Check(tca.ConverterTypeName.Contains("TestPropertyConverter"), "Did not retrieve the TypeConverterAttribute on the Property Settor");
        }

        private DockPanel _root;
        private Frame _frame;
        private Button _frameContent;
        private StackPanel _designSurface;
        private Frame _frameInDesignSurface;
        private Button _frameContentInDesignSurface;

    }

    public class TestTypeConverter : TypeConverter {}
    public class TestPropertyConverter : TypeConverter {}

    [TypeConverter(typeof(TestTypeConverter))]
    public class GreekStandard
    {
        public static readonly DependencyProperty AlphaProperty
            = DependencyProperty.RegisterAttached("Alpha", typeof(int), typeof(GreekStandard),
                                          new FrameworkPropertyMetadata(0,
                                                                        FrameworkPropertyMetadataOptions.Inherits),
                                          new ValidateValueCallback(ValidateAlphaValue));

        public static readonly DependencyProperty BetaProperty
            = DependencyProperty.RegisterAttached("Beta", typeof(int), typeof(GreekStandard),
                                          new FrameworkPropertyMetadata(0));

        public static readonly DependencyProperty DeltaProperty
            = DependencyProperty.RegisterAttached("Delta", typeof(double), typeof(GreekStandard),
                                          new PropertyMetadata(0.0));

        public static readonly DependencyProperty GammaProperty
            = DependencyProperty.RegisterAttached("Gamma", typeof(int), typeof(GreekStandard),
                                          new FrameworkPropertyMetadata(0,
                                                                        FrameworkPropertyMetadataOptions.Inherits));

        public static readonly DependencyProperty SigmaProperty
            = DependencyProperty.RegisterAttached("Sigma", typeof(int), typeof(GreekStandard),
                                          new FrameworkPropertyMetadata(0,
                                                                        FrameworkPropertyMetadataOptions.Inherits));

        internal static readonly DependencyProperty CoersionValueProperty
            = DependencyProperty.RegisterAttached("CoersionValue", typeof(int), typeof(GreekStandard),
                                          new PropertyMetadata(0));

        internal static readonly DependencyProperty AlphaDependentValueProperty
            = DependencyProperty.RegisterAttached("AlphaDependentValue", typeof(int), typeof(GreekStandard),
                                          new PropertyMetadata(0));

        internal static readonly DependencyProperty GammaDependentValueProperty
            = DependencyProperty.RegisterAttached("GammaDependentValue", typeof(int), typeof(GreekStandard),
                                          new PropertyMetadata(0));

        public static readonly DependencyProperty YabbaProperty
            = DependencyProperty.RegisterAttached("Yabba", typeof(int), typeof(GreekStandard),
                                          new FrameworkPropertyMetadata(0,
                                            FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.OverridesInheritanceBehavior));

        public static bool ValidateAlphaValue(object value)
        {
            int alpha = (int)value;
            return alpha <= 99999;
        }


        public class Group
        {
            public int Alpha;
            public int Beta;

            public override bool Equals(object o)
            {
                Group g = o as Group;
                if (g == null)
                {
                    return false;
                }

                return Alpha == g.Alpha &&
                       Beta == g.Beta;
            }

            public override int GetHashCode()
            {
                return Alpha ^ Beta;
            }
        }

        private static object ReadGreekGroup(DependencyObject d)
        {
            GreekStandard.Group g = new GreekStandard.Group();

            g.Alpha = (int)d.GetValue(GreekStandard.AlphaProperty);
            g.Beta = (int)d.GetValue(GreekStandard.BetaProperty);

            return g;
        }
    }

    /* Expressions removed from public use

        public class HalfExpression : Expression
        {
            public HalfExpression(DependencyObject d, DependencyProperty id) :
                base(ExpressionOptions.NonShareable)
            {
                _source = new DependencySource(d, id);
            }

            public override DependencySource[] GetSources()
            {
                return new DependencySource[] { _source };
            }

            public override object GetValue(DependencyObject d, DependencyProperty id)
            {
                int value = (int)_source.DependencyObject.GetValue(_source.DependencyProperty);

                value = value / 2;

                return value;
            }

            private DependencySource _source;
        }
    */
    public class Mercury : DependencyObject
    {
        static Mercury()
        {
            FrameworkPropertyMetadata metadata;

            metadata = new FrameworkPropertyMetadata(124);
            metadata.PropertyChangedCallback = new PropertyChangedCallback(OnAlphaChanged);
            GreekStandard.AlphaProperty.OverrideMetadata(typeof(Mercury), metadata);

            metadata = new FrameworkPropertyMetadata(200);
            metadata.CoerceValueCallback = new CoerceValueCallback(CoerceBeta);
            GreekStandard.BetaProperty.OverrideMetadata(typeof(Mercury), metadata);

            // Delta (overrides DefaultValue)
            metadata = new FrameworkPropertyMetadata();
            metadata.DefaultValue = 3.14;
            GreekStandard.DeltaProperty.OverrideMetadata(typeof(Mercury), metadata);
        }

        public int Alpha
        {
            get { return (int)GetValue(GreekStandard.AlphaProperty); }
            set { SetValue(GreekStandard.AlphaProperty, value); }
        }

        public int Beta
        {
            get { return (int)GetValue(GreekStandard.BetaProperty); }
            set { SetValue(GreekStandard.BetaProperty, value); }
        }

        public double Delta
        {
            get { return (double)GetValue(GreekStandard.DeltaProperty); }
            set { SetValue(GreekStandard.DeltaProperty, value); }
        }

        private static object CoerceBeta(DependencyObject d, object value)
        {
            int beta = (int)value;
            int alpha = ((Mercury)d).Alpha;
            if (beta < alpha)
            {
                return alpha;
            }

            return value;
        }

        private static void OnAlphaChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Alpha is an input to the coercion for Beta --
            // so invalidate Beta on a change to Alpha
            d.CoerceValue(GreekStandard.BetaProperty);
        }

        public void TestSettingUsingDependencyPropertyKey(int value)
        {
            // Normally people wouldn't directly expose a read-only property like this...
            SetValue(MercuryReadOnlyPropertyKey, value);
        }

        protected static DependencyPropertyKey MercuryReadOnlyPropertyKey = DependencyProperty.RegisterReadOnly("MercuryReadOnly", typeof(int), typeof(Mercury), null, null);
        public static DependencyProperty MercuryReadOnlyProperty = MercuryReadOnlyPropertyKey.DependencyProperty;

        public static DependencyProperty OverrideMeProperty = DependencyProperty.Register("OverrideMe", typeof(bool), typeof(Mercury), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnOverrideMeChanged)));
        public int _bestValueEver = 0;

        private static void OnOverrideMeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Mercury)d)._bestValueEver |= 0x1;
        }
    }

    public class BadMercury : Mercury
    {
        static BadMercury()
        {
            // This should fail up on first initialization since the property is read-only and we're not using a key
            MercuryReadOnlyProperty.OverrideMetadata(typeof(BadMercury), new PropertyMetadata());
        }
    }

    public class GoodMercury : Mercury
    {
        static GoodMercury()
        {
            // In contrast to BadMercury, we're passing in the appropriate authorization key.
            MercuryReadOnlyProperty.OverrideMetadata(typeof(GoodMercury), new PropertyMetadata(), MercuryReadOnlyPropertyKey);

            // make sure all the property change handlers are called
            OverrideMeProperty.OverrideMetadata(typeof(GoodMercury), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnOverrideMeChanged)));
        }

        private static void OnOverrideMeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((GoodMercury)d)._bestValueEver |= 0x2;
        }
    }

    public class VeryGoodMercury : GoodMercury
    {
        static VeryGoodMercury()
        {
            // make sure all the property change handlers are called
            OverrideMeProperty.OverrideMetadata(typeof(VeryGoodMercury), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnOverrideMeChanged)));
        }

        private static void OnOverrideMeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                ((VeryGoodMercury)d)._bestValueEver |= 0x4;
            }
        }
    }

    [ContentProperty("Children")]
    public class MyFrameworkElement : Control, IAddChild, ILogicalTreeParent
    {
        public MyFrameworkElement()
            : base()
        {
        }

        public MyFrameworkElement(InheritanceBehavior inheritanceBehavior)
            : base()
        {
            InheritanceBehavior = inheritanceBehavior;
        }

        public int Alpha
        {
            get { return (int)GetValue(GreekStandard.AlphaProperty); }
            set { SetValue(GreekStandard.AlphaProperty, value); }
        }

        public int Beta
        {
            get { return (int)GetValue(GreekStandard.BetaProperty); }
            set { SetValue(GreekStandard.BetaProperty, value); }
        }

        public double Delta
        {
            get { return (double)GetValue(GreekStandard.DeltaProperty); }
            set { SetValue(GreekStandard.DeltaProperty, value); }
        }

        public int Gamma
        {
            get { return (int)GetValue(GreekStandard.GammaProperty); }
            set { SetValue(GreekStandard.GammaProperty, value); }
        }

        public int Sigma
        {
            get { return (int)GetValue(GreekStandard.SigmaProperty); }
            set { SetValue(GreekStandard.SigmaProperty, value); }
        }

        public int Yabba
        {
            get { return (int)GetValue(GreekStandard.YabbaProperty); }
            set { SetValue(GreekStandard.YabbaProperty, value); }
        }

        public static readonly DependencyProperty ThetaProperty
            = DependencyProperty.RegisterAttached("Theta", typeof(int), typeof(MyFrameworkElement),
                                          new FrameworkPropertyMetadata(0,
                                            FrameworkPropertyMetadataOptions.Inherits,
                                            null,
                                            (CoerceValueCallback)delegate(DependencyObject d, object value)
                                            {
                                                int theta = (int)value;
                                                return theta+1;
                                            }));

        public int Theta
        {
            get { return (int)GetValue(ThetaProperty); }
            set { SetValue(ThetaProperty, value); }
        }

        public static readonly DependencyProperty ZetaProperty
            = DependencyProperty.RegisterAttached("Zeta", typeof(int), typeof(MyFrameworkElement),
                                          new FrameworkPropertyMetadata(267,
                                            FrameworkPropertyMetadataOptions.Inherits,
                                            null,
                                            new CoerceValueCallback(CoerceZeta)));

        public int Zeta
        {
            get { return (int)GetValue(ZetaProperty); }
            set { SetValue(ZetaProperty, value); }
        }

        internal int ZetaCoercionValue { get; set; }

        private static object CoerceZeta(DependencyObject d, object baseValue)
        {
            MyFrameworkElement fe = (MyFrameworkElement)d;
            return (fe.ZetaCoercionValue == 0) ? baseValue : fe.ZetaCoercionValue;
        }

        internal int CoersionValue
        {
            get { return (int)GetValue(GreekStandard.CoersionValueProperty); }
            set { SetValue(GreekStandard.CoersionValueProperty, value); }
        }

        internal int AlphaDependentValue
        {
            get { return (int)GetValue(GreekStandard.AlphaDependentValueProperty); }
            set { SetValue(GreekStandard.AlphaDependentValueProperty, value); }
        }

        internal int GammaDependentValue
        {
            get { return (int)GetValue(GreekStandard.GammaDependentValueProperty); }
            set { SetValue(GreekStandard.GammaDependentValueProperty, value); }
        }

        public static readonly DependencyProperty TestTypeConverterProperty
            = DependencyProperty.RegisterAttached("TestTypeConverter", typeof(GreekStandard), typeof(MyFrameworkElement),
                                          new FrameworkPropertyMetadata(null));

        public static GreekStandard GetTestTypeConverter(DependencyObject d)
        {
            return (GreekStandard)d.GetValue(TestTypeConverterProperty);
        }

        public static void SetTestTypeConverter(DependencyObject d, GreekStandard value)
        {
            d.SetValue(TestTypeConverterProperty, value);
        }

        public static readonly DependencyProperty TestPropertyConverterProperty
            = DependencyProperty.RegisterAttached("TestPropertyConverter", typeof(GreekStandard), typeof(MyFrameworkElement),
                                          new FrameworkPropertyMetadata(null));

        [TypeConverter(typeof(TestPropertyConverter))]
        public static GreekStandard GetTestPropertyConverter(DependencyObject d)
        {
            return (GreekStandard)d.GetValue(TestPropertyConverterProperty);
        }

        public static void SetTestPropertyConverter(DependencyObject d, GreekStandard value)
        {
            d.SetValue(TestPropertyConverterProperty, value);
        }

        static MyFrameworkElement()
        {
            FrameworkPropertyMetadata metadata;

            // Alpha (overrides DefaultValue)
            metadata = new FrameworkPropertyMetadata();
            metadata.DefaultValue = 458;
            metadata.CoerceValueCallback = new CoerceValueCallback(CoerceAlpha);
            metadata.PropertyChangedCallback = new PropertyChangedCallback(OnAlphaChanged);
            GreekStandard.AlphaProperty.OverrideMetadata(typeof(MyFrameworkElement), metadata);

            // Beta (override DefaultValue)
            metadata = new FrameworkPropertyMetadata();
            metadata.DefaultValue = 628;
            GreekStandard.BetaProperty.OverrideMetadata(typeof(MyFrameworkElement), metadata);

            // Delta
            GreekStandard.DeltaProperty.OverrideMetadata(typeof(MyFrameworkElement),
                                                         new FrameworkPropertyMetadata());

            // Gamma (overrides DefaultValue)
            metadata = new FrameworkPropertyMetadata();
            metadata.DefaultValue = 888;
            metadata.PropertyChangedCallback = new PropertyChangedCallback(OnGammaChanged);
            GreekStandard.GammaProperty.OverrideMetadata(typeof(MyFrameworkElement), metadata);

            // Sigma (overrides DefaultValue)
            metadata = new FrameworkPropertyMetadata();
            metadata.DefaultValue = 918;
            GreekStandard.SigmaProperty.OverrideMetadata(typeof(MyFrameworkElement), metadata);
        }

        private static object CoerceAlpha(DependencyObject d, object baseValue)
        {
            int coerceValue = ((MyFrameworkElement)d).CoersionValue;
            int defaultValue = (int)GreekStandard.AlphaProperty.GetMetadata(d).DefaultValue;
            if (coerceValue != 0 && (int)baseValue == defaultValue)
            {
                return coerceValue;
            }

            return baseValue;
        }

        private static void OnAlphaChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MyFrameworkElement fe = (MyFrameworkElement)d;
            int alphaDependentValue = fe.AlphaDependentValue;
            if (alphaDependentValue != 0)
            {
                fe.Gamma = alphaDependentValue;
            }
        }

        private static void OnGammaChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MyFrameworkElement fe = (MyFrameworkElement)d;
            int gammaDependentValue = fe.GammaDependentValue;
            if (gammaDependentValue != 0)
            {
                fe.Sigma = gammaDependentValue;
            }
        }

        public void AppendModelChild(object modelChild)
        {
            Children.Add(modelChild);
        }

        public void RemoveModelChild(object modelChild)
        {
            Children.Remove(modelChild);
        }

        void IAddChild.AddChild(object o)
        {
            AppendModelChild(o);
        }

        void IAddChild.AddText(string s)
        {
        }


        #region ILogicalTreeParent
        void ILogicalTreeParent.AddLogicalChild(object child)
        {
            AddLogicalChild(child);
        }

        void ILogicalTreeParent.RemoveLogicalChild(object child)
        {
            RemoveLogicalChild(child);
        }
        #endregion ILogicalTreeParent

        protected override IEnumerator LogicalChildren
        {
            get { return Children.GetEnumerator(); }
        }

        public void AppendChild(Visual child)
        {
            Children.Add(child);
        }


        /// <summary>
        ///   Derived class must implement to support Visual children. The method must return
        ///    the child at the specified index. Index must be between 0 and GetVisualChildrenCount-1.
        ///
        ///    By default a Visual does not have any children.
        ///
        ///  Remark:
        ///       During this virtual call it is not valid to modify the Visual tree.
        /// </summary>
        protected override Visual GetVisualChild(int index)
        {
            // if you have a template
            if (base.VisualChildrenCount != 0 && index == 0)
            {
                return base.GetVisualChild(0);
            }
            // otherwise you can have your own children
            if (Children == null)
            {
                throw new ArgumentOutOfRangeException("index is out of range");
            }
            if (index < 0 || index >= Children.Count)
            {
                throw new ArgumentOutOfRangeException("index is out of range");
            }

            return Children[index] as Visual;
        }

        /// <summary>
        ///  Derived classes override this property to enable the Visual code to enumerate
        ///  the Visual children. Derived classes need to return the number of children
        ///  from this method.
        ///
        ///    By default a Visual does not have any children.
        ///
        ///  Remark: During this virtual method the Visual tree must not be modified.
        /// </summary>
        protected override int VisualChildrenCount
        {
            get
            {
                //you can either have a Template or your own children
                if (base.VisualChildrenCount > 0) return 1;
                else return Children.Count;
            }
        }

        private UIElementCollection2 _children;
        public UIElementCollection2 Children
        {
            get
            {
                if (_children == null)
                    _children = new UIElementCollection2(this);
                return _children;
            }
        }
    }



    public class MyFrameworkContentElement : FrameworkContentElement
    {
        // For arbitrary names - counting backwards from "Z" using radio alphabet.
        public static readonly DependencyProperty ZuluProperty
            = DependencyProperty.Register("Zulu", typeof(double), typeof(MyFrameworkContentElement));

        public MyFrameworkContentElement() : base() { }

        static MyFrameworkContentElement()
        {
            FrameworkPropertyMetadata metadata;

            // Alpha (overrides Default and Inherits)
            metadata = new FrameworkPropertyMetadata();
            metadata.DefaultValue = 542;
            GreekStandard.AlphaProperty.OverrideMetadata(typeof(MyFrameworkContentElement), metadata);
        }

        public void AppendModelChild(object modelChild)
        {
            _modelChildren.Add(modelChild);
            AddLogicalChild(modelChild);
        }

        public void RemoveModelChild(object modelChild)
        {
            _modelChildren.Remove(modelChild);
            RemoveLogicalChild(modelChild);
        }

        protected override IEnumerator LogicalChildren
        {
            get { return _modelChildren.GetEnumerator(); }
        }

        private ArrayList _modelChildren = new ArrayList();
    }

    public class FrameworkElementWithDefaultStyle : FrameworkElement
    {
        public FrameworkElementWithDefaultStyle() : base() { }

        static FrameworkElementWithDefaultStyle()
        {
            StyleProperty.OverrideMetadata(typeof(FrameworkElementWithDefaultStyle),
                new FrameworkPropertyMetadata(new Style(typeof(FrameworkElementWithDefaultStyle))));
        }
    }

    public class MyDataItem
    {
        public MyDataItem(int alpha, int beta, double delta)
        {
            _alpha = alpha;
            _beta = beta;
            _delta = delta;
        }

        int _alpha;
        public int Alpha { get { return _alpha; } }

        int _beta;
        public int Beta { get { return _beta; } }

        double _delta;
        public double Delta { get { return _delta; } }
    }

    interface ILogicalTreeParent
    {
        void AddLogicalChild(object element);
        void RemoveLogicalChild(object element);
    }

    public class UIElementCollection2 : Collection<object>
    {
        private VisualCollection _visuals;
        private ILogicalTreeParent _logicalTreeParent;

        internal UIElementCollection2(FrameworkElement parent)
        {
            _visuals = new VisualCollection(parent);
            _logicalTreeParent = parent as ILogicalTreeParent; //If the class is a logical tree parent, we'll maintain LogicalChildren with the help of the parent
        }

        protected override void InsertItem(int index, object item)
        {
            _visuals.Insert(index, item as Visual);
            if (_logicalTreeParent != null)
                _logicalTreeParent.AddLogicalChild(item);
            base.InsertItem(index, item);
        }

        protected override void SetItem(int index, object item)
        {
            _visuals[index] = item as Visual;
            if (_logicalTreeParent != null)
            {
                _logicalTreeParent.RemoveLogicalChild(this[index]);
                _logicalTreeParent.AddLogicalChild(item);
            }
            base.SetItem(index, item);
        }
        protected override void RemoveItem(int index)
        {
            _visuals.RemoveAt(index);
            if (_logicalTreeParent != null)
            {
                _logicalTreeParent.RemoveLogicalChild(this[index]);
            }
            base.RemoveItem(index);
        }
        protected override void ClearItems()
        {
            _visuals.Clear();
            base.ClearItems();
        }
    }
}
