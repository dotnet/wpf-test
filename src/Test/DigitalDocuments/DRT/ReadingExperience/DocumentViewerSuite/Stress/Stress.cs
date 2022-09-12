// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
//
// Description: Stresses DocumentViewer's operations, Properties, and Commands
//

using DRT;
using System;
using System.ComponentModel;
using System.IO;    //FileInfo, Path
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Markup;     //ParserContext, XamlReader
using System.Windows.Xps.Packaging;     //XpsDocument

using WinForms = System.Windows.Forms;  // OpenFileDialog

namespace DRTDocumentViewerSuite
{
         
    /// <summary>
    /// DocumentViewerStress Suite is responsible for running the Suite of DocumentViewer & Command tests.
    /// </summary>
    public sealed class StressSuite : DrtTestSuite
    {
        public StressSuite() : base("Stress")
        {           
            TeamContact = "WPF";
            Contact = "Microsoft";
        }

        /// <summary>
        /// Prepare the list of tests to run.  Called by base class.
        /// </summary>
        /// <returns></returns>
        public override DrtTest[] PrepareTests()
        {            

            Visual root = CreateTree();
            DRT.Show(root);

            switch( _state )
            {
                case State.Hold:
            
                    // return the lists of tests to run against the tree
                    return new DrtTest[]{ 
                                            new DrtTest( LoadUserContent ),
                                            new DrtTest( Hold )                        
                                        };

                default:
                    // return the lists of tests to run against the tree
                    return new DrtTest[]{     
                        
                                        /* 
                                         * Test that initial Offsets are honored prior to assigning content
                                         * (Related to 
*/
                                        new DrtTest( SetOffsets ),
                                        new DrtTest( SetFixedContent ),
                                        new DrtTest( WaitForPaginationCompleted ),
                                        new DrtTest( TestOffsets ),

                                        /*
                                         * Test that VerticalOffset is reset after re-assigning content
                                         * (
*/
                                        new DrtTest( SetFixedContent ),
                                        new DrtTest( WaitForPaginationCompleted ),
                                        new DrtTest( SetVaryingFixedContent ),
                                        new DrtTest( WaitForPaginationCompleted ),
                                        new DrtTest( TestVerticalOffsetIsZero ),
                                        new DrtTest( SetNullContent ),
                                        new DrtTest( WaitForPaginationCompleted ),
                                        new DrtTest( TestVerticalOffsetIsZero ),

                                        /*
                                         * Test that MasterPageNumber is reset after re-assigning content
*/
                                        new DrtTest( SetFixedContent ),
                                        new DrtTest( WaitForPaginationCompleted ),
                                        new DrtTest( EndCommand ),
                                        new DrtTest( SetVaryingFixedContent ),                                        
                                        new DrtTest( WaitForPaginationCompleted ),
                                        new DrtTest( TestMasterPageNumberIsOne ), 
                        
                                        /*
                                         * Test that all layout operations complete, not just the last one.
*/
                                        new DrtTest( SetZoom100Percent ),
                                        new DrtTest( SetBug1360178 ),
                                        new DrtTest( TestBug1360178 ),

                                        /*
                                         * Stress Scrolling, Column, and Zoom changes
                                         * for our content types.
                                         */

                                        //Fixed Content
                                        new DrtTest( SetFixedContent ),
                                        new DrtTest( WaitForPaginationCompleted ),
                                        new DrtTest( ViewThumbnailsCommand ),
                                        new DrtTest( FitHeightCommand ),
                                        new DrtTest( PageDownCommand ),
                                        new DrtTest( PageUpCommand ),
                                        new DrtTest( PageLeftCommand ),
                                        new DrtTest( PageRightCommand ),
                                        new DrtTest( NextPageCommand ),
                                        new DrtTest( PreviousPageCommand ),
                                        new DrtTest( ScrollToBottom ),
                                        new DrtTest( IncreasePagesAcross ),                                        
                                        new DrtTest( FitPagesAcrossCommand ),
                                        new DrtTest( IncreaseZoomCommand ),
                                        new DrtTest( ScrollToTop ),
                                        new DrtTest( IncreasePagesAcross ),
                                        new DrtTest( IncreasePagesAcross ),
                                        new DrtTest( FitPagesAcrossCommand ),
                                        new DrtTest( DecreaseZoomCommand ),
                                        new DrtTest( ScrollToBottom ),                                        
                                        new DrtTest( SetZoom100Percent ),
                                        new DrtTest( SetPagesAcross15 ),
                                        new DrtTest( FitPagesAcrossCommand ),
                                        new DrtTest( TestSpacingIncrease ),   
                                        new DrtTest( TogglePageBorders ),
                                        new DrtTest( TestSpacingDecrease ),                 
                                        new DrtTest( TogglePageBorders ),
                                        new DrtTest( ResetSpacing ),                 
                                        new DrtTest( HomeCommand ),
                                        new DrtTest( FitWidthCommand ),
                                        new DrtTest( EndCommand ),                                        

                                        //Test Fixed w/varying page sizes
                                        new DrtTest( SetPagesAcross1 ),
                                        new DrtTest( SetVaryingFixedContent ),
                                        new DrtTest( WaitForPaginationCompleted ),
                                        new DrtTest( ViewThumbnailsCommand ),
                                        new DrtTest( FitHeightCommand ),
                                        new DrtTest( PageDownCommand ),
                                        new DrtTest( PageUpCommand ),
                                        new DrtTest( PageLeftCommand ),
                                        new DrtTest( PageRightCommand ),
                                        new DrtTest( NextPageCommand ),
                                        new DrtTest( PreviousPageCommand ),
                                        new DrtTest( SetZoom100Percent ),
                                        new DrtTest( ScrollToBottom ),
                                        new DrtTest( IncreasePagesAcross ),
                                        new DrtTest( FitPagesAcrossCommand ),
                                        new DrtTest( IncreaseZoomCommand ),
                                        new DrtTest( ScrollToTop ),
                                        new DrtTest( IncreasePagesAcross ),
                                        new DrtTest( IncreasePagesAcross ),
                                        new DrtTest( FitPagesAcrossCommand ),
                                        new DrtTest( DecreaseZoomCommand ),
                                        new DrtTest( ScrollToBottom ),                                        
                                        new DrtTest( SetZoom100Percent ),
                                        new DrtTest( SetPagesAcross15 ),
                                        new DrtTest( FitPagesAcrossCommand ),   
                                        new DrtTest( TestSpacingIncrease ), 
                                        new DrtTest( TogglePageBorders ),
                                        new DrtTest( TestSpacingDecrease ),                 
                                        new DrtTest( TogglePageBorders ),
                                        new DrtTest( ResetSpacing ),                 
                                        new DrtTest( HomeCommand ),
                                        new DrtTest( FitWidthCommand ),
                                        new DrtTest( EndCommand ),

                                        /*
                                         * Stress changing between fixed and flow after column changes, etc.
                                         */

                                        //Load some fixed content
                                        new DrtTest( SetPagesAcross1 ),
                                        new DrtTest( SetFixedContent ),
                                        new DrtTest( WaitForPaginationCompleted ),
                                        new DrtTest( ViewThumbnailsCommand ),
                                        new DrtTest( EndCommand ),
                                        
                                        //Switch to new fixed content
                                        new DrtTest( SetVaryingFixedContent ),
                                        new DrtTest( WaitForPaginationCompleted ),
                                        new DrtTest( HomeCommand ),
                                        new DrtTest( SetPagesAcross1 ),
                                        new DrtTest( EndCommand ),
                                        
                                        //Back to original fixed
                                        new DrtTest( SetFixedContent ),
                                        new DrtTest( WaitForPaginationCompleted ),
                                        
                                        //To new content
                                        new DrtTest( SetVaryingFixedContent ),
                                        new DrtTest( WaitForPaginationCompleted ),
                                                                                
                                    };            
            }
        }

        /// <summary>
        /// Parse any command-line arguments here.  Called by base class.
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="option"></param>
        /// <param name="args"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public override bool HandleCommandLineArgument(string arg, bool option, string[] args, ref int k)
        {
            //If the base can handle this, let it.
            if (base.HandleCommandLineArgument(arg, option, args, ref k))
                return true;

            bool handled = false;

            _state = State.Start;

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

                    case "test":
                        _state = State.Hold;
                        if (k != args.Length - 1)
                        {
                            _userContentFile = args[k+1];
                            ++k;
                        }                       
                        handled = true;
                        break;
                }
            }

            return handled;
        }

        #region Individual tests

        /// <summary>
        /// Run this test forever (allows user-interaction)
        /// </summary> 
        private void Hold()
        {
            DRT.Pause(_paginationDelay);
            DRT.ResumeAt(Hold);
        }

        /// <summary>
        /// Loads the optionally specified file into DocumentViewer.
        /// This can be either an Xps Document or a Xaml file.
        /// </summary>
        private void LoadUserContent()
        {
            if (!string.IsNullOrEmpty(_userContentFile))
            {
                //Determine if the extension ends in ".xaml".
                //If it does, treat this as a non-Xps document.
                string extension = System.IO.Path.GetExtension(_userContentFile);
                bool isXps = !(extension.Equals(
                                    _xamlExtension, 
                                    StringComparison.OrdinalIgnoreCase));

                Console.WriteLine("_userContentFile: " + _userContentFile);
                Console.WriteLine("_xamlExtension: " + _xamlExtension);
                Console.WriteLine("extension: " + extension);

                AssignContent(_userContentFile, isXps);
            }
        }

        /// <summary>
        /// Run this test until pagination completes
        /// </summary>        
        private void WaitForPaginationCompleted()
        {
            Console.WriteLine("Waiting for pagination to complete...");
            if (!_paginationCompleted)
            {
                DRT.Pause(_paginationDelay);
                DRT.ResumeAt(WaitForPaginationCompleted);
            }
        }

        /// <summary>
        /// Sets the Vertical and Horizontal offsets to predefined values.
        /// </summary>
        private void SetOffsets()
        {
            Console.WriteLine("Setting offsets to {0},{1}", _testVerticalOffset, _testHorizontalOffset);
            _documentViewer.VerticalOffset = _testVerticalOffset;
            _documentViewer.HorizontalOffset = _testHorizontalOffset;
        }

        /// <summary>
        /// Tests that  the Vertical and Horizontal offsets were set to predefined values.
        /// </summary>
        private void TestOffsets()
        {
            Console.WriteLine("Testing that the offsets are {0},{1}", _testVerticalOffset, _testHorizontalOffset);
            DRT.Assert(_documentViewer.VerticalOffset == _testVerticalOffset,
               String.Format("VerticalOffset was {0}, expected {1}", _documentViewer.VerticalOffset,
               _testVerticalOffset));

            DRT.Assert(_documentViewer.HorizontalOffset == _testHorizontalOffset,
               String.Format("HorizontalOffset was {0}, expected {1}", _documentViewer.HorizontalOffset,
               _testHorizontalOffset));            
        }

        /// <summary>
        /// Test that VerticalOffset == 0.
        /// </summary>
        private void TestVerticalOffsetIsZero()
        {
            Console.WriteLine("Testing that VerticalOffset=0.  VerticalOffset={0}", _documentViewer.VerticalOffset);
            DRT.Assert(_documentViewer.VerticalOffset == 0.0, 
                "VerticalOffset was not reset after content change.");
        }

        /// <summary>
        /// Test that MasterPageNumber == 1.
        /// </summary>
        private void TestMasterPageNumberIsOne()
        {
            Console.WriteLine("Testing that MasterPageNumberNumber==1.  MasterPageNumberNumber={0}", _documentViewer.MasterPageNumber);
            DRT.Assert(_documentViewer.MasterPageNumber == 1,
                "MasterPageNumber was not reset after content change.");
        }

        /// <summary>
        /// Does two different layout actions in immediate succession
        /// </summary>
        private void SetBug1360178()
        {
            Console.WriteLine("Fitting to 2 pages across and setting MaxPagesAcross to 4.");
            DocumentViewer.FitToMaxPagesAcrossCommand.Execute(2, _documentViewer);
            _documentViewer.MaxPagesAcross = 4;
        }

        /// <summary>
        /// Ensures that the actions done above have executed properly.
        /// </summary>
        private void TestBug1360178()
        {
            //If the first action in SetBug1360178 was ignored, then the Zoom will still be 100%
            //afterwards; we assert that Zoom != 100% here.
            Console.WriteLine("Testing that Zoom is 100%");
            DRT.Assert(_documentViewer.Zoom != 100.0, "FitToMaxPagesAcross command was ignored.");
        }

        /// <summary>
        /// Scroll to the bottom of the document, 100 pixels at a time.
        /// </summary>
        private void ScrollToBottom()
        {
            Console.WriteLine("Scrolling down.  VerticalOffset={0}", _documentViewer.VerticalOffset);
            if (_documentViewer.VerticalOffset < _documentViewer.ExtentHeight - _documentViewer.ViewportHeight)
            {
                _documentViewer.VerticalOffset += 500;
                DRT.ResumeAt(ScrollToBottom);
            }
        }

        /// <summary>
        /// Scroll to the top of the document, 100 pixels at a time.
        /// </summary>
        private void ScrollToTop()
        {
            Console.WriteLine("Scrolling up.  VerticalOffset={0}", _documentViewer.VerticalOffset);
            if (_documentViewer.VerticalOffset > 0)
            {                
                _documentViewer.VerticalOffset = Math.Max( _documentViewer.VerticalOffset-500, 0.0);
                DRT.ResumeAt(ScrollToTop);
            }
        }

        /// <summary>
        /// Increase the PageSize, 100px in both dimensions at a time.
        /// </summary>
        private void IncreasePageSize()
        {
            Size pageSize = _documentViewer.Document.DocumentPaginator.PageSize;
            Console.WriteLine("Increasing PageSize.  Size={0},{1}", pageSize.Width, pageSize.Height);
            
            pageSize.Width += 100;
            pageSize.Height += 100;
            _documentViewer.Document.DocumentPaginator.PageSize = pageSize;                
        
        }

        /// <summary>
        /// Decrease the PageSize, 100px in both dimensions, down to 100px minimum.
        /// </summary>
        private void DecreasePageSize()
        {
            Size pageSize = _documentViewer.Document.DocumentPaginator.PageSize;
            Console.WriteLine("Decreasing PageSize.  Size={0},{1}", pageSize.Width, pageSize.Height);
            if (pageSize.Width > 100 && pageSize.Height > 100)
            {
                pageSize.Width -= 100;
                pageSize.Height -= 100;
                _documentViewer.Document.DocumentPaginator.PageSize = pageSize;                
            }
        }

        /// <summary>
        /// Resets the page size to 8.5x11.
        /// </summary>
        private void ResetPageSize()
        {
            Console.WriteLine("Resetting PageSize.  Size=816,1056");
            _documentViewer.Document.DocumentPaginator.PageSize = new Size(816, 1056);
        }

        /// <summary>
        /// Increase the page spacing, 5 units at a time.
        /// </summary>
        private void TestSpacingIncrease()
        {
            Console.WriteLine("Increasing page spacing.  Vertical/HorizontalPageSpacing={0}", _documentViewer.VerticalPageSpacing);
            if (_documentViewer.VerticalPageSpacing < 50)
            {
                _documentViewer.VerticalPageSpacing += 5;
                _documentViewer.HorizontalPageSpacing += 5;

                DRT.ResumeAt(TestSpacingIncrease);
            }
        }

        /// <summary>
        /// Decrease the page spacing, 5 units at a time.
        /// </summary>
        private void TestSpacingDecrease()
        {
            Console.WriteLine("Decreasing page spacing.  Vertical/HorizontalPageSpacing={0}", _documentViewer.VerticalPageSpacing);
            if (_documentViewer.VerticalPageSpacing > 0)
            {
                _documentViewer.VerticalPageSpacing -= 5;
                _documentViewer.HorizontalPageSpacing -= 5;

                DRT.ResumeAt(TestSpacingIncrease);
            }
        }

        /// <summary>
        /// Reset the page spacing to defaults.
        /// </summary>
        private void ResetSpacing()
        {
            Console.WriteLine("Resetting page spacing to 10,10");

            _documentViewer.VerticalPageSpacing = 10;
            _documentViewer.HorizontalPageSpacing = 10;
          
            DRT.Pause(_delay);
        }

        /// <summary>
        /// Increase the number of columns in the layout by 1.
        /// </summary>
        private void IncreasePagesAcross()
        {            
            _documentViewer.MaxPagesAcross++;

            Console.WriteLine("Increased column count to {0}", _documentViewer.MaxPagesAcross);

            DRT.Pause(_delay);
        }

        /// <summary>
        /// Decrease the number of columns in the layout by 1.
        /// </summary>
        private void DecreasePagesAcross()
        {
            _documentViewer.MaxPagesAcross = Math.Max(1, _documentViewer.MaxPagesAcross - 1);

            Console.WriteLine("Decreased column count to {0}", _documentViewer.MaxPagesAcross);

            DRT.Pause(_delay);
        }

        /// <summary>
        /// Set the Zoom to 100%
        /// </summary>
        private void SetZoom100Percent()
        {
            _documentViewer.Zoom = 100.0;

            Console.WriteLine("Set Zoom to {0}", _documentViewer.Zoom);

            DRT.Pause(_delay);
        }

        /// <summary>
        /// Show 1 column.
        /// </summary>
        private void SetPagesAcross1()
        {
            _documentViewer.MaxPagesAcross = 1;

            Console.WriteLine("Set column count to {0}", _documentViewer.MaxPagesAcross);

            DRT.Pause(_delay);
        }

        /// <summary>
        /// Show 5 columns.
        /// </summary>
        private void SetPagesAcross5()
        {
            _documentViewer.MaxPagesAcross = 5;

            Console.WriteLine("Set column count to {0}", _documentViewer.MaxPagesAcross);

            DRT.Pause(_delay);
        }

        /// <summary>
        /// Show 15 columns.
        /// </summary>
        private void SetPagesAcross15()
        {
            _documentViewer.MaxPagesAcross = 15;

            Console.WriteLine("Set column count to {0}", _documentViewer.MaxPagesAcross);

            DRT.Pause(_delay);
        }

        /// <summary>
        /// Fit the currently requested number of columns into the view.
        /// </summary>
        private void FitPagesAcrossCommand()
        {
            DocumentViewer.FitToMaxPagesAcrossCommand.Execute(_documentViewer.MaxPagesAcross, _documentViewer);
 
            Console.WriteLine("Fit column count to {0}", _documentViewer.MaxPagesAcross);

            DRT.Pause(_delay);
        }

        /// <summary>
        /// Fit a single page.
        /// </summary>
        private void FitPageCommand()
        {
            DocumentViewer.FitToMaxPagesAcrossCommand.Execute(1, _documentViewer);

            Console.WriteLine("Fit to single page");

            DRT.Pause(_delay);
        }

        /// <summary>
        /// Fit to the width of the page.
        /// </summary>
        private void FitWidthCommand()
        {
            DocumentViewer.FitToWidthCommand.Execute(null, _documentViewer);

            Console.WriteLine("Fit to single width");

            DRT.Pause(_delay);
        }

        /// <summary>
        /// Fit to the height of the page.
        /// </summary>
        private void FitHeightCommand()
        {
            DocumentViewer.FitToHeightCommand.Execute(null, _documentViewer);

            Console.WriteLine("Fit to height");

            DRT.Pause(_delay);
        }

        /// <summary>
        /// Display the thumbnail view.
        /// </summary>
        private void ViewThumbnailsCommand()
        {
            DocumentViewer.ViewThumbnailsCommand.Execute(null, _documentViewer);

            Console.WriteLine("Viewing thumbnails");

            DRT.Pause(_delay);
        }

        /// <summary>
        /// Switch on/off the borders around pages.
        /// </summary>
        private void TogglePageBorders()
        {
            _documentViewer.ShowPageBorders = !_documentViewer.ShowPageBorders;

            Console.WriteLine("Toggling page borders");

            DRT.Pause(_delay);
        }

        /// <summary>
        /// Page left one viewport.
        /// </summary>
        private void PageLeftCommand()
        {
            ComponentCommands.MoveLeft.Execute(null, _documentViewer);

            Console.WriteLine("Paging left");

            DRT.Pause(_delay);
        }

        /// <summary>
        /// Page right one viewport.
        /// </summary>
        private void PageRightCommand()
        {
            ComponentCommands.MoveRight.Execute(null, _documentViewer);

            Console.WriteLine("Paging right");

            DRT.Pause(_delay);
        }

        /// <summary>
        /// Page up one viewport.
        /// </summary>
        private void PageUpCommand()
        {
            ComponentCommands.ScrollPageUp.Execute(null, _documentViewer);

            Console.WriteLine("Paging up");

            DRT.Pause(_delay);
        }

        /// <summary>
        /// Page down one viewport.
        /// </summary>
        private void PageDownCommand()
        {
            ComponentCommands.ScrollPageDown.Execute(null, _documentViewer);

            Console.WriteLine("Paging down");

            DRT.Pause(_delay);
        }

        /// <summary>
        /// Go to the previous page.
        /// </summary>
        private void PreviousPageCommand()
        {
            _documentViewer.PreviousPage();

            Console.WriteLine("Going to previous page");

            DRT.Pause(_delay);
        }

        /// <summary>
        /// Go to the next page.
        /// </summary>
        private void NextPageCommand()
        {
            _documentViewer.NextPage();

            Console.WriteLine("Going to next page");

            DRT.Pause(_delay);
        }

        /// <summary>
        /// Go to the top of the document.
        /// </summary>
        private void HomeCommand()
        {
            _documentViewer.FirstPage();

            Console.WriteLine("Moving to Home");

            DRT.Pause(_delay);
        }

        /// <summary>
        /// Go to the bottom of the document.
        /// </summary>
        private void EndCommand()
        {
            _documentViewer.LastPage();

            Console.WriteLine("Moving to End");

            DRT.Pause(_delay);
        }

        /// <summary>
        /// Load Fixed content into DocumentViewer.
        /// </summary>
        private void SetFixedContent()
        {
            AssignContent("drtfiles\\documentviewersuite\\stress\\fixed.xaml");

            Console.WriteLine("Loading Fixed content");

            DRT.Pause(_delay);
        }

        /// <summary>
        /// Load Fixed content with varying page sizes into DocumentViewer.
        /// </summary>
        private void SetVaryingFixedContent()
        {
            AssignContent("drtfiles\\documentviewersuite\\stress\\vfixed.xaml");

            Console.WriteLine("Loading Fixed content w/varying page sizes");

            DRT.Pause(_delay);
        }

        /// <summary>
        /// Load null content into DocumentViewer.
        /// </summary>
        private void SetNullContent()
        {
            _documentViewer.Document = null;

            Console.WriteLine("Nulling content");

            DRT.Pause(_delay);
        }


        /// <summary>
        /// Load a user-specified XPS Document or 
        /// Xaml file into DocumentViewer.
        /// </summary>
        /// <param name="isXps">Is Content an XpsDocument</param>
        private void SetUserContent(bool isXps)
        {
            //Show Open File Dialog to let user choose
            string filename = GetFileToOpen(isXps);

            if (!string.IsNullOrEmpty(filename))
            {
                // Now load the context.
                AssignContent(filename, isXps);
            }
        }

        /// <summary>
        /// Shows the OpenFileDialog to let the user select
        /// either an Xps document or a Xaml File depending
        /// on the isXps parameter.
        /// </summary>
        /// <param name="isXps">Is File an XpsDocument</param>
        /// <returns>full path of the file</returns>
        private string GetFileToOpen(bool isXps)
        {
            WinForms.OpenFileDialog dialog;
            dialog = new WinForms.OpenFileDialog();

            if (isXps)
            {
                //Add extension support for .xps files.
                dialog.Filter = "Xps Documents (*.xps)|*.xps";
            }
            else
            {
                //Add extension support for only .xaml files.
                dialog.Filter = "Xaml Files (*.xaml)|*.xaml";
            }

            if (dialog.ShowDialog(null) == WinForms.DialogResult.OK)
            {
                return dialog.FileName;
            }
            else
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// Decrease Zoom one notch.
        /// </summary>
        private void DecreaseZoomCommand()
        {
            NavigationCommands.DecreaseZoom.Execute(null, _documentViewer);            

            Console.WriteLine("Decreasing zoom");

            DRT.Pause(_delay);
        }

        /// <summary>
        /// Increase Zoom one notch.
        /// </summary>
        private void IncreaseZoomCommand()
        {
            NavigationCommands.IncreaseZoom.Execute(null, _documentViewer);            

            Console.WriteLine("Increasing zoom");

            DRT.Pause(_delay);
        }

        /// <summary>
        /// Go to the specified page in the document.
        /// </summary>
        /// <param name="pageNumber"></param>
        private void GotoPage(int pageNumber)
        {
            _documentViewer.GoToPage(pageNumber);
        }

        #endregion Individual tests

        #region Event handlers

        /// <summary>
        /// When we get a PaginationProgress event, it means that pagination is not completed, so we
        /// reset the flag.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPaginationProgress(Object sender, PaginationProgressEventArgs e)
        {
            _paginationCompleted = false;            
        }

        /// <summary>
        /// When we get a PaginationCompleted event, pagination is done so we set the flag.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPaginationCompleted(Object sender, EventArgs e)
        {            
            _paginationCompleted = true;            
        }

        

        #endregion Event handlers

        #region Implementation helpers
        
        /// <summary>
        /// Load Xaml content from file.
        /// </summary>
        /// <param name="filename">Relative filename to load.</param>
        private IDocumentPaginatorSource LoadXaml(string filename)
        {
            //If we had a previously-opened file, close it now.
            if (_fileStream != null)
            {
                _fileStream.Close();
                _fileStream = null;
            }

            object newXaml = null;

            // Open and parse the XAML file.
            if (File.Exists(filename))
            {
                //Need to grab the fileInfo, so that we can sucessfully build a 
                //Uri for the ParserContext.
                FileInfo fileInfo = new FileInfo(filename);
                _fileStream = fileInfo.OpenRead();
                Uri contentUri = new Uri(fileInfo.DirectoryName + "\\", UriKind.Absolute);

                try
                {
                    // Let the parser know where the related content files and resource files are.
                    System.Windows.Markup.ParserContext parserContext = 
                                new System.Windows.Markup.ParserContext();
                    parserContext.BaseUri = contentUri;

                    newXaml = XamlReader.Load(_fileStream, parserContext);
                    // Leave fileStream open due to async loading.
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.Assert(false, ex.ToString());
                }
            }

            if (newXaml is IDocumentPaginatorSource)
            {
                return (IDocumentPaginatorSource)newXaml;
            }
            else
            {
                System.Diagnostics.Debug.Assert(false, "DocumentViewer can only accept children of type IDocumentPaginatorSource.");
                return null;
            }
        }
        
        /// <summary>
        /// Load an XPS Document content from file.
        /// </summary>
        /// <param name="filename">Relative filename to load.</param>
        private IDocumentPaginatorSource LoadXps(string filename)
        {            
            // Open the Xps file.
            XpsDocument xpsDoc = new XpsDocument(filename, FileAccess.Read);

            //retreive the Document
            IDocumentPaginatorSource document = xpsDoc.GetFixedDocumentSequence();

            //close the document
            xpsDoc.Close();

            return document;
        }
        
        /// <summary>
        /// Loads a XAML file, assigns it to DocumentViewer and attaches
        /// the appropriate event handlers.
        /// </summary>
        /// <param name="filename"></param>
        private void AssignContent(string filename)
        {
            AssignContent(filename, false /* isXps */);
        }

        /// <summary>
        /// Loads a XAML file or Xps Document, assigns it to DocumentViewer
        /// and attaches the appropriate event handlers.
        /// </summary>
        /// <param name="filename"></param>
        private void AssignContent(string filename, bool isXps )
        {

            IDocumentPaginatorSource content = null;
            DynamicDocumentPaginator paginator = null;

            if (isXps)
            {
                content = LoadXps(filename);
            }
            else
            {
                content = LoadXaml(filename);
            }

            if (content != null)
            {
                _paginationCompleted = false;

                if (_documentViewer.Document != null)
                {
                    paginator = (DynamicDocumentPaginator)_documentViewer.Document.DocumentPaginator;
                    paginator.PaginationCompleted -= new EventHandler(OnPaginationCompleted);
                    paginator.PaginationProgress -= new PaginationProgressEventHandler(OnPaginationProgress);
                }

                paginator = (DynamicDocumentPaginator)content.DocumentPaginator;
                paginator.PaginationCompleted += new EventHandler(OnPaginationCompleted);
                paginator.PaginationProgress += new PaginationProgressEventHandler(OnPaginationProgress);
                _documentViewer.Document = content;

                if (paginator.IsPageCountValid)
                {
                    OnPaginationCompleted(_documentViewer.Document, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Creates the tree for this DRT test.
        /// </summary>
        /// <returns></returns>
        private Visual CreateTree()
        {

            //Border around everything (gray background)
            Border border = new Border();
            border.Background = Brushes.DarkGray;

            //Our main grid
            Grid grid = new Grid();
            border.Child = grid;

            //Column 1 contains the toolbars & DocumentViewer
            ColumnDefinition col1 = new ColumnDefinition();
            col1.Width = new GridLength(1, GridUnitType.Star);

            //Column 2 contains our "fake" scrollbar along the side
            ColumnDefinition col2 = new ColumnDefinition();
            col2.Width = new GridLength(1, GridUnitType.Auto);

            //Row 1 - Toolbar 1
            RowDefinition row1 = new RowDefinition();
            row1.Height = new GridLength(1, GridUnitType.Auto);
            //Row 2 - Toolbar 2
            RowDefinition row2 = new RowDefinition();
            row2.Height = new GridLength(1, GridUnitType.Auto);
            //Row 3 - Toolbar 3
            RowDefinition row3 = new RowDefinition();
            row3.Height = new GridLength(1, GridUnitType.Auto);
            //Row 4 - DocumentViewer
            RowDefinition row4 = new RowDefinition();
            row4.Height = new GridLength(1, GridUnitType.Star);
            //Row 5 - "fake" horizontal scrollbar
            RowDefinition row5 = new RowDefinition();
            row5.Height = new GridLength(1, GridUnitType.Auto);

            grid.ColumnDefinitions.Add(col1);
            grid.ColumnDefinitions.Add(col2);            
            grid.RowDefinitions.Add(row1);
            grid.RowDefinitions.Add(row2);
            grid.RowDefinitions.Add(row3);
            grid.RowDefinitions.Add(row4);
            grid.RowDefinitions.Add(row5);

            /*
             * Top: Menu
             */

            Menu mainMenu = new Menu();                        
            grid.Children.Add(mainMenu);
            Grid.SetRow(mainMenu, 0);
            Grid.SetColumn(mainMenu, 0);        
   
            //Build the top-level menu
            MenuItem documentMenu = new MenuItem();
            documentMenu.Header = "Document";
            mainMenu.Items.Add(documentMenu);

            MenuItem viewMenu = new MenuItem();
            viewMenu.Header = "View";
            mainMenu.Items.Add(viewMenu);

            MenuItem actionMenu = new MenuItem();
            actionMenu.Header = "Action";
            mainMenu.Items.Add(actionMenu);

            //Add the menu items    
            
            // **Document Menu**
            //Fixed Content menu
            _fixedContentMenu = new MenuItem();
            documentMenu.Items.Add(_fixedContentMenu);                        
            _fixedContentMenu.Header = "Simple Fixed Content";
            _fixedContentMenu.Click += new RoutedEventHandler(OnFixedContentMenuClick);

            //Fixed Content w/Varying page sizes menu
            _varyingFixedContentMenu = new MenuItem();
            documentMenu.Items.Add(_varyingFixedContentMenu);                        
            _varyingFixedContentMenu.Header = "Varying Page Size Fixed Content";
            _varyingFixedContentMenu.Click += new RoutedEventHandler(OnVaryingFixedContentMenuClick);

            //Null Content button
            _nullContentMenu = new MenuItem();
            documentMenu.Items.Add(_nullContentMenu);                                  
            _nullContentMenu.Header = "Null Content";
            _nullContentMenu.Click += new RoutedEventHandler(OnNullContentMenuClick);

            //Xps Document Content button
            _xpsContentMenu = new MenuItem();
            documentMenu.Items.Add(_xpsContentMenu);
            _xpsContentMenu.Header = "Xps Document Content";
            _xpsContentMenu.Click += new RoutedEventHandler(OnXpsContentMenuClick);

            //Fixed Xaml Content button
            _xamlContentMenu = new MenuItem();
            documentMenu.Items.Add(_xamlContentMenu);
            _xamlContentMenu.Header = "Fixed Xaml Content";
            _xamlContentMenu.Click += new RoutedEventHandler(OnXamlContentMenuClick);


            //** View Menu **
            //Fit PagesAcross menu
            _fitColumnsMenu = new MenuItem();
            viewMenu.Items.Add(_fitColumnsMenu);                        
            _fitColumnsMenu.Header = "Fit Columns";
            _fitColumnsMenu.Click += new RoutedEventHandler(OnFitPagesAcrossMenuClick);

            //Fit Page button
            _fitPageMenu = new MenuItem();
            viewMenu.Items.Add(_fitPageMenu);                        
            _fitPageMenu.Header = "Fit Page";
            _fitPageMenu.Click += new RoutedEventHandler(OnFitPageMenuClick);

            //Fit Width button
            _fitWidthMenu = new MenuItem();
            viewMenu.Items.Add(_fitWidthMenu);                        
            _fitWidthMenu.Header = "Fit Width";
            _fitWidthMenu.Click += new RoutedEventHandler(OnFitWidthMenuClick);

            //Fit Height button
            _fitHeightMenu = new MenuItem();
            viewMenu.Items.Add(_fitHeightMenu);                        
            _fitHeightMenu.Header = "Fit Height";
            _fitHeightMenu.Click += new RoutedEventHandler(OnFitHeightMenuClick);

            //View Thumbnails button
            _thumbnailsMenu = new MenuItem();
            viewMenu.Items.Add(_thumbnailsMenu);                        
            _thumbnailsMenu.Header = "View Thumbnails";
            _thumbnailsMenu.Click += new RoutedEventHandler(OnThumbnailsMenuClick);

            //Toggle Page Borders button
            _showBordersMenu = new MenuItem();
            viewMenu.Items.Add(_showBordersMenu);            
            _showBordersMenu.Header = "Show Borders";
            _showBordersMenu.IsChecked = true;
            _showBordersMenu.Click += new RoutedEventHandler(OnShowBordersMenuClick);

            //Decrease Zoom button
            _decreaseZoomMenu = new MenuItem();
            viewMenu.Items.Add(_decreaseZoomMenu);                        
            _decreaseZoomMenu.Header = "Zoom Out";
            _decreaseZoomMenu.Click += new RoutedEventHandler(OnDecreaseZoomMenuClick);

            //Increase Zoom button
            _increaseZoomMenu = new MenuItem();
            viewMenu.Items.Add(_increaseZoomMenu);                        
            _increaseZoomMenu.Header = "Zoom In";
            _increaseZoomMenu.Click += new RoutedEventHandler(OnIncreaseZoomMenuClick);

            //Increase PageSize button
            _increasePageSizeMenu = new MenuItem();
            actionMenu.Items.Add(_increasePageSizeMenu);
            _increasePageSizeMenu.Header = "Increase PageSize";
            _increasePageSizeMenu.Click += new RoutedEventHandler(OnIncreasePageSizeMenuClick);

            //Decrease PageSize button
            _decreasePageSizeMenu = new MenuItem();
            actionMenu.Items.Add(_decreasePageSizeMenu);
            _decreasePageSizeMenu.Header = "Decrease PageSize";
            _decreasePageSizeMenu.Click += new RoutedEventHandler(OnDecreasePageSizeMenuClick);
             
            /*
             * Top: menu end
             */

            /*
             * Middle: toolbar start
             */

            DockPanel toolBarMiddle = new DockPanel();
            toolBarMiddle.Height = 24.0;
            grid.Children.Add(toolBarMiddle);
            Grid.SetRow(toolBarMiddle, 1);
            Grid.SetColumn(toolBarMiddle, 0);
                      
            //H. Spacing Slider & label
            Label horizontalSpacingSliderLabel = new Label();
            horizontalSpacingSliderLabel.Content = "HorizontalPageSpacing:";
            toolBarMiddle.Children.Add(horizontalSpacingSliderLabel);

            _horizontalSpacingSlider = new Slider();
            toolBarMiddle.Children.Add(_horizontalSpacingSlider);
            DockPanel.SetDock(_horizontalSpacingSlider, Dock.Left);
            _horizontalSpacingSlider.Width = 70;
            _horizontalSpacingSlider.Height = 16;
            _horizontalSpacingSlider.Minimum = 0.0;
            _horizontalSpacingSlider.Maximum = 100.0;
            _horizontalSpacingSlider.AutoToolTipPlacement = AutoToolTipPlacement.BottomRight;

            //V. Spacing Slider & label
            Label verticalSpacingSliderLabel = new Label();
            verticalSpacingSliderLabel.Content = "VerticalPageSpacing:";
            toolBarMiddle.Children.Add(verticalSpacingSliderLabel);

            _verticalSpacingSlider = new Slider();
            toolBarMiddle.Children.Add(_verticalSpacingSlider);
            DockPanel.SetDock(_verticalSpacingSlider, Dock.Left);
            _verticalSpacingSlider.Width = 70;
            _verticalSpacingSlider.Height = 16;
            _verticalSpacingSlider.Minimum = 0.0;
            _verticalSpacingSlider.Maximum = 100.0;
            _verticalSpacingSlider.AutoToolTipPlacement = AutoToolTipPlacement.BottomRight;       

            //Zoom Slider & label
            Label zoomSliderLabel = new Label();
            zoomSliderLabel.Content = "ZoomPercentage:";
            toolBarMiddle.Children.Add(zoomSliderLabel);

            _zoomSlider = new Slider();
            toolBarMiddle.Children.Add(_zoomSlider);
            DockPanel.SetDock(_zoomSlider, Dock.Left);
            _zoomSlider.Width = 70;
            _zoomSlider.Height = 16;
            _zoomSlider.Minimum = 5.0;
            _zoomSlider.Maximum = 1000.0;
            _zoomSlider.AutoToolTipPlacement = AutoToolTipPlacement.BottomRight;

            //Column Slider & label
            Label columnSliderLabel = new Label();
            columnSliderLabel.Content = "GridColumnCount:";
            toolBarMiddle.Children.Add(columnSliderLabel);

            _columnSlider = new Slider();
            toolBarMiddle.Children.Add(_columnSlider);
            DockPanel.SetDock(_columnSlider, Dock.Left);
            _columnSlider.Width = 70;
            _columnSlider.Height = 16;
            _columnSlider.Minimum = 1.0;
            _columnSlider.Maximum = 25.0;
            _columnSlider.AutoToolTipPlacement = AutoToolTipPlacement.BottomRight;

            /*
             * Bottom toolbar start
             */

            DockPanel toolBarBottom = new DockPanel();
            toolBarBottom.Height = 24.0;
            grid.Children.Add(toolBarBottom);
            Grid.SetRow(toolBarBottom, 2);
            Grid.SetColumn(toolBarBottom, 0);

            //MasterPageNumber Text & label
            Label firstPageLabel = new Label();
            firstPageLabel.Content = "MasterPageNumber:";
            toolBarBottom.Children.Add(firstPageLabel);

            _firstPageTextBox = new TextBox();
            _firstPageTextBox.Width = 32.0;            
            toolBarBottom.Children.Add(_firstPageTextBox);
            DockPanel.SetDock(_firstPageTextBox, Dock.Left);

            _gotoPageButton = new Button();
            _gotoPageButton.Width = 32.0;            
            _gotoPageButton.Content = "Go";
            toolBarBottom.Children.Add(_gotoPageButton);
            _gotoPageButton.Click += new RoutedEventHandler(OnGotoPageButtonClick);

            //Zoom Text & label
            Label zoomBoxLabel = new Label();
            zoomBoxLabel.Content = "Zoom:";
            toolBarBottom.Children.Add(zoomBoxLabel);

            _zoomTextBox = new TextBox();
            _zoomTextBox.Width = 32.0;
            toolBarBottom.Children.Add(_zoomTextBox);
            DockPanel.SetDock(_zoomTextBox, Dock.Left);

            _setZoomButton = new Button();
            _setZoomButton.Width = 32.0;
            _setZoomButton.Content = "Set";
            toolBarBottom.Children.Add(_setZoomButton);
            _setZoomButton.Click += new RoutedEventHandler(OnSetZoomButtonClick);

            //CanMoveX Text & label
            Label canMoveLabel = new Label();
            canMoveLabel.Content = "Can Move U D L R:";
            toolBarBottom.Children.Add(canMoveLabel);

            _canMoveUpLabel = new Label();
            toolBarBottom.Children.Add(_canMoveUpLabel);
            DockPanel.SetDock(_canMoveUpLabel, Dock.Left);

            _canMoveDownLabel = new Label();
            toolBarBottom.Children.Add(_canMoveDownLabel);
            DockPanel.SetDock(_canMoveDownLabel, Dock.Left);

            _canMoveLeftLabel = new Label();
            toolBarBottom.Children.Add(_canMoveLeftLabel);
            DockPanel.SetDock(_canMoveLeftLabel, Dock.Left);

            _canMoveRightLabel = new Label();
            toolBarBottom.Children.Add(_canMoveRightLabel);
            DockPanel.SetDock(_canMoveRightLabel, Dock.Left);            

            

            /*
             * Bottom toolbar end
             */

            //The document viewer we're stressing, inside of a Frame          
            _documentViewer = new DocumentViewer();

            Frame frame = new Frame();
            frame.Content = _documentViewer;
            Grid.SetRow(frame, 3);
            Grid.SetColumn(frame, 0);
            grid.Children.Add(frame);
          
            /*
             *  Our bottom "fake" scrollbar made of two buttons and a horizontal slider
             *  in a Grid.
             */
            Grid horizontalPanel = new Grid();
            horizontalPanel.Height = 20;
            Grid.SetRow(horizontalPanel, 4);
            Grid.SetColumn(horizontalPanel, 0);
            grid.Children.Add(horizontalPanel);

            ColumnDefinition col4 = new ColumnDefinition();
            col4.Width = new GridLength(1, GridUnitType.Auto);
            ColumnDefinition col5 = new ColumnDefinition();
            col5.Width = new GridLength(1, GridUnitType.Star);
            ColumnDefinition col6 = new ColumnDefinition();
            col6.Width = new GridLength(1, GridUnitType.Auto);

            horizontalPanel.ColumnDefinitions.Add(col4);
            horizontalPanel.ColumnDefinitions.Add(col5);
            horizontalPanel.ColumnDefinitions.Add(col6);

            _pageLeftButton = new Button();
            horizontalPanel.Children.Add(_pageLeftButton);
            Grid.SetColumn(_pageLeftButton, 0);
            _pageLeftButton.Width = 20;
            _pageLeftButton.Height = 20;
            _pageLeftButton.Padding = new Thickness(0, 0, 0, 0);
            _pageLeftButton.Content = "<";
            _pageLeftButton.Click += new RoutedEventHandler(OnPageLeftButtonClick);            

            _horizontalOffsetSlider = new Slider();
            horizontalPanel.Children.Add(_horizontalOffsetSlider);
            Grid.SetColumn(_horizontalOffsetSlider, 1);            
            _horizontalOffsetSlider.Width = 400;
            _horizontalOffsetSlider.AutoToolTipPlacement = AutoToolTipPlacement.BottomRight;

            _pageRightButton = new Button();
            horizontalPanel.Children.Add(_pageRightButton);
            Grid.SetColumn(_pageRightButton, 2);
            _pageRightButton.Width = 20;
            _pageRightButton.Height = 20;
            _pageRightButton.Padding = new Thickness(0, 0, 0, 0);
            _pageRightButton.Content = ">";
            _pageRightButton.Click += new RoutedEventHandler(OnPageRightButtonClick);            


            /*
             * Our "fake" vertical Scrollbar made up of six buttons and a Vertical Slider
             * in a Grid.
             */

            Grid verticalPanel = new Grid();
            verticalPanel.Width = 20;
            Grid.SetRow(verticalPanel, 3);
            Grid.SetColumn(verticalPanel, 1);
            grid.Children.Add(verticalPanel);

            row1 = new RowDefinition();
            row1.Height = new GridLength(1, GridUnitType.Auto);
            row2 = new RowDefinition();
            row2.Height = new GridLength(1, GridUnitType.Auto);
            row3 = new RowDefinition();
            row3.Height = new GridLength(1, GridUnitType.Auto);
            row4 = new RowDefinition();
            row4.Height = new GridLength(1, GridUnitType.Star);
            row5 = new RowDefinition();
            row5.Height = new GridLength(1, GridUnitType.Auto);
            RowDefinition row6 = new RowDefinition();
            row6.Height = new GridLength(1, GridUnitType.Auto);
            RowDefinition row7 = new RowDefinition();
            row7.Height = new GridLength(1, GridUnitType.Auto);

            verticalPanel.RowDefinitions.Add(row1);
            verticalPanel.RowDefinitions.Add(row2);
            verticalPanel.RowDefinitions.Add(row3);
            verticalPanel.RowDefinitions.Add(row4);
            verticalPanel.RowDefinitions.Add(row5);
            verticalPanel.RowDefinitions.Add(row6);
            verticalPanel.RowDefinitions.Add(row7);

            _homeButton = new Button();
            verticalPanel.Children.Add(_homeButton);
            Grid.SetRow(_homeButton, 0);
            _homeButton.Width = 20;
            _homeButton.Height = 20;
            _homeButton.Padding = new Thickness(0, 0, 0, 0);
            _homeButton.Content = "*";
            _homeButton.Click += new RoutedEventHandler(OnHomeButtonClick);

            _pageUpButton = new Button();
            verticalPanel.Children.Add(_pageUpButton);
            Grid.SetRow(_pageUpButton, 1);
            _pageUpButton.Width = 20;
            _pageUpButton.Height = 20;
            _pageUpButton.Padding = new Thickness(0, 0, 0, 0);
            _pageUpButton.Content = "^";
            _pageUpButton.Click += new RoutedEventHandler(OnPageUpButtonClick);

            _prevPageButton = new Button();
            verticalPanel.Children.Add(_prevPageButton);
            Grid.SetRow(_prevPageButton, 2);
            _prevPageButton.Width = 20;
            _prevPageButton.Height = 20;
            _prevPageButton.Padding = new Thickness(0, 0, 0, 0);
            _prevPageButton.Content = "-";
            _prevPageButton.Click += new RoutedEventHandler(OnPreviousPageButtonClick);

            _verticalOffsetSlider = new Slider();
            _verticalOffsetSlider.Orientation = Orientation.Vertical;
            verticalPanel.Children.Add(_verticalOffsetSlider);
            Grid.SetRow(_verticalOffsetSlider, 3);            
            _verticalOffsetSlider.Height = 400;
            _verticalOffsetSlider.VerticalAlignment = VerticalAlignment.Stretch;
            _verticalOffsetSlider.IsDirectionReversed = true;
            _verticalOffsetSlider.AutoToolTipPlacement = AutoToolTipPlacement.BottomRight;

            _nextPageButton = new Button();
            verticalPanel.Children.Add(_nextPageButton);
            Grid.SetRow(_nextPageButton, 4);
            _nextPageButton.Width = 20;
            _nextPageButton.Height = 20;
            _nextPageButton.Padding = new Thickness(0, 0, 0, 0);
            _nextPageButton.Content = "+";
            _nextPageButton.Click += new RoutedEventHandler(OnNextPageButtonClick);

            _pageDownButton = new Button();
            verticalPanel.Children.Add(_pageDownButton);
            Grid.SetRow(_pageDownButton, 5);
            _pageDownButton.Width = 20;
            _pageDownButton.Height = 20;
            _pageDownButton.Padding = new Thickness(0, 0, 0, 0);
            _pageDownButton.Content = "v";            
            _pageDownButton.Click += new RoutedEventHandler(OnPageDownButtonClick);

            _endButton = new Button();
            verticalPanel.Children.Add(_endButton);
            Grid.SetRow(_endButton, 6);
            _endButton.Width = 20;
            _endButton.Height = 20;
            _endButton.Padding = new Thickness(0, 0, 0, 0);
            _endButton.Content = "*";
            _endButton.Click += new RoutedEventHandler(OnEndButtonClick);

            //Set up bindings                        
            CreateDocumentViewerBinding("Zoom", _zoomSlider, Slider.ValueProperty, BindingMode.TwoWay);
            CreateDocumentViewerBinding("Zoom", _zoomTextBox, TextBox.TextProperty, BindingMode.TwoWay);
            CreateDocumentViewerBinding("MaxPagesAcross", _columnSlider, Slider.ValueProperty, BindingMode.TwoWay);
            CreateDocumentViewerBinding("VerticalOffset", _verticalOffsetSlider, Slider.ValueProperty, BindingMode.TwoWay);
            CreateDocumentViewerBinding("ExtentHeight", _verticalOffsetSlider, Slider.MaximumProperty, BindingMode.OneWay);
            CreateDocumentViewerBinding("HorizontalOffset", _horizontalOffsetSlider, Slider.ValueProperty, BindingMode.TwoWay);
            CreateDocumentViewerBinding("ExtentWidth", _horizontalOffsetSlider, Slider.MaximumProperty, BindingMode.OneWay);
            CreateDocumentViewerBinding("MasterPageNumber", _firstPageTextBox, TextBox.TextProperty, BindingMode.OneWay);            
            CreateDocumentViewerBinding("HorizontalPageSpacing", _horizontalSpacingSlider, Slider.ValueProperty, BindingMode.TwoWay);
            CreateDocumentViewerBinding("VerticalPageSpacing", _verticalSpacingSlider, Slider.ValueProperty, BindingMode.TwoWay);
            CreateDocumentViewerBinding("CanMoveUp", _canMoveUpLabel, Label.ContentProperty, BindingMode.OneWay);
            CreateDocumentViewerBinding("CanMoveDown", _canMoveDownLabel, Label.ContentProperty, BindingMode.OneWay);
            CreateDocumentViewerBinding("CanMoveLeft", _canMoveLeftLabel, Label.ContentProperty, BindingMode.OneWay);
            CreateDocumentViewerBinding("CanMoveRight", _canMoveRightLabel, Label.ContentProperty, BindingMode.OneWay);            
            
            return border;
        }
        
        //Event Handlers

        /// <summary>
        /// Handler for the Click event on the FitPagesAcrossButton.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        private void OnFitPagesAcrossMenuClick(object source, RoutedEventArgs args)
        {
            FitPagesAcrossCommand();
        }

        /// <summary>
        /// Handler for the Click event on the FitPageMenu.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        private void OnFitPageMenuClick(object source, RoutedEventArgs args)
        {
            FitPageCommand();
        }

        /// <summary>
        /// Handler for the Click event on the FitWidthMenu.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        private void OnFitWidthMenuClick(object source, RoutedEventArgs args)
        {
            FitWidthCommand();
        }

        /// <summary>
        /// Handler for the Click event on the FitHeightMenu.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        private void OnFitHeightMenuClick(object source, RoutedEventArgs args)
        {
            FitHeightCommand();
        }

        /// <summary>
        /// Handler for the Click event on the ThumbnailsMenu.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        private void OnThumbnailsMenuClick(object source, RoutedEventArgs args)
        {
            ViewThumbnailsCommand();
        }

        /// <summary>
        /// Handler for the Click event on the ShowBordersMenu.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        private void OnShowBordersMenuClick(object source, RoutedEventArgs args)
        {
            _showBordersMenu.IsChecked = !_showBordersMenu.IsChecked;
            TogglePageBorders();
        }

        /// <summary>
        /// Handler for the Click event on the PageLeftButton.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        private void OnPageLeftButtonClick(object source, RoutedEventArgs args)
        {
            PageLeftCommand();
        }

        /// <summary>
        /// Handler for the Click event on the PageRightButton.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        private void OnPageRightButtonClick(object source, RoutedEventArgs args)
        {
            PageRightCommand();
        }

        /// <summary>
        /// Handler for the Click event on the PageUpButton.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        private void OnPageUpButtonClick(object source, RoutedEventArgs args)
        {
            PageUpCommand();
        }

        /// <summary>
        /// Handler for the Click event on the PageDownButton.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        private void OnPageDownButtonClick(object source, RoutedEventArgs args)
        {
            PageDownCommand();
        }

        /// <summary>
        /// Handler for the Click event on the PreviousPageButton.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        private void OnPreviousPageButtonClick(object source, RoutedEventArgs args)
        {
            PreviousPageCommand();
        }

        /// <summary>
        /// Handler for the Click event on the NextPageButton.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        private void OnNextPageButtonClick(object source, RoutedEventArgs args)
        {
            NextPageCommand();
        }

        /// <summary>
        /// Handler for the Click event on the HomeButton.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        private void OnHomeButtonClick(object source, RoutedEventArgs args)
        {
            HomeCommand();
        }

        /// <summary>
        /// Handler for the Click event on the EndButton.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        private void OnEndButtonClick(object source, RoutedEventArgs args)
        {
            EndCommand();
        }

        /// <summary>
        /// Handler for the Click event on the FixedContentButton.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        private void OnFixedContentMenuClick(object source, RoutedEventArgs args)
        {
            SetFixedContent();
        }

        /// <summary>
        /// Handler for the Click event on the VaryingFixedContentMenu.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        private void OnVaryingFixedContentMenuClick(object source, RoutedEventArgs args)
        {
            SetVaryingFixedContent();
        }

        /// <summary>
        /// Handler for the Click event on the NullContentMenu.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        private void OnNullContentMenuClick(object source, RoutedEventArgs args)
        {
            SetNullContent();
        }

        /// <summary>
        /// Handler for the Click event on the XpsContentMenu.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        private void OnXpsContentMenuClick(object source, RoutedEventArgs args)
        {
            SetUserContent(true /* isXps */);
        }

        /// <summary>
        /// Handler for the Click event on the XamlContentMenu.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        private void OnXamlContentMenuClick(object source, RoutedEventArgs args)
        {
            SetUserContent(false /* isXps */);
        }

        /// <summary>
        /// Handler for the Click event on the DecreaseZoomMenu.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        private void OnDecreaseZoomMenuClick(object source, RoutedEventArgs args)
        {
            DecreaseZoomCommand();
        }

        /// <summary>
        /// Handler for the Click event on the IncreaseZoomMenu.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        private void OnIncreasePageSizeMenuClick(object source, RoutedEventArgs args)
        {
            IncreasePageSize();
        }

        /// <summary>
        /// Handler for the Click event on the DecreaseZoomMenu.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        private void OnDecreasePageSizeMenuClick(object source, RoutedEventArgs args)
        {
            DecreasePageSize();
        }

        /// <summary>
        /// Handler for the Click event on the IncreaseZoomMenu.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        private void OnIncreaseZoomMenuClick(object source, RoutedEventArgs args)
        {
            IncreaseZoomCommand();
        }

        /// <summary>
        /// Handler for the Click event on the IncreaseZoomMenu.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        private void OnGotoPageButtonClick(object source, RoutedEventArgs args)
        {
            GotoPage( (int)double.Parse(_firstPageTextBox.Text) );
        }

        /// <summary>
        /// Handler for the Click event on the ZoomButton.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        private void OnSetZoomButtonClick(object source, RoutedEventArgs args)
        {
            _documentViewer.Zoom = (int)double.Parse(_zoomTextBox.Text);
        }


        /// <summary>
        /// Creates a new Two-Way binding between a property on DocumentViewer and a property 
        /// on the passed-in FrameworkElement.
        /// </summary>
        /// <param name="path">The path to the property on DocumentViewer</param>
        /// <param name="target">The target FE of the bind</param>
        /// <param name="targetProperty">The target property of the bind</param>
        private void CreateDocumentViewerBinding(string path, 
            FrameworkElement target, DependencyProperty targetProperty, BindingMode mode)
        {
            Binding newBinding = new Binding(path);
            newBinding.Source = _documentViewer;
            newBinding.Mode = mode;
            target.SetBinding(targetProperty, newBinding);
        }

        #endregion Implementation helpers

        //UI elements
        private DocumentViewer _documentViewer;

        private Slider _zoomSlider;
        private Slider _columnSlider;        
        private Slider _horizontalSpacingSlider;
        private Slider _verticalSpacingSlider;
        private Slider _horizontalOffsetSlider;
        private Slider _verticalOffsetSlider;
        private TextBox _firstPageTextBox;
        private TextBox _zoomTextBox;
        private Label _canMoveUpLabel;
        private Label _canMoveDownLabel;
        private Label _canMoveLeftLabel;
        private Label _canMoveRightLabel;
        private MenuItem _fitColumnsMenu;
        private MenuItem _fitPageMenu;
        private MenuItem _fitWidthMenu;
        private MenuItem _fitHeightMenu;
        private MenuItem _thumbnailsMenu;
        private MenuItem _showBordersMenu;       
        private MenuItem _fixedContentMenu;
        private MenuItem _varyingFixedContentMenu;
        private MenuItem _nullContentMenu;
        private MenuItem _xpsContentMenu;
        private MenuItem _xamlContentMenu;
        private MenuItem _decreaseZoomMenu;
        private MenuItem _increaseZoomMenu;
        private MenuItem _decreasePageSizeMenu;
        private MenuItem _increasePageSizeMenu;
        private Button _gotoPageButton;
        private Button _setZoomButton;
        private Button _pageLeftButton;
        private Button _pageRightButton;
        private Button _pageUpButton;
        private Button _pageDownButton;
        private Button _prevPageButton;
        private Button _nextPageButton;
        private Button _homeButton;
        private Button _endButton;

        /// <summary>
        /// The current state of the DRT.
        /// </summary>
        private State _state;

        /// <summary>
        /// Whether pagination has completed since the last content was loaded.
        /// </summary>
        private bool _paginationCompleted;

        // Amount of time (msec) to delay between tests so that a human can see what's happening.
        private int _delay = 0;

        // Default value for _delay (if command line option -delay is specified without an argument).
        private const int _defaultDelay = 500;

        // Amount of time (msec) to delay while waiting for pagination to complete.
        private const int _paginationDelay = 500;

        // Offsets used for testing that initial offset assignments are honored.
        private const double _testVerticalOffset = 2500.0;
        private const double _testHorizontalOffset = 50.0;

        // The stream we use to open our XAML files.
        private System.IO.FileStream _fileStream;

        private string _userContentFile = null;

        private const string _xamlExtension = ".xaml";

        enum State
        {
            Start = 0,
            Hold,
            PaginationCompleted
        }
    }
}

