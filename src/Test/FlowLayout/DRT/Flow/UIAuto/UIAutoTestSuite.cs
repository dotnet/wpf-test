// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
//
// Description: Common functionality for UI Automation tests.
//

using System;                               // string
using System.Collections.Generic;           // List<T>
using System.Reflection;                    // FieldInfo
using System.Windows;                       // DependencyObject
using System.Windows.Automation;            // SupportedTextSelection
using System.Windows.Automation.Peers;      // AutomationPeer
using System.Windows.Automation.Provider;   // ITextProvider
using System.Windows.Automation.Text;       // TextPatternRangeEndpoint
using System.Windows.Interop;               // HwndSource
using System.Windows.Media;                 // VisualTreeHelper
using System.Windows.Documents;             // ITextPointer

namespace DRT
{
    /// <summary>
    /// Common functionality for UI Automation tests.
    /// </summary>
    internal abstract class UIAutoTestSuite : FlowTestSuite
    {
        //-------------------------------------------------------------------
        //
        //  Constructors
        //
        //-------------------------------------------------------------------

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="suiteName">Name of the suite.</param>
        protected UIAutoTestSuite(string suiteName) : 
            base(suiteName)
        {
            // Disable Background Layout.
            Type typeTextRangeAdaptor = DrtFlowBase.FrameworkAssembly.GetType("MS.Internal.Automation.TextRangeAdaptor");
            _fiTRAStart = typeTextRangeAdaptor.GetField("_start", BindingFlags.Instance | BindingFlags.NonPublic);
            _fiTRAEnd = typeTextRangeAdaptor.GetField("_end", BindingFlags.Instance | BindingFlags.NonPublic);
            Type typeTextPointer = DrtFlowBase.FrameworkAssembly.GetType("System.Windows.Documents.TextPointer");
            _piTPOffset = typeTextPointer.GetProperty("Offset", BindingFlags.Instance | BindingFlags.NonPublic);
            _piTPTextContainer = typeTextPointer.GetProperty("TextContainer", BindingFlags.Instance | BindingFlags.NonPublic);
            Type typeTextContainer = DrtFlowBase.FrameworkAssembly.GetType("System.Windows.Documents.TextContainer");
            _piTCTextSelection = typeTextContainer.GetProperty("TextSelection", BindingFlags.Instance | BindingFlags.NonPublic);
            Type typeElementProxy = DrtFlowBase.CoreAssembly.GetType("MS.Internal.Automation.ElementProxy");
            _piEPPeer = typeElementProxy.GetProperty("Peer", BindingFlags.Instance | BindingFlags.NonPublic);
            _miAPGetRootPeer = typeof(UIElementAutomationPeer).GetMethod("GetRootAutomationPeer", BindingFlags.Static | BindingFlags.NonPublic);
        }

        #endregion Constructors

        //-------------------------------------------------------------------
        //
        //  Verification Methods
        //
        //-------------------------------------------------------------------

        #region Verification Methods
    
        /// <summary>
        /// Verifies ITextRangeProvider's range and basic functionality.
        /// </summary>
        /// <param name="context">Context fo this call.</param>
        /// <param name="textRangeProvider">ITextRangeProvider to verify.</param>
        /// <param name="start">Expected start position of the range.</param>
        /// <param name="end">Expected end position of the range.</param>
        protected void VerifyTextRange(string context, ITextRangeProvider textRangeProvider, TextPointer start, TextPointer end)
        {
            int value;

            // Verify order.
            value = textRangeProvider.CompareEndpoints(TextPatternRangeEndpoint.Start, textRangeProvider, TextPatternRangeEndpoint.Start);
            DRT.Assert(value == 0, "{0}: ITextRangeProvider.CompareEndpoints(start, start) failied. Expecting 0, got (1}", context, value);
            value = textRangeProvider.CompareEndpoints(TextPatternRangeEndpoint.End, textRangeProvider, TextPatternRangeEndpoint.End);
            DRT.Assert(value == 0, "{0}: ITextRangeProvider.CompareEndpoints(end, end) failied. Expecting 0, got (0}", context, value);
            value = textRangeProvider.CompareEndpoints(TextPatternRangeEndpoint.Start, textRangeProvider, TextPatternRangeEndpoint.End);
            DRT.Assert(value <= 0, "{0}: ITextRangeProvider.CompareEndpoints(start, end) failied. Expecting <= 0, got (0}", context, value);

            // Verify start and end positions.
            if (start != null && end != null)
            {
                TextPointer rangeStart, rangeEnd;
                GetPositionsFromTextRangeProvider(textRangeProvider, out rangeStart, out rangeEnd);
                DRT.Assert(rangeStart != null && rangeEnd != null, "{0}: Failed to retrieve start and end TextPositions from ITextRangeProvider.", context);
                if (rangeStart != null && rangeEnd != null)
                {
                    if (start.CompareTo(rangeStart) != 0)
                    {
                        DRT.Assert(false, "{0}: Start positions do not match. Expecting offset {1}, got {2}.", context, GetOffsetFromTextPointer(start), GetOffsetFromTextPointer(rangeStart));
                    }
                    if (end.CompareTo(rangeEnd) != 0)
                    {
                        DRT.Assert(false, "{0}: End positions do not match. Expecting offset {1}, got {2}.", context, GetOffsetFromTextPointer(end), GetOffsetFromTextPointer(rangeEnd));
                    }
                }
            }

            // Verify Clone
            ITextRangeProvider textRangeProviderClone = textRangeProvider.Clone();
            DRT.Assert(textRangeProviderClone != null, "{0}: ITextRangeProvider.Clone failed.", context);
            if (textRangeProviderClone != null)
            {
                DRT.Assert(textRangeProvider.Compare(textRangeProviderClone), "{0}: ITextRangeProvider.Compare(clone) failed.", context);
                value = textRangeProvider.CompareEndpoints(TextPatternRangeEndpoint.Start, textRangeProviderClone, TextPatternRangeEndpoint.Start);
                DRT.Assert(value == 0, "{0}: ITextRangeProvider.CompareEndpoints(start, clone.start) failied. Expecting 0, got (1}", context, value);
                value = textRangeProvider.CompareEndpoints(TextPatternRangeEndpoint.End, textRangeProviderClone, TextPatternRangeEndpoint.End);
                DRT.Assert(value == 0, "{0}: ITextRangeProvider.CompareEndpoints(end, clone.end) failied. Expecting 0, got (1}", context, value);
            }
        }

        /// <summary>
        /// Verify AutomationPeer of child element.
        /// </summary>
        /// <param name="context">Context fo this call.</param>
        /// <param name="rawChild">AutomationPeer for the child.</param>
        /// <param name="child">Instance of child element.</param>
        protected void VerifyChildAutomationPeer(string context, AutomationPeer peer, DependencyObject child)
        {
            if (peer is UIElementAutomationPeer)
            {
                UIElementAutomationPeer uiPeer = (UIElementAutomationPeer)peer;
                DRT.Assert(uiPeer.Owner != null, "{0}: AutomationPeer.Owner is null.", context);
                if (uiPeer.Owner != null)
                {
                    DRT.Assert(uiPeer.Owner == child, "{0}: AutomationPeer.Owner is not matching expected element. Expecting {1}, got {2}.", context, child.GetType().ToString(), uiPeer.Owner.GetType().ToString());
                }
            }
            else if (peer is ContentElementAutomationPeer)
            {
                ContentElementAutomationPeer cePeer = (ContentElementAutomationPeer)peer;
                DRT.Assert(cePeer.Owner != null, "{0}: AutomationPeer.Owner is null.", context);
                if (cePeer.Owner != null)
                {
                    DRT.Assert(cePeer.Owner == child, "{0}: AutomationPeer.Owner is not matching expected element. Expecting {1}, got {2}.", context, child.GetType().ToString(), cePeer.Owner.GetType().ToString());
                }
            }
            else if (peer != null)
            {
                DRT.Assert(false, "{0}: Unknown AutomationPeer: {1}", context, peer.GetType().ToString());
            }
        }

        /// <summary>
        /// Verify AutomationPeer of child element.
        /// </summary>
        /// <param name="context">Context fo this call.</param>
        /// <param name="rawChild">Raw automation element for the child.</param>
        /// <param name="child">Instance of child element.</param>
        protected void VerifyChildAutomationPeer(string context, IRawElementProviderSimple rawChild, DependencyObject child)
        {
            AutomationPeer peer = GetPeerFromRawElement(rawChild);
            DRT.Assert(peer != null, "{0}: Failed to get AutomationPeer from raw element.", context);
            VerifyChildAutomationPeer(context, peer, child);
        }

        /// <summary>
        /// Verify ITextRangeProvider for child element.
        /// </summary>
        /// <param name="context">Context fo this call.</param>
        /// <param name="textProvider">ITextProvider to verify.</param>
        /// <param name="rawChild">Raw automation element for the child.</param>
        /// <param name="child">Instance of child element.</param>
        protected void VerifyChildTextRange(string context, ITextProvider textProvider, IRawElementProviderSimple rawChild, DependencyObject child)
        {
            ITextRangeProvider textRangeProvider = textProvider.RangeFromChild(rawChild);
            DRT.Assert(textRangeProvider != null, "{0}: ITextProvider.RangeFromChild returned null.", context);
            if (textRangeProvider != null)
            {
                TextPointer elementStart, elementEnd;
                GetTextRangeFromElement(child, out elementStart, out elementEnd);
                DRT.Assert(elementStart != null && elementEnd != null, "{0}: Failed to retrieve text range positions from child element.", context);
                if (elementStart != null && elementEnd != null)
                {
                    VerifyTextRange("ChildRange", textRangeProvider, elementStart.GetInsertionPosition(LogicalDirection.Forward), elementEnd.GetInsertionPosition(LogicalDirection.Backward));
                }
            }
        }

        /// <summary>
        /// Verifies ITextRangeProvider's range and basic functionality.
        /// </summary>
        /// <param name="context">Context fo this call.</param>
        /// <param name="textProvider">ITextProvider to verify.</param>
        /// <param name="textRangeProvider">ITextRangeProvider to verify.</param>
        /// <param name="children">Collection of expected children.</param>
        protected void VerifyTextRangeChildren(string context, ITextProvider textProvider, ITextRangeProvider textRangeProvider, List<DependencyObject> children)
        {
            IRawElementProviderSimple[] rawChildren = textRangeProvider.GetChildren();
            DRT.Assert(rawChildren != null, "{0}: ITextRangeProvider.GetChildren failed. Got null collection.", context);
            if (rawChildren != null)
            {
                DRT.Assert(rawChildren.Length == children.Count, "{0}: ITextRangeProvider.GetChildren failed. Expecting children count {1}, got {2}.", context, children.Count, rawChildren.Length);
                if (rawChildren.Length == children.Count)
                {
                    for (int i = 0; i < rawChildren.Length; i++)
                    {
                        VerifyChildAutomationPeer(context, rawChildren[i], children[i]);
                        VerifyChildTextRange(context, textProvider, rawChildren[i], children[i]);
                    }
                }
            }
        }
        
        /// <summary>
        /// Verifies DocumentRange exposed by ITextProvider.
        /// </summary>
        /// <param name="textProvider">ITextProvider to verify.</param>
        /// <param name="documentStart">Document's start position.</param>
        /// <param name="documentEnd">Document's end position.</param>
        protected void VerifyDocumentRange(ITextProvider textProvider, TextPointer documentStart, TextPointer documentEnd)
        {
            ITextRangeProvider textRangeProvider = textProvider.DocumentRange;
            DRT.Assert(textRangeProvider != null, "ITextProvider.DocumentRange is null.");
            if (textRangeProvider != null)
            {
                VerifyTextRange("DocumentRange", textRangeProvider, documentStart.GetInsertionPosition(LogicalDirection.Forward), documentEnd.GetInsertionPosition(LogicalDirection.Backward));
            }
        }

        /// <summary>
        /// Verifies TextSelection exposed by ITextProvider.
        /// </summary>
        /// <param name="textProvider">ITextProvider to verify.</param>
        /// <param name="position">Position within TextContainer.</param>
        /// <param name="selectionEnabled">Whether selection is enabled or not.</param>
        protected void VerifyTextSelection(ITextProvider textProvider, TextPointer position, bool selectionEnabled)
        {
            bool supportsSelection = (textProvider.SupportedTextSelection == SupportedTextSelection.Single);
            DRT.Assert(supportsSelection == selectionEnabled, "ITextProvider.SupportedTextSelection failed. Expecting {0}, got {1}.", selectionEnabled, supportsSelection);
            if (supportsSelection && selectionEnabled)
            {
                ITextRangeProvider[] trpSelections = textProvider.GetSelection();
                DRT.Assert(trpSelections != null && trpSelections.Length == 1, "ITextProvider.GetSelection failed. Expecting range count 1, got {0}.", (trpSelections != null) ? trpSelections.Length : -1);
                if (trpSelections != null && trpSelections.Length == 1)
                {
                    ITextRangeProvider trpSelection = trpSelections[0];
                    DRT.Assert(trpSelection != null, "ITextProvider.GetSelection returned null selection range.");
                    if (trpSelection != null)
                    {
                        TextPointer selectionStart, selectionEnd;
                        GetTextSelectionFromTextContainer(position, out selectionStart, out selectionEnd);
                        DRT.Assert(selectionStart != null && selectionEnd != null, "Failed to retrieve TextSelection positions from TextContainer.");
                        if (selectionStart != null && selectionEnd != null)
                        {
                            VerifyTextRange("SelectionRange", trpSelection, selectionStart, selectionEnd);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Verifies visible ranges exposed by ITextProvider.
        /// </summary>
        /// <param name="textProvider">ITextProvider to verify.</param>
        /// <param name="documentStart">Document's start position.</param>
        /// <param name="documentEnd">Document's end position.</param>
        /// <param name="entireDocumentVisible">Whether entire document is visible or not.</param>
        protected void VerifyVisibleRanges(ITextProvider textProvider, TextPointer documentStart, TextPointer documentEnd, bool entireDocumentVisible)
        {
            ITextRangeProvider[] trpVisibleRanges = textProvider.GetVisibleRanges();
            DRT.Assert(trpVisibleRanges != null && trpVisibleRanges.Length > 0, "ITextProvider.GetVisibleRanges failed. Expecting range count > 0, got {0}.", (trpVisibleRanges != null) ? trpVisibleRanges.Length : -1);
            if (trpVisibleRanges != null)
            {
                for (int i = 0; i < trpVisibleRanges.Length; i++)
                {
                    ITextRangeProvider trpVisibleRange = trpVisibleRanges[i];
                    DRT.Assert(trpVisibleRange != null, "ITextProvider.GetVisibleRanges returned null range at position {0}.", i);
                    if (trpVisibleRange != null)
                    {
                        VerifyTextRange("VisibleRange", trpVisibleRange, null, null);
                    }
                }
                if (entireDocumentVisible)
                {
                    TextPointer rangeStart, rangeEnd;
                    // Check start position.
                    GetPositionsFromTextRangeProvider(trpVisibleRanges[0], out rangeStart, out rangeEnd);
                    DRT.Assert(rangeStart != null && rangeEnd != null, "VisibleRanges: Failed to retrieve start and end TextPositions from ITextRangeProvider.");
                    if (rangeStart != null)
                    {
                        if (documentStart.GetInsertionPosition(documentStart.LogicalDirection).CompareTo(rangeStart) != 0)
                        {
                            DRT.Assert(false, "VisibleRange start position mismatch. Expecting offset {0}, got {1}.", GetOffsetFromTextPointer(documentStart), GetOffsetFromTextPointer(rangeStart));
                        }
                    }
                    // Check end position.
                    GetPositionsFromTextRangeProvider(trpVisibleRanges[trpVisibleRanges.Length-1], out rangeStart, out rangeEnd);
                    DRT.Assert(rangeStart != null && rangeEnd != null, "VisibleRanges: Failed to retrieve start and end TextPositions from ITextRangeProvider.");
                    if (rangeEnd != null)
                    {
                        if (documentEnd.GetInsertionPosition(documentEnd.LogicalDirection).CompareTo(rangeEnd) != 0)
                        {
                            DRT.Assert(false, "VisibleRange end position mismatch. Expecting offset {0}, got {1}.", GetOffsetFromTextPointer(documentEnd), GetOffsetFromTextPointer(rangeEnd));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Verifies children for DocumentRange exposed by ITextProvider.
        /// </summary>
        /// <param name="textProvider">ITextProvider to verify.</param>
        /// <param name="children">Collection of expected children.</param>
        protected void VerifyDocumentRangeChildren(ITextProvider textProvider, List<DependencyObject> children)
        {
            ITextRangeProvider textRangeProvider = textProvider.DocumentRange;
            DRT.Assert(textRangeProvider != null, "ITextProvider.DocumentRange is null.");
            if (textRangeProvider != null)
            {
                VerifyTextRangeChildren("DocumentRange", textProvider, textRangeProvider, children);
            }
        }

        #endregion Verification Methods

        //-------------------------------------------------------------------
        //
        //  Static Helpers
        //
        //-------------------------------------------------------------------

        #region Static Helpers

        /// <summary>
        /// Retrieves AutomationPeer associated with element.
        /// </summary>
        internal static AutomationPeer CreateAutomationPeer(DependencyObject element)
        {
            AutomationPeer peer = null;
            if (element is UIElement)
            {
                peer = UIElementAutomationPeer.CreatePeerForElement((UIElement)element);
            }
            else if (element is ContentElement)
            {
                peer = ContentElementAutomationPeer.CreatePeerForElement((ContentElement)element);
            }
            return peer;
        }

        /// <summary>
        /// Ensures that AutomationPeer is connected to _hwnd, otherwise ProviderFromPeer will fail.
        /// </summary>
        internal static bool EnsureConnected(UIElementAutomationPeer peer)
        {
            // Get root AutomationPeer and all ancestors of the current peer.
            List<AutomationPeer> ancestors = new List<AutomationPeer>();
            AutomationPeer rootPeer = peer;
            while (rootPeer.GetParent() != null)
            {
                rootPeer = rootPeer.GetParent();
                ancestors.Add(rootPeer);
            }
            if (rootPeer is UIElementAutomationPeer)
            {
                // Create root AutomationPeer
                UIElement localRoot = VisualTreeHelper.GetParent(((UIElementAutomationPeer)rootPeer).Owner) as UIElement;
                if (localRoot != null)
                {
                    HwndSource hwndSource = PresentationSource.FromVisual(localRoot) as HwndSource;
                    if (hwndSource != null)
                    {
                        rootPeer = _miAPGetRootPeer.Invoke(null, new object[] { localRoot, hwndSource.Handle }) as AutomationPeer;
                        if (rootPeer != null)
                        {
                            rootPeer.GetChildren();

                            // For each AutomationPeer in ancestors collection create a peer and force
                            // to populate children collection.
                            for (int i = ancestors.Count - 1; i >= 0; i--)
                            {
                                ancestors[i].GetChildren();
                            }
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Retrieves TextPositions representing ITextRangeProvider's start and end positions.
        /// </summary>
        /// <param name="textRangeProvider">ITextRangeProvider used to retrieve positions.</param>
        /// <param name="start">[out] TextPosition representing the start of the range.</param>
        /// <param name="end">[out] TextPosition representing the end of the range.</param>
        private static void GetPositionsFromTextRangeProvider(ITextRangeProvider textRangeProvider, out TextPointer start, out TextPointer end)
        {
            start = _fiTRAStart.GetValue(textRangeProvider) as TextPointer;
            end = _fiTRAEnd.GetValue(textRangeProvider) as TextPointer;
        }

        /// <summary>
        /// Retrieves TextPositions representing TextSelection of the TextContainer.
        /// </summary>
        /// <param name="position">Position within TextContainer.</param>
        /// <param name="start">[out] TextPosition representing the start of the range.</param>
        /// <param name="end">[out] TextPosition representing the end of the range.</param>
        private static void GetTextSelectionFromTextContainer(TextPointer position, out TextPointer start, out TextPointer end)
        {
            start = end = null;
            object textContainer = _piTPTextContainer.GetValue(position, null);
            if (textContainer != null)
            {
                TextSelection textSelection = _piTCTextSelection.GetValue(textContainer, null) as TextSelection;
                if (textSelection != null)
                {
                    start = textSelection.Start;
                    end = textSelection.End;
                }
            }
        }

        /// <summary>
        /// Retrieves TextPositions representing DependencyObject hosted by TextContainer.
        /// </summary>
        /// <param name="position">Element hosted by TextContainer.</param>
        /// <param name="start">[out] TextPosition representing the start of the range.</param>
        /// <param name="end">[out] TextPosition representing the end of the range.</param>
        private static void GetTextRangeFromElement(DependencyObject element, out TextPointer start, out TextPointer end)
        {
            start = end = null;
            if (element is TextElement)
            {
                start = ((TextElement)element).ElementStart;
                end = ((TextElement)element).ElementEnd;
            }
            else if (element is UIElement)
            {
                TextElement uiContainer = LogicalTreeHelper.GetParent(element) as TextElement;
                if (uiContainer != null)
                {
                    start = uiContainer.ContentStart;
                    end = uiContainer.ContentEnd;
                }
            }
        }

        /// <summary>
        /// Retrieves TextPosition's offset within ITextContainer.
        /// </summary>
        /// <param name="textPointer">TextPosition used to retrieve the offset.</param>
        private static int GetOffsetFromTextPointer(TextPointer textPointer)
        {
            return (int)_piTPOffset.GetValue(textPointer, null);
        }

        /// <summary>
        /// Retrieves AutomationPeer from raw element.
        /// </summary>
        internal static AutomationPeer GetPeerFromRawElement(IRawElementProviderSimple rawElement)
        {
            return _piEPPeer.GetValue((object)rawElement, null) as AutomationPeer;
        }

        #endregion Static Helpers

        private static FieldInfo _fiTRAStart;               // TextRangeAdaptor._start accessor
        private static FieldInfo _fiTRAEnd;                 // TextRangeAdaptor._end accessor
        private static PropertyInfo _piTPOffset;            // TextPointer.Offset accessor
        private static PropertyInfo _piTPTextContainer;     // TextPointer.TextContainer accessor
        private static PropertyInfo _piTCTextSelection;     // TextContainer.TextSelection accessor
        private static PropertyInfo _piEPPeer;              // ElementProxy.Peer
        private static MethodInfo _miAPGetRootPeer;         // AutomationPeer.GetRootAutomationPeer
    }
}
