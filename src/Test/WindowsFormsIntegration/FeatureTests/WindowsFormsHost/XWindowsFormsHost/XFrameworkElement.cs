// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using ReflectTools;
using ReflectTools.AutoPME;
using WFCTestLib.Util;
using WFCTestLib.Log;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;

//
// System.Windows.FrameworkElement AutoPME Test
//
public class XFrameworkElement : XUIElement {
    public XFrameworkElement(String[] args) : base(args) { }

    FrameworkElement GetFrameworkElement(TParams p) {
        if ( p.target is FrameworkElement )
            return (FrameworkElement)p.target;
        else {
            p.log.WriteLine("target isn't type FrameworkElement");
            return null;
        }
    }

    //========================================
    // Test Methods
    //========================================
    protected ScenarioResult get_Parent(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult RegisterName(TParams p, String name, Object scopedElement) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult UnregisterName(TParams p, String name) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult FindName(TParams p, String name) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult get_Style(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult set_Style(TParams p, Style value) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult ShouldSerializeStyle(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult get_OverridesDefaultStyle(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult set_OverridesDefaultStyle(TParams p, Boolean value) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult ApplyTemplate(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult OnApplyTemplate(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult BeginStoryboard(TParams p, Storyboard storyboard) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult BeginStoryboard(TParams p, Storyboard storyboard, HandoffBehavior handoffBehavior) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult BeginStoryboard(TParams p, Storyboard storyboard, HandoffBehavior handoffBehavior, Boolean isControllable) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult get_Triggers(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult ShouldSerializeTriggers(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult get_TemplatedParent(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult get_Resources(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult set_Resources(TParams p, ResourceDictionary value) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult ShouldSerializeResources(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult FindResource(TParams p, Object resourceKey) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult TryFindResource(TParams p, Object resourceKey) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult SetResourceReference(TParams p, DependencyProperty dp, Object name) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult get_DataContext(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult set_DataContext(TParams p, Object value) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult GetBindingExpression(TParams p, DependencyProperty dp) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult SetBinding(TParams p, DependencyProperty dp, BindingBase binding) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult SetBinding(TParams p, DependencyProperty dp, String path) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult get_Language(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult set_Language(TParams p, XmlLanguage value) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult get_Name(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult set_Name(TParams p, String value) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult get_Tag(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult set_Tag(TParams p, Object value) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult get_InputScope(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult set_InputScope(TParams p, InputScope value) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult BringIntoView(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult BringIntoView(TParams p, Rect targetRectangle) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult get_ActualWidth(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult get_ActualHeight(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult get_LayoutTransform(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult set_LayoutTransform(TParams p, Transform value) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult get_Width(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult set_Width(TParams p, Double value) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult get_MinWidth(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult set_MinWidth(TParams p, Double value) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult get_MaxWidth(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult set_MaxWidth(TParams p, Double value) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult get_Height(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult set_Height(TParams p, Double value) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult get_MinHeight(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult set_MinHeight(TParams p, Double value) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult get_MaxHeight(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult set_MaxHeight(TParams p, Double value) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult get_FlowDirection(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult set_FlowDirection(TParams p, FlowDirection value) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult GetFlowDirection(TParams p, DependencyObject element) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult SetFlowDirection(TParams p, DependencyObject element, FlowDirection value) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult get_Margin(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult set_Margin(TParams p, Thickness value) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult get_HorizontalAlignment(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult set_HorizontalAlignment(TParams p, HorizontalAlignment value) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult get_VerticalAlignment(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult set_VerticalAlignment(TParams p, VerticalAlignment value) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult get_FocusVisualStyle(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult set_FocusVisualStyle(TParams p, Style value) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult get_Cursor(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult set_Cursor(TParams p, Cursor value) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult get_ForceCursor(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult set_ForceCursor(TParams p, Boolean value) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult BeginInit(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult EndInit(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult get_IsInitialized(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult get_IsLoaded(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult get_ToolTip(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult set_ToolTip(TParams p, Object value) {
        return new ScenarioResult(true);
    }
}
