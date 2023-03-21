// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: 


using Size = System.Windows.Size;

using System;
using System.Windows;
using System.Drawing;
using Annotations.Test;
using Annotations.Test.Framework;
using System.Windows.Documents;
using System.Collections.Generic;
using Proxies.System.Windows.Annotations;
using Proxies.MS.Internal.Annotations;
using Test.Uis.Imaging;
using System.Text;
using System.Windows.Media;
using System.Windows.Controls.Primitives;

namespace Avalon.Test.Annotations
{
	public abstract class ATableSuite : AFlowSuite
    {
        #region Overrides

        public override IDocumentPaginatorSource  FlowContent
        {
            get 
            {
                return LoadContent(ViewerTestConstants.TableTests.Filename);
            }
        }

        [TestCase_Setup()]
        protected override void DoSetup()
        {
            base.DoSetup();

            // For brevetty of test cases store the tablename once.
            if (CaseNumber.Contains("simple"))
                _tablename = ViewerTestConstants.TableTests.SimpleTableName;
            if (CaseNumber.Contains("spanning"))
                _tablename = ViewerTestConstants.TableTests.SpanningTableName;
            if (CaseNumber.Contains("nested"))
                _tablename = ViewerTestConstants.TableTests.OuterNestedTableName;
            if (CaseNumber.Contains("embedded"))
                _tablename = ViewerTestConstants.TableTests.EmbeddedTableName;

            _tableSelectionFactory = new TableSelectionFactory(TableName);
        }

        protected override AnnotationMode DetermineAnnotationMode(string[] args)
        {
            if (CaseNumber.Contains("highlight"))
                return AnnotationMode.Highlight;
            return base.DetermineAnnotationMode(args);
        }

        protected override Size WindowSize
        {
            get
            {
                return new Size(913, 704);
            }
        }

        #endregion

        #region Protected Methods

        protected string GetText(ITableSegment selection) 
        {
            return GetText(new ITableSegment[] { selection });
        }

        protected string GetText(ITableSegment[] selectionParts)
        {
            StringBuilder result = new StringBuilder();
            foreach (ITableSegment part in selectionParts)
            {
                if (part is LineBreak)
                {
                    result.Append("\r\n");
                }
                else
                {
                    result.Append(GetText(TableSelectionFactory.Create(part)));
                }
                
                // This method is used for generating expected anchors from numerous
                // dijoint table segments, depending upon the segment we may need to 
                // add additional tabs or linebreaks to produce the correct expected 
                // anchors.
                CellSpan cellSpan = part as CellSpan;
                if (cellSpan != null)
                {
                    // Single cell text is missing ending tab.
                    if (cellSpan.IsSingleCell)
                        result.Append("\t");
                    // Same row selections that include ending column are missing linebreak.
                    if (cellSpan.ContainsEndOfRow)
                        result.Append("\r\n");
                }
                else if (part is CellEnd)
                {
                    if (!(part as CellEnd).ExcludeEndingTab)
                        result.Append("\t");
                }
                else if (part is TableStart)
                {
                    if ((part as TableStart).OnlyBeforeTable)
                        result.Append("\r\n");
                }
            }
            return result.ToString();
        }

        protected void CreateHighlight(ITableSegment selection, System.Windows.Media.Color color)
        {
            printStatus("Create " + color + " Highlight...");
            new HighlightDefinition(TableSelectionFactory.Create(selection), color).Create(DocViewerWrapper);
        }

        protected void ClearHighlight(ITableSegment selection)
        {
            printStatus("Clear Highlight...");
            DeleteHighlight(DocViewerWrapper, TableSelectionFactory.Create(selection));
        }

        protected void TestTableSelection(ITableSegment selection, ITableSegment expectedAnchorInTable)
        {
            TestTableSelection(selection, new ITableSegment[] { expectedAnchorInTable });
        }

        /// <summary>
        /// -Create annotation for selection.
        /// -Verify Annotation: anchor and rendering.
        /// -Toggle Annotation service.
        /// -Verify anchor restored correctly.
        /// -Delete annotation for selection.
        /// -Verify annotation is deleted.
        /// </summary>
        /// <param name="selection"></param>
        protected void TestTableSelection(ITableSegment selection, ITableSegment[] expectedAnchorInTable)
        {
            ISelectionData selectionData = TableSelectionFactory.Create(selection);
            Bitmap master = GetBitmapOfView();

            CreateAnnotation(selectionData);
            VerifyTableAnnotation(master, new ITableSegment[] { selection }, expectedAnchorInTable);
            ToggleAnnotationService();
            VerifyTableAnnotation(master, new ITableSegment[] { selection }, expectedAnchorInTable);
            DeleteAnnotation(selectionData);
            VerifyNumberOfAttachedAnnotations(0);

            passTest("Verified annotating table selection.");
        }

        /// <summary>
        /// Test creating an annotation across a table that is split on a page boundary.
        /// </summary>
        /// <param name="zoom">Zoom percentage to set on viewer.</param>
        /// <param name="windowSize">Size to change window to.</param>
        /// <param name="selection">Selection to create annotation for.</param>
        /// <param name="createBeforeReflow">If true then change zoom and window size before creating annotation.</param>
        protected void TestCrossPageTableSelection(
            double zoom,
            Size windowSize,
            ITableSegment selection,
            ITableSegment [] expectedFirstPageAnchor,
            ITableSegment [] expectedSecondPageAnchor,
            bool createBeforeReflow,
            bool firstPageInView)
        {
            ISelectionData selectionData = TableSelectionFactory.Create(selection);
            Bitmap masterA, masterB;

            CreateCrosspageMasters(zoom, windowSize, out masterA, out masterB);

            if (createBeforeReflow)
            {
                CreateAnnotation(selectionData);
                ReflowDocument(zoom, windowSize);
            }
            else
            {
                ReflowDocument(zoom, windowSize);
                if (!firstPageInView)
                    GoToPage(1);
                CreateAnnotation(selectionData);
            }            

            GoToPage(0);
            VerifyCrosspageAnnotation(masterA, expectedFirstPageAnchor, masterB, expectedSecondPageAnchor);
            ToggleAnnotationService();
            VerifyCrosspageAnnotation(masterA, expectedFirstPageAnchor, masterB, expectedSecondPageAnchor);
            DeleteAnnotation(selectionData);
            VerifyCrosspageAnnotation(masterA, null, masterB, null);

            passTest("Verified annotating table selection.");
        }

        private void ToggleAnnotationService()
        {
            printStatus("Toggling AnnotationService...");
            DisableAnnotationService();
            SetupAnnotationService();
        }

        /// <summary>
        /// Reflow the document, create the masters, then restore the reflow to its original state.
        /// </summary>
        protected void CreateCrosspageMasters(double zoom, Size windowSize, out Bitmap firstPage, out Bitmap secondPage)
        {
            double initialZoom = DocViewerWrapper.GetZoom();
            Size initialWindowSize = new Size(MainWindow.ActualWidth, MainWindow.ActualHeight);

            ReflowDocument(zoom, windowSize);
            CreateCrosspageMasters(out firstPage, out secondPage);
            ReflowDocument(initialZoom, initialWindowSize);
        }

        protected void CreateCrosspageMasters(out Bitmap firstPage, out Bitmap secondPage)
        {            
            firstPage = GetBitmapOfView();
            PageDown();
            secondPage = GetBitmapOfView();
            PageUp();
        }

        protected void ReflowDocument(double zoom, Size windowSize)
        {
            // Reflow document: change window size before zoom
            printStatus("Reflow Document...");
            SetWindowWidth(windowSize.Width);
            SetWindowHeight(windowSize.Height);
            SetZoom(zoom);
        }

        protected void VerifyCrosspageAnnotation(Bitmap firstPageMaster, ITableSegment[] expectedFirstPageAnchor, Bitmap secondPageMaster, ITableSegment[] expectedSecondPageAnchor)
        {
            printStatus("Verify first page...");
            if (expectedFirstPageAnchor != null)
                VerifyTableAnnotation(firstPageMaster, expectedFirstPageAnchor, expectedFirstPageAnchor);
            else
                VerifyNumberOfAttachedAnnotations(0);
            PageDown();
            printStatus("Verify second page...");
            if (expectedSecondPageAnchor != null)
                VerifyTableAnnotation(secondPageMaster, expectedSecondPageAnchor, expectedSecondPageAnchor);
            else
                VerifyNumberOfAttachedAnnotations(0);
            PageUp();
        }

        /// <summary>
        /// -Create annotation for selection.
        /// -Verify Annotation: anchor and rendering.
        /// -Toggle Annotation service.
        /// -Verify anchor restored correctly.
        /// -Delete annotation for selection.
        /// -Verify annotation is deleted.
        /// </summary>
        /// <param name="selection"></param>
        protected void TestTableSelection(ITableSegment selection)
        {
            TestTableSelection(selection, selection);
        }

        protected void VerifyTableAnnotation(Bitmap pageMaster, ITableSegment expectedAnchor, ITableSegment expectedSegmentInTable)
        {
            VerifyTableAnnotation(pageMaster, new ITableSegment[] { expectedAnchor }, new ITableSegment[] { expectedSegmentInTable });
        }

        protected void VerifyTableAnnotation(Bitmap pageMaster, ITableSegment[] expectedAnchor)
        {
            VerifyTableAnnotation(pageMaster, expectedAnchor, expectedAnchor);
        }

        protected void VerifyTableAnnotation(Bitmap pageMaster, ITableSegment[] expectedAnchor, ITableSegment[] expectedSegmentInTable)
        {
            printStatus("Verifying attached anchor...");
            VerifyAnnotation(GetText(expectedAnchor));

            // Save s screen shot showing what the actually rendering looked like.
            string filename = CaseNumber + ".bmp";
            GetBitmapOfView().Save(filename);
            LogToFile(filename);

            // Only do render verification for highlights because StickyNote's brackets or bubble
            // will get in the way.
            if (AnnotationType == AnnotationMode.Highlight)
            {
                printStatus("Verify anchor rendering...");
                VerifyThatTableChanged(pageMaster);
                VerifyThatExpectRegionWasUnchanged(pageMaster, expectedSegmentInTable);
            }
        }

        protected void VerifyThatTableChanged(Bitmap master)
        {
            printStatus("Stage 1: Verify that something changed.");
            CompareBitmaps(master, GetBitmapOfView(), false);
        }

        protected void VerifyThatExpectRegionWasUnchanged(Bitmap master, ITableSegment[] selections)
        {
            printStatus("Stage 2: Verify that change occurred only in expected region.");
            Bitmap currentImage = BlackOutSegments(GetBitmapOfView(), selections);
            Bitmap expectedImage = BlackOutSegments(master, selections);
            CompareBitmaps(expectedImage, currentImage, true);
        }

        protected void VerifyAnnotations(ITableSegment[][] expectedAnchors)
        {

        }      

        #endregion

        #region Imaging

        /// <summary>
        /// Get a Bitmap for the current table.
        /// </summary>
        /// <returns>Bitmap of just the current table.</returns>
        protected Bitmap GetTableBitmap()
        {
            return GetSubBitmap(GetBitmapOfView(), new TableSpan());
        }

        /// <summary>
        /// Get a Bitmap for the current table with a certain portion blacked out.
        /// </summary>
        /// <param name="segmentToExclude">Segment of table to be blacked out.</param>
        /// <returns>Bitmap of current table with segment blacked out.</returns>
        protected Bitmap GetTableBitmap(ITableSegment segmentToExclude)
        {
            return GetTableBitmap(new ITableSegment[] { segmentToExclude });
        }

        /// <summary>
        /// Get a Bitmap for the current table with a certain portions blacked out.
        /// </summary>
        /// <param name="segmentToExclude">Segment of table to be blacked out.</param>
        /// <returns>Bitmap of current table with segment blacked out.</returns>
        protected Bitmap GetTableBitmap(ITableSegment[] segmentsToExclude)
        {
            Bitmap blackedOutCapture = BlackOutSegments(GetBitmapOfView(), segmentsToExclude);
            return GetSubBitmap(blackedOutCapture, new TableSpan());
        }

        /// <returns>Bitmap of the current FlowDocumentPageViewer.</returns>
        protected Bitmap GetBitmapOfView()
        {
            UIElement element = ViewerBase;
            Bitmap capture = BitmapCapture.CreateBitmapFromElement(element);
            return capture;
        }

        /// <summary>
        /// Return region of DocumentPageView represented by the ITableSegment.
        /// </summary>
        /// <param name="pageCapture">DPV bitmap to select a sub region of.</param>
        /// <param name="segment">Sub region of bitmap to return.</param>
        /// <returns>Sub bitmap of pageCapture as defined by a selection.</returns>
        protected Bitmap GetSubBitmap(Bitmap pageCapture, ITableSegment segment)
        {
            Rect selectionArea = ComputeSelectionArea(segment);
            Bitmap subCapture = BitmapUtils.CreateSubBitmap(pageCapture, selectionArea);
            return subCapture;
        }

        /// <summary>
        /// Create bitmap with ITableSegment blacked out.
        /// </summary>
        /// <param name="pageCapture">DPV bitmap to blackout a portion of.</param>
        /// <param name="segment">Range to be blacked out.</param>
        /// <returns>Copy of pageCapture with specified region blacked out.</returns>
        protected Bitmap BlackOutSegment(Bitmap pageCapture, ITableSegment segment)
        {
            return BlackOutSegments(pageCapture, new ITableSegment[] { segment });
        }

        /// <summary>
        /// Create bitmap with ITableSegments blacked out.
        /// </summary>
        /// <param name="pageCapture">DPV bitmap to blackout a portion of.</param>
        /// <param name="segments">Ranges to be blacked out.</param>
        /// <returns>Copy of pageCapture with specified regions blacked out.</returns>
        protected Bitmap BlackOutSegments(Bitmap pageCapture, ITableSegment[] segments)
        {
            Bitmap blockedCapture = pageCapture;
            foreach (ITableSegment segment in segments)
            {
                Rect selectionArea = ComputeSelectionArea(segment);
                blockedCapture = BitmapUtils.CreateBlockedBitmap(blockedCapture, ToRectangle(selectionArea));
            }
            return blockedCapture;
        }

        /// <summary>
        /// Compute rectangle within DocumentPageView that represents a single ITableSegment.
        /// </summary>
        /// <param name="segment">Selection to compute bounding rect for.</param>
        /// <returns>Rectangle encompassing entire selection range.</returns>
        protected Rect ComputeSelectionArea(ITableSegment segment)
        {
            TextRange range = GetTextRange(TableSelectionFactory.Create(segment));
            System.Windows.Point topLeft = range.Start.GetCharacterRect(LogicalDirection.Forward).TopLeft;
            System.Windows.Point bottomRight = range.End.GetCharacterRect(LogicalDirection.Forward).BottomRight;

            Rect selectionArea = new Rect(topLeft, new Size(bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y));
            return selectionArea;
        }

        /// <summary>
        /// Convert between System.Windows.Rect and System.Drawing.Rectangle.
        /// </summary>
        protected Rectangle ToRectangle(Rect rect)
        {
            // In order to avoid border problems caused by rounding, make the 
            // return Rectangle slightly bigger on all sides.
            return new Rectangle(
                (int)(rect.X), (int)(rect.Y),
                (int)(rect.Width + .5 + 2), (int)(rect.Height + .5 + 2)
                );
        }

        /// <summary>
        /// Compate two bitmaps and check that the result is expected.  If there are differences
        /// and they are not expected, log the images and their difference.
        /// </summary>
        /// <param name="master">Master image.</param>
        /// <param name="sample">Sample image.</param>
        /// <param name="expectedResult">True if two images should be equal.</param>
        protected void CompareBitmaps(Bitmap master, Bitmap sample, bool expectedResult)
        {
            Bitmap difference;
            string log;
            bool result = AreBitmapsEqual(master, sample, out difference, out log);
            if (result != expectedResult)
            {
                string diffFile = "RenderDifference.bmp";
                string masterFile = "Master.bmp";
                string sampleFile = "Sample.bmp";
                printStatus(log);
                if (difference != null)
                {
                    difference.Save(diffFile);
                    LogToFile(diffFile);
                }
                master.Save(masterFile);
                sample.Save(sampleFile);                
                LogToFile(masterFile);
                LogToFile(sampleFile);
            }
            assertEquals("Verify result of bitmap comparison.", expectedResult, result);
        }

        bool AreBitmapsEqual(Bitmap master, Bitmap sample,
            out Bitmap differences, out string differenceLog)
        {
            ComparisonOperation op = new ComparisonOperation();
            op.MasterImage = master;
            op.SampleImage = sample;

            // Number of pixels that we will allow to be off before we consider the two images "different".
            int PixelTolerance = 5;

            // Allow some variation in color to account for differences in 
            // text rendering.
            op.Criteria = new ComparisonCriteria();
            op.Criteria.MaxColorDistance = (float).01;
            // Convert pixel difference count into a error proportion.
            op.Criteria.MaxErrorProportion = (float)PixelTolerance / (float)(master.Width * master.Height);

            ComparisonResult result = op.Execute();
            if (result.CriteriaMet)
            {
                differences = null;
                differenceLog = string.Empty;
                return true;
            }
            else
            {
                differences = new Bitmap(sample);
                result.HighlightDifferences(differences);
                // Use ToString rather than ToStringBrief - there will not be multiple
                // differences in a quick strict comparison.
                differenceLog = result.ToString();
                return false;
            }
        }

        #endregion

        #region Protected Properties

        protected TableSelectionFactory TableSelectionFactory
        {
            get
            {
                return _tableSelectionFactory;
            }
            set
            {
                _tableSelectionFactory = value;
            }
        }

        protected string TableName
        {
            get
            {
                return _tablename;
            }
        }

        #endregion

        #region Private Fields

        private string _tablename;
        TableSelectionFactory _tableSelectionFactory;

        #endregion
    }

    public class TableSelectionFactory
    {
        public TableSelectionFactory(string tablename) 
        {
            TableName = tablename;
        }

        public ISelectionData Create(ITableSegment segment)
        {
            return Create(new ITableSegment[] { segment })[0];
        }

        public ISelectionData[] Create(ITableSegment[] segments)
        {
            ISelectionData[] selections = new ISelectionData[segments.Length];
            for (int i=0; i < segments.Length; i++)
            {
                selections[i] = segments[i].ToTableSelection(TableName);
            }
            return selections;
        }
        
        public string TableName;
    }

    public interface ITableSegment {
        TableSelectionData ToTableSelection(string tablename);
    }

    public class LineBreak : ITableSegment
    {
        public virtual TableSelectionData ToTableSelection(string tablename)
        {
            return new TableSelectionData(tablename, PagePosition.Beginning, 0, PagePosition.Beginning, 0);
        }
    }

    public class TableSpan : ITableSegment
    {
        public TableSpan() : this(0,0) { }
        public TableSpan(int offsetBeforeTable, int offsetAfterTable)
        {
            OffsetBeforeTable = offsetBeforeTable;
            OffsetAfterTable = offsetAfterTable;            
        }

        public virtual TableSelectionData ToTableSelection(string tablename)
        {
            return new TableSelectionData(tablename, PagePosition.Beginning, OffsetBeforeTable, PagePosition.End, OffsetAfterTable);
        }

        protected int OffsetBeforeTable, OffsetAfterTable;
    }

    public class TableStart : TableSpan
    {
        public TableStart(int offsetBeforeTable, int endRow)
            : base(offsetBeforeTable, -1)
        {
            EndRow = endRow;
            OnlyBeforeTable = (EndRow == -1);
        }

        public override TableSelectionData ToTableSelection(string tablename)
        {
            if (!OnlyBeforeTable)
                return new TableSelectionData(tablename, PagePosition.Beginning, OffsetBeforeTable, EndRow);
            return new TableSelectionData(tablename, PagePosition.Beginning, -1, PagePosition.Beginning, OffsetBeforeTable);
        }

        public int EndRow;
        public bool OnlyBeforeTable = false;
    }

    public class TableEnd : TableSpan
    {
        public TableEnd(int startRow, int offsetAfterTable)
            : base(-1, offsetAfterTable)
        {
            StartRow = startRow;
        }

        public override TableSelectionData ToTableSelection(string tablename)
        {
            if (StartRow > 0)
                return new TableSelectionData(tablename, StartRow, PagePosition.End, OffsetAfterTable);
            return new TableSelectionData(tablename, PagePosition.End, OffsetAfterTable, PagePosition.End, 1);
        }

        int StartRow;
    }

    public class RowSpan : ITableSegment 
    {
        public RowSpan(int row) 
        {
            Row = row;
        }

        public TableSelectionData ToTableSelection(string tablename)
        {
            return new TableSelectionData(tablename, Row);
        }

        public int Row;
    }

    public class CellSpan : ITableSegment
    {

        public CellSpan(int rowNumber, int columnNumber)
            : this(rowNumber, columnNumber, rowNumber, columnNumber)
        {
            IsSingleCell = true;   
        }

        public CellSpan(int startRow, int startColumn, int endRow, int endColumn)
            : this(startRow, startColumn, endRow, endColumn, false)
        {
            // Nothing.
        }

        /// <param name="startRow"></param>
        /// <param name="startColumn"></param>
        /// <param name="endRow"></param>
        /// <param name="endColumn"></param>
        /// <param name="containsEndOfRow">True if span should contain linebreak at the end of the row.  
        /// This is used when generating expected anchors from multiple ITableSegments.</param>
        public CellSpan(int startRow, int startColumn, int endRow, int endColumn, bool containsEndOfRow)
        {
            ContainsEndOfRow = containsEndOfRow;
            StartRow = startRow;
            StartColumn = startColumn;
            EndRow = endRow;
            EndColumn = endColumn;
        }

        public TableSelectionData ToTableSelection(string tablename)
        {
            return new TableSelectionData(tablename, StartRow, StartColumn, EndRow, EndColumn);
        }

        public bool IsSingleCell = false;
        public bool ContainsEndOfRow = false;
        public int StartRow, EndRow;
        public int StartColumn, EndColumn;
    }

    /// <summary>
    /// Should be used when selection only contains a single table cell.  If this represents
    /// a portion of a larger anchor than use CellSpan(row, column) instead.
    /// </summary>
    public class SingleCell : CellSpan
    {
        public SingleCell(int rowNumber, int columnNumber)
            : base(rowNumber, columnNumber, rowNumber, columnNumber)
        {
            // nothing.
        }

        public TableSelectionData ToTableSelection(string tablename)
        {
            return new TableSelectionData(tablename, StartRow, StartColumn, PagePosition.Beginning, 1, PagePosition.End, -1);
        }
    }

    public class CellSubset : ITableSegment
    {
        public CellSubset(int row, int column, PagePosition startPosition, int startOffset, PagePosition endPosition, int endOffset)
        {
            Row = row;
            Column = column;
            StartPosition = startPosition;
            EndPosition = endPosition;
            StartOffset = startOffset;
            EndOffset = endOffset;
        }

        public TableSelectionData ToTableSelection(string tablename)
        {
            return new TableSelectionData(tablename, Row, Column, StartPosition, StartOffset, EndPosition, EndOffset);
        }

        int Row, Column;
        PagePosition StartPosition, EndPosition;
        int StartOffset, EndOffset;
    }

    public class CellMiddle : CellSubset
    {
        public CellMiddle(int row, int column, int startOffset, int endOffset)
            : base(row, column, PagePosition.Beginning, startOffset, PagePosition.End, endOffset)
        {
            // Empty.
        }
    }

    public class CellStart : CellSubset
    {
        public CellStart(int row, int column, int startOffset)
            : base(row, column, PagePosition.Beginning, 0, PagePosition.Beginning, startOffset)
        {
            // Empty.
        }
    }

    public class CellEnd : CellSubset
    {
        public CellEnd(int row, int column, int endOffset)
            : base(row, column, PagePosition.End, 0, PagePosition.End, endOffset)
        {
            // Empty.
        }

        public CellEnd(int row, int column, int endOffset, bool excludeEndingTab)
            : base(row, column, PagePosition.End, 0, PagePosition.End, endOffset)
        {
            ExcludeEndingTab = excludeEndingTab;
        }

        public bool ExcludeEndingTab = false;
    }
}	

