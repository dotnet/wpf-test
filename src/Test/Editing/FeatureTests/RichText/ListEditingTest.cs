// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Test cases for editing list items in documents.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 3 $ $Source: //depot/winmain_oob/wap_rtm/windowstest/client/wcptests/uis/Text/BVT/RichText/ListEditingTest.cs $")]

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using Bitmap = System.Drawing.Bitmap;
    using System.ComponentModel;

    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;
    using System.Windows.Input;

    using Microsoft.Test.Imaging;
    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;

    #endregion Namespaces.

    /// <summary>
    /// Verifies that users can perform the following actions on
    /// list items:
    /// - append list items
    /// - increase indentation
    /// - increase from paragraph
    /// - decrease
    /// - decrease to paragraph
    /// - navigate up and down across list items
    /// - navigate up and down within list items
    /// - navigate left and right across list items
    /// - navigate left and right within list items
    /// - change formatting on list item
    /// - change paragraph formatting on list item
    /// - select a list item with the mouse
    /// - cut a list item and paste it at the end of another
    /// - cut a list item and paste it at the end of the document
    /// - cut a list item and paste it within another
    /// - delete within / across list items
    /// - use empty formatting tags in empty list items
    /// </summary>
    [TestOwner("Microsoft"),TestTactics("123")]
    public class ListEditingTest: CustomTestCase
    {
        #region Main flow.

        private const int SmallOffset = 2;
        private const string MultipleParagraphXaml =
            "<Paragraph>Paragraph 0 that is LONG and wraps.</Paragraph>" +
            "<Paragraph>Paragraph 1.</Paragraph>";

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            SetupRichTextBox();
            QueueDelegate(AppendListItems);
        }
        
        private void AppendListItems()
        {
            SetContentToXaml(
                "<List><ListElementItem></ListElementItem></List>");
            MouseInput.MouseClick(_wrapper.Element);
            KeyboardInput.TypeString("^{HOME}");
            KeyboardInput.TypeString("foo{ENTER}");
            QueueDelegate(CheckListItemAppended);
        }
        
        private void CheckListItemAppended()
        {
            TextRange[] ranges;
            
            ranges = XPathNavigatorUtils.ListElements(new TextRange(_wrapper.Start, _wrapper.End),
                ".//ListElementItem");
            Log("Element items: " + ranges.Length);
            foreach(TextRange range in ranges)
            {
                Log("Range text: [" + range.Text + "]");
            }

            Verifier.Verify(ranges.Length == 2);
            Verifier.Verify(ranges[0].Text == "foo");
            Verifier.Verify(ranges[1].Text == "");
            
            KeyboardInput.TypeString("^{HOME}");
            QueueDelegate(CheckIndenting);
        }
        
        private void CheckIndenting()
        {
//            int depthCount;
//            int modifiedDepthCount;
//            TextRange range;

//			range = XPathNavigatorUtils.ListElements(new TextRange(_wrapper.Start, _wrapper.End),
//				".//ListElementItem")[0];
//            depthCount = TextUtils.CountPositionDepth(range.Start);            
//            Log("Depth of first list item before indenting: " + depthCount);
//wi
//            Log("Increasing indentation...");
//            Verifier.Verify(_wrapper.RaiseCommand("ParagraphIncreaseIndentation"),
//                "ParagraphIncreaseIndentation handled.", true);
//
//			range = XPathNavigatorUtils.ListElements(new TextRange(_wrapper.Start, _wrapper.End),
//				".//ListElementItem")[0];
////            modifiedDepthCount = TextUtils.CountPositionDepth(range.Start);
////            Log("Depth of first list item after indenting: " + modifiedDepthCount);
////            Verifier.Verify(modifiedDepthCount > depthCount,
////                "Depth was increased by indenting.", true);
//
//            Log("Decreasing indentation...");
//            Verifier.Verify(_wrapper.RaiseCommand("ParagraphDecreaseIndentation"),
//                "ParagraphDecreaseIndentation handled.", true);
//
//			range = XPathNavigatorUtils.ListElements(new TextRange(_wrapper.Start, _wrapper.End),
//				".//ListElementItem")[0];
//            modifiedDepthCount = TextUtils.CountPositionDepth(range.Start);
//            Log("Depth of first list item after un-indenting: " + modifiedDepthCount);
//            Verifier.Verify(modifiedDepthCount == depthCount,
//                "Depth was restored by un-indenting.", true);

            // Uncomment when Regression_Bug1 is fixed.
            
            /*
            System.Windows.HorizontalAlignment alignment;

            Log("Decreasing indentation into a right-aligned paragraph...");
            CurrentSelection.Start.TextContainer.SetValue(
                CurrentSelection.Start, FlowDocumentScrollViewer.HorizontalAlignmentProperty,
                System.Windows.HorizontalAlignment.Right);
            Verifier.Verify(_wrapper.RaiseCommand("ParagraphDecreaseIndentation"),
                "ParagraphDecreaseIndentation handled.", true);
                
            modifiedDepthCount = TextUtils.CountPositionDepth(CurrentSelection.Start);
            Log("Depth of first line after un-indenting: " + modifiedDepthCount);
            alignment = (System.Windows.HorizontalAlignment)
                CurrentSelection.Start.GetValue(FlowDocumentScrollViewer.HorizontalAlignmentProperty);
            Verifier.Verify(alignment == System.Windows.HorizontalAlignment.Right,
                "Horizontal alignment remains to the right.", true);
            Verifier.Verify(modifiedDepthCount == 1,
                "Depth is one after decreasing indentation.", true);
            Verifier.Verify(CurrentSelection.Start.GetElementType() == typeof(Paragraph),
                "First line of text is placed in paragraph.", true);

            */

            Logger.Current.ReportSuccess();
        }

        #endregion Main flow.

        #region Private fields.

        private Canvas _canvas;
        private RichTextBox _textControl;
        private UIElementWrapper _wrapper;
        // private LogicalDirection anyDirection = LogicalDirection.Backward;

        #endregion Private fields.

        #region Helper methods.

        /// <summary>Current TextSelection.</summary>
        private TextSelection CurrentSelection
        {            
            get
            {
                return _textControl.Selection;
            }
        }

        private void SetContentToXaml(string xaml)
        {
            System.Diagnostics.Debug.Assert(xaml != null);
            Log("Setting content to:\r\n" + xaml);
            //_textControl.TextRange.Text = String.Empty;
            //XamlUtils.SetXamlContent(_textControl.TextRange, xaml);

            _canvas.Children.Remove(_textControl);

            xaml = "<RichTextBox xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
                "Height='100%' Width='100%' KeyboardNavigation.AcceptsReturn='True'>" +
                xaml + "</RichTextBox>";
            _textControl = (RichTextBox) XamlUtils.ParseToObject(xaml);

            _canvas.Children.Add(_textControl);
//            SetupTextEditor();
            _wrapper = new UIElementWrapper(_textControl);
        }

        /// <summary>Sets up a rich text box for editing.</summary>
        private void SetupRichTextBox()
        {
            // Set up the visual tree.
            _canvas = new Canvas();
            _textControl = new RichTextBox();
            _textControl.Focusable = true;
            _textControl.SetValue(KeyboardNavigation.AcceptsReturnProperty, true);
            //_textControl.Height = new Length(100, UnitType.Percent);
            //_textControl.Width = new Length(100, UnitType.Percent);
            _canvas.Children.Add(_textControl);
            TestWindow.Content = _canvas;

//            SetupTextEditor();

            _wrapper = new UIElementWrapper(_textControl);
        }

//        private void SetupTextEditor()
//        {
//            _textControl.SetValue(TextEditor.IsEnabledProperty, true);
//        }

        #endregion Helper methods.
    }
}
