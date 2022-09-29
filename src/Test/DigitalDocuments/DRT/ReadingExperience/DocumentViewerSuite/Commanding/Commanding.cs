// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
//
// Description: Tests DocumentViewer's operations and Commands
//

using DRT;
using System;
using System.Collections;   //IList, for selection tests
using System.Collections.Generic;    //IList, for selection tests
using System.IO;    // FilePath, for LoadXaml
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Markup; //For XamlReader, ParserContext

namespace DRTDocumentViewerSuite
{
      
    /// <summary>
    /// CommandingSuite is responsible for running the Suite of DocumentViewer & Command tests.
    /// </summary>
    public sealed class CommandingSuite : DrtTestSuite
    {
        public CommandingSuite() : base("Commanding")
        {
            TeamContact = "WPF";
            Contact = "Microsoft";
        }

        public override DrtTest[] PrepareTests()
        {
            _state = State.Start;

            Visual root = CreateTree();
            DRT.Show(root);           

            // return the lists of tests to run against the tree
            return new DrtTest[]{
                        new DrtTest( WaitForPaginationCompleted ),
                        new DrtTest( TestFitToWidth ),
                        new DrtTest( WaitForPaginationCompleted ),
                        new DrtTest( PrepareForTestFitToHeight ),
                        new DrtTest( TestFitToHeight ),
                        new DrtTest( VerifyTestFitToHeight ),
                        new DrtTest( TestZoom ),
                        new DrtTest( WaitForPaginationCompleted ),
                        new DrtTest( TestDecreaseZoom ),
                        new DrtTest( WaitForPaginationCompleted ),
                        new DrtTest( TestIncreaseZoom ),
                        new DrtTest( WaitForPaginationCompleted ),
                        new DrtTest( TestScrollPageDown ),
                        new DrtTest( TestScrollPageUp ),
                        new DrtTest( TestMoveDown ),
                        new DrtTest( TestMoveUp ),
                        new DrtTest( TestMoveRight ),
                        new DrtTest( TestMoveLeft ),
                        new DrtTest( TestMoveToEnd ),
                        new DrtTest( VerifyTestMoveToEnd ),
                        new DrtTest( TestMoveToHome),
                        new DrtTest( VerifyTestMoveToHome ),

                        // keep these tests at the end of the suite,
                        // because they change the view state so 
                        // dramatically.
                        new DrtTest( PrepareForTestGotoNextPage ),
                        new DrtTest( TestGotoNextPage ),
                        new DrtTest( VerifyTestGotoNextPage ),
                        new DrtTest( TestGotoPreviousPage ),
                        new DrtTest( VerifyTestGotoPreviousPage ),
                
                        // setting the Find DRT to be the last,
                        // as it causes a selection and will impact
                        // the later Zoom behaviors.
                        new DrtTest( TestFind ),
                        new DrtTest( ValidateFind ),
                        new DrtTest( TestFindPageChange ),
            };

        }

        /// <summary>
        /// Parse any command-line arguments here
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="option"></param>
        /// <param name="args"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public override bool HandleCommandLineArgument(string arg, bool option, string[] args, ref int k)
        {
            bool handled = false;

            if (option)
            {
                switch (arg)
                {
                    case "delay":
                        // If there's an integer argument after the "-delay" option flag, use it
                        // as the delay value in msec; otherwise use the default.
                        _delay = _defaultDelay;
                        if (k != args.Length - 1)
                        {
                            try
                            {
                                _delay = Int32.Parse(args[k+1]);
                                ++k;
                            }
                            catch (FormatException)
                            {
                            }
                        }
                        handled = true;
                        break;
                }
            }

            return handled;
        }

        #region Individual tests

        // Run this test until pagination completes.
        private void WaitForPaginationCompleted()
        {
            if (_state != State.PaginationCompleted)
            {
                DRT.Pause(_paginationDelay);
                DRT.ResumeAt(WaitForPaginationCompleted);
            }
        }

        private void TestFitToWidth()
        {            
            // Need to find actual width of page.
            DocumentViewer.FitToWidthCommand.Execute(null, _documentViewer);
            Console.WriteLine("After FitToWidth command: ExtentWidth = {0}.", _documentViewer.ExtentWidth);
            // Because the document is paginated in a single column, test to see
            //   if the page's width is the same as the viewport width.
            DRT.Assert(_documentViewer.ExtentWidth == _documentViewer.ViewportWidth,
                String.Format("Expected ExtentWidth = {0}, actual ExtentWidth = {1}.", _documentViewer.ViewportWidth, _documentViewer.ExtentWidth));
            if (_delay > 0)
                DRT.Pause(_delay);
        }

        private void PrepareForTestFitToHeight()
        {
            Console.WriteLine("Setting zoom to {0}% to prep for FitToHeight test.", _prepForFitToHeightZoomValue);

            // First set the Zoom to a value that ensure FitToHeight will change the height of element.
            _documentViewer.Zoom = _prepForFitToHeightZoomValue;

            if (_delay > 0)
                DRT.Pause(_delay);
        }

        private void TestFitToHeight()
        {
            //Verify documentViewer's Zoom value is what we expect.
            CheckExpectedZoom(_prepForFitToHeightZoomValue);

            _beforeFitToHeight = GetPageHeight();
            Console.WriteLine("Before FitToHeight command: page height = {0}.", _beforeFitToHeight);

            // Execute FitToHeightCommand.
            DocumentViewer.FitToHeightCommand.Execute(null, _documentViewer);

            if (_delay > 0)
                DRT.Pause(_delay);
        }

        private void VerifyTestFitToHeight()
        {
            // Grab final height of the 1st page.
            _afterFitToHeight = GetPageHeight();
            Console.WriteLine("After FitToHeight command: page height = {0}.", _afterFitToHeight);

            DRT.Assert(_beforeFitToHeight > _afterFitToHeight,
                String.Format("Expected page height to shrink. Before value = {0}, after value = {1}.", _beforeFitToHeight, _afterFitToHeight));
            
            // Test is complete. Now restore Zoom to 100% and go back to the top of the
            // first page.
            _documentViewer.Zoom = 100.0;
            _documentViewer.HorizontalOffset = 0.0;
            _documentViewer.VerticalOffset = 0.0;

            if (_delay > 0)
                DRT.Pause(_delay);
        }

        private void TestFind()
        {
            // Locate the Find TextBox.
            TextBox findTextBox = GetNamedElement(_findTextBoxName) as TextBox;

            // Verify the TextBox does not have focus.
            CheckElementFocus(findTextBox, false);

            // Click in the center of the window to make sure that we know we have
            // text to search.
            Point point = new Point(25,25);
            Input.MoveToAndClick(point);

            // Verify that the selection is what we think it is.
            string selectionBefore = GetSelection();

            DRT.Assert(selectionBefore.Equals(beforeFindSelection, StringComparison.Ordinal),
                String.Format("Current selection is wrong. Expected: \"{0}\", Actual: \"{1}\" . ",
                    selectionBefore,
                    beforeFindSelection));


            // Now execute the Find command.
            ApplicationCommands.Find.Execute(null, _documentViewer);
        }

        private void ValidateFind() {
            // Locate the Find TextBox.
            TextBox findTextBox = GetNamedElement(_findTextBoxName) as TextBox;

            // Locate the FindNext Button.
            Button findNextButton = GetNamedElement(_findNextButtonName) as Button;

            //verify this gives focus to the Find TextBox.
            CheckElementFocus(findTextBox, true);    

            // Now test the functionality.
            // Set the Textbox Text to "Suspendisse".  (Only appears on 2nd page)
            findTextBox.Text = "Suspendisse";
            DRT.Assert(findTextBox.Text == "Suspendisse",
                String.Format("{0}'s Text value should be Suspendisse.", _findTextBoxName));

            // Invoke the FindNext button.
            DoClick(findNextButton);

            // Then verify that the selection has changed.
            string selectionAfter = GetSelection();

            DRT.Assert(selectionAfter.Equals(findTextBox.Text, StringComparison.OrdinalIgnoreCase),
                String.Format("Current selection is wrong. Expected: \"{0}\", Actual: \"{1}\" . ",
                    selectionAfter,
                    findTextBox.Text));

            if (_delay > 0)
                DRT.Pause(_delay);
        }

        private void TestFindPageChange()
        {
            //The find result should have brought page 2 into view; ensure this is the case.
            DRT.Assert(_documentViewer.MasterPageNumber == 2, "Page 2 was not brought into view.");
        }

        private void TestScrollPageDown()
        {
            _documentViewer.HorizontalOffset = 0.0;
            _documentViewer.VerticalOffset = 0.0;
            ComponentCommands.ScrollPageDown.Execute(null, _documentViewer);
            Console.WriteLine("After ScrollPageDown command: OffsetY = {0}.", _documentViewer.VerticalOffset);
            CheckExpectedVerticalPosition(_documentViewer.ViewportHeight);
            if (_delay > 0)
                DRT.Pause(_delay);
        }

        private void TestScrollPageUp()
        {
            ComponentCommands.ScrollPageUp.Execute(null, _documentViewer);
            Console.WriteLine("After ScrollPageUp command: OffsetY = {0}.", _documentViewer.VerticalOffset);
            CheckExpectedVerticalPosition(0.0);
            if (_delay > 0)
                DRT.Pause(_delay);
        }

        private void TestMoveDown()
        {
            ComponentCommands.MoveDown.Execute(null, _documentViewer);
            Console.WriteLine("After MoveDown command: OffsetY = {0}.", _documentViewer.VerticalOffset);
            CheckExpectedVerticalPosition(16.0);
            if (_delay > 0)
                DRT.Pause(_delay);
        }

        private void TestMoveUp()
        {
            ComponentCommands.MoveUp.Execute(null, _documentViewer);
            Console.WriteLine("After MoveDown command: OffsetY = {0}.", _documentViewer.VerticalOffset);
            CheckExpectedVerticalPosition(0.0);
            if (_delay > 0)
                DRT.Pause(_delay);
        }

        private void TestMoveRight()
        {
            ComponentCommands.MoveRight.Execute(null, _documentViewer);
            Console.WriteLine("After MoveRight command: OffsetX = {0}.", _documentViewer.HorizontalOffset);
            CheckExpectedHorizontalPosition(16.0);
            if (_delay > 0)
                DRT.Pause(_delay);
        }

        private void TestMoveLeft()
        {
            ComponentCommands.MoveLeft.Execute(null, _documentViewer);
            Console.WriteLine("After MoveLeft command: OffsetX = {0}.", _documentViewer.HorizontalOffset);
            CheckExpectedHorizontalPosition(0.0);
            if (_delay > 0)
                DRT.Pause(_delay);
        }

        private void PrepareForTestGotoNextPage()
        {
            //Set our view to show two pages up
            DocumentViewer.FitToMaxPagesAcrossCommand.Execute(2, _documentViewer);

            //Make sure we are at the start of the document.
            NavigationCommands.GoToPage.Execute(1, _documentViewer);
        }

        private void TestGotoNextPage()
        {
            //Verify we are in the correct starting place.
            CheckExpectedMasterPageNumber(1);

            //Scroll To Next Row.
            NavigationCommands.NextPage.Execute(null, _documentViewer);
        }

        private void VerifyTestGotoNextPage()
        {
            Console.WriteLine("After PreviousPage command: MasterPageNumber = {0}.", _documentViewer.MasterPageNumber);

            CheckExpectedMasterPageNumber(3);

            if (_delay > 0)
                DRT.Pause(_delay);
        }

        private void TestGotoPreviousPage()
        {
            //Verify we are in the correct starting place place.
            CheckExpectedMasterPageNumber(3);

            //Scroll To Previous Row.
            NavigationCommands.PreviousPage.Execute(null, _documentViewer);
        }

        private void VerifyTestGotoPreviousPage()
        {
            Console.WriteLine("After NextPage command: MasterPageNumber = {0}.", _documentViewer.MasterPageNumber);

            CheckExpectedMasterPageNumber(1);

            //Restore the setting to 1 page across, viewing the 1st page.
            DocumentViewer.FitToMaxPagesAcrossCommand.Execute(1, _documentViewer);
            _documentViewer.Zoom = 100.0;
            NavigationCommands.GoToPage.Execute(1, _documentViewer);

            if (_delay > 0)
                DRT.Pause(_delay);
        }

        private void TestMoveToHome()
        {
            _documentViewer.FirstPage();
            Console.WriteLine("After FirstPage command: OffsetY = {0}.", _documentViewer.VerticalOffset);
            CheckExpectedVerticalPosition(0.0);

            if (_delay > 0)
                DRT.Pause(_delay);
        }


        private void VerifyTestMoveToHome()
        {
            Console.WriteLine("After LastPage command: MasterPageNumber = {0}.", _documentViewer.MasterPageNumber);

            CheckExpectedMasterPageNumber(1);
            
            if (_delay > 0)
                DRT.Pause(_delay);
        }

        private void TestMoveToEnd()
        {
            _documentViewer.LastPage();
            Console.WriteLine("After LastPage command: OffsetY = {0}.", _documentViewer.VerticalOffset);
            
            CheckExpectedVerticalPosition( _lastPageOffset );
            
            if (_delay > 0)
                DRT.Pause(_delay);
        }

        private void VerifyTestMoveToEnd()
        {
            Console.WriteLine("After LastPage command: MasterPageNumber = {0}.", _documentViewer.MasterPageNumber);

            CheckExpectedMasterPageNumber(_documentViewer.PageCount);

            if (_delay > 0)
                DRT.Pause(_delay);
        }

        private void TestZoom()
        {         
            NavigationCommands.Zoom.Execute(1200.0, _documentViewer);
            Console.WriteLine("After Zoom command: Zoom = {0}.", _documentViewer.Zoom);
            CheckExpectedZoom(1200.0);

            //Reset the offset to the top (zooming in on fixed content changes the offset)
            _documentViewer.VerticalOffset = 0.0;
            _documentViewer.HorizontalOffset = 0.0;

            if (_delay > 0)
                DRT.Pause(_delay);            
        }

        private void TestDecreaseZoom()
        {
            NavigationCommands.DecreaseZoom.Execute(null, _documentViewer);
            Console.WriteLine("After DecreaseZoom command: Zoom = {0}.", _documentViewer.Zoom);
            CheckExpectedZoom(800.0);

            //Reset the offset to the top (zooming in on fixed content changes the offset)
            _documentViewer.VerticalOffset = 0.0;
            _documentViewer.HorizontalOffset = 0.0;

            if (_delay > 0)
                DRT.Pause(_delay);
        }
        private void TestIncreaseZoom()
        {            
            NavigationCommands.IncreaseZoom.Execute(null, _documentViewer);
            Console.WriteLine("After IncreaseZoom command: Zoom = {0}.", _documentViewer.Zoom);
            CheckExpectedZoom(1200.0);

            //Reset the offset to the top (zooming in on fixed content changes the offset)
            _documentViewer.VerticalOffset = 0.0;
            _documentViewer.HorizontalOffset = 0.0;

            if (_delay > 0)
                DRT.Pause(_delay);
        }

        #endregion Individual tests

        #region Event handlers


        private void OnPaginationCompleted(Object sender, EventArgs e)
        {
            switch (_state)
            {
                case State.Start :
                case State.PaginationCompleted: // When we test the Zoom commands, we'll repaginate, which
                                                // will cause us to get another PaginationComplete event
                                                // while we're already in the PaginitionComplete state.
                                                // Don't complain about that.
                    _state = State.PaginationCompleted;
                    break;

                default :
                    DRT.Assert(false, "OnPaginationCompleted: The order of event firing is wrong");
                    break;
            }
        }
        

        #endregion Event handlers

        #region Implementation helpers

        /// <summary>
        /// Load Xaml content from file.
        /// </summary>
        /// <param name="filename">Relative filename to load.</param>
        private System.Windows.Documents.IDocumentPaginatorSource LoadXaml(string filename)
        {
            object newXaml = null;

            // Open and parse the XAML file.
            if (File.Exists(filename))
            {
                //Need to grab the fileInfo, so that we can sucessfully build a 
                //Uri for the ParserContext.
                FileInfo fileInfo = new FileInfo(filename);
                FileStream fileStream = fileInfo.OpenRead();
                Uri contentUri = new Uri(fileInfo.DirectoryName + "\\", UriKind.Absolute);

                try
                {
                    // Let the parser know where the related content files and resource files are.
                    ParserContext parserContext = new ParserContext();
                    parserContext.BaseUri = contentUri;

                    newXaml = System.Windows.Markup.XamlReader.Load(fileStream, parserContext);
                    // Leave fileStream open due to async loading.
                    //  fileStream.Close();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.Assert(false, ex.ToString());
                }
            }

            if (newXaml is System.Windows.Documents.IDocumentPaginatorSource)
            {
                return (System.Windows.Documents.IDocumentPaginatorSource)newXaml;
            }
            else
            {
                System.Diagnostics.Debug.Assert(false, "DocumentViewer can only accept children of type IDocumentPaginatorSource.");
                return null;
            }
        }

        private Visual CreateTree()
        {
            _documentViewer = new DocumentViewer();
            _documentViewer.Document = LoadXaml(@"DrtFiles\DocumentViewerSuite\Commanding\FixedDocument.xaml");
            _documentViewer.FitToWidth();

            DRT.Assert(_documentViewer.Document != null,
                String.Format("Document should not be null."));
            ((DynamicDocumentPaginator)_documentViewer.Document.DocumentPaginator).PaginationCompleted += new EventHandler(OnPaginationCompleted);
            //If the content is already paginated (as is the case for certain Fixed content)
            if (_documentViewer.Document.DocumentPaginator.IsPageCountValid)
            {
                OnPaginationCompleted(_documentViewer.Document, EventArgs.Empty);
            }

            return _documentViewer;
        }

        private void CheckExpectedVerticalPosition(double expectedOffset)
        {
            DRT.Assert(_documentViewer.VerticalOffset == expectedOffset,
                String.Format("Expected vertical offset = {0}, actual offset = {1}.", expectedOffset, _documentViewer.VerticalOffset));
        }

        private void CheckExpectedHorizontalPosition(double expectedOffset)
        {
            DRT.Assert(_documentViewer.HorizontalOffset == expectedOffset,
                String.Format("Expected horizontal offset = {0}, actual offset = {1}.", expectedOffset, _documentViewer.HorizontalOffset));
        }

        private void CheckExpectedZoom(double expectedZoom)
        {
            DRT.Assert(_documentViewer.Zoom == expectedZoom,
               String.Format("Expected zoom = {0}, actual zoom = {1}.", expectedZoom, _documentViewer.Zoom));
        }


        private void CheckExpectedMasterPageNumber(int expectedPageNumber)
        {
            DRT.Assert(_documentViewer.MasterPageNumber == expectedPageNumber,
                   String.Format("Expected PageNumber = {0}, actual PageNumber = {1}.", expectedPageNumber, _documentViewer.MasterPageNumber));

        }

        private void CheckElementFocus(UIElement element, bool expectedFocus)
        {
            bool actualFocus = element.IsFocused;
            DRT.Assert((actualFocus == expectedFocus),
                String.Format("Expected focus = {0}, actual focus = {1}.", expectedFocus, actualFocus));
        }

        private double GetPageHeight()
        {
            DRT.Assert((_documentViewer.PageViews[0] != null),
                String.Format("PageViews[0] is null for _documentViewer."));
            
            // Grab the height of the 1st Page.
            return  _documentViewer.PageViews[0].ActualHeight;
        }

        private UIElement GetNamedElement(string name)
        {
            UIElement element = DRT.FindElementByID(name, _documentViewer) as UIElement;
            DRT.Assert(element != null,
                String.Format("Expected to find named element, {0}, in the DocumentViewer tree.", name));

            return element;
        }

        /// <summary>
        /// Programmatically retreives the current selection, using Reflection.
        /// </summary>
        private string GetSelection()
        {
            string selection = string.Empty;


            PropertyInfo textSelectionInfo = typeof(DocumentViewer).GetProperty("TextSelection", BindingFlags.NonPublic | BindingFlags.Instance);
            DRT.Assert(textSelectionInfo != null,
                String.Format("Unable to retreive TextSelectionInfo for retreiving current Selection."));


            TextSelection textSelection = textSelectionInfo.GetValue(_documentViewer, null) as TextSelection;
            DRT.Assert(textSelection != null,
                String.Format("Unable to retreive textSelection for retreiving current Selection."));

            selection = textSelection.Text;

            return selection;
        }

        /// <summary>
        /// Helper. Invokes Click on a given ButtonBase. 
        /// </summary>
        private void DoClick(ButtonBase b)
        {
            MethodInfo info = typeof(ButtonBase).GetMethod("OnClick", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod);
            if (info == null) throw new Exception("Could not find ButtonBase.OnClick method");
            info.Invoke(b, new object[] { });
        }

        #endregion Implementation helpers

        private DocumentViewer _documentViewer;

        private State _state;

        // Storage values for verifying that FitToHeight is working.
        private double _beforeFitToHeight = 0.0;
        private double _afterFitToHeight = 0.0;

        // The offset of the last page used to test the LastPage command.
        private double _lastPageOffset = 63410.0;

        // Names of the key FindToolbar elements.
        private const string _findTextBoxName = "FindTextBox";
        private const string _findNextButtonName = "FindNextButton";

        // Const value used to ensure that FitToHeight will shrink the page height.
        private const double _prepForFitToHeightZoomValue = 400;

        // Amount of time (msec) to delay between tests so that a human can see what's happening.
        private int _delay = 0;

        // Default value for _delay (if command line option -delay is specified without an argument).
        private const int _defaultDelay = 500;

        // Amount of time (msec) to delay while waiting for pagination to complete.
        private const int _paginationDelay = 500;

        // pre-defined selection that occurs before the Find action.
        const string beforeFindSelection = "";

        enum State
        {
            Start = 0,
            PaginationCompleted
        }
    }
}

