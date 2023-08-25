// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// Copyright (C) Microsoft Corporation.  All rights reserved.
// Description: DRT for fixed page and fixed panel. 
//

namespace DrtPayloads
{
    using System;
    using System.IO;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Documents;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Threading;
    using System.Windows.Markup;
    using System.Windows.Input;
    using System.Windows.Interop;
    using System.Windows.Automation;
    using DOC=System.Windows.Documents;
    using DRT;
    using System.Text;

    //=====================================================================
    // Sequence 
    internal class DrtSequence : PayloadsTestSuite
    {
        // ------------------------------------------------------------------
        [STAThread]
        internal static int Main(string[] args)
        {
            DrtSequence driver = new DrtSequence("DrtSequence", "Microsoft");
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            return driver.LaunchDRT(args); 
        }

        // ------------------------------------------------------------------
        // Constructor.
        // ------------------------------------------------------------------
        private DrtSequence(string suiteName, string owner) : base(suiteName, owner)
        {}

        public override void PrintUsage()
        {
            Console.WriteLine("  DrtSequence Test Suite Arguments");
            Console.WriteLine("     /C:1    Verify Sequence ContentAPI");
            Console.WriteLine("     /C:2    Verify Sequence Fixed Selection at End of Document");
            Console.WriteLine("     /C:3    Verify Sequence Word Selection");
            Console.WriteLine("     /C:4    Verify Sequence Keyboard Selection"); // eventually across pages
            Console.WriteLine("     /C:5    Verify Sequence Select All");
            Console.WriteLine("");
        }

        public override void QueueTests()
        {
            if (SuiteArgs.Count == 0 || SuiteArgs.Contains("1"))
            {
                PostTask(new DispatcherOperationCallback(TestSequenceContentAPI));
            }
            else
            {
                PostTask(new DispatcherOperationCallback(TestSequenceFixedStitching));
            }
        }


        private object TestSequenceContentAPI(object arg)
        {
            LogProgressLine("TestSequenceContentAPI");

            // Add content
            _documentSequence = new FixedDocumentSequence();
            DocumentReference doc;

            FileInfo info = new FileInfo(@"DrtFiles\Payloads\Sequence\SampleDoc.xaml");
            Uri baseUri = new Uri(info.DirectoryName + @"\");

            doc = new DocumentReference();
            doc.Source = new Uri(@"SampleDoc.xaml", UriKind.RelativeOrAbsolute);
            ((IUriContext)doc).BaseUri = baseUri;
            _documentSequence.References.Add(doc);

            doc = new DocumentReference();
            doc.Source = new Uri(@"SampleXls.xaml", UriKind.RelativeOrAbsolute);
            ((IUriContext)doc).BaseUri = baseUri;
            _documentSequence.References.Add(doc);

            // Verify parent/child relationship
            IEnumerator ienum = LogicalTreeHelper.GetChildren(_documentSequence).GetEnumerator();
            int count = 0;
            while (ienum.MoveNext())
            {
                count++;
                doc = (DocumentReference)ienum.Current;
                DrtVerify("TestSequenceContentAPI", "Failed Logical Tree Parent/Child", _documentSequence, LogicalTreeHelper.GetParent(doc));
            }
            DrtVerify("TestSequenceContentAPI", "Logical Tree incorrect child count", 2, count);


            // Launch next test in non-batch mode
            if (SuiteArgs.Count == 0)
            {
                PostTask(new DispatcherOperationCallback(TestSequenceFixedStitching));
            }
            else
            {
                PostTask(new DispatcherOperationCallback(ShutDownDRT), false);
            }
            return null;
        }


        private object TestSequenceFixedStitching(object arg)
        {
            LogProgressLine("TestSequenceFixedStitching");

#if USINGFIXEDVIEWER
                        _dv  = new FixedDocViewer();
#else
            _dv = new DocumentViewer();
#endif
#if USINGFIXEDVIEWER
            _dv.Source = new FixedDocData(new Uri(System.Environment.CurrentDirectory.ToString() + @"\DrtFiles\Payloads\Sequence\ForTesting.xaml", false, true));
#else
            _dv.Document = LoadXaml(@"DrtFiles\Payloads\Sequence\ForTesting.xaml") as System.Windows.Documents.IDocumentPaginatorSource;
#endif

            // Caret has to look for adorner
            AdornerDecorator ad = new AdornerDecorator();
            ad.Child = _dv;
            AddTree(ad);

            if (_dv.Document.DocumentPaginator.IsPageCountValid)
            {
                OnFixedPaginationCompleted(null, null);
            }
            else
            {
                ((DynamicDocumentPaginator)_dv.Document.DocumentPaginator).PaginationCompleted += new EventHandler(OnFixedPaginationCompleted);
            }
            return null;
        }


        private void OnFixedPaginationCompleted(Object sender, EventArgs e)
        {
            LogProgressLine("OnFixedPaginationCompleted");

            _dv.FitToMaxPagesAcross(1);  // Set WholePage view.

            if (SuiteArgs.Count != 0 && !SuiteArgs.Contains("2"))
            {
                //not running in batch mode, not running this test, need to determine next test
                if (SuiteArgs.Contains("3"))
                {
                    PostTask(new DispatcherOperationCallback(TestSequenceSelectWord));
                }
                else if (SuiteArgs.Contains("4"))
                {
                    PostTask(new DispatcherOperationCallback(TestSequenceSelectAcrossPages));
                }
                else //(SuiteArgs.Contains("5"))
                {
                    PostTask(new DispatcherOperationCallback(TestSequenceSelectAll));
                }
            }
            else
            {
                DelayPostTask(new DispatcherOperationCallback(FixedTurnToPage), 4, 1500);
            }
        }


        private object FixedTurnToPage(object arg)
        {
            LogProgressLine("FixedTurnToPage");
            TurnToPage(5);
            DelayPostTask(new DispatcherOperationCallback(TestSequenceSelectEndOfDocument), null, 50);
            return null;
        }


        private object TurnToPage(object arg)
        {
            LogProgressLine(string.Format("TurnToPage {0}", (int)arg));
#if USINGFIXEDVIEWER        
            _dv.MoveToPage((int)arg); 
#else
            NavigationCommands.GoToPage.Execute(arg, _dv); //1-indexed.
#endif             
            return null;
        }

        private object TestSequenceSelectEndOfDocument(object arg)
        {
            LogProgressLine("TestSequenceSelectEndOfDocument");

            FrameworkElement fe = FindFixedPage(_dv);

            if (fe == null)
            {
                _retries++;
                DrtVerify("TestSequenceSelectEndOfDocument", "time-out on getting FixedPage, retries must be less than 50", true, _retries < 50);
                DelayPostTask(new DispatcherOperationCallback(TestSequenceSelectEndOfDocument), null, 50);
                return null;
            }

            MoveMouse(fe, .02, .8);
            Input.SendMouseInput(0, 0, 0, SendMouseInputFlags.LeftDown);
            Input.SendMouseInput(0, 0, 0, SendMouseInputFlags.LeftUp);
            MoveMouse(fe, .98, .99);
            Input.SendKeyboardInput(Key.LeftShift, true);
            Input.SendMouseInput(0, 0, 0, SendMouseInputFlags.LeftDown);
            Input.SendMouseInput(0, 0, 0, SendMouseInputFlags.LeftUp);
            Input.SendKeyboardInput(Key.LeftShift, false);

            CopySelection();

            PostTask(new DispatcherOperationCallback(VerifySequenceSelectEndOfDocument));
            return null;
        }

        private object VerifySequenceSelectEndOfDocument(object arg)
        {
            LogProgressLine("VerifySequenceSelectEndOfDocument");

            // Check what we copied
            String copied = (String)Clipboard.GetDataObject().GetData(typeof(String));

            DrtVerify("VerifySequenceSelectEndOfDocument", "0copied contains...", true, copied.Contains("Total stockholders' equity"));
            DrtVerify("VerifySequenceSelectEndOfDocument", "1copied contains...", true, copied.Contains("01/15/2034/7:35 AM"));
            // Launch next test if in batch mode
            if (SuiteArgs.Count == 0)
            {
                PostTask(new DispatcherOperationCallback(TestSequenceSelectWord));
            }
            else
            {
                PostTask(new DispatcherOperationCallback(ShutDownDRT), false);
            }
            return null;
        }

        private object TestSequenceSelectWord(object arg)
        {
            LogProgressLine("TestSequenceSelectWord");
            TurnToPage(2);
            DelayPostTask(new DispatcherOperationCallback(DoubleClickWord), null, 50);
            return null;
        }

        private object DoubleClickWord(object arg)
        {
            LogProgressLine("DoubleClickWord");

            FrameworkElement fe = FindFixedPage(_dv);

            if (fe == null)
            {
                _retries++;
                DrtVerify("DoubleClickWord", "time-out on getting FixedPage, retries must be less than 50", true, _retries < 50);
                DelayPostTask(new DispatcherOperationCallback(DoubleClickWord), null, 50);
                return null;
            }

            MoveMouse(fe, .5, .5);
            Input.SendMouseInput(0, 0, 0, SendMouseInputFlags.LeftDown);
            Input.SendMouseInput(0, 0, 0, SendMouseInputFlags.LeftUp);
            Input.SendMouseInput(0, 0, 0, SendMouseInputFlags.LeftDown);
            Input.SendMouseInput(0, 0, 0, SendMouseInputFlags.LeftUp);

            CopySelection();

            PostTask(new DispatcherOperationCallback(VerifySequenceSelectWord));
            return null;
        }

        private object VerifySequenceSelectWord(object arg)
        {
            LogProgressLine("VerifySequenceSelectWord");

            // Check what we copied
            String copied = (String)Clipboard.GetDataObject().GetData(typeof(String));

            DrtVerify("VerifySequenceSelectWord", "Copied word", "costs ", copied);
            // Launch next test if in batch mode
            if (SuiteArgs.Count == 0)
            {
                PostTask(new DispatcherOperationCallback(TestSequenceSelectAcrossPages));
            }
            else
            {
                PostTask(new DispatcherOperationCallback(ShutDownDRT), false);
            }
            return null;
        }

        private object TestSequenceSelectAcrossPages(object arg)
        {
            LogProgressLine("TestSequenceSelectAcrossPages");
            TurnToPage(3); // Could change to 4 to select across documents
            DelayPostTask(new DispatcherOperationCallback(KeyboardSelect), null, 50);
            return null;
        }

        private object KeyboardSelect(object arg)
        {
            LogProgressLine("KeyboardSelect");

            FrameworkElement fe = FindFixedPage(_dv);

            if (fe == null)
            {
                _retries++;
                DrtVerify("KeyboardSelect", "time-out on getting FixedPage, retries must be less than 50", true, _retries < 50);
                DelayPostTask(new DispatcherOperationCallback(KeyboardSelect), null, 50);
                return null;
            }

            MoveMouse(fe, .5, .85);
            Input.SendMouseInput(0, 0, 0, SendMouseInputFlags.LeftDown);
            Input.SendMouseInput(0, 0, 0, SendMouseInputFlags.LeftUp);

            Input.SendKeyboardInput(Key.LeftShift, true);
            Input.SendKeyboardInput(Key.Down, true);
            Input.SendKeyboardInput(Key.Down, false);
            /*Input.SendKeyboardInput(Key.Down, true);
            Input.SendKeyboardInput(Key.Down, false);
            Input.SendKeyboardInput(Key.Down, true); // Not selecting across page because
            Input.SendKeyboardInput(Key.Down, false); // when second page is hidden the
            Input.SendKeyboardInput(Key.Down, true); // rest of the document is selected
            Input.SendKeyboardInput(Key.Down, false);
            Input.SendKeyboardInput(Key.Down, true);
            Input.SendKeyboardInput(Key.Down, false);*/
            Input.SendKeyboardInput(Key.End, true);
            Input.SendKeyboardInput(Key.End, false);
            Input.SendKeyboardInput(Key.Right, true);
            Input.SendKeyboardInput(Key.Right, false);
            Input.SendKeyboardInput(Key.Right, true);
            Input.SendKeyboardInput(Key.Right, false);
            Input.SendKeyboardInput(Key.LeftShift, false);

            CopySelection();

            PostTask(new DispatcherOperationCallback(VerifySequenceSelectAcrossPages));
            return null;
        }

        private object VerifySequenceSelectAcrossPages(object arg)
        {
            LogProgressLine("VerifySequenceSelectAcrossPages");

            // Check what we copied
            String copied = (String)Clipboard.GetDataObject().GetData(typeof(String));

            DrtVerify("VerifySequenceSelectAcrossPages", "Copied text", "or-drawn 2D shapes, such as the rectangles and ellipses. The shapes aren't just for display; shapes implement many of the features that you expect from controls, in", copied);
            // Launch next test if in batch mode
            if (SuiteArgs.Count == 0)
            {
                PostTask(new DispatcherOperationCallback(TestSequenceSelectAll));
            }
            else
            {
                PostTask(new DispatcherOperationCallback(ShutDownDRT), false);
            }
            return null;
        }

        private object TestSequenceSelectAll(object arg)
        {
            LogProgressLine("TestSequenceSelectAll");

            Input.SendKeyboardInput(Key.LeftCtrl, true);
            Input.SendKeyboardInput(Key.A, true);
            Input.SendKeyboardInput(Key.A, false);
            Input.SendKeyboardInput(Key.LeftCtrl, false);

            CopySelection();

            PostTask(new DispatcherOperationCallback(VerifySequenceSelectAll));
            return null;
        }

        private object VerifySequenceSelectAll(object arg)
        {
            LogProgressLine("VerifySequenceSelectAll");

            PostTask(new DispatcherOperationCallback(ShutDownDRT), false);
            return null;
        }

        // Move the mouse
        //  fe - framework element
        //  x - horizontal position:  0.0 = left edge,  1.0 = right edge
        //  y - vertical position:    0.0 = top edge,   1.0 = bottom edge
        private void MoveMouse(FrameworkElement fe, double x, double y)
        {
            Point pt = new Point(x * fe.RenderSize.Width, y * fe.RenderSize.Height);
            GeneralTransform g = fe.TransformToAncestor(Source.RootVisual);

            g.TryTransform(pt, out pt);

            Input.MoveTo(PointToScreen(Source, pt));
        }

        internal static Point PointToScreen(HwndSource window, Point ptSrc)
        {
            MS.Win32.NativeMethods.POINT pt = new MS.Win32.NativeMethods.POINT(Convert.ToInt32(ptSrc.X), Convert.ToInt32(ptSrc.Y));

            MS.Win32.UnsafeNativeMethods.ClientToScreen(window.Handle, pt);
            return new Point(pt.x, pt.y);
        }

        // Click the mouse (at its current position)
        private void ClickMouse()
        {
            Input.SendMouseInput(0, 0, 0, SendMouseInputFlags.LeftDown);
            Input.SendMouseInput(0, 0, 0, SendMouseInputFlags.LeftUp);
        }

        private void CopySelection()
        {
            Input.SendKeyboardInput(Key.LeftCtrl, true);
            Input.SendKeyboardInput(Key.C, true);
            Input.SendKeyboardInput(Key.C, false);
            Input.SendKeyboardInput(Key.LeftCtrl, false);
        }

        private FrameworkElement FindFixedPage(DependencyObject node)
        {
            // see if the node itself has the right value
            FixedPage fp = node as FixedPage;

            if (fp != null)
            {
                return fp;
            }

            // if not, recursively look at the visual children
            int count = VisualTreeHelper.GetChildrenCount(node);

            for(int i = 0; i < count; i++)
            {
                FrameworkElement result = FindFixedPage(VisualTreeHelper.GetChild(node, i));

                if (result != null)
                    return result;
            }

            // not found
            return null;
        }

        // ------------------------------------------------------------------
        // Private Fields
        // ------------------------------------------------------------------
        private FixedDocumentSequence _documentSequence;
        private int _retries;
#if USINGFIXEDVIEWER        
        private FixedDocViewer _dv;
#else
        private DocumentViewer _dv;
#endif
    }
}

