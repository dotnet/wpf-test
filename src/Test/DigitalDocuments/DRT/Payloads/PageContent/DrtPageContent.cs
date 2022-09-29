// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// Description: DRT for fixed page and fixed panel. 
//

namespace DrtPayloads
{
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.IO;
    using System.Text;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Documents;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Threading;
    using System.Windows.Markup;
    using DOC=System.Windows.Documents;
    using D2Payloads;

    //=====================================================================
    // FixedDocument 
    internal class DrtPageContent : PayloadsTestSuite
    {

        // ------------------------------------------------------------------
        [STAThread]
        internal static int Main(string[] args)
        {
            DrtPageContent driver = new DrtPageContent("DrtPageContent", "ZhenbinX");

            return driver.LaunchDRT(args);
        }

        // ------------------------------------------------------------------
        // Constructor.
        // ------------------------------------------------------------------
        private DrtPageContent(string suiteName, string owner) : base(suiteName, owner)
        {
        }

        public override void PrintUsage()
        {
            Console.WriteLine("  DrtPageContent Test Suite Arguments");
            Console.WriteLine("     /C:1    Verify PageContent Serialization Inline");
            Console.WriteLine("     /C:2    Verify PageContent Serialization External");
            Console.WriteLine("     /C:3    Verify PageContent GetPageRoot API");
            Console.WriteLine("     /C:4    Verify PageContent GetPageRootAsync API");
            Console.WriteLine("     /C:5    Verify PageContent WeakRef");
            Console.WriteLine("");
        }

        public override void QueueTests()
        {
            if (SuiteArgs.Count == 0 || SuiteArgs.Contains("1"))
            {
                PostTask(new DispatcherOperationCallback(TestPageContentSerializationInline));
                PostTask(new DispatcherOperationCallback(VerifyPageContentSerializationInline));
            }

            if (SuiteArgs.Count == 0 || SuiteArgs.Contains("2"))
            {
                PostTask(new DispatcherOperationCallback(TestPageContentSerializationExternal));
                PostTask(new DispatcherOperationCallback(VerifyPageContentSerializationExternal));
            }

            if (SuiteArgs.Count == 0 || SuiteArgs.Contains("3"))
            {
                PostTask(new DispatcherOperationCallback(TestPageContentGetPageRoot));
                PostTask(new DispatcherOperationCallback(VerifyPageContentGetPageRoot));
            }

            if (SuiteArgs.Count == 0 || SuiteArgs.Contains("4"))
            {
                PostTask(new DispatcherOperationCallback(TestPageContentGetPageRootAsync));
            }

            // suite 4 will launch suite 5 in runall mode
            if (SuiteArgs.Count != 0 && 
                !SuiteArgs.Contains("4") 
                && SuiteArgs.Contains("5")
                )
            {
                PostTask(new DispatcherOperationCallback(TestPageContentWeakRef));
            }

            if (SuiteArgs.Count != 0 && !SuiteArgs.Contains("4") && !(SuiteArgs.Contains("5")))
            {
                LogProgressLine("Scheule Shutdown Event On Batch Mode or non Async single Mode");
                PostTask(new DispatcherOperationCallback(ShutDownDRT), false);
            }
        }


        private object TestPageContentSerializationInline(object arg)
        {
            LogProgressLine("TestPageContentSerializationInline");
            object e = LoadXaml(@"DrtFiles\Payloads\PageContent\DrtSS.xaml");
            string serializationString = XamlWriter.Save(e);

            LogVerboseLine("\n");
            LogVerboseLine(serializationString);
            LogVerboseLine("\n");

            MemoryStream m = new MemoryStream();
            StreamWriter sw = new StreamWriter(m);
            sw.Write(serializationString);
            sw.Flush();
            m.Seek(0, SeekOrigin.Begin);
            
            _pageContent = XamlReader.Load(m) as PageContent;
            Assert(_pageContent != null, "serialization error");

            _page = _pageContent.GetPageRoot(true);
            Assert(_page != null, "serialization error, external child page missing after serialization");
            
            AddTree(_page);
            return null;
        }


        private object VerifyPageContentSerializationInline(object arg)
        {
            LogProgressLine("VerifyPageContentSerializationInline");
            return null;
        }


        private object TestPageContentSerializationExternal(object arg)
        {
            LogProgressLine("TestPageContentSerializationExternal");
            object e = LoadXaml(@"DrtFiles\Payloads\PageContent\DrtES.xaml");
            _pageContent = (PageContent)e;
        ((IUriContext)_pageContent).BaseUri = new Uri(@"pack://siteoforigin:,,,/DrtFiles/Payloads/PageContent/DrtEs.xaml", UriKind.RelativeOrAbsolute);
            _page = _pageContent.GetPageRoot(true);
            Assert(_page != null, "serialization error, external child page missing after serialization");

            string serializationString = XamlWriter.Save(e);

            LogVerboseLine("\n");
            LogVerboseLine(serializationString);
            LogVerboseLine("\n");

            MemoryStream m = new MemoryStream();
            StreamWriter sw = new StreamWriter(m);

            sw.Write(serializationString);
            sw.Flush();
            m.Seek(0, SeekOrigin.Begin);
            
            /*
            ParserContext pc = new ParserContext();
            ((IUriContext)pc).BaseUri = new Uri(@"DrtFiles\Payloads\PageContent\", false, true);
            _pageContent = XamlReader.Load(m, null, pc, null, System.Security.Persmissions.FileIOPermission) as PageContent;
            Assert(_pageContent != null, "serialization error");
            _page = _pageContent.GetPageRoot(true);
            */

            AddTree(_page);
            return null;
        }


        private object VerifyPageContentSerializationExternal(object arg)
        {
            LogProgressLine("VerifyPageContentSerializationExternal");
            return null;
        }


        // GetPageRoot
        private object TestPageContentGetPageRoot(object arg)
        {
            LogProgressLine("TestPageContentGetPageRoot");

            _pageContent = new PageContent();
            _pageContent.Source = new Uri(@"pack://siteoforigin:,,,/DrtFiles/Payloads/PageContent/DrtPageContentPage.xaml", UriKind.RelativeOrAbsolute);
            _page = _pageContent.GetPageRoot(true);

            AddTree(_page);
            return null;
        }

        private object VerifyPageContentGetPageRoot(object arg)
        {
            LogProgressLine("VerifyPageContentGetPageRoot");
            return null;
        }

        // GetPageRootAsync
        private object TestPageContentGetPageRootAsync(object arg)
        {
            LogProgressLine("TestPageContentGetPageRootAsync");
            _pageContent = new PageContent();
            _pageContent.Source = new Uri(@"pack://siteoforigin:,,,/DrtFiles/Payloads/PageContent/DrtPageContentPageAsync.xaml", UriKind.RelativeOrAbsolute);
            _pageContent.GetPageRootCompleted += new GetPageRootCompletedEventHandler(OnGetPageRootCompleted);
            _pageContent.GetPageRootAsync(true);
            return null;
        }

        private void OnGetPageRootCompleted(object sender, GetPageRootCompletedEventArgs e)
        {
            LogProgressLine("OnGetPageRootCompleted");
            Assert(sender == _pageContent, "error sender != _pageContent");
            _page = e.Result;
            AddTree(_page);
            PostTask(new DispatcherOperationCallback(VerifyGetPageRootCompleted));
        }


        private object VerifyGetPageRootCompleted(object arg)
        {
            LogProgressLine("VerifyGetPageRootCompleted");

            if (SuiteArgs.Count == 0)
            {
                LogProgressLine("Lauch TestPageContentWeakRef in Batch Mode");
                // launch 5 in runall mode
                PostTask(new DispatcherOperationCallback(TestPageContentWeakRef));
            }
            else
            {
                LogProgressLine("Request Shutdown In Single Test Mode");
                // or quit in run single test mode
                PostTask(new DispatcherOperationCallback(ShutDownDRT), false);
            }
            return null;
        }

        
        private object TestPageContentWeakRef(object arg)
        {
            LogProgressLine("Test PageContent weak ref");

            _pv  = new FixedDocViewer();
            _pv.PaginationCompleted += new EventHandler(OnPaginationCompleted);
            _pv.Source = new FixedDocData(new Uri(System.Environment.CurrentDirectory.ToString() + @"\DrtFiles\Payloads\PageContent\MSNMain.xaml"));

            AddTree(_pv);
            return null;
        }


        private void OnPaginationCompleted(object sender, EventArgs args)
        {
            LogProgressLine("OnPaginationCompleted");
            Assert(_pv.DocumentPaginator.DocumentPaginator.PageCount == 2, "PageCount should be 2");
            _pv.PaginationCompleted -= new EventHandler(OnPaginationCompleted);
            LogProgressLine("Turn to Page 2");
            _pv.MoveToPage(2);
            _pv.PagesDisplayed += new EventHandler(OnPage2Displayed);
        }


        private void OnPage2Displayed(Object sender, EventArgs e)
        {
            LogProgressLine("OnPage2Displayed");
            _pv.PagesDisplayed -= new EventHandler(OnPage2Displayed);
            PostTask(new DispatcherOperationCallback(VerifyPageContentCache), null);
        }

        private object VerifyPageContentCache(object arg)
        {
            LogProgressLine("VerifyPageContentWeakRef");

            // Get the page currenlty in display from PageReference
            FixedDocument fp = (FixedDocument)_pv.Source.FixedDoc;
            FixedPage p = (FixedPage)fp.Pages[1].GetPageRoot(false);
            Assert(p != null, "Page 2 cannot be null");
            LogVerboseLine("Get the page again from FixedDocument");

            // This page should have been a cached page
            Visual v = p;
            while (v != null)
            {
                if (v.GetType() == typeof(FixedDocViewer))
                {
                    break;
                }
                v = (Visual) VisualTreeHelper.GetParent(v);
            }
            Assert(v != null, "FixedPage is not hooked up to PageViewer's visual tree");
            LogVerboseLine("FixedPage identity verified! Good cache maintained by PageContent");

            // Now we put a WeakRefernce on the currently displayed page 
            // Later we are going to verify this FixedPage points to 
            // by the PageReference does not survive a GC.
            _pageRef = new WeakReference(p);
            
            // Now Turn to page 1 so that page 2 is out of display. 
            // Which means no hard reference to page 2 anymore. 
            LogVerboseLine("Turn to Page 1");
            _pv.MoveToPage(1);
            _pv.PagesDisplayed += new EventHandler(OnPage1Displayed);
            return null;
        }

        private void OnPage1Displayed(object sender, EventArgs args)
        {
            // Now page 2 is really out of display. 
            LogProgressLine("OnPage1Displayed");
            _pv.PagesDisplayed -= new EventHandler(OnPage1Displayed);
            PostTask(new DispatcherOperationCallback(ForceGC));
        }

        private object ForceGC(object arg)
        {
            LogProgressLine("ForceGC");
            GC.Collect();
            GC.WaitForPendingFinalizers();
            PostTask(new DispatcherOperationCallback(VerifyPageContentWeakRef), null);
            return null;
        }


        private object VerifyPageContentWeakRef(object arg)
        {
            LogProgressLine("Verify PageContent weak ref");
            if (_pageRef.IsAlive)
            {
                Assert(false, "Some Hard Reference to the page exists!!!");
            }
            else
            {
                LogProgressLine("Page Successfully Collected");
            }
            DelayPostTask(new DispatcherOperationCallback(ShutDownDRT), false, 500);
            return null;
        }

        // ------------------------------------------------------------------
        // Private Fields
        // ------------------------------------------------------------------
        private FixedDocViewer  _pv;
        private PageContent   _pageContent;
        private WeakReference _pageRef;
        private FixedPage _page;
     }
}

