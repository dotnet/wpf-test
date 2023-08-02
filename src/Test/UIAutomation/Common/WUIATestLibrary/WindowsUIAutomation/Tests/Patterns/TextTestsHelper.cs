// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
* Purpose: InternalHelper
* Owner: Microsoft
* Contributors:
*******************************************************************/
#define CODE_ANALYSIS  // Required for FxCop suppression attributes

using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Text;
using System.Text;
using System.Collections;
using System.CodeDom;
using ATGTestInput;
using System.Diagnostics.CodeAnalysis; // Required for FxCop suppression attributes

namespace InternalHelper.Tests.Patterns
{
    using InternalHelper;
    using InternalHelper.Tests;
    using InternalHelper.Enumerations;
    using Microsoft.Test.WindowsUIAutomation;
    using Microsoft.Test.WindowsUIAutomation.Core;
    using Microsoft.Test.WindowsUIAutomation.TestManager;
    using Microsoft.Test.WindowsUIAutomation.Interfaces;
    using Microsoft.Test.WindowsUIAutomation.Tests.Scenarios;

    #region TextTestsHelper Class

    /// -----------------------------------------------------------------------
    /// <summary></summary>
    /// -----------------------------------------------------------------------
    public class TextTestsHelper : TextWrapper
    {
        #region attributes class definition

        //----------------------------------------------------------------------------
        // Class used to describe the "attributes" of a given AutomationTextAttribute
        //----------------------------------------------------------------------------
        private class AttributeMadness
        {
            private bool _isEditControl;
            private bool _isRichEditControl;
            private bool _isEnum;

            //---------------------------------------------            
            // Constructor for initialize attributes
            //---------------------------------------------            
            public AttributeMadness(bool isEditControl, bool isRichEditControl, bool isEnum)
            {
                _isEditControl = isEditControl;
                _isRichEditControl = isRichEditControl;
                _isEnum = isEnum;
            }

            //------------
            // Properties 
            //------------
            public bool isEditControl { get { return _isEditControl; } }
            public bool isRichEditControl { get { return _isRichEditControl; } }
            public bool isEnum { get { return _isEnum; } }
            //            public Type type { get { return _type; } }
        }

        #endregion attributes class definition

        #region Member Variables

        const int randomBlockSize            = 16;      // Size of random block to be duplicated
        const int randomBlockSizeOffset      = 22;      // Size of random block plus distinguishing "<<<" and ">>>"
        const int recurseLevelMax            = 32;      // Recursion upper limit for # of generations of child automation elements
        const int editControlAttribCount     = 11;      // # of attributes for Win32 Edit control
        const int richEditControlAttribCount = 21;      // # of attributes for Win32 RichEdit control
        const int enumAttributesCount        = 12;      // # of attributes that are enums
        const int attributesCount            = 43;      // # of possible AutomationTextAttributes
        internal Hashtable attributeMadness;            // Hash table for details/behavior of AutomationTextAttributes

        internal const string noText        = "";
        internal const string textIsOneChar = "X";
        internal const string string1       = "String 1";
        internal const string easyText      = "String 1 String 2 String 3 String 4 String 5";

        static internal SampleText _sampleText   = SampleText.Unused;   // Default state
        static internal string     _actualText   = "";      // Actual text in contorl, could have trailing \r, \r\n or \n on RichEdit
        static internal string     _originalText = "";      // text we requested (regardless of whats in RichEdit)
        static internal int        _trailingCRLFCount = 0;  // Count of \r, \r\n or \n that RichEdit added
        static internal int[]      _runtimeID    = new int[0]; // Used to identify if we're on some runtime id as previously
        
        #endregion Member Variables

        #region TextTestsHelper constructor

        ///---------------------------------------------------------------------------
        /// <summary></summary>
        ///---------------------------------------------------------------------------
        public TextTestsHelper(AutomationElement element, string testSuite, TestPriorities priority, TypeOfControl typeOfControl, TypeOfPattern typeOfPattern, string dirResults, bool testEvents, IApplicationCommands commands)
            :
            base(element, testSuite, priority, typeOfControl, typeOfPattern, dirResults, testEvents, commands)
        {
            // Determine what type of provider is being used (useful for things like supported text units, attributes, etc.)
            string FrameworkId = (string)element.GetCurrentPropertyValue(AutomationElement.FrameworkIdProperty);

            Comment("Type of Provider for this target application is : " + FrameworkId);

            TextLibrary.SetProvider(FrameworkId);
            TextLibraryCount.SetProvider(FrameworkId);

            // Instantiate attribute madness table
            attributeMadness = new Hashtable(attributesCount);

            // Defines expected support for attributes.
            attributeMadness.Add(TextPattern.AnimationStyleAttribute, new AttributeMadness(false, true, true));
            attributeMadness.Add(TextPattern.BackgroundColorAttribute, new AttributeMadness(true, true, false));
            attributeMadness.Add(TextPattern.BulletStyleAttribute, new AttributeMadness(false, true, true));
            attributeMadness.Add(TextPattern.CapStyleAttribute, new AttributeMadness(true, true, true));
            attributeMadness.Add(TextPattern.CultureAttribute, new AttributeMadness(false, false, false));
            attributeMadness.Add(TextPattern.FontNameAttribute, new AttributeMadness(true, true, false));
            attributeMadness.Add(TextPattern.FontSizeAttribute, new AttributeMadness(true, true, false));
            attributeMadness.Add(TextPattern.FontWeightAttribute, new AttributeMadness(true, true, false));
            attributeMadness.Add(TextPattern.ForegroundColorAttribute, new AttributeMadness(true, true, false));
            attributeMadness.Add(TextPattern.HorizontalTextAlignmentAttribute, new AttributeMadness(true, true, true));
            attributeMadness.Add(TextPattern.IndentationFirstLineAttribute, new AttributeMadness(false, true, false));
            attributeMadness.Add(TextPattern.IndentationLeadingAttribute, new AttributeMadness(false, true, false));
            attributeMadness.Add(TextPattern.IndentationTrailingAttribute, new AttributeMadness(false, true, false));
            attributeMadness.Add(TextPattern.IsHiddenAttribute, new AttributeMadness(false, true, false));
            attributeMadness.Add(TextPattern.IsItalicAttribute, new AttributeMadness(true, true, false));
            attributeMadness.Add(TextPattern.IsReadOnlyAttribute, new AttributeMadness(true, true, false));
            attributeMadness.Add(TextPattern.IsSubscriptAttribute, new AttributeMadness(false, true, false));
            attributeMadness.Add(TextPattern.IsSuperscriptAttribute, new AttributeMadness(false, true, false));
            attributeMadness.Add(TextPattern.MarginBottomAttribute, new AttributeMadness(false, false, false));
            attributeMadness.Add(TextPattern.MarginLeadingAttribute, new AttributeMadness(false, false, false));
            attributeMadness.Add(TextPattern.MarginTopAttribute, new AttributeMadness(false, false, false));
            attributeMadness.Add(TextPattern.MarginTrailingAttribute, new AttributeMadness(false, false, false));
            attributeMadness.Add(TextPattern.OutlineStylesAttribute, new AttributeMadness(false, true, false));
            attributeMadness.Add(TextPattern.OverlineColorAttribute, new AttributeMadness(false, false, false));
            attributeMadness.Add(TextPattern.OverlineStyleAttribute, new AttributeMadness(false, false, false));
            attributeMadness.Add(TextPattern.StrikethroughColorAttribute, new AttributeMadness(false, false, false));
            attributeMadness.Add(TextPattern.StrikethroughStyleAttribute, new AttributeMadness(true, true, false));
            attributeMadness.Add(TextPattern.TabsAttribute, new AttributeMadness(false, true, false));
            attributeMadness.Add(TextPattern.TextFlowDirectionsAttribute, new AttributeMadness(false, false, false));
            attributeMadness.Add(TextPattern.UnderlineColorAttribute, new AttributeMadness(false, false, false));
            attributeMadness.Add(TextPattern.UnderlineStyleAttribute, new AttributeMadness(true, true, false));
        }

        #endregion TextTestsHelper constructor

        #region Helper

        #region TextPattern Helpers

        #region TextPattern Property Helpers

        #region 1.1 TextPattern.DocumentRange Property Helpers
        //---------------------------------------------------------------------------
        // Helper for DocumentRange test cases
        //---------------------------------------------------------------------------
        internal void DocumentRangeHelper(SampleText sampleText)
        {
            string actualText = "";
            TextPatternRange range = null;

            // Pre-Condition Verify/Set text in control is as expected
            TS_SetText(sampleText, out actualText, CheckType.IncorrectElementConfiguration);

            // Call DocumentRange
            TS_DocumentRange(ref range, CheckType.Verification);
        }


        #endregion 1.1 TextPattern.DocumentRange Property Helpers

        #region 1.2 TextPattern.SupportedTextSelection Property Helpers

        //---------------------------------------------------------------------------
        // Helper for SupportedTextSelection test cases
        //---------------------------------------------------------------------------
        internal void SupportedTextSelectionHelper(SampleText sampleText)
        {
            SupportedTextSelection supportedTextSelection = SupportedTextSelection.None;

            // Get SupportedTextSelection Property Helpers
            TS_SupportedTextSelection(ref supportedTextSelection);

            // Validate actual result against known controls
            TS_VerifySupportedTextSelection(supportedTextSelection, CheckType.Verification);

        }

        #endregion TextPattern.SupportedTextSelection Property Helpers

        #endregion TextPattern Property Helpers

        #region TextPattern Method Helpers

        #region 2.1 TextPattern.RangeFromPoint Method Helpers

        //---------------------------------------------------------------------------
        // Helper for RangeFromPoint() test cases
        //---------------------------------------------------------------------------
        internal void RangeFromPointHelper(SampleText sampleText, int expandBy, TextPatternRangeEndpoint endPoint, RangeLocation rangeLocation, 
            BoundingRectangleLocation pointLocation, RangeCompare rangeCompare, Type expectedException)
        {
            Point screenLocation;
            Rect[] boundRects = new Rect[0];
            string actualText = "";
            TextPatternRange callingRange = null;
            TextPatternRange rangeFromPt = null;

            // Pre-Condition Verify text is expected value <<sampleText>>
            TS_SetText(sampleText, out actualText, CheckType.IncorrectElementConfiguration);

            // Pre-Condition Create empty calling range @ <<RangeLocation>> of TextPattern.DocumentRange
            TS_CreateEmptyRange(out callingRange, rangeLocation, CheckType.Verification);

            // Pre-Condition Expand <<endPoint>> endPoint by <<expandby>> character(s)
            TS_ExpandRange(ref callingRange, endPoint, TextUnit.Character, expandBy, true, CheckType.Verification);

            // Pre-Condition Scroll range into view
            // 


            Range_ScrollIntoView(callingRange, false, null, CheckType.Verification);
            TS_ScrollIntoView(callingRange, true, null, CheckType.Verification);

            // Pre-Condition Call GetBoundingRectangles() on that range
            TS_GetBoundingRectangles(callingRange, ref boundRects, false, null, CheckType.Verification);

            // Pre-Condition Create a point <<pointLocation>> of bounding rect
            TS_CreatePoint(ref callingRange, boundRects, out screenLocation, pointLocation, CheckType.Verification);

            // Call RangeFromPoint on point 
            TS_RangeFromPoint(ref rangeFromPt, screenLocation, expectedException, CheckType.Verification);

            // Verify that range returned is  <<rangeCompare>>
            TS_CompareRanges(callingRange, rangeFromPt, rangeCompare, expectedException, CheckType.Verification);
        }

        //---------------------------------------------------------------------------
        // Helper for RangeFromPoint() test cases
        //---------------------------------------------------------------------------
        internal void RangeFromPointHelper2(SampleText sampleText, bool isMultiLineExpected, TargetRangeType rangeType, BoundingRectangleLocation pointLocation, RangeCompare rangeCompare)
        {
            Point screenLocation;
            Rect[] boundRects = new Rect[0];
            TextPatternRange callingRange = null;
            TextPatternRange rangeFromPt = null;

            // Pre-Condition Control must supports multi-line = <<expectedResult>>
            TS_IsMultiLine(isMultiLineExpected, CheckType.IncorrectElementConfiguration);

            // Pre-Condition Verify text is expected value <<sampleText>>
            TS_SetText(sampleText, CheckType.IncorrectElementConfiguration);

            // Pre-Condition Create range equal to <<rangeType>>
            TS_CreateRange(out callingRange, rangeType, null, false, CheckType.IncorrectElementConfiguration);

            // Pre-Condition Scroll range into view
            // 


            Range_ScrollIntoView(callingRange, false, null, CheckType.Verification);
            TS_ScrollIntoView(callingRange, true, null, CheckType.Verification);

            // Pre-Condition Call GetBoundingRectangles() on that range
            TS_GetBoundingRectangles(callingRange, ref boundRects, false, null, CheckType.Verification);

            // Pre-Condition Create a point <<pointLocation>> of bounding rect
            TS_CreatePoint(ref callingRange, boundRects, out screenLocation, pointLocation, CheckType.Verification);

            // Call RangeFromPoint on point without errors
            TS_RangeFromPoint(ref rangeFromPt, screenLocation, null, CheckType.Verification);

            // Verify that range returned is <<rangeCompare>>
            TS_CompareRanges(callingRange, rangeFromPt, rangeCompare, null, CheckType.Verification);
        }

        #endregion 2.1 TextPattern.RangeFromPoint Method Helpers

        #region 2.2 TextPattern.GetSelection Method Helpers

        //---------------------------------------------------------------------------
        // Helper for GetSelection() test cases
        //---------------------------------------------------------------------------
        internal void GetSelectionHelper(SampleText sampleText, SupportedTextSelection supportedTextSelection, TargetRangeType rangeType, bool expectMatch, uint arrayCount, Type expectedException)
        {
            bool               isEqual       = false;
            string             actualText    = "";
            TextPatternRange   callingRange  = null;
            TextPatternRange[] selectedRange = null;

            // Pre-Condition Verify SupportedTextSelection=<<supportedTextSelection>>
            TS_VerifySupportedTextSelection(supportedTextSelection, CheckType.IncorrectElementConfiguration);

            // Pre-Condition Verify text is expected value <<sampleText>>
            TS_SetText(sampleText, out actualText, CheckType.IncorrectElementConfiguration);

            // Pre-Condition Create calling range=<<rangeType>>
            TS_CreateRange(out callingRange, rangeType, null, false, CheckType.Verification);

            // Call Select()
            TS_Select(callingRange, null, CheckType.Verification);

            // Call GetSelection() on calling range
            TS_GetSelection(ref selectedRange, arrayCount, expectedException, CheckType.Verification);

            // Compare selected range and calling range, comparison should = <<expectMatch>>
            TS_Compare(callingRange, selectedRange[0], ref isEqual, expectMatch, null, CheckType.Verification);
        }

        #endregion 2.2 TextPattern.GetSelection Method Helpers

        #region 2.3 TextPattern.RangeFromChild Method Helpers

        //---------------------------------------------------------------------------
        // Helper for RangeFromChild() test cases
        //---------------------------------------------------------------------------
        internal void RangeFromChildHelper(SampleText sampleText, bool requiresChildren, AutoElementType autoElementArgument, Type expectedException)
        {
            string actualText = "";
            AutomationElement[] autoElements = null;

            //Pre-Condition Verify text is expected value <<sampleText>>
            TS_SetText(sampleText, out actualText, CheckType.IncorrectElementConfiguration);

            //Pre-Condition Verify range children c<<requiresChildren>>
            TS_VerifyChildren(requiresChildren, CheckType.IncorrectElementConfiguration);

            //Pre-Condition Acquire automation element(s)
            TS_GetAutomationElement(ref autoElements, autoElementArgument, CheckType.Verification);

            // Iterate through each AutomationElement, calling RangeFromChild (<<expectedException>> for any expected errors)
            TS_EnumerateChildren(autoElements, true, expectedException, CheckType.Verification);
        }

        #endregion 2.3 TextPattern.RangeFromChild Method Helpers

        #region 2.4 TextPattern.GetVisibleRanges Method Helpers

        //---------------------------------------------------------------------------
        // Helper for GetVisibleRange() test cases
        // NOTE: This method assumes only a single range is selected, and will
        //       not work correctly if multiple selections have taken place
        //       Win32/DUI/Avalon do not support multiple selection.
        //---------------------------------------------------------------------------
        internal void GetVisibleRangesHelper(SampleText sampleText, bool isMultiLineExpected, SupportedTextSelection supportedTextSelection, uint arrayCount)
        {
            string             expectedText  = "";
            string             actualText    = "";
            TextPatternRange[] visibleRanges = null;

            // Pre-Condition Control must supports multi-line = <<expectedResult>>
            TS_IsMultiLine(isMultiLineExpected, CheckType.IncorrectElementConfiguration);

            // Pre-Condition Verify text is expected value <<sampleText>>
            TS_SetText(sampleText, out expectedText, CheckType.IncorrectElementConfiguration);

            // Call GetVisibleRange on the control with no errors
            TS_GetVisibleRanges(ref visibleRanges, null, CheckType.Verification);

            // Call GetText() on FIRST visible range (post V1, this should validate against each/all ranges)
            TS_GetText(visibleRanges[0], ref actualText, -1, null, CheckType.Verification);

            // Verify that text range matches what was set in the control
            TS_TextWithin("GetVisibleRange()", actualText, expectedText, CheckType.Verification);

        }
        #endregion 2.4 TextPattern.GetVisibleRanges Method Helpers

        #endregion TextPattern Method Helpers

        #region TextPattern Event Helpers

        #region 3.1 TextPattern.TextSelectionChanged Event

        //---------------------------------------------------------------------------
        // Helper for TextSelectionChangedEvent() test cases
        //---------------------------------------------------------------------------
        internal void TextSelectionChangedEventHelper(SampleText sampleText, TargetRangeType rangeType)
        {
            TextPatternRange       callingRange          = null;
            SupportedTextSelection supportedTextSelection = SupportedTextSelection.None; 

            // Pre-Condition Verify SupportedTextSelection != SupportedTextSelection.None
            Pattern_SupportedTextSelection(ref supportedTextSelection);
            TS_VerifySupportedTextSelection(supportedTextSelection, CheckType.IncorrectElementConfiguration);

            // Pre-Condition Verify text is expected value <<sampleText>> 
            TS_SetText(sampleText, CheckType.IncorrectElementConfiguration);

            // Pre-Condition Create range equal to <<rangeType>>
            TS_CreateRange(out callingRange, rangeType, null, false, CheckType.Verification);

            // Pre-Condition Set listener for TextPattern.SelectionChangedEvent
            TSC_AddEventListener(m_le, TextPattern.TextSelectionChangedEvent, TreeScope.Element, CheckType.Verification);

            // Select the element 
            TS_Select(callingRange, null, CheckType.Verification);

            // Wait for event
            TSC_WaitForEvents(1);

            // Verify ElementAddedToSelection event did/didnot happen
            TSC_VerifyEventListener(m_le, TextPattern.TextSelectionChangedEvent, EventFired.True, CheckType.Verification);
        }

        //---------------------------------------------------------------------------
        // Helper for TextSelectionChangedEvent() test cases
        //---------------------------------------------------------------------------
        internal void TextSelectionChangedEventHelper2(SampleText sampleText, bool backward)
        {
            SupportedTextSelection supportedTextSelection = SupportedTextSelection.None;

            // Pre-Condition Verify SupportedTextSelection!=SupportedTextSelection.None
            Pattern_SupportedTextSelection(ref supportedTextSelection);
            TS_VerifySupportedTextSelection(supportedTextSelection, CheckType.IncorrectElementConfiguration);

            // Pre-Condition Verify text is expected value <<sampleText>> 
            TS_SetText(sampleText, CheckType.IncorrectElementConfiguration);

            // Pre-Condition set focus to control containing TextPattern
            TS_SetFocus(CheckType.IncorrectElementConfiguration);

            // Pre-Condition Set listener for TextPattern.SelectionChangedEvent
            TSC_AddEventListener(m_le, TextPattern.TextSelectionChangedEvent, TreeScope.Element, CheckType.Verification);

            // Select one character using keyboard input
            TS_KeyboardMove(backward);

            // Wait for event
            TSC_WaitForEvents(1);

            // Verify ElementAddedToSelection event fires
            TSC_VerifyEventListener(m_le, TextPattern.TextSelectionChangedEvent, EventFired.True, CheckType.Verification);
        }

        #endregion 3.1 TextPattern.TextSelectionChanged Event

        #endregion TextPattern Event Helpers

        #endregion TextPattern Helpers

        #region TextPatternRange Helpers

        #region TextPatternRange Property Helpers

        #region 4.1 TextPatternRange.TextPattern Property Helpers

        //---------------------------------------------------------------------------
        // Helper for TextPattern() test cases
        //---------------------------------------------------------------------------
        internal void TextPatternHelper(SampleText sampleText)
        {
            string actualText = "";
            TextPattern newPattern = null;
            TextPatternRange range = null;

            // Pre-Condition Verify/Set text in control is as expected
            TS_SetText(sampleText, out actualText, CheckType.IncorrectElementConfiguration);

            // Get TextPattern Property Helpers
            range = Pattern_DocumentRange(CheckType.Verification);
            TS_TextPattern(range, ref newPattern, CheckType.Verification);

            // ReferenceEquals comparison of pattern and TextPattern Property Helpers
            TS_IsMatchingTextPattern(_pattern, newPattern, CheckType.Verification);
        }

        #endregion

        #region 4.2 TextPatternRange.Children Property Helpers

        //---------------------------------------------------------------------------
        // Helper for GetChildren() test cases
        //---------------------------------------------------------------------------
        internal void ChildrenHelper(SampleText sampleText, bool requiresChildren)
        {
            AutomationElement[] children = null;
            TextPatternRange documentRange = null;

            // Pre-Condition Verify text is expected value <<sampleText>> 
            TS_SetText(sampleText, CheckType.IncorrectElementConfiguration);

            // Pre-Condition Verify (Children Property Helpers is non-null) == <<hasChildren>>
            documentRange = Pattern_DocumentRange(CheckType.IncorrectElementConfiguration);
            TS_Children(documentRange, ref children, requiresChildren, CheckType.IncorrectElementConfiguration);

            // Enumerate children (if they exist)
            TS_EnumerateChildren(children, false, null, CheckType.Verification);
        }

        #endregion

        #endregion Properties

        #region TextPatternRange Method Helpers

        #region 5.1 TextPatternRange.Clone Method Helpers

        //---------------------------------------------------------------------------
        // Helper for Clone() test cases
        //---------------------------------------------------------------------------
        internal void CloneHelper(SampleText sampleText, TargetRangeType callingRangeType, bool requiresChildren)
        {
            TextPatternRange callingRange;
            TextPatternRange cloneRange = null;
            AutomationElement[] children = null;
            TextPatternRange documentRange = Pattern_DocumentRange(CheckType.IncorrectElementConfiguration);

            // Pre-Condition Verify text is expected value <<sampleText>> 
            TS_SetText(sampleText, CheckType.IncorrectElementConfiguration);

            // Pre-Condition Create range = <<callingRange>>
            TS_CreateRange(out callingRange, callingRangeType, null, false, CheckType.Verification);

            // Pre-Condition Children Property Helpers is <<requiresChildren>> array
            TS_Children(documentRange, ref children, requiresChildren, CheckType.IncorrectElementConfiguration);

            // Clone the range
            TS_Clone(callingRange, ref cloneRange, CheckType.Verification);

            // Verify identical ranges by calling Compare()
            TS_IsMatchingRange(callingRange, cloneRange, CheckType.Verification);

            // If children, Verify each child element is identical
            TS_IsMatchingRangeChildren(callingRange, cloneRange, CheckType.Verification);
        }

        #endregion 5.1 TextPatternRange.Clone Method Helpers

        #region 5.2 TextPatternRange.Compare Method Helpers

        //---------------------------------------------------------------------------
        // Helper for Compare() test cases
        //---------------------------------------------------------------------------
        internal void CompareHelper(SampleText sampleText, TargetRangeType callingRangeType, TargetRangeType argumentRangeType, bool expectMatch, Type expectedException)
        {
            bool isEqual = false;
            TextPatternRange callingRange;
            TextPatternRange argumentRange;
            string expectedText = "";
            IDictionary attribsExpected = null;
            AutomationElement enclosingExpected = null;
            AutomationElement[] childrenExpected = null;

            // Pre-Condition Verify text is expected value <<sampleText>> 
            TS_SetText(sampleText, out expectedText, CheckType.IncorrectElementConfiguration);

            // Pre-Condition Create calling range = <<callingRange>>
            TS_CreateRange(out callingRange, callingRangeType, null, false, CheckType.Verification);

            // Pre-Condition Argument range is <<argumentRange>>
            TS_CreateRange(out argumentRange, argumentRangeType, callingRange, false, CheckType.Verification);

            // Compare the ranges (<<expectedException>> for any expected errors)
            TS_Compare(callingRange, argumentRange, ref isEqual, expectMatch, expectedException, CheckType.Verification);

            // Calling Range: Get Attribute dictionary (assuming we're not done first!)
            if (expectedException != null)
            {
                Comment("Cannot continue with comparison tests when exception was raised as expected");
                return;
            }

            TS_GetAttributeValues(callingRange, out attribsExpected, AttributeType.SupportedAttributes, CheckType.Verification);

            // Calling Range: Get Get Enclosing element
            TS_GetEnclosingElement(callingRange, ref enclosingExpected, null, CheckType.Verification);

            // Calling Range: Get children
            // add the support for WPF controls - i.e. FlowDocument/Table scenario
            if (CheckTable(callingRange))
            {
                TS_Children(callingRange, ref childrenExpected, true, CheckType.Verification);
            }
            else
            {
                TS_Children(callingRange, ref childrenExpected, false, CheckType.Verification);
            }

            // Verify <<expectMatch>> Start/End Points
            TS_IsMatchingEndPoints(callingRange, argumentRange, expectMatch, CheckType.Verification);

            // Verify <<expectMatch>>  text
            TS_IsMatchingText("Compare", argumentRange, expectedText, expectMatch, true, CheckType.Verification);

            // Verify <<expectMatch>>  Attribute dictionary
            TS_IsMatchingDictionary(argumentRange, attribsExpected, AttributeType.SupportedAttributes, expectMatch, CheckType.Verification);

            // Verify <<expectMatch>>  Get Enclosing element
            TS_IsMatchingEnclosingElement(argumentRange, enclosingExpected, expectMatch, CheckType.Verification);

            // Verify <<expectMatch>>  children
            TS_IsMatchingChildren(argumentRange, childrenExpected, expectMatch, CheckType.Verification);
        }

        #endregion 5.2 TextPatternRange.Compare Method Helpers

        #region 5.3 TextPatternRange.CompareEndpoints Method Helpers

        //---------------------------------------------------------------------------
        // Helper for CompareEndpoints() test cases
        //---------------------------------------------------------------------------
        internal void CompareEndpointsHelper(SampleText sampleText, TargetRangeType callingRangeType, TargetRangeType argumentRangeType, ValueComparison startStart, ValueComparison startEnd, ValueComparison endStart, ValueComparison endEnd, Type expectedException)
        {
            TextPatternRange callingRange = null;
            TextPatternRange argumentRange = null;

            // Pre-Condition Verify text is expected value <<sampleText>> 
            TS_SetText(sampleText, CheckType.IncorrectElementConfiguration);

            // Pre-Condition Create calling range = <<callingRangeType>>
            TS_CreateRange(out callingRange, callingRangeType, null, true, CheckType.Verification);

            // Pre-Condition Create argument range = <<argumentRangeType>>
            TS_CreateRange(out argumentRange, argumentRangeType, callingRange, true, CheckType.Verification);

            // Verify CompareEndPoints(start,start) <<startStart>> zero
            TS_CompareEndpoints(callingRange, TextPatternRangeEndpoint.Start, argumentRange, 
                                              TextPatternRangeEndpoint.Start, startStart, expectedException, CheckType.Verification);

            // Verify CompareEndPoints(start,  end) <<startEnd>> zero
            TS_CompareEndpoints(callingRange, TextPatternRangeEndpoint.Start, argumentRange,
                                              TextPatternRangeEndpoint.End,   startEnd, expectedException, CheckType.Verification);

            // Verify CompareEndPoints(  end,start) <<endStart>> zero
            TS_CompareEndpoints(callingRange, TextPatternRangeEndpoint.End,   argumentRange,
                                              TextPatternRangeEndpoint.Start, endStart, expectedException, CheckType.Verification);

            // Verify CompareEndPoints(  end,  end) <<endEnd>> zero
            TS_CompareEndpoints(callingRange, TextPatternRangeEndpoint.End,   argumentRange, 
                                              TextPatternRangeEndpoint.End,   endEnd, expectedException, CheckType.Verification);
        }

        #endregion 5.3 TextPatternRange.CompareEndpoints Method Helpers

        #region 5.4 TextPatternRange.Select Method Helpers

        //---------------------------------------------------------------------------
        // Helper for Select() test cases
        //---------------------------------------------------------------------------
        internal void SelectHelper(SampleText sampleText, TargetRangeType callingRangeType, Type expectedException)
        {
            string actualText             = "";   
            string expectedText           = "";   // comparison of these strings.
            TextPatternRange callingRange = null;

            // Pre-Condition SetFocus to control containing TextPattern
            TS_SetFocus(CheckType.IncorrectElementConfiguration);

            // Pre-Condition Verify text is expected value <<sampleText>> 
            TS_SetText(sampleText, out expectedText, CheckType.IncorrectElementConfiguration);

            // Pre-Condition Create calling range = <<callingRangeType>>
            TS_CreateRange(out callingRange, callingRangeType, null, false, CheckType.Verification);

            // Select the range
            TS_Select(callingRange, expectedException, CheckType.Verification);

            // Get the text from the selection
            TS_GetTextSelection(ref actualText, CheckType.Verification);

            // Verify the selection is as expected
            TS_IsMatchingText("Select()", actualText, expectedText, true, CheckType.Verification);
        }

        #endregion 5.4 TextPatternRange.Select Method Helpers

        #region 5.5 TextPatternRange.FindAttribute Method Helpers

        //---------------------------------------------------------------------------
        // Helper for FindAttribute() test cases
        //---------------------------------------------------------------------------
        internal void FindAttributeHelper(SampleText sampleText, TypeValue typeValue, AttributeType attribType, bool backward, FindResults FindResults, Type expectedException)
        {
            bool isConsistentAttributes;
            IDictionary attributes = null;
            TextPatternRange documentRange = null;

            // Pre-Condition Verify text is expected value <<sampleText>> 
            TS_SetText(sampleText, CheckType.IncorrectElementConfiguration);

            // Pre-Condition Identify & use <<attribType>> attributes
            documentRange = Pattern_DocumentRange(CheckType.IncorrectElementConfiguration);
            TS_GetAttributeValues(documentRange, out attributes, attribType, CheckType.Verification);

            // Pre-Condition Identify if document has consistent attribute values
            TS_VerifyAttributeRanges(attributes, out isConsistentAttributes);

            // Pre-Condition For each attribute, Val argument has <<typeValue>> type and <<typeValue>> value
            TS_SetAttributeValues(ref attributes, typeValue);

            // Call FindAttribute(<<backward>>) without errors (<<FindResults>>)/with <<expectedException, if not null>>
            TS_FindAttributes(attributes, typeValue, backward, FindResults, expectedException, CheckType.Verification);
        }

        //---------------------------------------------------------------------------
        // Helper for FindAttribute() test cases
        //---------------------------------------------------------------------------
        internal void FindAttributeHelper2(SampleText sampleText, Type expectedException)
        {
            object value = null;
            TextPatternRange returnedRange = null;
            TextPatternRange documentRange = Pattern_DocumentRange(CheckType.IncorrectElementConfiguration); 
            
            // Pre-Condition Verify text is expected value <<sampleText>> 
            TS_SetText(sampleText, CheckType.IncorrectElementConfiguration);

            // Call FindAttribute(null,<correct type & value>,false) <<expectedException>>
            Range_FindAttribute(documentRange, ref returnedRange, TextPattern.IsReadOnlyAttribute, value, false, expectedException, CheckType.Verification);
        }

        //---------------------------------------------------------------------------
        // Helper for FindAttribute() test cases
        //---------------------------------------------------------------------------
        internal void FindAttributeHelper3(SampleText sampleText, Type expectedException)
        {
            int bogusValue = 0;
            TextPatternRange returnedRange = null;
            TextPatternRange documentRange = Pattern_DocumentRange(CheckType.IncorrectElementConfiguration); 

            // Pre-Condition Verify text is expected value <<sampleText>> 
            TS_SetText(sampleText, CheckType.IncorrectElementConfiguration);

            // Call FindAttribute(<faux random Attribute>,null,false) <<expectedException>>
            Range_FindAttribute(documentRange, ref returnedRange, null, bogusValue, false, expectedException, CheckType.Verification);
        }

        #endregion

        #region 5.6 TextPatternRange.FindText Method Helpers

        //---------------------------------------------------------------------------
        // Helper for FindText() test cases       
        //---------------------------------------------------------------------------
        internal void FindTextHelper(SampleText sampleText, SearchText searchTextType, FindResults result1, FindResults result2, FindResults result3, FindResults result4, Type expectedException)
        {
            string searchText;

            // Pre-Condition Verify text is expected value <<sampleText>> 
            TS_SetText(sampleText, CheckType.IncorrectElementConfiguration);

            // Pre-Condition Text to search for is <<searchText>>
            TS_SetSearchText(out searchText, searchTextType, CheckType.Verification);

            // CallFindText(forwards, case-sensitive) <<result1>>
            if ((searchTextType != SearchText.MatchesLastBlock) && (searchTextType != SearchText.MismatchedCaseLastBlock))
                TS_FindText(searchText, false, false, result1, expectedException, CheckType.Verification);
            else
                m_TestStep++;

            // CallFindText(forwards, case-insensitive)   <<result2>>
            if ((searchTextType != SearchText.MatchesLastBlock) && (searchTextType != SearchText.MismatchedCaseLastBlock))
                TS_FindText(searchText, false, true, result2, expectedException, CheckType.Verification);
            else
                m_TestStep++;

            // CallFindText(backwards,case-sensitive) <<result3>>
            if ((searchTextType != SearchText.MatchesFirstBlock) && (searchTextType != SearchText.MismatchedCaseFirstBlock))
                TS_FindText(searchText, true, false, result3, expectedException, CheckType.Verification);
            else
                m_TestStep++;

            // CallFindText(backwards,case-insensitive)  <<result4>>
            if ((searchTextType != SearchText.MatchesFirstBlock) && (searchTextType != SearchText.MismatchedCaseFirstBlock))
                TS_FindText(searchText, true, true, result4, expectedException, CheckType.Verification);
            else
                m_TestStep++;

        }

        #endregion

        #region 5.7 TextPatternRange.GetAttributeValue Method Helpers

        //---------------------------------------------------------------------------
        // Helper for GetAttributeValue() test cases
        //---------------------------------------------------------------------------
        internal void GetAttributeValueHelper(SampleText sampleText, TargetRangeType callingRangeType, AttributeType attribType, GetResult getResult, Type expectedException)
        {
            IDictionary attributes = null;
            TextPatternRange callingRange;

            // Pre-Condition Verify text is expected value <<sampleText>> 
            TS_SetText(sampleText, CheckType.IncorrectElementConfiguration);

            // Pre-Condition Create calling range = <<callingRangeType>>
            TS_CreateRange(out callingRange, callingRangeType, null, false, CheckType.Verification);

            // Pre-Condition Identify & use <<attribType>>
            TS_GetAttributeValues(callingRange, out attributes, attribType, CheckType.Verification);

            // For each attribute identified, GetAttributeValue() returns <<getResult>>
            TS_GetAttributeValue(callingRange, attributes, attribType, getResult, expectedException, CheckType.Verification);
        }

        #endregion

        #region 5.8 TextPatternRange.GetBoundingRectangles Method Helpers

        //---------------------------------------------------------------------------
        // Helper for GetBoundingRectangles() test cases
        //---------------------------------------------------------------------------
        internal void GetBoundingRectanglesHelper(SampleText sampleText, TargetRangeType callingRangeType, bool isMultiLine, ScrollLocation scrollLocation, GetBoundRectResult boundRectResults)
        {
            Rect autoElement;
            Rect[] boundRects = new Rect[0];
            TextPatternRange callingRange;

            // Pre-Condition Control must supports multi-line = <<expectedResult>>
            TS_IsMultiLine(isMultiLine, CheckType.IncorrectElementConfiguration);

            // Pre-Condition Verify text is expected value <<sampleText>> 
            TS_SetText(sampleText, CheckType.IncorrectElementConfiguration);

            // Pre-Condition Create calling range = <<callingRangeType>>
            TS_CreateRange(out callingRange, callingRangeType, null, false, CheckType.Verification);

            if (!(CheckDocument(callingRange))) // 
            {
                // Pre-Condition Scroll viewport to <<ScrollLocation>>
                TS_ScrollViewPort(scrollLocation, false, CheckType.IncorrectElementConfiguration);

                // Pre-Condition Get Automation Element Bounding Rectangle
                TS_GetAutomationElementBoundingRectangle(out autoElement);

                // Call GetBoundingRectangles()
                TS_GetBoundingRectanglesLite(callingRange, ref boundRects, false, null, CheckType.Verification);

                // Valdiate Bounding Rectangles <<getResult>>
                TS_VerifyWithinRects(autoElement, boundRects, boundRectResults, CheckType.Verification);
            }
        }

        #endregion

        #region 5.9 TextPatternRange.GetText Method Helpers

        //---------------------------------------------------------------------------
        // Helper for GetText() test cases
        //---------------------------------------------------------------------------
        internal void GetTextHelper(SampleText sampleText, TargetRangeType callingRangeType, MaxLength maxLengthType, GetResult getResult, Type expectedException)
        {
            int maxLength = 0;
            string actualText = "";
            string expectedText = "";
            TextPatternRange callingRange;

            // Pre-Condition Verify text is expected value <<sampleText>> 
            TS_SetText(sampleText, out expectedText, CheckType.IncorrectElementConfiguration);

            // Pre-Condition Create calling range = <<callingRangeType>>
            TS_CreateRange(out callingRange, callingRangeType, null, false, CheckType.Verification);

            // Pre-Condition Determine length of text to get = <<maxLength>>
            TS_CalcMaxLength(maxLengthType, out maxLength, expectedText);

            // Call GetText(<<maxLength>>) <<expectedException>>
            TS_GetText(callingRange, ref actualText, maxLength, expectedException, CheckType.Verification);

            // Validate text is <<getResult>>
            TS_VerifyTextLength(getResult, actualText, expectedText, maxLength, expectedException, CheckType.Verification);
        }

        #endregion

        #region 5.10 TextPatternRange.Move Method Helpers

        //---------------------------------------------------------------------------
        // Helper for Move() test cases
        //---------------------------------------------------------------------------
        internal void MoveHelper(SampleText sampleText, TargetRangeType callingRangeType, Count count, Count expectedCount)
        {
            TextUnit[]       supportedTextUnits = new TextUnit[((int)TextUnit.Document) + 1];
            int[]            numberOfTextUnits  = new int[((int)TextUnit.Document) + 1];
            TextPatternRange callingRange       = null;
            TextPatternRange documentRange      = null;

            // Pre-Condition Verify text is expected value <<sampleText>> 
            TS_SetText(sampleText, CheckType.IncorrectElementConfiguration);

            // Pre-Condition: Acquire range for entire document
            TS_DocumentRange( ref documentRange, CheckType.IncorrectElementConfiguration );

            // Pre-Condition Create calling range = <<callingRangeType>>
            TS_CreateRange(out callingRange, callingRangeType, null, false, CheckType.Verification);

            // Pre-Condition Determine supported TextUnits for this control
            TS_IdentifySupportedTextUnits(ref supportedTextUnits);

            // Pre-Condition: Determine Count for each TextUnit in document
            TS_CountTextUnits( documentRange, supportedTextUnits, ref numberOfTextUnits );
            
            // Call Move(<<count>>) for each TextUnit, validiting result is <<result>>
            TS_Move(callingRange, supportedTextUnits, numberOfTextUnits, count, expectedCount, CheckType.Verification);
        }

        //---------------------------------------------------------------------------
        // Helper for Move() test cases
        //---------------------------------------------------------------------------
        internal void MoveHelper2(SampleText sampleText, TargetRangeType callingRangeType, int count, int result)
        {
            int[] numberOfTextUnits = new int[((int)TextUnit.Document) + 1];
            TextUnit[] supportedTextUnits = new TextUnit[((int)TextUnit.Document) + 1];
            TextPatternRange callingRange;

            //  Pre-Condition Verify text is expected value <<sampleText>> 
            TS_SetText(sampleText, CheckType.IncorrectElementConfiguration);

            // Pre-Condition Create calling range = <<callingRangeType>>
            TS_CreateRange(out callingRange, callingRangeType, null, false, CheckType.Verification);

            // Pre-Condition Determine supported TextUnits for this control
            TS_IdentifySupportedTextUnits(ref supportedTextUnits);

            // Pre-Condition Determine Count for each TextUnit in document
            TS_CountTextUnits(callingRange, supportedTextUnits, ref numberOfTextUnits);

            // Call Move(<<count>>) (N+1) times where N=# characters in doc, verify result = <<result>>
            TS_MoveNTimes(TextUnit.Character, numberOfTextUnits, callingRange, count, result, CheckType.Verification);

            // Call Move(<<count>>) (N+1) times where N=# formats in doc, verify result = <<result>>
            TS_MoveNTimes(TextUnit.Format, numberOfTextUnits, callingRange, count, result, CheckType.Verification);

            // Call Move(<<count>>) (N+1) times where N=# words in doc, verify result = <<result>>
            TS_MoveNTimes(TextUnit.Word, numberOfTextUnits, callingRange, count, result, CheckType.Verification);

            // Call Move(<<count>>) (N+1) times where N=# lines in doc, verify result = <<result>>
            TS_MoveNTimes(TextUnit.Line, numberOfTextUnits, callingRange, count, result, CheckType.Verification);

            // Call Move(<<count>>) (N+1) times where N=# paragraphs in doc, verify result = <<result>>
            TS_MoveNTimes(TextUnit.Paragraph, numberOfTextUnits, callingRange, count, result, CheckType.Verification);

            // Call Move(<<count>>) (N+1) times where N=# pages in doc, verify result = <<result>>
            TS_MoveNTimes(TextUnit.Page, numberOfTextUnits, callingRange, count, result, CheckType.Verification);

            // Call Move(<<count>>) (N+1) times where N=# document in doc, verify result = <<result>>
            TS_MoveNTimes(TextUnit.Document, numberOfTextUnits, callingRange, count, result, CheckType.Verification);
        }

        #endregion

        #region 5.11 TextPatternRange.MoveEndpointByUnit Method Helpers

        //---------------------------------------------------------------------------
        // Helper for MoveEndpointByUnit() test cases
        //---------------------------------------------------------------------------
        internal void MoveEndpointByUnitHelper1(SampleText sampleText, TargetRangeType callingRangeType, int startOne, int startNegOne, int endOne, int endNegOne)
        {
            TextPatternRange documentRange      = Pattern_DocumentRange(CheckType.Verification);
            int              richEditOffset     = TextLibrary.CountTrailingCRLF(m_le, documentRange);
            TextUnit[]       supportedTextUnits = new TextUnit[((int)TextUnit.Document) + 1]; ;
            TextPatternRange callingRange       = null;

            // Pre-Condition Verify text is expected value <<sampleText>> 
            TS_SetText(sampleText, CheckType.IncorrectElementConfiguration);

            // Pre-Condition Create calling range = <<callingRangeType>>
            TS_CreateRange(out callingRange, callingRangeType, null, false, CheckType.Verification);

            // Pre-Condition Determine supported TextUnits for this control
            TS_IdentifySupportedTextUnits(ref supportedTextUnits);

            // Verify MoveEndpointEndPoints(start,TextUnit.*, 1) returns <<startOne>> for all TextUnits
            TS_MoveEndpointByUnitAndValidate(callingRange, TextPatternRangeEndpoint.Start, 1, startOne, richEditOffset, CheckType.Verification);

            // Verify MoveEndpointEndPoints(start,TextUnit.*,-1) returns <<startNegOne>> for all TextUnits
            TS_MoveEndpointByUnitAndValidate(callingRange, TextPatternRangeEndpoint.Start, -1, startNegOne, richEditOffset, CheckType.Verification);

            // Verify MoveEndpointEndPoints(end,  TextUnit.*, 1) returns <<endOne>> for all TextUnits
            TS_MoveEndpointByUnitAndValidate(callingRange, TextPatternRangeEndpoint.End, 1, endOne, richEditOffset, CheckType.Verification);

            // Verify MoveEndpointEndPoints(end,  TextUnit.*,-1) returns <<endNegOne>> for all TextUnits
            TS_MoveEndpointByUnitAndValidate(callingRange, TextPatternRangeEndpoint.End, -1, endNegOne, richEditOffset, CheckType.Verification);
        }

        //---------------------------------------------------------------------------
        // Helper for MoveEndpointByUnit() test cases
        //---------------------------------------------------------------------------
        internal void MoveEndpointByUnitHelper2(SampleText sampleText, TargetRangeType callingRangeType, Count requestedCount, Count startExpectedCount, Count endExpectedCount )
        {
            TextPatternRange documentRange      = Pattern_DocumentRange(CheckType.Verification);
            int              richEditOffset     = TextLibrary.CountTrailingCRLF(m_le, documentRange);
            int[]            numberOfTextUnits  = new int[((int)TextUnit.Document) + 1];
            TextUnit[]       supportedTextUnits = new TextUnit[((int)TextUnit.Document) + 1]; 
            TextPatternRange callingRange       = null;

            // Pre-Condition Verify text is expected value <<sampleText>> 
            TS_SetText(sampleText, CheckType.IncorrectElementConfiguration);

            // Pre-Condition Create calling range = <<callingRangeType>>
            TS_CreateRange(out callingRange, callingRangeType, null, false, CheckType.Verification);

            // Pre-Condition Determine supported TextUnits for this control
            TS_IdentifySupportedTextUnits(ref supportedTextUnits);
            
            // Pre-Condition Calculate # of text units to move by
            TS_CountTextUnits(callingRange, supportedTextUnits, ref numberOfTextUnits);

            // Verify MoveEndpointEndPoints(start,TextUnit.*, 1) returns <<startOne>> for all TextUnits
            TS_MoveEndpointByUnitAndValidate2(callingRange, TextPatternRangeEndpoint.Start, requestedCount, startExpectedCount, numberOfTextUnits, richEditOffset, CheckType.Verification);

            // Verify MoveEndpointEndPoints(end,  TextUnit.*, 1) returns <<endOne>> for all TextUnits
            TS_MoveEndpointByUnitAndValidate2(callingRange, TextPatternRangeEndpoint.End, requestedCount, endExpectedCount, numberOfTextUnits, richEditOffset, CheckType.Verification);

        }


        //---------------------------------------------------------------------------
        // Helper for MoveEndpointByUnit() test cases
        //---------------------------------------------------------------------------
        internal void MoveEndpointByUnitHelper3(SampleText sampleText, TargetRangeType callingRangeType, bool isMultiLineExpected, TextUnit textUnit, TextPatternRangeEndpoint endPoint)
        {
            TextPatternRange callingRange;

            // Pre-Condition Control must supports multi-line = <<expectedResult>>
            TS_IsMultiLine(isMultiLineExpected, CheckType.IncorrectElementConfiguration);

            // Pre-Condition Verify text is expected value <<sampleText>> 
            TS_SetText(sampleText, CheckType.IncorrectElementConfiguration);

            // Pre-Condition Create calling range = <<callingRangeType>>
            TS_CreateRange(out callingRange, callingRangeType, null, false, CheckType.Verification);

            ThrowMe(CheckType.IncorrectElementConfiguration, "Not impelemented Yet");

            // Pre-Condition Calculate # of random text units to move between 1..8

            // Verify MoveEndpointByUnit(End,  Character,<rand#>) returns <rand#>

            // Verify that the text enclosed is as expected

        }
        #endregion

        #region 5.12 TextPatternRange.MoveEndpointByRange Method Helpers

        internal void MoveEndpointByRangeHelper1(SampleText sampleText, TargetRangeType callingRangeType, TargetRangeType targetRangeType, MoveEPResults result1, MoveEPResults result2, MoveEPResults result3, MoveEPResults result4, Type expectedException)
        {
            TextPatternRange callingRange = null;
            TextPatternRange targetRange = null;

            // Pre-Condition Verify text is expected value <<sampleText>> 
            TS_SetText(sampleText, CheckType.IncorrectElementConfiguration);

            // Pre-Condition Create calling range = <<callingRangeType>>
            TS_CreateRange(out callingRange, callingRangeType, null, true, CheckType.Verification);

            // Pre-Condition Createtarget range = <<targetRangeType>>
            TS_CreateRange(out targetRange, targetRangeType, callingRange, true, CheckType.Verification);

            // Call MoveEndpointByRange(Start,target range,Start) with result = <<result1>>
            TS_MoveEndpointByRange(callingRange, TextPatternRangeEndpoint.Start,
                                    targetRange, TextPatternRangeEndpoint.Start,
                                    result1, expectedException, CheckType.Verification);

            // Call MoveEndpointByRange(Start,target range,End  ) with result = <<result2>>
            TS_MoveEndpointByRange(callingRange, TextPatternRangeEndpoint.Start,
                                   targetRange, TextPatternRangeEndpoint.End,
                                   result2, expectedException, CheckType.Verification);

            // Call MoveEndpointByRange(End,  target range,Start) with result = <<result3>>
            TS_MoveEndpointByRange(callingRange, TextPatternRangeEndpoint.End,
                                   targetRange, TextPatternRangeEndpoint.Start,
                                   result3, expectedException, CheckType.Verification);

            // Call MoveEndpointByRange(End,  target range,End  ) with result = <<result4>>
            TS_MoveEndpointByRange(callingRange, TextPatternRangeEndpoint.End,
                                   targetRange, TextPatternRangeEndpoint.End,
                                   result4, expectedException, CheckType.Verification);
        }
        #endregion

        #region 5.13 TextPatternRange.ExpandToEnclosingUnit Method Helpers

        //---------------------------------------------------------------------------
        // Helper for MoveEndpointByRange() test cases
        //---------------------------------------------------------------------------
        internal void ExpandToEnclosingUnitHelper(SampleText sampleText, TargetRangeType callingRangeType)
        {
            string           text               = "";
            TextPatternRange callingRange       = null;
            TextUnit[]       supportedTextUnits = new TextUnit[((int)TextUnit.Document) + 1]; ;

            // PreCondition: Determine supported TextUnits for this control
            TS_IdentifySupportedTextUnits(ref supportedTextUnits);

            // Pre-Condition Verify text is expected value <<sampleText>> 
            TS_SetText(sampleText, out text, CheckType.IncorrectElementConfiguration);

            // Pre-Condition Create calling range = <<callingRangeType>>
            TS_CreateRange(out callingRange, callingRangeType, null, false, CheckType.Verification);

            // Clone range and call ExpandToEnclosingUnits(Character) and verify results
            TS_ExpandToEnclosingUnits(callingRange, supportedTextUnits, TextUnit.Character, text.Length, CheckType.Verification);

            //



            // Clone range and call ExpandToEnclosingUnits(Word)      and verify results
            TS_ExpandToEnclosingUnits(callingRange, supportedTextUnits, TextUnit.Word, text.Length, CheckType.Verification);

            //



            // Clone range and call ExpandToEnclosingUnits(Praagraph) and verify results
            TS_ExpandToEnclosingUnits(callingRange, supportedTextUnits, TextUnit.Paragraph, text.Length, CheckType.Verification);

            // Clone range and call ExpandToEnclosingUnits(Page)      and verify results
            TS_ExpandToEnclosingUnits(callingRange, supportedTextUnits, TextUnit.Page, text.Length, CheckType.Verification);

            // Clone range and call ExpandToEnclosingUnits(Document)  and verify results   
            TS_ExpandToEnclosingUnits(callingRange, supportedTextUnits, TextUnit.Document, text.Length, CheckType.Verification);
        }

        #endregion

        #region 5.14 TextPatternRange.ScrollIntoView Method Helpers

        //---------------------------------------------------------------------------
        // Helper for ScrollIntoView() test cases
        //---------------------------------------------------------------------------
        internal void ScrollIntoViewHelper(SampleText sampleText, TargetRangeType callingRangeType, bool alignToTop)
        {
            string actualText = "";
            string expectedText = "";
            TextPatternRange callingRange;
            TextPatternRange[] visibleRanges = null;

            // Pre-Condition Verify text is expected value <<sampleText>> 
            TS_SetText(sampleText, CheckType.IncorrectElementConfiguration);

            // Pre-Condition Create calling range = <<callingRangeType>>
            TS_CreateRange(out callingRange, callingRangeType, null, false, CheckType.Verification);

            // Pre-Condition Get text from calling range
            TS_GetText(callingRange, ref expectedText, -1, null, CheckType.Verification);

            // Pre-Condition SetFocus to control containing TextPattern
            TS_SetFocus(CheckType.Verification);

            // Call ScrollIntoView(<<alignToTop>>)
            TS_ScrollIntoView(callingRange, alignToTop, null, CheckType.Verification);

            // Call GetVisibleRange() on TextPattern
            TS_GetVisibleRanges(ref visibleRanges, null, CheckType.Verification);

            // Call GetText() on FIRST visible range (post V1, this should validate against each/all ranges)
            TS_GetText(visibleRanges[0], ref actualText, -1, null, CheckType.Verification);

            // Verify visible range is within expected range
            TS_TextWithin("ScrollIntoView()", actualText, expectedText, CheckType.Verification);
        }
        #endregion

        #region 5.15 TextPatternRange.GetEnclosingElement Method Helpers

        //---------------------------------------------------------------------------
        // Helper for GetEnclosingElement() test cases
        //---------------------------------------------------------------------------
        internal void GetEnclosingElementHelper(SampleText sampleText, bool hasChildren)
        {
            AutomationElement parent = null;
            TextPatternRange documentRange = Pattern_DocumentRange(CheckType.Verification);

            // Pre-Condition Verify text is expected value <<sampleText>> 
            TS_SetText(sampleText, CheckType.IncorrectElementConfiguration);

            // Pre-Condition Confirm (children are present) = <<hasChildren>>
            TS_VerifyChildren(hasChildren, CheckType.IncorrectElementConfiguration);

            // Call GetEnclosingElement()
            TS_GetEnclosingElement(documentRange, ref parent, null, CheckType.Verification);

            // Perform value equality to validate instances have same identity
            TS_VerifyAutomationElement(m_le, parent, CheckType.Verification);

            // Enumerate through children (if present), calling GetEnclosingElement on each
            TS_RecurseChildAutomationElements(documentRange, CheckType.Verification);
        }

        #endregion

        #region 5.16 TextPatternRange.AddToSelection Method Helpers

        //---------------------------------------------------------------------------
        // Helper for AddToSelection() test cases
        //---------------------------------------------------------------------------
        internal void AddToSelectionHelper(Type expectedException)
        {
            TextPatternRange documentRange = Pattern_DocumentRange(CheckType.Verification);

            TS_AddToSelection( documentRange, expectedException, CheckType.Verification );
        }

        #endregion 5.16 TextPatternRange.AddToSelection Method Helpers


        #region 5.17 TextPatternRange.RemoveFromSelection Method Helpers

        //---------------------------------------------------------------------------
        // Helper for RemoveFromSelection() test cases
        //---------------------------------------------------------------------------
        internal void RemoveFromSelectionHelper(Type expectedException)
        {
            TextPatternRange documentRange = Pattern_DocumentRange(CheckType.Verification);

            TS_RemoveFromSelection(documentRange, expectedException, CheckType.Verification);
        }

        #endregion 5.17 TextPatternRange.RemoveFromSelection Method Helpers

        #endregion TextPatternRange Method Helpers

        #endregion TextPatternRange Helpers

        #endregion Helper

        #region TestSteps

        #region TextPattern TestSteps

        #region TextPattern Property TestSteps

        //---------------------------------------------------------------------------
        // TestStep for TextPattern.DocumentRange Property
        //---------------------------------------------------------------------------
        internal void TS_DocumentRange(ref TextPatternRange range, CheckType checkType)
        {
            range = Pattern_DocumentRange(checkType);
            m_TestStep++;
        }

        //---------------------------------------------------------------------------
        // TestStep for TextPattern.SupportedTextSelection Property
        //---------------------------------------------------------------------------
        internal void TS_SupportedTextSelection(ref SupportedTextSelection supportedTextSelection)
        {
            Pattern_SupportedTextSelection(ref supportedTextSelection);
            m_TestStep++;
        }

        #endregion TextPattern Property TestStep

        #region TextPattern Method TestSteps

        //---------------------------------------------------------------------------
        // TestStep for TextPattern.RangeFromPoint Method
        //---------------------------------------------------------------------------
        internal void TS_RangeFromPoint(ref TextPatternRange range, Point screenLocation, Type expectedException, CheckType checkType)
        {
            Pattern_RangeFromPoint(_pattern, ref range, screenLocation, expectedException, checkType);

            if (expectedException == null)
                Comment("RangeFromPoint(" + screenLocation + ") returned of size " + Range_GetText(range).Length);

            m_TestStep++;
        }

        //---------------------------------------------------------------------------
        // TestStep for TextPattern.GetSelection Method
        //---------------------------------------------------------------------------
        internal void TS_GetSelection(ref TextPatternRange[] ranges, uint arrayCount, Type expectedException, CheckType checkType)
        {
            Pattern_GetSelection(_pattern, ref ranges, expectedException, checkType);

            if( ranges.Length != arrayCount )
            {
                Comment("EXPECTED array of ranges length = " + arrayCount );
                ThrowMe(checkType, "GetSelection() did not return an array of ranges of the expected size");
            }
            m_TestStep++;
        }

        //---------------------------------------------------------------------------
        // TestStep for TextPattern.GetVisibleRange Method
        //---------------------------------------------------------------------------
        internal void TS_GetVisibleRanges(ref TextPatternRange[] ranges, Type expectedException, CheckType checkType)
        {
            Pattern_GetVisibleRanges(_pattern, ref ranges, expectedException, checkType);

            m_TestStep++;
        }

        #endregion TextPattern Method TestStep

        #endregion TextPattern TestSteps

        #region TextPatternRange TestSteps

        #region TextPatternRange Property TestSteps

        //---------------------------------------------------------------------------
        // TestStep for TextPatternRange.TextPattern Property
        //---------------------------------------------------------------------------
        static internal void TS_TextPattern(TextPatternRange range, ref TextPattern pattern, CheckType checkType)
        {
            Range_TextPattern(range, ref pattern);

            if (pattern == null)
                ThrowMe(checkType, "TextPatternRange.TextPattern property should not return NULL");

            m_TestStep++;
        }

        //---------------------------------------------------------------------------
        // TestStep for TextPatternRange.Children Property
        //---------------------------------------------------------------------------
        static internal void TS_Children(TextPatternRange range, ref AutomationElement[] children, bool requiresChildren, CheckType checkType)
        {
            // Note that CheckType is hard-coded for a reason, it will always be an error
            Range_GetChildren(range, ref children, checkType);

            if ((children.Length == 0) && (requiresChildren == true))
                ThrowMe(checkType, "Requires non-empty array of child automation elements, no child elements found");

            if ((children.Length > 0) && (requiresChildren == false))
                ThrowMe(checkType, "Requires empty array of child automation elements," + children.Length + " elements found");

            Comment("TS_Children returned " + children.Length + " child AutomationElements");
            m_TestStep++;
        }


        #endregion TextPatternRange Property TestStep

        #region TextPatternRange Method TestSteps

        //---------------------------------------------------------------------------
        // TestStep for TextPatternRange.Clone Method
        //---------------------------------------------------------------------------
        internal void TS_Clone(TextPatternRange range, ref TextPatternRange clonedRange, CheckType checkType)
        {
            Range_Clone(range, ref clonedRange, null, checkType);
            m_TestStep++;
        }

        //---------------------------------------------------------------------------
        // TestStep for TextPatternRange.Compare Method
        //---------------------------------------------------------------------------
        internal void TS_Compare(TextPatternRange range, TextPatternRange targetRange, ref bool isEqual, bool expectEqual, Type expectedException, CheckType checkType)
        {
            Range_Compare(range, targetRange, ref isEqual, expectedException, checkType);

            Comment("TS_Compare returned " + isEqual.ToString() + ", i.e. ranges are " +
                    (isEqual == true
                        ? "equal"
                        : "not equal"));

            // Compare expected and actual results
            if (isEqual == expectEqual)
                Comment("Comparison of ranges gave expected results");
            else
            {
                ThrowMe(
                        checkType, "Comparison of ranges did not Match expectations, should have been " +
                        (expectEqual == true
                            ? "equal"
                            : "not equal"));
            }

            m_TestStep++;
        }

        //---------------------------------------------------------------------------
        // TestStep for TextPatternRange.CompareEndpoints Method
        //---------------------------------------------------------------------------
        internal void TS_CompareEndpoints(TextPatternRange callingRange, TextPatternRangeEndpoint endPoint, TextPatternRange argumentRange, TextPatternRangeEndpoint targetEndPoint, ValueComparison valueComparison, Type expectedException, CheckType checkType)
        {
            int endPointOffset = 0;
            string msg = endPoint.ToString() + "/" + targetEndPoint.ToString();

            Range_CompareEndpoints(callingRange, endPoint,
                        argumentRange, targetEndPoint, ref endPointOffset, expectedException, checkType);

            IsMatchingValue(endPointOffset, valueComparison, 0, msg, checkType);
            m_TestStep++;
        }

        //---------------------------------------------------------------------------
        // TestStep for TextPatternRange.Select Method
        //---------------------------------------------------------------------------
        internal void TS_Select(TextPatternRange range, Type expectedException, CheckType checkType)
        {
            // Bug2: TextPattern.TextSelectionChangedEvent does not fire unless control has focus.
            m_le.SetFocus();
            Range_Select(range, expectedException, checkType);

            m_TestStep++;
        }

        //---------------------------------------------------------------------------
        // Test step to find attributes
        //---------------------------------------------------------------------------
        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "System.ArgumentException.#ctor(System.String)")]
        internal void TS_FindAttributes(IDictionary attributes, TypeValue typeValue, bool backward, FindResults FindResults, Type expectedException, CheckType checkType)
        {
            int errorCount = 0;
            bool isEqual = false;
            string text = "";
            AutomationTextAttribute key = null;    // key / name of attribute
            object valueToFind = null;    // Value to perform FindAttribute on
            TextPatternRange callingRange = null;   // Range to do FindAttribute on, default to entire document
            TextPatternRange findRange = null;    // Range FindAttribute returns
            IDictionaryEnumerator enum1 = attributes.GetEnumerator(); // Enumerate through the dictionary

            callingRange = Pattern_DocumentRange(checkType);

            // RichEdit oddness again. We can't just use the entire document range.
            TextLibrary.TrimRangeCRLF(m_le, callingRange);

            // Enumerate through attributes
            while (enum1.MoveNext())
            {
                string keyString = "";

                // get key and value for this attribute
                key = (AutomationTextAttribute)enum1.Key;
                keyString = Helpers.GetProgrammaticName(key);

                if (enum1.Value == TextPattern.MixedAttributeValue) // Try to get a smaller range for a non-mixed attribute value
                {
                    Comment("Range has mixed attribute value for attribute " + keyString + ". Tryig to create single-character range to determine non-mixed attribute");
                    
                    // Limit range to first character so we should get CONSISTENT attribute value(!)
                    //
                    // 



                    Range_ExpandToEnclosingUnit(callingRange, TextUnit.Character, null, checkType);

                    Range_GetAttributeValue(callingRange, (AutomationTextAttribute)enum1.Key, ref valueToFind, null, checkType);

                    if (valueToFind == TextPattern.MixedAttributeValue)
                    {
                        ThrowMe(checkType, "Single character range should(!) not have a mixed attribute value for attribute " + keyString + ". (International Locale perhaps?)");
                    }
                }
                else if (enum1.Value == AutomationElement.NotSupported)
                {
                    Comment("Attribute " + keyString + " is not supported, skipping...");
                    continue;
                }
                else
                {
                    valueToFind = enum1.Value;
                }

                SetValue(typeValue, key, ref valueToFind); // cast to correct type

                string msg = "FindAttribute(" + keyString + "," +
                        (valueToFind != null ? valueToFind.ToString() : "NULL") + "," +
                        (backward == true ? "backwards" : "forwards") +
                        ")";

                // Finally, find the attribute value(!)
                Range_FindAttribute(callingRange, ref findRange, (AutomationTextAttribute)enum1.Key, valueToFind, backward,
                                    expectedException, checkType);

                // Did we get correct results?
                switch (FindResults)
                {
                    case FindResults.Exception:
                        if (expectedException == null)
                            ThrowMe(checkType, "Test code error, null expected exception is incorrect when getresult = " + Parse(FindResults, expectedException));
                        break;                  // actually takes place in Range_FindAttribute
                    case FindResults.MatchingRange:
                        if (findRange == null)
                        {
                            Comment("#######" + msg + " returned null range. Expected non-null range");
                            errorCount++;
                        }
                        else
                        {


                            Range_Compare(callingRange, findRange, ref isEqual, null, checkType);
                            if (isEqual == false)
                            {
                                string callingText = "", findText = "";
                                Comment("####### Comparison failed. Expected calling and actual range to have matching text");
                                Range_GetText(callingRange, ref callingText, -1, null, checkType);
                                Comment("Calling range text = '" + TrimText(callingText, 512) + "'");
                                Range_GetText(findRange, ref findText, -1, null, checkType);
                                Comment("Actual  range text = '" + TrimText(findText, 512) + "'");
                                errorCount++;
                            }
                        }
                        break;
                    case FindResults.EmptyRange:
                        if (findRange == null)
                        {
                            Comment("#######" + msg + " returned null range. Expected non-null range");
                            errorCount++;
                        }
                        else
                        {
                            Range_GetText(findRange, ref text, -1, null, checkType);
                            if (text.Length != 0)
                            {
                                Comment("#######" + msg + " returned non-zero length text for range. Expected zero-length" +
                                        ", text = '" + TrimText(Range_GetText(findRange), 512) + "'");
                                errorCount++;
                            }
                        }
                        break;
                    case FindResults.Null:
                        if (findRange != null)
                        {
                            if (key == TextPattern.TabsAttribute)
                            {
                                // This test condition failure is only really valid for this attribute
                                // if we have a control with a non-zero length array of tab marks
                                if (((double[])valueToFind).Length == 0)
                                    continue; // skip to next attribute
                            }
                            Comment("#######" + msg + " returned non-null range, expected null for attribute " +
                                    keyString +
                                    ", text = '" + TrimText(Range_GetText(findRange), 512) + "'");
                            errorCount++;
                        }
                        break;
                    default:
                        throw new ArgumentException("TS_FindAttributes() has no support for " + ParseType(FindResults));
                }
            }
            if (errorCount == 0)
                Comment("FindAttribute(...) returned the correct range (" + ParseType(FindResults) + ")");
            else
                ThrowMe(checkType, "FindAttribute(...) returned " + errorCount + " incorrect results");

            m_TestStep++;
        }

        //-------------------------------------------------------------------------------
        // TestStep for TextPatternRange.FindText Method
        //-------------------------------------------------------------------------------
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")] // FxCop doesn't recognize the check of searchText
        [SuppressMessage("Microsoft.Globalization", "CA130:UseOrdinalStringComparison", MessageId = "System.String.Compare(System.String,System.String,System.Boolean,System.Globalization.CultureInfo)")]
        internal void TS_FindText(string searchText, bool backward, bool ignoreCase, FindResults results, Type expectedException, CheckType checkType)
        {
            int actualResult = 0;  // contains flags for each "find result" that occurs
            string msg = "FindText(<<searchText>>," +
                        (backward == true ? "backward" : "forward") + "," +
                        (ignoreCase == true ? "ignore case" : "case-sensitive") + ")";
            TextPatternRange findRange = null;
            TextPatternRange documentRange = Pattern_DocumentRange(CheckType.Verification);

            //
            if (!CheckDocument(documentRange))
            {
                if (expectedException == null)
                    Library.ValidateArgumentNonNull(searchText, "searchText cannot be null");

                Range_FindText(documentRange, ref findRange, searchText, backward, ignoreCase, expectedException, checkType);

                // Identify actual range
                if (expectedException != null)
                    actualResult += (int)FindResults.Exception;

                if (findRange == null)
                    actualResult += (int)FindResults.Null;
                else
                {
                    int compareResult = 0;   // End point comparison
                    object val = true;
                    string findText = " ";   // Required for FXCop Rule CA1820
                    string allText = " "; // text for entire range.
                    TextPatternRange hiddenRange = null;

                    // Get text from FindRange, get text for entire document
                    Range_GetText(findRange, ref findText, -1, null, checkType);
                    Range_GetText(documentRange, ref allText, -1, null, checkType);
                    TrimTrailingCRLF(m_le, ref allText);
                    TrimTrailingCRLF(m_le, ref findText);
                    TrimTrailingCRLF(m_le, ref searchText);

                    if (string.Compare(searchText, findText, false, CultureInfo.InvariantCulture) == 0) // case sensitive
                        actualResult += (int)FindResults.MatchingRange;
                    else if (string.Compare(searchText, findText, true, CultureInfo.InvariantCulture) == 0) // case insensitive
                    {
                        actualResult += (int)FindResults.MatchingRangeCaseLess;
                    }

                    // Do start points match?
                    Range_CompareEndpoints(documentRange, TextPatternRangeEndpoint.Start, findRange, TextPatternRangeEndpoint.Start, ref compareResult, null, checkType);
                    if (compareResult == 0)
                        actualResult += (int)FindResults.MatchesFirst;

                    // Do end points (mostly) match?
                    Range_CompareEndpoints(documentRange, TextPatternRangeEndpoint.End, findRange, TextPatternRangeEndpoint.End, ref compareResult, null, checkType);
                    if (TextLibrary.IsRichEdit(m_le))
                    {
                        if ((compareResult >= 0) && (compareResult <= 2))
                            actualResult += (int)FindResults.MatchesLast;
                    }
                    else
                    {
                        if (compareResult == 0)
                            actualResult += (int)FindResults.MatchesLast;
                    }

                    if (findText.Length == 0)
                        actualResult += (int)FindResults.EmptyRange;

                    Range_FindAttribute(findRange, ref hiddenRange, TextPattern.IsHiddenAttribute, val, true, null, checkType);

                    if (hiddenRange == null)
                        actualResult += (int)FindResults.NoHiddenText;
                }

                if ((actualResult & ((int)results)) > 0)
                    Comment(msg + " returned " + Parse(results, expectedException));
                else
                    ThrowMe(checkType, msg + " failed to return " + Parse(results, expectedException));
                m_TestStep++;
            }
        }

        //---------------------------------------------------------------------------
        // TestSTep for TextPatternRange.GetAttributeValue
        //---------------------------------------------------------------------------
        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "System.ArgumentException.#ctor(System.String)")]
        internal void TS_GetAttributeValue(TextPatternRange callingRange, IDictionary attributes, AttributeType attribType, GetResult getResult, Type expectedException, CheckType checkType)
        {
            bool result = false;
            string key = "";
            object valueActual = null;
            object valueExpected = null;
            IDictionaryEnumerator enum1 = null;
            AutomationTextAttribute attrib = null;

            if (attribType == AttributeType.Null)
            {
                Range_GetAttributeValue(callingRange, null, ref valueActual, expectedException, checkType);
                return;
            }
            else
            {
                enum1 = attributes.GetEnumerator();

                for (int i = 0; i < attributes.Count; i++)
                {
                    // Get next item in dictionary
                    valueActual = null;
                    enum1.MoveNext();
                    attrib = (AutomationTextAttribute)enum1.Key;
                    key = Helpers.GetProgrammaticName(attrib);

                    valueExpected = enum1.Value;

                    Range_GetAttributeValue(callingRange, attrib, ref valueActual, expectedException, checkType);

                    switch (getResult)
                    {
                        case GetResult.Exception:
                            // If we get this far, exception was raised correctly
                            if (expectedException != null)
                                result = true;
                            else
                                result = false;
                            break;
                        case GetResult.MatchingAttributeValue: // verify result MATCHES expected attribute values
                            if (valueExpected.ToString() == valueActual.ToString())
                                result = true;
                            else
                                result = false;
                            break;
                        case GetResult.NotSupported:
                            if (valueActual == AutomationElement.NotSupported)
                            {
                                valueActual = "AutomationElement.NotSupported";
                                valueExpected = "AutomationElement.NotSupported";
                                result = true;
                            }
                            break;
                        case GetResult.Null:
                            if (valueActual == null)
                                result = true;
                            else
                                result = false;
                            break;
                        case GetResult.DocumentRange: // verify result matches ENTIRE TextPattern document
                        case GetResult.CallingRangeLength: // verify result matches LENGTH of calling range
                        case GetResult.Empty:  // verify result is EMPTY range
                        default:
                            throw new ArgumentException("TS_GetAttributeValue() has no support for " + ParseType(getResult));
                    }

                    // Start crafting message
                    Comment("GetAttributeValue(" + key + ") returned " +
                            (valueActual == null ? "null" : valueActual.ToString()) +
                            ", expected = " +
                            (valueExpected == null ? "null" : valueExpected.ToString()));
                    if (result != true)
                    {
                        ThrowMe(checkType, "GetAttributeValue did not return expected results, " +
                                Parse(getResult, expectedException));
                    }
                }
            }
            m_TestStep++;
        }

        //---------------------------------------------------------------------------
        // TestStep for TextPatternRange.GetAttributeValues Method
        //---------------------------------------------------------------------------
        internal void TS_GetAttributeValues(TextPatternRange range, out IDictionary dict, AttributeType attribType, CheckType checkType)
        {
            dict = null;

            if (attribType != AttributeType.Null)
                GetAttributeValues(range, out dict, attribType, checkType);
            else
                Comment("No dictionary needed for test sending NULL as argument to GetAttributeValue()");
            m_TestStep++;
        }

        //---------------------------------------------------------------------------
        // TestStep for TextPatternRange.GetBoundingRectangles Method
        //---------------------------------------------------------------------------
        internal void TS_GetBoundingRectangles(TextPatternRange callingRange, ref Rect[] boundRects, bool scrollIntoView, Type expectedException, CheckType checkType)
        {
            string text = "";
            TextPatternRange[] visibleRanges = null;

            Library.ValidateArgumentNonNull(callingRange, "callingRange cannot be null");

            if (scrollIntoView == true)
                Range_ScrollIntoView(callingRange, true, null, CheckType.Verification);

            TextLibrary.TrimRangeCRLF(m_le, callingRange);

            Range_GetBoundingRectangles(callingRange, ref boundRects, expectedException, checkType);
            Pattern_GetVisibleRanges(_pattern, ref visibleRanges, null, checkType);
            TextLibrary.TrimRangeCRLF(m_le, visibleRanges[0]);
            // Call GetText() on FIRST visible range (post V1, this should validate against each/all ranges)
            Range_GetText(visibleRanges[0], ref text, -1, null, checkType);
            Comment("GetText(-1) returned text of length " + text.Length);

            // if an empty range, then boundrects.length = 0 is okay
            if ((boundRects.Length == 0) && (text.Length > 0))
            {
                ThrowMe(checkType, "Expected non-zero array of bounding rectangles for non-zero length range (has range been scrolled into view correctly?)");
            }
            else
                Comment("TS_GetBoundingRectangles returned " + boundRects.Length + " bounding rectangles ");

            m_TestStep++;
        }

        //---------------------------------------------------------------------------
        // TestStep for TextPatternRange.GetBoundingRectangles Method (Lite Version)
        //---------------------------------------------------------------------------
        internal void TS_GetBoundingRectanglesLite(TextPatternRange callingRange, ref Rect[] boundRects, bool scrollIntoView, Type expectedException, CheckType checkType)
        {
            Library.ValidateArgumentNonNull(callingRange, "callingRange cannot be null");

            if (scrollIntoView == true)
                Range_ScrollIntoView(callingRange, true, null, CheckType.Verification);

            Range_GetBoundingRectangles(callingRange, ref boundRects, expectedException, checkType);

            m_TestStep++;
        }

        //---------------------------------------------------------------------------
        // TestStep for TextPatternRange.GetText Method
        //---------------------------------------------------------------------------
        internal void TS_GetText(TextPatternRange range, ref string text, int maxLength, Type expectedException, CheckType checkType)
        {
            Range_GetText(range, ref text, maxLength, expectedException, checkType);
            Comment("TS_GetText(" + maxLength + ") returned '" + TrimText(text, 512) + "'");
            m_TestStep++;
        }

        //---------------------------------------------------------------------------
        // TestStep for TextPatternRange.Move Method
        //---------------------------------------------------------------------------
        internal void TS_Move(TextPatternRange callingRange, TextUnit[] supportedTextUnits, int[] numberOfTextUnits, Count countEnum, Count expectedCountEnum, CheckType checkType)
        {
            int count         = 0;
            int expectedCount = 0;
            int errorCount    = 0;
            int actualCount   = 0;
            TextUnit         unit;
            TextPatternRange rangeToMove = null;

            for (unit = TextUnit.Character; unit <= TextUnit.Document; unit++)
            {
                // Only perform test for supported text units
                if (supportedTextUnits[(int)unit] == unit)
                {
                    Comment("");
                    Comment("TextUnit                    = " + Parse(unit));

                    // Clone range, so we always start with "pristine" range
                    Range_Clone(callingRange, ref rangeToMove, null, checkType);

                    count         = CountConvert(countEnum,         numberOfTextUnits[(int) unit]);
                    expectedCount = CountConvert(expectedCountEnum, numberOfTextUnits[(int) unit]);

                    // Move the range
                    Range_Move(rangeToMove, unit, count, ref actualCount, null, checkType);

                    Comment("Requested move count        = " + count);
                    Comment("Expected move count         = " + expectedCount);

                    if (actualCount == expectedCount)
                        Comment("Actual move count           = " + actualCount + " (as expected)");
                    else
                    {
                        errorCount++;
                        Comment("###### Actual move count    = " + actualCount + " WHICH DOES NOT EQUAL EXPECTED COUNT");
                    }
                }
                else
                {
                    Comment("");
                    Comment("TextUnit " + Parse(unit) + " not supported on this control");
                }
            }
            if (errorCount > 0)
                ThrowMe(checkType, errorCount + " errors while calling Move() method");
            m_TestStep++;
        }

        //---------------------------------------------------------------------------
        // TestStep for TextPatternRange.ExpandToEnclosingUnit Method
        // 
        // This method is destructive to the calling range we want to Expand. So
        // we clone it.  Then we craete a new range, and walk through the entire doc
        // Textunit-by-TextUnit until our "walking range" matches the expanded range
        // or we run out of text units.
        // If the entire doucment is empty, that is a special case that needs to be 
        // handled as well.
        // Special Case (of sorts): Calling ExpandToEnclosingUnit on an empty range
        // at the end of the document
        //---------------------------------------------------------------------------
        internal void TS_ExpandToEnclosingUnits(TextPatternRange callingRange, TextUnit[] supportedTextUnits, TextUnit expandUnit, int upperBound, CheckType checkType)
        {
            int                 iterations    = -5; // This is sanity "last check" to make sure we don't
            int                 actualCount   = 0;
            int                 startOffset   = 0;
            int                 endOffset     = 0;
            bool                foundMatch    = false;
            string              walkingText   = "";
            string              expandedText  = "";
            TextPatternRange    walkingRange  = null;
            TextPatternRange    expandedRange = null;
            TextPatternRange    documentRange = null;

            Comment("---------------------------ExpandToEnclosingUnit(" + Parse(expandUnit) + ")---------------------------");

            documentRange = Pattern_DocumentRange(checkType);

            // Create range to expand
            Range_Clone(callingRange, ref expandedRange, null, checkType);

            // Expand "range to expand" to target text unit
            Range_ExpandToEnclosingUnit(expandedRange, expandUnit, null, checkType);
            Range_GetText(expandedRange, ref expandedText, -1, null, checkType);

            // Do we have an empty string? THis could be okay, or it could be error
            if (expandedText.Length == 0)
            {
                // Is our "expanded range" actually empty?
                bool isEmptyRange = IsEmptyRange( expandedRange, checkType );

                // Is our "expanded range" actually at the end of the document
                Range_CompareEndpoints(expandedRange, TextPatternRangeEndpoint.Start, documentRange,   TextPatternRangeEndpoint.End, ref startOffset, null, checkType);
                Range_CompareEndpoints(expandedRange, TextPatternRangeEndpoint.End,   documentRange,   TextPatternRangeEndpoint.End, ref endOffset,   null, checkType);

                if ((isEmptyRange == true) && (startOffset == _trailingCRLFCount) && (endOffset == _trailingCRLFCount))
                {
                    Comment("ExpandToEnclosingUnit() on empty range at end of doc correctly returned empty document");
                    return;
                }
                else
                {
                    Comment("Expanded range is empty                          = " + isEmptyRange);
                    Comment("Expanded range.Start offset from end of document = " + startOffset);
                    Comment("Expanded range.End   offset from end of document = " + endOffset);
                    ThrowMe(checkType, "Unexpected empty range after ExpandToEnclosingUnit()");
                }
            }


            // Verification: Create walking range (new empty range) at start of doc
            Comment("Creating 'walking range' to validate expanded range against'");
            CreateRange(out walkingRange, TargetRangeType.EmptyStart, null, checkType);
            Range_MoveEndpointByUnit(walkingRange, TextPatternRangeEndpoint.End, expandUnit, 1, ref actualCount, null, checkType);
            do
            {
                iterations++;
            
                Range_GetText(walkingRange, ref walkingText, -1, null, checkType);
                Comment("Expanded range = '" + TrimText(expandedText, 512) + "'");
                Comment("Walking range  = '" + TrimText(walkingText, 512) + "'");
                
                // Comapre expanded text unit against current text unit in walking range
                Range_CompareEndpoints(expandedRange, TextPatternRangeEndpoint.Start, walkingRange, TextPatternRangeEndpoint.Start, ref startOffset, null, checkType);
                Range_CompareEndpoints(expandedRange, TextPatternRangeEndpoint.End,   walkingRange, TextPatternRangeEndpoint.End,   ref endOffset,   null, checkType);

                if ((startOffset == 0) && (endOffset == 0))
                {
                    foundMatch = true;
                    break;
                }

                // Move to next text unit
                Range_MoveEndpointByUnit(walkingRange, TextPatternRangeEndpoint.Start, expandUnit, 1, ref actualCount, null, checkType);
                Range_MoveEndpointByUnit(walkingRange, TextPatternRangeEndpoint.End,   expandUnit, 1, ref actualCount, null, checkType);

                // now compare current text unit in walking range against the document
                Range_CompareEndpoints(Pattern_DocumentRange(checkType), TextPatternRangeEndpoint.End, walkingRange, TextPatternRangeEndpoint.End, ref endOffset, null, checkType);
            }
            while((endOffset != 0) && (iterations <= upperBound));

            if (IsEmptyRange(Pattern_DocumentRange(checkType), checkType))
                foundMatch = true; // Special case


            // This assumes the calling range isn't a size greater than the text unit we expanded to
            if (foundMatch == true)
                Comment("ExpandToEnclosingUnit(" + Parse(expandUnit) + ") successfully expanded to entire text unit");
            else
            {
                if( iterations >= upperBound )
                {
                    Comment( "ExpandToEnclosingUnit() test dropped out of possible infinite loop.");
                    Comment( "    Expected Upper Bound of Iterations: " + upperBound );
                    Comment( "    Actuall # of iterations:            " + iterations );
                    
                }

                ThrowMe( checkType, "***Unable to validate ExpandToEnclosingUnit actually matched a complete textunit");
            }

            m_TestStep++;
        }

        //---------------------------------------------------------------------------
        // TestStep for TextPatternRange.ScrollIntoView Method
        //---------------------------------------------------------------------------
        internal void TS_ScrollIntoView(TextPatternRange range, bool alignToTop, Type expectedException, CheckType checkType)
        {
            Range_ScrollIntoView(range, alignToTop, expectedException, checkType);
            m_TestStep++;
        }

        //---------------------------------------------------------------------------
        // TestStep for TextPatternRange.GetEnclosingElement Method
        //---------------------------------------------------------------------------
        internal void TS_GetEnclosingElement(TextPatternRange range, ref AutomationElement parent, Type expectedException, CheckType checkType)
        {
            Range_GetEnclosingElement(range, ref parent, expectedException, checkType);
            Comment("TS_GetEnclosingElement() returned " + parent.ToString());
            m_TestStep++;
        }

        //---------------------------------------------------------------------------
        // TestStep for TextPatternRange.AddToSelection Method
        //---------------------------------------------------------------------------
        internal void TS_AddToSelection(TextPatternRange range, Type expectedException, CheckType checkType)
        {
            Range_AddToSelection(range, expectedException, checkType);
            m_TestStep++;
        }

        //---------------------------------------------------------------------------
        // TestStep for TextPatternRange.RemoveFromSelection Method
        //---------------------------------------------------------------------------
        internal void TS_RemoveFromSelection(TextPatternRange range, Type expectedException, CheckType checkType)
        {
            Range_RemoveFromSelection(range, expectedException, checkType);
            m_TestStep++;
        }

        #endregion TextPatternRange Method TestSteps

        #endregion TextPatternRange TestSteps

        #region Utility TestSteps

        //---------------------------------------------------------------------------
        // Calculate the max length to use when calling GetText()
        //---------------------------------------------------------------------------
        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "System.ArgumentException.#ctor(System.String)")]
        static internal void TS_CalcMaxLength(MaxLength lengthType, out int maxLength, string actualText)
        {
            maxLength = actualText.Length; // default to actual length

            // Calculate max length
            switch (lengthType)
            {
                case MaxLength.All:                         // -1 (all) for calling range
                    maxLength = -1;
                    break;
                case MaxLength.Zero:                        // 0 for text length
                    maxLength = 0;
                    break;
                case MaxLength.MinusTwo:                    // -2 (error) for text length)
                    maxLength = -2;
                    break;
                case MaxLength.Length:                      // actual length of calling range
                    maxLength = actualText.Length;
                    break;
                case MaxLength.RandomWithinValidSize:       // RANDOM size <= actual size
                    maxLength = (int)Helpers.RandomValue(1, maxLength);
                    break;
                case MaxLength.RandomOutsideValidSize:      // RANDOM size > actual size
                    maxLength = (int)Helpers.RandomValue(maxLength + 3, Int32.MaxValue);
                    break;
                case MaxLength.MaxInt:                      // max Integer value
                    maxLength = Int32.MaxValue;
                    break;
                case MaxLength.NegativeMaxInt:                   // negative max Integer value
                    maxLength = Int32.MinValue;
                    break;
                case MaxLength.One:                         // 1 (single chracter) for text length
                    maxLength = 1;
                    break;
                default:
                    throw new ArgumentException("TS_CalcMaxLength() has no support for " + ParseType(lengthType));
            }

            Comment("maxLength value to use in GetText() is = " + maxLength);
            m_TestStep++;
        }


        //---------------------------------------------------------------------------
        // Verify that range returned is within the range we created
        //---------------------------------------------------------------------------
        internal void TS_CompareRanges(TextPatternRange callingRange, TextPatternRange rangeFromPt, RangeCompare compareResults, Type expectedException, CheckType checkType)
        {
            int actualResult = 0;
            int docStart = 0; // offset of range compared to document start
            int docEnd = 0; // offset of range compared to document end
            int compareStart = 0; // offset of range compared to callingRange start
            int compareEnd = 0; // offset of range compared to callingRange end
            TextPatternRange documentRange = Pattern_DocumentRange(CheckType.Verification);

            Comment("Validating that RangeFromPoint return value is " + Parse(compareResults));

            // Sanity check
            if (callingRange == null)
                throw new ArgumentNullException("TS_CompareRanges requires non-null TextPatternRange");

            // The earlier call to Pattern_GetRangeFromPoint() will validate expecting exception/not getting one
            // But if we did get one/expected it, then "range" is null
            if (expectedException != null)
                actualResult += (int)RangeCompare.Exception;
            else
            {
                // Evaluate what actual results are
                Range_CompareEndpoints(documentRange, TextPatternRangeEndpoint.Start, rangeFromPt, TextPatternRangeEndpoint.Start, ref docStart, null, checkType);
                Range_CompareEndpoints(documentRange, TextPatternRangeEndpoint.End, rangeFromPt, TextPatternRangeEndpoint.End, ref docEnd, null, checkType);
                Range_CompareEndpoints(callingRange, TextPatternRangeEndpoint.Start, rangeFromPt, TextPatternRangeEndpoint.Start, ref compareStart, null, checkType);
                Range_CompareEndpoints(callingRange, TextPatternRangeEndpoint.End, rangeFromPt, TextPatternRangeEndpoint.End, ref compareEnd, null, checkType);

                if ((docStart <= 0) && (docEnd >= 0))
                    actualResult += (int)RangeCompare.WithinDocRange;  // likely this will always be true

                if ((compareStart <= 0) && (compareEnd >= 0))
                    actualResult += (int)RangeCompare.Contained;

                if (IsEmptyRange(rangeFromPt, checkType))
                    actualResult += (int)RangeCompare.Empty;

                if ((compareStart == 0) && (compareEnd == 0))
                    actualResult += (int)RangeCompare.Equal;
            }

            // Good to go???                
            if ((actualResult & ((int)compareResults)) > 0)
                Comment("RangeFromPoint returned expected range");
            else
                ThrowMe(checkType, "RangeFromPoint failed to return expected range " + ParseType(compareResults));
            m_TestStep++;
        }

        //---------------------------------------------------------------------------
        // Count text units
        //---------------------------------------------------------------------------
        internal void TS_CountTextUnits(TextPatternRange rangeToCount, TextUnit[] supportedTextUnits, ref int[] numberOfTextUnits)
        {
            TextLibraryCount.CountTextUnitsInRange(rangeToCount, supportedTextUnits, ref numberOfTextUnits);
            m_TestStep++;
        }

        //---------------------------------------------------------------------------
        // Create a degenerate range within pattern, requires non-zero length pattern
        //---------------------------------------------------------------------------
        internal void TS_CreateEmptyRange(out TextPatternRange range, RangeLocation rangeLoc, CheckType checkType)
        {
            CreateEmptyRange(out range, rangeLoc, checkType);
            m_TestStep++;
        }

        //---------------------------------------------------------------------------
        // Create valid point within one of the bounding rects  
        //---------------------------------------------------------------------------
        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "System.ArgumentException.#ctor(System.String)")]
        internal void TS_CreatePoint(ref TextPatternRange range, Rect[] boundRects, out Point screenLocation, BoundingRectangleLocation boundRectLoc, CheckType checkType)
        {
            int rectIdx = 0;
            Rect autoElementRect = new Rect();
            Rect[] tempRects = new Rect[0];
            TextPatternRange documentRange = Pattern_DocumentRange(CheckType.Verification);

            screenLocation = new Point();

            // Sanity check
            Library.ValidateArgumentNonNull(range, "range argument cannot be null");

            if ((boundRects.Length == 0) && (boundRectLoc != BoundingRectangleLocation.OutsideAutomationElement))
            {
                throw new ArgumentException("TS_CreatePoint requires non-empty array of bounding rectangles");
            }

            // Finally, generate the point!
            switch (boundRectLoc)
            {
                case BoundingRectangleLocation.InsideTopLeft:
                    rectIdx = 0;
                    screenLocation.X = boundRects[rectIdx].Left + 1;
                    screenLocation.Y = boundRects[rectIdx].Top + 1;
                    break;
                case BoundingRectangleLocation.Middle:
                    screenLocation.X = (boundRects[rectIdx].Left + boundRects[rectIdx].Right) / 2;
                    screenLocation.Y = (boundRects[rectIdx].Top + boundRects[rectIdx].Bottom) / 2;
                    break;
                case BoundingRectangleLocation.InsideBottomRight:
                    rectIdx = boundRects.Length - 1;
                    screenLocation.X = boundRects[rectIdx].Right - 1;
                    screenLocation.Y = boundRects[rectIdx].Bottom - 1;
                    break;
                case BoundingRectangleLocation.OutsideBottomRight:
                    rectIdx = boundRects.Length - 1;
                    screenLocation.X = boundRects[rectIdx].Right + 1;
                    screenLocation.Y = boundRects[rectIdx].Bottom + 1;
                    break;
                case BoundingRectangleLocation.OutsideTopLeft:
                    rectIdx = 0;
                    screenLocation.X = boundRects[rectIdx].Left - 1;
                    screenLocation.Y = boundRects[rectIdx].Top - 1;
                    break;
                case BoundingRectangleLocation.OutsideAutomationElement:
                    // Get automation element bounding rectangle
                    GetAutomationElementBoundingRectangle(out autoElementRect);
                    screenLocation.X = autoElementRect.Left - 1;
                    screenLocation.Y = autoElementRect.Top - 1;
                    break;
                case BoundingRectangleLocation.FirstChar:
                    tempRects = null;
                    Range_GetBoundingRectangles(documentRange, ref tempRects, null, checkType);
                    if (tempRects.Length == 0)
                        ThrowMe(checkType, "TS_CreatePoint expects non-empy bounding rectangles array for document");
                    screenLocation.X = tempRects[0].Left + 1; // essentially top-left of first rect
                    screenLocation.Y = tempRects[0].Top + 1;
                    break;
                case BoundingRectangleLocation.FirstCharInRange:
                    rectIdx = 0;
                    screenLocation.X = boundRects[0].Left + 1; // essentially top-left of first rect
                    screenLocation.Y = boundRects[0].Top + 1;
                    break;
                case BoundingRectangleLocation.LastCharInRange:
                    rectIdx = boundRects.Length - 1;
                    screenLocation.X = boundRects[rectIdx].Right - 1;   // essentially bottom-right of last rect
                    screenLocation.Y = boundRects[rectIdx].Bottom - 1;
                    break;
                default:
                    throw new ArgumentException("TS_CreatePoint() has no support for " + ParseType(boundRectLoc));
            }

            Comment("Created Point (" + screenLocation.ToString(CultureInfo.InvariantCulture) + ") at " +
                    Parse(boundRectLoc) +
                    " relative to boundRect " +
                    (boundRectLoc != BoundingRectangleLocation.OutsideAutomationElement ?
                        boundRects[rectIdx].ToString(CultureInfo.InvariantCulture) :
                        autoElementRect.ToString(CultureInfo.InvariantCulture)));
            m_TestStep++;

        }

        //---------------------------------------------------------------------------
        // TS_CreateRange, has no support for returning offsets
        //---------------------------------------------------------------------------
        internal void TS_CreateRange(out TextPatternRange range, TargetRangeType rangeType, TextPatternRange rangeToClone, bool trimRange, CheckType checkType)
        {
            int startOffset = 0;
            int endOffset = 0;

            CreateRange(out range, rangeType, out startOffset, out endOffset, rangeToClone, checkType);

            if ((trimRange == true) && (range != null))
                TextLibrary.TrimRangeCRLF(m_le, range);

            m_TestStep++;
        }
        
        //---------------------------------------------------------------------------
        // Enumerate through child automation elements
        //---------------------------------------------------------------------------
        internal void TS_EnumerateChildren(AutomationElement[] autoElements, bool allowNull, Type expectedException, CheckType checkType)
        {
            TextPatternRange range = null;

            // special case: do we call RangeFromChild on a null automation element???
            if ((allowNull == true) && (autoElements == null))
            {
                // If we're expecting an exception, its okay
                if (expectedException != null)
                {
                    Pattern_RangeFromChild(_pattern, ref range, null, expectedException, checkType);
                    m_TestStep++;
                    return;
                }
                else
                {
                    ThrowMe(checkType, "Expecting non-null automation element(s) for calls to RangeFromChild()");
                }
            }

            Comment("There are " + autoElements.Length + " automation elements to get range from");
            for (int i = 0; i < autoElements.Length; i++)
            {
                int []        runtimeID   = new int[0];
                StringBuilder elementInfo = new StringBuilder();

                // Now collect info about our automation element--bare minimum, eh?
                elementInfo.Append("Automation Element #" + i.ToString() + " Name = " + autoElements[i].Current.Name + ", Runtime ID = ");
                runtimeID = autoElements[i].GetRuntimeId();
                for (int j = 0; j < runtimeID.Length; j++)
                {
                    elementInfo.Append(runtimeID[j].ToString() + ((j < (runtimeID.Length - 1)) ? ":" : " "));
                }
                elementInfo.Append(", ClassName = " + TextLibrary.GetClassName(autoElements[i]));
                Comment(elementInfo.ToString());

                // Call range from child (finally!)
                Pattern_RangeFromChild(_pattern, ref range, autoElements[i], expectedException, checkType);

                // if we didn't expect an exception, range should not be null
                if ((expectedException == null) && (range == null))
                    ThrowMe(checkType, "RangeFromChild should never return null, but this automation element returned a null range");
            }
            m_TestStep++;
        }

        //---------------------------------------------------------------------------
        // Moves the end point of a range by a specified # of units/count
        //---------------------------------------------------------------------------
        internal void TS_ExpandRange(ref TextPatternRange callingRange, TextPatternRangeEndpoint endPoint, TextUnit unit, int expectedCount, bool mustMatch, CheckType checkType)
        {
            int actualCount;

            ExpandRange(callingRange, endPoint, unit, expectedCount, out actualCount, mustMatch, checkType);

            Comment("Successfully expanded range by " + actualCount + " " + Parse(unit) + " units");
            m_TestStep++;
        }

        //---------------------------------------------------------------------------
        // Get specific Automation Element
        //---------------------------------------------------------------------------
        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "System.ArgumentException.#ctor(System.String)")]
        internal void TS_GetAutomationElement(ref AutomationElement[] autoElements, AutoElementType autoElementArgument, CheckType checkType)
        {
            TextPatternRange documentRange = Pattern_DocumentRange(CheckType.Verification);

            switch (autoElementArgument)
            {
                case AutoElementType.DifferentFromPattern:
                    autoElements = new AutomationElement[1];
                    autoElements[0] = TreeWalker.ControlViewWalker.GetParent(m_le);
                    if (autoElements[0] == null)
                        ThrowMe(checkType, "Unable to get parent AutomationElement of TextPattern's AutomationElement");
                    break;
                case AutoElementType.Null:
                    autoElements = null;
                    break;
                case AutoElementType.SameAsPattern:
                    autoElements = new AutomationElement[1];
                    autoElements[0] = m_le;
                    break;
                case AutoElementType.UseChildren:
                    // Assumption: we don't get to this point without verifying there *ARE* children
                    Range_GetChildren(documentRange, ref autoElements, checkType);
                    break;
                default:
                    throw new ArgumentException("TS_GetAutomationElement() has no support for " + ParseType(autoElementArgument));
            }

            Comment("Acquired automation element(s) " + Parse(autoElementArgument));
            m_TestStep++;
        }

        //---------------------------------------------------------------------------
        // Get automation element bounding rectangle
        //---------------------------------------------------------------------------
        internal void TS_GetAutomationElementBoundingRectangle(out Rect boundingRect)
        {
            GetAutomationElementBoundingRectangle(out boundingRect);
            m_TestStep++;
        }

        //---------------------------------------------------------------------------
        // Gets range for selected text and returns text for that range
        // NOTE: This method assumes only a single range is selected, and will
        //       not work correctly if multiple selections have taken place
        //       Win32/DUI/Avalon do not support multiple selection.
        //---------------------------------------------------------------------------
        internal void TS_GetTextSelection(ref string actualText, CheckType checkType)
        {
            TextPatternRange[] selectedRanges = null;

            Pattern_GetSelection(_pattern, ref selectedRanges, null, checkType);
            Range_GetText(selectedRanges[0], ref actualText, -1, null, checkType);

            Comment("Returned actual text = " + actualText);
            m_TestStep++;
        }

        //---------------------------------------------------------------------------
        // Test step to identify supported units
        //---------------------------------------------------------------------------
        internal void TS_IdentifySupportedTextUnits(ref TextUnit[] supportedTextUnits)
        {
            IdentifySupportedTextUnits(ref supportedTextUnits);

            // Report mappings
            for (int i = (int)TextUnit.Character; i <= (int)TextUnit.Document; i++)
            {
                if (((int)supportedTextUnits[i]) != i)
                    Comment(ParseType((TextUnit)i) + " maps to TextUnit " + ParseType((TextUnit)supportedTextUnits[i]));
            }

            Comment("Identified supported text units for control");
            m_TestStep++;
        }

        //---------------------------------------------------------------------------
        // Destructive mechanism to validate if control is single or multi-line txt
        //---------------------------------------------------------------------------
        internal void TS_IsMultiLine(bool isMultiLineExpected, CheckType checkType)
        {
            string msg = "<<UNKNOWN>>";

            // Determine supported text units
            bool results = TextLibrary.IsMultiLine(m_le);

            if (results == true)
            {
                if (isMultiLineExpected == false)
                    ThrowMe(checkType, "Expected single-line control, control is acutally multi-line");

                msg = "multi-line text";

                if (isMultiLineExpected == true)
                    msg += ", as expected";
            }
            else if (results == false)
            {
                if (isMultiLineExpected == true)
                    ThrowMe(checkType, "Expected multi-line control, control is acutally single-line");

                msg = "single-line text";

                if (isMultiLineExpected == false)
                    msg += ", as expected";
            }
            else
                ThrowMe(checkType, "Return value for   IsMultiLine = " + results.ToString() + ", expected either True or False");

            Comment("Control supports " + msg);
            m_TestStep++;
        }

        //---------------------------------------------------------------------------
        // Validate matching children
        //---------------------------------------------------------------------------
        static internal void TS_IsMatchingChildren(TextPatternRange targetRange, AutomationElement[] childrenActual, bool expectMatch, CheckType checkType)
        {
            AutomationElement[] childrenClone = null;

            Range_GetChildren(targetRange, ref childrenClone, checkType);
            IsMatchingChildren(childrenActual, childrenClone, expectMatch);
            m_TestStep++;
        }

        //---------------------------------------------------------------------------
        // Validate matching Attribute dictionary
        //---------------------------------------------------------------------------
        internal void TS_IsMatchingDictionary(TextPatternRange targetRange, IDictionary attribsActual, AttributeType attribType, bool expectMatch, CheckType checkType)
        {
            IDictionary attribsClone = null;

            GetAttributeValues(targetRange, out attribsClone, attribType, checkType);
            IsMatchingDictionary(attribsActual, attribsClone, expectMatch);
            m_TestStep++;
        }

        //---------------------------------------------------------------------------
        // Validate matching Get Enclosing element
        //---------------------------------------------------------------------------
        internal void TS_IsMatchingEnclosingElement(TextPatternRange targetRange, AutomationElement enclosingActual, bool expectMatch, CheckType checkType)
        {
            AutomationElement enclosingClone = null;

            Range_GetEnclosingElement(targetRange, ref enclosingClone, null, checkType);
            IsMatchingAutomationElement(enclosingActual, enclosingClone, expectMatch);
            m_TestStep++;
        }

        //---------------------------------------------------------------------------
        // Validate that two ranges are equal
        //---------------------------------------------------------------------------
        internal void TS_IsMatchingRange(TextPatternRange range, TextPatternRange cloneRange, CheckType checkType)
        {
            bool isEqual = false;

            Range_Compare(range, cloneRange, ref isEqual, null, checkType);

            if (isEqual == true)
            {
                Comment("Cloned range is identical to original range");
            }
            else
            {
                ThrowMe(
                            checkType, "Compare() indicates that cloned range (incorrectly) does not equal the original range");
            }
            m_TestStep++;
        }

        //---------------------------------------------------------------------------
        // Do child automation elements matching the calling range?
        //---------------------------------------------------------------------------
        static internal void TS_IsMatchingRangeChildren(TextPatternRange callingRange, TextPatternRange cloneRange, CheckType checkType)
        {
            int misMatch = 0;

            if (callingRange.GetChildren().Length != cloneRange.GetChildren().Length)
            {
                Comment("Calling range has " + callingRange.GetChildren().Length + " child elements");
                Comment("Cloned  range has " + cloneRange.GetChildren().Length + " child elements");
                ThrowMe(checkType, "Cloned range has mis-matched # of child automation elements from calling range");
            }

            for (int i = 0; i < callingRange.GetChildren().Length; i++)
            {
                if (callingRange.GetChildren()[i] != cloneRange.GetChildren()[i])
                {
                    misMatch++;
                    Comment("Mismatched child elements[" + i + "]");
                    Comment("   calling range child element " + i + " runtime id = " + callingRange.GetChildren()[i].GetRuntimeId().ToString());
                    Comment("   cloned  range child element " + i + " runtime id = " + cloneRange.GetChildren()[i].GetRuntimeId().ToString());
                }
                else
                    Comment("Child elements [" + i + "] match");
            }

            if (misMatch > 0)
                ThrowMe(checkType, misMatch + "/" + callingRange.GetChildren().Length + " mis-matched child elements, should be no mismatches");
            else
                Comment("All child elements match in both calling and cloned ranges");
            m_TestStep++;
        }

        //---------------------------------------------------------------------------
        // Validate matching text via calling GetText
        //---------------------------------------------------------------------------
        internal void TS_IsMatchingText(string methodTested, TextPatternRange targetRange, string expectedText, bool expectMatch, bool allowSubstrings, CheckType checkType)
        {
            string actualText = "";

            Range_GetText(targetRange, ref actualText, -1, null, checkType);

            Comment("Used " + methodTested + "() to acquire actual text");
            Comment("Actual   text = '" + TrimText(actualText, 512) + "'");
            Comment("Expected text = '" + TrimText(expectedText, 512) + "'");

            IsMatchingText(actualText, expectedText, expectMatch, allowSubstrings, checkType);
            m_TestStep++;
        }

        //---------------------------------------------------------------------------
        // Validate matching text
        //---------------------------------------------------------------------------
        static internal void TS_IsMatchingText(string methodTested, string actualText, string expectedText, bool allowSubstrings, CheckType checkType)
        {
            Comment("Used to " + methodTested + "() to acquire actual text");
            Comment("Actual   text = '" + TrimText(actualText, 512) + "'");
            Comment("Expected text = '" + TrimText(expectedText, 512) + "'");

            // By default, we expeta  match
            IsMatchingText(actualText, expectedText, true, allowSubstrings, checkType);
            m_TestStep++;
        }

        //---------------------------------------------------------------------------
        // TestStep for TextPatternRange.TextPattern property
        //---------------------------------------------------------------------------
        static internal void TS_IsMatchingTextPattern(TextPattern _pattern, TextPattern newPattern, CheckType checkType)
        {
            if (ReferenceEquals(_pattern, newPattern) == true)
                Comment("TextPattern property matches expected instance of TextPattern class");
            else
                ThrowMe(
                        checkType, "TextPattern property does not match expected instance of TextPattern class");
            m_TestStep++;
        }

        //---------------------------------------------------------------------------
        // Compares the start and end points of two ranges to determine if they are identical
        //---------------------------------------------------------------------------
        static internal void TS_IsMatchingEndPoints(TextPatternRange range, TextPatternRange targetRange, bool expectMatch, CheckType checkType)
        {
            bool actualMatch;
            int startPoint;
            int endPoint;
            string expectMatchString = (expectMatch == true ? "matches" : "doesn't match");

            // Get start/end point offsets
            startPoint = range.CompareEndpoints(TextPatternRangeEndpoint.Start, targetRange,
                                                 TextPatternRangeEndpoint.Start);
            endPoint = range.CompareEndpoints(TextPatternRangeEndpoint.End, targetRange,
                                              TextPatternRangeEndpoint.End);

            // if the ranges Match than the return value of both calls to CompareEndPoints = 0
            actualMatch = ((startPoint == 0) && (endPoint == 0));

            Comment("Start point offset = " + startPoint + ", End point offset = " + endPoint + ", expected " +
                     (expectMatch == true
                        ? "non-zero"
                        : "zero") +
                     " offsets");

            // Report results
            if (actualMatch == expectMatch)
                Comment("Start/End point(s) " + expectMatchString + ", as expected");
            else
            {
                ThrowMe(checkType, "Start/End point(s) " + expectMatchString + ", which is not expected result");
            }
            m_TestStep++;
        }

        //---------------------------------------------------------------------------
        // Using keyboard input, select one character
        //---------------------------------------------------------------------------
        static internal void TS_KeyboardMove(bool backward)
        {
            if (backward == false)
            {
                TextLibrary.SendOneKey(System.Windows.Input.Key.Home, "Home", 300);
                TextLibrary.SendTwoKeys(System.Windows.Input.Key.LeftShift, System.Windows.Input.Key.Right, "Right One Key", 300);
            }
            else
            {
                TextLibrary.SendOneKey(System.Windows.Input.Key.End, "End", 300);
                TextLibrary.SendTwoKeys(System.Windows.Input.Key.LeftShift, System.Windows.Input.Key.Left, "Left One Key", 300);
            }

            Comment("Selected one character using keyboard input");
            m_TestStep++;
        }

        //---------------------------------------------------------------------------
        // Move a range's endpoint(s) relative to position of endpoint in another range
        //---------------------------------------------------------------------------
        internal void TS_MoveEndpointByRange(TextPatternRange callingRange, TextPatternRangeEndpoint endPoint, TextPatternRange targetRange, TextPatternRangeEndpoint targetEndPoint, MoveEPResults expectedResult, Type expectedException, CheckType checkType)
        {
            string msg = "MoveEndpointByRange(" + endPoint + ",targetRange," + targetEndPoint + ") returned ";
            int moveCount = 0;
            int actualResult = 0;
            TextPatternRange cloneOfRange = null;

            Comment("Validating expected result is " + Parse(expectedResult, expectedException));

            if (callingRange != null)
                Range_Clone(callingRange, ref cloneOfRange, null, checkType);

            // Move endPoint
            Range_MoveEndpointByRange(cloneOfRange, endPoint, targetRange, targetEndPoint, expectedException, checkType);

            if (expectedException != null)
                return; // we're done

            // verify location of end point that we moved...
            Range_CompareEndpoints(cloneOfRange, endPoint, targetRange, targetEndPoint, ref moveCount, null, checkType);

            // Evaluate the state of the callingRange
            if (expectedException != null)
                actualResult += (int)MoveEPResults.Exception;
            else
            {
                if (IsEmptyRange(cloneOfRange, checkType) == true)
                    actualResult += (int)MoveEPResults.EmptyRange;
                else
                    actualResult += (int)MoveEPResults.NonemptyRange;
            }

            // Were we a success? expected result and correct endpoint location
            if (((actualResult & ((int)expectedResult)) > 0) && (moveCount == 0))
                Comment(msg + Parse(expectedResult, expectedException) + " as expected");
            else
                ThrowMe(
                        checkType, msg + Parse(expectedResult, null) + ", expected " + Parse(expectedResult, expectedException));

            m_TestStep++;
        }

        //---------------------------------------------------------------------------
        // Calls MOveEndPointByUnit(range,TextUnit,count) and validates expected / actual return value
        //---------------------------------------------------------------------------
        internal void TS_MoveEndpointByUnitAndValidate(TextPatternRange callingRange, TextPatternRangeEndpoint endPoint, int count, int expectedCount, int richEditOffset, CheckType checkType)
        {
            int actualCount = 0;
            int errorCount = 0;
            TextUnit unit;
            TextPatternRange rangeToMove = null;

            for (unit = TextUnit.Character; unit <= TextUnit.Document; unit++)
            {
                // 



                if ((unit == TextUnit.Format || unit == TextUnit.Line) || count == -1)
                {
                    continue;
                }
                else
                {
                    // Clone range
                    Range_Clone(callingRange, ref rangeToMove, null, checkType);

                    Range_MoveEndpointByUnit(rangeToMove, endPoint, unit, count, ref actualCount, null, checkType);

                    if (actualCount == expectedCount)
                        Comment("MoveEndpointByUnit(" + endPoint + "," + unit + "," + count + ") moved the range by " + actualCount + ", as expected");
                    else if ((richEditOffset != 0)
                          && ((actualCount + richEditOffset) == expectedCount))
                    {
                        Comment("MoveEndpointByUnit(" + endPoint + "," + unit + "," + count + ") moved the range by " + actualCount + ", as expected taking into account extra characters added by RichEdit");
                    }
                    else
                    {
                        Comment("### MoveEndpointByUnit(" + endPoint + "," + unit + "," + count + ") moved the range by " + actualCount + ", expected " + expectedCount);
                        errorCount++;
                    }
                }
            }
            if (errorCount > 0)
                ThrowMe(checkType, errorCount + " errors while calling MoveEndpointByUnit() method");

            m_TestStep++;
        }

        //---------------------------------------------------------------------------
        // Calls MOveEndPointByUnit2(range,TextUnit,count) and validates expected / actual return value
        //---------------------------------------------------------------------------
        internal void TS_MoveEndpointByUnitAndValidate2(TextPatternRange callingRange, TextPatternRangeEndpoint endPoint, Count requestedCountEnum, Count expectedCountEnum, int[] countOfTextUnits, int richEditOffset, CheckType checkType)
        {
            int requestedCount = 0;
            int expectedCount  = 0;
            int actualCount    = 0;
            int errorCount = 0;
            TextPatternRange rangeToMove = null;

            for( TextUnit unit = TextUnit.Character; unit <= TextUnit.Document; unit ++ )
            {
                requestedCount =  CountConvert(requestedCountEnum, countOfTextUnits[(int) unit]);
                expectedCount  =  CountConvert(expectedCountEnum,  countOfTextUnits[(int) unit]);

                // Clone range
                Range_Clone(callingRange, ref rangeToMove, null, checkType);

                Range_MoveEndpointByUnit(rangeToMove, endPoint, unit, requestedCount, ref actualCount, null, checkType);

                if (actualCount == expectedCount)
                    Comment("MoveEndpointByUnit(" + endPoint + "," + unit + "," + requestedCount + ") moved the range by " + actualCount + ", as expected");
                else if ((richEditOffset != 0)
                      && ((actualCount + richEditOffset) == expectedCount))
                {
                    Comment("MoveEndpointByUnit(" + endPoint + "," + unit + "," + requestedCount + ") moved the range by " + actualCount + ", as expected taking into account extra characters added by RichEdit");
                }
                else
                {
                    Comment("### MoveEndpointByUnit(" + endPoint + "," + unit + "," + requestedCount + ") moved the range by " + actualCount + ", expected " + expectedCount);
                    errorCount++;
                }
            }
            if (errorCount > 0)
                ThrowMe(checkType, errorCount + " errors while calling MoveEndpointByUnit() method");

            m_TestStep++;
        }

        //---------------------------------------------------------------------------
        // Calls move N+1 times and validates results
        // First N times should equal expected result
        // N+1'th time should return 0
        // Unit is the actual unit we're going to call move with.
        //---------------------------------------------------------------------------
        internal void TS_MoveNTimes(TextUnit unit, int[] numberOfTextUnits, TextPatternRange range, int moveCount, int expectedResult, CheckType checkType)
        {
            int count = 0;
            int countOffset = 0;
            int actualResult = 0;
            bool isRichEdit = TextLibrary.IsRichEdit(m_le);
            string msg = "Move(" + unit.ToString() + ", " + moveCount + ")";
            TextPatternRange callingRange = null;

            // Get document range and clone calling range
            Range_Clone(range, ref callingRange, null, checkType);

            // Get count of type
            count = numberOfTextUnits[(int)unit];

            if (isRichEdit == true)    // 
            {
                if (unit == TextUnit.Character)
                {
                    countOffset = _trailingCRLFCount;
                    count += countOffset;
                    Comment("Depricating expected count of chracters by " + countOffset + " for textunit " + ParseType(TextUnit.Character));
                }
                else if (unit == TextUnit.Word)
                {
                    count--;
                    Comment("Depricating expected count of words by -1 for textunit " + ParseType(TextUnit.Word));
                }
            }

            // First N times, the return value of Move should equal result
            for (int i = 0; i < count; i++)
            {
                Range_Move(callingRange, unit, moveCount, ref actualResult, null, checkType);

                if (actualResult != expectedResult)
                {
                    ThrowMe(checkType, msg + " returned " + actualResult + ", expected = " + expectedResult);
                }
            }
            Comment(msg + " called " + count + " times successfully");


            // Now move N+1'th time, should always return 0
            moveCount = 0;
            Range_Move(callingRange, unit, moveCount, ref actualResult, null, checkType);

            if (actualResult != 0)
                ThrowMe(checkType, msg + " returned " + actualResult + ", expected = 0");
            else
                Comment(msg + " returned 0 as expected");

            m_TestStep++;
        }

        //---------------------------------------------------------------------------
        // Iterate through child automation elements
        //---------------------------------------------------------------------------
        internal void TS_RecurseChildAutomationElements(TextPatternRange callingRange, CheckType checkType)
        {
            int recurseLevel = 1;

            Comment("Performing recursion through child elements (if any)");

            // 



            if (!CheckTable(callingRange))
            {
                RecurseChildAutomationElements(recurseLevel, callingRange, checkType);
            }

            m_TestStep++;
        }

        //---------------------------------------------------------------------------
        // Scrolls control viewport
        //---------------------------------------------------------------------------
        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "System.ArgumentException.#ctor(System.String)")]
        internal void TS_ScrollViewPort(ScrollLocation scrollLocation, bool scrollRequired, CheckType checkType)
        {
            bool isHorizScrollable;
            bool isVertScrollable;
            

            const double NoScroll      = -1L;
            const double ScrollLeft    = 0L;
            const double ScrollTop     = 0L;
            const double ScrollMiddle  = 50L;
            const double ScrollRight   = 100L;
            const double ScrollBottom  = 100L;
            
            double horizontalPercent = 0;
            double verticalPercent   = 0;

            ScrollPattern scrollPattern = null;

            if (scrollLocation == ScrollLocation.NotApplicable)
            {
                Comment("Viewport location is not important to this test");
            }
            else
            {
                try
                {
                    // can we get the scroll pattern?
                    bool isScrollPatternAvailable = (bool)m_le.GetCurrentPropertyValue(AutomationElement.IsScrollPatternAvailableProperty);

                    // Apparently so...            
                    if (isScrollPatternAvailable)
                    {
                        scrollPattern = m_le.GetCurrentPattern(ScrollPattern.Pattern) as ScrollPattern;
                    }
                }
                catch (Exception exception)
                {
                    if (Library.IsCriticalException(exception))
                        throw;

                    ThrowMe(checkType, "Exception raised trying to obtain ScrollPattern. Error = " + exception.Message);
                }

                // Now we determine if null scroll pattern is bad or not
                if (scrollPattern == null)
                {
                    if (scrollRequired == true)
                        ThrowMe(checkType, "ScrollPattern required for test");
                    else
                    {
                        m_TestStep++;
                        Comment("Unable to acquire ScrollPattern, continuing with test");
                        return;
                    }
                }

                // can we scroll on either axis?
                isHorizScrollable = scrollPattern.Current.HorizontallyScrollable;
                isVertScrollable = scrollPattern.Current.VerticallyScrollable;
                Comment("ScrollPattern.HorizontallyScrollable = " + isHorizScrollable.ToString());
                Comment("ScrollPattern.VerticallyScrollable   = " + isVertScrollable.ToString());


                // Calcualte how we want to scroll. And if we can't do the scroll we want, reflect that too.
                switch (scrollLocation)
                {
                    case ScrollLocation.LeftTop:    // Do nothing, default.
                        horizontalPercent = ScrollLeft;
                        verticalPercent   = ScrollTop;
                        break;

                    case ScrollLocation.RightTop:
                        if (isHorizScrollable == false)
                            ThrowMe(checkType, "Unable to scroll viewport horizontally to the right");
                        horizontalPercent = ScrollRight;
                        verticalPercent   = ScrollTop;
                        break;

                    case ScrollLocation.LeftCenter:
                        if (isVertScrollable == false)
                            ThrowMe(checkType, "Unable to scroll viewport vertically to the middle");
                        horizontalPercent = ScrollLeft;
                        verticalPercent   = ScrollMiddle;
                        break;

                    case ScrollLocation.Center:
                        if (isHorizScrollable == false)
                            ThrowMe(checkType, "Unable to scroll viewport horizontally to the right");
                        if (isVertScrollable == false)
                            ThrowMe(checkType, "Unable to scroll viewport vertically to the middle");
                        horizontalPercent = ScrollMiddle;
                        verticalPercent   = ScrollMiddle;
                        break;

                    case ScrollLocation.RightCenter:
                        if (isHorizScrollable == false)
                            ThrowMe(checkType, "Unable to scroll viewport horizontally to the right");
                        if (isVertScrollable == false)
                            ThrowMe(checkType, "Unable to scroll viewport vertically to the middle");
                        horizontalPercent = ScrollRight;
                        verticalPercent   = ScrollMiddle;
                        break;

                    case ScrollLocation.LeftBottom:
                        if (isVertScrollable == false)
                            ThrowMe(checkType, "Unable to scroll viewport vertically to the middle");
                        horizontalPercent = ScrollLeft;
                        verticalPercent   = ScrollBottom;
                        break;

                    case ScrollLocation.RightBottom:
                        if (isHorizScrollable == false)
                            ThrowMe(checkType, "Unable to scroll viewport horizontally to the right");
                        if (isVertScrollable == false)
                            ThrowMe(checkType, "Unable to scroll viewport vertically to the middle");
                        horizontalPercent = ScrollLeft;
                        verticalPercent   = ScrollBottom;
                        break;

                    case ScrollLocation.NotApplicable: // should not get here...
                    default:
                        throw new ArgumentException("TS_ScrollViewPort() has no support for " + ParseType(scrollLocation));
                }

                // If we're not scrollable, adjust our scroll values
                if (isHorizScrollable == false)
                {
                    Comment("Unable to scroll horizontally");
                    horizontalPercent = NoScroll;
                }
                if (isVertScrollable == false)
                {
                    Comment("Unable to scroll vertically");
                    verticalPercent = NoScroll;
                }

                if ( (isHorizScrollable == false) && (isVertScrollable == false) )
                {
                    ThrowMe(checkType, "Unable to scroll, perhaps text isn't right width (height?) or control doesn't support scrolling");
                }
                
                Comment("---Calling ScrollPattern.SetScrollPercent( " + horizontalPercent + ", " + verticalPercent + ")");
                scrollPattern.SetScrollPercent(horizontalPercent, verticalPercent);
                Comment("Viewport is in " + Parse(scrollLocation));
            }
            m_TestStep++;
        }

        //---------------------------------------------------------------------------
        // Sets values of attributes to correct/incorrect type and/or value
        //---------------------------------------------------------------------------
        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "System.ArgumentException.#ctor(System.String)")]
        static internal void TS_SetAttributeValues(ref IDictionary attributes, TypeValue typeValue)
        {
            float f = 0; // A type that no attribute uses as of 7/7/05
            IDictionary newAttribs = new Hashtable(attributes.Count);
            IDictionaryEnumerator enum1 = null;
            AutomationTextAttribute key = null;
            object value = null;

            enum1 = attributes.GetEnumerator();

            if (typeValue == TypeValue.MatchesTypeAndValue) // i.e. we already have everything we need
            {
                Comment("Default Attribute values and type are correct for this test");
            }
            else
            {
                for (int i = 0; i < attributes.Count; i++)
                {
                    enum1.MoveNext();
                    key = (AutomationTextAttribute)enum1.Key;
                    value = enum1.Value;

                    // Strictly speaking, its not necessary to add the key/value within each case statement
                    // but for clarity, i.e. making it clear what (if anything) is done to the value variable
                    switch (typeValue)
                    {
                        case TypeValue.Null:                                     // NULL
                            newAttribs.Add(key, null);
                            break;
                        case TypeValue.WrongTypeAndValue:                           // INCORRECT type and value
                            newAttribs.Add(key, f);
                            break;
                        case TypeValue.WrongType:                                // INCORRECT type: CORRECT value
                            SetWrongType(ref value);
                            newAttribs.Add(key, value);
                            break;
                        case TypeValue.WrongValue:                               // CORRECT type: INCORRECT value
                        case TypeValue.WrongEnumValue:                           // Incorrect enum value
                            SetWrongValue(ref value, key);
                            newAttribs.Add(key, value);
                            break;
                        default:
                            throw new ArgumentException("TS_SetAttributeValues has no support for " + ParseType(typeValue));
                    }
                }

                // Delete existing attribute library and rebuild it
                attributes.Clear();
                enum1 = newAttribs.GetEnumerator();

                for (int i = 0; i < newAttribs.Count; i++)
                {
                    enum1.MoveNext();
                    key = (AutomationTextAttribute)enum1.Key;
                    value = enum1.Value;

                    attributes.Add(key, value);
                }

                Comment("Modified dictionary of known attribute values to be " + Parse(typeValue));
            }

            m_TestStep++;
        }

        //---------------------------------------------------------------------------
        // Sets focus, but skips IPControl
        //---------------------------------------------------------------------------
        internal void TS_SetFocus(CheckType checkType)
        {
            if (IsIPControl() == false)  // raises exception if true
            {
                m_le.SetFocus();
                Comment("Calling SetFocus() for control ensures selection will be visible");
            }
            else
            {
                ThrowMe(checkType, "TextPattern tests that set text are not supported on SysIPAddress32 control");
            }
            m_TestStep++;
        }

        //---------------------------------------------------------------------------
        // Set text to search for
        //---------------------------------------------------------------------------
        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "System.ArgumentException.#ctor(System.String)")]
        internal void TS_SetSearchText(out string searchText, SearchText searchTextType, CheckType checkType)
        {
            string allText = _actualText;
            TextPatternRange subRange = null;
            TextPatternRange documentRange = Pattern_DocumentRange(CheckType.Verification);

            // Get all text for the document
            // Initialize serachText
            searchText = "";

            switch (searchTextType)
            {
                case SearchText.Empty:                        // EMPTY Text
                    searchText = "";
                    break;
                case SearchText.MatchesAll:                   // matches ALL Text
                    searchText = _actualText;
                    break;
                case SearchText.MatchesSubRangeFirst:         // matches sub range at START of doc
                    CreateSubRange(out subRange, RangeLocation.Start, checkType);
                    Range_GetText(subRange, ref searchText, -1, null, checkType);
                    break;
                case SearchText.MatchesSubRangeMiddle:        // matches sub range in MIDDLE of doc
                    CreateSubRange(out subRange, RangeLocation.Middle, checkType);
                    Range_GetText(subRange, ref searchText, -1, null, checkType);
                    break;
                case SearchText.MatchesSubRangeEnd:           // matches sub range at END of doc
                    CreateSubRange(out subRange, RangeLocation.Middle, checkType);
                    Range_GetText(subRange, ref searchText, -1, null, checkType);
                    break;
                case SearchText.MismatchedCaseAll:            // matches ALL Text, w/MISMATCHED case
                    searchText = _actualText;
                    CreateMismatchedCase(ref searchText, allText, checkType);
                    break;
                case SearchText.MismatchedCaseSubRangeFirst:  // matches sub range at START of doc, w/MISMATCHED case
                    CreateSubRange(out subRange, RangeLocation.Start, checkType);
                    Range_GetText(subRange, ref allText, -1, null, checkType);
                    searchText = allText;
                    CreateMismatchedCase(ref searchText, allText, checkType);
                    break;
                case SearchText.MismatchedCaseSubRangeMiddle: // matches sub range in MIDDLE of doc, w/MISMATCHED case
                    CreateSubRange(out subRange, RangeLocation.Middle, checkType);
                    Range_GetText(subRange, ref allText, -1, null, checkType);
                    searchText = allText;
                    CreateMismatchedCase(ref searchText, allText, checkType);
                    break;
                case SearchText.MismatchedCaseSubRangeEnd:    // matches sub range at END of doc, w/MISMATCHED case
                    CreateSubRange(out subRange, RangeLocation.Middle, checkType);
                    Range_GetText(subRange, ref allText, -1, null, checkType);
                    searchText = allText;
                    CreateMismatchedCase(ref searchText, allText, checkType);
                    break;
                case SearchText.Random256:                    // RANDOM 256 characters
                    searchText = Helpers.RandomString(256, true);
                    break;
                case SearchText.Null:                         // NULL
                    searchText = null;
                    break;
                // Searchtext.Matches... & SearchText.Mismatched... require SampleText.DuplicateBlocksOfText
                case SearchText.MatchesFirstBlock:            // matches 1ST instance of repeating block of text
                    searchText = allText.Substring(0, randomBlockSizeOffset);
                    break;
                case SearchText.MatchesLastBlock:             // matches LAST instance of repeating block of text
                    searchText = allText.Substring(allText.Length - randomBlockSizeOffset, randomBlockSizeOffset);
                    break;
                case SearchText.MismatchedCaseFirstBlock:     // matches 1ST instance of repeating block of text, w/MISMATCHED case
                    searchText = allText.Substring(0, randomBlockSizeOffset);
                    CreateMismatchedCase(ref searchText, searchText, checkType);
                    break;
                case SearchText.MismatchedCaseLastBlock:      // matches LAST instance of repeating block of text, w/MISMATCHED case
                    searchText = allText.Substring(allText.Length - randomBlockSizeOffset, randomBlockSizeOffset);
                    CreateMismatchedCase(ref searchText, searchText, checkType);
                    break;
                default:
                    throw new ArgumentException("TS_SetSearchText() has no support for " + ParseType(searchTextType));
            }
            Comment("Text to search for is " + Parse(searchTextType) );
            Comment("Search text is      = '" + TrimText(searchText, 512) + "'");
            m_TestStep++;
        }

        //---------------------------------------------------------------------------
        // Overloaded helper for TS_SetText
        //---------------------------------------------------------------------------
        internal void TS_SetText(SampleText sampleText, CheckType checkType)
        {
            string actualText = "";

            TS_SetText(sampleText, out actualText, checkType);
        }

        //---------------------------------------------------------------------------
        // Overloaded helper for TS_SetText
        //---------------------------------------------------------------------------
        internal void TS_SetText(SampleText sampleText, out string actualText, CheckType checkType)
        {
            // When we change the text of the pattern, any existing ranges are now invalid
            SetText(sampleText, out actualText, checkType);

            m_TestStep++;
        }

        //---------------------------------------------------------------------------
        // Verify if there are mixed attribute values
        //---------------------------------------------------------------------------
        static internal void TS_VerifyAttributeRanges(IDictionary attributes, out bool isConsistentAttributes)
        {
            IDictionaryEnumerator enum1 = attributes.GetEnumerator();

            // default value
            isConsistentAttributes = true;

            // Enumerate through attributes
            while (enum1.MoveNext())
            {
                if (enum1.Value == TextPattern.MixedAttributeValue)
                {
                    isConsistentAttributes = false;
                    break;
                }
            }
            if (isConsistentAttributes == true)
                Comment("All attributes for the document have consistent attribute values");
            else
                Comment("Document contains mixed attribute values for a given attribute");
            m_TestStep++;
        }

        //---------------------------------------------------------------------------
        // Verify value equality of two automation elements
        //---------------------------------------------------------------------------
        static internal void TS_VerifyAutomationElement(AutomationElement autoElement1, AutomationElement autoElement2, CheckType checkType)
        {
            VerifyAutomationElement(autoElement1, autoElement2, checkType);
            m_TestStep++;
        }

        //---------------------------------------------------------------------------
        // Verify if child automation elements are present or not present (as expected)        
        //---------------------------------------------------------------------------
        internal void TS_VerifyChildren(bool requiresChildren, CheckType checkType)
        {
            bool hasChildren = false;
            TextPatternRange documentRange = Pattern_DocumentRange(CheckType.Verification);

            hasChildren = (documentRange.GetChildren().Length != 0);

            if ((requiresChildren == true)
             && (requiresChildren != hasChildren))
            {
                ThrowMe(checkType, "Required TextPattern to have Child AutomationElements, none are present");
            }

            Comment("TextPattern " + (hasChildren == true ? "has" : "does not have") + " child AutomationElement(s) as expected");
            m_TestStep++;
        }

        //---------------------------------------------------------------------------
        // Verifies the value of SupportedTextSelection
        //---------------------------------------------------------------------------
        internal void TS_VerifySupportedTextSelection(SupportedTextSelection expectedValue, CheckType checkType)
        {
            SupportedTextSelection actualValue = TextLibrary.ProviderSupportedTextSelection();;
            
            // Compare what TextPattern tells us against what we expect based on the
            // test application says we should support
            if ( expectedValue != actualValue )
            {
                Comment("TextPattern says SupportedTextSelection = " + actualValue.ToString());
                Comment("Expected value to be                    = " + expectedValue.ToString());

                ThrowMe(checkType, "Inconsistent expectedValue for SupportedTextSelection");
            }

            Comment("SupportedTextSelection = " + actualValue + ", as expected");
            m_TestStep++;
        }

        //---------------------------------------------------------------------------
        // Validate that text returned from GetText(...) was correct / met expectations
        //---------------------------------------------------------------------------
        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "System.ArgumentException.#ctor(System.String)")]
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
        internal void TS_VerifyTextLength(GetResult getResult, string actualText, string expectedText, int maxLength, Type expectedException, CheckType checkType)
        {
            int expectedLength = 0;

            Library.ValidateArgumentNonNull(actualText, "actualText cannot be null");
            Library.ValidateArgumentNonNull(expectedText, "expectedText cannot be null");

            // Level the playing field...
            TrimTrailingCRLF(m_le, ref actualText);
            TrimTrailingCRLF(m_le, ref expectedText);

            switch (getResult)
            {
                case GetResult.DocumentRange:
                    expectedLength = expectedText.Length;
                    break;
                case GetResult.CallingRangeLength:
                    expectedLength = actualText.Length;
                    break;
                case GetResult.Empty:
                    expectedLength = 0;
                    break;
                case GetResult.Exception:
                    if (expectedException == null)
                        ThrowMe(checkType, "Test code error, null expected exception is incorrect when getresult = " + Parse(getResult, expectedException));
                    actualText = ""; // Not really, but it gets us the right reporting below
                    expectedText = "";
                    expectedLength = actualText.Length;
                    break;
                case GetResult.Null:
                    if (actualText != null)
                        ThrowMe(checkType, "Expected null value to be returned from GetText(" + maxLength + ")");
                    actualText = ""; // Not really, but it gets us the right reporting below
                    expectedText = "";
                    expectedLength = actualText.Length;
                    break;
                case GetResult.MatchingAttributeValue:
                default:
                    throw new ArgumentException("TS_VerifyTextLength() has no support for " + ParseType(getResult));
            }

            if (actualText.Length == expectedLength)
                Comment("GetText(" + maxLength + ") returned correct result :" + Parse(getResult, expectedException));
            else
            {
                ThrowMe(checkType, "GetText(" + maxLength + ") returned text of size " + actualText.Length +
                         ", expected size " + expectedLength + " for getResult = " + Parse(getResult, expectedException));
            }

            m_TestStep++;
        }

        //---------------------------------------------------------------------------
        // Verifies that actual text = or is subset of expected text
        //---------------------------------------------------------------------------
        static void TS_TextWithin(string methodTested, string actualText, string expectedText, CheckType checkType)
        {
            TextWithin(methodTested, actualText, expectedText, checkType);
            m_TestStep++;
        }

        //---------------------------------------------------------------------------
        // Verify bounding rectangles are within a larger automation element bounding rectangle
        //---------------------------------------------------------------------------
        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "System.ArgumentException.#ctor(System.String)")]
        static void TS_VerifyWithinRects(Rect outerRect, Rect[] innerRects, GetBoundRectResult getBoundRectResult, CheckType checkType)
        {
            double areaOuter = 0;
            double areaInner = 0;

            // Sanity check... are any of our inners outside our outer?
            for (int i = 0; i < innerRects.Length; i++)
            {
                if ((innerRects[i].Top < outerRect.Top)
                 || (innerRects[i].Left < outerRect.Left)
                 || (innerRects[i].Right > outerRect.Right)
                 || (innerRects[i].Bottom > outerRect.Bottom))
                {
                    ThrowMe(checkType, "InnerRect " + innerRects[i].ToString(CultureInfo.InvariantCulture) +
                            " incorrectly outside autoElement.BoundRect " + outerRect.ToString(CultureInfo.InvariantCulture));
                }
                else
                {
                    areaInner += (innerRects[i].Bottom - innerRects[i].Top) * (innerRects[i].Right - innerRects[i].Left);

                    Comment("Bounding Rectangle # " + i + " (" + innerRects[i].ToString(CultureInfo.InvariantCulture) +
                            ") within autoElement.BoundRect " + outerRect.ToString(CultureInfo.InvariantCulture));
                }
            }

            // Now, lets split some hairs
            areaOuter = (outerRect.Bottom - outerRect.Top) * (outerRect.Right - outerRect.Left);
            Comment("Inner area = " + areaInner);
            Comment("Outer area = " + areaOuter);

            // Now, validate results
            switch (getBoundRectResult)
            {
                case GetBoundRectResult.SubsetOfAutoElementBoundRect:
                    if (areaInner < areaOuter)
                        Comment("Inner rectangle(s) is/are expected size, i.e. a small portion of the outer rectangle");
                    else if (innerRects.Length == 0)
                        ThrowMe(checkType, "Was not expecting an EMPTY array of bounding rectangles");
                    else
                        ThrowMe(checkType, "Unexpected state for test");
                    break;
                case GetBoundRectResult.EmptyArray:
                    if (innerRects.Length == 0)
                        Comment("Empty array of bounding rectangles, as expected");
                    else 
                        ThrowMe(checkType, "Was not expecting an a NON-EMPTY array of bounding rectangles");
                    break;
                default:
                    throw new ArgumentException("TS_VerifyWithinRects() has no support for " + ParseType(getBoundRectResult));
            }

            m_TestStep++;
        }

        #endregion utility TestSteps

        #endregion TestSteps

        #region Misc

        //---------------------------------------------------------------------------
        // Add attribute to dictionary
        //---------------------------------------------------------------------------
        internal void AddAttributeToDictionary(TextPatternRange callingRange, ref IDictionary dict, AutomationTextAttribute attrib, CheckType checkType)
        {
            object attribValue = null;

            Range_GetAttributeValue(callingRange, attrib, ref attribValue, null, checkType);

            if (attribValue == null)
                ThrowMe(checkType, "Null incorrectly returned for value of text attribute " + Helpers.GetProgrammaticName(attrib));

            dict.Add(attrib, attribValue);
        }


        //---------------------------------------------------------------------------
        // Add Converts Count enum value into integer
        //---------------------------------------------------------------------------
        internal int CountConvert(Count countType, int countOfTextUnits)
        {
            switch (countType)
            {
                case Count.MinInt:                                 return Int32.MinValue;
                case Count.NegativeNPlusOne:                       return -1 * ( countOfTextUnits + 1 );
                case Count.NegativeN:                              return -1 * ( countOfTextUnits );
                case Count.NegativeNMinusOne:                      return -1 * ( countOfTextUnits - 1 );
                case Count.NegativeHalfN:                          return (int)(( 0.5 * (double)countOfTextUnits) * -1);
                case Count.NegativeOne:                            return -1;
                case Count.Zero:                                   return 0;
                case Count.One:                                    return 1;
                case Count.HalfN:                                  return (int)( 0.5 * (double)countOfTextUnits);
                case Count.NMinusOne:                              return ( countOfTextUnits - 1);
                case Count.N:                                      return countOfTextUnits;
                case Count.NPlusOne:                               return ( countOfTextUnits + 1);
                case Count.MaxInt:                                 return Int32.MaxValue;
                default:
                    throw new ArgumentException("CountConvert() has no support for " + ParseType(countType));
            }
        
        }

        //---------------------------------------------------------------------------
        // Creates empty range with specific qualities
        //---------------------------------------------------------------------------
        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "System.ArgumentException.#ctor(System.String)")]
        internal void CreateEmptyRange(out TextPatternRange range, RangeLocation rangeLoc, CheckType checkType)
        {
            string text = "";
            int expectedCount = 0;
            int actualCount = 0;

            // Get range and text in the range            
            range = null;
            range = Pattern_DocumentRange(checkType);

            if (IsEmptyRange(range, checkType))
            {
                Comment("Document already has an empty range, proceeding with next test step");
                return;
            }
            else
            {
                Range_GetText(range, ref text, -1, null, checkType);
            }

            // Now, create the degenerate range            
            switch (rangeLoc)
            {
                case RangeLocation.Start:
                    // Move end point to start of document
                    expectedCount = -1;
                    actualCount = expectedCount;
                    ExpandRange(range, TextPatternRangeEndpoint.End, TextUnit.Document, expectedCount, out actualCount, true, checkType);
                    break;
                case RangeLocation.Middle:
                    // Move start point to middle of doc
                    expectedCount = text.Length / 2;
                    actualCount = expectedCount;
                    ExpandRange(range, TextPatternRangeEndpoint.Start, TextUnit.Character, expectedCount, out actualCount, true, checkType);

                    // Move End point to middle of doc
                    expectedCount = -1 * (text.Length - expectedCount);
                    actualCount = expectedCount;
                    ExpandRange(range, TextPatternRangeEndpoint.End, TextUnit.Character, expectedCount, out actualCount, true, checkType);
                    break;
                case RangeLocation.End:
                    // Move end point to start of document
                    expectedCount = 1;
                    actualCount = expectedCount;
                    ExpandRange(range, TextPatternRangeEndpoint.Start, TextUnit.Document, expectedCount, out actualCount, true, checkType);

                    // Resolve RichEdit control inconsistencies / unexpected behavior
                    TextLibrary.TrimRangeCRLF(m_le, range);
                    break;
                default:
                    throw new ArgumentException("TS_CreateEmptyRange() has no support for " + ParseType(rangeLoc));
            }

            // Final check
            if (IsEmptyRange(range, checkType))
                Comment("Created degenerate (empty) range at " + rangeLoc + " of the document");
            else
            {
                ThrowMe(checkType, "TS_CreateEmptyRange() failed to create empty range, text = '" + TrimText(Range_GetText(range), 512) + "'");
            }

        }

        //---------------------------------------------------------------------------
        // Creates empty range with specific qualities
        //---------------------------------------------------------------------------
        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "System.ArgumentException.#ctor(System.String)")]
        internal void CreateEmptyRandomRange(out TextPatternRange range, RangeLocation rangeLoc, TargetRangeType targetRangeType, CheckType checkType)
        {
            int actualCount = 0;
            TextPatternRange documentRange = Pattern_DocumentRange(CheckType.Verification);

            // Requires non-empty range
            if (IsEmptyRange(documentRange, checkType))
                ThrowMe(checkType, ParseType(targetRangeType) + " requires non-empty range");

            // Get range and text in the range            
            range = null;
            range = Pattern_DocumentRange(checkType);

            // calculate position of random range
            int length = Range_GetText(range).Length;
            int offset = 1 + Helpers.RandomValue(1, length / 4);

            // Create empty range by collapsing endpoint to start
            CreateEmptyRange(out range, RangeLocation.Start, checkType);

            // Now, create the degenerate range            
            switch (rangeLoc)
            {
                case RangeLocation.Start:
                    // Move start (and end) endPoint to location in document
                    // This works because Start EndPoint can never roll past end endPoint (it just rolls with start)
                    Range_MoveEndpointByUnit(range, TextPatternRangeEndpoint.Start, TextUnit.Character,
                            offset, ref actualCount, null, checkType);
                    break;
                case RangeLocation.End:
                    offset = 0 - offset; // was length - offset. This seems to be correct
                    // Move start (and end) endPoint to location in document
                    // This works because Start EndPoint can never roll past end endPoint (it just rolls with start)
                    Range_MoveEndpointByUnit(range, TextPatternRangeEndpoint.Start, TextUnit.Character,
                            offset, ref actualCount, null, checkType);
                    break;
                default:
                    throw new ArgumentException("TS_CreateEmptyRandomRange() has no support for " + ParseType(rangeLoc));
            }

            // Final check
            if (IsEmptyRange(range, checkType))
                Comment("Created degenerate (empty) range near " + rangeLoc + " of the document");
            else
            {
                ThrowMe(checkType, "TS_CreateEmptyRange() failed to create empty range, text = '" +
                         TrimText(Range_GetText(range), 512) + "'");
            }
        }

        //---------------------------------------------------------------------------
        // Create range of specific type (random ranges)
        //---------------------------------------------------------------------------
        internal void CreateRamdomRange(TextPatternRange sourceRange, out TextPatternRange targetRange, TargetRangeType targetRangeType, CheckType checkType)
        {
            int startOffset = 0; 
            int endOffset   = 0;
            int length      = _actualText.Length;

            targetRange = null;
            
            // Determine offsets of existing range
            TextPatternRange documentRange = Pattern_DocumentRange(checkType);
            
            Range_CompareEndpoints( sourceRange, TextPatternRangeEndpoint.Start, documentRange, TextPatternRangeEndpoint.Start, ref startOffset, null, checkType );
            Range_CompareEndpoints( sourceRange, TextPatternRangeEndpoint.End,   documentRange, TextPatternRangeEndpoint.End,   ref endOffset,   null, checkType );

            Comment("Calling range is from " + startOffset + " to " + (length + endOffset) + " (characters)" );

            // Adjust offsets
            switch (targetRangeType)
            {
                case TargetRangeType.RandomEndsStart:               //range that ENDs at START of calling range
                    endOffset = startOffset - length;
                    startOffset = 0;
                    break;
                case TargetRangeType.RandomStartStart:              //range that STARTs at START of calling range
                    endOffset   = 0;                                //start endPoint remains unchanged
                    break;
                case TargetRangeType.RandomEndsEnd:                 //range that ENDs at END of calling range
                    startOffset = 0;                                //end endPoint remains unchanges
                    break;
                case TargetRangeType.RandomStartsEnd:               //range that STARTs at END of calling range
                    startOffset = endOffset + length;
                    endOffset   = 0;
                    break;
                default:
                    throw new ArgumentException("CreateRandomRange() has no support for " + ParseType(targetRangeType));
            }

            // Set offsets of target range
            Range_Clone( documentRange, ref targetRange, null, checkType );
            Range_MoveEndpointByUnit( targetRange, TextPatternRangeEndpoint.Start, TextUnit.Character, startOffset, ref startOffset, null, checkType );
            Range_MoveEndpointByUnit( targetRange, TextPatternRangeEndpoint.End,   TextUnit.Character, endOffset,   ref endOffset,   null, checkType );

            Comment("Target range is from " + startOffset + " to " + (length + endOffset) + " (characters)");
        }

        ///---------------------------------------------------------------------------
        /// <summary>Creates misc. ranges that don't fit in other convinenet helpers</summary>
        ///---------------------------------------------------------------------------
        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "System.ArgumentException.#ctor(System.String)")]
        internal void CreateMiscRange(TargetRangeType targetRangeType, out TextPatternRange targetRange, CheckType checkType)
        {
            int startOffset = 0;
            int endOffset = 0;
            int actualStartOffset = 0;
            int actualEndOffset = 0;
            string text = "";
            TextPatternRange documentRange = Pattern_DocumentRange(CheckType.Verification);

            Range_GetText(documentRange, ref text, -1, null, checkType);

            // Sanity check            
            TrimTrailingCRLF(m_le, ref text);
            if (text.Length == 0)
                ThrowMe(checkType, "Require non-empty document to create range " + Parse(targetRangeType));

            // calculate where range begins/ends
            switch (targetRangeType)
            {
                case TargetRangeType.TwoCharsAdjacent:              //two adjacent characters
                    // 

                    startOffset = 1;
                    endOffset = 3;
                    break;
                case TargetRangeType.TwoCharsSplitAcrossLine:       //two chracters split across a line break
                    // 

                    startOffset = text.IndexOf("\r") - 1;
                    endOffset = startOffset + 3;
                    break;
                default:
                    throw new ArgumentException("CreateMiscRange() has no support for " + ParseType(targetRangeType));
            }

            // sanity check
            if ((startOffset < 0) || (endOffset > text.Length))
                ThrowMe(checkType, "CreateMiscRange is unable to calculate correct start/end (" + startOffset + "," + endOffset + ") for range");

            // Create range
            CreateEmptyRange(out targetRange, RangeLocation.Start, checkType);
            Range_MoveEndpointByUnit(targetRange, TextPatternRangeEndpoint.End, TextUnit.Character, endOffset, ref actualEndOffset, null, checkType);
            Range_MoveEndpointByUnit(targetRange, TextPatternRangeEndpoint.Start, TextUnit.Character, startOffset, ref actualStartOffset, null, checkType);

            // sanity check
            if ((startOffset != actualStartOffset) || (endOffset != actualEndOffset))
            {
                ThrowMe(checkType, "CreateMisMatchRange could not move end points as expected. Expected offsets = " +
                        startOffset + ", " + endOffset + ", actual offsets = " + actualStartOffset + ", " + actualEndOffset);
            }
            else
                Comment("Created range of type " + Parse(targetRangeType));
        }

        //---------------------------------------------------------------------------
        // Create text that has mis-matched case from source text
        //---------------------------------------------------------------------------
        [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase")]
        [SuppressMessage("Microsoft.Performance", "CA1807:AvoidUnnecessaryStringCreation", MessageId = "sourceText")]
        static void CreateMismatchedCase(ref string destinationText, string sourceText, CheckType checkType)
        {
            destinationText = "";

            string temp = sourceText.ToUpper(CultureInfo.InvariantCulture);
            if (!sourceText.Equals(temp))
            {
                destinationText = temp;
                return;
            }

            temp = sourceText.ToLower(CultureInfo.InvariantCulture);
            if (!sourceText.Equals(temp))
            {
                destinationText = temp;
                return;
            }
            else
            {
                ThrowMe(checkType, "Unable to create mis-matched casing for text");
            }
        }

        //---------------------------------------------------------------------------
        // Create range of specific type (overload)
        //---------------------------------------------------------------------------
        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "System.ArgumentException.#ctor(System.String)")]
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        internal void CreateRange(out TextPatternRange targetRange, TargetRangeType targetRangeType, TextPatternRange sourceRange, CheckType checkType)
        {
            int startOffset = 0;
            int endOffset   = 0;
            
            CreateRange( out targetRange, targetRangeType, out startOffset, out endOffset, sourceRange, checkType ); 
        }

        //---------------------------------------------------------------------------
        // Create range of specific type
        //---------------------------------------------------------------------------
        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "System.ArgumentException.#ctor(System.String)")]
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        internal void CreateRange(out TextPatternRange targetRange, TargetRangeType targetRangeType, out int startOffset, out int endOffset, TextPatternRange sourceRange, CheckType checkType)
        {
            TextPattern differentTextPattern = null;

            // Initialize ref params
            targetRange = null;
            startOffset = 0;
            endOffset = 0;

            Comment("ENTERING CreateRange(" + Parse(targetRangeType) + "," + startOffset + "," + endOffset + "," + (sourceRange == null ? null : "<sourceRange>") + ",...)");

            switch (targetRangeType)
            {
                case TargetRangeType.DocumentRange:         // entire document
                    targetRange = Pattern_DocumentRange(checkType);
                    Comment("Created range of type " + Parse(targetRangeType));
                    break;
                case TargetRangeType.Clone:
                    Range_Clone(sourceRange, ref targetRange, null, checkType);
                    Comment("Created range of type " + Parse(targetRangeType));
                    break;
                case TargetRangeType.SameAsCaller:                  //same as calling range
                    if (sourceRange == null)
                        ThrowMe(checkType, "CreateRange(" + ParseType(targetRangeType) + ") cannot succeed, calling range was not passed into method");
                    targetRange = sourceRange;
                    Comment("Created range of type " + Parse(targetRangeType));
                    break;
                case TargetRangeType.Null: //null value
                    targetRange = null;
                    Comment("Created range of type " + Parse(targetRangeType));
                    break;
                case TargetRangeType.EmptyStart:            // EMPTY range at START of document
                    CreateEmptyRange(out targetRange, RangeLocation.Start, checkType);
                    break;
                case TargetRangeType.EmptyMiddle:           // EMPTY range in MIDDLE of document
                    CreateEmptyRange(out targetRange, RangeLocation.Middle, checkType);
                    break;
                case TargetRangeType.EmptyEnd:              // EMPTY range at END of document
                    CreateEmptyRange(out targetRange, RangeLocation.End, checkType);
                    break;
                case TargetRangeType.RandomStart:           //RANDOM range at START of document
                    //
                    CreateSubRange(out targetRange, RangeLocation.Start, out startOffset, out endOffset, checkType);
                    break;
                case TargetRangeType.RandomMiddle:          //RANDOM range in MIDDLE of document
                    CreateSubRange(out targetRange, RangeLocation.Middle, out startOffset, out endOffset, checkType);
                    break;
                case TargetRangeType.RandomEnd:             //RANDOM range at END of document
                    CreateSubRange(out targetRange, RangeLocation.End,
                                      out startOffset, out endOffset, checkType);
                    break;
                case TargetRangeType.RandomEmptyStart:      //EMPTY range at random location near START of document
                    CreateEmptyRandomRange(out targetRange, RangeLocation.Start, targetRangeType, checkType);
                    break;
                case TargetRangeType.RandomEmptyEnd:        //EMPTY range at random location near END of document
                    CreateEmptyRandomRange(out targetRange, RangeLocation.End, targetRangeType, checkType);
                    break;
                case TargetRangeType.FirstCharacter:        //first character in document
                    //
                    CreateTextUnitRange(out targetRange, TextUnit.Character, 1, targetRangeType, RangeLocation.Start, checkType);
                    break;
                case TargetRangeType.MiddleCharacter:       //MIDDLE chracter in document
                    CreateTextUnitRange(out targetRange, TextUnit.Character, 1, targetRangeType, RangeLocation.Middle, checkType);
                    break;
                case TargetRangeType.LastCharacter:         //last character in document
                    CreateTextUnitRange(out targetRange, TextUnit.Character, -1, targetRangeType, RangeLocation.End, checkType);
                    break;
                case TargetRangeType.FirstFormat:           //First format range in document
                    CreateTextUnitRange(out targetRange, TextUnit.Format, 1, targetRangeType, RangeLocation.Start, checkType);
                    break;
                case TargetRangeType.FirstWord:             //First word range in document
                    CreateTextUnitRange(out targetRange, TextUnit.Word, 1, targetRangeType, RangeLocation.Start, checkType);
                    break;
                case TargetRangeType.FirstLine:             //First line range in document
                    CreateTextUnitRange(out targetRange, TextUnit.Line, 1, targetRangeType, RangeLocation.Start, checkType);
                    break;
                case TargetRangeType.FirstParagraph:        //First paragraph range in document
                    CreateTextUnitRange(out targetRange, TextUnit.Paragraph, 1, targetRangeType, RangeLocation.Start, checkType);
                    break;
                case TargetRangeType.FirstPage:             //First page range in document
                    CreateTextUnitRange(out targetRange, TextUnit.Page, 1, targetRangeType, RangeLocation.Start, checkType);
                    break;
                case TargetRangeType.DifferentTextPattern:          //range from a different TextPattern
                    differentTextPattern = GetDifferentTextPattern(m_le, checkType);
                    targetRange = differentTextPattern.DocumentRange;
                    break;
                case TargetRangeType.TwoCharsAdjacent:              //two adjacent characters
                case TargetRangeType.TwoCharsSplitAcrossLine:       //two chracters split across a line break
                    CreateMiscRange(targetRangeType, out targetRange, checkType);
                    break;
                case TargetRangeType.VisibleRange:                  // Equal to visible range of control
                    CreateVisibleRange(ref targetRange, checkType );
                    break;
                case TargetRangeType.RandomEndsStart:               //range that ENDs at START of calling range
                case TargetRangeType.RandomStartStart:              //range that STARTs at START of calling range
                case TargetRangeType.RandomEndsEnd:                 //range that ENDs at END of calling range
                case TargetRangeType.RandomStartsEnd:               //range that STARTs at END of calling range
                    CreateRamdomRange(sourceRange, out targetRange, targetRangeType, checkType);
                    break;
                case TargetRangeType.MiddleSpaces:                  //In middle of spaces between 1st and 2nd word
                case TargetRangeType.MiddlePunctuation:             //In middle of punctuation between 1st and 2nd word
                case TargetRangeType.HiddenLast:                    //Last instance of hidden text within document
                case TargetRangeType.HiddenFirst:                   //1st instance of hidden text within document
                default:
                    throw new ArgumentException("CreateRange() has no support for " + ParseType(targetRangeType));
            }

            Comment("     EXITING CreateRange(" + ParseType(targetRangeType) + "," + startOffset + "," + endOffset + "," + (sourceRange == null ? null : "<sourceRange>") + ",...)");
        }

        //---------------------------------------------------------------------------
        // Overload for CreateSubRange
        //---------------------------------------------------------------------------
        internal void CreateSubRange(out TextPatternRange subRange, RangeLocation rangeLocation, CheckType checkType)
        {
            int startOffset = 0;
            int endOffset = 0;

            CreateSubRange(out subRange, rangeLocation, out startOffset, out endOffset, checkType);
        }

        //---------------------------------------------------------------------------
        // Create a TextPatternRange that is a subset of the existing TextPattern.DocumentRange
        //---------------------------------------------------------------------------
        internal void CreateSubRange(out TextPatternRange subRange, RangeLocation rangeLocation, out int startOffset, out int endOffset, CheckType checkType)
        {
            TextPatternRange documentRange  = Pattern_DocumentRange(CheckType.Verification);
            int              trailingCount  = TextLibrary.CountTrailingCRLF(m_le, Pattern_DocumentRange(checkType));
            int              stringLength   = Range_GetText(documentRange).Length - trailingCount;
            int              subRangeLength = (stringLength / 3) - (stringLength/12) + 1;  // should 1 less than 1/4. Note that +1 is very important, -1 causes overlaping sub-ranges!!!

            Comment("Creating sub-range at " + Parse(rangeLocation));

            // sanity check: we must have a string length >= 8
            if (stringLength < 8)
                ThrowMe(checkType, "CreateSubRange(" + ParseType(rangeLocation) + ") expected TextPattern.DocumentRange of length 8 or greater, actual length = " + stringLength);

            // Is it impossible to create sub-range???
            while( (subRangeLength > 0) && ((subRangeLength*4)>stringLength) )
                subRangeLength--;  // this is very small string lengths.

            // sanity check: If our sub range is too small, we're hosed. Technically, we shouldn't have gotten this far because of earlier checks
            if( subRangeLength == 0)
                ThrowMe( checkType, "Unable to create subRange, length of document is likely too small. Should be >= 6, actual length = " + stringLength  );

            Comment("String length          = " + stringLength);
            Comment("Maximum sub-range size = " + subRangeLength);

            // initialize vars
            startOffset = 0;
            endOffset = 0;

            // Find upper / lower bounds
            switch (rangeLocation)
            {
                case RangeLocation.Start:  // Range 0...{1/8 to 1/4} of document
                    // range from 0 to {7/12...9/12}
                    startOffset = 0;
                    endOffset -= (subRangeLength * 3 ) + (int)Helpers.RandomValue(0, subRangeLength/2);     // e.g. -1 * (3/4 + Random(0...1/8))
                    break;
                case RangeLocation.Middle:  // Range =  ~1/4...~3/4
                    startOffset = subRangeLength + (int) Helpers.RandomValue(0, (subRangeLength));          // e.g. 1/4 + Random(0..1/4)
                    endOffset -= subRangeLength + (int) Helpers.RandomValue(0, (subRangeLength));           // e.g. -1 * (1/4 + Random(0..1/4))
                    break;
                case RangeLocation.End:     // Range = ~3/4...7/8 of document
                    // Range from {7/12...9/12} to stringLength
                    startOffset = (subRangeLength * 3) + (int) Helpers.RandomValue(0, (subRangeLength/2));  // e.g. 3/4 + Random(0..1/8)
                    endOffset = 0;
                    break;
                default:
                    break;
            }

            // Sanity check... can't be greater than length. 
            // Can't be equal to length (we'd have an empty range then--not what we wanted)
            if ((startOffset + (-1 * endOffset)) >= stringLength)
            {
                Comment("StartOffset      = " + startOffset);
                Comment("EndOffset        = " + endOffset);
                ThrowMe(checkType, "startOffset + (-1*endOffset) cannot be greater than length of document");
            }

            // Another sanity check, end can't be equal to greater than start (must before start
            if( startOffset >= (stringLength + endOffset))
            {
                Comment("StartOffset      = " + startOffset);
                Comment("EndOffset        = " + endOffset);
                ThrowMe(checkType, "Test case problem:  startoffset (" + startOffset + ") cannot be <= [stringLength + endOffset(" + (stringLength + endOffset) + ")], must have at non-zero length range(!)");
            }

            // create sub-range
            subRange = null;
            Range_Clone(documentRange, ref subRange, null, checkType);
            if( startOffset != 0 )
                Range_MoveEndpointByUnit(subRange, TextPatternRangeEndpoint.Start, TextUnit.Character, startOffset, ref startOffset, null, checkType);
            if( endOffset != 0 )
                Range_MoveEndpointByUnit(subRange, TextPatternRangeEndpoint.End, TextUnit.Character, endOffset, ref endOffset, null, checkType);

            // Give results
            string s = Range_GetText(subRange);
            Comment("Created sub-range from " + startOffset + " to " + (_actualText.Length + endOffset) + " (characters)" );
            Comment("Sub-range length = " + s.Length);
            Comment("Sub-range        = '" + TrimText(s, 512) + "'");
        }

        //---------------------------------------------------------------------------
        // Create range of specific text unit size
        //---------------------------------------------------------------------------
        internal void CreateTextUnitRange(out TextPatternRange range, TextUnit unit, int expectedCount, TargetRangeType targetRangeType, RangeLocation rangeLocation, CheckType checkType)
        {
            int actualCount = 0;
            TextPatternRange documentRange = Pattern_DocumentRange(CheckType.Verification);

            if (IsEmptyRange(documentRange, checkType))
                ThrowMe(checkType, ParseType(targetRangeType) + " requires non-empty range");

            // Create empty range    
            CreateEmptyRange(out range, rangeLocation, checkType);

            // Expand range to match 
            Range_MoveEndpointByUnit(range, TextPatternRangeEndpoint.End, unit, expectedCount, ref actualCount, null, checkType);

            // If we did a move endPoint, did expected count = actual count?
            if (expectedCount != actualCount)
            {
                ThrowMe(checkType, "MoveEndpointByUnit(" + ParseType(unit) + "," + expectedCount + ") returned " +
                        actualCount + ", unable to create range of type " + Parse(targetRangeType));
            }
            else
            {
                Comment("Created range size " + expectedCount + " " + Parse(unit) + " units");
            }
        }

        //---------------------------------------------------------------------------
        // Create a TextPatternRange that contains all visible ranges
        //---------------------------------------------------------------------------
        internal void CreateVisibleRange( ref TextPatternRange targetRange, CheckType checkType )
        {
            int startMin = Int32.MaxValue, startTemp = 0;
            int endMax   = Int32.MinValue, endTemp   = 0;;
            TextPatternRange[] visibleRanges = null;
            
            Pattern_GetVisibleRanges( _pattern, ref visibleRanges, null, checkType );

            targetRange = Pattern_DocumentRange(checkType);

            // Find smallest start endPoint, highest end endPoint
            for( int i=0; i < visibleRanges.Length; i++ )
            {
                Range_CompareEndpoints(visibleRanges[i], TextPatternRangeEndpoint.Start, targetRange, TextPatternRangeEndpoint.Start, ref startTemp, null, checkType);
                Range_CompareEndpoints(visibleRanges[i], TextPatternRangeEndpoint.End,   targetRange, TextPatternRangeEndpoint.End, ref endTemp, null, checkType);
                
                startMin = Math.Min( startMin, startTemp );
                endMax   = Math.Max( endMax,   endTemp   );
            }
            
            // Build new range
            Range_MoveEndpointByUnit(targetRange, TextPatternRangeEndpoint.Start, TextUnit.Character, startMin, ref startTemp, null, checkType);
            Range_MoveEndpointByUnit(targetRange, TextPatternRangeEndpoint.End,   TextUnit.Character, endMax,   ref endMax,    null, checkType);

            Comment("Created composite range of " + visibleRanges.Length + " visible ranges");
            Comment("   Start = " + startMin.ToString().PadLeft(5) + " (actual = " + startTemp.ToString().PadLeft(5) + ")");
            Comment("   End   = " + endMax.ToString().PadLeft(5) + " (actual = " + endTemp.ToString().PadLeft(5) + ")");
            Comment("   Text  = " + TrimText( Range_GetText(targetRange), 512 ) );
        }

        //---------------------------------------------------------------------------
        // Expands range by a given # of units, and optionally then 
        // validates if # of units moved actual == expected
        //---------------------------------------------------------------------------
        internal void ExpandRange(TextPatternRange range, TextPatternRangeEndpoint endPoint, TextUnit unit, int expectedCount, out int actualCount, bool mustMatchExpected, CheckType checkType)
        {
            actualCount = 0;

            Range_MoveEndpointByUnit(range, endPoint, unit, expectedCount, ref actualCount, null, checkType);

            if (mustMatchExpected == true)
            {
                if (actualCount != expectedCount)
                {
                    ThrowMe(
                            checkType, "Unable to modify range " + endPoint + " endPoint by expected " +
                            expectedCount + " " + unit + " units. Actual = " + actualCount);
                }
            }
        }

        //---------------------------------------------------------------------------
        // Determine supported attributes and their values
        //---------------------------------------------------------------------------
        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "System.ArgumentException.#ctor(System.String)")]
        internal void GetAttributeValues(TextPatternRange callingRange, out IDictionary dict, AttributeType attribType, CheckType checkType)
        {
            bool isRichEdit = TextLibrary.IsRichEdit(m_le);
            int supportedAttribs = 0;   // How many attributes for this control???
            int expectedCount = 0;   // How many attributes do we expect to have?
            IDictionaryEnumerator enum1 = null;
            AutomationTextAttribute key = null;
            AttributeMadness attrib = null;

            dict = new Hashtable(attributesCount);

            enum1 = attributeMadness.GetEnumerator();

            // count attributes
            for (int i = 0; i < attributeMadness.Count; i++)
            {
                enum1.MoveNext();
                key = (AutomationTextAttribute)enum1.Key;
                attrib = (AttributeMadness)enum1.Value;

                switch (attribType)
                {
                    case AttributeType.SupportedAttributes:                // Supported attributes on the edit/richedit control
                        if (((isRichEdit == true) && (attrib.isRichEditControl == true))
                         || ((isRichEdit == false) && (attrib.isEditControl == true)))
                            AddAttributeToDictionary(callingRange, ref dict, key, checkType);
                        break;
                    case AttributeType.UnsupportedAttributes:              // Attributes not supported by the edit/richedit control
                        if (((isRichEdit == true) && (attrib.isRichEditControl == false))
                         || ((isRichEdit == false) && (attrib.isEditControl == false)))
                            AddAttributeToDictionary(callingRange, ref dict, key, checkType);
                        break;
                    case AttributeType.EnumAttributes:                     // Those attributes whose data type is an enum
                        if (attrib.isEnum == true)
                            AddAttributeToDictionary(callingRange, ref dict, key, checkType);
                        break;
                    //case AttributeType.Null:  // Delibratly skipped, this is for sending NULL as arg to GetAttributeValue
                    //      Building a dictionary here doesn't makes sense.
                    default:
                        throw new ArgumentException("GetAttributeValues() has no support for  " + ParseType(attribType));
                }
            }

            // Determine what correct count should be
            if (isRichEdit == true)
                supportedAttribs = richEditControlAttribCount;
            else
                supportedAttribs = editControlAttribCount;

            switch (attribType)
            {
                case AttributeType.SupportedAttributes:                // Supported attributes on the edit/richedit control
                    expectedCount = supportedAttribs;
                    break;
                case AttributeType.UnsupportedAttributes:              // Attributes not supported by the edit/richedit control
                    expectedCount = attributesCount - supportedAttribs;
                    break;
                case AttributeType.EnumAttributes:                     // Those attributes whose data type is an enum
                    expectedCount = enumAttributesCount;
                    break;
                case AttributeType.Null:                               // Null value for   attribute(s)
                    expectedCount = supportedAttribs;
                    break;
                default:
                    throw new ArgumentException("GetAttributeValues() has no support for  " + ParseType(attribType));
            }

            // Now, does count equal expectations???

            if (dict.Count != expectedCount)
            {
                ThrowMe(checkType, "Expected " + expectedCount + " attributes for Win32 " +
                        (isRichEdit == true ? "RichEdit" : "Edit") +
                        " control, got " + dict.Count);
            }
            else
                Comment("Correctly retrived values for " + dict.Count + " attributes for Win32 " +
                        (isRichEdit == true ? "RichEdit" : "Edit") + " control");
        }

        //---------------------------------------------------------------------------
        // Acquire valid bounding rectangle for AutomationElement
        //---------------------------------------------------------------------------
        internal void GetAutomationElementBoundingRectangle(out Rect boundingRect)
        {
            // FxCop doesn't like calls that return object and box value types
            // hence the two step process for getting a property
            object temp = m_le.GetCurrentPropertyValue(
                                            AutomationElement.BoundingRectangleProperty);
            boundingRect = (Rect)temp;

            Comment("AutomationElement Bounding Rectangle = " + boundingRect.ToString(CultureInfo.InvariantCulture));
        }

       /// -------------------------------------------------------------------
        /// <summary>
        /// Gets a TextPatternRange from another AutomationElement that supports TextPattern
        /// Assumes that the current app is a child of AutomationElement.RootElement
        /// Will navigate up the tree until the RootElement's child and then try to find a
        /// control that supports TextPattern.
        /// </summary>
        /// -------------------------------------------------------------------
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
        internal static TextPattern GetDifferentTextPattern(AutomationElement element, CheckType checkType)
        {
            Library.ValidateArgumentNonNull(element, "element cannot be null");

            AutomationElement tempElement = element;
            AutomationElement parent = element;

            // Find apps AutomationElement (should be child of the RootElement)
            while (AutomationElement.RootElement != parent)
            {
                tempElement = parent;
                parent = TreeWalker.ControlViewWalker.GetParent(tempElement);
            }

            // Get all apps AutomationElement windows that support TextPattern
            AutomationElementCollection textPatterns = tempElement.FindAll(TreeScope.Subtree, new PropertyCondition(AutomationElement.IsTextPatternAvailableProperty, true));

            if (textPatterns.Count == 1)
            {
                ThrowMe(checkType, "There is only one AutomationElement that supports TextPattern in this app. Unable to proceed with test");
                return null;
            }

            // Return a random AutomationElement that supports TestPattern
            Random rnd = new Random(~unchecked((int)DateTime.Now.Ticks));
            int i = rnd.Next(textPatterns.Count);
            while (Automation.Compare(element, (AutomationElement)textPatterns[i]))
            {
                i = rnd.Next(textPatterns.Count);
            }

            TextPattern textPattern = textPatterns[i].GetCurrentPattern(TextPattern.Pattern) as TextPattern;
            Comment("Found AutomationElement that supports TextPattern from control ID = " + textPatterns[i].Current.AutomationId ); 

            return textPattern;
        }

        //---------------------------------------------------------------------------
        // Overload for GetSampleText, for versions that don't need a duplicate block of text
        //---------------------------------------------------------------------------
        static internal void GetSampleText(SampleText sampleText, ref string text)
        {
            string dupBlock = "";
            GetSampleText(sampleText, ref text, out dupBlock);
        }

        //---------------------------------------------------------------------------
        // GetSampleText will return text string based on value of SampleText enum
        //---------------------------------------------------------------------------
        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "System.ArgumentException.#ctor(System.String)")]
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        static internal void GetSampleText(SampleText sampleText, ref string text, out string dupBlock)
        {
            int i = 0;
            string tempText;
            StringBuilder tempBuilder = new StringBuilder();

            dupBlock = "";

            // If noclobber flag is set, we cannot update the existing text
            if (_noClobber == true)
            {
                // For some tests, noClobber equates IncorrectElementConfiguration. Filter out for 
                // acceptable values only.
                if(sampleText != SampleText.NotApplicable)
                    ThrowMe(CheckType.IncorrectElementConfiguration, "/NOCLOBBER switch means this test cannot be run. It requires changing the text in the control being tested");
            }
            
            // Determine type of text we're going to set            
            switch (sampleText)
            {
                case SampleText.NotApplicable:              // Value of text does not matter for test
                    // We don't care, just return with text as-is
                    return;
                case SampleText.NotEmpty:                   // Verify text is NOT empty (or SET text if it is empty)
                    if ((text.Length == 0)      // If nothing is there...
                        || (text.Equals("\r"))     // Default from old RichEdit
                        || (text.Equals("\r\n"))) // Default from newer RichEdit
                    {
                        text = easyText;    // ...just assign something, don't care what
                    }
                    return;
                case SampleText.Empty:                      // Set text to EMPTY string
                    text = noText;
                    break;
                case SampleText.String1:                // Set text to be 'String 1' text
                    text = string1;
                    break;
                case SampleText.EasyText:                   // Set text to be ONE character
                    text = easyText;
                    break;
                case SampleText.TextIsOneChar:
                    text = textIsOneChar;
                    break;
                case SampleText.Random256:
                    text = Helpers.RandomString(256, true);
                    break;
                case SampleText.MultipleLines:            // Set text to be multi-line
                    text = Helpers.RandomString(336, true);
                    text = text.Replace(text.Substring(47, 2), "\r\n");  // Each line is 46 chars wide, six lines
                    text = text.Replace(text.Substring(95, 2), "\r\n");
                    text = text.Replace(text.Substring(143, 2), "\r\n");
                    text = text.Replace(text.Substring(191, 2), "\r\n");
                    text = text.Replace(text.Substring(239, 2), "\r\n");
                    text = text.Replace(text.Substring(287, 2), "\r\n");
                    break;

                case SampleText.Random64:
                    text = Helpers.RandomString(64, true);
                    break;
                case SampleText.Random64CR:
                    text = Helpers.RandomString(64, true);
                    text = text.Replace(text.Substring(4, 1), " ");      // each line has two(+) words
                    text = text.Replace(text.Substring(12, 1), " ");
                    text = text.Replace(text.Substring(20, 1), " ");
                    text = text.Replace(text.Substring(28, 1), " ");
                    text = text.Replace(text.Substring(36, 1), " ");
                    text = text.Replace(text.Substring(44, 1), " ");
                    text = text.Replace(text.Substring(15, 2), "\r\n");
                    text = text.Replace(text.Substring(23, 2), "\r\n");
                    text = text.Replace(text.Substring(31, 2), "\r\n");
                    text = text.Replace(text.Substring(47, 2), "\r\n");
                    text = text.Replace(text.Substring(52, 2), "\r\n");  // Each line is 8 chars wide
                    break;
                case SampleText.RandomTextAnySize:          // Set text to be RANDOM sample text
                    text = Helpers.RandomString((int)Helpers.RandomValue(32, 1024), true);
                    break;
                case SampleText.Complex1:                   // Set text to complex multi-line text with long line of text at end
                    text = "\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n" + Helpers.RandomString(256, true);
                    break;
                case SampleText.Complex2:                   // Set text to complex multi-line text with long line of text at start
                    text = Helpers.RandomString(256, true) + "\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n";
                    break;
                case SampleText.Num123:                     // Set text to 123456789
                    text = "123456789";
                    break;
                case SampleText.Num123CR:
                    text = "1 1\r\n2 2\r\n3 3\r\n4 4\r\n5 5\r\n6 6\r\n7 7\r\n8 8\r\n9 9";
                    break;
                case SampleText.OneTwoThree:             // Set text to OneTwoThree...Nine
                    text = "OneTwoThreeFourFiveSixSevenEightNine";
                    break;
                case SampleText.OneCRTwoCRThreeCR:       // Set text to One\\nTwo\\nThree\\n...\\nNine
                    text = "One\r\nTwo\r\nThree\r\nFour\r\nFive\r\nSix\r\nSeven\r\nEight\r\nNine";
                    break;
                case SampleText.Null:                       // set text to be NULL
                    text = null;
                    break;
                case SampleText.WordsAndSpaces:             // Range of 3x words each followed 2 spaces
                    tempText = Helpers.RandomString(8, true);
                    text = tempText + "  " + tempText + "  " + tempText + "  ";
                    break;
                case SampleText.WordsAndPunctuation:        // Range of 3x words each followed by 2 punctuation marks
                    tempText = Helpers.RandomString(8, true);
                    text = tempText + ".." + tempText + ".." + tempText + "..";   // 
                    break;
                case SampleText.ExtraLarge:                 // Text that has lots of lines and is wider than our controls
                    text = "";
                    for (i = 0; i < 10; i++)
                    {
                        tempBuilder.Append(Helpers.RandomString(256, true));
                        tempBuilder.Append("\r\n");
                    }
                    text = tempBuilder.ToString();
                    break;
                case SampleText.DuplicateBlocksOfText:      // Set text to have duplicate blocks of text
                    dupBlock = "<<<" + Helpers.RandomString(randomBlockSize, true) + ">>>";
                    text = Helpers.RandomString(256, true);
                    text = text.Replace(text.Substring(0, randomBlockSize), dupBlock);
                    text = text.Replace(text.Substring(text.Length - randomBlockSize, randomBlockSize), dupBlock);
                    break;
                case SampleText.Unused: // Its SUPPOSED to be a flag used for initial state of the class!!!
                    ThrowMe( CheckType.Verification, ParseType(sampleText) + " should not be explicitly used in a test");
                    break;
                case SampleText.ComplexHidden:              // Set text to complex multi-line text with big block of hidden text
                default:
                    throw new ArgumentException("GetSampleText() has no support for " + ParseType(sampleText));
            }
        }

        //---------------------------------------------------------------------------
        // Count supported text units
        //---------------------------------------------------------------------------
        internal void IdentifySupportedTextUnits(ref TextUnit[] supportedTextUnits)
        {
            // Determine supported text units
            TextLibrary.IdentifySupportedTextUnits(m_le, ref supportedTextUnits);
        }

        //---------------------------------------------------------------------------
        // Tests if start and end points of a range are identical
        //---------------------------------------------------------------------------
        internal bool IsEmptyRange(TextPatternRange range, CheckType checkType)
        {
            int results = 0;

            // We compare if start and end points are the same...
            Range_CompareEndpoints(range, TextPatternRangeEndpoint.Start,
                                    range, TextPatternRangeEndpoint.End, ref results,
                                    null, checkType);

            return (results == 0);
        }

        //---------------------------------------------------------------------------
        // Verify if we are an IPControl
        //---------------------------------------------------------------------------
        internal bool IsIPControl()
        {
            string className;
            string localizedControlType;

            TextLibrary.GetClassName(m_le, out className, out localizedControlType);

            if ((className.LastIndexOf(TextLibrary.EditClassName) > -1)
             && (localizedControlType.LastIndexOf(TextLibrary.OctetName) > -1))
                return true;
            else
                return false;

        }

        //---------------------------------------------------------------------------
        // Performs specific comparison on two integer values
        //---------------------------------------------------------------------------
        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "System.ArgumentException.#ctor(System.String)")]
        static internal void IsMatchingValue(int actual, ValueComparison valueComparison, int expected, string msg, CheckType checkType)
        {
            bool result = false;

            Comment("Expecting " + actual + " " + valueComparison.ToString() + " " +
                        expected);

            switch (valueComparison)
            {
                case ValueComparison.LessThan:
                    result = (actual < expected);
                    break;
                case ValueComparison.Equals:
                    result = (actual == expected);
                    break;
                case ValueComparison.GreaterThan:
                    result = (actual > expected);
                    break;
                default:
                    throw new ArgumentException("IsMatchingValue() has no support for " + ParseType(valueComparison));
            }

            if (result == true)
                Comment(msg + " results matched as expected, ValueComparison = " + Parse(valueComparison));
            else
            {
                Comment("Evaluated " + actual + " " + valueComparison + " " + expected + " as " + result);
                ThrowMe(checkType, msg + " results were not as expected ");
            }
        }

        //---------------------------------------------------------------------------
        // Compares two automation elements for equality and reports/raises exception accordingly
        // Performs a ReferenceCompare as we are working with reference types
        //---------------------------------------------------------------------------
        static internal void IsMatchingAutomationElement(AutomationElement enclosing1, AutomationElement enclosing2, bool expectMatch)
        {
            bool actualMatch = (enclosing1 == enclosing2);
            string expectMatchString = (expectMatch == true ? "matches" : "doesn't match");

            if (actualMatch == expectMatch)
                Comment("Automation Element " + expectMatchString + ", as expected");
            else
                Comment("Automation Element " + expectMatchString + ", not expected results--but not catastrophic error either");
        }

        //---------------------------------------------------------------------------
        // Compares two child automation element arrays for equality and reports/raises exception accordingly
        // Performs a ReferenceCompare as we are working with reference types
        //---------------------------------------------------------------------------
        static internal void IsMatchingChildren(AutomationElement[] child1, AutomationElement[] child2, bool expectMatch)
        {
            bool actualMatch = true;
            string expectMatchString = (expectMatch == true ? "matches" : "doesn't match");

            if (child1.Length != child2.Length)
                actualMatch = false;
            else
            {
                // Compare each child element
                for (int i = 0; i < child1.Length; i++)
                {
                    // Perform equality check, not reference check
                    if ((child1[i] == child2[i]) == false)
                    {
                        actualMatch = false;
                        break;
                    };
                }
            }

            if (actualMatch == expectMatch)
                Comment("Children " + expectMatchString + ", as expected");
            else
                Comment("Children " + expectMatchString + ", not expected results--but not catastrophic error either");
        }

        //---------------------------------------------------------------------------
        // Compares attribute hash tables for equality and reports/raises exception accordingly
        //---------------------------------------------------------------------------
        static internal void IsMatchingDictionary(IDictionary attribs1, IDictionary attribs2, bool expectMatch)
        {
            bool actualMatch = true;
            string expectMatchString = (expectMatch == true ? "matches" : "doesn't match");

            if (attribs1.Count != attribs2.Count)
                actualMatch = false;
            else
            {
                IDictionaryEnumerator enum1 = attribs1.GetEnumerator();
                IDictionaryEnumerator enum2 = attribs2.GetEnumerator();

                while (enum1.MoveNext() && enum2.MoveNext() && actualMatch)
                {
                    bool isArray = false;

                    // Note we have to use Equals() on Value property when
                    // Value property is of type double (perhaps other types as well)
                    if (enum1.Key != enum2.Key)
                    {
                        actualMatch = false;
                        break;
                    }

                    if ((AutomationTextAttribute)enum1.Key == TextPattern.TabsAttribute)
                        isArray = true;
                    else
                        isArray = false;

                    actualMatch = IsMatchingDictionaryItem(enum1.Value, enum2.Value, isArray);

                    if (actualMatch == false)
                        break;
                }
            }

            if (actualMatch == expectMatch)
                Comment("Attributes list " + expectMatchString + ", as expected");
            else
            {
                Comment("Attributes list " + expectMatchString + ", not expected results--but not catastrophic error either");
            }
        }

        //---------------------------------------------------------------------------
        // Validates if two dictionary items are matches. Accounts for things like arrays
        //---------------------------------------------------------------------------
        internal static bool IsMatchingDictionaryItem(object value1, object value2, bool isArray)
        {
            bool returnValue = true;

            // We have to use Equals() as object type might be an array... 
            // Testing against == is inadequate for that scenario
            if (!isArray)
            {
                // Its not an array...
                returnValue = value1.Equals(value2);
            }
            else
            {
                Array a1 = (Array)value1;
                Array a2 = (Array)value2;

                for (int j = 0; j < a1.Length; j++)
                {
                    if (a1.GetValue(j) != a2.GetValue(j))
                    {
                        returnValue = false;
                        break;
                    }
                }
            }

            return returnValue;
        }

        //---------------------------------------------------------------------------
        // Compares two strings for equality and reports/raises exception accordingly
        //---------------------------------------------------------------------------
        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "idx")] // FxCop is incorrectly saying idx is unused.
        static internal void IsMatchingText(string actualText, string expectedText, bool expectMatch, bool allowSubstrings, CheckType checkType)
        {
            int idx = 0;
            bool actualMatch = (actualText == expectedText);
            string expectMatchString = (expectMatch == true ? "matches" : "doesn't match");

            // Strip out \n 
            // This is because the actual text can be a bit... off from our expected text
            // Various controls, and even different versions of controls will add\remove 
            // (mostly remove) \r or \n
            while ((idx = actualText.LastIndexOf("\n")) > 0)
                actualText = actualText.Replace("\n", "");

            while ((idx = expectedText.LastIndexOf("\n")) > 0)
                expectedText = expectedText.Replace("\n", "");

            // Strip out \r as well  
            while ((idx = actualText.LastIndexOf("\r")) > 0)
                actualText = actualText.Replace("\r", "");

            while ((idx = expectedText.LastIndexOf("\r")) > 0)
                expectedText = expectedText.Replace("\r", "");

            // 1st Test: Do we have an exact match
            if (actualMatch == expectMatch)
            {
                Comment("Text " + expectMatchString + ", as expected");
            }

            // 2nd Test: No match yet, but do we allow substring testing, and if so, do we have a substring?
            if ((actualMatch != expectMatch) && (allowSubstrings == true))
            {
                actualMatch = false;  // assume no match

                if (actualText.LastIndexOf(expectedText) > -1) // now prove we have one
                    actualMatch |= true;
                if (expectedText.LastIndexOf(actualText) > -1)
                    actualMatch |= true;
            }

            if (actualMatch == expectMatch)
                Comment("Text " + expectMatchString + ", as expected");
            else
                ThrowMe(checkType, "Text " + expectMatchString + ", they should not ");
        }

        //---------------------------------------------------------------------------
        // Valdiate if exception is a legitimate UIVerify exception raised by a
        // ThrowMe(...) statement
        //---------------------------------------------------------------------------
        static internal bool IsUIVerifyException(Exception ex)
        { 
            if (ex is IncorrectElementConfigurationForTestException)    // Don't count legitimate exceptions
                return true;                                            // used by UIVerify as regression/test failures
            else if (ex is TestWarningException) 
                return true;                     
            else if (ex is TestErrorException)   
                return true;
            else if (ex is KnownProductIssueException)
                return true;
            else
                return false;
        }
    
        //---------------------------------------------------------------------------
        // Recursively iterate through children, grandchildrent, etc.
        //---------------------------------------------------------------------------
        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "System.ArgumentException.#ctor(System.String)")]
        internal void RecurseChildAutomationElements(int recurseLevel, TextPatternRange callingRange, CheckType checkType)
        {
            AutomationElement parent = null;
            AutomationElement[] children = null;

            if (recurseLevel > recurseLevelMax)
                throw new ArgumentException("Arbitrary sanity check: RecurseChildAutomationElement likely should not have exceeded " + recurseLevelMax + " generations of children for a given document");

            if (callingRange == null)
                throw new ArgumentException("RecurseChildAutomationElement requires non-null callingRange");

            // Get child and parent automation elements
            Range_GetEnclosingElement(callingRange, ref parent, null, checkType);
            Range_GetChildren(callingRange, ref children, checkType);

            Comment("Level " + recurseLevel + " has " + children.Length + " child Automation Elements");

            for (int i = 0; i < children.Length; i++)
            {
                int childrenCount;
                TextPatternRange childRange = null;

                // Get child range
                Pattern_RangeFromChild(_pattern, ref childRange, children[i], null, checkType);

                // Validate child range
                if (childRange == null)
                    ThrowMe(checkType, "Level " + recurseLevel + ", Child[" + i + "] incorrectly returned null range.");

                childrenCount = childRange.GetChildren().Length;

                Comment("Level " + recurseLevel + ", Child[" + i + "] range has " +
                        childRange.GetChildren().Length + " child elements.");

                if (childrenCount > 0)
                    RecurseChildAutomationElements(recurseLevel + 1, childRange, checkType);

                // Perform value equality to validate instances have same identity
                VerifyAutomationElement(parent, children[i], checkType);
            }
        }

        //---------------------------------------------------------------------------
        // standard header for regression tests
        //---------------------------------------------------------------------------
        internal void TS_RegressionTest(string bugID, string controls, string bugTitle)
        {
            Comment("Regression Test");
            Comment("   Bug ID         = " + bugID);
            Comment("   Bug Title      = " + bugTitle);
            Comment("   Controls       = " + controls);
            Comment("");
            Comment("NOTE: If there is a regression, it may not occur on the originally reported controls");
            Comment("");
            
            m_TestStep++;
        }

        //---------------------------------------------------------------------------
        // Do we reset the _sampleText var. based on automation elements runtime ID?
        // This is for the scenario where more than one /ID #### is used on the 
        // command line UIVerify. Edit and RichEdit controls will have different
        // text size, even for the same value of sampleText. Our SetText() method 
        // will try to save time by not resetting the text of a control unless
        // absolutely necessary: Conditions which make it necessary:
        //  - A different value of SampleText is sent to SetText() than the last
        //    call to SetText()
        //  - The previous call to SetText() was for a DIFFERENT automation element
        // The logic for SetText handles the first case. This method identifies
        // the second case and resets the global _sampleText variable accordingly
        //---------------------------------------------------------------------------
        internal void ResetSampleText()
        {
            int[] runtimeID = m_le.GetRuntimeId();
            
            if (_runtimeID.Length == 0)
                _sampleText = SampleText.Unused; // reset
            else
            {
                if ( runtimeID.Length != _runtimeID.Length )    // should never happen (we hope)
                    _sampleText = SampleText.Unused;            // reset anyway
                else
                {
                    for( int i = 0; i < runtimeID.Length; i++ )
                    {
                        if( runtimeID[i] != _runtimeID[i] )
                        {
                            _sampleText = SampleText.Unused;    // reset
                        }
                    }                
                }
            }
            _runtimeID = runtimeID;
        }

        //---------------------------------------------------------------------------
        // Overload of SetText. Most commonly used flavor, relies upon pre-defined
        // string values (associated with sampleText) to set text int he control
        //---------------------------------------------------------------------------
        internal void SetText(SampleText sampleText, out string actualText, CheckType checkType)
        {
            string originalText = "";
            
            GetSampleText(sampleText, ref originalText);  // Text to assign to control
            
            SetText(sampleText, originalText, out actualText, checkType);
        }

        //---------------------------------------------------------------------------
        // SetText will set the text of a control to a specific value
        // Caller may, optionally, set the text the want to set into the control
        // Going to violate convention and hardcode the CheckType arguments in this method
        // originalText is either...
        // A) Text of the control before this call was made
        // B) Textstring sent to the control (without any pesky \r, \n or \r\n that
        //    Win32 RichEdit automatically adds to any string it contains)
        //---------------------------------------------------------------------------
        internal void SetText(SampleText sampleText, string textToSet, out string actualText, CheckType checkType)
        {
            TextPatternRange    documentRange = Pattern_DocumentRange(checkType);;
            bool                isEmptyRange  = IsEmptyRange(documentRange, checkType);
            bool                setText       = true;
            
            actualText = "";

            Comment("ENTERING SetText(" + Parse(sampleText) + ",...)");

            // Sanity check: verify usage due to overloads...
            if( String.IsNullOrEmpty(textToSet) && (sampleText == SampleText.Custom) )
            {
                throw new ArgumentException("Incorrect usage. SampleText.Custom requires non-null textToSet argument");
            }

            // Sanity check: No IP Control support
            if (IsIPControl())
            {
                ThrowMe(checkType, "TextPattern tests that set text are not supported on SysIPAddress32 control");
            }

            // Sanity check: caller cannot pass in SampleText.Unused
            if( sampleText == SampleText.Unused )
            {
                ThrowMe(checkType, "Incorrect usage. You cannot call SetText(SampleText.Unused...)");
            }
            
            // State check: Is this first time through *THIS* method for the current automation element?
            // Only applies to cmd line usage with /ID <id1> /ID <id2> otherwise this method should 
            // esentaially equate a no-op
            ResetSampleText();

            // Okay... process the request...
            if( (isEmptyRange == true) && (sampleText == SampleText.Empty) )
            {
                // Special Case: sampleText = SampleText.Empty
                // We want empty, we got empty(!)
                textToSet  = "";
                actualText = "";
                
                Comment("   Control is already empty");
                setText = false;
            }
            else if( (isEmptyRange == false) && (sampleText == SampleText.NotEmpty))
            {
                // Special Case: sampleText = SampleText.NotEmpty
                // We want non-empty, we got non-empty(!)
                Range_GetText(documentRange, ref actualText, -1, null, checkType);
                if( (actualText.Length + _trailingCRLFCount) != 0) 
                {
                    Comment("   Control is already non-empty");
                    textToSet = actualText;
                    setText   = false;
                }
                else
                    textToSet = easyText;
            }
            else if( sampleText == _sampleText )
            {
                // Special case: We already have the desired text, so continue
                actualText = _actualText;
                setText    = false;
                Comment("   Control already has correct text");       
            }
            else if( _noClobber == true )
            {
                // Special case. If test doesn't care what text is there, continue else bail
                if( sampleText == SampleText.NotApplicable )
                {
                    actualText = _actualText;
                    Comment("   Unable to alter text of control due to /NOCLOBBER switch, continuing with the test though"); 
                    setText = false;
                }
                else
                   ThrowMe(checkType, "Unable to alter text of control due to /NOCLOBBER switch"); 
            }

            if( setText == true )
            {            
                // Get new text to shove into the control
                if( sampleText != SampleText.Custom )
                {
                    GetSampleText(sampleText, ref textToSet);  // Text to assign to control
                }
                
                SetValue(textToSet, checkType);            // assign it!

                documentRange = Pattern_DocumentRange(checkType);   // Any trialing \r \n added?
                Range_GetText(documentRange, ref actualText, -1, null, checkType);

                // Look for any discrpenecies based on trailing /r /n added to the text
                // i.e. this is the diff between textToSet.length and actualText.Length
                _trailingCRLFCount = TextLibrary.CountTrailingCRLF(m_le, documentRange);

                if (_trailingCRLFCount != 0)
                {
                    Comment("   Count of trailing \\r and/or \\n = " + (-1 * _trailingCRLFCount));
                    Comment("   Requested Text Length          = " + textToSet.Length);
                    Comment("   Requested Text                 = " + textToSet);
                }
            }

            Comment("   Actual Text Length             = " + actualText.Length);
            Comment("   Actual Text                    = " + actualText);

            _originalText = textToSet;
            _actualText   = actualText;
            _sampleText   = sampleText;            

            Comment("    EXITING SetText(" + ParseType(sampleText) + ",...)");
        }

        //---------------------------------------------------------------------------
        // Set value will cast the value to the correct (or incorrect type--as appropriate)
        //---------------------------------------------------------------------------
        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "System.ArgumentException.#ctor(System.String)")]
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        static internal void SetValue(TypeValue typeValue, AutomationTextAttribute key, ref object value)
        {
            switch (typeValue)
            {
                case TypeValue.MatchesTypeAndValue:                      // CORRECT type and value
                case TypeValue.WrongValue:                               // CORRECT type: INCORRECT value
                case TypeValue.WrongEnumValue:                           // Incorrect enum value
                    break; // Do nothing, casts that follow below will work
                case TypeValue.Null:                                     // NULL
                case TypeValue.WrongTypeAndValue:                        // INCORRECT type and value
                case TypeValue.WrongType:                                // INCORRECT type: CORRECT value
                    // no casts, we're done, type is handled elsewhere
                    return;
                default:
                    throw new ArgumentException("No support for " + ParseType(typeValue));
            }

            // Switch statement requires integral types, have to use a massive if-else instead.
            if (key == TextPattern.AnimationStyleAttribute)
            {
                value = (AnimationStyle)value;
            }
            else if ((key == TextPattern.IsHiddenAttribute)
                  || (key == TextPattern.IsItalicAttribute)
                  || (key == TextPattern.IsReadOnlyAttribute)
                  || (key == TextPattern.IsSubscriptAttribute)
                  || (key == TextPattern.IsSuperscriptAttribute))
            {
                value = (bool)value;
            }
            else if (key == TextPattern.BulletStyleAttribute)
            {
                value = (BulletStyle)value;
            }
            else if (key == TextPattern.CapStyleAttribute)
            {
                value = (CapStyle)value;
            }
            else if (key == TextPattern.CultureAttribute)
            {
                value = (CultureInfo)value;
            }
            else if ((key == TextPattern.FontSizeAttribute)
                  || (key == TextPattern.IndentationFirstLineAttribute)
                  || (key == TextPattern.IndentationLeadingAttribute)
                  || (key == TextPattern.IndentationTrailingAttribute)
                  || (key == TextPattern.MarginBottomAttribute)
                  || (key == TextPattern.MarginLeadingAttribute)
                  || (key == TextPattern.MarginTopAttribute)
                  || (key == TextPattern.MarginTrailingAttribute))
            {
                value = (double)value;
            }
            else if (key == TextPattern.TabsAttribute)
            {
                value = (double[])value;
            }
            else if (key == TextPattern.TextFlowDirectionsAttribute)
            {
                value = (FlowDirections)value;
            }
            else if (key == TextPattern.HorizontalTextAlignmentAttribute)
            {
                value = (HorizontalTextAlignment)value;
            }
            else if ((key == TextPattern.BackgroundColorAttribute)
                  || (key == TextPattern.FontWeightAttribute)
                  || (key == TextPattern.ForegroundColorAttribute)
                  || (key == TextPattern.StrikethroughColorAttribute)
                  || (key == TextPattern.UnderlineColorAttribute)
                  || (key == TextPattern.OverlineColorAttribute))
            {
                value = (int)value;
            }
            else if (key == TextPattern.OutlineStylesAttribute)
            {
                value = (OutlineStyles)value;
            }
            else if (key == TextPattern.FontNameAttribute)
            {
                value = (string)value;
            }
            else if ((key == TextPattern.OverlineStyleAttribute)
                  || (key == TextPattern.StrikethroughStyleAttribute)
                  || (key == TextPattern.UnderlineStyleAttribute))
            {
                value = (TextDecorationLineStyle)value;
            }
            else
            {
                throw new ArgumentException("SetValue() has no support for AutomationTextAttribute " + Helpers.GetProgrammaticName(key));
            }
        }

        //---------------------------------------------------------------------------
        // SetValue will set the text of a control to a provided value
        //---------------------------------------------------------------------------
        internal void SetValue(string actualText, CheckType checkType)
        {
            pattern_IsFocusable(m_le, checkType); // Either returns quietly or raises a ThrowMe

            // Sanity Check
            if (_noClobber == true)
            {
                ThrowMe(checkType, "/NOCLOBBER flag used, unable to set the text of control to: " + 
                        ( string.IsNullOrEmpty(actualText) 
                            ? TrimText(actualText, 512) 
                            : "any value") );
            }

            //



            // Display actual text, don't trim too much, comes in handy later
            Comment("Text Length = " + actualText.Length);
            Comment("Text        = " + TrimText(actualText, 512));
        }

        /// <summary>
        /// use WPF behavior to set value
        /// </summary>
        /// <param name="element">the AutomationElement to set the value to</param>
        /// <param name="actualText">the value to set</param>
        internal void SetValueForAE(AutomationElement element, string actualText)
        {
            ValuePattern vp = element.GetCurrentPattern(ValuePattern.Pattern) as ValuePattern;            
            vp.SetValue(actualText);
        }

        //---------------------------------------------------------------------------
        // SetWrongType will attempt to change the type of the object while still
        // retaining its current value.
        //---------------------------------------------------------------------------
        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "System.ArgumentException.#ctor(System.String)")]
        static internal void SetWrongType(ref object value)
        {
            int intTemp = 0;
            bool boolTemp = false;
            double doubleTemp = 0;
            string stringTemp = "";
            string typeString = value.GetType().ToString();

            if (value == AutomationElement.NotSupported)
            {
                float floatTemp = 0.0F; // A type that no attribute uses as of 7/7/05
                value = (object)floatTemp;
                return;
            }

            switch (typeString)
            {
                case "System.Windows.Automation.AutomationIdentifier":  // Do nothing, not supported by control
                    break;
                case "System.Double[]":
                    value = (object)intTemp;
                    break;
                case "System.Int32":
                    doubleTemp = Convert.ToDouble((int)value);
                    value = (object)doubleTemp;
                    break;
                case "System.Boolean":
                    intTemp = Convert.ToInt32((bool)value);
                    value = (object)intTemp;
                    break;
                case "System.Double":
                    stringTemp = ((double)value).ToString(CultureInfo.InvariantCulture);
                    value = (object)stringTemp;
                    break;
                case "System.String":
                    value = (object)boolTemp;
                    break;
                case "System.Windows.Automation.Text.AnimationStyle":
                case "System.Windows.Automation.Text.BulletStyle":
                case "System.Windows.Automation.Text.CapStyle":
                case "System.Windows.Automation.Text.HorizontalTextAlignment":
                case "System.Windows.Automation.Text.OutlineStyles":
                case "System.Windows.Automation.Text.TextDecorationLineStyle":
                case "System.Windows.Automation.Text.FlowDirections":
                    intTemp = Convert.ToInt32(value, CultureInfo.InvariantCulture);
                    value = (object)intTemp;
                    break;
                case "System.Globalization.CultureInfo":
                default:
                    throw new ArgumentException("SetWrongType has no support for " + typeString);
            }
        }

        //---------------------------------------------------------------------------
        // SetWrongValue will change the value of the object while still
        // retaining its current type.
        //---------------------------------------------------------------------------
        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "System.ArgumentException.#ctor(System.String)")]
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        static internal void SetWrongValue(ref object value, AutomationTextAttribute key)
        {
            double[] doubleArray = new double[0];
            string typeString = value.GetType().ToString();

            // If not supported, goto separate helper method
            if (value == AutomationElement.NotSupported)
            {
                SetWrongValueNotSupported(ref value, key);
            }
            else
            {
                switch (typeString)
                {
                    case "System.Windows.Automation.AutomationIdentifier":  // Do nothing, not supported by control
                        break;
                    case "System.Double[]":
                        doubleArray = (double[])value;
                        for (int i = 0; i < doubleArray.Length; i++)
                            doubleArray[i] = doubleArray[i] + 1;
                        value = (object)doubleArray;
                        break;
                    case "System.Int32":
                        value = (object)(((int)value) + 1);
                        break;
                    case "System.Boolean":
                        value = (object)(!((bool)value));
                        break;
                    case "System.Double":
                        value = (object)(((double)value) + 1);
                        break;
                    case "System.String":
                        value = (object)(((string)value) + "x");
                        break;
                    case "System.Windows.Automation.Text.AnimationStyle":
                        if (((AnimationStyle)value) == AnimationStyle.BlinkingBackground)
                            value = (object)AnimationStyle.LasVegasLights;
                        else
                            value = (object)AnimationStyle.BlinkingBackground;
                        break;
                    case "System.Windows.Automation.Text.BulletStyle":
                        if (((BulletStyle)value) == BulletStyle.DashBullet)
                            value = (object)BulletStyle.FilledRoundBullet;
                        else
                            value = (object)BulletStyle.DashBullet;
                        break;
                    case "System.Windows.Automation.Text.CapStyle":
                        if (((CapStyle)value) == CapStyle.AllCap)
                            value = (object)CapStyle.AllPetiteCaps;
                        else
                            value = (object)CapStyle.AllCap;
                        break;
                    case "System.Windows.Automation.Text.HorizontalTextAlignment":
                        if (((HorizontalTextAlignment)value) == HorizontalTextAlignment.Centered)
                            value = (object)HorizontalTextAlignment.Justified;
                        else
                            value = (object)HorizontalTextAlignment.Centered;
                        break;
                    case "System.Windows.Automation.Text.OutlineStyles":
                        if (((OutlineStyles)value) == OutlineStyles.Shadow)
                            value = (object)OutlineStyles.Outline;
                        else
                            value = (object)OutlineStyles.Shadow;
                        break;
                    case "System.Windows.Automation.Text.TextDecorationLineStyle":
                        if (((TextDecorationLineStyle)value) == TextDecorationLineStyle.Dash)
                            value = (object)TextDecorationLineStyle.DashDot;
                        else
                            value = (object)TextDecorationLineStyle.Dash;
                        break;
                    case "System.Windows.Automation.Text.FlowDirections":
                        if (((FlowDirections)value) == FlowDirections.BottomToTop)
                            value = (object)FlowDirections.Default;
                        else
                            value = (object)FlowDirections.BottomToTop;
                        break;
                    case "System.Globalization.CultureInfo":
                    default:
                        throw new ArgumentException("SetWrongValue has no support for " + typeString);
                }
            }
        }

        //---------------------------------------------------------------------------
        // SetWrongValue will change the value of the object while still
        // retaining its current type for attributes not supported.
        //---------------------------------------------------------------------------
        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "System.ArgumentException.#ctor(System.String)")]
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        static internal void SetWrongValueNotSupported(ref object value, AutomationTextAttribute key)
        {
            int intType = 0;     // actual values for these variables is not relevant
            bool boolType = false; // We just need the type, and for that we need an instance
            double doubleType = 0;     // of each type
            double[] doubleArrayType = new double[0];
            string stringType = "";
            AnimationStyle animationStyleType = AnimationStyle.BlinkingBackground;
            BulletStyle bulletStyleType = BulletStyle.DashBullet;
            CapStyle capStyleType = CapStyle.AllCap;
            CultureInfo cultureInfoType = new CultureInfo(CultureInfo.CurrentCulture.ToString());
            HorizontalTextAlignment horizontalTextAlignmentType = HorizontalTextAlignment.Centered;
            OutlineStyles outlineStylesType = OutlineStyles.Shadow;
            TextDecorationLineStyle textDecorationLineStyleType = TextDecorationLineStyle.Dash;
            FlowDirections flowDirectionsType = FlowDirections.Default;

            // Switch statement requires integral types, have to use a massive if-else instead.
            if (key == TextPattern.AnimationStyleAttribute)
            {
                value = (object)animationStyleType;
            }
            else if ((key == TextPattern.IsHiddenAttribute)
                  || (key == TextPattern.IsItalicAttribute)
                  || (key == TextPattern.IsReadOnlyAttribute)
                  || (key == TextPattern.IsSubscriptAttribute)
                  || (key == TextPattern.IsSuperscriptAttribute))
            {
                value = (object)boolType;
            }
            else if (key == TextPattern.BulletStyleAttribute)
            {
                value = (object)bulletStyleType;
            }
            else if (key == TextPattern.CapStyleAttribute)
            {
                value = (object)capStyleType;
            }
            else if (key == TextPattern.CultureAttribute)
            {
                value = (object)cultureInfoType;
            }
            else if ((key == TextPattern.FontSizeAttribute)
                  || (key == TextPattern.IndentationFirstLineAttribute)
                  || (key == TextPattern.IndentationLeadingAttribute)
                  || (key == TextPattern.IndentationTrailingAttribute)
                  || (key == TextPattern.MarginBottomAttribute)
                  || (key == TextPattern.MarginLeadingAttribute)
                  || (key == TextPattern.MarginTopAttribute)
                  || (key == TextPattern.MarginTrailingAttribute))
            {
                value = (object)doubleType;
            }
            else if (key == TextPattern.TabsAttribute)
            {
                value = (object)doubleArrayType;
            }
            else if (key == TextPattern.TextFlowDirectionsAttribute)
            {
                value = (object)flowDirectionsType;
            }
            else if (key == TextPattern.HorizontalTextAlignmentAttribute)
            {
                value = (object)horizontalTextAlignmentType;
            }
            else if ((key == TextPattern.BackgroundColorAttribute)
                  || (key == TextPattern.FontWeightAttribute)
                  || (key == TextPattern.ForegroundColorAttribute)
                  || (key == TextPattern.StrikethroughColorAttribute)
                  || (key == TextPattern.UnderlineColorAttribute)
                  || (key == TextPattern.OverlineColorAttribute))
            {
                value = (object)intType;
            }
            else if (key == TextPattern.OutlineStylesAttribute)
            {
                value = (object)outlineStylesType;
            }
            else if (key == TextPattern.FontNameAttribute)
            {
                value = (object)stringType;
            }
            else if ((key == TextPattern.OverlineStyleAttribute)
                  || (key == TextPattern.StrikethroughStyleAttribute)
                  || (key == TextPattern.UnderlineStyleAttribute))
            {
                value = (object)textDecorationLineStyleType;
            }
            else
            {
                throw new ArgumentException("CastAttributeToCorrectType() has no support for AutomationTextAttribute " + Helpers.GetProgrammaticName(key));
            }
        }

        //---------------------------------------------------------------------------
        // Trim test string to a fixed size, adding "..." if longer than allowed
        // not an actual test step
        //---------------------------------------------------------------------------
        static internal string TrimText(string text, int stringSize)
        {
            if (text == null)
                return "";

            if (text.Length > stringSize)
                text = text.Substring(0, stringSize) + "... (Trimmed for length, length = " + text.Length + ")";
            return text;
        }

        //---------------------------------------------------------------------------
        // Verifies that actual text = or is subset of expected text
        //---------------------------------------------------------------------------
        static internal void TextWithin(string methodTested, string actualText, string expectedText, CheckType checkType)
        {
            // Case 1:  (success) actualText matches expectedText
            // Case 2:  (success) actualText is a subset of expectedText
            // Case 3:  (failure) actualText bares no relation to expectedText

            Comment("actualText   length = " + actualText.Length + ", text = '" + TrimText(actualText, 512) + "'");
            Comment("expectedText length = " + expectedText.Length + ", text = '" + TrimText(expectedText, 512) + "'");

            if (actualText == expectedText)                 // Case 1
                Comment(methodTested + "() Actual text equals expected text, test passes");
            else if (actualText.IndexOf(expectedText) > -1)  // Case 2
                Comment(methodTested + "() Expected text is a subset of actual text, test passes");
            else if (expectedText.IndexOf(actualText) > -1)  // Case 3
                Comment(methodTested + "() Actual text is a subset of expected text, test passes");
            else
                ThrowMe(checkType, methodTested + ": Actual text bares no relation to expected text, test has failed ");
        }

        ///---------------------------------------------------------------------------
        /// <summary>
        /// Trims trailing CRLF from a string
        /// 


        [SuppressMessage("Microsoft.Naming", "CA1705:LongAcronymsShouldBePascalCased", MessageId = "Member")]
        [SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#")]
        public static void TrimTrailingCRLF(AutomationElement autoElement, ref string text)
        {
            if (string.IsNullOrEmpty(text) == true)
                return;

            if (TextLibrary.IsRichEdit(autoElement) == true)
            {
                int idx = 0;

                //---------------------------------------------------
                // Case 1: Trailing \r\n, newer versions of RichEdit
                //---------------------------------------------------
                idx = text.LastIndexOf("\r\n");
                // What if idx = -1? Don't want to trim anyway so idx must be >= 0
                if ((idx >= 0) && (idx == (text.Length - 2)))
                {
                    text = text.Substring(0, text.Length - 2);
                    Comment("Trimmed trailing CRLF from text");
                    return;
                }

                //---------------------------------------------------
                // Case 2: Trailing \r, older versions of RichEdit
                //---------------------------------------------------
                idx = text.LastIndexOf('\r');
                // What if idx = -1? Don't want to trim anyway so idx must be >= 0
                if ((idx >= 0) && (idx == (text.Length - 1)))
                {
                    text = text.Substring(0, text.Length - 1);
                    Comment("Trimmed trailing CR from text");
                    return;
                }
            }
        }

        //---------------------------------------------------------------------------
        // Verify value equality of two automation elements
        //---------------------------------------------------------------------------
        static internal void VerifyAutomationElement(AutomationElement autoElement1, AutomationElement autoElement2, CheckType checkType)
        {
            if (autoElement1 != autoElement2)
                ThrowMe(checkType, "Automation elements do not match, expected matching runtimeID's");
            else
                Comment("Automation elements match, as expected");
        }

        #endregion misc
        
        #region 6.1 Regression Tests

        //-------------------------------------------------------------------
        // Regress 

        internal void Bug296( string bugID, string controls, string bugTitle )
        {
            int  count         = -1;
            int  actualCount   = 0;
            TextPatternRange range       = null;

            TS_RegressionTest(bugID, controls, bugTitle);

            try
            {
                string actualText = "";
                SetText(SampleText.String1, out actualText, CheckType.IncorrectElementConfiguration);
                range = Pattern_DocumentRange(CheckType.Verification);
                Range_MoveEndpointByUnit(range, TextPatternRangeEndpoint.End, TextUnit.Document, count, ref actualCount, null, CheckType.IncorrectElementConfiguration);

                Range_ScrollIntoView(range, true, null, CheckType.Verification);

                Comment("As expected: No exception raised");
            }
            catch (Exception ex)
            {
                if (IsUIVerifyException(ex))  // Don't interfere with legitimate ThrowMe(...)
                    throw;
            
                if (ex is InvalidOperationException)
                    ThrowMe(CheckType.Verification, "Bug has regressed, InvalidOperationException raised");
                else
                    ThrowMe(CheckType.Verification, "Unexpected exception encountered:\n" + ex.ToString());
            }
        }

        //-------------------------------------------------------------------
        // Regress 

        internal void Bug297( string bugID, string controls, string bugTitle )
        {
            TextPatternRange range = null;

            TS_RegressionTest(bugID, controls, bugTitle);

            try
            {
                string actualText = "";
                SetText(SampleText.String1, out actualText, CheckType.IncorrectElementConfiguration);
                range = Pattern_DocumentRange(CheckType.Verification);

                try
                {
                    string s = Range_GetText(range);
                }
                catch (ArgumentOutOfRangeException)
                {
                    ThrowMe(CheckType.Verification, "GetText(-1) incorrectly raised ArugmentOutOfRangeException");
                }

                Comment("As expected: GetText(-1) did not raise ArugmentOUtOfRangeException");
            }
            catch (Exception ex)
            {
                if (IsUIVerifyException(ex))  // Don't interfere with legitimate ThrowMe(...)
                    throw;

                ThrowMe(CheckType.Verification, "Unexpected exception encountered:\n" + ex.ToString());
            }

        }

        //-------------------------------------------------------------------
        // Regress 

        internal void Bug298( string bugID, string controls, string bugTitle )
        {
            int    count       = 1;
            int    actualCount = 0;
            TextPatternRange range       = null;
            TextPatternRange clonedRange = null;

            TS_RegressionTest(bugID, controls, bugTitle);

            if( (TextLibrary.IsRichEdit(m_le) == true)
             || (TextLibrary.IsMultiLine(m_le) == true))
                ThrowMe(CheckType.IncorrectElementConfiguration, "Bug only applies to Win32 single-line Edit Controls");

            try
            {
                string actualText = "";
                SetText(SampleText.String1, out actualText, CheckType.IncorrectElementConfiguration);
                range = Pattern_DocumentRange(CheckType.Verification);

                // Iterate through all text units even if one of them raises an error.
                for (TextUnit tu = TextUnit.Character; (tu <= TextUnit.Document) ; tu++)
                {
                    actualCount = 0;
                    
                    Range_Clone( range, ref clonedRange, null, CheckType.IncorrectElementConfiguration);

                    Range_MoveEndpointByUnit(clonedRange, TextPatternRangeEndpoint.Start, tu, count, ref actualCount, null, CheckType.IncorrectElementConfiguration);
                        
                    if (actualCount != 1)
                    {
                        // Soft error, process all text units
                        ThrowMe(CheckType.Verification, "Should have returned " + count + " for TextUnit " + tu.ToString() + ", actual = " + actualCount);
                    }
                }

                Comment("As expected: Returned 1 from MoveEndpointByUnit");
            }
            catch (Exception ex)
            {
                if (IsUIVerifyException(ex))  // Don't interfere with legitimate ThrowMe(...)
                    throw;

                ThrowMe(CheckType.Verification, "Unexpected exception encountered:\n" + ex.ToString());
            }
        }

        //-------------------------------------------------------------------
        // Regress 

        internal void Bug7( string bugID, string controls, string bugTitle )
        {
            int count          = 1;
            int actualCount    = 0;
            TextPatternRange range       = null;
            TextPatternRange clonedRange = null;


            TS_RegressionTest(bugID, controls, bugTitle);

            if (TextLibrary.IsRichEdit(m_le) == true)
                ThrowMe(CheckType.IncorrectElementConfiguration, "Bug only applies to Win32 single-line Edit Controls");

            Comment("NOTE: Bug regression will be on TextUnit.Line");

            try
            {
                string actualText = "";
                SetText(SampleText.String1, out actualText, CheckType.IncorrectElementConfiguration);
                range = Pattern_DocumentRange(CheckType.Verification);

                // Iterate through all text units even if one of them raises an error.
                for (TextUnit tu = TextUnit.Character; (tu <= TextUnit.Document); tu++)
                {
                    Range_Clone(range, ref clonedRange, null, CheckType.IncorrectElementConfiguration);
                    Range_MoveEndpointByUnit(clonedRange, TextPatternRangeEndpoint.End, TextUnit.Character, count, ref actualCount, null, CheckType.IncorrectElementConfiguration);

                }

                Comment("As expected: MoveEndPointByUnit returned 0 in all TextUnits");
            }
            catch (Exception ex)
            {
                if (IsUIVerifyException(ex))  // Don't interfere with legitimate ThrowMe(...)
                    throw;

                ThrowMe(CheckType.Verification, "Unexpected exception encountered:\n" + ex.ToString());
            }
        }

        //-------------------------------------------------------------------
        // Regress 

        internal void Bug6( string bugID, string controls, string bugTitle )
        {
            string textToFind  = "";
            string textFound   = "";
            TextPatternRange range       = null;
            TextPatternRange targetRange = null;

            TS_RegressionTest(bugID, controls, bugTitle);

            if (TextLibrary.IsRichEdit(m_le) == true)
                ThrowMe(CheckType.IncorrectElementConfiguration, "Bug only applies to Win32 single-line Edit Controls");

            try
            {
                string actualText = "";
                SetText(SampleText.String1, out actualText, CheckType.IncorrectElementConfiguration);
                range = Pattern_DocumentRange(CheckType.Verification);

                Range_GetText(range, ref textToFind, -1, null, CheckType.IncorrectElementConfiguration);

                Range_FindText( range, ref targetRange, textToFind, false, false, null, CheckType.IncorrectElementConfiguration);

                Range_GetText(targetRange, ref textFound, -1, null, CheckType.IncorrectElementConfiguration);

                if (textToFind != textFound)
                {
                    Comment("GetText  returned string length " + textToFind.Length.ToString().PadLeft(3) + ", text = " + textToFind);
                    Comment("FindText returned string length " + textFound.Length.ToString().PadLeft(3) + ", text = " + textFound);
                    if( (textFound.Length + 1) == (textToFind.Length))
                        ThrowMe(CheckType.Verification, "GetText/FindText did not return matching strings, bug has regressed");
                    else
                        ThrowMe(CheckType.Verification, "GetText/FindText did not return matching strings, bug hasn't regressed, but new behavior has appeared");
                }

                Comment("As expected: GetText/FindText returned matching strings");
            }
            catch (Exception ex)
            {
                if (IsUIVerifyException(ex))  // Don't interfere with legitimate ThrowMe(...)
                    throw;

                ThrowMe(CheckType.Verification, "Unexpected exception encountered:\n" + ex.ToString());
            }
        }

        //-------------------------------------------------------------------
        // Regress 


        internal void Bug299( string bugID, string controls, string bugTitle )
        {
            TextPatternRange range  = null;

            TS_RegressionTest(bugID, controls, bugTitle);

            try
            {
                string actualText = "";
                SetText(SampleText.String1, out actualText, CheckType.IncorrectElementConfiguration);
                range = Pattern_DocumentRange(CheckType.Verification);

                range.ScrollIntoView(false);

                TextPatternRange [] visibleRanges = _pattern.GetVisibleRanges();

                Comment("As expected: Acquired Visible Range");
                Comment("Assertion should have been raised if bug has regressed");
            }
            catch (Exception ex)
            {
                if (IsUIVerifyException(ex))  // Don't interfere with legitimate ThrowMe(...)
                    throw;

                ThrowMe(CheckType.Verification, "Unexpected exception encountered:\n" + ex.ToString());
            }

        }

        //-------------------------------------------------------------------
        // Regress 

        internal void Bug300( string bugID, string controls, string bugTitle )
        {
            TextPatternRange range  = null;

            TS_RegressionTest(bugID, controls, bugTitle);

            try
            {
                string actualText = "";
                SetText(SampleText.String1, out actualText, CheckType.IncorrectElementConfiguration);
                range = Pattern_DocumentRange(CheckType.Verification);

                ArgumentException     aeEx  = new ArgumentException();
                ArgumentNullException aneEx = new ArgumentNullException();
                TextPatternRange targetRange = null;

                Range_FindText( range, ref targetRange, null, false, false, aneEx.GetType(), CheckType.IncorrectElementConfiguration );
                
                Range_FindText( range, ref targetRange, "",   false, false, aeEx.GetType(), CheckType.IncorrectElementConfiguration );

                Comment("FindText raised correct exceptions");
            }
            catch (Exception ex)
            {
                if (IsUIVerifyException(ex))  // Don't interfere with legitimate ThrowMe(...)
                    throw;

                ThrowMe(CheckType.Verification, "Unexpected exception encountered:\n" + ex.ToString());
            }
        }

        //-------------------------------------------------------------------
        // Regress 

        internal void Bug301( string bugID, string controls, string bugTitle )
        {
            Rect[] boundRects = new Rect[0];
            
            TextPatternRange range       = null;

            TS_RegressionTest(bugID, controls, bugTitle);

            if (TextLibrary.IsRichEdit(m_le) == true)
                ThrowMe(CheckType.IncorrectElementConfiguration, "Bug only applies to Win32 single-line Edit Controls");

            try
            {
                string actualText = "";
                SetText(SampleText.String1, out actualText, CheckType.IncorrectElementConfiguration);
                range = Pattern_DocumentRange(CheckType.Verification);

                Range_GetBoundingRectangles(range, ref boundRects, null, CheckType.IncorrectElementConfiguration);

                if (boundRects == null)
                {
                    ThrowMe(CheckType.Verification, "Unexpected error, bounding rectangles returned should not be 0 (new bug)");
                }
                else
                {
                    Comment("As expected: GetBoundingRectangles() called without raising ComException");
                }
            }
            catch (Exception ex)
            {
                if (IsUIVerifyException(ex))  // Don't interfere with legitimate ThrowMe(...)
                    throw;

                if (ex is COMException)
                    ThrowMe(CheckType.Verification, "Bug has likely regress, COMException raised.\n" + ex.ToString());
                else
                    ThrowMe(CheckType.Verification, "Unexpected exception encountered:\n" + ex.ToString());
            }
        }

        //-------------------------------------------------------------------
        // Regress 

        internal void Bug302( string bugID, string controls, string bugTitle )
        {
            TextPatternRange   range         = null;
            TextPatternRange[] visibleRanges = null;

            TS_RegressionTest(bugID, controls, bugTitle);

            if (TextLibrary.IsRichEdit(m_le) == true)
                ThrowMe(CheckType.IncorrectElementConfiguration, "Bug only applies to Win32 single-line Edit Controls");

            try
            {
                string actualText = "";
                SetText(SampleText.String1, out actualText, CheckType.IncorrectElementConfiguration);
                range = Pattern_DocumentRange(CheckType.Verification);
                
                Pattern_GetVisibleRanges(_pattern, ref visibleRanges, null, CheckType.Verification);

                Comment("As expected: GetVisibleRange did not raise an assertion");
            }
            catch (Exception ex)
            {
                if (IsUIVerifyException(ex))  // Don't interfere with legitimate ThrowMe(...)
                    throw;

                ThrowMe(CheckType.Verification, "Unexpected exception encountered:\n" + ex.ToString());
            }
        }

        //-------------------------------------------------------------------
        // Regress 

        internal void Bug303( string bugID, string controls, string bugTitle )
        {
            int  actualCount   = 0;
            string text        = "";
            TextPatternRange range       = null;
            TextPatternRange clonedRange = null;

            TS_RegressionTest(bugID, controls, bugTitle);

            if (TextLibrary.IsRichEdit(m_le) == true)
                ThrowMe(CheckType.IncorrectElementConfiguration, "Bug only applies to Win32 single-line Edit Controls");

            try {
                string actualText = "";
                SetText(SampleText.String1, out actualText, CheckType.IncorrectElementConfiguration);
                range = Pattern_DocumentRange(CheckType.Verification);

                Range_Clone(range, ref clonedRange, null, CheckType.IncorrectElementConfiguration);
                Range_MoveEndpointByUnit(clonedRange, TextPatternRangeEndpoint.End, TextUnit.Document, -1, ref actualCount, null, CheckType.IncorrectElementConfiguration);

                Range_ExpandToEnclosingUnit(clonedRange, TextUnit.Line, null, CheckType.IncorrectElementConfiguration);
                Range_GetText(clonedRange, ref text, -1, null, CheckType.IncorrectElementConfiguration);

                // The repro confused Character with Line, so we'll do both
                Range_Clone(range, ref clonedRange, null, CheckType.IncorrectElementConfiguration);
                Range_MoveEndpointByUnit(clonedRange, TextPatternRangeEndpoint.End, TextUnit.Document, -1, ref actualCount, null, CheckType.IncorrectElementConfiguration);

                Range_ExpandToEnclosingUnit(clonedRange, TextUnit.Character, null, CheckType.IncorrectElementConfiguration);
                Range_GetText(clonedRange, ref text, -1, null, CheckType.IncorrectElementConfiguration);

                Comment("As expected: ExpandToEnclsoingUnit did not return empty range for character or line");
            }
            catch( Exception ex ) 
            {
                if (IsUIVerifyException(ex))  // Don't interfere with legitimate ThrowMe(...)
                    throw;

                ThrowMe(CheckType.Verification, "Unexpected exception encountered:\n" + ex.ToString());
            }
        }
        
        //-------------------------------------------------------------------
        // Regress 

        internal void Bug304( string bugID, string controls, string bugTitle )
        {
            int  actualCount1   = 0;
            int  actualCount2   = 0;
            TextPatternRange range        = null;
            TextPatternRange clonedRange1 = null;
            TextPatternRange clonedRange2 = null;

            TS_RegressionTest(bugID, controls, bugTitle);

            try
            {
                string actualText = "";
                SetText(SampleText.String1, out actualText, CheckType.IncorrectElementConfiguration);
                range = Pattern_DocumentRange(CheckType.Verification);

                Range_Clone(range, ref clonedRange1, null, CheckType.IncorrectElementConfiguration);
                Range_Clone(range, ref clonedRange2, null, CheckType.IncorrectElementConfiguration);
                
                Range_MoveEndpointByUnit(clonedRange1, TextPatternRangeEndpoint.Start, TextUnit.Document, 1, ref actualCount1, null, CheckType.IncorrectElementConfiguration);
                Range_MoveEndpointByUnit(clonedRange2, TextPatternRangeEndpoint.Start, TextUnit.Document, 1, ref actualCount2, null, CheckType.IncorrectElementConfiguration);

                Range_Move(clonedRange1, TextUnit.Line, Int32.MinValue, ref actualCount1, null, CheckType.IncorrectElementConfiguration);
                Range_MoveEndpointByUnit( clonedRange2, TextPatternRangeEndpoint.End, TextUnit.Line, Int32.MinValue, ref actualCount2, null, CheckType.IncorrectElementConfiguration );

                if((actualCount1 == -2) || (actualCount2 == -2))
                {
                    ThrowMe(CheckType.Verification, "Bug has regressed, -2 returned from one or both Move calls");
                }
                else if((actualCount1 != -1) || (actualCount2 != -1))
                {
                    Comment("Likely a new problem, both calls should have returned -1 (success) or -2 (bug regressed).");                    
                    ThrowMe(CheckType.Verification, "Different values returned than expected, new bug(???)");
                }
            }
            catch (Exception ex)
            {
                if (IsUIVerifyException(ex))  // Don't interfere with legitimate ThrowMe(...)
                    throw;

                ThrowMe(CheckType.Verification, "Unexpected exception encountered:\n" + ex.ToString());
            }
        }

        //-------------------------------------------------------------------
        // Regress 

        internal void Bug305( string bugID, string controls, string bugTitle )
        {
            int actualCount1 = 0;
            int actualCount2 = 0;
            TextPatternRange range = null;
            TextPatternRange clonedRange1 = null;
            TextPatternRange clonedRange2 = null;

            TS_RegressionTest(bugID, controls, bugTitle);

            try
            {
                string actualText = "";
                SetText(SampleText.String1, out actualText, CheckType.IncorrectElementConfiguration);
                range = Pattern_DocumentRange(CheckType.Verification);

                Range_Clone(range, ref clonedRange1, null, CheckType.IncorrectElementConfiguration);
                Range_Clone(range, ref clonedRange2, null, CheckType.IncorrectElementConfiguration);

                Range_MoveEndpointByUnit(clonedRange1, TextPatternRangeEndpoint.Start, TextUnit.Line, Int32.MaxValue, ref actualCount1, null, CheckType.IncorrectElementConfiguration);
                Range_MoveEndpointByUnit(clonedRange2, TextPatternRangeEndpoint.Start, TextUnit.Line, Int32.MaxValue, ref actualCount2, null, CheckType.IncorrectElementConfiguration);

                if ((actualCount1 == 0) || (actualCount2 == 0))
                {
                    ThrowMe(CheckType.Verification, "Bug has regressed, 0 returned from one or both Move calls");
                }
                else if ((actualCount1 != 1) || (actualCount2 != 1))
                {
                    Comment("Likely a new problem, both calls should have returned 1 (success) or 0 (bug regressed).");
                    ThrowMe(CheckType.Verification, "Different values returned than expected, new bug(???)");
                }
            }
            catch (Exception ex)
            {
                if (IsUIVerifyException(ex))  // Don't interfere with legitimate ThrowMe(...)
                    throw;

                ThrowMe(CheckType.Verification, "Unexpected exception encountered:\n" + ex.ToString());
            }
        }

        //-------------------------------------------------------------------
        // Regress 

        internal void Bug306( string bugID, string controls, string bugTitle )
        {
            string actualText  = "";
            TextPatternRange range        = null;
            TextPatternRange[] visibleRanges = null;

            TS_RegressionTest(bugID, controls, bugTitle);

            if (TextLibrary.IsRichEdit(m_le) == true)
                ThrowMe(CheckType.IncorrectElementConfiguration, "Bug only applies to Win32 single-line Edit Controls");

            try
            {
                string text = "";
                
                SetText(SampleText.Random256, out actualText, CheckType.IncorrectElementConfiguration);
            
                range = Pattern_DocumentRange(CheckType.Verification);

                Pattern_GetVisibleRanges(_pattern, ref visibleRanges, null, CheckType.IncorrectElementConfiguration);
                if (visibleRanges.Length == 0)
                {
                    ThrowMe(CheckType.Verification, "Expected non-null return value from GetVisibleRange");
                }

                Range_GetText(visibleRanges[0], ref text, -1, null, CheckType.IncorrectElementConfiguration);

               


                if( (actualText.Length ) < (text.Length) )
                {
                    ThrowMe(CheckType.Verification, "GetVisibleRange returned text much larger (size = " + actualText.Length + ") than expected");
                }

                Comment("As expected: GetVisibleRange returned text approximetly equal to what is visible");
            }
            catch( Exception ex ) 
            {
                if (IsUIVerifyException(ex))  // Don't interfere with legitimate ThrowMe(...)
                    throw;
            
                ThrowMe(CheckType.Verification, "Unexpected exception encountered:\n" + ex.ToString());
            }
        }
        
        //-------------------------------------------------------------------
        // Regress 

        internal void Bug307( string bugID, string controls, string bugTitle )
        {
            string      text = "";
            TextPatternRange[] ranges = null;

            TS_RegressionTest(bugID, controls, bugTitle);

            try
            {
                string actualText = "";
                SetText(SampleText.Empty, out actualText, CheckType.IncorrectElementConfiguration);

                Pattern_GetVisibleRanges(_pattern, ref ranges, null, CheckType.IncorrectElementConfiguration);
                
                Range_GetText(ranges[0], ref text, -1, null, CheckType.IncorrectElementConfiguration);
                                
                TrimTrailingCRLF(m_le, ref text);

                if (text.Length == 0)
                {
                    Comment("As expected: Empty range returned");
                }
                else
                {
                    Comment("Text Length = " + text.Length);
                    Comment("Text        = " + text);
                    ThrowMe(CheckType.Verification, "Expected non-empty range.  Assertion should have been raised if bug has regressed, i.e. you shouldn't see this message!");
                }
            }
            catch (Exception ex)
            {
                if (IsUIVerifyException(ex))  // Don't interfere with legitimate ThrowMe(...)
                    throw;

                ThrowMe(CheckType.Verification, "Unexpected exception encountered:\n" + ex.ToString());
            }
        }

        //-------------------------------------------------------------------
        // Regress 

        internal void Bug308( string bugID, string controls, string bugTitle )
        {
            string text = "";
            TextPatternRange range = null;
            TextPatternRange   targetRange   = null;
            TextPatternRange[] visibleRanges = null;

            TS_RegressionTest(bugID, controls, bugTitle);

            if (TextLibrary.IsRichEdit(m_le) == true)
                ThrowMe(CheckType.IncorrectElementConfiguration, "Bug only applies to Win32 single-line Edit Controls");

            try
            {
                // Reset control 
                string actualText = "";
                SetText(SampleText.Custom, "A\r\nB\r\nC\r\nD\r\nE\r\nF\r\nG\r\nH\r\nI\r\nJ\r\nK\r\nL\r\nM\r\n", out actualText, CheckType.IncorrectElementConfiguration);
                           
                range = Pattern_DocumentRange(CheckType.Verification);

                Range_FindText(range, ref targetRange, "H", false, true, null, CheckType.IncorrectElementConfiguration);

                Range_ScrollIntoView(targetRange, true, null, CheckType.IncorrectElementConfiguration);

                Pattern_GetVisibleRanges(_pattern, ref visibleRanges, null, CheckType.IncorrectElementConfiguration);

                if (visibleRanges.Length < 1)
                {
                    ThrowMe(CheckType.Verification, "Unable to acquire visible ranges");
                }

                Range_GetText(visibleRanges[0], ref text, -1, null, CheckType.IncorrectElementConfiguration);

                if (text.LastIndexOf("H") == -1)
                {
                    ThrowMe(CheckType.Verification, "Unable to find chracter H");
                }
                else
                {
                    Comment("As expected: ScrollIntoView worked on multi-line edit control");
                }
            }
            catch (Exception ex)
            {
                if (IsUIVerifyException(ex))  // Don't interfere with legitimate ThrowMe(...)
                    throw;

                ThrowMe(CheckType.Verification, "Unexpected exception encountered:\n" + ex.ToString());
            }

        }

        //-------------------------------------------------------------------
        // Regress 

        internal void Bug309( string bugID, string controls, string bugTitle )
        {
            int actualCount = 0;
            string text = "";
            TextPatternRange range = null;
            TextPatternRange clonedRange = null;

            TS_RegressionTest(bugID, controls, bugTitle);

            if (TextLibrary.IsRichEdit(m_le) == true)
                ThrowMe(CheckType.IncorrectElementConfiguration, "Bug only applies to Win32 single-line Edit Controls");
            m_TestStep--; // Side-effect of auto-generated caller for this method
            TS_IsMultiLine(false, CheckType.IncorrectElementConfiguration);

            try
            {
                string actualText = "";
                SetText(SampleText.Custom, "String 1\r\r\nxxxyyyzzzxxxyyzzxxyyzzxxyyzz               String 2", out actualText, CheckType.IncorrectElementConfiguration);

                range = Pattern_DocumentRange(CheckType.Verification);

                Range_Clone(range, ref clonedRange, null, CheckType.IncorrectElementConfiguration);
                Range_MoveEndpointByUnit(clonedRange, TextPatternRangeEndpoint.End, TextUnit.Document, -1, ref actualCount,  null, CheckType.IncorrectElementConfiguration);
                Range_MoveEndpointByUnit(clonedRange, TextPatternRangeEndpoint.End, TextUnit.Line,      1, ref actualCount, null, CheckType.IncorrectElementConfiguration);

                Range_GetText( clonedRange, ref text, -1, null, CheckType.IncorrectElementConfiguration);
                
                if (text.LastIndexOf("String 1") == -1)
                {
                    ThrowMe(CheckType.Verification, "Expected string to contain 'String 1', actual string = " + text);
                }
                else
                {
                    Comment("As expected: GetText(-1) did not return an empty string");
                }
            }
            catch (Exception ex)
            {
                if (IsUIVerifyException(ex))  // Don't interfere with legitimate ThrowMe(...)
                    throw;

                ThrowMe(CheckType.Verification, "Unexpected exception encountered:\n" + ex.ToString());
            }
        }

        //-------------------------------------------------------------------
        // Regress 

        internal void Bug310(string bugID, string controls, string bugTitle)
        {
            TS_RegressionTest(bugID, controls, bugTitle);

            try
            {
                object bogus = (object)m_le.GetRuntimeId();
                Comment("As expected: Cast of 'AutomationElement.GetRuntimeId()' to int[] did not raise exception");
            }
            catch (Exception ex)
            {
                if (IsUIVerifyException(ex))  // Don't interfere with legitimate ThrowMe(...)
                    throw;

                if (ex is InvalidCastException)
                    ThrowMe(CheckType.Verification, "Bug has (likely) regressed, unable to cast to object\n" + ex.ToString());
                else
                    ThrowMe(CheckType.Verification, "Bug may not have regressed, but new problem has arisen\n" + ex.ToString());
            }
        }

        //-------------------------------------------------------------------
        // Regress 

        internal void Bug311(string bugID, string controls, string bugTitle)
        {
            bool                compare     = false;
            int                 actualCount = 0;
            TextPatternRange    range       = null;
            TextPatternRange    clonedRange = null;

            TS_RegressionTest(bugID, controls, bugTitle);

            try
            {
                range = Pattern_DocumentRange(CheckType.IncorrectElementConfiguration);

                if (IsEmptyRange(range, CheckType.IncorrectElementConfiguration) == true)
                {
                    ThrowMe(CheckType.IncorrectElementConfiguration, "Unable to perform test on control with no/empty text");
                }
                
                Range_Clone(range, ref clonedRange, null, CheckType.IncorrectElementConfiguration );
                
                Range_Move(clonedRange, TextUnit.Character, 1, ref actualCount, null, CheckType.IncorrectElementConfiguration );
                
                Range_Compare( range, clonedRange, ref compare, null, CheckType.IncorrectElementConfiguration);

                if( compare == false )
                    Comment("Correctly compared mis-matched ranges and returned false");
                else
                {
                    string text = "";
                    
                    Range_GetText( range, ref text, -1, null, CheckType.Verification);
                    Comment("Range: Entire doucment           = " + TrimText( text, 256 ) );
                    Comment("Range: Entire Document length    = " + text.Length);
                    Comment("");
                    
                    Range_GetText(clonedRange, ref text, -1, null, CheckType.Verification );
                    Comment("Range: Subset of document        = " + TrimText(text, 256));
                    Comment("Range: Subset of document length = " + text.Length);
                    Comment("");
                
                    ThrowMe(CheckType.Verification, "Compare returned true, but the ranges were not supposed to be equal (Move failure?)");
                }
            }
            catch (Exception ex)
            {
                if( IsUIVerifyException( ex ))  // Don't interfere with legitimate ThrowMe(...)
                    throw;

                if (ex is InvalidCastException)
                    ThrowMe(CheckType.Verification, "Bug has (likely) regressed, unable to cast to object\n" + ex.ToString());
                else
                    ThrowMe(CheckType.Verification, "Bug may not have regressed, but new problem has arisen\n" + ex.ToString());
            }
        }


        #endregion 6.1 Regression Tests

        #region Other Helpers
        
        /// <summary>
        /// Support WPF controls behavior - i.e. FlowDocument/Table scenario
        /// </summary>
        /// <param name="callingRange">text pattern range to work against</param>
        /// <returns>true if w/ the child table, false w/o</returns>
        internal bool CheckTable(TextPatternRange callingRange)
        {
            AutomationElement[] children = callingRange.GetChildren();
            if (children.Length > 0)
            {
                AutomationElement aeTable = children[0];
                ControlType controlType = (ControlType)aeTable.GetCurrentPropertyValue(AutomationElement.ControlTypeProperty);
                if (controlType == ControlType.Table)
                    return true;
            }
            return false;
        }

        internal bool CheckDocument(TextPatternRange callingRange)
        {
            AutomationElement element = callingRange.GetEnclosingElement();
            string className = (string)element.GetCurrentPropertyValue(AutomationElement.ClassNameProperty);
            if ("Document" == className ||
                "FlowDocumentPageViewer" == className ||
                "FlowDocumentScrollViewer" == className ||
                "DocumentViewer" == className
                )
            {
                return true;
            }
            else
            {
                Comment("**************** The class name is {0} ****************", (string)element.GetCurrentPropertyValue(AutomationElement.ClassNameProperty));
            }

            return false;
        }

        static internal bool CheckControlClassGeneral(TextPatternRange callingRange, string controlClass)
        {
            AutomationElement element = callingRange.GetEnclosingElement();
            string className = (string)element.GetCurrentPropertyValue(AutomationElement.ClassNameProperty);
            return className == controlClass;
        }
        #endregion

    }
    #endregion TextTestsHelper Class
}
