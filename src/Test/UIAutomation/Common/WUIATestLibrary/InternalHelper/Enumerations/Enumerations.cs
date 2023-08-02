// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//******************************************************************* 
//* Purpose: 
//* Owner: Microsoft
//* Contributors:
//*******************************************************************/
using System;
using System.Diagnostics;
using System.Reflection;

namespace InternalHelper.Enumerations
{
    /// -----------------------------------------------------------------------
    /// <summary></summary>
    /// -----------------------------------------------------------------------
    public enum EventFired
    {
        /// -------------------------------------------------------------------
        /// <summary>Event is fired</summary>
        /// -------------------------------------------------------------------
        True,
        /// -------------------------------------------------------------------
        /// <summary>Event is not fired</summary>
        /// -------------------------------------------------------------------
        False,
        /// -------------------------------------------------------------------
        /// <summary>Can't determine if event is fired</summary>
        /// -------------------------------------------------------------------
        Undetermined,
    }

    /// -----------------------------------------------------------------------
    /// <summary></summary>
    /// -----------------------------------------------------------------------
    public enum TestType
    {
        /// <summary></summary>
        ApplicationControlTests,
        /// <summary></summary>
        ApplicationWindowTests,
        /// <summary></summary>
        AutomationElementTests,
        /// <summary></summary>
        ButtonControlTests,
        /// <summary></summary>
        CalendarControlTests,
        /// <summary></summary>
        CheckBoxControlTests,
        /// <summary></summary>
        ComboBoxControlTests,
        /// <summary></summary>
        CommandingTests,
        /// <summary></summary>
        DateTimePickerControlTests,
        /// <summary></summary>
        DialogControlTests,
        /// <summary></summary>
        DockTests,
        /// <summary></summary>
        DocumentControlTests,
        /// <summary></summary>
        DragDropTests,
        /// <summary></summary>
        EditControlTests,
        /// <summary></summary>
        ExpandCollapseTests,
        /// <summary></summary>
        FrameControlTests,
        /// <summary></summary>
        GridItemTests,
        /// <summary></summary>
        GridTests,
        /// <summary></summary>
        HeaderControlTests,
        /// <summary></summary>
        HeaderItemControlTests,
        /// <summary></summary>
        HierarchyItemTests,
        /// <summary></summary>
        HyperLinkControlTests,
        /// <summary></summary>
        HyperLinkListControlTests,
        /// <summary></summary>
        ImageControlTests,
        /// <summary></summary>
        InvokeTests,
        /// <summary></summary>
        IPAddressControlTests,
        /// <summary></summary>
        ItemContainerTests,
        /// <summary></summary>
        ListBoxControlTests,
        /// <summary></summary>
        ListItemControlTests,
        /// <summary></summary>
        ListViewControlTests,
        /// <summary></summary>
        MenuBarControlTests,
        /// <summary></summary>
        MenuControlTests,
        /// <summary></summary>
        MenuItemControlTests,
        /// <summary></summary>
        MultiMediaTests,
        /// <summary></summary>
        MultipleViewTests,
        /// <summary></summary>
        PasswordControlTests,
        /// <summary></summary>
        ProgressBarControlTests,
        /// <summary></summary>
        RadioButtonControlTests,
        /// <summary></summary>
        RangeValueTests,
        /// <summary></summary>
        RepresentsObjectTests,
        /// <summary></summary>
        ScrollBarControlTests,
        /// <summary></summary>
        ScrollTests,
        /// <summary></summary>
        ScrollItemTests,
        /// <summary></summary>
        SelectionItemTests,
        /// <summary></summary>
        SelectionTests,
        /// <summary></summary>
        SliderControlTests,
        /// <summary></summary>
        SpinnerControlTests,
        /// <summary></summary>
        SplitTests,
        /// <summary></summary>
        StatusBarControlTests,
        /// <summary></summary>
        TabControlTests,
        /// <summary></summary>
        TabItemControlTests,
        /// <summary></summary>
        TableItemTests,
        /// <summary></summary>
        TableTests,
        /// <summary></summary>
        TextTests,
        /// <summary></summary>
        TitleBarControlTests,
        /// <summary></summary>
        ToggleTests,
        /// <summary></summary>
        ToolBarControlTests,
        /// <summary></summary>
        TooltipControlTests,
        /// <summary></summary>
        TransformTests,
        /// <summary></summary>
        TreeViewControlTests,
        /// <summary></summary>
        TreeViewItemControlTests,
        /// <summary></summary>
        UnknownControlTests,
        /// <summary></summary>
        ValueTests,
        /// <summary></summary>
        VirtualizedItemTests,
        /// <summary></summary>
        VisualInformationTests,
        /// <summary></summary>
        WindowTests,
        /// <summary></summary>
        HoolieScenarioTests,
        /// <summary></summary>
        UISpyScenarioTests,
        /// <summary></summary>
        NarratorScenarioTests,
        /// <summary></summary>
        MenuScenarioTests,
        /// <summary></summary>
        MsaaScenarioTests,
        /// <summary></summary>
        TextScenarioTests,
        /// <summary></summary>
        ScreenReaderScenarioTests,
        /// <summary></summary>
        TopLevelEventsScenarioTests,
        /// <summary></summary>
        AvalonTextScenarioTests
    }

    /// -----------------------------------------------------------------------
    /// <summary>
    /// Type of pattern
    /// </summary>
    /// -----------------------------------------------------------------------
    public enum TypeOfPattern
    {
        /// <summary></summary>
        ApplicationWindow,
        /// <summary></summary>
        Commanding,
        /// <summary></summary>
        Dock,
        /// <summary></summary>
        DragDrop,
        /// <summary></summary>
        ExpandCollapse,
        /// <summary></summary>
        Grid,
        /// <summary></summary>
        GridItem,
        /// <summary></summary>
        HierarchyItem,
        /// <summary></summary>
        Invoke,
        /// <summary></summary>
        ItemContainer,
        /// <summary></summary>
        MultiMedia,
        /// <summary></summary>
        MultipleView,
        /// <summary></summary>
        NotInteresting,
        /// <summary></summary>
        RangeValue,
        /// <summary></summary>
        RepresentsObject,
        /// <summary></summary>
        Scroll,
        /// <summary></summary>
        ScrollItem,
        /// <summary></summary>
        Selection,
        /// <summary></summary>
        SelectionItem,
        /// <summary></summary>
        SynchronizedInput,
        /// <summary></summary>
        Split,
        /// <summary></summary>
        Table,
        /// <summary></summary>
        TableItem,
        /// <summary></summary>
        Text,
        /// <summary></summary>
        Toggle,
        /// <summary></summary>
        Transform,
        /// <summary></summary>
        Unknown,
        /// <summary></summary>
        Value,
        /// <summary></summary>
        VirtualizedItem,
        /// <summary></summary>
        VisualInformation,
        /// <summary></summary>
        Window,
    }

    /// -----------------------------------------------------------------------
    /// <summary>
    /// Type of control
    /// </summary>
    /// -----------------------------------------------------------------------
    public enum TypeOfControl
    {
        /// <summary></summary>
        ApplicationControl,
        /// <summary></summary>
        ButtonControl,
        /// <summary></summary>
        CalendarControl,
        /// <summary></summary>
        CheckBoxControl,
        /// <summary></summary>
        ComboBoxControl,
        /// <summary></summary>
        CustomControl,
        /// <summary></summary>
        DataItemControl,
        /// <summary></summary>
        DateTimePickerControl,
        /// <summary></summary>
        DialogControl,
        /// <summary></summary>
        DocumentControl,
        /// <summary></summary>
        EditControl,
        /// <summary></summary>
        FrameControl,
        /// <summary></summary>
        GroupControl,
        /// <summary></summary>
        HeaderControl,
        /// <summary></summary>
        HeaderItemControl,
        /// <summary></summary>
        HyperLinkControl,
        /// <summary></summary>
        HyperLinkListControl,
        /// <summary></summary>
        ImageControl,
        /// <summary></summary>
        IPAddressControl,
        /// <summary></summary>
        ListControl,
        /// <summary></summary>
        ListItemControl,
        /// <summary></summary>
        PaneControl,
        /// <summary></summary>
        MenuBarControl,
        /// <summary></summary>
        MenuControl,
        /// <summary></summary>
        MenuItemControl,
        /// <summary></summary>
        PasswordControl,
        /// <summary></summary>
        ProgressBarControl,
        /// <summary></summary>
        RadioButtonControl,
        /// <summary></summary>
        ScrollBarControl,
        /// <summary></summary>
        SeparatorControl,
        /// <summary></summary>
        SliderControl,
        /// <summary></summary>
        SplitButtonControl,
        /// <summary></summary>
        SpinnerControl,
        /// <summary></summary>
        StatusBarControl,
        /// <summary></summary>
        TabControl,
        /// <summary></summary>
        TableControl,
        /// <summary></summary>
        TabItemControl,
        /// <summary></summary>
        TextControl,
        /// <summary></summary>
        ThumbControl,
        /// <summary></summary>
        TitleBarControl,
        /// <summary></summary>
        ToolbarControl,
        /// <summary></summary>
        TooltipControl,
        /// <summary></summary>
        TreeControl,
        /// <summary></summary>
        TreeItemControl,
        /// <summary></summary>
        WindowControl,
        /// <summary></summary>
        UnknownControl,
    };

    /// -----------------------------------------------------------------------
    /// <summary></summary>
    /// -----------------------------------------------------------------------
    internal class IDSStrings
    {
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal const string IDS_NAMESPACE_UIVERIFY = "Microsoft.Test.WindowsUIAutomation";

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal const string IDS_NAMESPACE_PATTERN = "Tests.Patterns";

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal const string IDS_NAMESPACE_CONTROL = "Tests.Controls";

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal const string IDS_NAPESPACE_SCENARIO = "Tests.Scenarios";
    }
}
