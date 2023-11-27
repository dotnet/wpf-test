// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// Copyright (C) Microsoft Corporation.  All rights reserved.
// Description: Test cases for Fixed hyperlink navigation.
//

using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Reflection;
using System.IO;
using System.Windows.Markup;
using System.Windows.Interop;

namespace DRT
{
    public class FixedStructureSuite : DrtTestSuite
    {
        public FixedStructureSuite()
            : base("FixedStructureSuite")
        {
            Contact = "Microsoft";
        }

        private
        static
        string[]
        ContainerTestFiles = {
            ".\\DrtFiles\\NGCTest\\constitution.xps",
            ".\\DrtFiles\\Payloads\\Sequence\\SampleXls.xps"
        };

        public override DrtTest[] PrepareTests()
        {

            if (DRT.KeepAlive)
            {
                return new DrtTest[0];
            }
            else
            {
                return new DrtTest[]
                {
                    new DrtTest(LoadingDocument),
                    new DrtTest(CloseWindow),
                    
                    new DrtTest(LoadingDocument),
                    new DrtTest(SelectAll),
                    new DrtTest(CopySelection),
                    new DrtTest(VerifySelection),
                    new DrtTest(CloseWindow),

                    new DrtTest(Cleanup),
                };
            }
        }

        private void LoadingDocument()
        {
            DRT.LogOutput("LoadingDocument");

            MainTestWindow = new NavigationWindow();
            MainTestWindow.Title = "DrtStructureSelection";

            DrtFixedBase drtFixed = DRT as DrtFixedBase;

            MainTestWindow.Content = drtFixed.LoadPackage(ContainerTestFiles[_fileIndex]) as System.Windows.Documents.IDocumentPaginatorSource;
            _fileIndex++;

            MainTestWindow.Show();

            DRT.Pause(500);
        }

        private void SelectAll()
        {
            DRT.LogOutput("SelectAll");

            // focus and move caretto the window
            FrameworkElement fe = FindFixedPage(MainTestWindow);

            MoveMouse(fe, MainTestWindow, .1, .1);
            Input.SendMouseInput(0, 0, 0, SendMouseInputFlags.LeftDown);
            Input.SendMouseInput(0, 0, 0, SendMouseInputFlags.LeftUp);

            // Select all.
            Input.SendKeyboardInput(Key.LeftCtrl, true);
            Input.SendKeyboardInput(Key.A, true);
            Input.SendKeyboardInput(Key.A, false);
            Input.SendKeyboardInput(Key.LeftCtrl, false);

            DRT.Pause(500);
            return;
        }

        private void CopySelection()
        {
            DRT.LogOutput("CopySelection");

            Input.SendKeyboardInput(Key.LeftCtrl, true);
            Input.SendKeyboardInput(Key.C, true);
            Input.SendKeyboardInput(Key.C, false);
            Input.SendKeyboardInput(Key.LeftCtrl, false);

        }
        
        protected void VerifySelection()
        {
            DRT.LogOutput("VerifySelection");

            // 1. Verify plain text selection.
            String copied = (String)Clipboard.GetDataObject().GetData(typeof(String));

#if true
            DRT.AssertEqual(true, copied.Contains("Sample company for test Balance Sheets\r\n(In thousands)\r\nJune 13, 2033 (1) March 15, 2034\r\nAssets Current assets: Cash and equivalents"),
                "Verify Selection with Explicit Document Structure. The acutal copied text:\n *****{0}*****\n",
                    copied.Length > 400 ? copied.Substring(0, 400) : copied);
#else            
            DRT.LogOutput("+++++++++++++++++++++++++++++++++++++++++");
            if (copied.Length > 400)
                DRT.LogOutput(copied.Substring(0, 400));
            else
                DRT.LogOutput(copied);
            DRT.LogOutput("+++++++++++++++++++++++++++++++++++++++++");
#endif
            // 2. Verify XAML selection, thus the RTF selection.
            String xamlString = Clipboard.GetDataObject().GetData("Xaml", false) as String;

            Byte[] encodedBytes = System.Text.Encoding.UTF8.GetBytes(xamlString);
            MemoryStream xamlStream = new MemoryStream(encodedBytes);
            
            object o = XamlReader.Load(xamlStream);            

            return;
        }

        private void CloseWindow()
        {
            DRT.LogOutput("CloseWindow");
            
            DrtFixedBase drtFixed = DRT as DrtFixedBase;

            //Call cleanup to remove package from PackagStore
            drtFixed.Cleanup();

            MainTestWindow.Close();
        }

        private void Cleanup()
        {
            DRT.LogOutput("Cleanup");
            //
            // If that is the last suite, then we need to shutdown DRT manually.
            //
            Dispatcher.CurrentDispatcher.InvokeShutdown();
            DRT.Dispose();
        }

        private void MoveMouseAndSelection(double x1, double y1, double x2, double y2)
        {
            FrameworkElement fe = FindFixedPage(MainTestWindow);

            MoveMouse(fe, MainTestWindow, x1, y1);
            Input.SendMouseInput(0, 0, 0, SendMouseInputFlags.LeftDown);
            MoveMouse(fe, MainTestWindow, x2, y2);
            Input.SendMouseInput(0, 0, 0, SendMouseInputFlags.LeftUp);

            CopySelection();
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
                FrameworkElement result = FindFixedPage(VisualTreeHelper.GetChild(node,i));

                if (result != null)
                    return result;
            }

            // not found
            return null;
        }
        // Move the mouse
        //  fe - framework element
        //  x - horizontal position:  0.0 = left edge,  1.0 = right edge
        //  y - vertical position:    0.0 = top edge,   1.0 = bottom edge
        private void MoveMouse(FrameworkElement fe, Window reference, double x, double y)
        {
            Point pt = new Point(x * fe.RenderSize.Width, y * fe.RenderSize.Height);
            GeneralTransform g = fe.TransformToAncestor(reference);
            g.TryTransform(pt, out pt);

            Point screetPt = PointToScreen(GetHwndSource(reference), pt);
            Input.MoveTo(screetPt);
            Console.WriteLine("Move mouse to {0} and {1}", screetPt.X, screetPt.Y);
        }
        private HwndSource GetHwndSource(Window window)
        {
            PropertyInfo info = window.GetType().GetProperty("HwndSourceWindow", BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.NonPublic);

            Object obj = info.GetValue(window, null);

            return (HwndSource)obj;
        }

        private Point PointToScreen(HwndSource window, Point ptSrc)
        {
            MS.Win32.NativeMethods.POINT pt = new MS.Win32.NativeMethods.POINT(Convert.ToInt32(ptSrc.X), Convert.ToInt32(ptSrc.Y));

            MS.Win32.UnsafeNativeMethods.ClientToScreen(window.Handle, pt);
            return new Point(pt.x, pt.y);
        }

        //
        // Test resource
        //
        private NavigationWindow MainTestWindow = null;
        private int _fileIndex = 0;

    }
}




