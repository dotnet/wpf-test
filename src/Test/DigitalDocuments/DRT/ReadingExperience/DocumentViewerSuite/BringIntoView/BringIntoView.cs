// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
//
// Description: The BringIntoView Suite tests DocumentViewer's 
//              BringIntoView operations.
//

using DRT;
using System;
using System.Collections;
using System.ComponentModel;
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
using System.Windows.Markup;    


namespace DRTDocumentViewerSuite
{
      
    /// <summary>
    /// The BringIntoView Suite tests DocumentViewer's BringIntoView operations.
    /// </summary>
    public sealed class BringIntoViewSuite : DrtTestSuite
    {
        public BringIntoViewSuite() : base("BringIntoView")
        {
            TeamContact = "WPF";
            Contact = "Microsoft";
        }

        /// <summary>
        /// Prepare the list of tests to run and sets up initial state.  Called by base class.
        /// </summary>
        /// <returns></returns>
        public override DrtTest[] PrepareTests()
        {
            Visual root = CreateTree();
            DRT.Show(root);

            _paginationCompleted = false;
            _documentViewer.Document = CreateDocument();
            DocumentPaginator.PaginationCompleted += new EventHandler(OnPaginationCompleted);
            DocumentPaginator.PaginationProgress += new PaginationProgressEventHandler(OnPaginationProgress);

            if (DocumentPaginator.IsPageCountValid)
            {
                OnPaginationCompleted(DocumentPaginator, EventArgs.Empty);
            }            

             // return the lists of tests to run against the tree

            if (_hold)
            {
                return new DrtTest[]
                {
                    new DrtTest( Hold ),
                };
            }
            else
            {
                return new DrtTest[]
                {
                    new DrtTest( WaitForPaginationCompleted ),
                    new DrtTest( StartTest ),                                        
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

            _hold = false;

            if (option)
            {
                switch (arg)
                {                   
                    case "test":
                        _hold = true;                        
                        handled = true;
                        break;
                }
            }

            return handled;
        }

        /// <summary>
        /// Holds the DRT forever to allow user interaction.
        /// </summary>
        private void Hold()
        {
            DRT.Pause(10000);
            DRT.ResumeAt(Hold);
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
        /// Invokes the test
        /// </summary>
        private void StartTest()
        {
            Console.WriteLine("Starting test");
            _currentElement = 0;
            _currentZoom = 0;

            DRT.ResumeAt(BringNextElementIntoView);
        }

        /// <summary>
        /// Invokes BringIntoView on the next element to be made visible.
        /// </summary>
        private void BringNextElementIntoView()
        {
            //grab the first element
            ElementInfo info = _elements[_currentElement];           
            
            //We need the element's size 
            //Measure the element
            info.Element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            Rect bounds = new Rect(0, 0, info.Element.DesiredSize.Width, info.Element.DesiredSize.Height);
           
            //Make it visible now!
            info.Element.BringIntoView(bounds);

            DRT.ResumeAt(ConfirmNextElementIsVisible);
        }
        
        /// <summary>
        /// Verifies that the element was actually made visible.
        /// </summary>
        private void ConfirmNextElementIsVisible()
        {
            //Ensure that the element is currently visible onscreen.
            ElementInfo info = _elements[_currentElement];

            DRT.Assert(((Visual)DocumentViewerContentHost).IsAncestorOf(info.Element),
                String.Format("Page {0} was not brought into view!", info.PageNumber));

            //Get the bounding rects of the elements
            Rect elementRect = VisualTreeHelper.GetDescendantBounds(info.Element);
            Rect viewportRect = VisualTreeHelper.GetDescendantBounds(DocumentViewerContentHost);
            
            GeneralTransform transform = info.Element.TransformToAncestor(DocumentViewerContentHost);           
            Rect offsetRect = transform.TransformBounds(elementRect);

            //We assert that at least a portion of the target was brought into view.
            //(We don't use "viewportRect.Contains" here because at larger zoom levels it's very unlikely that
            //all of the target can be made visible)
            DRT.Assert(viewportRect.IntersectsWith(offsetRect),
                String.Format("Element {0} was not brought into view!", info.Name));

            DRT.ResumeAt(MoveToNextElement);
        
        }

        /// <summary>
        /// Prepares the test to bring the next element into view, or moves to the next Zoom
        /// level if no elements are left in the list.
        /// </summary>
        private void MoveToNextElement()
        {
            _currentElement++;

            if (_currentElement < _elements.Length)
            {
                System.Console.WriteLine("Moving to element {0}", _elements[_currentElement].Name);
                DRT.ResumeAt(BringNextElementIntoView);
            }
            else
            {
                //We're out of elements, reset the count and move to the next Zoom level                
                _currentElement = 0;
                DRT.ResumeAt(MoveToNextZoom);
            }
        }

        /// <summary>
        /// Sets DocumentViewer to the next Zoom level and continues the test.
        /// </summary>
        private void MoveToNextZoom()
        {
            _currentZoom++;

            if (_currentZoom < _zoomLevels.Length)
            {
                System.Console.WriteLine("Moving to zoom {0}", _zoomLevels[_currentZoom]);
                _documentViewer.Zoom = _zoomLevels[_currentZoom];
                DRT.ResumeAt(BringNextElementIntoView);
            }

            //Otherwise we're done with the DRT tests.  Do nothing.
        }


        /// <summary>
        /// Creates our visual tree -- just a DocumentViewer.
        /// </summary>
        /// <returns></returns>
        private Visual CreateTree()
        {
            _documentViewer = new DocumentViewer();                        

            return _documentViewer;
        }
        
        /// <summary>
        /// Uses DocumentFactory to create a new FixedDocument.
        /// </summary>
        /// <returns></returns>
        private IDocumentPaginatorSource CreateDocument()
        {
            DocumentFactory factory = new DocumentFactory();
            return factory.CreateDocument(_elements);            
        }
                    

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

        /// <summary>
        /// The DocumentPaginator attached to DocumentViewer.
        /// </summary>
        private DynamicDocumentPaginator DocumentPaginator
        {
            get { return _documentViewer.Document.DocumentPaginator as DynamicDocumentPaginator; }
        }

        /// <summary>
        /// The ContentHost for DocumentViewer's Content.
        /// </summary>
        private UIElement DocumentViewerContentHost
        {
            get 
            {
                if (_documentViewer != null && _contentHost == null)
                {
                    _contentHost = _documentViewer.Template.FindName("PART_ContentHost", _documentViewer) as UIElement;
                }

                return _contentHost;
            }
        }

        /// <summary>
        /// DocumentFactory creates Documents with specified elements in them.
        /// </summary>
        private class DocumentFactory
        {
            public DocumentFactory()
            {

            }

            /// <summary>
            /// Creates a new document that includes the given ElementInfo objects.
            /// </summary>
            /// <param name="elements"></param>
            /// <returns></returns>
            public FixedDocument CreateDocument(ElementInfo[] elements)
            {
                CreatePages(elements);

                //Return a new FixedDocument
                FixedDocument doc = new FixedDocument();
                
                for (int i = 0; i <= _lastPage; i++)
                {
                    Canvas pageCanvas = GetCanvasForPage(i);

                    FixedPage fixedPage = new FixedPage();
                    FixedPage.SetLeft(pageCanvas, 0);
                    FixedPage.SetTop(pageCanvas, 0);
                    fixedPage.Width = pageCanvas.Width;
                    fixedPage.Height = pageCanvas.Height;
                    fixedPage.Children.Add(pageCanvas);
                    
                    PageContent content = new PageContent();

                    content.BeginInit();
                    ((IAddChild)content).AddChild(fixedPage);
                    content.EndInit();

                    ((IAddChild)doc).AddChild(content);

                    
                }

                return doc;

            }

            /// <summary>
            /// Creates a set of pages that includes each of the incoming
            /// ElementInfo objects in their proper locations.
            /// </summary>
            /// <param name="elements"></param>
            private void CreatePages(ElementInfo[] elements)
            {
                _pageTable = new Hashtable();
                _lastPage = 0;

                for (int i = 0; i < elements.Length; i++)
                {
                    ElementInfo info = elements[i];
                    //Get the canvas for the link
                    Canvas linkCanvas = GetCanvasForPage(info.PageNumber);

                    //Add the target to the canvas                    
                    Canvas.SetLeft(info.Element, info.Location.X);
                    Canvas.SetTop(info.Element, info.Location.Y);
                    linkCanvas.Children.Add( info.Element );

                    _lastPage = Math.Max(_lastPage, info.PageNumber);                    
                }
            }           

            /// <summary>
            /// Returns the canvas for the given page number from the Page Hashtable,
            /// or creates a new one if none exists.
            /// </summary>
            /// <param name="pageNumber"></param>
            /// <returns></returns>
            private Canvas GetCanvasForPage(int pageNumber)
            {
                if (_pageTable.ContainsKey(pageNumber))
                {
                    //return the existing Canvas
                    return _pageTable[pageNumber] as Canvas;
                }
                else
                {
                    //create a new empty Canvas
                    Canvas canvas = new Canvas();
                    canvas.Width = 96 * 8.5;
                    canvas.Height = 96 * 11;

                    _pageTable.Add(pageNumber, canvas);
                    return canvas;
                }
            }

            private Hashtable _pageTable;
            private int _lastPage;            
        }
        
        /// <summary>
        /// Represents an Element in a document that will be brought into view.
        /// </summary>
        private struct ElementInfo
        {
            public ElementInfo(String name, String label, int pageNumber, Point location)
            {
                _name = name;
                _label = label;
                _pageNumber = pageNumber;
                _location = location;

                _element = new Label();
                ((Label)_element).Content = _label;
                _element.Name = _name;
            }

            /// <summary>
            /// The element to be added to the document and brought into view
            /// </summary>
            public FrameworkElement Element { get { return _element; } }

            /// <summary>
            /// The Name of the element
            /// </summary>
            public String Name { get { return _name; } }

            /// <summary>
            /// A text label to put on the element
            /// </summary>
            public String Label { get { return _label; } }

            /// <summary>
            /// The page the element will be placed on
            /// </summary>
            public int PageNumber { get { return _pageNumber; } }            

            /// <summary>
            /// The position on the page the element will be placed at.
            /// </summary>
            public Point Location { get { return _location; } }

            private FrameworkElement _element;
            private String _name;
            private String _label;
            private int _pageNumber;
            private Point _location;
        }
        

        /// <summary>
        /// The list of elements to add to the document, which will later be brought into view
        /// NOTE: when adding new elements, be sure that the offsets are such that the entire text will
        /// fit on the line -- otherwise the DRT test will fail because part of the element text will end up out
        /// of the bounding rect of DocumentViewer.
        /// </summary>
        private ElementInfo[] _elements = 
            {
                //Two elements on the same page
                new ElementInfo( "Target0", "Raindrops on roses", 0, new Point( 150.0, 150.0 ) ),
                new ElementInfo( "Target1", "Whiskers on kittens", 0, new Point( 540.0, 1030.0 ) ),

                //Middle of a new page
                new ElementInfo( "Target2", "Bright copper kettles", 5, new Point( 500.0, 500.0 ) ),

                //Upper left of a new page
                new ElementInfo( "Target3", "Warm woolen mittens", 3, new Point( 0.0, 0.0 ) ),

                //Lower right of a new page
                new ElementInfo( "Target4", "Brown paper packages tied up with strings", 10, new Point( 300.0, 1020.0 ) ),

                //Top right of a new page
                new ElementInfo( "Target5", "Z?", 1, new Point( 800.0, 0.0 ) ),

                //A bunch of elements on the same page
                new ElementInfo( "Target6", "All across the country", 2, new Point( 10.0, 10.0 ) ),
                new ElementInfo( "Target7", "coast to coast", 2, new Point(500.0, 320.0 ) ),
                new ElementInfo( "Target8", "people always say", 2, new Point(700.0, 10.0) ),
                new ElementInfo( "Target9", "'What do you like most?'", 2, new Point( 75.32, 900.0 ) ),
                new ElementInfo( "Target10", "Now I don't wanna brag,", 2, new Point( 400, 700.0 ) ),
                new ElementInfo( "Target11", "'n I don't wanna boast,", 2, new Point( 650, 32 ) ),
                new ElementInfo( "Target12", "But I always tell 'em", 2, new Point( 370, 1020 ) ),
                new ElementInfo( "Target13", "I like Toast!", 2, new Point( 760, 400 ) ),
                new ElementInfo( "Target14", "Yeah Toast!", 2, new Point( 200, 700 ) ),
                                        
            };

        /// <summary>
        /// The list of Zoom levels to test the bring into view operations at.
        /// Note that 100% (the default) is included by default.
        /// </summary>
        private double[] _zoomLevels =
            {
                250.0, 25.0, 500.0, 2500.0, 5.0, 5000.0
            };

        private bool _hold;

        private DocumentViewer _documentViewer;
        private UIElement _contentHost;

        private int _currentElement;
        private int _currentZoom;

        // Amount of time (msec) to delay while waiting for pagination to complete.
        private const int _paginationDelay = 500;
        private bool _paginationCompleted;
    }
}

