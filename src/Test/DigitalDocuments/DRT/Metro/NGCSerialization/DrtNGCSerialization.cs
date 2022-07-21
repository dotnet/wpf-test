// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Threading;
using System.IO.Packaging;
using System.Collections;
using System.Collections.Specialized;

using System.Windows;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Xps.Packaging;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Controls;
using System.Windows.Shapes;

using System.Printing;
using System.Windows.Xps;
using DRT;

namespace DrtPrinting
{
    internal class DrtNGCTest : DrtBase
    {
       
        [STAThread]
        static int Main(String[] args)
        {
            DrtBase drt = new DrtNGCTest();
            return drt.Run(args);
        }

        // ------------------------------------------------------------------
        // Constructor.
        // ------------------------------------------------------------------
        private DrtNGCTest()
        {
            WindowTitle = "Serialization Test";
            TeamContact = "WPF";
            Contact = "Microsoft";
            DrtName = "DrtNGCTest";
            Suites = new DrtTestSuite[]{
                        new DrtNGCTestSuite(),
                        null            // list terminator - optional
                        };
        }

        // Override this in derived classes to handle command-line arguments one-by-one.
        // Return true if handled.
        protected override bool HandleCommandLineArgument(string arg, bool option, string[] args, ref int k)
        {
            // start by giving the base class the first chance
            if (base.HandleCommandLineArgument(arg, option, args, ref k))
                return true;

            // process your own arguments here, using these parameters:
            //      arg     - current argument
            //      option  - true if there was a leading - or /.
            //      args    - all arguments
            //      k       - current index into args
            // Here's a typical sketch:

            if (option)
            {
                switch (arg)    // arg is lower-case, no leading - or /
                {
                    case "pq":             // option with parameter:  -use something
                        _printerName = args[++k];
                        break;

                    case "in":             // option with parameter:  -use something
                        _inFileName = args[++k];
                        break;

                    case "out":             // option with parameter:  -use something
                        _outFileName = args[++k];
                        break;

                    default:                // unknown option.  don't handle it
                        return false;
                }
                return true;
            }
            return false;
        }

        protected override void PrintOptions()
        {
            Console.WriteLine("Next Generation Converter Test Suite Arguments");
            Console.WriteLine("     /IN InputFile   The input XAML file. Use hand crafted visual if not specified.");
            Console.WriteLine("     /OUT OutputFile The output container. Use %InputFile%.xps if not specified.");
            Console.WriteLine("     /PQ QueueName   The printer queue name. Serialize to container file if not specified.");
            Console.WriteLine("");

            // 1. Write hand crafed FixedDocument
            //  drtngctest 
            // 2. Write Visual
            //  drtNgcTest /IN TestVisualPrint.xaml  
            // 3. Write FixedDocumentSequnce
            //  drtNgcTest /IN drtfiles\Payloads\Sequence\MSFTQ3-04.xaml  
            // 4. Write FixedDocument
            //  drtNgcTest /IN  DrtFixedEdit_Paginated.xaml
            //  drtNgcTest /IN drtfiles\Payloads\Sequence\MSFTQ3-04Doc.xaml  
            //  drtNgcTest /IN drtfiles\Payloads\Sequence\MSFTQ3-04xls.xaml
            // 5. Write IDP (flow)
            //  drtNgcTest /IN  constitution.xaml 
            //  Note: constitution.xaml contains some flow hyperlink and it will be serialized into fixed hyperlink
            // 6. Write FixedPage
            //  drtNgcTest /IN  DrtFiles\Payloads\PageContent\MSNMain_page1.xaml

            // Note: Add "/pq MODI" if you want print to printer, otherwise it outputs to test.xps.

            // Existing Problems:
            // 1. (external) Sometimes, MODI printer driver fail to write files to the redirected "My Documents"
            //    folder probably due to some security implication. For example, it always fail to
            //    create file from DrtFixedEdit_Paginated.xaml. 
            //    workaround: Writing to the local folder.
            // 2. (internal) For pps fixed document / sequence, sometimes only the first page is now visible.
            //     Cause: Although we run the layout for each page before printing/serialization,
            //      but the PageContent only store the the weakReference visual.
            //      The serialization code does not call GetPage (return DocumentPage), instead it call GetPageSync()
            //      directly, which load the page back but not running any layout.
            //      This should happen to both print and saveas.
            //      One Fix: Make serialization code calling GetPage.
            // 3. (internal) Print: For fixed documente sequence, each FD triggers a print job.
            //               SaveAs: For fixed documente sequence, it output empty FixedDocument for the second FD.
        }

        new public object LoadXamlFile(string filename)
        {
            // Set the baseUri of ParserContext
            FileInfo info = new FileInfo(filename);
            Uri baseUri = new Uri(info.FullName);
            ParserContext pc = new ParserContext();
            pc.BaseUri = baseUri;

            System.IO.Stream stream = File.OpenRead(filename);

            return XamlReader.Load(stream, pc);
        }

        public object LoadPackage(string filename)
        {
            Console.WriteLine("This function will be completed in M11. Loading container to get object ");
            Console.WriteLine("In M10, loading container with all resources is not trivial");

            object element = null;
            
            return element;
        }
                    
        public String InFileName
        {
            get { return _inFileName; }
        }
        public String PrinterName
        {
            get {return _printerName; }
        }
        public String OutFileName
        {
            get { return _outFileName; }
        }

        
        private String _printerName = null;
        private String _inFileName;
        private String _outFileName;
    }


    public sealed class DrtNGCTestSuite : DrtTestSuite 
    {

        internal static string OutputReachFile = "MyTest.xps";

        public DrtNGCTestSuite()
            : base("NGC Serialization Test")
        {
            TeamContact = "WPF";
            Contact = "Microsoft";
        }


        public override DrtTest[] PrepareTests()
        {
            //
            // Prepare the test matrix
            //
            DrtTest[] drtTests = new DrtTest[1];
            drtTests[0] = null;
            int testIndex = 0;

            //
            // Prepare the print queue. 
            //
            DrtNGCTest drtNgc = DRT as DrtNGCTest;

            if (drtNgc.PrinterName != null)
            {
                GetPrintQueue(drtNgc.PrinterName);
                if (_pq == null)
                {
                    return drtTests;
                }

                Console.WriteLine("Printer {0} will be used in the test", _pq.Name);
            }
    
            //
            // Prepare the input avalon tree, we load the files here, otherwise we might process the
            // the document before parser finish loading document asynchronously.
            //
            if (drtNgc.InFileName != null)
            {
                String extension = System.IO.Path.GetExtension(drtNgc.InFileName);
                if (String.Compare(extension, ".xps", true) == 0 || 
                    String.Compare(extension, ".zip", true) == 0  )
                {
                    _rootElement = drtNgc.LoadPackage(drtNgc.InFileName);
                }
                else  // if (String.Compare(extension, ".xaml", true) == 0)
                {
                    _rootElement = drtNgc.LoadXamlFile(drtNgc.InFileName);
                }
            }
            else
            {
                // hand crafted document
                _rootElement = CreateFixedDocumentPaginator();
            }

            if (_rootElement == null)
            {
                DRT.Assert(false, "Fail to get the input avalon tree");
                return drtTests;
            }

            drtTests[testIndex++] = new DrtTest(StartTestSerialization);

            return drtTests;
        }

        private bool IsInitialized()
        {
            FrameworkElement fe = _rootElement as FrameworkElement;
            if (fe != null)
                return fe.IsInitialized ;
            else
            {
                FrameworkContentElement fce = _rootElement as FrameworkContentElement;
                if (fce != null)
                    return fce.IsInitialized;
            }

            return true;
        }

        private void UpdateLayout(object o)
        {
            Size sz = new Size(8.5 * 96, 11 * 96);

            UIElement element = o as UIElement;
            if (element != null)
            {
                element.Measure(sz);
                element.Arrange((new Rect((new Point()), sz)));
                element.UpdateLayout();
            }
        }

        private void PreprocessIDP(IDocumentPaginatorSource idp)
        {
            DocumentPaginator paginator = idp.DocumentPaginator;

            for (int i = 0; !paginator.IsPageCountValid || (i < paginator.PageCount); i++)
            {
                DocumentPage page = paginator.GetPage(i);
                UpdateLayout(page.Visual);
                if (page == DocumentPage.Missing)
                { 
                    break;
                }
            }
        }

        static void SetEllipse(Ellipse shape, double cx, double cy, double rx, double ry)
        {
            Thickness thick = new Thickness();
            thick.Left = cx - rx;
            thick.Top = cy - ry;

            shape.Margin = thick;
            shape.Width = rx * 2;
            shape.Height = ry * 2;
        }


        private
        void
        StartTestSerialization(
            )
        {
            DrtNGCTest drtNgc = DRT as DrtNGCTest;

            XpsDocumentWriter pad;
            XpsDocument rp = null;
            Package package = null;
            if (_pq == null)
            {
                String outputContainer = drtNgc.OutFileName;
                if (outputContainer == null)
                {
                    if (drtNgc.InFileName == null)
                    {
                        outputContainer = OutputReachFile;
                    }
                    else
                    {
                        outputContainer = System.IO.Path.ChangeExtension(drtNgc.InFileName, ".xps");
                    }
                }
                package = Package.Open(outputContainer, FileMode.Create);
                rp = new XpsDocument(package);
                pad = XpsDocument.CreateXpsDocumentWriter(rp);
            }
            else
            {
                pad = PrintQueue.CreateXpsDocumentWriter(_pq);
            }

            try
            {

                // do some prepare work.
                UpdateLayout(_rootElement);

                // let's start!
                ContentControl cc = _rootElement as ContentControl;
                if (cc != null)
                {
                    IDocumentPaginatorSource idp = cc.Content as IDocumentPaginatorSource;

                    // hope this content is a IDP
                    if (idp != null)
                    {
                        PreprocessIDP(idp);
                        if (idp is FixedDocument)
                        {
                            pad.Write((FixedDocument)idp);
                        }
                        else if (idp is FixedDocumentSequence)
                        {
                            pad.Write((FixedDocumentSequence)idp);
                        }
                        else
                        {
                            pad.Write(idp.DocumentPaginator);
                        }
                    }
                    else
                    {
                        Console.WriteLine("The content of the ContentControl is not IDP");
                    }
                    return;
                }

                FixedDocumentSequence fds = _rootElement as FixedDocumentSequence;
                if (fds != null)
                {
                    PreprocessIDP((IDocumentPaginatorSource)fds);
                    pad.Write(fds);
                    return;
                }

                FixedDocument fd = _rootElement as FixedDocument;
                if (fd != null)
                {
                    PreprocessIDP((IDocumentPaginatorSource)fd);
                    pad.Write(fd);
                    return;
                }

                FixedPage fp = _rootElement as FixedPage;
                if (fp != null)
                {
                    pad.Write(fp);
                    return;
                }

                IDocumentPaginatorSource idp1 = _rootElement as IDocumentPaginatorSource;
                if (idp1 != null)
                {
                    PreprocessIDP(idp1);
                    pad.Write(idp1.DocumentPaginator);
                    return;
                }

                Visual visual = _rootElement as Visual;
                if (visual != null)
                {
                    pad.Write(visual);
                    return;
                }
            }
            finally
            {
                if (rp != null)
                    rp.Close(); 
                if( package != null )
                {
                    package.Close();
                }
            }

            return;
        }

        private
        FixedDocument
        CreateFixedDocumentPaginator(
        )
        {

            FixedDocument testFixedPanel = new FixedDocument();

            double pageWidth = 200.0;
            double pageHeight = 200.0 * 1.618;

            PageContent pageContent = CreateFixedTextPage();
            
            FixedPage pageVisual = pageContent.GetPageRoot(false);

            pageVisual.Width = pageWidth;
            pageVisual.Height = pageHeight;
            testFixedPanel.Pages.Add(pageContent);

            PageContent anotherPageContent = CreateFixedTextPage();
            FixedPage anotherPageVisual = anotherPageContent.GetPageRoot(false);
            anotherPageVisual.Width = pageWidth;
            anotherPageVisual.Height = pageHeight;
            testFixedPanel.Pages.Add(anotherPageContent);

            return testFixedPanel;
        }

        private
        PageContent
        CreateFixedTextPage(
            )
        {
            PageContent pageContent  = new PageContent();
            FixedPage fixedPage  = new FixedPage();

            Canvas canvas1 = new Canvas();

            // Top-Left
            TextBlock label = new TextBlock();
            label.Foreground = Brushes.Red;
            label.Text = "TopLeft";
            Canvas.SetTop(label, 100);
            Canvas.SetLeft(label, 0);
            canvas1.Children.Add(label);

            // Bottom-Right
            label = new TextBlock();
            label.Foreground = Brushes.Green;
            label.Text = "BottomRight";
            Canvas.SetBottom(label, 0);
            Canvas.SetRight(label, 0);
            canvas1.Children.Add(label);

            // Top-Right
            label = new TextBlock();
            label.Foreground = Brushes.Blue;
            label.Text = "TopRight";
            Canvas.SetTop(label, 0);
            Canvas.SetRight(label,0);
            canvas1.Children.Add(label);

            // Bottom-Left
            label = new TextBlock(); 
            label.Foreground = Brushes.Cyan;
            label.Text = "BottomLeft";
            Canvas.SetBottom(label, 0);
            Canvas.SetLeft(label, 0);
            canvas1.Children.Add(label);

            //
            // Adding a rectangle to the page
            //
            Rectangle firstRectangle = new Rectangle();
            firstRectangle.Fill = new SolidColorBrush(Colors.Blue);
            Thickness thick = new Thickness();
            thick.Left = 100;
            thick.Top  = 100;
            firstRectangle.Margin = thick;
            firstRectangle.Width  = 100;
            firstRectangle.Height = 100;


            // ((IAddChild)fixedPage).AddChild(firstRectangle);
            canvas1.Children.Add(firstRectangle);

            //
            //Adding a button to the page
            //
            Button firstButton = new Button();
            firstButton.Background = Brushes.Red;
            firstButton.BorderBrush = new SolidColorBrush(Colors.Black);
            firstButton.BorderThickness = (new Thickness(2));
            firstButton.Content = "I am button 1...";
            firstButton.FontSize = 16;

            canvas1.Children.Add(firstButton);

            //
            // Adding an Ellipse
            //
            Ellipse firstEllipse = new Ellipse();
            SolidColorBrush firstSolidColorBrush = new SolidColorBrush(Colors.DarkCyan);
            firstSolidColorBrush.Opacity = 0.5;
            firstEllipse.Fill = firstSolidColorBrush;
            SetEllipse(firstEllipse, 200, 200, 100, 100);

            canvas1.Children.Add(firstEllipse);

            fixedPage.Children.Add(canvas1);

            Size sz = new Size(8.5 * 96, 11 * 96);

            fixedPage.Measure(sz);
            fixedPage.Arrange((new Rect((new Point()), sz)));
            fixedPage.UpdateLayout();


            //_label = label;
            ((IAddChild)pageContent).AddChild(fixedPage);
            
            return pageContent;
            }

        private
        void
        GetPrintQueue(
            string printerName
            )
        {
            if (printerName != null)
            {
                try
                {
                    _pq = new PrintQueue(new LocalPrintServer(), printerName);
                }
                catch (PrintQueueException)
                {
                     Console.WriteLine("{0} is not a valid printer queue name.", printerName);
                      _pq = null;
                }
            }
            else
            {
                LocalPrintServer lps = new LocalPrintServer();
                PrintQueue pq = null;

                PrintQueueCollection lpCollection = lps.GetPrintQueues();
                IEnumerator ienum = lpCollection.GetEnumerator();

                while (ienum.MoveNext())
                {
                    pq = (PrintQueue)ienum.Current;

                    //if (pq.IsMetroDevice &&  // is local queue
                    {
                        _pq = pq;
                        break;
                    }
                }
            }

        }

        private PrintQueue _pq;
        private object  _rootElement;

    }
};


