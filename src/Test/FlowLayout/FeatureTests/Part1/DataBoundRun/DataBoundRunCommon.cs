// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Data;

namespace Microsoft.Test.FlowLayout
{
    /// <summary>   
    /// Collection of common code used in databound run tests for FlowLayout 
    /// </summary>
    public class DataBoundRunCommon
    {
        private Run _dataBoundRun;
        private Paragraph _paragraph;        

        /// <summary>   
        /// All tests use a Run that has its text source bound to the title of the test Window.
        /// </summary>
        public DataBoundRunCommon(Window testWindow)
        {  
            Binding binding = new Binding("Title");           
            binding.Source = testWindow;
            _dataBoundRun = new Run();
            _dataBoundRun.SetBinding(Run.TextProperty, binding);
        }
             
        public Paragraph DocumentParagraph
        {           
            get
            {
                return _paragraph;
            }
        }                     

        public FrameworkElement CreateDocumentViewerWithBoundedRun(string documentViewerType, string contentBeforeBoundedRun, string contentAfterBoundedRun)
        {
            _paragraph = new Paragraph(new Run(contentBeforeBoundedRun));
            _paragraph.Inlines.Add(_dataBoundRun);
            _paragraph.Inlines.Add(new Run(contentAfterBoundedRun));

            FlowDocument flowDocument = new FlowDocument(_paragraph);           
            FrameworkElement viewer = new FrameworkElement();

            switch(documentViewerType)
            {
                case "FlowDocumentReader":
                    viewer = new FlowDocumentReader();
                    ((FlowDocumentReader)viewer).Document = flowDocument; 
                    break;
                case "FlowDocumentScrollViewer":
                    viewer = new FlowDocumentScrollViewer();
                    ((FlowDocumentScrollViewer)viewer).Document = flowDocument; 
                    break;
                case "FlowDocumentPageViewer":
                    viewer = new FlowDocumentPageViewer();
                    ((FlowDocumentPageViewer)viewer).Document = flowDocument; 
                    break;                
            }                   
            return viewer;            
        }

        public TextSelection GetDocumentSelection(FrameworkElement documentViewer)
        {
            TextSelection textSelection = null;            
            if (documentViewer is FlowDocumentReader)
            {
                textSelection = ((FlowDocumentReader)documentViewer).Selection;
            }
            else if (documentViewer is FlowDocumentPageViewer)
            {
                textSelection = ((FlowDocumentPageViewer)documentViewer).Selection;
            }
            else
            {
                textSelection = ((FlowDocumentScrollViewer)documentViewer).Selection;
            }
            return textSelection;
        }
    }
}