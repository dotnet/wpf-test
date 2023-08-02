// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// 
//
// Description: Test suite for ContentElement eventing. 
//

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;                 // NavigationCommands

namespace DRT
{
    // ----------------------------------------------------------------------
    // Test suite for ContentElement eventing.
    // ----------------------------------------------------------------------
    internal sealed class ContentElementEventsSuite : DrtTestSuite
    {
        // ------------------------------------------------------------------
        // Constructor.
        // ------------------------------------------------------------------
        internal ContentElementEventsSuite() : base("ContentElementEvents")
        {
            this.Contact = "Microsoft";
        }

        // ------------------------------------------------------------------
        // Return the list of individual tests (i.e. callback methods).
        // ------------------------------------------------------------------
        public override DrtTest[] PrepareTests()
        {
            // Initialize the suite here. This includes loading the tree.
            _contentRoot = new Border();
            _contentRoot.Width = 800;
            _contentRoot.Height = 600;
            _contentRoot.Background = Brushes.White;
            Border root = new Border();
            root.Background = Brushes.DarkGray;
            root.Child = _contentRoot;
            DRT.Show(root);

            // Return the lists of tests to run against the tree
            return new DrtTest[] {
                new DrtTest(ViewFlowDocument),                      // View sample document in FlowDocumentReader
                new DrtTest(WaitForPaginationHack),                 // Wait for pagination to happen
                new DrtTest(VerifyEventRoutingOnInvisibleHyperlink),// Scenario1: Events for FCE not displayed on any page.
                new DrtTest(VerifyEventRoutingOnInvisibleButton),   // Scenario1: Events for FE not displayed on any page.
                new DrtTest(VerifyEventRoutingOnVisibleHyperlink),  // Scenario2: Events for FCE in the view.
                new DrtTest(VerifyEventRoutingOnVisibleButton),     // Scenario2: Events for FE in the view.
                new DrtTest(VerifyHittesting),                      // Scenario2: Hittesting for elements in the view.
                new DrtTest(GotoLastPage),                          // Scenario3: Disconnect currently viewed page.
                new DrtTest(VerifyEventRoutingOnInvisibleHyperlink),// Scenario1: Events for FCE not displayed on any page.
                new DrtTest(VerifyEventRoutingOnInvisibleButton),   // Scenario1: Events for FE not displayed on any page.
                new DrtTest(VerifyEventRoutingOnVisibleHyperlink),  // Scenario2: Events for FCE in the view.
                new DrtTest(VerifyEventRoutingOnVisibleButton),     // Scenario2: Events for FE in the view.
                new DrtTest(NavigateToFlowDocument),                // Navigate to sample document
                new DrtTest(WaitForNavigation),                     // Wait for navigation to happen
                new DrtTest(WaitForPaginationHack),                 // Wait for pagination to happen
                new DrtTest(VerifyEventRoutingOnInvisibleHyperlink),// Scenario4.1: Events for FCE not displayed on any page.
                new DrtTest(VerifyEventRoutingOnInvisibleButton),   // Scenario4.1: Events for FE not displayed on any page.
                new DrtTest(VerifyEventRoutingOnVisibleHyperlink),  // Scenario4.2: Events for FCE in the view.
                new DrtTest(VerifyEventRoutingOnVisibleButton),     // Scenario4.2: Events for FE in the view.
                new DrtTest(VerifyHittesting),                      // Scenario4: Hittesting for elements in the view.
                new DrtTest(GotoLastPage),                          // Scenario4.3: Disconnect currently viewed page.
                new DrtTest(VerifyEventRoutingOnInvisibleHyperlink),// Scenario4.1: Events for FCE not displayed on any page.
                new DrtTest(VerifyEventRoutingOnInvisibleButton),   // Scenario4.1: Events for FE not displayed on any page.
                new DrtTest(VerifyEventRoutingOnVisibleHyperlink),  // Scenario4.2: Events for FCE in the view.
                new DrtTest(VerifyEventRoutingOnVisibleButton),     // Scenario4.2: Events for FE in the view.
            };
        }

        // ------------------------------------------------------------------
        // ViewFlowDocument
        // ------------------------------------------------------------------
        private void ViewFlowDocument()
        {
            _testName = "RoutingFlowDocumentReader";
            
            // Load content from xaml file
            _flowDocument = LoadContentFromXaml("FlowDocumentSample1") as FlowDocument;
            DRT.Assert(_flowDocument != null, "{0}: Failed: Expecting FlowDocument as root content.", this.TestName);

            // Setup FlowDocumentReader
            _flowDocumentReader = new FlowDocumentReader();
            _flowDocumentReader.Document = _flowDocument;
            _contentRoot.Child = _flowDocumentReader;
        }

        // ------------------------------------------------------------------
        // NavigateToFlowDocument
        // ------------------------------------------------------------------
        private void NavigateToFlowDocument()
        {
            _testName = "RoutingFrame";

            // Load content from xaml file
            _flowDocument = LoadContentFromXaml("FlowDocumentSample1") as FlowDocument;
            DRT.Assert(_flowDocument != null, "{0}: Failed: Expecting FlowDocument as root content.", this.TestName);

            // Setup Frame
            _frame = new Frame();
            _frame.Navigate(_flowDocument);
            _contentRoot.Child = _frame;
        }

        // ------------------------------------------------------------------
        // WaitForNavigation
        // ------------------------------------------------------------------
        private void WaitForNavigation()
        {
            // Find FlowDocumentReader in the style of Frame
            DependencyObject visual = DRT.FindVisualByType(typeof(FlowDocumentReader), _frame, false);
            _flowDocumentReader = visual as FlowDocumentReader;
            DRT.Assert(_flowDocumentReader != null, "{0}: Failed: Cannot find FlowDocumentReader in the style of Frame.", this.TestName);

            // Force pagination to happen, otherwise FlowDocumentReader will not 
            // show the page. It will wait for BackgroundPagination to finish.
            // But for the DRT it is already too late.
            ((IDocumentPaginatorSource)_flowDocument).DocumentPaginator.GetPage(100);
        }

        // ------------------------------------------------------------------
        // WaitForPaginationHack
        // ------------------------------------------------------------------
        private void WaitForPaginationHack()
        {
            // Force pagination to happen, otherwise FlowDocumentReader will not 
            // show the page. It will wait for BackgroundPagination to finish.
            // But for the DRT it is already too late.
            ((IDocumentPaginatorSource)_flowDocument).DocumentPaginator.GetPage(100);
        }

        // ------------------------------------------------------------------
        // GotoLastPage
        // ------------------------------------------------------------------
        private void GotoLastPage()
        {
            NavigationCommands.LastPage.Execute(null, _flowDocumentReader);
        }

        // ------------------------------------------------------------------
        // VerifyEventRoutingOnInvisibleHyperlink
        // ------------------------------------------------------------------
        private void VerifyEventRoutingOnInvisibleHyperlink()
        {
            VerifyFlowDocumentEventRouting("HL2");
        }

        // ------------------------------------------------------------------
        // VerifyEventRoutingOnInvisibleButton
        // ------------------------------------------------------------------
        private void VerifyEventRoutingOnInvisibleButton()
        {
            VerifyFlowDocumentEventRouting("BTN2");
        }

        // ------------------------------------------------------------------
        // VerifyEventRoutingOnVisibleHyperlink
        // ------------------------------------------------------------------
        private void VerifyEventRoutingOnVisibleHyperlink()
        {
            VerifyFlowDocumentEventRouting("HL1");
        }

        // ------------------------------------------------------------------
        // VerifyEventRoutingOnVisibleButton
        // ------------------------------------------------------------------
        private void VerifyEventRoutingOnVisibleButton()
        {
            VerifyFlowDocumentEventRouting("BTN1");
        }

        // ------------------------------------------------------------------
        // VerifyHittesting
        // ------------------------------------------------------------------
        private void VerifyHittesting()
        {
            IInputElement ie;
            GeneralTransform transform;
            Point ptHyperlink, ptButton;
            Hyperlink hyperlink;
            Button button;
            
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetAssembly(typeof(System.Windows.FrameworkElement));
            Type docPageViewType = assembly.GetType("System.Windows.Controls.Primitives.DocumentPageView");

            // Find DocumentPageView displaying the current page.
            DependencyObject visual = DRT.FindVisualByType(docPageViewType, _flowDocumentReader, false);
            DocumentPageView docPageView = visual as DocumentPageView;
            DRT.Assert(docPageView != null, "{0}: Failed: Cannot find DocumentPageView inside FlowDocumentReader.", this.TestName);
            DocumentPage docPage = docPageView.DocumentPage;
            DRT.Assert(docPage != null, "{0}: Failed: Cannot retrieve DocumentPage.", this.TestName);

            // Find Hyperlink and get its position relative to the root of the test
            hyperlink = DRT.FindElementByID("HL1", _flowDocument) as Hyperlink;
            DRT.Assert(hyperlink != null, "{0}: Failed: Cannot find element: HL1.", this.TestName);
            ReadOnlyCollection<Rect> rects = ((IContentHost)docPage.Visual).GetRectangles(hyperlink);
            DRT.Assert(rects != null && rects.Count > 0, "{0}: Failed: Cannot retrieve rectangles for HL1.", this.TestName);
            ptHyperlink = new Point(rects[0].Left + 5, rects[0].Top + 5);
            transform = docPage.Visual.TransformToAncestor(_contentRoot);
            transform.TryTransform(ptHyperlink, out ptHyperlink);            

            // Find Button and get its position relative to the root of the test
            button = DRT.FindElementByID("BTN1", _flowDocument) as Button;
            DRT.Assert(button != null, "{0}: Failed: Cannot find element: BTN1.", this.TestName);
            ptButton = new Point(5, 5);
            transform = button.TransformToAncestor(_contentRoot);
            transform.TryTransform(ptButton, out ptButton);

            // Hittest Hyperlink
            ie = _contentRoot.InputHitTest(ptHyperlink);
            DRT.Assert((ie is Run) && ((Run)ie).Parent == hyperlink, "{0}: Failed: Hyperlink hittest failed.", this.TestName);

            // Hittest Button
            ie = _contentRoot.InputHitTest(ptButton);
            DRT.Assert(ie != null, "{0}: Failed: Button hittest failed.", this.TestName);
            if (ie != button)
            {
                DRT.Assert(ie is FrameworkElement && ((FrameworkElement)ie).TemplatedParent == button, "{0}: Failed: Button hittest failed.", this.TestName);
            }
        }

        // ------------------------------------------------------------------
        // VerifyFlowDocumentEventRouting
        // ------------------------------------------------------------------
        private void VerifyFlowDocumentEventRouting(string elementID)
        {
            DependencyObject obj;
            FrameworkElement fe;
            TextElement fce;

            ResetEventHandled();

            obj = DRT.FindElementByID(elementID, _flowDocument);
            fe = obj as FrameworkElement;
            fce = obj as TextElement;
            DRT.Assert(fe != null || fce != null, "{0}: Failed: Cannot find element: {1}.", this.TestName, elementID);
            Inline inline = (fe != null ? (fe.Parent as Inline) : (fce as Inline));

            DRT.Assert(inline != null, "{0}: Failed: Cannot find Inline parent of element: {1}.", this.TestName, elementID);

            // Setup event handlers
            if (fe != null)
                fe.AddHandler(CustomEventID, new RoutedEventHandler(_HandleCustomEventOnElement));
            else
                fce.AddHandler(CustomEventID, new RoutedEventHandler(_HandleCustomEventOnElement));
            inline.AddHandler(CustomEventID, new RoutedEventHandler(_HandleCustomEventOnParagraph));
            _flowDocument.AddHandler(CustomEventID, new RoutedEventHandler(_HandleCustomEventOnFlowDocument));
            _flowDocumentReader.AddHandler(CustomEventID, new RoutedEventHandler(_HandleCustomEventOnFlowDocumentReader));
            
            // Raise events
            if (fe != null)
                fe.RaiseEvent(new RoutedEventArgs(CustomEventID));
            else
                fce.RaiseEvent(new RoutedEventArgs(CustomEventID));
            
            // Remove event handlers
            if (fe != null)
                fe.RemoveHandler(CustomEventID, new RoutedEventHandler(_HandleCustomEventOnElement));
            else
                fce.RemoveHandler(CustomEventID, new RoutedEventHandler(_HandleCustomEventOnElement));
            inline.RemoveHandler(CustomEventID, new RoutedEventHandler(_HandleCustomEventOnParagraph));
            _flowDocument.RemoveHandler(CustomEventID, new RoutedEventHandler(_HandleCustomEventOnFlowDocument));
            _flowDocumentReader.RemoveHandler(CustomEventID, new RoutedEventHandler(_HandleCustomEventOnFlowDocumentReader));
            
            // Verify event routing
            DRT.Assert(GetEventHandled(ElementType.Element)       == 1, "{0}: Failed: Notifications received: {1}, expecting: {2} on {3}.", this.TestName, GetEventHandled(ElementType.Element), 1, elementID);
            DRT.Assert(GetEventHandled(ElementType.Paragraph)     == 1, "{0}: Failed: Notifications received: {1}, expecting: {2} on Paragraph.", this.TestName, GetEventHandled(ElementType.Paragraph), 1);
            DRT.Assert(GetEventHandled(ElementType.FlowDocument)  == 1, "{0}: Failed: Notifications received: {1}, expecting: {2} on FlowDocument.", this.TestName, GetEventHandled(ElementType.FlowDocument), 1);
            DRT.Assert(GetEventHandled(ElementType.FlowDocumentReader) == 1, "{0}: Failed: Notifications received: {1}, expecting: {2} on FlowDocumentReader.", this.TestName, GetEventHandled(ElementType.FlowDocumentReader), 1);
        }

        // ------------------------------------------------------------------
        // Load content from xaml file.
        // ------------------------------------------------------------------
        private DependencyObject LoadContentFromXaml(string xamlName)
        {
            DependencyObject content = null;
            System.IO.Stream stream = null;
            string fileName = this.DrtFilesDirectory + xamlName + ".xaml";
            try
            {
                stream = System.IO.File.OpenRead(fileName);
                content = System.Windows.Markup.XamlReader.Load(stream) as DependencyObject;
            }
            finally
            {
                // done with the stream
                if (stream != null) { stream.Close(); }
            }
            DRT.Assert(content != null, "{0}: Failed to load xaml file '{1}'", this.TestName, fileName);
            return content;
        }

        // ------------------------------------------------------------------
        // Routed event helpers
        // ------------------------------------------------------------------
        private void _HandleCustomEventOnFlowDocumentReader(object sender, RoutedEventArgs args)
        {
            SetEventHandled(ElementType.FlowDocumentReader);
        }
        private void _HandleCustomEventOnFlowDocument(object sender, RoutedEventArgs args)
        {
            SetEventHandled(ElementType.FlowDocument);
        }
        private void _HandleCustomEventOnParagraph(object sender, RoutedEventArgs args)
        {
            SetEventHandled(ElementType.Paragraph);
        }
        private void _HandleCustomEventOnElement(object sender, RoutedEventArgs args)
        {
            SetEventHandled(ElementType.Element);
        }
        private void ResetEventHandled()
        {
            _eventHandled = new int[(int)ElementType.Max];
        }
        private void SetEventHandled(ElementType element)
        {
            _eventHandled[(int)element] = _eventHandled[(int)element] + 1;
        }
        private int GetEventHandled(ElementType element)
        {
            return _eventHandled[(int)element];
        }

        // ------------------------------------------------------------------
        // Location of all DRT related files.
        // ------------------------------------------------------------------
        private string DrtFilesDirectory
        {
            get { return DRT.BaseDirectory + "DrtFiles\\FlowLayout\\"; }
        }

        // ------------------------------------------------------------------
        // Unique name of the test.
        // ------------------------------------------------------------------
        private string TestName
        {
            get { return this.Name + _testName; }
        }

        // ------------------------------------------------------------------
        // Event handled array.
        // ------------------------------------------------------------------

        // ------------------------------------------------------------------
        // Custom RoutedEvent.
        // ------------------------------------------------------------------
        internal static readonly RoutedEvent CustomEventID = EventManager.RegisterRoutedEvent("Custom", RoutingStrategy.Bubble, typeof(EventHandler), typeof(DependencyObject));

        // ------------------------------------------------------------------
        // Private fields.
        // ------------------------------------------------------------------
        private string _testName;
        private Border _contentRoot;
        private FlowDocumentReader _flowDocumentReader;
        private Frame _frame;
        private FlowDocument _flowDocument;
        private int[] _eventHandled;

        //-------------------------------------------------------------------
        // Type of elements.
        //-------------------------------------------------------------------
        private enum ElementType
        {
            Element                 = 0,
            Paragraph               = 1,
            FlowDocument            = 2,
            FlowDocumentReader  = 3,
            Max                     = 4,
        }
    }
}
