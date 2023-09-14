// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using Avalon.Test.CoreUI.CoreInput.Common;

using Microsoft.Test;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test;
using Microsoft.Test.Logging;

namespace Avalon.Test.CoreUI
{
    // Bugfix   implements changes to behavior of two public classes:
    //      System.Windows.Setter and
    //      System.Windows.FrameworkElementFactory.
    //
    // Changed Behavior:
    // ** Setter.Value assignment no longer checks the type of the object being assigned.
    // ** Setter.Seal() method and FrameworkElementFactory.SetValue() methods both call a
    //    modified internal StyleHelper.IsStylingLogicalTree() method which examines
    //    the particular combination of its arguments.
    // ** Setter.Seal() and FrameworkElementFactory.SetValue()
    //    each throw a NotSupportedException when IsStylingLogicalTree() returns true
    //
    // Due to:
    // StyleHelper.IsStylingLogicalTree(DependencyObject dp, Object value) method
    // is changed in the following way:
    //
    // Before: return true if value is Visual or value is ContentElement
    //         -- but unconditionally return false
    //            if dp ==ItemsControl.ItemsPanelProperty
    //
    // After: return true if value is Visual or value is ContentElement
    //         -- but unconditionally return false
    //            if dp == IntemsControl.ItemsPanelProperty
    //            or dp == FrameworkElement.ContextMenuProperty
    //            or dp == FrameworkElement.ToolTipProperty
    // .....................................................................................

    [TestDefaults(DefaultPriority = 1, DefaultMethodName = "RunTest")]
    public class StyleSetterSpecialCaseApp_Bug35
    {
        private static TestLog s_log;

        [Test(1, @"PropertyEngine\StyleBugs", TestCaseSecurityLevel.FullTrust, "StyleSetterSpecialCaseApp_Bug35_Style", MethodName = "RunTest", MethodParams = "StyleSetterSpecialCaseApp_Bug35_Style")]
        [Test(1, @"PropertyEngine\StyleBugs", TestCaseSecurityLevel.FullTrust, "StyleSetterSpecialCaseApp_Bug35_Factory", MethodName = "RunTest", MethodParams = "StyleSetterSpecialCaseApp_Bug35_Factory")]
        public static TestLog RunTest(string testName)
        {
            s_log = TestLog.Current;

            switch (testName)
            {
                case "StyleSetterSpecialCaseApp_Bug35_Style":
                    TestStylingHelper_via_Style_Seal(testName);
                    break;
                case "StyleSetterSpecialCaseApp_Bug35_Factory":
                    TestStylingHelper_via_FrameworkElementFactory_SetValue(testName);
                    break;
            }
            return s_log;
        }

        /// <summary>
        /// A sub-test that targets bugfix   changes that relax all restriction on the type
        /// that can be assigned to a Setter.Value, even when Setter.Property is null.
        /// </summary>
        /// <param name="setterA1"></param>
        /// <param name="setterA2"></param>
        /// <param name="contentElementA1"></param>
        /// <param name="contextMenuA2"></param>
        /// <returns></returns>
        private static TestResult TestSetter_SetValue( out Setter setterA1,
                                                       out Setter setterA2,
                                                       out Setter setterA3,
                                                       out Setter setterA4,
                                                       out ContentElement contentElementA1,
                                                       out UIElement contextMenuA2,
                                                       out ItemsPanelTemplate panelTemplateA3,
                                                       out ContentElement contentElementA4)
        {
            setterA1 = new Setter();
            Debug.Assert(null != setterA1);

            setterA2 = new Setter();
            Debug.Assert(null != setterA2);

            setterA3 = new Setter();
            Debug.Assert(null != setterA3);

            setterA4 = new Setter();
            Debug.Assert(null != setterA4);

            contentElementA1 = new ContentElement();
            Debug.Assert(null != contentElementA1);

            // UIElement must be a ContextMenu instance to be a valid value for ContextMenuProperty later
            contextMenuA2 = new ContextMenu();
            Debug.Assert(null != contextMenuA2);

            panelTemplateA3 = new ItemsPanelTemplate();
            Debug.Assert(null != panelTemplateA3);

            contentElementA4 = new ContentElement();
            Debug.Assert(null != contentElementA4);

            // Verify bugfix now unconditionally allows setting Setter.Value to a ContentElement
            s_log.LogStatus("Test: Setter.Value must accept assignment of ContentElement instance (#1).");
            setterA1.Value = contentElementA1;

            if (setterA1.Value != contentElementA1)
            {
                s_log.LogEvidence("FAIL!: Setter.Value did not get set to ContentElement instance (#1).");
                return TestResult.Fail;
            }

            // Verify bugfix now unconditionally allows setting Setter.Value to a Visual
            s_log.LogStatus("Test: Setter.Value must accept assignment of UIElement instance.");
            setterA2.Value = contextMenuA2;

            if (setterA2.Value != contextMenuA2)
            {
                s_log.LogEvidence("FAIL!: Setter.Value property did not get set to UIElement instance.");
                return TestResult.Fail;
            }

            // Verify bugfix still unconditionally allows setting Setter.Value to an ItemsPanelTemplate
            s_log.LogStatus("Test: Setter.Value must accept assignment of ItemsPanelTemplate instance.");
            setterA3.Value = panelTemplateA3;

            if (setterA3.Value != panelTemplateA3)
            {
                s_log.LogEvidence("FAIL!: Setter.Value property did not get set to ItemsPanelTemplate instance.");
                return TestResult.Fail;
            }

            // Verify bugfix now unconditionally allows setting Setter.Value to a ContentElement
            // This test is redundant with #1 but implemented for uniformity for caller.
            s_log.LogStatus("Test: Setter.Value must accept assignment of ContentElement instance (#4).");
            setterA4.Value = contentElementA4;

            if (setterA4.Value != contentElementA4)
            {
                s_log.LogEvidence("FAIL!: Setter.Value property did not get set to ContentElement instance (#4).");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        /// <summary>
        /// Invoke Style.Seal() to target bugfix   changes that relax constraint of StyleHelper.IsStylingLogicalTree()
        /// and now return false when DP is any of:
        ///      { ItemsControl.ItemsPanelProperty, FrameworkElement.ContextMenuProperty, FrameworkElement.ToolTipProperty}
        /// Excercize internal IsStylingLogicalTree( indirectly via Style.Seal()
        /// </summary>
        /// <param name="testName"></param>
        public static void TestStylingHelper_via_Style_Seal(String testName)
        {
            s_log.LogStatus("Starting test: " + testName);

            Setter setterA1;
            Setter setterA2;
            Setter setterA3;
            Setter setterA4;
            ContentElement contentElementA1;
            UIElement contextMenuA2;
            ItemsPanelTemplate panelTemplateA3;
            ContentElement contentElementA4;

            // Invoke sub-test of Setter.Value fix
            {
                TestResult result = TestSetter_SetValue(out setterA1,
                                                        out setterA2,
                                                        out setterA3,
                                                        out setterA4,
                                                        out contentElementA1,
                                                        out contextMenuA2,
                                                        out panelTemplateA3,
                                                        out contentElementA4);
                if ( result != TestResult.Pass)
                {
                    s_log.Result= result;
                    return;
                }
            }

            Style styleA1 = new Style(typeof(FrameworkElement));
            Debug.Assert(null != styleA1);

            Style styleA2 = new Style(typeof(FrameworkElement));
            Debug.Assert(null != styleA2);

            Style styleA3 = new Style(typeof(FrameworkElement));
            Debug.Assert(null != styleA3);

            Style styleA4 = new Style(typeof(FrameworkElement));
            Debug.Assert(null != styleA4);

            // Setter.Seal() tests
            // =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            //    Setter.Property     |   Setter.Value          |    !IsStylingLogicalTree(dp,value)
            //    -------------      ---  ---------------      ---   -----------------------------
            // 1. ToolTipProperty     |  ContentElement         |     true
            // 2. ContextMenuProperty |  UIElement              |     true
            // 3. ItemsPanelProperty  |  ItemsPanelTemplate     |     true    <<Extra-Special case since ItemsPanelTemplate is neither a Visual nor a ContentElement>>
            // 4. TagProperty         |  ContentElement         |     false   Setter.Seal() throws NotSupportedException
            // =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-

            // 1. Verify bugfix affects Setter.Seal() to allow Setter.Value to be ContentElement when
            //    Setter.Property is FrameworkElement.ToolTipProperty.
            // Note: Style.Seal() calls Setter.Seal() on members of its Setter collection
            //
            Debug.Assert(contentElementA1 is ContentElement);
            Debug.Assert(setterA1.Value == contentElementA1);

            setterA1.Property = FrameworkElement.ToolTipProperty;
            Debug.Assert(setterA1.Property == FrameworkElement.ToolTipProperty);

            styleA1.Setters.Add(setterA1);
            Debug.Assert(styleA1.Setters.Contains(setterA1));

            s_log.LogStatus("Test: Setter Seal() for Setter.Property == ToolTipProperty && Setter.Value is ContentElement.");
            styleA1.Seal();
            if (!styleA1.IsSealed)
            {
                s_log.Result = TestResult.Fail;
                s_log.LogEvidence("FAIL!: Setter.Seal() for Setter.Property == ToolTipProperty && Setter.Value is ContentElement.");
                return;
            }

            // 2. Verify bugfix affects Setter.Seal() to allow Setter.Value to be UIElement when
            //    Setter.Property is FrameworkElement.ContextMenuProperty
            // Note: Style.Seal() calls Setter.Seal() on members of its Setter collection
            //
            Debug.Assert(contextMenuA2 is ContextMenu);
            Debug.Assert(setterA2.Value == contextMenuA2);

            setterA2.Property = FrameworkElement.ContextMenuProperty;
            Debug.Assert(setterA2.Property == FrameworkElement.ContextMenuProperty);

            styleA2.Setters.Add(setterA2);
            Debug.Assert(styleA2.Setters.Contains(setterA2));

            s_log.LogStatus("Test: Setter.Seal() for Setter.Property == ContextMenuProperty && Setter.Value is ContextMenu, a Visual.");
            styleA2.Seal();
            if (!styleA2.IsSealed)
            {
                s_log.Result = TestResult.Fail;
                s_log.LogEvidence("FAIL!: Setter.Seal() for Setter.Property == ContextMenuProperty && Setter.Value is ContextMenu, a Visual.");
                return;
            }

            // 3. Verify bugfix unchanged for Setter.Seal() to allow Setter.Value to be ItemsPanelTemplate when
            //    Setter.Property is ItemsControl.ItemsPanelProperty
            // Note: Style.Seal() calls Setter.Seal() on members of its Setter collection
            // Note: Extra-Special case since ItemsPanelTemplate is neither a Visual nor a ContentElement
            //
            Debug.Assert(panelTemplateA3 is ItemsPanelTemplate);
            Debug.Assert(setterA3.Value == panelTemplateA3);

            setterA3.Property = ItemsControl.ItemsPanelProperty;
            Debug.Assert(setterA3.Property == ItemsControl.ItemsPanelProperty);

            styleA3.Setters.Add(setterA3);
            Debug.Assert(styleA3.Setters.Contains(setterA3));

            s_log.LogStatus("Test: Setter.Seal() for Setter.Property == ItemsPanelProperty && Setter.Value is ItemsPanelTemplate.");
            styleA3.Seal();
            if (!styleA3.IsSealed)
            {
                s_log.Result = TestResult.Fail;
                s_log.LogEvidence("FAIL!: Setter.Seal() for Setter.Property == ItemsPanelProperty && Setter.Value is ItemsPanelTemplate.");
                return;
            }

            // 4. Verify bugfix continues to reject Setter.Value == ContentElement when Setter.Property is
            //    not in the set of special-cased DPs {ItemsControl.ItemsPanelProperty, FrameworkElement.ContextMenuProperty,
            //    and FrameworkElement.ToolTipProperty}
            Debug.Assert(contentElementA4 is ContentElement);
            Debug.Assert(setterA4.Value == contentElementA4);

            setterA4.Property = FrameworkElement.TagProperty;
            Debug.Assert(setterA4.Property == FrameworkElement.TagProperty);

            styleA4.Setters.Add(setterA4);
            Debug.Assert(styleA4.Setters.Contains(setterA4));

            s_log.LogStatus("Test: Setter.Seal() throws NotSupportedException for DP == TagProperty && VALUE is ContentElement.");
            ExceptionHelper.ExpectException<NotSupportedException>( delegate() { styleA4.Seal(); },
                                                                    delegate(NotSupportedException e) { ;});
            
            s_log.Result = TestResult.Pass;
            s_log.LogStatus("Finished test: " + testName);
            return;
        }

        /// <summary>
        /// Invoke FrameworkElementFactory.SetValue(dp,value) to target bugfix   changes that relax constraint of
        /// StyleHelper.IsStylingLogicalTree() and now return false when DP is any of:
        ///      { ItemsControl.ItemsPanelProperty, FrameworkElement.ContextMenuProperty, FrameworkElement.ToolTipProperty}
        /// Excercize internal IsStylingLogicalTree() indirectly via FrameworkElementFactory_SetValue(...)
        /// </summary>
        /// <param name="testName"></param>
        public static void TestStylingHelper_via_FrameworkElementFactory_SetValue(String testName)
        {
            s_log.LogStatus("Starting test: " + testName);
            TestResult testResult = TestResult.Fail;

            // 1 ...................
            // UIElement must be a ContextMenu class to be a IsValid value for ContextMenuProperty
            // ie. DependencyProperty.IsValidValue(Object value)
            UIElement contextMenuB1 = new ContextMenu();
            Debug.Assert(null != contextMenuB1);

            FrameworkElementFactory elementFactoryB1 = new FrameworkElementFactory();
            Debug.Assert(null != elementFactoryB1);

            // 2 ...................
            ContentElement contentElementB2 = new ContentElement();
            Debug.Assert(null != contentElementB2);

            FrameworkElementFactory elementFactoryB2 = new FrameworkElementFactory();
            Debug.Assert(null != elementFactoryB2);

            // 3 ...................
            // Value must be a ItemsPanelTemplate instance to be a IsValid value for ItemsPanelProperty
            // ie. DependencyProperty.IsValidValue(Object value)
            ItemsPanelTemplate panelTemplateB3 = new ItemsPanelTemplate();
            Debug.Assert(null != panelTemplateB3);

            FrameworkElementFactory elementFactoryB3 = new FrameworkElementFactory();
            Debug.Assert(null != elementFactoryB3);

            // 4 ...................
            // objectB4 IS NOT a Visual NOR a ContentElement so IsStylingLogicalTree(dp,value)
            // wiil be false given a DP that IsValid for Object (eg. TagProperty)
            Object objectB4 = new Object();
            Debug.Assert(null != objectB4);

            FrameworkElementFactory elementFactoryB4 = new FrameworkElementFactory();
            Debug.Assert(null != elementFactoryB4);

            // 5 ...................
            // menuB5 IS a Visual so IsStylingLogicalTree(dp,value) will be true given
            // a DP (eg. TagProperty) which is not one of the special-cased DP types. 
            UIElement menuB5 = new Menu();
            Debug.Assert(null != menuB5);

            FrameworkElementFactory elementFactoryB5 = new FrameworkElementFactory();
            Debug.Assert(null != elementFactoryB5);

            // 6 ...................
            // contentElementB6 is a ContentElement so IsStylingLogicalTree(dp,value) will be 
            // true given a DP (eg. TagProperty) which is not one of the special-cased DP types.
            //
            ContentElement contentElementB6 = new ContentElement();
            Debug.Assert(null != contentElementB6);

            FrameworkElementFactory elementFactoryB6 = new FrameworkElementFactory();
            Debug.Assert(null != elementFactoryB6);

            // FrameworkElementFactory.SetValue( dp, value) tests
            // =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
            //          dp               |       value           |    !IsStylingLogicalTree(dp,value)
            //     -----------------    ---   ---------------   ---   -------------------------------
            // 1.  ContextMenuProperty   |    ContextMenu        |         true
            // 2.  ToolTipProperty       |    ContentElement     |         true
            // 3.  ItemsPanelProperty    |    ItemsPanelTemplate |         true    <<Extra-Special case since ItemsPanelTemplate is neither a Visual nor a ContentElement>>
            // 4.  TagProperty           |    Object             |         true
            // 5.  TagProperty           |    Menu             |         false   SetValue(dp,value) throws NotSupportedException
            // 6.  TagProperty           |    ContentElement     |         false   SetValue(dp,value) throws NotSupportedException
            // =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=

            // 1. Verify FrameworkElementFactory.SetValue(dp, value) now accepts a UIElement value when
            //    dp is FrameworkElement.ContextMenuProperty -- because StyleHelper.IsStylingLogicalTree(dp,value) special-cases ContextMenuProperty
            //    for Visual value
            // FYI: There is no "GetValue" method to call for direct confirmation of "SetValue"
            s_log.LogStatus("Test: FrameworkElementFactory.SetValue( DP, VALUE) for DP == ContextMenuProperty && VALUE is ContextMenu, a Visual.");
            elementFactoryB1.SetValue(FrameworkElement.ContextMenuProperty, contextMenuB1);

            // 2. Verify FrameworkElementFactory.SetValue(dp, value) now accepts a ContentElement value when
            //    dp is FrameworkElement.ToolTipProperty -- because StyleHelper.IsStylingLogicalTree(dp,value) special-cases ToolTipProperty
            //    for ContentElement value.
            //
            s_log.LogStatus("Test: FrameworkElementFactory.SetValue( DP, VALUE) for DP == ToolTipProperty && VALUE is ContentElement.");
            elementFactoryB2.SetValue(FrameworkElement.ToolTipProperty, contentElementB2);

            // 3. Verify FrameworkElementFactory.SetValue(dp, value) still accepts a ItemsPanelTemplate value when
            //    dp is ItemsControl.ItemsPanelProperty -- because StyleHelper.IsStylingLogicalTree(dp,value) special-cases ItemsPanelProperty
            // Note: Extra-Special case since ItemsPanelTemplate is neither a Visual nor a ContentElement
            //
            s_log.LogStatus("Test: FrameworkElementFactory.SetValue( DP, VALUE) for DP == ItemsPanelProperty && VALUE is ItemsPanelTemplate.");
            elementFactoryB3.SetValue(ItemsControl.ItemsPanelProperty, panelTemplateB3);


            // 4. Verify FrameworkElementFactory.SetValue(dp, value) accepts a non-Visual, non-ContentElement Object when
            //    dp is FrameworkElement.TagProperty -- StyleHelper.IsStylingLogicalTree(dp,value) is only concerned about
            //    Visual and ContentElement values.
            //
            s_log.LogStatus("Test: FrameworkElementFactory.SetValue( DP, VALUE) for DP == TagProperty && VALUE is Object.");
            elementFactoryB3.SetValue(FrameworkElement.TagProperty, objectB4);

            // 5. Verify FrameworkElementFactory.SetValue(dp, value) still does not accept a UIElement when
            //    dp is FrameworkElement.TagProperty -- because StyleHelper.IsStylingLogicalTree(dp,value) does NOT special-case TagProperty
            //    for Visual value
            //
            s_log.LogStatus("Test: FrameworkElementFactory.SetValue( DP, VALUE) throws NotSupportedException for DP == TagProperty && VALUE is Menu, a Visual.");
            ExceptionHelper.ExpectException<NotSupportedException>(delegate() { elementFactoryB5.SetValue(FrameworkElement.TagProperty, menuB5); },
                                                                    delegate(NotSupportedException e) {;});

            // 6. Verify FrameworkElementFactory.SetValue(dp, value) still does not accept a UIElement when
            //    dp is FrameworkElement.TagProperty -- because StyleHelper.IsStylingLogicalTree(dp,value) does NOT special-case TagProperty
            //    for Visual value
            //
            s_log.LogStatus("Test: FrameworkElementFactory.SetValue( DP, VALUE) throws NotSupportedException for DP == TagProperty && VALUE is ContentElement.");
            ExceptionHelper.ExpectException<NotSupportedException>(delegate() { elementFactoryB6.SetValue(FrameworkElement.TagProperty, contentElementB6); },
                                                                    delegate(NotSupportedException e) { ;});

            testResult = TestResult.Pass;
            s_log.LogStatus("Finished test: " + testName);
            s_log.Result = testResult;
        }
    }
}

