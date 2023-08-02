// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
//
// Description: Test suite for event routing functionality inside viewers.
//

using System;                               // string
using System.Reflection;                    // PropertyInfo, BindingFlags
using System.Text;                          // StringBuilder
using System.Windows;                       // Size, Rect
using System.Windows.Controls;              // FlowDocumentPageViewer
using System.Windows.Documents;             // FlowDocument
using System.Windows.Media;                 // Brushes
using System.Windows.Input;                 // NavigationCommands

namespace DRT
{
    /// <summary>
    /// Test suite for event routing functionality inside viewers.
    /// </summary>
    internal class ViewerEventRoutingTestSuite : FlowTestSuite
    {
        /// <summary>
        /// Static constructor.
        /// </summary>
        static ViewerEventRoutingTestSuite()
        {
            s_piCurrentViewer = typeof(FlowDocumentReader).GetProperty("CurrentViewer", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="suiteName">Name of the suite.</param>
        internal ViewerEventRoutingTestSuite() :
            base("ViewerEventRouting")
        {
            this.Contact = "Microsoft";
        }

        /// <summary>
        /// Returns collection of tests.
        /// </summary>
        protected override DrtTest[] CreateTests()
        {
            // Return the lists of tests to run against the tree
            return new DrtTest[] {
                // FlowDocumentReader in LogicalTree
                new DrtTest(CreateReaderViewer),                new DrtTest(WaitForPaginationCompleted),
                new DrtTest(AddEventHanldersForReaderViewer),   new DrtTest(Empty),
                new DrtTest(Hyperlink3ReaderViewer),            new DrtTest(Empty),
                new DrtTest(Button3ReaderViewer),               new DrtTest(Empty),
                new DrtTest(Hyperlink1ReaderViewer),            new DrtTest(Empty),
                new DrtTest(Button1ReaderViewer),               new DrtTest(Empty),
                new DrtTest(NestedHyperlinkReaderViewer),       new DrtTest(Empty),
                new DrtTest(NestedButtonReaderViewer),          new DrtTest(Empty),
                new DrtTest(GoToNextPage),                      new DrtTest(Empty),
                new DrtTest(NestedHyperlinkReaderViewer),       new DrtTest(Empty),
                new DrtTest(NestedButtonReaderViewer),          new DrtTest(Empty),
                new DrtTest(GoToNextPage),                      new DrtTest(Empty),
                new DrtTest(Hyperlink3ReaderViewer),            new DrtTest(Empty),
                new DrtTest(Button3ReaderViewer),               new DrtTest(Empty),
                new DrtTest(Hyperlink1ReaderViewer),            new DrtTest(Empty),
                new DrtTest(Button1ReaderViewer),               new DrtTest(Empty),
                new DrtTest(DisposeViewer),                     new DrtTest(Empty),
                new DrtTest(NavigateToReaderViewer),            new DrtTest(WaitForNavigation),
                new DrtTest(AddEventHanldersForReaderViewer),   new DrtTest(Empty),
                new DrtTest(Hyperlink3ReaderViewer),            new DrtTest(Empty),
                new DrtTest(Button3ReaderViewer),               new DrtTest(Empty),
                new DrtTest(Hyperlink1ReaderViewer),            new DrtTest(Empty),
                new DrtTest(Button1ReaderViewer),               new DrtTest(Empty),
                new DrtTest(NestedHyperlinkReaderViewer),       new DrtTest(Empty),
                new DrtTest(NestedButtonReaderViewer),          new DrtTest(Empty),
                new DrtTest(GoToNextPage),                      new DrtTest(Empty),
                new DrtTest(NestedHyperlinkReaderViewer),       new DrtTest(Empty),
                new DrtTest(NestedButtonReaderViewer),          new DrtTest(Empty),
                new DrtTest(GoToNextPage),                      new DrtTest(Empty),
                new DrtTest(Hyperlink3ReaderViewer),            new DrtTest(Empty),
                new DrtTest(Button3ReaderViewer),               new DrtTest(Empty),
                new DrtTest(Hyperlink1ReaderViewer),            new DrtTest(Empty),
                new DrtTest(Button1ReaderViewer),               new DrtTest(Empty),
                new DrtTest(DisposeViewer),                     new DrtTest(Empty),
            };
        }

        /// <summary>
        /// Creates FlowDocumentReader and loads FlowDocument.
        /// </summary>
        private void CreateReaderViewer()
        {
            // Load content from xaml file
            _document = LoadFromXaml("FlowDocumentEvents.xaml") as FlowDocument;
            DRT.Assert(_document != null, "Failed to load 'FlowDocumentEvents.xaml'.");

            // Setup FlowDocumentReader
            _readerViewer = new FlowDocumentReader();
            _readerViewer.Document = _document;
            this.Root.Child = _readerViewer;
        }

        /// <summary>
        /// Navigates to FlowDocumentReader.
        /// </summary>
        private void NavigateToReaderViewer()
        {
            // Load content from xaml file
            _document = LoadFromXaml("FlowDocumentEvents.xaml") as FlowDocument;
            DRT.Assert(_document != null, "Failed to load 'FlowDocumentEvents.xaml'.");

            // Setup Frame
            _frame = new Frame();
            _frame.Navigate(_document);
            this.Root.Child = _frame;
        }

        /// <summary>
        /// Wait for PaginationCompleted
        /// </summary>
        private void WaitForPaginationCompleted()
        {
            // Force pagination to happen, otherwise FlowDocumentReader will not 
            // show the page. It will wait for BackgroundPagination to finish.
            // But for the DRT it is already too late.
            ((IDocumentPaginatorSource)_document).DocumentPaginator.GetPage(100);
        }

        /// <summary>
        /// Wait for navigation.
        /// </summary>
        private void WaitForNavigation()
        {
            // Find FlowDocumentReader in the style of Frame
            DependencyObject visual = DRT.FindVisualByType(typeof(FlowDocumentReader), _frame, false);
            _readerViewer = visual as FlowDocumentReader;
            DRT.Assert(_readerViewer != null, "Cannot find FlowDocumentReader in the style of Frame.");

            WaitForPaginationCompleted();
        }

        /// <summary>
        /// Add event handlers for FlowDocumentReader
        /// </summary>
        private void AddEventHanldersForReaderViewer()
        {
            AddEventHandlers();
            _readerViewer.AddHandler(CustomEventID, new RoutedEventHandler(OnCustomEventOnViewer));
            _embeddedViewer = s_piCurrentViewer.GetValue(_readerViewer, null) as Control;
            DRT.Assert(_embeddedViewer != null, "Cannot find embedded viewer.");
            _embeddedViewer.AddHandler(CustomEventID, new RoutedEventHandler(OnCustomEventOnEmbeddedViewer));
        }

        /// <summary>
        /// Clean up FlowDocumentReader.
        /// </summary>
        private void DisposeViewer()
        {
            RemoveEventHandlers();
            if (_readerViewer != null)
            {
                _readerViewer.RemoveHandler(CustomEventID, new RoutedEventHandler(OnCustomEventOnViewer));
                _readerViewer = null;
            }
            if (_embeddedViewer != null)
            {
                _embeddedViewer.RemoveHandler(CustomEventID, new RoutedEventHandler(OnCustomEventOnEmbeddedViewer));
                _embeddedViewer = null;
            }
            _frame = null;
            this.Root.Child = null;
        }

        /// <summary>
        /// Hyperlink1ReaderViewer
        /// </summary>
        private void Hyperlink1ReaderViewer()
        {
            VerifyEventRouting("H1", "H1 P1 FD CV FDV");
        }

        /// <summary>
        /// Hyperlink3ReaderViewer
        /// </summary>
        private void Hyperlink3ReaderViewer()
        {
            VerifyEventRouting("H3", "H3 P3 FD CV FDV");
        }

        /// <summary>
        /// NestedHyperlinkReaderViewer
        /// </summary>
        private void NestedHyperlinkReaderViewer()
        {
            VerifyEventRouting("FD2H1", "FD2H1 FD2P1 FD2 FDR H2 P2 FD CV FDV");
        }

        /// <summary>
        /// Button1ReaderViewer
        /// </summary>
        private void Button1ReaderViewer()
        {
            VerifyEventRouting("B1", "B1 P1 FD CV FDV");
        }

        /// <summary>
        /// Button3ReaderViewer
        /// </summary>
        private void Button3ReaderViewer()
        {
            VerifyEventRouting("B3", "B3 P3 FD CV FDV");
        }

        /// <summary>
        /// NestedButtonReaderViewer
        /// </summary>
        private void NestedButtonReaderViewer()
        {
            VerifyEventRouting("FD2B1", "FD2B1 FD2P1 FD2 FDR H2 P2 FD CV FDV");
        }

        // Goes to the next page.
        private void GoToNextPage()
        {
            DependencyObject target = (_readerViewer != null) ? _readerViewer : null;
            NavigationCommands.NextPage.Execute(null, (IInputElement)target);
        }

        /// <summary>
        /// Add event handlers for content of FlowDocument.
        /// </summary>
        private void AddEventHandlers()
        {
            FrameworkElement fe;
            FrameworkContentElement fce;

            // Buttons
            fe = DRT.FindElementByID("B1", _document) as FrameworkElement;
            DRT.Assert(fe != null, "Cannot find element with name '{0}'.", "B1");
            fe.AddHandler(CustomEventID, new RoutedEventHandler(OnCustomEventOnElement));
            fe = DRT.FindElementByID("B3", _document) as FrameworkElement;
            DRT.Assert(fe != null, "Cannot find element with name '{0}'.", "B3");
            fe.AddHandler(CustomEventID, new RoutedEventHandler(OnCustomEventOnElement));
            fe = DRT.FindElementByID("FD2B1", _document) as FrameworkElement;
            DRT.Assert(fe != null, "Cannot find element with name '{0}'.", "FD");
            fe.AddHandler(CustomEventID, new RoutedEventHandler(OnCustomEventOnElement));
            // Hyperlinks
            fce = DRT.FindElementByID("H1", _document) as FrameworkContentElement;
            DRT.Assert(fce != null, "Cannot find element with name '{0}'.", "H1");
            fce.AddHandler(CustomEventID, new RoutedEventHandler(OnCustomEventOnElement));
            fce = DRT.FindElementByID("H2", _document) as FrameworkContentElement;
            DRT.Assert(fce != null, "Cannot find element with name '{0}'.", "H2");
            fce.AddHandler(CustomEventID, new RoutedEventHandler(OnCustomEventOnElement));
            fce = DRT.FindElementByID("H3", _document) as FrameworkContentElement;
            DRT.Assert(fce != null, "Cannot find element with name '{0}'.", "H3");
            fce.AddHandler(CustomEventID, new RoutedEventHandler(OnCustomEventOnElement));
            fce = DRT.FindElementByID("FD2H1", _document) as FrameworkContentElement;
            DRT.Assert(fce != null, "Cannot find element with name '{0}'.", "FD2H1");
            fce.AddHandler(CustomEventID, new RoutedEventHandler(OnCustomEventOnElement));
            // Paragraphs
            fce = DRT.FindElementByID("P1", _document) as FrameworkContentElement;
            DRT.Assert(fce != null, "Cannot find element with name '{0}'.", "P1");
            fce.AddHandler(CustomEventID, new RoutedEventHandler(OnCustomEventOnElement));
            fce = DRT.FindElementByID("P2", _document) as FrameworkContentElement;
            DRT.Assert(fce != null, "Cannot find element with name '{0}'.", "P2");
            fce.AddHandler(CustomEventID, new RoutedEventHandler(OnCustomEventOnElement));
            fce = DRT.FindElementByID("P3", _document) as FrameworkContentElement;
            DRT.Assert(fce != null, "Cannot find element with name '{0}'.", "P3");
            fce.AddHandler(CustomEventID, new RoutedEventHandler(OnCustomEventOnElement));
            fce = DRT.FindElementByID("FD2P1", _document) as FrameworkContentElement;
            DRT.Assert(fce != null, "Cannot find element with name '{0}'.", "FD2P1");
            fce.AddHandler(CustomEventID, new RoutedEventHandler(OnCustomEventOnElement));
            // Nested FlowDocument
            fce = DRT.FindElementByID("FD2", _document) as FrameworkContentElement;
            DRT.Assert(fce != null, "Cannot find element with name '{0}'.", "FD2");
            fce.AddHandler(CustomEventID, new RoutedEventHandler(OnCustomEventOnElement));
            // Nested FlowDocumentReader
            fe = DRT.FindElementByID("FDR", _document) as FrameworkElement;
            DRT.Assert(fe != null, "Cannot find element with name '{0}'.", "FDR");
            fe.AddHandler(CustomEventID, new RoutedEventHandler(OnCustomEventOnElement));
            // FlowDocument
            _document.AddHandler(CustomEventID, new RoutedEventHandler(OnCustomEventOnElement));
        }

        /// <summary>
        /// Remove event handlers from content of FlowDocument.
        /// </summary>
        private void RemoveEventHandlers()
        {
            FrameworkElement fe;
            FrameworkContentElement fce;

            // Buttons
            fe = DRT.FindElementByID("B1", _document) as FrameworkElement;
            DRT.Assert(fe != null, "Cannot find element with name '{0}'.", "B1");
            fe.RemoveHandler(CustomEventID, new RoutedEventHandler(OnCustomEventOnElement));
            fe = DRT.FindElementByID("B3", _document) as FrameworkElement;
            DRT.Assert(fe != null, "Cannot find element with name '{0}'.", "B3");
            fe.RemoveHandler(CustomEventID, new RoutedEventHandler(OnCustomEventOnElement));
            fe = DRT.FindElementByID("FD2B1", _document) as FrameworkElement;
            DRT.Assert(fe != null, "Cannot find element with name '{0}'.", "FD");
            fe.RemoveHandler(CustomEventID, new RoutedEventHandler(OnCustomEventOnElement));
            // Hyperlinks
            fce = DRT.FindElementByID("H1", _document) as FrameworkContentElement;
            DRT.Assert(fce != null, "Cannot find element with name '{0}'.", "H1");
            fce.RemoveHandler(CustomEventID, new RoutedEventHandler(OnCustomEventOnElement));
            fce = DRT.FindElementByID("H2", _document) as FrameworkContentElement;
            DRT.Assert(fce != null, "Cannot find element with name '{0}'.", "H2");
            fce.RemoveHandler(CustomEventID, new RoutedEventHandler(OnCustomEventOnElement));
            fce = DRT.FindElementByID("H3", _document) as FrameworkContentElement;
            DRT.Assert(fce != null, "Cannot find element with name '{0}'.", "H3");
            fce.RemoveHandler(CustomEventID, new RoutedEventHandler(OnCustomEventOnElement));
            fce = DRT.FindElementByID("FD2H1", _document) as FrameworkContentElement;
            DRT.Assert(fce != null, "Cannot find element with name '{0}'.", "FD2H1");
            fce.RemoveHandler(CustomEventID, new RoutedEventHandler(OnCustomEventOnElement));
            // Paragraphs
            fce = DRT.FindElementByID("P1", _document) as FrameworkContentElement;
            DRT.Assert(fce != null, "Cannot find element with name '{0}'.", "P1");
            fce.RemoveHandler(CustomEventID, new RoutedEventHandler(OnCustomEventOnElement));
            fce = DRT.FindElementByID("P2", _document) as FrameworkContentElement;
            DRT.Assert(fce != null, "Cannot find element with name '{0}'.", "P2");
            fce.RemoveHandler(CustomEventID, new RoutedEventHandler(OnCustomEventOnElement));
            fce = DRT.FindElementByID("P3", _document) as FrameworkContentElement;
            DRT.Assert(fce != null, "Cannot find element with name '{0}'.", "P3");
            fce.RemoveHandler(CustomEventID, new RoutedEventHandler(OnCustomEventOnElement));
            fce = DRT.FindElementByID("FD2P1", _document) as FrameworkContentElement;
            DRT.Assert(fce != null, "Cannot find element with name '{0}'.", "FD2P1");
            fce.RemoveHandler(CustomEventID, new RoutedEventHandler(OnCustomEventOnElement));
            // Nested FlowDocument
            fce = DRT.FindElementByID("FD2", _document) as FrameworkContentElement;
            DRT.Assert(fce != null, "Cannot find element with name '{0}'.", "FD2");
            fce.RemoveHandler(CustomEventID, new RoutedEventHandler(OnCustomEventOnElement));
            // Nested FlowDocumentReader
            fe = DRT.FindElementByID("FDR", _document) as FrameworkElement;
            DRT.Assert(fe != null, "Cannot find element with name '{0}'.", "FDR");
            fe.RemoveHandler(CustomEventID, new RoutedEventHandler(OnCustomEventOnElement));
            // FlowDocument
            _document.RemoveHandler(CustomEventID, new RoutedEventHandler(OnCustomEventOnElement)); 
        }

        /// <summary>
        /// Custom RoutedEvent.
        /// </summary>
        internal static readonly RoutedEvent CustomEventID = EventManager.RegisterRoutedEvent("Custom", RoutingStrategy.Bubble, typeof(EventHandler), typeof(DependencyObject));

        /// <summary>
        /// Reset event route
        /// </summary>
        private void ResetEventRoute()
        {
            _eventRoute = new StringBuilder();
        }

        /// <summary>
        /// Raise custom event and verify event route.
        /// </summary>
        /// <param name="elementName">Name of the target element.</param>
        /// <param name="eventRoute">Expected event route.</param>
        private void VerifyEventRouting(string elementName, string eventRoute)
        {
            // Reset event route.
            ResetEventRoute();

            DependencyObject obj = DRT.FindElementByID(elementName, _document);
            FrameworkElement fe = obj as FrameworkElement;
            FrameworkContentElement fce = obj as FrameworkContentElement;
            DRT.Assert(fe != null || fce != null, "Cannot find element '{0}' in the FlowDocument.", elementName);

            // Raise custom event.
            if (fe != null)
                fe.RaiseEvent(new RoutedEventArgs(CustomEventID));
            else
                fce.RaiseEvent(new RoutedEventArgs(CustomEventID));

            // Verify event routing
            string route = _eventRoute.ToString();
            DRT.Assert(route == eventRoute, "Expecting event route '{0}, got {1}.", eventRoute, route);
        }

        /// <summary>
        /// CustomEventID handler.
        /// </summary>
        private void OnCustomEventOnElement(object sender, RoutedEventArgs args)
        {
            DependencyObject obj = sender as DependencyObject;
            DRT.Assert(obj != null, "Sender of CustomEventID is null.");
            _eventRoute.Append((string)obj.GetValue(FrameworkElement.NameProperty));
            _eventRoute.Append(" ");
        }

        /// <summary>
        /// CustomEventID handler.
        /// </summary>
        private void OnCustomEventOnViewer(object sender, RoutedEventArgs args)
        {
            _eventRoute.Append("FDV");
        }

        /// <summary>
        /// CustomEventID handler.
        /// </summary>
        private void OnCustomEventOnEmbeddedViewer(object sender, RoutedEventArgs args)
        {
            _eventRoute.Append("CV ");
        }

        private FlowDocument _document;
        private FlowDocumentReader _readerViewer;
        private Control _embeddedViewer;
        private Frame _frame;
        private StringBuilder _eventRoute;
        private static PropertyInfo s_piCurrentViewer;
    }
}
