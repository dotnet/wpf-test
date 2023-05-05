// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Interop;
using System.Windows.Navigation;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Shapes;
using Microsoft.Test.Logging;
using Microsoft.Test.Layout;
using Microsoft.Test.Input;
using ElementLayout.TestLibrary;

//namespace ElementLayout.FeatureTests.Transforms
//{
//    public class LayoutTransform1 : CodeTest
//    {
//        public LayoutTransform1()
//        { }

//        public override void WindowSetup()
//        {
//            this.window.Height = 600;
//            this.window.Width = 900;
//            this.window.Top = 0;
//            this.window.Left = 0;
//            this.window.Content = this.TestContent();
//        }

//        public override FrameworkElement TestContent()
//        {
//            return null;
//        }

//        public override void TestActions()
//        {
//        }

//        public override void TestVerify()
//        {
//        }
//    }
//}

//namespace System.LayoutTest.Test
//{
//    class LayoutTransformBVT : Application
//    {
//        public static TestLog logrm;
//        public static Window testWin;
//        public static string testStr;
//        public static bool result = false;

//        protected override void OnStartup(StartupEventArgs e)
//        {
//            testWin = TestWin.Launch(typeof(Window), 500, 700, 0, 0, TestContent(), true);
//            Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new DispatcherOperationCallback(DoMouseClick), null);
//        }

//        public static DockPanel eRoot;
//        public static Grid grid;
//        public static TextBlock text;
//        public static Button btn;
//        public static FrameworkElement TestContent()
//        {
//            eRoot = new DockPanel();

//            btn = new Button();
//            btn.Width = 200;
//            btn.Content = "Test";
//            btn.Click += new RoutedEventHandler(OnClick);
//            DockPanel.SetDock(btn, Dock.Bottom);

//            Border b = new Border();
//            b.Background = Brushes.Orange;
//            b.Width = 300;
//            text = new TextBlock();
//            //text.ContentEnd.InsertTextInRun("This is the test for LayoutTransform. This text should be reflowed when it's scaled :)");
//            text.Text = "This is the test for LayoutTransform. This text should be reflowed when it's scaled :)";
//            text.TextWrapping = TextWrapping.WrapWithOverflow;
//            b.Child = text;

//            grid = GridCommon.CreateGrid(2, 2);
//            grid.Children.Add(b);

//            eRoot.Children.Add(btn);
//            eRoot.Children.Add(grid);
//            return eRoot;
//        }

//        private static void OnClick(object sender, RoutedEventArgs e)
//        {
//            switch (testStr)
//            {
//                case "Reflowing":
//                    text.LayoutTransform = new ScaleTransform(2, 2);
//                    break;
//                default:
//                    break;
//            }
//            CommonFunctionality.FlushDispatcher();
//            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new DispatcherOperationCallback(Verification), null);
//        }
//        private object DoMouseClick(object args)
//        {
//            UserInput.MouseLeftClickCenter(btn);
//            CommonFunctionality.FlushDispatcher();
//            return args;
//        }
//        private static object Verification(object obj)
//        {
//            VScanCommon vScanVerification = new VScanCommon(testWin);
//            vScanVerification.CompareImage();
//            testWin.Close();
//            return null;
//        }
//        [STAThread]
//        static void Main(string[] args)
//        {
//            frm = new AutomationFramework();
//            Helpers.Log("Starting Test...");
//            if (frm == null)
//                throw new Exception("Automation Framework object is null...");
//            else
//            {
//                args = frm.ParseCommandLine(args);
//                testStr = args[0];
//                Helpers.Log("Test = " + testStr.ToString());
//                LayoutTransformBVT testApp = new LayoutTransformBVT();
//                testApp.Run();
//            }
//        }
//    }
//}
