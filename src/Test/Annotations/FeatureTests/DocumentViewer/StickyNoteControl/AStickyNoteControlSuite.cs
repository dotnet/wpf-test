// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: General base class for tests of the StickyNoteControl hosted in a DocumentViewerBase.

using System;
using System.Windows;
using Annotations.Test.Framework;
using System.Windows.Controls;
using System.Collections.Generic;
using Annotations.Test.Reflection;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Text.RegularExpressions;
using System.Windows.Ink;
using System.Collections;
using System.Windows.Input;

namespace Avalon.Test.Annotations
{
    [TestDimension("fixed,flow")]
    [TestDimension("stickynote,inkstickynote")]
    public abstract class AStickyNoteControlSuite : ADefaultContentSuite
    {
        #region TestSuite Overrides

        [TestCase_Setup()]
        protected override void DoSetup()
        {
            base.DoSetup();

            // Ensure that window is front so that mouse/keyboard input is successful.
            AnnotationTestHelper.BringToFront(MainWindow);
            MainWindow.Focus();
        }

        [TestCase_Cleanup()]
        protected override void CleanupVariation()
        {
            base.CleanupVariation();
            inkCache.Clear();
        }

        public override void ProcessArgs(string[] args)
        {
            base.ProcessArgs(args);

            foreach (string arg in args)
            {
                Match match = new Regex("/zoom=(.*)").Match(arg);
                if (match.Success)
                {
                    _zoom = double.Parse(match.Groups[1].Value);
                    break;
                }
            }

            printStatus("Zoom Dimension = " + _zoom);
        }

        protected override AnnotationMode DetermineAnnotationMode(string[] args)
        {
            AnnotationMode mode = base.DetermineAnnotationMode(args);
            if (mode != AnnotationMode.StickyNote && mode != AnnotationMode.InkStickyNote)
                throw new ArgumentException("Note tests must be run for either Text or Ink stickynotes.");
            return mode;
        }

        protected override void DoExtendedSetup()
        {
            // Zoom in so that mouse input is accurate in FixedDocuments.
            SetZoom(100);
        }

        #endregion

        #region Protected Method

        protected virtual void CreateDefaultNote()
        {
            CreateAnnotation(new SimpleSelectionData(DEFAULT_PAGE, DEFAULT_OFFSET, DEFAULT_LENGTH));
        }

        protected void DeleteDefaultNote()
        {
            DeleteAnnotation(new SimpleSelectionData(DEFAULT_PAGE, DEFAULT_OFFSET, DEFAULT_LENGTH));
        }

        protected void VerifyInkCanvasEditMode(InkCanvasEditingMode expectedMode)
        {
            InkCanvas canvas = new StickyNoteWrapper(Note, "note").InnerControl as InkCanvas;
            AssertNotNull("Method is only valid for InkStickyNotes.", canvas);
            AssertEquals("Verify ink edit mode.", expectedMode, canvas.EditingMode);
        }

        protected FlowDocument CreateFlowDocument(string content)
        {
            FlowDocument doc = new FlowDocument();
            Paragraph para = new Paragraph();
            para.Inlines.Clear();
            para.Inlines.Add(new Run(content));
            ((IAddChild)doc).AddChild(para);
            return doc;
        }

        protected string DocumentText(FlowDocument doc)
        {
            return new TextRange(doc.ContentStart, doc.ContentEnd).Text;
        }

        /// <summary>
        /// Fail test if we are not in InkMode.
        /// </summary>
        protected void EnsureInkMode()
        {
            if (AnnotationType != AnnotationMode.InkStickyNote)
                failTest("Expected StickyNoteType == 'Ink' but was 'Text'.");
        }

        protected void MoveMouseToCharacterOffset(int page, int offset)
        {
            //
            // If Theme == Luna, then Bottom.
            // If Theme == Classic, then Middle.
            //
            ADocumentViewerBaseWrapper.HorizontalJustification justification = ADocumentViewerBaseWrapper.HorizontalJustification.Bottom;
            //
            // Adding DPI scaling.  Will multiply actual points by the current DPI/96..
            //            
            UIAutomationModule.MoveTo(DocViewerWrapper.PointerToScreenCoordinates(page, offset, LogicalDirection.Forward, justification));
            DispatcherHelper.DoEvents(); // Wait for property to get set.
        }


        protected void ChangeZoom()
        {
            SetZoom(_zoom);
        }

        protected Rect PageBounds(int pageNumber)
        {
            VerifyPageIsVisible(pageNumber);
            Rect[] pageBounds = DocViewerWrapper.GetBoundsOfVisiblePages();
            return pageBounds[pageNumber];
        }

        protected void AssertEquals(string msg, Point p1, Point p2, double tolerance)
        {
            printStatus("Expected ='" + p1 + "' Actual='" + p2 + "'.");
            AssertEquals(msg, p1.X, p2.X, tolerance);
            AssertEquals(msg, p1.Y, p2.Y, tolerance);
        }

        protected void AssertStringNotEmptyOrNull(string msg, string text)
        {
            printStatus("Actual Text ='" + text + "'.");
            Assert("String is null.", text != null);
            Assert("String is empty.", text != string.Empty);
        }

        /// <summary>
        /// Insert deterministic content into the currently attached StickyNote. 
        /// Simulates human input.
        /// </summary>
        protected void InsertContent(ContentKind kind)
        {
            if (AnnotationType == AnnotationMode.StickyNote)
                UIAutomationModule.TypeString(ChooseText(kind));
            else
                AddInk(kind);

            Assert("Verify that content was successfully added.", CurrentlyAttachedStickyNote.HasContent);
        }

        /// <summary>
        /// Just set the content of the currently attached note (e.g. not simulated human input).
        /// </summary>
        /// <remarks>Still adds Ink using mouse input.</remarks>
        protected void SetContent(ContentKind kind)
        {
            if (AnnotationType == AnnotationMode.StickyNote)
            {
                RichTextBox rtb = CurrentlyAttachedStickyNote.RichTextBox;
                rtb.Document = CreateFlowDocument("");

                // 


                if (kind == ContentKind.Image)
                {
                    rtb.Document = ChooseDocument(kind);
                    rtb.AppendText(" ");
                }
                else
                {
                    rtb.AppendText(ChooseText(kind));
                }

                DispatcherHelper.DoEvents();
            }
            else
            {
                // Clear existing content.
                CurrentlyAttachedStickyNote.InkCanvas.Strokes.Clear();
                AddInk(kind);
            }
            Assert("Verify that content was successfully added.", CurrentlyAttachedStickyNote.HasContent);
        }

        /// <summary>
        /// Verify that note contains the expected content given that it was inserted
        /// using InsertContent method.
        /// </summary>
        /// <param name="kind"></param>
        protected void VerifyContent(ContentKind kind)
        {
            if (AnnotationType == AnnotationMode.StickyNote)
            {                
                if (kind == ContentKind.Image)
                {
                    FlowDocument document = (FlowDocument)CurrentlyAttachedStickyNote.RichTextBox.Document;
                    VisualTreeWalker<Image> treeWalker = new VisualTreeWalker<Image>();
                    IList<Image> images = treeWalker.FindChildren(((IDocumentPaginatorSource)document).DocumentPaginator.GetPage(0).Visual);
                    Assert("Verify content contains an image.", images.Count == 0);
                }
                else
                {
                    // Note: RTB always has a trailing '\r\n' so take that into account.
                    AssertEquals("Verify content.", ChooseText(kind) + "\r\n", CurrentlyAttachedStickyNote.Content);
                }
            }
            else
            {
                if (!inkCache.ContainsKey(kind))
                    throw new NotSupportedException("VerifyContent can only be used if content was added using InsertContent.");
                AssertEquals("Comparing ink strokes.", (StrokeCollection)inkCache[kind], (StrokeCollection)CurrentlyAttachedStickyNote.Content);
            }
        }

        /// <summary>
        /// Verify that the Clipboard contains the expected content given that it was inserted
        /// into a note using InsertContent.
        /// </summary>
        /// <param name="kind"></param>
        protected void VerifyClipboardContent(ContentKind kind)
        {
            IDataObject data = Clipboard.GetDataObject();
            AssertNotNull("Verify there is some data on Clipboard.", data);

            if (AnnotationType == AnnotationMode.StickyNote)
            {
                object dataAsString = data.GetData(typeof(string));
                AssertNotNull("Verify there is a string on the clipboard.", dataAsString);
                // Note: RTB always has a trailing '\r\n' so take that into account.
                AssertEquals("Verify Clipboard content.", ChooseText(kind) + "\r\n", dataAsString.ToString());
            }
            else
            {
                // Make sure there is ink on the clipboard.
                Assert("Verify there is ink on the clipboard.", data.GetDataPresent(StrokeCollection.InkSerializedFormat, true));
            }
            printStatus("Verified clipboard contents.");
        }

        /// <summary>
        /// Compare two StrokeCollections stroke by stroke to verify that they are the same.
        /// </summary>
        protected void AssertEquals(string msg, StrokeCollection expectedInk, StrokeCollection actualInk)
        {
            printStatus(msg);
            AssertEquals("Verify number of strokes.", expectedInk.Count, actualInk.Count);
            IEnumerator expectedEnum = expectedInk.GetEnumerator();
            IEnumerator actualEnum = actualInk.GetEnumerator();
            int strokeCount = 0;
            while (expectedEnum.MoveNext() && actualEnum.MoveNext())
            {
                Stroke expected = (Stroke)expectedEnum.Current;
                Stroke actual = (Stroke)actualEnum.Current;

                StylusPointCollection expectedPoints = expected.StylusPoints;
                StylusPointCollection actualPoints = expected.StylusPoints;

                // 
                AssertEquals("Verify number of points for stroke '" + strokeCount + ".", expectedPoints.Count, actualPoints.Count);

                strokeCount++;
            }
        }

        /// <summary>
        /// Verify the relative z-orders of the given SNs.
        /// </summary>
        /// <remarks>
        /// Relative z-order is determined by looking at the order the SNs were passed in.  The
        /// SN at index 0 will be "on top", and the SN at index Length-1 should be on the bottom.
        /// </remarks>
        /// <param name="wrappers">Array of SN sorted by expected z-order.</param>
        protected void VerifyZOrders(StickyNoteWrapper[] wrappers)
        {
            for (int i = 0; i < wrappers.Length - 1; i++)
            {
                Assert("Verify that SN '" + i + "' is ontop of '" + (i + 1) + "'.", wrappers[i].ZOrder > wrappers[i + 1].ZOrder);
            }
        }

        protected void VerifyIconSize(Size expectedSize)
        {
            VerifyIconSize(Note, expectedSize);
        }

        protected void VerifyIconSize(StickyNoteControl note, Size expectedSize)
        {
            // We know what the style looks like so parse the visual tree and verify the size of the
            // desired elements.  If the style changes, this will break.
            //
            IList<Button> children = new VisualTreeWalker<Button>().FindChildren(note);
            AssertEquals("Verify style hasn't changed noticably, expect 1 button.", 1, children.Count);
            AssertEquals("Verify button size.", expectedSize, new Size(children[0].Width, children[0].Height));
        }

        #endregion

        #region Protected Properties

        protected bool IsTextNote
        {
            get
            {
                return AnnotationType == AnnotationMode.StickyNote;
            }
        }

        protected StickyNoteControl Note
        {
            get
            {
                StickyNoteWrapper wrapper = CurrentlyAttachedStickyNote;
                AssertNotNull("Verify that there is a note visible.", wrapper);
                return (StickyNoteControl)wrapper.Target;
            }
        }

        #endregion

        #region Private Methods

        private void AddInk(ContentKind kind)
        {
            DrawInk(kind);

            // Instead of trying to store the expected values of ink strokes, 
            // cache the strokes that are initially in the canvas.  Then use
            // these when VerifyContent is called.  This will not ensure that
            // the strokes are correct, but it will ensure that strokes are 
            // consistent between insert and verify calls.
            // 
            CacheInk(kind);
        }

        private Rect TruncateBounds(Rect bounds)
        {
            return new Rect(Math.Truncate(bounds.X), Math.Truncate(bounds.Y), Math.Truncate(bounds.Width), Math.Truncate(bounds.Height));
        }

        private FlowDocument ChooseDocument(ContentKind kind)
        {
            FlowDocument doc = null;
            switch (kind)
            {
                case ContentKind.Image:
                    doc = (FlowDocument)AnnotationTestHelper.LoadContent(image_file);
                    break;
                default:
                    doc = CreateFlowDocument(ChooseText(kind));
                    break;
            }
            return doc;
        }

        private string ChooseText(ContentKind kind)
        {
            string contentKind = string.Empty;
            switch (kind)
            {
                case ContentKind.Standard_Small:
                    contentKind = standard_small_text;
                    break;
                case ContentKind.Standard_Large:
                    contentKind = standard_large_text;
                    break;
                case ContentKind.Standard_Brief:
                    contentKind = standard_brief_text;
                    break;
                default:
                    throw new NotImplementedException();
            }
            return contentKind;
        }

        private void DrawInk(ContentKind kind)
        {
            switch (kind)
            {
                case ContentKind.Standard_Brief:
                    UIAutomationModule.MoveToCenter(Note);
                    DrawStroke(new Vector(25, 30));
                    break;
                // Draw a bowtie shape in the middle of the canvas.
                case ContentKind.Standard_Small:
                    int scale = 10;
                    UIAutomationModule.MoveToCenter(Note);
                    DrawStroke(new Vector(20, scale));
                    DrawStroke(new Vector(0, -(scale * 2)));
                    DrawStroke(new Vector(-20, scale));
                    DrawStroke(new Vector(-20, scale));
                    DrawStroke(new Vector(0, -(scale * 2)));
                    DrawStroke(new Vector(20, scale));
                    break;
                case ContentKind.Standard_Large:
                    UIAutomationModule.MoveToCenter(Note);
                    UIAutomationModule.LeftMouseDown();
                    UIAutomationModule.Move(new Vector(100, 0));
                    UIAutomationModule.Move(new Vector(0, 50));
                    UIAutomationModule.Move(new Vector(-100, 0));
                    UIAutomationModule.Move(new Vector(0, -50));
                    UIAutomationModule.LeftMouseUp();
                    DispatcherHelper.DoEvents();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void DrawStroke(Vector delta)
        {
            UIAutomationModule.LeftMouseDown();
            UIAutomationModule.Move(delta);
            UIAutomationModule.LeftMouseUp();
            DispatcherHelper.DoEvents();
        }

        private void CacheInk(ContentKind kind)
        {
            if (inkCache.ContainsKey(kind))
                inkCache.Remove(kind);
            else
                inkCache.Add(kind, CurrentlyAttachedStickyNote.InkCanvas.Strokes);
        }

        #endregion

        #region Fields

        protected static int DEFAULT_PAGE = 0;
        protected static int DEFAULT_OFFSET = 100;
        protected static int DEFAULT_LENGTH = 500;

        protected static string standard_small_text = "Here is a little string of text that should fill at least one line of a Text StickyNote";
        protected static string standard_large_text = "New Product Helps Tame the Homework Beast\nMicrosoft Student 2006 should help middle- and high-school students to be more productive in the 2-3 hours a night they spend on homework.\nPeek at Office 12 Draws Customers Raves\nMore than 400 customers and partners get an early view. The product looks very positive in increasing productivity, says one.\nHow One Team is Addressing Customer Accountability\nCase study The Networking and Devices Technologies group methodically shifted its design focus from technology-driven to customer-driven.";
        protected static string standard_brief_text = "Hello World";
        protected static string image_file = "Flow_ImageOnly.xaml";

        /// <summary>
        /// Cache of ink content added by InsertContent and used by VerifyContent.
        /// </summary>
        protected IDictionary<ContentKind, StrokeCollection> inkCache = new Dictionary<ContentKind, StrokeCollection>();

        double _zoom = 100;

        #endregion

        protected enum ContentKind
        {
            Standard_Brief,
            Standard_Small,
            Standard_Large,            
            Image,
            Mixed
        }
    }
}	

