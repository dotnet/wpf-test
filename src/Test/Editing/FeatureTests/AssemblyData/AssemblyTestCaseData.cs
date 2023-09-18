// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Assembly-level test case metadata


[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 3 $ $Source: //depot/winmain_oob/wap_rtm/windowstest/client/wcptests/uis/Text/BVT/AssemblyData/AssemblyTestCaseData.cs $")]

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.Collections;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Threading;
    using System.Xml;

    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Threading;

    using DragDropAPI;
    using DataTransfer;

    using Test.Uis.Data;
    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.TextEditing;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;

    #endregion

    /// <summary>Test case data for test cases in this assembly.</summary>
    [TestCaseDataTableClass]
    public static class AssemblyTestCaseData
    {
        /// <summary>Test case data for test cases in this assembly.</summary>
        [TestCaseDataTable]
        public static TestCaseData[] Data = new TestCaseData[] {
            new TestCaseData(typeof(AutoWordExpansionFormatting), "",
                new Dimension("FormattingCommand" , EditingCommandData.CharacterEditingValues),
                new Dimension("VerifyToggle", BooleanValues)),

            new TestCaseData(typeof(FocusUndoDelimiterTest), "",
                new Dimension ("LoseFocus", new object[] {"ControlOutOfFocus", "AppOutOfFocus", "MouseClickOnTestControl", "MouseClickOutOfTestControl"}),
                new Dimension ("TextControl", new object [] {"TextBox", "RichTextBox"}),
                new Dimension ("TextForFirstUndoUnit", new object[]{ "a", "a1", ""})),

            new TestCaseData(typeof(KeyboardUndoDelimiterTest), "",
                new Dimension ("StartText", new object [] {"", "a", "a3?"}),
                new Dimension("AcceptsReturn", new object[] {true, false}),
                new Dimension ("TextControl", new object [] {"TextBox", "RichTextBox"}),
                new Dimension("Delimiter", KeyboardEditingData.UndoBoundayValues)),

            new TestCaseData(typeof(AutoWordSelectionPropertyTest), "",
                new Dimension("UseGetValue" , BooleanValues),
                new Dimension("UseSetValue" , BooleanValues),
                new Dimension("AutoWordSelectionEnabled" , BooleanValues),
                new Dimension("EditableType", TextEditableType.Values)),

            new TestCaseData(typeof(TextRangeSerializationTestBVT), "",
                new Dimension("Test" , TextRangeSerializationTestBVT.GetAllRoundTripTests)),

            new TestCaseData(typeof(TextControlRecoverbilityTest), "",
                new Dimension("TextChange" , BooleanValues),
                new Dimension("EditableType" , TextEditableType.Values)),

            new TestCaseData(typeof(TextRangeChangedEventTest), "",
                new Dimension("Action" , TextRangeChangedEventTest.Actions),
                new Dimension("InitialDocument", new Object[] {0, 1, 2})),

            new TestCaseData(typeof(TextRangeChangedEventTest), "Pri=1",
                new Dimension("Action" , TextRangeChangedEventTest.Actions),
                new Dimension("SelectionData", TextSelectionData.Values)),

            new TestCaseData(typeof(HyperlinkCornerPositionTest), "",
                new Dimension("XamlSample", HyperlinkCornerPositionTest.XamlSamples)),

            new TestCaseData(typeof(BackgroundLayoutControls), "",
                new Dimension("SetStatus", BackgroundLayoutControls.ControlStatus),
                new Dimension("EditableType", new object[]{TextEditableType.GetByName("RichTextBox"),/*TextEditableType.GetByName("TextBox")*/})),

            new TestCaseData(typeof(BackgrounLayoutForSpellRendering), "",
                new Dimension("Action", new object [] {ActionsForBackgroundLayout.SpellCheck, ActionsForBackgroundLayout.MouseSelection,ActionsForBackgroundLayout.DragDrop}),
                new Dimension("EditableType", new object[]{TextEditableType.GetByName("RichTextBox"),TextEditableType.GetByName("TextBox")})),

            new TestCaseData(typeof(BackgroundLayoutKeyboardInput), "",
                new Dimension("KeyboardAction", BackgroundLayoutKeyboardInput.KeyboardActions),
                new Dimension("EditableType", new object[]{TextEditableType.GetByName("RichTextBox"),TextEditableType.GetByName("TextBox")})),

            new TestCaseData(typeof(CaretElementRender), "",
                new Dimension("EditableType",   new TextEditableType[] {TextEditableType.GetByName("TextBox"), TextEditableType.GetByName("RichTextBox")/*, TextEditableType.GetByName("PasswordBox")*/ })),

            new TestCaseData(typeof(CaretRegressionTesting), "",
                new Dimension("EditableType",   TextEditableType.PlatformTypes),
                new Dimension("TextSample",     new object[] { String.Empty, TextScript.Latin.Sample })),

            new TestCaseData(typeof(CaretSystemSettingsTest), "",
                new Dimension("EditableType", TextEditableType.PlatformTypes)),

            new TestCaseData(typeof(ClipboardAccessInPartialTrust), "Pri=0",
                new Dimension("EditableType",   TextEditableType.Values)),

            new TestCaseData(typeof(ClipboardLeakTestCase), "",
                new Dimension("SetDataBehavior",    GetEnumValues(typeof(SetBehavior))),
                new Dimension("SetDataFormat",      GetEnumValues(typeof(SetFormat))),
                new Dimension("GetDataFormat",      GetEnumValues(typeof(GetFormat)))),

            new TestCaseData(typeof(CombinatorialNavigationTest), "Pri=0.0",
                new Dimension("EditableType",   new TextEditableType[] {TextEditableType.GetByName("TextBox"), TextEditableType.GetByName("RichTextBox")}),
                new Dimension("StringData",     new StringData[] { StringData.Values[0] }),
                new Dimension("SelectionData",  TrivialSelections),
                new Dimension("EditingData",    KeyboardEditingData.Values),
                new Dimension("Wrap",           new object[] { true })),

            new TestCaseData(typeof(CombinatorialNavigationTest), "Pri=0.1",
                new Dimension("EditableType",   new TextEditableType[] {TextEditableType.GetByName("subclass:TextBoxSubClass"), TextEditableType.GetByName("subclass:RichTextBoxSubClass")}),
                new Dimension("StringData",     new StringData[] { StringData.Values[0] }),
                new Dimension("SelectionData",  TrivialSelections),
                new Dimension("EditingData",    KeyboardEditingData.Values),
                new Dimension("Wrap",           new object[] { true })),

            new TestCaseData(typeof(CombinatorialNavigationTest), "Pri=0.2",
                new Dimension("EditableType",   new TextEditableType[] {TextEditableType.GetByName("PasswordBox")}),
                new Dimension("StringData",     new StringData[] { StringData.Values[0] }),
                new Dimension("SelectionData",  TrivialSelections),
                new Dimension("EditingData",    KeyboardEditingData.Values),
                new Dimension("Wrap",           new object[] { false })),

            new TestCaseData(typeof(CombinatorialNavigationTest), "Pri=1.0",
                new Dimension("EditableType",   new TextEditableType[] {TextEditableType.GetByName("TextBox"), TextEditableType.GetByName("RichTextBox")}),
                new Dimension("StringData",     new StringData[] { StringData.Values[0] }),
                new Dimension("SelectionData",  TrivialSelections),
                new Dimension("EditingData",    KeyboardEditingData.Values),
                new Dimension("Wrap",           new object[] { false })),

            new TestCaseData(typeof(CombinatorialNavigationTest), "Pri=1.1",
                new Dimension("EditableType",   new TextEditableType[] {TextEditableType.GetByName("subclass:TextBoxSubClass"), TextEditableType.GetByName("subclass:RichTextBoxSubClass")}),
                new Dimension("StringData",     new StringData[] { StringData.Values[0] }),
                new Dimension("SelectionData",  TrivialSelections),
                new Dimension("EditingData",    KeyboardEditingData.Values),
                new Dimension("Wrap",           new object[] { false })),

            new TestCaseData(typeof(CombinatorialNavigationTest), "Pri=2.1.0",
                new Dimension("EditableType",   new TextEditableType[] {TextEditableType.GetByName("TextBox"), TextEditableType.GetByName("RichTextBox")}),
                new Dimension("StringData",     new StringData[] { StringData.Values[0] }),
                new Dimension("SelectionData",  TextSelectionData.Values),
                new Dimension("EditingData",    KeyboardEditingData.Values),
                new Dimension("PreferCommands", new object[] { true }),
                new Dimension("Wrap",           new object[] { true })),

            new TestCaseData(typeof(CombinatorialNavigationTest), "Pri=2.1.1",
                new Dimension("EditableType",   new TextEditableType[] {TextEditableType.GetByName("subclass:RichTextBoxSubClass")}),
                new Dimension("StringData",     new StringData[] { StringData.Values[0] }),
                new Dimension("SelectionData",  TextSelectionData.Values),
                new Dimension("EditingData",    KeyboardEditingData.Values),
                new Dimension("PreferCommands", new object[] { true }),
                new Dimension("Wrap",           new object[] { false })),

            new TestCaseData(typeof(CombinatorialNavigationTest), "Pri=2.1.2",
                new Dimension("EditableType",   new TextEditableType[] {TextEditableType.GetByName("PasswordBox")}),
                new Dimension("StringData",     new StringData[] { StringData.Values[0] }),
                new Dimension("SelectionData",  TextSelectionData.Values),
                new Dimension("EditingData",    KeyboardEditingData.Values),
                new Dimension("PreferCommands", new object[] { true }),
                new Dimension("Wrap",           new object[] { true })),

            new TestCaseData(typeof(CombinatorialNavigationTest), "Pri=2.2",
                new Dimension("EditableType",   new TextEditableType[] {TextEditableType.GetByName("TextBox"), TextEditableType.GetByName("RichTextBox")}),
                new Dimension("StringData",     new StringData[] { StringData.Values[0] }),
                new Dimension("SelectionData",  TextSelectionData.Values),
                new Dimension("EditingData",    KeyboardEditingData.Values),
                new Dimension("PreferCommands", new object[] { true }),
                new Dimension("Wrap",           new object[] { false })),


            new TestCaseData(typeof(CombinatorialNavigationTest), "Pri=3",
                new Dimension("EditableType",   TextEditableType.Values),
                new Dimension("StringData",     new StringData[] { StringData.Values[0] }),
                new Dimension("SelectionData",  TrivialSelections),
                new Dimension("EditingData",    KeyboardEditingData.Values),
                new Dimension("PreferCommands", new object[] { true }),
                new Dimension("Wrap",           BooleanValues)),


            new TestCaseData(typeof(ContentAlignmentTest), "",
                new Dimension("TestHorizontalAlignment", GetEnumValues(typeof(HorizontalAlignment))),
                new Dimension("TestVerticalAlignment", GetEnumValues(typeof(VerticalAlignment))),
                new Dimension("TestFlowDirection", GetEnumValues(typeof(FlowDirection))),
                new Dimension("EditableType",   new object[] {TextEditableType.GetByName("TextBox"),
                                                              TextEditableType.GetByName("subclass:TextBoxSubClass"),
                                                              TextEditableType.GetByName("PasswordBox")})),

            new TestCaseData(typeof(ContextMenuTest), "Pri=0", /*PartialTrust testing*/
                new Dimension("ContextMenuOpenWith", new object[]{"+{F10}", "{ALT}e", "RightClick"}),
                new Dimension("EditableType",   new TextEditableType[] {TextEditableType.GetByName("TextBox"), TextEditableType.GetByName("RichTextBox")})),

            new TestCaseData(typeof(ContextMenuTest), "Pri=1", /*FullTrust testing*/
                new Dimension("ContextMenuOpenWith", new object[]{"+{F10}", "RightClick"}),
                new Dimension("EditableType",   new TextEditableType[] {TextEditableType.GetByName("TextBox"), TextEditableType.GetByName("RichTextBox")})),

            new TestCaseData(typeof(CreateTextPointerFromPoint), "",
                new Dimension("SnapToText",  BooleanValues)),

            new TestCaseData(typeof(CursorInFlowDocument), "",
                new Dimension("TargetElement",        new object[] {"Run", "Paragraph", "Button", "FlowDocument"}),
                new Dimension("WithSelection",  BooleanValues)),

            new TestCaseData(typeof(CustomContextMenuTest), "",
                new Dimension("ContextMenuOpenedWith", new object[]{"+{F10}"}),
                new Dimension("EditableType",   TextEditableType.Values),
                new Dimension("InputChoiceSwitch", GetEnumValues(typeof(InputChoices)))),

            new TestCaseData(typeof(DataObjectFormatsTest), "",
                new Dimension("SetAPI", new object[]{"SetAudio", "SetFileDropList", "SetImage", "SetText"})),

            new TestCaseData(typeof(DataObjectLockDown), "",
                new Dimension("ContentData",    TransferContentData.Values),
                new Dimension("Flush",          BooleanValues),
                new Dimension("UseDragDrop",    BooleanValues)),

            new TestCaseData(typeof(DataTransferExtensibilityTest), "",
                new Dimension("AttachDirectlyOnControl",    BooleanValues),
                new Dimension("IsHandlerValid",             BooleanValues),
                new Dimension("IsTargetValid",              BooleanValues),
                new Dimension("EditableType",               TextEditableType.Values),
                new Dimension("Extensibility",              DataTransferExtensibilityData.Values),
                new Dimension("HandlerBehavior",            GetEnumValues(typeof(DataTransferBehavior))),
                new Dimension("RemoveHandler",              BooleanValues),
                new Dimension("UseDragDrop",                new object[] { false })),

            new TestCaseData(typeof(DataTransferScripts), "",
                new Dimension("EditableType",   TextEditableType.Values)),

            new TestCaseData(typeof(DeleteBackspaceHomeEnd), "Pri=1",
                new Dimension("EditableType",   TextEditableType.Values),
                new Dimension("KeyboardData",   KeyboardEditingData.DeleteBackSpaceHomeEndValues),
                new Dimension("SelectionStartIndex",   new object [] {0, 1, 2, 3, 4, 5, 6, 7}),
                new Dimension("SelectionLength",   new object [] {0, 1, 2})),

            new TestCaseData(typeof(DeleteOperationsOnBUIC), "",
                new Dimension("EditableType",   TextEditableType.Values),
                new Dimension("ContentVariationNumber", new object[]{0,1,2,3})),

            new TestCaseData(typeof(DeleteTextInRunTest), "",
                new Dimension("PointerOffset" , DeleteTextInRunTest.OffsetValues),
                new Dimension("DeletingCount", DeleteTextInRunTest.DeletingValues)),

            new TestCaseData(typeof(DragDropEvents), "Pri=3",
                new Dimension("ElementType",            new object[] {
                    typeof(TextBox), typeof(RichTextBox), typeof(System.Windows.Shapes.Rectangle) }),
                new Dimension("EventToTest",            GetEnumValues(typeof(DragDropEvents.DragDropEvent))),
                new Dimension("AddMethod",              GetEnumValues(typeof(DragDropEvents.EventSettingMethod))),
                new Dimension("RemoveMethod",           GetEnumValues(typeof(DragDropEvents.EventSettingMethod))),
                new Dimension("IsEventRemoved",         BooleanValues),
                new Dimension("IsEventStagePreview",    BooleanValues)),

            new TestCaseData(typeof(DragDropEvents), "Pri=4",
                new Dimension("ElementType",            new object[] {
                    typeof(Paragraph), typeof(Bold) }),
                new Dimension("EventToTest",            GetEnumValues(typeof(DragDropEvents.DragDropEvent))),
                new Dimension("AddMethod",              GetEnumValues(typeof(DragDropEvents.EventSettingMethod))),
                new Dimension("RemoveMethod",           GetEnumValues(typeof(DragDropEvents.EventSettingMethod))),
                new Dimension("IsEventRemoved",         BooleanValues),
                new Dimension("IsEventStagePreview",    BooleanValues)),

            new TestCaseData(typeof(DragDropEvents), "Pri=0",
                new Dimension("ElementType",            new object[] {typeof(TextBox), typeof(RichTextBox), typeof(Bold)}),
                new Dimension("EventToTest",            GetEnumValues(typeof(DragDropEvents.DragDropEvent))),
                new Dimension("AddMethod",              new object[]{DragDropEvents.EventSettingMethod.Accessor}),
                new Dimension("RemoveMethod",           new object[]{DragDropEvents.EventSettingMethod.Accessor}),
                new Dimension("IsEventRemoved",         new object[]{false}),
                new Dimension("IsEventStagePreview",    BooleanValues)),

            new TestCaseData(typeof(DragLeaveEventTest), "",
                new Dimension("EditableType", new TextEditableType[] {TextEditableType.GetByName("TextBox"), TextEditableType.GetByName("RichTextBox")})),

            new TestCaseData(typeof(EditCommandText), "Pri=1",
                new Dimension("EditableType", new TextEditableType[]{TextEditableType.GetByName("TextBox"), TextEditableType.GetByName("RichTextBox")})),

            new TestCaseData(typeof(EditingContainerTest), "Test=SingleLine_EditingValues_TB",
                new Dimension("Container",      EditingContainer.Values),
                new Dimension("AcceptsReturn",  new object[]{false}),
                new Dimension("KeyboardEditing",KeyboardEditingData.EditingValues),
                new Dimension("TextSelection",  new object[] { TextSelectionData.GetForValue(TextSelectionTestValue.EmptyDocumentStart) }),
                new Dimension("EditableType",   new object[]{TextEditableType.GetByName("TextBox")}),
                new Dimension("PreferCommands", new object[] { false }),
                new Dimension("StringData",     new StringData[] { StringData.LatinScriptData })),

            new TestCaseData(typeof(EditingContainerTest), "Test=SingleLine_EditingValues_RTB",
                new Dimension("Container",      EditingContainer.Values),
                new Dimension("AcceptsReturn",  new object[]{false}),
                new Dimension("KeyboardEditing",KeyboardEditingData.EditingValues),
                new Dimension("TextSelection",  new object[] { TextSelectionData.GetForValue(TextSelectionTestValue.EmptyDocumentStart) }),
                new Dimension("EditableType",   new object[]{TextEditableType.GetByName("RichTextBox")}),
                new Dimension("PreferCommands", new object[] { false }),
                new Dimension("StringData",     new StringData[] { StringData.LatinScriptData })),

            new TestCaseData(typeof(EditingContainerTest), "Test=SingleLine_EditingValues_PB",
                new Dimension("Container",      EditingContainer.Values),
                new Dimension("AcceptsReturn",  new object[]{false}),
                new Dimension("KeyboardEditing",KeyboardEditingData.EditingValues),
                new Dimension("TextSelection",  new object[] { TextSelectionData.GetForValue(TextSelectionTestValue.EmptyDocumentStart) }),
                new Dimension("EditableType",   new object[]{TextEditableType.GetByName("PasswordBox")}),
                new Dimension("PreferCommands", new object[] { false }),
                new Dimension("StringData",     new StringData[] { StringData.LatinScriptData })),

            new TestCaseData(typeof(EditingContainerTest), "Test=MultipleLine_EditingValues_TB",
                new Dimension("Container",      EditingContainer.Values),
                new Dimension("AcceptsReturn",  new object[]{true}),
                new Dimension("KeyboardEditing",KeyboardEditingData.EditingValues),
                new Dimension("TextSelection",  new object[] { TextSelectionData.GetForValue(TextSelectionTestValue.EmptyDocumentStart) }),
                new Dimension("EditableType",   new object[]{TextEditableType.GetByName("TextBox")}),
                new Dimension("PreferCommands", new object[] { false }),
                new Dimension("StringData",     new StringData[] { StringData.LatinScriptData })),

            new TestCaseData(typeof(EditingContainerTest), "Test=MultipleLine_EditingValues_RTB",
                new Dimension("Container",      EditingContainer.Values),
                new Dimension("AcceptsReturn",  new object[]{true}),
                new Dimension("KeyboardEditing",KeyboardEditingData.EditingValues),
                new Dimension("TextSelection",  new object[] { TextSelectionData.GetForValue(TextSelectionTestValue.EmptyDocumentStart) }),
                new Dimension("EditableType",   new object[]{TextEditableType.GetByName("RichTextBox")}),
                new Dimension("PreferCommands", new object[] { false }),
                new Dimension("StringData",     new StringData[] { StringData.LatinScriptData })),

            new TestCaseData(typeof(EditingContainerTest), "Test=SingleLine_NavigationValues_TB",
                new Dimension("Container",      EditingContainer.Values),
                new Dimension("AcceptsReturn",  new object[]{false}),
                new Dimension("KeyboardEditing",KeyboardEditingData.NavigationValues),
                new Dimension("TextSelection",  new object[] { TextSelectionData.GetForValue(TextSelectionTestValue.EmptyDocumentStart) }),
                new Dimension("EditableType",   new object[]{TextEditableType.GetByName("TextBox")}),
                new Dimension("PreferCommands", new object[] { false }),
                new Dimension("StringData",     new StringData[] { StringData.LatinScriptData })),

            new TestCaseData(typeof(EditingContainerTest), "Test=SingleLine_NavigationValues_RTB",
                new Dimension("Container",      EditingContainer.Values),
                new Dimension("AcceptsReturn",  new object[]{false}),
                new Dimension("KeyboardEditing",KeyboardEditingData.NavigationValues),
                new Dimension("TextSelection",  new object[] { TextSelectionData.GetForValue(TextSelectionTestValue.EmptyDocumentStart) }),
                new Dimension("EditableType",   new object[]{TextEditableType.GetByName("RichTextBox")}),
                new Dimension("PreferCommands", new object[] { false }),
                new Dimension("StringData",     new StringData[] { StringData.LatinScriptData })),

            new TestCaseData(typeof(EditingContainerTest), "Test=SingleLine_NavigationValues_PB",
                new Dimension("Container",      EditingContainer.Values),
                new Dimension("AcceptsReturn",  new object[]{false}),
                new Dimension("KeyboardEditing",KeyboardEditingData.NavigationValues),
                new Dimension("TextSelection",  new object[] { TextSelectionData.GetForValue(TextSelectionTestValue.EmptyDocumentStart) }),
                new Dimension("EditableType",   new object[]{TextEditableType.GetByName("PasswordBox")}),
                new Dimension("PreferCommands", new object[] { false }),
                new Dimension("StringData",     new StringData[] { StringData.LatinScriptData })),

            new TestCaseData(typeof(EditingContainerTest), "Test=MultipleLine_NavigationValues_TB",
                new Dimension("Container",      EditingContainer.Values),
                new Dimension("AcceptsReturn",  new object[]{true}),
                new Dimension("KeyboardEditing",KeyboardEditingData.NavigationValues),
                new Dimension("TextSelection",  new object[] { TextSelectionData.GetForValue(TextSelectionTestValue.EmptyDocumentStart) }),
                new Dimension("EditableType",   new object[]{TextEditableType.GetByName("TextBox")}),
                new Dimension("PreferCommands", new object[] { false }),
                new Dimension("StringData",     new StringData[] { StringData.LatinScriptData })),

            new TestCaseData(typeof(EditingContainerTest), "Test=MultipleLine_NavigationValues_RTB",
                new Dimension("Container",      EditingContainer.Values),
                new Dimension("AcceptsReturn",  new object[]{true}),
                new Dimension("KeyboardEditing",KeyboardEditingData.NavigationValues),
                new Dimension("TextSelection",  new object[] { TextSelectionData.GetForValue(TextSelectionTestValue.EmptyDocumentStart) }),
                new Dimension("EditableType",   new object[]{TextEditableType.GetByName("RichTextBox")}),
                new Dimension("PreferCommands", new object[] { false }),
                new Dimension("StringData",     new StringData[] { StringData.LatinScriptData })),

            new TestCaseData(typeof(EditingInList), "",
                new Dimension("ListData", ListEditingData.BVTcases)),

            new TestCaseData(typeof(EditingKeyBindingTest), "",
                new Dimension("EditableType",  TextEditableType.Values)),

            new TestCaseData(typeof(EditingWithLigatures), "",
                new Dimension("EditableType",  TextEditableType.PlatformTypes),
                new Dimension("InitialString", new object[]{"\x644\x627\x644\x627"})),

            new TestCaseData(typeof(EmbeddedObjectUndoRedo), "",
                new Dimension("Addition",               GetEnumValues(typeof(EmbeddedObjectUndoRedo.AdditionKind))),
                new Dimension("EmbeddedObjectType",     EmbeddedObjectUndoRedo.EmbeddedObjectTypes),
                new Dimension("EditingData",            KeyboardEditingData.Values),
                new Dimension("InBlockContainer",       BooleanValues)),

            new TestCaseData(typeof(EnterKeyInRichTextBoxTest), "Pri=1",
                new Dimension("Content",   new object[] {"\x2460\x7D05\x300E\x6BDB\x300F\x5834\xFF08\x5927\x2793\x3086\x304D\xFF09\xFF12\xFF11\x4E16\x3092\x4F1A\x28\xFF7B\xFF9D\xFF8E\xFF9B\x29"}),
                new Dimension("SelectionStartIndex", new object[] {0, 1, 3, 5, 8, 13, 25}),
                new Dimension("SelectionLength", new object[] {0, 1, 2}),
                new Dimension("KeyboardData", new object[]{KeyboardEditingData.GetValue(KeyboardEditingTestValue.Enter)}),
                new Dimension("EditableType",   TextEditableType.Values)),

            new TestCaseData(typeof(FDSwitchTest), "",
                new Dimension("TestWithMultipleParagraphs", BooleanValues),
                new Dimension("TestWithMultipleRuns",       BooleanValues),
                new Dimension("TestWithRTLContent",         BooleanValues)),

            new TestCaseData(typeof(FigureAndFloaterTest), "",
                new Dimension("EditableType",   new object[] {TextEditableType.GetByName("RichTextBox")}),
                new Dimension("FigureOrFloater", new object[]{"Figure", "Floater"})),

            new TestCaseData(typeof(FlowDocumentProperties), "",
                new Dimension("TestProperty",      DependencyPropertyData.FlowDocumentPropertyData),
                new Dimension("CopyOnlyText", BooleanValues)),

            new TestCaseData(typeof(HyperlinkCornerPositionTest), "",
                new Dimension("XamlSample", HyperlinkCornerPositionTest.XamlSamples)),

            new TestCaseData(typeof(HyperlinkTest), "",
                new Dimension("EditableType",   new object[] {TextEditableType.GetByName("RichTextBox")})),

            new TestCaseData(typeof(InlineCreationAtPosition), "",
                new Dimension("Offset" , InlineCreationAtPosition.Offsets),
                new Dimension("ElementType" , InlineCreationAtPosition.TextElements),
                new Dimension("Content", InlineCreationAtPosition.Contents)),

            new TestCaseData(typeof(InsertEditingCase), "",
                new Dimension("ToggleBackInsertion",    BooleanValues),
                new Dimension("EditableType",           TextEditableType.PlatformTypes),
                new Dimension("SelectionData",          TextSelectionData.Values),
                new Dimension("EditingData",            KeyboardEditingData.EditingValues)),

            new TestCaseData(typeof(InsertLineParagraphBreakTest), "",
                new Dimension("APITest" , new string[]{"InsertParagraphBreak", "InsertLineBreak"}),
                new Dimension("PointerOffset" , InsertLineParagraphBreakTest.PointerOffsets())),

            new TestCaseData(typeof(InvalidClipboardTest), "",
                new Dimension("TestFormat",    new object[] {DataFormats.Rtf, DataFormats.Xaml, DataFormats.XamlPackage}),
                new Dimension("InvalidData",   new object[] {string.Empty, "invalid data"})),

            new TestCaseData(typeof(ListFlowDirection), "",
                new Dimension("EditableType",   TextEditableType.PlatformTypes),
                new Dimension("NumberOfCharsSelected",   new object [] {0,1,2,3}),
                new Dimension("StartLevel",   new object [] {1,2}),
                new Dimension("ListVariationNumber",   new object [] { 0, 1, 2, 3})),

            new TestCaseData(typeof(MouseDoubleClickTest), "Pri=1",
                new Dimension("EditableType",   new object[] {TextEditableType.GetByName("RichTextBox"), TextEditableType.GetByName("TextBox")}),
                new Dimension("StartPosition",  DocumentPositionData.Values)),

            new TestCaseData(typeof(MouseDragSelectTest), "Pri=0",
                new Dimension("StartToEnd", new object[]{true}),
                new Dimension("EditableType", new object[]{TextEditableType.GetByName("RichTextBox"), TextEditableType.GetByName("TextBox")}),
                new Dimension("EndPosition", new object[]{
                    DocumentPositionData.GetForValue(DocumentPosition.EndOfDocument),
                    DocumentPositionData.GetForValue(DocumentPosition.StartOfWord),
                    DocumentPositionData.GetForValue(DocumentPosition.InsideWord),
                    DocumentPositionData.GetForValue(DocumentPosition.EndOfWord),
                    DocumentPositionData.GetForValue(DocumentPosition.EndOfWrappedLine)}),
                new Dimension("StartPosition", new object[]{
                    DocumentPositionData.GetForValue(DocumentPosition.StartOfDocument),
                    DocumentPositionData.GetForValue(DocumentPosition.StartOfWord),
                    DocumentPositionData.GetForValue(DocumentPosition.InsideWord),
                    DocumentPositionData.GetForValue(DocumentPosition.EndOfWord),
                    DocumentPositionData.GetForValue(DocumentPosition.StartOfDelimitedLine)})),

            new TestCaseData(typeof(MouseDragSelectTest), "Pri=1",
                new Dimension("StartToEnd", new object[]{true}),
                new Dimension("EditableType", new object[]{TextEditableType.GetByName("RichTextBox"),}),
                new Dimension("EndPosition", new object[]{
                    DocumentPositionData.GetForValue(DocumentPosition.StartOfCell),
                    DocumentPositionData.GetForValue(DocumentPosition.EndOfCell),
                    DocumentPositionData.GetForValue(DocumentPosition.CrossCell)}),
                new Dimension("StartPosition", new object[]{
                    DocumentPositionData.GetForValue(DocumentPosition.StartOfCell),
                    DocumentPositionData.GetForValue(DocumentPosition.EndOfCell),
                    DocumentPositionData.GetForValue(DocumentPosition.CrossCell)})),

            new TestCaseData(typeof(MouseDragSelectTest), "Pri=2",
                new Dimension("StartToEnd",     new object[]{true}),
                new Dimension("EditableType",   new object[]{TextEditableType.GetByName("RichTextBox"), TextEditableType.GetByName("TextBox")}),
                new Dimension("EndPosition", new object[]{
                    DocumentPositionData.GetForValue(DocumentPosition.StartOfDocument),
                    DocumentPositionData.GetForValue(DocumentPosition.EndOfDocument),
                    DocumentPositionData.GetForValue(DocumentPosition.StartOfDelimitedLine),
                    DocumentPositionData.GetForValue(DocumentPosition.EndOfDelimitedLine),
                    DocumentPositionData.GetForValue(DocumentPosition.InsideDelimitedLine),
                    DocumentPositionData.GetForValue(DocumentPosition.StartOfWord),
                    DocumentPositionData.GetForValue(DocumentPosition.EndOfWord),
                    DocumentPositionData.GetForValue(DocumentPosition.InsideWord),
                    DocumentPositionData.GetForValue(DocumentPosition.StartOfWhitespace),
                    DocumentPositionData.GetForValue(DocumentPosition.InsideWhitespace),
                    DocumentPositionData.GetForValue(DocumentPosition.EndOfWhitespace),
                    DocumentPositionData.GetForValue(DocumentPosition.StartOfWrappedLine),
                    DocumentPositionData.GetForValue(DocumentPosition.EndOfWrappedLine)}),
                new Dimension("StartPosition", new object[]{
                    DocumentPositionData.GetForValue(DocumentPosition.StartOfDocument),
                    DocumentPositionData.GetForValue(DocumentPosition.EndOfDocument),
                    DocumentPositionData.GetForValue(DocumentPosition.StartOfDelimitedLine),
                    DocumentPositionData.GetForValue(DocumentPosition.EndOfDelimitedLine),
                    DocumentPositionData.GetForValue(DocumentPosition.InsideDelimitedLine),
                    DocumentPositionData.GetForValue(DocumentPosition.StartOfWord),
                    DocumentPositionData.GetForValue(DocumentPosition.EndOfWord),
                    DocumentPositionData.GetForValue(DocumentPosition.InsideWord),
                    DocumentPositionData.GetForValue(DocumentPosition.StartOfWhitespace),
                    DocumentPositionData.GetForValue(DocumentPosition.InsideWhitespace),
                    DocumentPositionData.GetForValue(DocumentPosition.EndOfWhitespace),
                    DocumentPositionData.GetForValue(DocumentPosition.StartOfWrappedLine),
                    DocumentPositionData.GetForValue(DocumentPosition.EndOfWrappedLine)})),

            new TestCaseData(typeof(MouseDragToScroll), "Pri=1",
                new Dimension("EditableType",       new object[]{TextEditableType.GetByName("TextBox"), TextEditableType.GetByName("RichTextBox")}),
                new Dimension("ScrollDirection",    new object[]{"Left", "Right", "Top", "Down"}),
                new Dimension("IsEnoughData",       BooleanValues)),

            new TestCaseData(typeof(MouseSelectionTest), "",
                new Dimension("EditableType",   TextEditableType.Values)),

            new TestCaseData(typeof(MouseTripleClicksTest), "Pri=0",
                new Dimension("ContentData",     MouseTripleClicksTest._rtbContent),
                new Dimension("ClickPosition",   new object[]{"Front", "End", "OnWord"}),
                new Dimension("EditableType",   new object[] {TextEditableType.GetByName("RichTextBox")})),

            new TestCaseData(typeof(MouseTripleClicksTest), "Pri=1",
                new Dimension("ClickPosition",   new object[]{"Front", "End", "OnWord"}),
                new Dimension("EditableType",   new object[] {TextEditableType.GetByName("TextBox")})),

            new TestCaseData(typeof(MouseWheelScrollSelectionTest), "",
                new Dimension("EditableType",   new object[] {TextEditableType.GetByName("TextBox"), TextEditableType.GetByName("RichTextBox")})),

            new TestCaseData(typeof(PageUpDownNavigation), "Pri=0",
                new Dimension("Control",        TextEditableType.Values),
                new Dimension("PageAction",     KeyboardEditingData.PageNavigationValues),
                new Dimension("TotalLines",     new object [] {10}),
                new Dimension("CaretAtLine",    new object[] {1, 5, 9}),
                new Dimension("FlowDirection",  new object[] {System.Windows.FlowDirection.RightToLeft, System.Windows.FlowDirection.LeftToRight})),

            new TestCaseData(typeof(ParagraphUndoRedo), "Pri=1",
                new Dimension("EditableType",   TextEditableType.PlatformTypes)),

            new TestCaseData(typeof(ParagraphUndoRedo), "Pri=2",
                new Dimension("EditableType",   TextEditableType.Values)),

            new TestCaseData(typeof(PlainTextConverterElements), "",
                new Dimension("ElementType",    TextElementType.NonAbstractValues),
                new Dimension("ElementCount",   new object[] { 1, 3 }),
                new Dimension("TextSample",     new object[] { "", TextScript.Latin.Sample })),

            new TestCaseData(typeof(PlainTextConverterScripts), "",
                new Dimension("EditableType", TextEditableType.Values)),

            new TestCaseData(typeof(ResetFormatTest), "",
                new Dimension("CharFormatProperty" , DependencyPropertyData.GetCharacterFormattingProperties()),
                new Dimension("VerifyOnSpan", BooleanValues)),

            new TestCaseData(typeof(RichTextBoxBUICBackSpaceDelete), "",
                new Dimension("EditableType",      TextEditableType.Values),
                new Dimension("BuicOperationsSwitch", GetEnumValues(typeof(BUICOperations)))),

            new TestCaseData(typeof(RichTextBoxBUICNavigation), "",
                new Dimension("EditableType", TextEditableType.Values)),

            new TestCaseData(typeof(RichTextBoxFlowDocument), "",
                new Dimension("EditableType",      TextEditableType.Values),
                new Dimension("FlowDocOperationsSwitch", GetEnumValues(typeof(FlowDocOperations)))),

            new TestCaseData(typeof(RichTextBoxInlineFlowDirection), "",
                new Dimension("EditableType", TextEditableType.Values),
                new Dimension ("InlineChoicesForRTB", GetEnumValues(typeof(InlineTypes))),
                new Dimension ("SelectionSwitch", GetEnumValues(typeof(SelectionChoices)))),

            new TestCaseData(typeof(RichTextBoxInlineFlowDirectionUsingTextRange), "",
                new Dimension("EditableType", TextEditableType.Values),
                new Dimension ("InlineChoicesForRTB", GetEnumValues(typeof(InlineTypes))),
                new Dimension ("SelectionSwitch", GetEnumValues(typeof(SelectionChoices)))),

            new TestCaseData(typeof(RichTextBoxFlowDirectionSerialization), "",
               new Dimension("EditableType",  TextEditableType.Values),
               new Dimension ("FlowDirectionChoicesSwitch", GetEnumValues(typeof(FlowDirection))),
               new Dimension ("FlowDirectionInputChoicesSwitch", GetEnumValues(typeof(FlowDirectionInputChoices)))),

            new TestCaseData(typeof(RichTextBoxIncreaseDecreaseFontSize), "",
               new Dimension("EditableType", TextEditableType.Values),
               new Dimension ("ContentDataSwitch", GetEnumValues(typeof(ContentChoicesRTB)))),

            new TestCaseData(typeof(RichTextBoxTabIndentation), "",
                new Dimension("EditableType", TextEditableType.Values),
                new Dimension ("TabOperationsSwitch", GetEnumValues(typeof(TabOperations)))),

            new TestCaseData(typeof(RichTextCutCopyPasteInPartialTrust), "",
                new Dimension("TestCommand", new object[] {ApplicationCommands.Copy, ApplicationCommands.Cut})),

            new TestCaseData(typeof(RTBAcceptsTabTest), "",
                new Dimension("TestValue", BooleanValues),
                new Dimension("XamlContent", new object[] {"<Paragraph><Run/></Paragraph>",
                                                        "<Paragraph>Hello world hello world hello world hello world</Paragraph>",
                                                        "<Paragraph TextIndent='-10'>Hello world hello world hello world hello world</Paragraph>",
                                                        "<Paragraph TextIndent='10'>Hello world hello world hello world hello world</Paragraph>",
                                                        "<Paragraph TextIndent='30'>Hello world hello world hello world hello world</Paragraph>",
                                                        "<Paragraph Margin='10,0,0,0'>Hello world hello world hello world hello world</Paragraph>",
                                                        "<Paragraph TextIndent='10' Margin='20,0,0,0'>Hello world hello world hello world hello world</Paragraph>"}),
                new Dimension("TestSelectionNonEmpty", BooleanValues),
                new Dimension("DecreseIndentCommand", new object[]{"+{TAB}","{BACKSPACE}"})),


            new TestCaseData(typeof(ScrollingPageupDownLineUpDown), "",
                new Dimension("EditableType", TextEditableType.Values)),

            new TestCaseData(typeof(SpanCreationInRange), "",
                new Dimension("StartOffset" , SpanCreationInRange.Offsets),
                new Dimension("ElementType" , SpanCreationInRange.SpanElements),
                new Dimension("EndOffset", SpanCreationInRange.Offsets)),

            new TestCaseData(typeof(SpellerAPITest), "",
                new Dimension("EditableType", TextEditableType.Values)),

            new TestCaseData(typeof(SpellerEditorTest), "",
                new Dimension("ControlLanguage", SpellerEditorTest.XmlLanguages),
                new Dimension("EditableType", new TextEditableType[] {TextEditableType.GetByName("TextBox"), TextEditableType.GetByName("RichTextBox")})),

            new TestCaseData(typeof(SpellerFailureForNonSupportedLang), "",
                new Dimension("EditableType", TextEditableType.Values)),

            new TestCaseData(typeof(SpellingReformTest), "",
                new Dimension("spellingReform", GetEnumValues(typeof(SpellingReform))),
                new Dimension("EditableType",       new object[]{TextEditableType.GetByName("TextBox"), TextEditableType.GetByName("RichTextBox")})),

            new TestCaseData(typeof(SpellerWordBreakingTest), "",
                new Dimension("EditableType", new TextEditableType[] {TextEditableType.GetByName("TextBox"),
                    TextEditableType.GetByName("RichTextBox")})),

            new TestCaseData(typeof(SpellerWordSelectionTest), "",
                new Dimension("StringContent", SpellerWordSelectionTest.content),
                new Dimension("EditableType", TextEditableType.Values)),

            new TestCaseData(typeof(SpellerWordSelectionTest), "",
                new Dimension("StringContent", SpellerWordSelectionTest.content),
                new Dimension("EditableType", TextEditableType.Values)),

            new TestCaseData(typeof(StickyNoteEditingTest), "",
                new Dimension("IsSelectionUnderneath",  BooleanValues),
                new Dimension("EditingData",            KeyboardEditingData.Values)),

            new TestCaseData(typeof(TabKeyTest), "Pri=1",
                new Dimension("InitialContent", new object[] {"a{Enter}b{Enter}cde"/*use [a{Enter 2}cde] to repro Regression_Bug5*/}),
                new Dimension("AcceptsTab", BooleanValues),
                new Dimension("EditableType",   TextEditableType.Values),
                new Dimension("KeyboardData",KeyboardEditingData.GetValues(new KeyboardEditingTestValue[] {KeyboardEditingTestValue.Tab})),
                new Dimension("SelectionStartIndex",   new object [] {0, 1, 2, 3, 4, 5, 6, 7}),
                new Dimension("SelectionLength",   new object [] {0, 1, 2})),

            new TestCaseData(typeof(TableSelectionsTest), "Pri=0",
                new Dimension("EditingData",            KeyboardEditingData.NavigationValues),
                new Dimension("PreferCommands",         new object[] { true }),
                new Dimension("HasTrailingParagraph",   new object[] { true }),
                new Dimension("HasLeadingParagraph",    new object[] { true }),
                new Dimension("SampleTableStyle",       new object[] { TableSelectionsTest.TableStyle.FullTable}),
                new Dimension("SelectionData",          GetEnumValues(typeof(TableSelectionsTest.TableSelection)))),

            new TestCaseData(typeof(TableSelectionsTest), "EditingAction=EditingValues",
                new Dimension("EditingData",            KeyboardEditingData.EditingValues),
                new Dimension("PreferCommands",         new object[] { true }),
                new Dimension("HasTrailingParagraph",   BooleanValues),
                new Dimension("HasLeadingParagraph",    BooleanValues),
                new Dimension("SampleTableStyle",       GetEnumValues(typeof(TableSelectionsTest.TableStyle))),
                new Dimension("SelectionData",          GetEnumValues(typeof(TableSelectionsTest.TableSelection)))),

            new TestCaseData(typeof(TableSelectionsTest), "EditingAction=NavigationValues",
                new Dimension("EditingData",            KeyboardEditingData.NavigationValues),
                new Dimension("PreferCommands",         new object[] { true }),
                new Dimension("HasTrailingParagraph",   BooleanValues),
                new Dimension("HasLeadingParagraph",    BooleanValues),
                new Dimension("SampleTableStyle",       GetEnumValues(typeof(TableSelectionsTest.TableStyle))),
                new Dimension("SelectionData",          GetEnumValues(typeof(TableSelectionsTest.TableSelection)))),

            new TestCaseData(typeof(TabletSmokeTest), "",
                new Dimension("EditableType",   TextEditableType.Values),
                new Dimension("EditingMode",    GetEnumValues(typeof(System.Windows.Controls.InkCanvasEditingMode)))),

            new TestCaseData(typeof(TableNegativeTest), "",
                new Dimension("KeyboardAction" , TableNegativeTest.KeyValues),
                new Dimension("SelectionLength" , TableNegativeTest.SelectionValues),
                new Dimension("TableMetrix" , TableNegativeTest.TableValues)),

            new TestCaseData(typeof(TabOperationsOnList), "",
                new Dimension("EditableType",   new object[] {TextEditableType.GetByName("RichTextBox")}),
                new Dimension("ListVariationNumber", new object[]{0,1})),

            new TestCaseData(typeof(TestApplyTemplate), "",
                new Dimension("EditableType",   TextEditableType.PlatformTypes),
                new Dimension("ValidStyle",   BooleanValues),
                new Dimension("EmptyStyle",   BooleanValues)),

            new TestCaseData(typeof(TestCaretAtBoundaries), "AroundElementName=Table",
                new Dimension("AroundElementName", new object[]{"Table"}),
                new Dimension("IsSurroundedByPara", BooleanValues),
                new Dimension("IsCellWidthSet", BooleanValues)),

            new TestCaseData(typeof(TestCaretAtBoundaries), "AroundElementName=BUIC",
                new Dimension("AroundElementName", new object[]{"BUIC"}),
                new Dimension("IsSurroundedByPara", BooleanValues),
                new Dimension("IsCellWidthSet", new object[]{false})),

            new TestCaseData(typeof(TestCaretAtBoundaries), "AroundElementName=IUIC",
                new Dimension("AroundElementName", new object[]{"IUIC"}),
                new Dimension("IsSurroundedByPara", BooleanValues),
                new Dimension("IsCellWidthSet", new object[]{false})),

            new TestCaseData(typeof(TestCaretBiDi), "",
                new Dimension("EditableType",   TextEditableType.Values),
                new Dimension("TestInputLocale",    new object[] {InputLocaleData.ArabicSaudiArabia.Identifier,
                                                                   InputLocaleData.Hebrew.Identifier})),

            new TestCaseData(typeof(TestCaretSnapsToDevicePixels), "",
                new Dimension("EditableType",       TextEditableType.PlatformTypes),
                //Padding having odd decimal value previously caused bugs on SnapsToDevicePixels
                new Dimension("PaddingValue",       new object[] {2.0d, 3.5d}),
                new Dimension("SettingOnControl",   BooleanValues)),

            new TestCaseData(typeof(TestCaretVisibility), "",
                new Dimension("EditableType",   TextEditableType.PlatformTypes),
                new Dimension("TextSample",     new object[] { "  ", TextScript.Latin.Sample })),

            new TestCaseData(typeof(TestCaretWithContextMenu), "",
                new Dimension("EditableType", TextEditableType.PlatformTypes)),


            new TestCaseData(typeof(TestChangeBlocks), "",
                new Dimension("CommandCommentIndex", new object[] {1,2,3,4,5,6,7,8}),
                new Dimension("EditableType",  new object[] {TextEditableType.GetByName("RichTextBox"),
                                             TextEditableType.GetByName("subclass:RichTextBoxSubClass")})),


            new TestCaseData(typeof(TestClearValueForDataBoundValues), "",
                new Dimension("EditableType", TextEditableType.Values)),

            new TestCaseData(typeof(TestDataBoundingOnReadOnlyControls), "",
                new Dimension("EditableType", TextEditableType.Values)),

            new TestCaseData(typeof(TestForAppendText), "",
                new Dimension("EditableType", TextEditableType.Values),
                new Dimension ("ContentSwitch", GetEnumValues(typeof(ContentChoices))),
                new Dimension ("FormatSwitch", GetEnumValues(typeof(FormatChoices))),
                new Dimension ("AppendTextSwitch", GetEnumValues(typeof(AppendTextChoices)))),

            new TestCaseData(typeof(TestForIME), "",
                new Dimension("EditableType",   TextEditableType.Values)),

            new TestCaseData(typeof(TestForProgrammaticChangeInReadOnlyControl), "",
                new Dimension("EditableType", TextEditableType.Values),
                new Dimension ("OperationSwitch", GetEnumValues(typeof(OperationChoices)))),

            new TestCaseData(typeof(TestHyphenationEnabled), "",
                new Dimension("isHyphenationEnabled",   BooleanValues),
                new Dimension("isOptimalParagraphEnabled",    BooleanValues),
                new Dimension("SetPropertyOnControl",    BooleanValues),
                new Dimension("EditableType",   new object[] {TextEditableType.GetByName("RichTextBox"),
                                                              TextEditableType.GetByName("subclass:RichTextBoxSubClass"),})),

            new TestCaseData(typeof(TestTabsAreCorrectlyRendered), "",
                new Dimension("EditableType",        TextEditableType.Values),
                new Dimension("ControlFlowDirection",  new object[] {System.Windows.FlowDirection.RightToLeft, System.Windows.FlowDirection.LeftToRight})),

            new TestCaseData(typeof(TextBoxAlignment), "",
                new Dimension("HorizontalAlignment", new object[] {HorizontalAlignment.Left, HorizontalAlignment.Center, HorizontalAlignment.Right }),
                new Dimension("TextWrapping", new object[] {TextWrapping.Wrap, TextWrapping.NoWrap }),
                new Dimension("TextAlignment", new object[] {HorizontalAlignment.Left, HorizontalAlignment.Center, HorizontalAlignment.Right })),

            new TestCaseData(typeof(TextBoxBaseScrollFunctionsBoundaryTest), "",
                new Dimension("EditableType", TextEditableType.Values)),

            new TestCaseData(typeof(TextBoxBaseScrollFunctionsLineTest), "",
                new Dimension("EditableType", TextEditableType.Values),
                new Dimension("LargeMultiLineContent",  BooleanValues),
                new Dimension("WrapText",  BooleanValues),
                new Dimension ("_FunctionSwitch", GetEnumValues(typeof(FunctionName)))),

            new TestCaseData(typeof(TextBoxBaseScrollFunctionsPageTest), "",
                new Dimension("EditableType", TextEditableType.Values),
                new Dimension("LargeMultiLineContent",  BooleanValues),
                new Dimension("WrapText",  BooleanValues),
                new Dimension ("_FunctionSwitch", GetEnumValues(typeof(PageFunctionName)))),

            new TestCaseData(typeof(TextBoxBaseScrollFunctionTestOffsets), "",
                new Dimension("EditableType", TextEditableType.Values),
                new Dimension("LargeMultiLineContent",  BooleanValues),
                new Dimension("WrapText",  BooleanValues),
                new Dimension("OffsetSwitch", GetEnumValues(typeof(OffsetList)))),

            new TestCaseData(typeof(TextBoxBaseScrollFunctionTestViewportExtent), "",
                new Dimension("EditableType",           TextEditableType.Values),
                new Dimension("LargeMultiLineContent",  BooleanValues),
                new Dimension("WrapText",               BooleanValues),
                new Dimension("ViewportExtentSwitch",   GetEnumValues(typeof(ViewportExtentList)))),

            new TestCaseData(typeof(TextBoxBaseUndoApiTest), "",
                new Dimension("EditableType",           TextEditableType.Values),
                new Dimension("UndoRedoAction",         GetEnumValues(typeof(ProgramUndoRedoActions))),
                new Dimension("InitialIsUndoEnabled",   BooleanValues),
                new Dimension("InitialIsEmpty",         BooleanValues),
                new Dimension("FinalIsUndoEnabled",     BooleanValues),
                new Dimension("FinalIsEmpty",           BooleanValues),
                new Dimension("UseChangeBlock",         BooleanValues)),

            new TestCaseData(typeof(TextBoxClearValue), "",
                new Dimension("EditableType", TextEditableType.Values),
                new Dimension("SetSwitch", GetEnumValues(typeof(SetOptions)))),

            new TestCaseData(typeof(TextBoxContentPropsStable), "",
                new Dimension("EditableType",           TextEditableType.Values),
                new Dimension("PropertyValueSource",    GetEnumValues(typeof(PropertyValueSource)))),

            new TestCaseData(typeof(TextBoxGetFirstLastVisibleIndex), "",
                new Dimension("EditableType",  TextEditableType.Values),
                new Dimension("TextWrap",  BooleanValues),
                new Dimension("MultiLine",  BooleanValues)),

            new TestCaseData(typeof(TextBoxGetLineCharIndex), "",
                new Dimension("EditableType",           TextEditableType.Values),
                new Dimension("FlowDirectionProperty",   BooleanValues),
                new Dimension("InputStringSwitch",      GetEnumValues(typeof(InputStringDataChoices))),
                new Dimension("FunctionNameSwitch",      GetEnumValues(typeof(FunctionChoices)))),

            new TestCaseData(typeof(TextBoxGetLineLengthAndText), "",
                new Dimension("EditableType",  TextEditableType.Values),
                new Dimension("InputStringDataSwitch",    GetEnumValues(typeof(InputStringData)))),

            new TestCaseData(typeof(TextBoxGetRectFromCharIndex), "",
                new Dimension("EditableType",           TextEditableType.Values),
                new Dimension("LocaleSwitch",    GetEnumValues(typeof(LocaleChoices))),
                new Dimension("InputStringSwitch",    GetEnumValues(typeof(InputStringChoices)))),

            new TestCaseData(typeof(TextBoxGetRectFromCharIndexSurrogate), "",
                new Dimension("EditableType",           TextEditableType.Values)),

            new TestCaseData(typeof(TextBoxLineUpLineDownTest), "",
                new Dimension("EditableType", new object[]{TextEditableType.GetByName("TextBox")}),
                new Dimension("LargeMultiLineContent",  BooleanValues),
                new Dimension("InputSwitch",    GetEnumValues(typeof(InputTrigger))),
                new Dimension("FontSize",   new object [] {20, 40, 80}),
                new Dimension("FontFamily",   new object [] {"Times New Roman","Comic", "Tahoma","Verdana"})),

            new TestCaseData(typeof(TextBoxOMCutCopyPaste), "",
                new Dimension("EditableType",               TextEditableType.Values),
                new Dimension("SelectTextOptionsSwitch",    GetEnumValues(typeof(SelectTextOptions))),
                new Dimension("CutCopyPasteSwitch",         GetEnumValues(typeof(CutCopyPasteOperations)))),

#if TESTBUILD_NET_ATLEAST_462 // TextBoxCutCopyFailureExceptionTest is only relevant on .NET 4.6.2+
            new TestCaseData(typeof(TextBoxCutCopyFailureExceptionTest), ""),
#endif

            new TestCaseData(typeof(TextBoxNavigationText), "",
                new Dimension("EditableType",           TextEditableType.Values),
                new Dimension("IsInitiallyPopulated",   BooleanValues),
                new Dimension("IsContentModified",      BooleanValues),
                new Dimension("IsUserModified",         BooleanValues)),

            new TestCaseData(typeof(TextBoxScrollFunctionTest), "",
                new Dimension("EditableType", TextEditableType.Values),
                new Dimension("LargeMultiLineContent",  new object[] { true }),
                new Dimension("WrapText",  BooleanValues),
                new Dimension("TextBoxScrollSwitch",  GetEnumValues(typeof(TextBoxScrollFunction)))),

            new TestCaseData(typeof(TextBoxSequenceTest1), "",
                new Dimension("EditableType",           TextEditableType.Values)),

            new TestCaseData(typeof(TextBoxTriggers), "",
                new Dimension("InitialValue",               new string[] { "", TextScript.Thaana.Sample }),
                new Dimension("InteractiveChange",          BooleanValues)),

            new TestCaseData(typeof(TextBoxUpdateLayout), "",
                new Dimension("EditableType",   TextEditableType.PlatformTypes)),

            new TestCaseData(typeof(TextChangeEventKeyboardInput), "",
                new Dimension("EditableType", TextEditableType.Values),
                new Dimension("KeyboardInputSwitch", GetEnumValues(typeof(KeyboardInputs)))),

            new TestCaseData(typeof(TextChangeEventTestLoop), "",
                new Dimension("EditableType", TextEditableType.Values)),

            new TestCaseData(typeof(TextChangeEventKeyboardSelectionInputs), "",
                new Dimension("EditableType", TextEditableType.Values),
                new Dimension("TextChangeTextManipulationSwitch", GetEnumValues(typeof(TextChangeTextManipulationList))),
                new Dimension ("TextChangeKeyBoardDataSwitch", GetEnumValues(typeof(TextChangeKeyBoardDataList)))),

            new TestCaseData(typeof(TextChangeTestSetAndClear), "",
                new Dimension("EditableType", TextEditableType.Values),
                new Dimension("TextChangeSetAndClearTrigger", GetEnumValues(typeof(TextChangeSetAndClearTriggerList)))),

            new TestCaseData(typeof(TextEditorEventTesting), "",
                new Dimension("EditableType", TextEditableType.PlatformTypes)),

            new TestCaseData(typeof(TextEditorKeyboardEditing), "Pri=0",
                new Dimension("AcceptsReturn",  BooleanValues),
                new Dimension("KeyboardEditing",KeyboardEditingData.EditingValues),
                new Dimension("TextSelection",  new object[] { TextSelectionData.GetForValue(TextSelectionTestValue.SpanAllText) }),
                //new Dimension("EditableType", TextEditableType.PlatformTypes),
                new Dimension("EditableType",   new object[]{TextEditableType.GetByName("TextBox"), TextEditableType.GetByName("RichTextBox")}),
                new Dimension("StringData",     new StringData[] { StringData.LatinScriptData })),

            new TestCaseData(typeof(TextEditorKeyboardEditing), "Pri=1",
                new Dimension("AcceptsReturn",      BooleanValues),
                new Dimension("KeyboardEditing",    KeyboardEditingData.EditingValues),
                new Dimension("TextSelection",      TextSelectionData.Values),
                new Dimension("EditableType",       TextEditableType.PlatformTypes),
                new Dimension("PreferCommands",     new object[] { true }),
                new Dimension("StringData",         new StringData[] { StringData.LatinScriptData })),

            new TestCaseData(typeof(TextEditorKeyboardEditing), "Pri=2",
                new Dimension("AcceptsReturn",      BooleanValues),
                new Dimension("KeyboardEditing",    KeyboardEditingData.EditingValues),
                new Dimension("TextSelection",      TextSelectionData.Values),
                new Dimension("EditableType",       TextEditableType.Values),
                new Dimension("PreferCommands",     new object[] { true }),
                new Dimension("StringData",         new StringData[] { StringData.Empty, StringData.LatinScriptData })),

            new TestCaseData(typeof(TextPointerIsAtLineStart), "",
                new Dimension("TestWrap", BooleanValues),
                new Dimension("TestFixedFont", BooleanValues),
                new Dimension("EditableType", new object[]{TextEditableType.GetByName("TextBox")}),
                new Dimension("TestStringData", StringData.Values)),

            new TestCaseData(typeof(TextRangeSaveLoadTest), "",
                //new Dimension("ContentData",      RichTextContentData.Values),
                new Dimension("ContentData",      new object[]{RichTextContentData.FullyPopulatedContent}),
                new Dimension("TestDataFormat",       DataFormatsData.TRSupportedValues)),

            new TestCaseData(typeof(TextRangeSourceSerialization), "",
                new Dimension("PropertyData", DependencyPropertyData.GetForTextSerialization(new RichTextBox()))),

            new TestCaseData(typeof(TextSelectionRenderingTest), "Pri=0",
                new Dimension("SelectionStartIndex",      new object[] {0, 1, 2, 3}),
                new Dimension("SelectionLength",    new object[] { 1, 2}),
                new Dimension("HightlightColor",      new object[] {Brushes.Blue.Color, Brushes.Red.Color}),
                //new Dimension("TextColor",     new object []  { Brushes.Black.Color, Brushes.Brown.Color }),
                //new Dimension("TextContent",     new object []  {"a\r\nb", "\x05d0\x05d1 \x05ea\x05e9 english" }),
                //new Dimension("Backgrouind",     new object []  {Brushes.gray, Radiant, ImageBrush}),
                //new Dimension("Forground",     new object []  {Brushes.gray, Radiant, ImageBrush}),
                new Dimension("EditableType",       TextEditableType.Values)),

            new TestCaseData(typeof(TextSelectionSelectTest), "",
                new Dimension("TextSample", new object[] {StringData.LatinScriptData.Value, StringData.CombiningCharacters.Value, StringData.SurrogatePair.Value}),
                new Dimension("WithFocus", BooleanValues)),

            new TestCaseData(typeof(ToolTipTypingTest), "",
                new Dimension("ToolTipTypeSwitch", GetEnumValues(typeof(ToolTipType))),
                new Dimension("TooltipTypingScenariosSwitch", GetEnumValues(typeof(TooltipTypingScenarios))),
                new Dimension("EditableType",   TextEditableType.Values)),

            new TestCaseData(typeof(TRApplyPropertyValueTest), "",
                new Dimension("FormattingDPData", DependencyPropertyData.GetFormattingProperties()),
                new Dimension("IsSelectionEmpty", BooleanValues),
                new Dimension("XamlContent", new string[]{"<Bold>Bold</Bold>Plain<Italic>Italic</Italic>Plain<Underline>Underline</Underline>Plain",
                                                            "Plain content",
                                                            "             ",
                                                            string.Empty})),

            new TestCaseData(typeof(TypingInDifferentLocales), "Pri=0",
                new Dimension("EditableType", new TextEditableType[]{TextEditableType.GetByName("TextBox"), TextEditableType.GetByName("RichTextBox")}),
                new Dimension("LocaleData", InputLocaleData.Values),
                new Dimension("TestFontSize", new object[]{(double)9, (double)9.5, (double)10})),

            new TestCaseData(typeof(TypingInDifferentLocales), "Pri=1",
                new Dimension("EditableType", new TextEditableType[]{TextEditableType.GetByName("TextBox"), TextEditableType.GetByName("RichTextBox")}),
                new Dimension("LocaleData", InputLocaleData.Values),
                new Dimension("TestFontSize", new object[]{(double)32, (double)76})),

            new TestCaseData(typeof(UIElementFlowDirectionAlignment), "",
                new Dimension("EditableType", TextEditableType.Values),
                new Dimension("FlowDirectionInputsSwitch", GetEnumValues(typeof(FlowDirectionInputs)))),

            new TestCaseData(typeof(UndoRedoTypingTest), "",
                new Dimension("EditableType", TextEditableType.Values),
                new Dimension("EditingData", Add(KeyboardEditingData.UndoTypingValues, null)),
                new Dimension("UndoStackCount", new object[] { 0, 1, 2 }),
                new Dimension("RedoStackCount", new object[] { 0, 1 }),
                new Dimension("OperationCount", new object[] { 1, 2 })),

            new TestCaseData(typeof(XamlPackageBVTTest), "",
                new Dimension("PastedFormat", DataTransfer.XamlPackageTestData.PasteFormats),
                new Dimension("ContentOrder", DataTransfer.XamlPackageTestData.XamlCombinations),
                new Dimension("InmageFormat", DataTransfer.XamlPackageTestData.ImageFiles)),
        };


        #region Public properties.

        /// <summary>Array with static boolean values.</summary>
        public static object[] BooleanValues
        {
            get
            {
                if (s_booleanValues == null)
                {
                    s_booleanValues = new object[] { true, false };
                }
                return s_booleanValues;
            }
        }

        /// <summary>Array with static boolean values.</summary>
        public static TextSelectionData[] TrivialSelections
        {
            get
            {
                if (s_trivialSelections == null)
                {
                    s_trivialSelections = new TextSelectionData[] {
                        TextSelectionData.GetForValue(TextSelectionTestValue.SpanAllText),
                        TextSelectionData.GetForValue(TextSelectionTestValue.EmptyOnPopulatedText),
                        TextSelectionData.GetForValue(TextSelectionTestValue.EmptyDocumentEnd),
                    };
                }
                return s_trivialSelections;
            }
        }

        #endregion Public properties.


        #region Private methods.

        /// <summary>Adds an object to the specified array.</summary>
        private static object[] Add(object[] array, object objectToAdd)
        {
            object[] result;

            result = new object[array.Length + 1];
            array.CopyTo(result, 0);
            result[array.Length] = objectToAdd;

            return result;
        }

        /// <summary>
        /// Returns an object array with all values defined in the
        /// specified enumeration.
        /// </summary>
        /// <param name='enumType'>Enumeration type to return values for.</param>
        /// <returns>
        /// An object array with all values defined in the enumeration.
        /// </returns>
        private static object[] GetEnumValues(Type enumType)
        {
            object[] result;
            Array values;

            if (enumType == null)
            {
                throw new ArgumentNullException("enumType");
            }

            // Simple typecasting from System.Array to System.Object[] fails.
            values = System.Enum.GetValues(enumType);
            result = new object[values.Length];
            values.CopyTo(result, 0);
            return result;
        }

        #endregion Private methods.


        #region Private fields.

        /// <summary>Array with static boolean values.</summary>
        private static object[] s_booleanValues;

        /// <summary>Array with the most basic selections.</summary>
        private static TextSelectionData[] s_trivialSelections;

        #endregion Private fields.
    }
}