// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description: Common base class that can be shared across all TestSuites
//               that verify annotations on TextControls.

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using System.IO;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Reflection;
using Annotations.Test.Framework;
using Annotations.Test;
using Annotations.Test.Reflection;
using System.Xml;
using System.Text.RegularExpressions;
using System.Windows.Controls.Primitives;
using System.Windows.Annotations;
using System.Windows.Annotations.Storage;

using Proxy_AnnotationPublicNamespace = Proxies.System.Windows.Annotations;
using Proxy_AnnotationInternalNamespace = Proxies.MS.Internal.Annotations;
using Microsoft.Test.Logging;

namespace Avalon.Test.Annotations
{
    public abstract class ATextControlTestSuite : VisualAutomationTestSuite
	{
		#region TestSuite Methods

        [TestCase_Cleanup()]
        protected override void CleanupVariation()
        {
            try
            {
                DisableAnnotationService();                
            }
            finally
            {
                _annotationStream = null; // Clear stream.
                // Don't close the window just clear its content.
                MainWindow.Content = null;
                _flowDocumentProperties = new FlowDocumentProperties();
            }
        }

		/// <summary>
		/// Parse command line args to configure suite.
		/// </summary>
		/// <param name="args"></param>
		public override void ProcessArgs(string []args) 
		{
			base.ProcessArgs(args); // inherit base functionality.

			_annotationMode = DetermineAnnotationMode(args);
			printStatus("Annotation Type = '" + _annotationMode + "'.");
			_runMode = DetermineRunMode(args);
			printStatus("Run Mode = '" + _runMode + "'.");
			_testMode = DetermineTestMode(args);
			printStatus("Content Mode = '" + _testMode + "'.");

            // Control type should be determined after TestMode as this will affect the default.
            _targetControlType = DetermineTargetControlType(args);
            printStatus("Control Type = '" + _targetControlType + "'.");

            _annotationFileToImport = DetermineAnnotationFileToImport(args);

            if (_testMode == TestMode.Flow)
            {
                _flowDocumentProperties.ProcessArgs(args);
            }
		}

		protected override IList<string> UsageParameters()
		{
			IList<string> parameters = base.UsageParameters();
			parameters.Add("[fixed|flow] - control whether to test should run on FixedDocument or FlowDocument.");
			parameters.Add("[highlight|stickynote|inkstickynote] - control what kind of annotation type test should run for.");			
			parameters.Add("[/u] - this flag will cause all VisualScan tests to generate screen shots instead of doing actual comparisions.");
            parameters.Add("[/import=XXX] - indicates annotation file to be loaded at startup..");
			return parameters;
		}

		protected override IList<string> UsageExamples()
		{
			IList<string> examples = base.UsageExamples();
			examples.Add("'XXX.exe case1 fixed highlight' - run 'case1' on a FixedDocument using highlight annotations.");			
			examples.Add("'XXX.exe case1 /u' - everwhere a vscan comparison should be performed, just print a screenshot instead.");
			examples.Add("'XXX.exe case1 /rendermode=1 /u' - output software masters for each call to vscan comparison.");
            examples.Add("'XXX.exe case1 /import=annot.xml' - prepopulates store with annotations in 'annot.xml'.");
			return examples;
		}

		protected virtual TestMode DetermineTestMode(string[] args)
		{
			TestMode mode = TestMode.Fixed;
			for (int i = 0; i < args.Length; i++)
			{
				if (args[i].ToLower().Equals("flow"))
				{
					mode = TestMode.Flow;
					break;
				}
			}
			return mode;
		}

        protected virtual AnnotationMode DetermineAnnotationMode(string[] args)
        {
            AnnotationMode aMode = AnnotationMode.StickyNote;
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].ToLower().Equals("highlight"))
                {
                    aMode = AnnotationMode.Highlight;
                    break;
                }
                if (args[i].ToLower().Equals("inkstickynote"))
                {
                    aMode = AnnotationMode.InkStickyNote;
                    break;
                }			
            }			
            return aMode;
        }

        protected virtual RunMode DetermineRunMode(string[] args)
        {
			if (args.Length > 0)
			{				
				if (args[args.Length - 1].Equals("/u"))
					return RunMode.VScanUpdate;
			}
            return RunMode.Normal;
		}

        protected virtual AnnotatableTextControlTypes DetermineTargetControlType(string[] args)
        {            
            Match match = FindParameter(args, new Regex("/control=(.*)"));
            if (match != null && match.Success)
            {
                switch (match.Groups[1].Value)
                {
                    case "dv":
                        return AnnotatableTextControlTypes.DocumentViewer;
                    case "fdpv":
                        return AnnotatableTextControlTypes.FlowDocumentPageViewer;
                    case "fdsv":
                        return AnnotatableTextControlTypes.FlowDocumentScrollViewer;
                    case "fdr":
                        return AnnotatableTextControlTypes.FlowDocumentReader;
                    default:
                        throw new NotSupportedException("Control target of type '" + match.Groups[1].Value + ".");
                }
            }
            // Otherwise use content type to determine default control type.
            else
            {
                switch (ContentMode)
                {
                    case TestMode.Flow:
                        return AnnotatableTextControlTypes.FlowDocumentPageViewer;
                    case TestMode.Fixed:
                        return AnnotatableTextControlTypes.DocumentViewer;
                    default:
                        throw new NotImplementedException();
                }
            }

            throw new InvalidProgramException("No TextControl type was selected.");
        }

        public virtual string DetermineAnnotationFileToImport(string[] args)
        {
            string filename = null;
            Match match = FindParameter(args, new Regex("/import=(.*)"));
            if (match != null)
            {
                filename = match.Groups[1].Value;
                if (!File.Exists(filename))
                    throw new FileNotFoundException("Couldn't find annotation file to import: '" + filename + "'.");                
            }
            return filename;
        }

		#endregion			

		#region Setup Methods

		/// <summary>
		/// Create the basic logical tree.
		/// 
		/// NOTE: does not add content to the test DocumentViewer.
		/// </summary>
		protected virtual void SetupTestWindow()
		{
            _textControlWrapper = CreateTextControlWrapper(TargetControlType);
			SetupAnnotationService();
			SetupWindow();
		}

		/// <summary>
		/// Create, initialize, and show MainWindow.
		/// </summary>
		/// <remarks>
		/// Sets the position and window size using 'WindowSize' property.  Also may change
		/// the render more of the window.
		/// </remarks>
		protected virtual void SetupWindow()
		{
            printStatus("Setting up MainWindow...");
            // If previous variations have run before us the window should already be created.
            if (MainWindow == null)
            {
                // Make sure there isn't a window from another Suite we can use.
                if ((MainWindow = Application.Current.MainWindow) == null)
                    MainWindow = new Window();
            }

			MainWindow.Content = CreateWindowContents(); // Generate and set content.

			// Set the width and height before we show the window.
			Size windowSize = WindowSize;
			MainWindow.Width = windowSize.Width;
			MainWindow.Height = windowSize.Height;

			// Explicitly set the window position so that vscan takes consistent screenshots.
			MainWindow.Left = 10;
			MainWindow.Top = 10;

            // Set this as the application's primary window.
            Application.Current.MainWindow = MainWindow;

			// Show window before setting RenderMode otherwise HwndTarget will not exist.
			MainWindow.Show();

			// If we are updating vscan masters and the mode is set to Software, then we should
			// render the window in software mode.
			// 
			if (VScan.ComparisionMode == RenderMode.Software)
			{
				VScanModule.SetRenderModeToSoftware(MainWindow);
				printStatus("[Window.RenderMode='Software']");
			}
			else
			{
				printStatus("[Window.RenderMode='SystemDefault']");
			}

			// Verify that size is set correctly.
			AssertEquals("Verify Window Size.", windowSize, new Size(MainWindow.ActualWidth, MainWindow.ActualHeight));
			printStatus("Window size = (" + MainWindow.ActualWidth + "," + MainWindow.ActualHeight + ").");

			// For debugging.
			MainWindow.PreviewMouseRightButtonDown += printSize;
		}

		/// <summary>
		/// Generate default window layout: 
        /// -control for performing simple selections and annotation actions.
        /// -TextControl specific to content type.
		/// </summary>
		/// <returns>Root element of window contents that can be passed to Window.Content.</returns>
		protected virtual object CreateWindowContents()
		{
			DockPanel mainPanel = new DockPanel();			
			mainPanel.Children.Add(TextControl);

			return mainPanel;
		}


        /// <summary>
        /// Change the text control that is displaying the content to the specified type.
        /// </summary>
        /// <param name="newControlType">Type of control to switch view to.</param>
        protected virtual void SwitchTextControl(AnnotatableTextControlTypes newControlType)
        {
            bool serviceEnabled = Service != null && Service.IsEnabled;
            if (serviceEnabled)
                DisableAnnotationService();

            // Save and remove document.
            IDocumentPaginatorSource currentDocument = TextControlWrapper.Document;
            TextControlWrapper.Document = null;

            MainWindow.Content = null;
            _textControlWrapper = CreateTextControlWrapper(newControlType);
            MainWindow.Content = CreateWindowContents();

            // Restore document.
            TextControlWrapper.Document = currentDocument;

            if (serviceEnabled)
                SetupAnnotationService();
        }

		/// <summary>
		/// Create FileStream and enable an AnnotationService on target Control.
		/// </summary>
		protected virtual void SetupAnnotationService()
		{
            // Instantiate AnnotationService using reflection so that we avoid having to add 
            // cases for each new control that we are enabled on.
            ConstructorInfo ctor = typeof(AnnotationService).GetConstructor(new Type[] { TextControl.GetType() });
            if (ctor == null)
                throw new NotSupportedException("AnnotationService cannot be created for control of type '" + TextControl.GetType() + "'.");
            AnnotationService service = (AnnotationService)ctor.Invoke(new object[] { TextControl });
            service.Enable(SetupAnnotationStore());
            DispatcherHelper.DoEvents();
			printStatus("AnnotationService Enabled.");

            if (!string.IsNullOrEmpty(_annotationFileToImport))
            {
                printStatus("Importing annotations from disk: " + _annotationFileToImport);
                ImportAnnotations(_annotationFileToImport);
            }
		}

        protected virtual System.Windows.Annotations.Storage.AnnotationStore SetupAnnotationStore()
        {
            AnnotationStore store = new XmlStreamStore(AnnotationStream);
            store.AutoFlush = true;
            return store;
        }

		/// <summary>
		/// Intialize TextControlWrapper based on the value of TargetControlType.
		/// </summary>
        protected virtual ATextControlWrapper CreateTextControlWrapper(AnnotatableTextControlTypes targetType)
        {
            ATextControlWrapper wrapper = null;
            switch (targetType)
            {
                case (AnnotatableTextControlTypes.FlowDocumentPageViewer):
                    wrapper = new FlowDocumentPageViewerWrapper();
                    break;
                case (AnnotatableTextControlTypes.DocumentViewer):
                    wrapper = new DocumentViewerWrapper();
                    break;
                case (AnnotatableTextControlTypes.FlowDocumentScrollViewer):
                    wrapper = new FlowDocumentScrollViewerWrapper();
                    break;
                case (AnnotatableTextControlTypes.FlowDocumentReader):
                    wrapper = new FlowDocumentReaderWrapper();
                    break;
                default:
                    throw new NotSupportedException("Unknown type " + TargetControlType);
            }
            return wrapper;
        }

        /// <summary>
        /// Set DocumentViewerBase.Document to the given IDocumentPaginatorSource.
        /// </summary>
        /// <remarks>
        /// Doesn't return until IsPageCountValid == true.
        /// </remarks>
        protected void SetContent(IDocumentPaginatorSource document)
        {
            // Apply user defined properties/values to the document before setting it.
            if (document is FlowDocument)
            {
                _flowDocumentProperties.ApplyProperties(document as FlowDocument);
            }

            // Set document.
            TextControlWrapper.Document = document;
        }

		#endregion

		#region Window Actions

		/// <summary>
		/// Size that the window will be set to when SetupTestWindow is called.
		/// </summary>
		protected virtual Size WindowSize
		{
			get
			{
				Size winSize = new Size(913, 670);
				return winSize;
			}
		}

		private void printSize(object sender, EventArgs e)
		{
			printStatus("Window size = (" + MainWindow.ActualWidth + "," + MainWindow.ActualHeight + ").");
		}

        private void GetInfo(object sender, EventArgs args)
        {
            MessageBox.Show("Window size = (" + MainWindow.ActualWidth + "," + MainWindow.ActualHeight + ")");
        }

        protected IDocumentPaginatorSource LoadContent(string filename)
		{
            return AnnotationTestHelper.LoadContent(filename);			
		}

		protected void CloseWindow()
		{            
            DisableAnnotationService();
            MainWindow.Close();
            MainWindow = null;
            Application.Current.MainWindow = null;
		}

        protected void DisableAnnotationService()
        {
            // Use proxies because we don't know what the TextControl type is. 
            Proxy_AnnotationPublicNamespace.AnnotationService service = Proxy_AnnotationPublicNamespace.AnnotationService.GetService(TextControl);
            try
            {
                if (service != null)
                {
                    service.Disable();
                    DispatcherHelper.DoEvents();
                }
            }
            finally
            {
                // Reset stream (will be used by next SetupService call).
                AnnotationStream.Seek(0, SeekOrigin.Begin);
            }
        }

		protected void ResizeWindow(Size size)
		{
            printStatus("Resizing Window... (" + size + ")");
            MainWindow.Height = size.Height;
            MainWindow.Width = size.Width;
            DispatcherHelper.DoEvents();
        }

        protected virtual void ChangeWindowHeight(double delta)
        {
			SetWindowHeight(MainWindow.ActualHeight + delta);
        }

        protected virtual void ChangeWindowWidth(double delta)
        {
            SetWindowWidth(MainWindow.ActualWidth + delta);
        }

        protected virtual void SetWindowHeight(double height)
        {
            MainWindow.Height = height;
			DispatcherHelper.DoEvents();
        }

        protected virtual void SetWindowWidth(double width)
        {
            MainWindow.Width = width;
			DispatcherHelper.DoEvents();
        }

        protected void SetWindowFocus()
        {
            printStatus("Clicking on Window to give it focus.");
            // Point to click at is at the current window location + 100,15
            UIAutomationModule.MoveToAndClick(new Point(MainWindow.Left + 100, MainWindow.Top + 15));
            DispatcherHelper.DoEvents();
        }

        #endregion Window Actions      

		#region VScan Actions

		protected virtual void CompareToMaster(string masterFile, string toleranceFile)
		{
			if (Mode == RunMode.VScanUpdate)
			{				
				string[] nameSegments = masterFile.Split('.');
				string outputName = nameSegments[0] + "_UPDATE";
				VScan.TakeScreenShot(TextControl, outputName);
			}
			else
				VScan.CompareToMaster(TextControl, masterFile, toleranceFile);
		}

		#endregion

		#region Annotation Actions

        #region StickyNote

        /// <summary>
        /// Returns a StickyNoteWrapper for the currently attached StickyNote.
        /// </summary>
        /// <exception cref="FailTestException">There is not exactly 1 attached StickyNote.</exception>
        protected StickyNoteWrapper CurrentlyAttachedStickyNote
        {
            get
            {
                IList<StickyNoteControl> stickynotes = AnnotationComponentFinder.GetVisibleStickyNotes(TextControl);
                if (stickynotes.Count != 1)
                    failTest("Method should only be used if only 1 attached StickyNote but there were '" + stickynotes.Count + "'.");
                return new StickyNoteWrapper(stickynotes[0], "currentSn");
            }
        }

        protected StickyNoteWrapper GetStickyNoteWithAuthor(string author)
        {
            return GetStickyNotesByAuthor(new string[] { author })[0];
        }

        /// <summary>
        /// Returns ordered array of StickyNoteWrappers for SNs that exist in the AdornerLayer that have the given authors. 
        /// </summary>
        /// <remarks>
        /// This method assumes that all authors are unique.  If not, the return value of this method is undefined.
        /// </remarks>
        /// <exception cref="TestCaseFailedException">If SN does not exist for each author.</exception>
        protected StickyNoteWrapper[] GetStickyNotesByAuthor(string[] titles)
        {
            IList<StickyNoteControl> stickynotes = AnnotationComponentFinder.GetVisibleStickyNotes(TextControl);
            Assert("Verify number of titles is <= num attached StickyNotes.", titles.Length <= stickynotes.Count);
            StickyNoteWrapper[] wrappers = new StickyNoteWrapper[titles.Length];
            for (int outer = 0; outer < titles.Length; outer++)
            {
                StickyNoteWrapper match = null;
                foreach (StickyNoteControl sn in stickynotes)
                {
                    // Wrap each StickyNoteControl becuase Title is not exposed publicly.
                    StickyNoteWrapper current = new StickyNoteWrapper(sn, "sn" + outer);
                    if (current.Author.Equals(titles[outer]))
                    {
                        match = current;
                        break;
                    }
                }

                AssertNotNull("Verify SN was found for title '" + titles[outer] + "'.", match);
                wrappers[outer] = match;
            }
            return wrappers;
        }

        /// <summary>
        /// Returns ordered array of StickyNoteWrappers for SNs that exist in the current AdornerLayer.
        /// </summary>
        protected StickyNoteWrapper[] GetStickyNoteWrappers()
        {
            IList<StickyNoteControl> stickynotes = AnnotationComponentFinder.GetVisibleStickyNotes(TextControl);
            StickyNoteWrapper[] wrappers = new StickyNoteWrapper[stickynotes.Count];
            for (int i = 0; i < wrappers.Length; i++)
            {
                wrappers[i] = new StickyNoteWrapper(stickynotes[i], "sn" + i);
            }
            return wrappers;
        }

        /// <summary>
        /// Check StickyNote's bounds against the current ViewPort position and determine whether or
        /// not any portion of the Note is visible.  Fail if visibility of Note is opposite from the
        /// expected.
        /// </summary>
        /// <param name="author">Author of Note to verify.</param>
        /// <param name="isVisibleInViewport">Expected visibility of Note, True if portion of note should be visible.</param>
        protected void VerifyNoteViewportVisibility(string author, bool isVisibleInViewport)
        {
            printStatus("Testing Note '" + author + "' visibility...");

            StickyNoteWrapper note = GetStickyNoteWithAuthor(author);
            ScrollViewer scrollViewer = TextControlWrapper.ScrollViewer;
            
            if (scrollViewer == null)
                throw new InvalidOperationException("Method should not be used if no ScrollViewer is present.");

            Size noteSize = note.BoundingRect.Size;

            // Compute Note position relative to the ScollViewer.
            Point topLeft = new Point(0, 0);
            Point bottomRight = new Point(0, 0);
            GeneralTransform transformToScrollViewer = note.Target.TransformToVisual(scrollViewer);
            if (!transformToScrollViewer.TryTransform(new Point(0, 0), out topLeft))
                throw new Exception("Failed to transform point.");
            if (!transformToScrollViewer.TryTransform(new Point(noteSize.Width, noteSize.Height), out bottomRight))
                throw new Exception("Failed to transform point.");

            VerifyViewportVisibility(new Rect(topLeft.X, topLeft.Y, bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y), isVisibleInViewport);            
        }

        #endregion

        #region Highlight


        /// <summary>
        /// Set the colour for a Higlight annotation creation.
        /// </summary>
        /// <returns>True if an annotation was created.</returns>
        private Brush SelectColor(int index)
        {
            Brush color = null;

            switch (index)
            {
                case 1: color = Brushes.Aqua; break;
                case 2: color = Brushes.BlueViolet; break;
                case 3: color = Brushes.Chocolate; break;
                case 4: color = Brushes.DarkOliveGreen; break;
                case 5: color = Brushes.Fuchsia; break;
            }
            return color;
        }

        /// <summary>
        /// Set the colour for a Higlight annotation creation.
        /// </summary>
        /// The color is simly a number, with 0 meaning null, and others being random
        protected void SetHighlightColor(int colorIndex)
        {
            if (colorIndex == 0)
            {
                HighlightColor = null;
            }
            else if (colorIndex > 5)
            {
                AssertEquals("Choose a color less than 6.", 5, colorIndex);
            }
            else
            {
                HighlightColor = SelectColor(colorIndex);
            }
        }

        public bool CreateHighlight(ATextControlWrapper TextControlWrapper, ISelectionData selection, int colorIndex)
        {
            AnnotationStore store = Service.Store;
            int initialAnnotationCount = store.GetAnnotations().Count;

            SetHighlightColor(colorIndex);
            selection.SetSelection(TextControlWrapper.SelectionModule);
            bool res = CreateAnnotation(AnnotationMode.Highlight);

            int finalAnnotationCount = store.GetAnnotations().Count;
            DispatcherHelper.DoEvents();
            return finalAnnotationCount > initialAnnotationCount;
        }

        public void DeleteHighlight(ATextControlWrapper TextControlWrapper, ISelectionData selection)
        {
            selection.SetSelection(TextControlWrapper.SelectionModule);
            DeleteAnnotation(AnnotationMode.Highlight);
        }

        #endregion

        #region General

        #region Visibility

        /// <summary>
        /// Verify that anchor is not visible and if a StickyNote that its bubble is also not visible.
        /// </summary>
        /// <param name="author">Name of annotation.</param>
        /// <param name="selection">Anchor of annotation.</param>
        /// <param name="isVisibleInViewport">Whether or not annotation is expected to be visible.</param>
        protected void VerifyAnnotationVisibility(string author, ISelectionData anchor, bool isVisibleInViewport)
        {
            printStatus("Verify Anchor Visiblity...");
            VerifyAnchorVisibility(anchor, isVisibleInViewport);
            if (AnnotationType == AnnotationMode.StickyNote || AnnotationType == AnnotationMode.InkStickyNote)
            {
                printStatus("Verify Note visiblity...");
                VerifyNoteViewportVisibility(author, isVisibleInViewport);
            }
        }

        /// <summary>
        /// Given a Rect in the Viewport coordinate space, verify whether or not any portion of it
        /// is visible in the ViewPort.
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="isVisibleInViewport"></param>
        protected void VerifyViewportVisibility(Rect rect, bool isVisibleInViewport)
        {
            ScrollViewer scrollViewer = TextControlWrapper.ScrollViewer;
            if (scrollViewer == null)
                throw new InvalidOperationException("Method should not be used if no ScrollViewer is present.");
            

            // Now compare the relative note position to the position of the ScrollViewer's ViewPort.
            printStatus("\tViewPort: Offset=(" + scrollViewer.HorizontalOffset + "," + scrollViewer.VerticalOffset + "), Size=(" + scrollViewer.ViewportWidth + "," + scrollViewer.ViewportHeight +").");
            printStatus("\tTest Rect: Offset=(" + rect.TopLeft + "), Size=(" + rect.Size + ").");

            double leftEdge = rect.TopLeft.X;
            double rightEdge = rect.TopRight.X;
            double topEdge = rect.TopLeft.Y;
            double bottomEdge = rect.BottomLeft.Y;

            bool isHorizontallyVisible = 
                leftEdge >= 0 && leftEdge <= scrollViewer.ViewportWidth ||   // Left inside ViewPort.
                rightEdge >= 0 && rightEdge <= scrollViewer.ViewportWidth || // Right inside ViewPort.
                leftEdge <= 0 && rightEdge >= scrollViewer.ViewportWidth;    // Horizontally spans viewport.

            bool isVerticallyVisible = 
                topEdge >= 0 && topEdge <= scrollViewer.ViewportHeight ||        // Top inside viewport.
                bottomEdge >= 0 && bottomEdge <= scrollViewer.ViewportHeight ||  // Bottom inside ViewPort.
                topEdge <= 0 && bottomEdge >= scrollViewer.ViewportHeight;       // Vertically spans ViewPort.
            printStatus("\tResults: HorizontalTest=" + isHorizontallyVisible + ", VeriticalTest=" + isVerticallyVisible + ".");
            AssertEquals("Verify visibility.", isVisibleInViewport, isHorizontallyVisible && isVerticallyVisible);
        }

        /// <summary>
        /// Check whether or not the START of the given anchor is inside the viewport.
        /// </summary>
        /// <param name="anchor">Anchor to verify</param>
        /// <param name="isVisibleInViewport">True if anchor should be visible.</param>
        protected void VerifyAnchorVisibility(ISelectionData anchor, bool isVisibleInViewport)
        {
            TextRange range = anchor.GetSelectionAsTextRange(TextControlWrapper.SelectionModule);
            Rect charRect = (Rect) ReflectionHelper.InvokeMethod(range.Start, "GetCharacterRect", new object[] { LogicalDirection.Forward });
            AssertNotNull("Verify CharacterRect is not null.", charRect);            
            VerifyViewportVisibility(charRect, isVisibleInViewport);
        }

        #endregion

        #region Attachment

        /// <summary>
        /// Verify that there is a currently Attached annotation with the expected anchor.
        /// </summary>
        /// <param name="expectedAnchor"></param>
        protected void VerifyAnnotationWithAnchorExists(string expectedAnchor)
        {
            IList<Proxy_AnnotationInternalNamespace.IAttachedAnnotation> attachedAnnotations = Service.GetAttachedAnnotations();
            bool found = false;

            foreach (Proxy_AnnotationInternalNamespace.IAttachedAnnotation attachedAnnotation in attachedAnnotations)
            {
                object anchor = attachedAnnotation.AttachedAnchor;
                AssertNotNull("AttachedAnchor should be of type 'TextAnchor' but was of type '" + anchor.GetType().Name + "'.", AnnotationTestHelper.IsTextAnchor(anchor));

                if (string.Equals(expectedAnchor, AnnotationTestHelper.GetText(anchor)))
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                printStatus("Actual anchors were:");
                foreach (Proxy_AnnotationInternalNamespace.IAttachedAnnotation attachedAnnotation in attachedAnnotations)
                    printStatus("\t'" + SelectionModule.PrintFriendlySelection(AnnotationTestHelper.GetText(attachedAnnotation.AttachedAnchor)) + "'");
                printStatus("Expected anchor:");
                printStatus("\t'" + SelectionModule.PrintFriendlySelection(expectedAnchor) + "'");
                failTest("Did not find AttachedAnnotation with expected anchor.");
            }
        }

		/// <summary>
		/// Verify that there is one AttachedAnnotation with an AttachedAnchor that meets the given constraints.
		/// </summary>
		protected void VerifyAnnotation(string expectedAnchorStart, string expectedAnchorEnd, int expectedAnchorLength)
		{
			Proxy_AnnotationPublicNamespace.AnnotationService service = Service;
            IList<Proxy_AnnotationInternalNamespace.IAttachedAnnotation> attachedAnnotations = service.GetAttachedAnnotations();
			AssertEquals("Verify only 1 attached annotation.", 1, attachedAnnotations.Count);

			Proxy_AnnotationInternalNamespace.IAttachedAnnotation current = attachedAnnotations[0];
			object anchor = current.AttachedAnchor;
            AssertNotNull("AttachedAnchor should be TextAnchor.", AnnotationTestHelper.IsTextAnchor(anchor));

            string anchorText = AnnotationTestHelper.GetText(anchor);
            printStatus("AttachedAnchor is '" + anchorText + "'.");
            Assert("Verify start of anchor.", anchorText.StartsWith(expectedAnchorStart));
            Assert("Verify end of anchor.", anchorText.EndsWith(expectedAnchorEnd));
            AssertEquals("Verify length of anchor.", expectedAnchorLength, anchorText.Length);
		}

		/// <summary>
		/// Weak test that only verifies that the expected number are found.
		/// </summary>
		/// <param name="expectedNumber"></param>
		protected void VerifyNumberOfAttachedAnnotations(int expectedNumber)
		{
			Proxy_AnnotationPublicNamespace.AnnotationService service = Service;
			IList<Proxy_AnnotationInternalNamespace.IAttachedAnnotation> attachedAnnotations = service.GetAttachedAnnotations();
			AssertEquals("Verify number of attached annotations.", expectedNumber, attachedAnnotations.Count);
		}

		/// <summary>
		/// Weak test that only verifies that the expected number are found.
		/// </summary>
		/// <param name="expectedNumber"></param>
		protected void VerifyNumberOfAttachedAnnotations(string statusMsg, int expectedNumber)
		{
			printStatus(statusMsg);
			Proxy_AnnotationPublicNamespace.AnnotationService service = Service;
			IList<Proxy_AnnotationInternalNamespace.IAttachedAnnotation> attachedAnnotations = service.GetAttachedAnnotations();
			AssertEquals("Verify number of attached annotations.", expectedNumber, attachedAnnotations.Count);
		}

		/// <summary>
		/// Verify that N AttachedAnnotations exist and that they are attached to their expected anchors.
		/// </summary>
		protected void VerifyAnnotations(int expectedNumAttachedAnnotations, string[] expectedAnchors)
		{
			Proxy_AnnotationPublicNamespace.AnnotationService service = Service;
			IList<Proxy_AnnotationInternalNamespace.IAttachedAnnotation> attachedAnnotations = service.GetAttachedAnnotations();
			AssertEquals("Verify number of attached annotations.", expectedNumAttachedAnnotations, attachedAnnotations.Count);

			// AttachedAnnotations is un-ordered, so search accordingly.
			for (int i = 0; i < expectedNumAttachedAnnotations; i++)
			{
				Proxy_AnnotationInternalNamespace.IAttachedAnnotation current = attachedAnnotations[i];
				object anchor = current.AttachedAnchor;
                AssertNotNull("AttachedAnchor should be of type 'TextAnchor' but was of type '" + current.AttachedAnchor.GetType().Name + "'.", AnnotationTestHelper.IsTextAnchor(anchor));

				bool found = false;
				for (int k = 0; k < expectedNumAttachedAnnotations; k++)
				{
                    if (string.Equals(expectedAnchors[k], AnnotationTestHelper.GetText(anchor)))
					{
						found = true;
						break;
					}
				}
				if (!found)
				{
					printStatus("Expected anchors were:");
					foreach (string anchorString in expectedAnchors)
                        printStatus("\t'" + SelectionModule.PrintFriendlySelection(anchorString) + "'");

                    printStatus("Unexpected anchor was:");
                    printStatus("\t'" + SelectionModule.PrintFriendlySelection(AnnotationTestHelper.GetText(anchor)) + "'.");
                    failTest("AttachedAnnotation with unexpected anchor.");
				}
			}
		}

        /// <summary>
        /// Verify that N AttachedAnnotations exist and that they are attached to their expected anchors.
        /// </summary>
        protected void VerifyAnnotations(string[] expectedAnchors)
        {
            VerifyAnnotations(expectedAnchors.Length, expectedAnchors);
        }		

        /// <summary>
        /// Verify that 1 AttachedAnnotation exist and that it is attached to the expected anchor.
        /// </summary>
        protected void VerifyAnnotation(string expectedAnchor)
        {
            VerifyAnnotations(1, new string[] { expectedAnchor });
        }

        #endregion

        #region Create

        protected void CreateAnnotation(AnnotationDefinition definition)
        {
            definition.Create(TextControlWrapper);
        }

		/// <summary>
		/// Actual call to AnnotationService to create an annotation.
		/// </summary>
		/// <returns>True if an annotation was created.</returns>
        public bool CreateAnnotation()
		{			
			return CreateAnnotation(AnnotationType);
		}

        public bool CreateAnnotation(AnnotationMode type)
        {
            return CreateAnnotation(type, string.Empty);
        }

		public bool CreateAnnotation(AnnotationMode type, string author)
		{
			if (TextControlWrapper.SelectionModule.Selection != null)
				printStatus("Creating annotation for selection: '" + SelectionModule.PrintFriendlySelection(TextControlWrapper.SelectionModule.Selection.Text) + "'.");

			AnnotationStore store = Service.Store;
			int initialAnnotationCount = store.GetAnnotations().Count;
            //
            // MAKE SURE WE ARE USING PUBLIC API HERE!
            //
            AnnotationService public_service = (AnnotationService)Service.Delegate;
			switch (type)
			{
				case AnnotationMode.StickyNote:
                    AnnotationHelper.CreateTextStickyNoteForSelection(public_service, author);
					break;
				case AnnotationMode.Highlight:
                    AnnotationHelper.CreateHighlightForSelection(public_service, author, HighlightColor);
					break;
				case AnnotationMode.InkStickyNote:
                    AnnotationHelper.CreateInkStickyNoteForSelection(public_service, author);
					break;
				default:
					throw new NotSupportedException("Annotation type '" + AnnotationType + "' is not supported.");
			}			
			int finalAnnotationCount = store.GetAnnotations().Count;

			DispatcherHelper.DoEvents();
			return finalAnnotationCount > initialAnnotationCount;
		}

        public void CreateAnnotation(ISelectionData selection, AnnotationMode type, string author)
        {            
            selection.SetSelection(TextControlWrapper.SelectionModule);
            CreateAnnotation(type, author);
        }

        public void CreateAnnotation(ISelectionData selection, AnnotationMode type)
        {
            CreateAnnotation(selection, type, string.Empty);
        }

        public void CreateAnnotation(ISelectionData selection)
        {
            CreateAnnotation(selection, string.Empty);
        }

        public void CreateAnnotation(ISelectionData selection, string author)
        {
            printStatus("Create " + AnnotationType + " by '" + author + "'...");
            CreateAnnotation(selection, AnnotationType, author);
        }

        public void CreateAnnotation(bool expectedResult)
        {
            bool result = CreateAnnotation();
            AssertEquals("Verify result of Create.", expectedResult, result);
        }

        /// <summary>
        /// Call create annotation and verify that the given exception occurs.
        /// </summary>
        /// <param name="selection">If not null, call SetSelection before CreateAnnotation.</param>
        /// <param name="expectedExceptionType">Excepted exception type to be thrown.</param>
        protected void VerifyCreateAnnotationFails(ISelectionData selection, Type expectedExceptionType)
        {
            if (selection != null)
                selection.SetSelection(TextControlWrapper.SelectionModule);
            bool exceptionOccurred = false;
            try
            {
                CreateAnnotation();
            }
            catch (Exception e)
            {
                AssertEquals("Verify exception type.", expectedExceptionType, e.GetType());
                exceptionOccurred = true;
            }
            Assert("Verify that exception occurred.", exceptionOccurred);
        }     

        #endregion

        #region Delete

        public void DeleteAnnotation(ISelectionData selection, AnnotationMode type)
        {
            printStatus("Delete " + type + "...");
            selection.SetSelection(TextControlWrapper.SelectionModule);
            DeleteAnnotation(type);
        }

		public void DeleteAnnotation(ISelectionData selection)
		{
            DeleteAnnotation(selection, AnnotationType);
		}           	

		public void DeleteAnnotation()
		{
			DeleteAnnotation(AnnotationType);
		}

        public void DeleteAnnotation(AnnotationMode type)
        {
            printStatus("Deleting annotation for selection: '" + SelectionModule.PrintFriendlySelection(TextControlWrapper.SelectionModule.Selection.Text) + "'.");

            //
            // MAKE SURE WE ARE USING PUBLIC API HERE!
            //
            AnnotationService public_service = (AnnotationService)Service.Delegate;
            switch (type)
            {
                case AnnotationMode.StickyNote:
                    AnnotationHelper.DeleteTextStickyNotesForSelection(public_service);
                    break;
                case AnnotationMode.Highlight:
                    AnnotationHelper.ClearHighlightsForSelection(public_service);
                    break;
                case AnnotationMode.InkStickyNote:
                    AnnotationHelper.DeleteInkStickyNotesForSelection(public_service);
                    break;
                default:
                    throw new NotSupportedException("Annotation type '" + AnnotationType + "' is not supported.");
            }
            DispatcherHelper.DoEvents();
        }

        #endregion

        /// <summary>
        /// Load the given file as an Annotation store and add all the annotations on
        /// the current Viewer.
        /// </summary>
        /// <param name="filename"></param>
        protected void ImportAnnotations(string filename)
        {
            using (Stream fileStream = new FileStream(filename, FileMode.Open))
            {
                AnnotationStore tmpStore = new XmlStreamStore(fileStream);
                IList<Annotation> toImport = tmpStore.GetAnnotations();
                foreach (Annotation annot in toImport)
                {
                    TextControlWrapper.Service.Store.AddAnnotation(annot);
                }
            }
        }

        #endregion

        #endregion Annotation Actions

        #region Text Actions

        /// <summary>
        /// Get the TextRange in the current DocumentViewer content that this selection object 
        /// corresponds to.
        /// </summary>
        public TextRange GetTextRange(ISelectionData selection)
        {
            return selection.GetSelectionAsTextRange(TextControlWrapper.SelectionModule);
        }

        /// <summary>
		/// Get the text of the current DocumentViewer content that this selection object 
		/// corresponds to.
		/// </summary>
		public virtual string GetText(ISelectionData selection)
		{
            return selection.GetSelection(TextControlWrapper.SelectionModule);
		}

		public void SetSelection(ISelectionData selection)
		{
			selection.SetSelection(TextControlWrapper.SelectionModule);
		}

		#endregion

        #region Wrapper Delegates

        protected void SetZoom(double zoomPercent) 
        {
            printStatus("SetZoom(" + zoomPercent + ")");
            TextControlWrapper.SetZoom(zoomPercent); 
        }
        protected void ZoomIn()
        {
            printStatus("ZoomIn...");
            TextControlWrapper.ZoomIn();
        }
        protected void ZoomOut()
        {
            printStatus("ZoomOut...");
            TextControlWrapper.ZoomOut();
        }
        protected void GoToEnd() 
        {
            printStatus("GoToEnd...");
            TextControlWrapper.GoToEnd(); 
        }
        protected void GoToStart() 
        {
            printStatus("GoToStart...");
            TextControlWrapper.GoToStart(); 
        }
        public void PageUp()
        {
            printStatus("PageUp()");
            TextControlWrapper.PageUp();
        }
        public void PageUp(int n)
        {
            printStatus("PageUp(" + n + ")");
            TextControlWrapper.PageUp(n);
        }
        public void PageDown()
        {
            printStatus("PageDown()");
            TextControlWrapper.PageDown();
        }
        public void PageDown(int n)
        {
            printStatus("PageDown(" + n + ")");
            TextControlWrapper.PageDown(n);
        }
        public void BringIntoView(ISelectionData selection)
        {
            printStatus("BringIntoView...");
            TextControlWrapper.BringIntoView(selection);
        }
        public void GoToPage(int n)
        {
            printStatus("GoToPage(" + n + ")");
            TextControlWrapper.GoToPage(n);
        }

        #endregion

        #region Properties

        protected Proxy_AnnotationPublicNamespace.AnnotationService Service
        {
            get
            {
                return TextControlWrapper.Service;
            }
        }

        protected virtual Stream AnnotationStream
        {
            get
            {
                if (_annotationStream == null)
                {
                    _annotationStream = new MemoryStream();
                }
                return _annotationStream;
            }
        }

        protected Control TextControl
        {
            get { return TextControlWrapper.Target; }
        }

        virtual protected RunMode Mode
        {
            get { return _runMode; }
        }

        virtual public AnnotationMode AnnotationType
        {
            get { return _annotationMode; }
        }

        virtual public TestMode ContentMode
        {
            get { return _testMode; }
        }

        virtual protected AnnotatableTextControlTypes TargetControlType
        {
            get
            {
                return _targetControlType;
            }
        }

        protected ATextControlWrapper TextControlWrapper
        {
            get
            {
                return _textControlWrapper;
            }
            set
            {
                _textControlWrapper = value;
            }
        }

        #endregion

        #region Fields

        protected Window MainWindow;
        private Stream _annotationStream;
        private ATextControlWrapper _textControlWrapper;        

        // contains the color that the Highlight will be made in
        private Brush HighlightColor = null;

        private FlowDocumentProperties _flowDocumentProperties = new FlowDocumentProperties();

        private RunMode _runMode = RunMode.Normal;
        private AnnotationMode _annotationMode = AnnotationMode.StickyNote;
        private TestMode _testMode;
        private AnnotatableTextControlTypes _targetControlType;

        private string _annotationFileToImport = null;

		#endregion Protected Fields

        #region Internal Enums

        /// <summary>
		/// Different behavior modes that subclasses of this may or may not support.
		/// </summary>
        protected enum RunMode
        {
            Normal,
            VScanUpdate		// VScan tests should dump screenshots instead of doing actual comparisons.
        }

        /// <summary>
        /// Different text controls that support annotations.
        /// </summary>
        protected enum AnnotatableTextControlTypes
        {
            DocumentViewer,
            FlowDocumentPageViewer,
            FlowDocumentScrollViewer,
            FlowDocumentReader
        }

        #endregion
    }

	public enum AnnotationMode
	{
		StickyNote,
		InkStickyNote,
		Highlight
	}

	public enum TestMode
	{
		Fixed,
		Flow
	}
}
