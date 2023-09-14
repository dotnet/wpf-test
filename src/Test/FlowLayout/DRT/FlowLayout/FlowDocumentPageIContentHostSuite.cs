// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// 
//
// Description: Test suite for pagination. 
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DRT
{
    // ----------------------------------------------------------------------
    // Test suite for IContentHost on FlowDocumentPage
    // ----------------------------------------------------------------------
    internal sealed class FlowDocumentPageIContentHostSuite : LayoutSuite
    {
        // ------------------------------------------------------------------
        // Constructor.
        // ------------------------------------------------------------------
        internal FlowDocumentPageIContentHostSuite()
            : base("FlowDocumentPageIContentHost")
        {
            this.Contact = "Microsoft";
        }

        // ------------------------------------------------------------------
        // Initialize tests.
        // ------------------------------------------------------------------
        protected override DrtTest[] CreateTests()
        {
            // Return the lists of tests to run against the tree
            return new DrtTest[] {
                new DrtTest(CreateEmpty),

                // This test resets the name, therefore has own dump file.
                new DrtTest(SetContentForGetRectangles),             new DrtTest(VerifyLayoutCreate),
                new DrtTest(VerifyRectangles),                       new DrtTest(VerifyLayoutFinalize),

                new DrtTest(SetContentForHostedElements),            new DrtTest(VerifyHostedElements),
                new DrtTest(SetContentForOnChildDesiredSizeChanged), new DrtTest(VerifyOnChildDesiredSizeChanged),
                };
        }

        // ------------------------------------------------------------------
        // Load simple content.
        // ------------------------------------------------------------------
        private void CreateEmpty()
        {
            _testName = "";

            _pageView1 = new DocumentPageView();
            _pageView1.PageNumber = 0;
            _pageView2 = new DocumentPageView();
            _pageView2.PageNumber = 1;
            
            _container = new Grid();
            _container.Background = Brushes.LightGreen;
            _container.Children.Add(_pageView1);
            _container.Children.Add(_pageView2);
            Grid.SetColumn(_pageView1, 0);
            _pageView1.HorizontalAlignment = HorizontalAlignment.Left;
            _pageView1.VerticalAlignment = VerticalAlignment.Top;

            Grid.SetColumn(_pageView2, 1);
            _pageView2.HorizontalAlignment = HorizontalAlignment.Right;
            _pageView2.VerticalAlignment = VerticalAlignment.Bottom;

            _contentRoot.Child = _container;
        }

        /// <summary>
        /// Load Xaml file with content for GetRectangles
        /// </summary>
        private void SetContentForGetRectangles()
        {
            _testName = "Rectangles";
            FlowDocument document = LoadFromXaml(this.TestName + ".xaml") as FlowDocument;
            DRT.Assert(document != null, this.TestName + ": Failed to load from xaml file.");
            _paginator = ((IDocumentPaginatorSource)document).DocumentPaginator as DynamicDocumentPaginator;
            _paginator.PageSize = new Size(700, 1400);
            _pageView1.DocumentPaginator = _paginator;
            _pageView2.DocumentPaginator = _paginator;
        }

        /// <summary>
        /// Load Xaml file with content for HostedElements
        /// </summary>
        private void SetContentForHostedElements()
        {
            _testName = "HostedElements";
            FlowDocument document = LoadFromXaml(this.TestName + ".xaml") as FlowDocument;
            DRT.Assert(document != null, this.TestName + ": Failed to load from xaml file.");
            _paginator = ((IDocumentPaginatorSource)document).DocumentPaginator as DynamicDocumentPaginator;
            _paginator.PageSize = new Size(700, 1000);
            _pageView1.DocumentPaginator = _paginator;
        }

        private void SetContentForOnChildDesiredSizeChanged()
        {
            _testName = "OnChildDesiredSizeChanged";
            FlowDocument document = LoadFromXaml(this.TestName + ".xaml") as FlowDocument;
            DRT.Assert(document != null, this.TestName + ": Failed to load from xaml file.");
            _paginator = ((IDocumentPaginatorSource)document).DocumentPaginator as DynamicDocumentPaginator;
            _paginator.PageSize = new Size(200, 200);
            _paginator.PagesChanged += new PagesChangedEventHandler(OnPagesChanged);
            _pageView1.DocumentPaginator = _paginator;
            _pageView2.DocumentPaginator = _paginator;
        }

        // ------------------------------------------------------------------
        // Verify content 
        // ------------------------------------------------------------------

        /// <summary>
        /// Verify GetRectangles functioning
        /// </summary>
        private void VerifyRectangles()
        {
            DocumentPage documentPage = _pageView1.DocumentPage;
            DRT.Assert(documentPage != DocumentPage.Missing);

            XmlTextWriter xmlTextWriter = OpenDumpFile(TestXmlFileName, true);

            xmlTextWriter.WriteStartElement("RectangleDump");

            DumpHostedElement("para1", typeof(Paragraph), (IContentHost)documentPage, (DependencyObject)_paginator.Source, xmlTextWriter);
            DumpHostedElement("para2", typeof(Paragraph), (IContentHost)documentPage, (DependencyObject)_paginator.Source, xmlTextWriter);
            DumpHostedElement("bold1", typeof(Bold), (IContentHost)documentPage, (DependencyObject)_paginator.Source, xmlTextWriter);
            DumpHostedElement("lb1",   typeof(LineBreak), (IContentHost)documentPage, (DependencyObject)_paginator.Source, xmlTextWriter);
            DumpHostedElement("list1", typeof(System.Windows.Documents.List), (IContentHost)documentPage, (DependencyObject)_paginator.Source, xmlTextWriter);
            DumpHostedElement("li1",   typeof(ListItem), (IContentHost)documentPage,  (DependencyObject)_paginator.Source, xmlTextWriter);
            DumpHostedElement("figure1", typeof(Figure), (IContentHost)documentPage,  (DependencyObject)_paginator.Source, xmlTextWriter);
            DumpHostedElement("floater1", typeof(Floater), (IContentHost)documentPage,(DependencyObject)_paginator.Source, xmlTextWriter);
            DumpHostedElement("table1", typeof(Table), (IContentHost)documentPage,    (DependencyObject)_paginator.Source, xmlTextWriter);
            DumpHostedElement("cell1", typeof(TableCell), (IContentHost)documentPage, (DependencyObject)_paginator.Source, xmlTextWriter);
            DumpHostedElement("cell2", typeof(TableCell), (IContentHost)documentPage, (DependencyObject)_paginator.Source, xmlTextWriter);

            xmlTextWriter.WriteEndElement();

            xmlTextWriter.Flush();
            xmlTextWriter.Close();
        }

        /// <summary>
        /// Verify HostedElements functioning
        /// </summary>
        private void VerifyHostedElements()
        {
            DocumentPage documentPage = _pageView1.DocumentPage;
            DRT.Assert(documentPage != DocumentPage.Missing);

            IEnumerator<IInputElement> hostedElements = ((IContentHost)documentPage).HostedElements;
            DRT.Assert(hostedElements != null);

            DRT.Assert(hostedElements.MoveNext());
            IInputElement element = hostedElements.Current as IInputElement;
            DRT.Assert(element is Paragraph);
            Paragraph para1 = (Paragraph)DRT.FindElementByID("para1", (DependencyObject)_paginator.Source);
            DRT.Assert(para1 != null);
            DRT.Assert((Paragraph)element == para1);

            DRT.Assert(hostedElements.MoveNext());
            element = hostedElements.Current as IInputElement;
            DRT.Assert(element is Run);
            DRT.Assert(((Run)element).Text.StartsWith("Early one day I met Pichet Ong"));

            DRT.Assert(hostedElements.MoveNext());
            element = hostedElements.Current as IInputElement;
            DRT.Assert(element is InlineUIContainer);

            DRT.Assert(hostedElements.MoveNext());
            element = hostedElements.Current as IInputElement;
            DRT.Assert(element is TextBlock);
            TextBlock text1 = (TextBlock)DRT.FindElementByID("text1", (DependencyObject)_paginator.Source);
            DRT.Assert(text1 != null);
            DRT.Assert((TextBlock)element == text1);
            DRT.Assert(((TextBlock)element).Text.StartsWith("A good point"));

            DRT.Assert(hostedElements.MoveNext());
            element = hostedElements.Current as IInputElement;
            DRT.Assert(element is Paragraph);
            Paragraph para2 = (Paragraph)DRT.FindElementByID("para2", (DependencyObject)_paginator.Source);
            DRT.Assert(para2 != null);
            DRT.Assert((Paragraph)element == para2);

            DRT.Assert(hostedElements.MoveNext());
            element = hostedElements.Current as IInputElement;
            DRT.Assert(element is Run);
            DRT.Assert(((Run)element).Text.StartsWith("To that end"));

            DRT.Assert(hostedElements.MoveNext());
            element = hostedElements.Current as IInputElement;
            DRT.Assert(element is Bold);
            Bold bold1 = (Bold)DRT.FindElementByID("bold1", (DependencyObject)_paginator.Source);
            DRT.Assert(bold1 != null);
            DRT.Assert((Bold)element == bold1);

            DRT.Assert(hostedElements.MoveNext());
            element = hostedElements.Current as IInputElement;
            DRT.Assert(element is Run);
            DRT.Assert(((Run)element).Text.StartsWith("\"I've done all kinds of coffee recipes"));

            DRT.Assert(hostedElements.MoveNext());
            element = hostedElements.Current as IInputElement;
            DRT.Assert(element is Run);
            DRT.Assert(((Run)element).Text.StartsWith(" he said."));

            DRT.Assert(hostedElements.MoveNext());
            element = hostedElements.Current as IInputElement;
            DRT.Assert(element is Paragraph);
            Paragraph para3 = (Paragraph)DRT.FindElementByID("para3", (DependencyObject)_paginator.Source);
            DRT.Assert(para3 != null);
            DRT.Assert((Paragraph)element == para3);

            DRT.Assert(hostedElements.MoveNext());
            element = hostedElements.Current as IInputElement;
            DRT.Assert(element is InlineUIContainer);

            DRT.Assert(hostedElements.MoveNext());
            element = hostedElements.Current as IInputElement;
            DRT.Assert(element is TextBox);
            TextBox box1 = (TextBox)DRT.FindElementByID("box1", (DependencyObject)_paginator.Source);
            DRT.Assert(box1 != null);
            DRT.Assert((TextBox)element == box1);
            DRT.Assert(((TextBox)element).Text.StartsWith("\"Traditionally you'd find tapioca in sweet soups,\""));

            DRT.Assert(hostedElements.MoveNext());
            element = hostedElements.Current as IInputElement;
            DRT.Assert(element is Run);
            DRT.Assert(((Run)element).Text.StartsWith(" \"I thought about other coffee beverage desserts"));

            DRT.Assert(!hostedElements.MoveNext());
        }

        private void VerifyOnChildDesiredSizeChanged()
        {
            // Get first page
            DocumentPage documentPage = _pageView1.DocumentPage;
            DRT.Assert(documentPage != DocumentPage.Missing);

            // Get TextBox from first page
            TextBox box1 = (TextBox)DRT.FindElementByID("box1", (DependencyObject)_paginator.Source);
            DRT.Assert(box1 != null);
           
            // Resize
            box1.Width = 10;
            box1.Height = 15;
            ((IContentHost)documentPage).OnChildDesiredSizeChanged(box1);
        }

        // ------------------------------------------------------------------
        // OnPagesChanged
        // ------------------------------------------------------------------
        private void OnPagesChanged(object sender, PagesChangedEventArgs e)
        {
        }

        // ------------------------------------------------------------------
        // Private fields.
        // ------------------------------------------------------------------
        private DynamicDocumentPaginator _paginator;
        private DocumentPageView _pageView1;
        private DocumentPageView _pageView2;
        Grid _container;
    }
}
