// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: 

using System;
using System.Windows;
using Annotations.Test.Framework;
using System.IO;
using System.Windows.Documents;
using System.Windows.Controls;
using System.Collections.Generic;
using Annotations.Test.Reflection;
using System.Windows.Annotations.Storage;
using System.Printing;
using System.Windows.Xps;
using System.Text.RegularExpressions;
using Proxies.MS.Internal.Annotations.Component;

using StickyNoteControl = System.Windows.Controls.StickyNoteControl;
using System.Collections;
using System.Windows.Annotations;
using System.Windows.Xps.Packaging;

namespace Avalon.Test.Annotations
{
	/// <summary>
	/// Base class for all Annotation printing tests.
	/// </summary>
    public abstract class APrintingSuite : ADefaultContentSuite
    {
		#region Overrides

        [TestCase_Setup()]
        protected override void DoSetup()
        {
            SetupPrintServer();
            base.DoSetup();
        }

        protected override void DoExtendedSetup()
        {
            SetZoom(80);
        }

		/// <summary>
		/// Create window that has two DocumentViewers, one on either side of the window.  The one on the left,
		/// will be the Source viewer (DocViewerWrapper) and the one on the right will be the result viewer which
		/// will be used to set the results of ADP.GetPage.
		/// </summary>
		/// <returns></returns>
		protected override object CreateWindowContents()
		{
			Menu menu = new Menu();
			DockPanel.SetDock(menu, Dock.Top);

			#region Print

			Button printButton = new Button();
			printButton.Content = "Print to Printer";
			printButton.Click += OnPrintToPrinter;

			//menu.Items.Add(CreatePrintQueueCombo());
			menu.Items.Add(printButton);

			#endregion								

			Grid grid = new Grid();
			grid.ShowGridLines = true;
			grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());	
			Grid.SetColumn(DocViewerWrapper.ViewerBase, 0);
			grid.Children.Add(DocViewerWrapper.ViewerBase);

            printViewer = new DocumentViewer();
            Grid.SetColumn(printViewer, 1);
            grid.Children.Add(printViewer);

			DockPanel panel = new DockPanel();
			panel.Children.Add(menu);
			panel.Children.Add(grid);

			return panel;
		}

		public override void ProcessArgs(string[] args)
		{
			base.ProcessArgs(args);

			foreach (string arg in args)
			{
				if (new Regex("/async=true").Match(arg).Success)
					_asyncMode = true;
			}
		}

		protected override IList<string> UsageParameters()
		{
			IList<string> usage = base.UsageParameters();
			usage.Add("/async=true - Invoke async equivalents of methods where applicable.");
			return usage;
		}

		protected override IList<string> UsageExamples()
		{
			IList<string> examples = base.UsageExamples();
			examples.Add("XXX.exe adp_getpage1 flow - test ADP.GetPage(int n) on flow content.");
			examples.Add("XXX.exe adp_getpage1 fixed /async=true - test ADP.GetPageAsync(int n) on fixed content");
			return examples;
		}

		#endregion

		#region Event Handlers	

		/// <summary>
		/// Called when "Print To Printer" button is pressed.
		/// Should send ADP to selected Printer.
		/// </summary>
		protected void OnPrintToPrinter(object sender, RoutedEventArgs e)
		{
            printStatus("Print - Starting.");
            try
            {
                if (_printedDocument != null)
                {
                    printViewer.Document = null;
                    _printedDocument.Close();
                }
                _printedDocument = SerializeToXps(CreateAnnotationDocumentPaginator());
                printViewer.Document = _printedDocument.GetFixedDocumentSequence();
            }
            catch (Exception exp)
            {
                MessageBox.Show("Exception: " + exp.ToString());
            }
			printStatus("Print - Complete.");
		}        

		#endregion

		#region Protected Methods		

		protected void SetupPrintServer()
		{
			try
			{
				_printServer = new LocalPrintServer();
			}
			catch (Exception e)
			{
				printStatus("Exception occurred initializing LocalPrintServer: " + e.Message);
			}

		}

		/// <summary>
		/// Return valid DocumentPaginatorSource for either a Fixed or Flow document depending upon
		/// the test mode.
		/// </summary>
		protected IDocumentPaginatorSource DocumentPaginatorSource()
		{
			throw new NotImplementedException(); // 
		}	

		/// <summary>
		/// Create an AnnotationDocumentPaginator for the CurrentAnnotationStore.
		/// </summary>
		protected DocumentPaginator CreateAnnotationDocumentPaginator()
		{
			return CreateAnnotationDocumentPaginator(CurrentAnnotationStore);		
		}
		
		/// <summary>
		/// Create AnnotationDocumentPaginator for the given stream.
		/// </summary>
		/// <param name="stream"></param>
		/// <returns></returns>
		protected DocumentPaginator CreateAnnotationDocumentPaginator(Stream stream)
		{
            return new AnnotationDocumentPaginator(LoadTestDocument().DocumentPaginator, stream);
		}

		/// <summary>
		/// Create AnnotationDocumentPaginator for the given store.
		/// </summary>
		protected DocumentPaginator CreateAnnotationDocumentPaginator(AnnotationStore store)
		{
			// Don't use the same IDP that we just added the annotations to becuase it is inside a Do----enViewer,
			// and therefore will have an AnnotationService overhead.  Need to ensure that we don't depend on this case.
			IDocumentPaginatorSource original = LoadTestDocument();
			DocumentPaginator adp = null;

			// If FlowContent, set the PageSize to be the same size as the FlowDocument we are currently viewing,
			// otherwise our positions won't match up.
			if (ContentMode == TestMode.Flow)
			{
                // Right FlowDocument defaults to System parameters unless it is attached to the FlowDocumentPageViewer logical tree. 
				// therefore we have to override the defaults in order to get the same pagination in the printout as the original.
				FlowDocument actualDoc = (FlowDocument)ViewerBase.Document;
				FlowDocument printDoc = (FlowDocument)original;
				printDoc.FontSize = actualDoc.FontSize;
				printDoc.FontFamily = actualDoc.FontFamily;
				printDoc.TextAlignment = actualDoc.TextAlignment;

				adp = CreateAnnotationDocumentPaginator(printDoc, store);
                adp.PageSize = ((IDocumentPaginatorSource)actualDoc).DocumentPaginator.PageSize;
				while (!adp.IsPageCountValid)
					DispatcherHelper.DoEvents();
			}
			else
				adp = CreateAnnotationDocumentPaginator(original, store);

			return adp;	
		}	

		/// <summary>
		/// Create and ADP for the given IDP and AnnotationStore.
		/// </summary>
		private DocumentPaginator CreateAnnotationDocumentPaginator(IDocumentPaginatorSource document, AnnotationStore store)
		{
			return new AnnotationDocumentPaginator(document.DocumentPaginator, store);
		}

		/// <summary>
		/// Create a DocumentStateInfo assuming that all annotations have already been manually added.
		/// </summary>
		protected DocumentStateInfo SetupDocumentState()
		{
			return GenerateStateInfo();
		}

		/// <summary>
		/// Create annotation for the given selection and then create and return a DocumentStatInfo.
		/// </summary>
		/// <returns>DocumentStateInfo reflecting the state of the ViewerBase.Document.</returns>
		protected DocumentStateInfo SetupDocumentState(ISelectionData selection)
		{
			return SetupDocumentState(new ISelectionData[] { selection });
		}

		/// <summary>
		/// Create annotations for the given selections and then create and return a DocumentStatInfo.
		/// </summary>
		/// <returns>DocumentStateInfo reflecting the state of the ViewerBase.Document.</returns>
		protected DocumentStateInfo SetupDocumentState(ISelectionData[] selections)
		{
			CreateAnnotations(selections);
			return GenerateStateInfo();
		}

		/// <summary>
		/// Call create on each of the given AnnotationDefinitions and then create and return a DocumentStatInfo.
		/// </summary>
		/// <returns>DocumentStateInfo reflecting the state of the ViewerBase.Document.</returns>
		protected DocumentStateInfo SetupDocumentState(AnnotationDefinition[] annotationDefinitions)
		{
			foreach (AnnotationDefinition def in annotationDefinitions)
				def.Create(DocViewerWrapper, true); // Always make annotation visible.
			return GenerateStateInfo();
		}

        /// <summary>
        /// Ensure that DocumentPaginator can be serialized to an Printer without exception.
        /// </summary>
        /// <param name="paginator"></param>
        protected void VerifySerializingToPrinter(DocumentPaginator paginator)
        {
            printStatus("Serializing - Start");

            PrintQueue target = _printServer.GetPrintQueue(BvtPrinterName);
            if (target == null)
            {
                failTest("Warning: could not find PrintQueue with name '" + BvtPrinterName + "', ensure that BVT printers are installed.");
            }

            XpsDocumentWriter writer = PrintQueue.CreateXpsDocumentWriter(target);
            writer.Write(paginator);
            printStatus("Serializing - End");
        }

        /// <summary>
        /// Ensure that DocumentPaginator can be serialized to an XpsDocument without exception.
        /// </summary>
        /// <param name="paginator"></param>
        protected void VerifySerializingToXps(DocumentPaginator paginator)
        {
            printStatus("Serializing - Start");

            using (XpsDocument doc = SerializeToXps(paginator))
            {
                AssertNotNull("Verify doc is not null.", doc);
            }

            printStatus("Serializing - End");
        }

		/// <summary>
		/// Do either IDP.GetPage or IDP.GetPageAsync depending upon the async mode.
		/// </summary>
		/// <returns>Requested DocumentPage.</returns>
		/// <exception cref="Exception">If Async operation appears stalled.</exception>
		protected DocumentPage GetPage(DocumentPaginator idp, int page)
		{
			DocumentPage result;
			if (DoAsync)
			{
				result = new AsyncOperationHelper().GetPage(idp, page);
			}
			else
			{
				result = idp.GetPage(page);
			}
			return result;
		}

		/// <summary>
		/// Compare the annotations in the VisualTree of the given DocumentPage to the given state info.
		/// </summary>
		/// <param name="expectedStates">Expected states of annotations on the given DocumentPage.</param>
		/// <param name="actualState">DocumentPage to compare to expected state.</param>
		protected void VerifyPrintedAnnotations(IList<AnnotationStateInfo> expectedStates, DocumentPage actualState)
		{
			IList<StickyNoteControl> printedStickyNotes = AnnotationComponentFinder.GetVisibleStickyNotes(actualState.Visual);
			IList printedHighlights = AnnotationComponentFinder.GetVisibleHighlightComponents(actualState.Visual);

			AssertEquals("Verify number of printed annotations.", expectedStates.Count, printedStickyNotes.Count + printedHighlights.Count);			
			
			// Verify all StickyNotes:
			foreach (StickyNoteControl sn in printedStickyNotes)
				VerifyAnnotationStateIsExpected(expectedStates, StickyNoteStateInfo.FromStickyNote(sn));			

			// Verify all Highlights:
			foreach (object highlight in printedHighlights)
				VerifyAnnotationStateIsExpected(expectedStates, HighlightStateInfo.FromHighlightComponent(highlight));		
		}

		private void VerifyAnnotationStateIsExpected(IList<AnnotationStateInfo> expectedStates, AnnotationStateInfo actualState)
		{
			bool match = false;
			foreach (AnnotationStateInfo expected in expectedStates)
			{
				if (expected.GetType().Equals(actualState.GetType()))
				{
					if (expected.Equals(actualState))
					{
						match = true;
							break;
					}
				}
			}
			if (!match)
			{
				printStatus("Expected Annotation states:");
				foreach (AnnotationStateInfo expected in expectedStates)
					printStatus("\t" + expected.ToString());
				failTest("Unexpected Annotation was found: " + actualState.ToString() + ".");
			}
			printStatus("Verified printed annotation: " + actualState.ToString() + ".");
		}


		/// <summary>
		/// Verify that annotations are correct on a number of pages by comparing AnnotationStateInfo objects.
		/// </summary>
		/// <param name="docState">Expected document state.</param>
		/// <param name="docPages">DocumentPages to parse and verify.</param>
		/// <param name="pages">Page to compare.</param>
		protected void VerifyPrintedAnnotations(DocumentStateInfo docState, IDictionary<int, DocumentPage> docPages, int[] pages)
		{
			foreach (int page in pages)
			{
				printStatus("Verifying page '" + page + "'.");
				VerifyPrintedAnnotations(docState.AnnotationState(page), docPages[page]);
			}
		}

		/// <summary>
		/// Verify that the correct number of annotations exist in the AdornerLayer of the given
		/// DocumentPage.
		/// </summary>
		protected void VerifyNumberOfPrintedAnnotations(DocumentPage page, int expectedNumAnnotations)
		{
			IList<StickyNoteControl> printedStickyNotes = AnnotationComponentFinder.GetVisibleStickyNotes(page.Visual);
			AssertEquals("Verify number of annotations.", expectedNumAnnotations, printedStickyNotes.Count);
		}

		#endregion

		#region Protected Properties

		protected bool DoAsync
		{
			get { return _asyncMode; }
		}

		protected AnnotationStore CurrentAnnotationStore
		{
			get
			{
				return Service.Store;
			}
		}

		protected Stream CurrentAnnotationStream
		{
			get
			{
				Stream stream = AnnotationStream;
				stream.Seek(0, SeekOrigin.Begin); // Reset stream to start.
				return stream;
			}
		}

		#endregion

		#region Private Methods

        private XpsDocument SerializeToXps(DocumentPaginator paginator)
        {
            string filepath = "test.xps";
            if (File.Exists(filepath))
            {
                File.Delete(filepath);
            }

            XpsDocument doc = new XpsDocument(filepath, FileAccess.ReadWrite);
            try
            {
                XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(doc);
                writer.Write(paginator);
            }
            catch
            {
                doc.Close();
            }
            return doc;
        }

		/// <summary>
		/// The actual act of create annotations on the document.
		/// </summary>
		private void CreateAnnotations(ISelectionData[] selections)
		{
			foreach (ISelectionData selection in selections)
			{
				selection.SetSelection(DocViewerWrapper.SelectionModule);
				CreateAnnotation();
			}
		}

		/// <summary>
		/// For each page, walk the visual tree and create state objects for each AnnotationComponent we find.
		/// </summary>
		/// <returns></returns>
		private DocumentStateInfo GenerateStateInfo()
		{
			DocumentStateInfo context = new DocumentStateInfo();
			AnnotationComponentFinder finder = new AnnotationComponentFinder(DocViewerWrapper.ViewerBase);
			DocViewerWrapper.WholePageLayout(); // process a page at a time.
			for (int i = 0; i < DocViewerWrapper.PageCount; i++)
			{
				DocViewerWrapper.GoToPage(i);
                context.AddState(i, new VisualTreeWalker<StickyNoteControl>().FindChildren(ViewerBase));
				context.AddState(i, finder.GetHighlightComponents()); // Note: this will fail if multiple pages are visible.
			}
			return context;
		}
		
		private ComboBox CreatePrintQueueCombo()
		{
			ComboBox printCombo = new ComboBox();
			try
			{
				if (_printServer != null)
				{
					_availablePrinters = _printServer.GetPrintQueues(new EnumeratedPrintQueueTypes[] { EnumeratedPrintQueueTypes.Connections });
					foreach (PrintQueue queue in _availablePrinters)
					{
						printCombo.Items.Add(queue.Name);
					}
				}
			}
			catch (Exception e)
			{
				printStatus("Failed to get PrintQueues: '" + e.Message + "'.");
			}
			printCombo.SelectionChanged += OnPrintSelectionChanged;
			return printCombo;
		}

		private void OnPrintSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ComboBox printCombo = (ComboBox)e.Source;
			string selection = (string)printCombo.SelectedItem;
			_selectedPrinter = (string.IsNullOrEmpty(selection)) ? null : selection;
			printStatus("Currently selected printer is: '" + _selectedPrinter + "'");
		}

		#endregion

		#region Fields

		LocalPrintServer _printServer;
		string _selectedPrinter = null;
		private bool _asyncMode = false;

        protected DocumentViewer printViewer;
        XpsDocument _printedDocument = null;

        static string BvtPrinterName = "TestPrinter";

		PrintQueueCollection _availablePrinters;

		#endregion

		#region Inner Classes 

		/// <summary>
		/// Class that helps invoke and verify Asynchronous operations on an IDP.
		/// </summary>
		public class AsyncOperationHelper
		{
			/// <summary>
			/// Calls GetPageAsync but makes it appear synchronous to the caller.
			/// </summary>
			/// <returns>Requested page.</returns>
			public DocumentPage GetPage(DocumentPaginator idp, int page)
			{
				GetPages(idp, new int[] { page });
				return _resultDict[page];
			}

			/// <summary>
			/// Call GetPageAsync for each of the given page numbers.
			/// </summary>
			/// <param name="pages">Ordered list of pages to call GetPageAsync for.</param>
			/// <returns>Dictionary mapping from page number to DocumentPage.</returns>
			public IDictionary<int, DocumentPage> GetPages(DocumentPaginator idp, int[] pages)
			{
				Start_GetPages(idp, pages);
				return WaitForEnd_GetPages();				
			}

			/// <summary>
			/// Called to start GetPageAsync.
			/// </summary>
			/// <remarks>
			/// Method returns immediately.  Call WaitForEnd_GetPage to synchronously wait for the
			/// async operation to complete.
			/// </remarks>
			public void Start_GetPage(DocumentPaginator idp, int page)
			{
				Start_GetPages(idp, new int[] { page });
			}

			/// <summary>
			/// Synchronously waits for async operations started by Start_GetPage to complete.
			/// </summary>
			public DocumentPage WaitForEnd_GetPage()
			{
				return WaitForEnd_GetPages()[_currentPages[0]];
			}

			/// <summary>
			/// Called to start GetPageAsync for each of the specified pages.
			/// </summary>
			/// /// <remarks>
			/// Method returns immediately.  Call WaitForEnd_GetPage to synchronously wait for the
			/// async operations to complete.
			/// </remarks>
			public void Start_GetPages(DocumentPaginator idp, int[] pages)
			{
				if (_jobInProgress)
					throw new InvalidProgramException("Can only call Start_GetPage once before WaitForEnd.");

				_currentPages = pages;
				_currentIdp = idp;
				_jobInProgress = true;
				_resultDict.Clear();

				idp.GetPageCompleted += OnGetPagesComplete;
				foreach(int page in pages)
					idp.GetPageAsync(page, PageToState(page));
			}

			/// <summary>
			/// Synchronously waits for async operations started by Start_GetPages to complete.
			/// </summary>
			public IDictionary<int, DocumentPage> WaitForEnd_GetPages()
			{
				// Synchronous yield to dispatcher until async operation is complete.
				int yieldCount = 0;
				while (_resultDict == null || _resultDict.Keys.Count < _currentPages.Length)
				{
					if (yieldCount > _yieldLimit)
						break;
					yieldCount++;
					DispatcherHelper.DoEvents(); // Yield.
				}

				_jobInProgress = false;
				_currentIdp.GetPageCompleted -= OnGetPagesComplete;

				return _resultDict;
			}

			/// <summary>
			/// Call CancelAsync for each active query.
			/// </summary>
			/// <returns>Any results that had been generated.</returns>
			public IDictionary<int, DocumentPage> CancelAll_GetPages()
			{
				foreach (int page in _currentPages)
					_currentIdp.CancelAsync(PageToState(page));
				return WaitForEnd_GetPages();
			}

			/// <summary>
			/// Call CancelAsync for each of the given UserState objects.
			/// </summary>
			/// <returns>Any results that had been generated.</returns>
			public IDictionary<int, DocumentPage> Cancel_GetPages(int [] pagesToCancel)
			{
				foreach (int page in pagesToCancel)
					_currentIdp.CancelAsync(PageToState(page));
				return WaitForEnd_GetPages();
			}

			private void OnGetPagesComplete(object sender, GetPageCompletedEventArgs e)
			{
				if (!e.Cancelled)
					_resultDict.Add(e.PageNumber, e.DocumentPage);
			}

			private object PageToState(int page)
			{
				return "page" + page.ToString();
			}

			#region Fields

			private bool _jobInProgress = false;
			private int[] _currentPages;
			private DocumentPaginator _currentIdp;
			private IDictionary<int, DocumentPage> _resultDict = new Dictionary<int, DocumentPage>();

			/// <summary>
			/// Limit to the number of DispatcherQueue lengths we will wait for an async
			/// operation to complete.
			/// </summary>
			int _yieldLimit = 10;

			#endregion 
		}

		#endregion
	}

	//
	// 

	#region Mock Objects	

	public class AnnotationDocumentPaginatorSource
	{
		public AnnotationDocumentPaginatorSource(DocumentPaginator originalDocument, AnnotationStore annotationStore)
		{
			throw new NotImplementedException();
		}

		public AnnotationDocumentPaginatorSource(DocumentPaginator originalDocument, Stream stream)
		{
			throw new NotImplementedException();
		}

		public DocumentPaginator DocumentPaginator
		{
			get { throw new NotImplementedException(); }
		}
	}

    #endregion
}	

