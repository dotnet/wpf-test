// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*
*************************************************************************************
*                                                                                   *
*  Title:                                                                           *
*                                                                                   *
*  Description:                                                                     *
*                                                                                   *
*                                                                                   *
*                                                                                   *
*                                                                                   *
*                                                                                   *
*************************************************************************************
*/

#define use_tools
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Media;
using System.Windows.Markup;
using System.Collections;
using System.Windows.Input;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Windows.Automation.Provider;
using System.Windows.Automation;
using System.Windows.Shapes;
using System.Reflection;

using Microsoft.Test.RenderingVerification;
using Microsoft.Test.RenderingVerification.Model.Analytical;


namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    public class BasicPageFunction : PageFunction<int>
    {
        public BasicPageFunction ()
        {
            Rectangle rect = new Rectangle ();

            rect.Fill = new SolidColorBrush (Colors.Fuchsia);
            rect.Width = 300;
            rect.Height = 300;
            this.Content = rect;
        }

        public bool CheckState ()
        {
            // Sync render implementation
            Assembly mcasm = Assembly.GetAssembly(typeof(System.Windows.Interop.HwndTarget));
            Type mcType = mcasm.GetType("System.Windows.Media.MediaContext");
            object mc = mcType.InvokeMember("From", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.InvokeMethod, null, null, new object[] { System.Windows.Threading.Dispatcher.CurrentDispatcher });
            mcType.InvokeMember("CompleteRender", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod, null, mc, new object[] { });

            if (!(this.Content is Rectangle))
            {
                NavigationHelper.Fail ("Child verification of the pagefunction failed");
                return false;
            }
            else
            {
                NavigationHelper.Output ("Child verification passed (rectangle)");
            }

            if (!HitTestVerify ())
            {
                NavigationHelper.Fail ("HitTest verification failed.");
                return false;
            }

            if (!UIAutomationVerify ())
            {
                NavigationHelper.Fail("UIAutomation verification failed.");
                return false;
            }
            return true;
        }

        private bool HitTestVerify ()
        {
            NavigationHelper.Output("Doing HitTest Verification for BasicPageFunction");

            float dpiXRatio = System.Drawing.Graphics.FromHwnd(IntPtr.Zero).DpiX / 96;

            Rect _clientrect = Bag.GetClientAreaRect(PageFunctionTestApp.CurrentPFTestApp.MainNavWindow);
            NavigationHelper.Output("Client coords of main window are: "
                + _clientrect.Left + "," + _clientrect.Top 
                + " "
                + _clientrect.Right + "," + _clientrect.Bottom);
            Point pt = new Point((_clientrect.Right/2) / dpiXRatio, (_clientrect.Bottom/2) / dpiXRatio);
            Visual rootVis = (Visual)PageFunctionTestApp.CurrentPFTestApp.MainNavWindow.Content;
            //use exact center of client area since that's where the square is drawn
            NavigationHelper.Output("Hittesting at " + _clientrect.Right/2 + ", " + _clientrect.Bottom/2 + " from navigation client area");
            Visual vis =
                (Visual)VisualTreeHelper.HitTest(rootVis, pt).VisualHit;
            if (vis == this.Content) {
                NavigationHelper.Output("Found expected visual at position " + pt.ToString());
                return true;
            } else {
                NavigationHelper.Fail("Found unexpected visual: " + vis.GetType().ToString());
                return false;
            }
        }

        private bool UIAutomationVerify ()
        {
            return true;
        }
    }

    public class ChildMultipleGetSetPF : PageFunction<int>
    {
        public ChildMultipleGetSetPF ()
        {
            Rectangle rect = new Rectangle ();

            rect.Fill = new SolidColorBrush(Colors.Fuchsia);
            rect.Width = 300;
            rect.Height = 300;

            // set
            this.Content = rect;

            //get
            Rectangle _child = Content as Rectangle;
            if (!(_child == rect))
            {
                NavigationHelper.Fail ("Set/get of Child property failed the 1st time");
                //Shutdown ();
            } else
            {
                NavigationHelper.Output ("Set/get of Child property passed 1st time");
            }

            NavigationHelper.Output ("set / get child a 2nd time. Adding a 2nd time in code should not throw exception");
            Rectangle rect2 = new Rectangle ();
            rect2.Fill = new SolidColorBrush (Colors.OrangeRed);
            rect2.Width = 300;
            rect2.Height = 300;
            try
            {
                Content = rect2;
            }
            catch (System.Exception e_error)
            {
                NavigationHelper.Fail ("Got an unexpected error when adding a child a second time after it has already been added. Actual: "
                    + e_error.Message);
                goto END_CTOR;
            }
            NavigationHelper.Output ("As expected, got no error when adding a child the 2nd time");
        END_CTOR:
            NavigationHelper.Output ("Finished construction");
        }

        public bool CheckState ()
        {
            // Sync render implementation
            Assembly mcasm = Assembly.GetAssembly(typeof(System.Windows.Interop.HwndTarget));
            Type mcType = mcasm.GetType("System.Windows.Media.MediaContext");
            object mc = mcType.InvokeMember("From", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.InvokeMethod, null, null, new object[] { System.Windows.Threading.Dispatcher.CurrentDispatcher });
            mcType.InvokeMember("CompleteRender", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod, null, mc, new object[] { });

            if (!(this.Content is Rectangle))
            {
                NavigationHelper.Fail ("Child get property of pagefunction failed");
                return false;
            }
            else
            {
                NavigationHelper.Output ("Child verification (GET) passed (rectangle)");
            }

            if (!HitTestVerify ())
            {
                NavigationHelper.Fail ("HitTest verification failed.");
                return false;
            }

            if (!UIAutomationVerify ())
            {
                NavigationHelper.Fail ("UIAutomation verification failed.");
                return false;
            }

            return true;
        }

        private bool HitTestVerify ()
        {
            NavigationHelper.Output("Doing HitTest Verification for ChildMultipleGetSetPF");

            float dpiXRatio = System.Drawing.Graphics.FromHwnd(IntPtr.Zero).DpiX / 96;

            Rect _clientrect = Bag.GetClientAreaRect(PageFunctionTestApp.CurrentPFTestApp.MainNavWindow);
            NavigationHelper.Output("Client coords of main window are: "
                + _clientrect.Left + "," + _clientrect.Top
                + " "
                + _clientrect.Right + "," + _clientrect.Bottom);
            //use exact center of client area since that's where the square is drawn
            Point pt = new Point((_clientrect.Right/2) / dpiXRatio, (_clientrect.Bottom/2) / dpiXRatio);
            Visual vis =
                (Visual)VisualTreeHelper.HitTest((Visual)PageFunctionTestApp.CurrentPFTestApp.MainNavWindow.Content, pt).VisualHit;
            if (vis == this.Content) {
                NavigationHelper.Output("Found expected visual at position " + pt.ToString());
                return true;
            } else {
                NavigationHelper.Fail("Found unexpected visual: " + vis.GetType().ToString());
                return false;
            }
        }

        private bool UIAutomationVerify ()
        {
            return true;
        }
    }

    public class ChildGetSetPF : PageFunction<string>
    {
        public ChildGetSetPF ()
        {
            Rectangle rect = new Rectangle ();

            rect.Fill = new SolidColorBrush (Colors.LightGreen);
            rect.Width = 300;
            rect.Height = 300;

            // set
            this.Content = rect;

            //get
            Rectangle _child = Content as Rectangle;

            if (!(_child == rect))
            {
                NavigationHelper.Fail ("Set/get of Child property failed the 1st time");
                //Shutdown ();
            }
            else
            {
                NavigationHelper.Output ("Set/get of Child property passed 1st time");
            }

        }

        public bool CheckState ()
        {
            // Sync render implementation
            Assembly mcasm = Assembly.GetAssembly(typeof(System.Windows.Interop.HwndTarget));
            Type mcType = mcasm.GetType("System.Windows.Media.MediaContext");
            object mc = mcType.InvokeMember("From", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.InvokeMethod, null, null, new object[] { System.Windows.Threading.Dispatcher.CurrentDispatcher });
            mcType.InvokeMember("CompleteRender", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod, null, mc, new object[] { });

            if (!(this.Content is Rectangle))
            {
                NavigationHelper.Fail ("Child get property of pagefunction failed");
                return false;
            }
            else
            {
                NavigationHelper.Output ("Child verification (GET) passed (rectangle)");
            }

            if (!HitTestVerify ())
            {
                NavigationHelper.Fail ("HitTest verification failed.");
                return false;
            }

            if (!UIAutomationVerify ())
            {
                NavigationHelper.Fail ("UIAutomation verification failed.");
                return false;
            }

            return true;
        }

        private bool HitTestVerify ()
        {
            NavigationHelper.Output("Doing HitTest Verification for ChildGetSetPF");

            float dpiXRatio = System.Drawing.Graphics.FromHwnd(IntPtr.Zero).DpiX / 96;

            Rect _clientrect = Bag.GetClientAreaRect(PageFunctionTestApp.CurrentPFTestApp.MainNavWindow);
            NavigationHelper.Output("Client coords of main window are: "
                + _clientrect.Left + "," + _clientrect.Top
                + " "
                + _clientrect.Right + "," + _clientrect.Bottom);
            //use exact center of client area since that's where the square is drawn
            Point pt = new Point((_clientrect.Right/2) / dpiXRatio, (_clientrect.Bottom/2) / dpiXRatio);
            HitTestResult htr = VisualTreeHelper.HitTest((Visual)PageFunctionTestApp.CurrentPFTestApp.MainNavWindow.Content, pt);
            Visual vis =
                (Visual)VisualTreeHelper.HitTest((Visual)PageFunctionTestApp.CurrentPFTestApp.MainNavWindow.Content, pt).VisualHit;
            if (vis == this.Content) {
                NavigationHelper.Output("Found expected visual at position " + pt.ToString());
                return true;
            } else {
                NavigationHelper.Fail("Found unexpected visual: " + vis.GetType().ToString());
                return false;
            }
        }

        private bool UIAutomationVerify ()
        {
            return true;
        }

    }
}

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    public partial class PageFunctionTestApp
    {
        private void BasicPFTestSetup ()
        {
            NavigationHelper.CreateLog("BasicPFTest");
            NavigationHelper.Output("Basic test of a pagefunction");
            MainNavWindow.Navigate (new BasicPageFunction ());
            Application.Current.LoadCompleted += new LoadCompletedEventHandler (OnLoad_BasicTestPF);
        }

        private void OnLoad_BasicTestPF (object sender, NavigationEventArgs e)
        {
            PostVerificationItem (new VerificationDelegate (Verify_BasicPFTest));
        }

        private void Verify_BasicPFTest ()
        {
            NavigationHelper.Output ("Verification of Basic PF.");

            BasicPageFunction pf = MainNavWindow.Content as BasicPageFunction;

            if (pf == null)
            {
                NavigationHelper.Fail ("Window content was not the pagefunction expected");
            }

            if (!pf.CheckState ())
            {
                NavigationHelper.Fail ("State of PageFunction was incorrect.");
            }

            NavigationHelper.Pass ("Basic PageFunction test passed.");
        }

        private void ChildGetSetTestPF ()
        {
            NavigationHelper.CreateLog("PF get/set Child property");
            NavigationHelper.Output("Setting and getting Child property of PF");
            MainNavWindow.Navigate (new ChildGetSetPF ());
            MainNavWindow.ContentRendered += new EventHandler (Load_ChildGetSetPF);
        }

        private void Load_ChildGetSetPF (object sender, EventArgs e)
        {
            NavigationHelper.Output("Content Rendered on window");
            PostVerificationItem (new VerificationDelegate (Verify_ChildGetSetPF));
        }

        private void Verify_ChildGetSetPF ()
        {
            NavigationHelper.Output ("Verification of Get/Set of Child property.");

            ChildGetSetPF pf = MainNavWindow.Content as ChildGetSetPF;

            if (pf == null)
            {
                NavigationHelper.Fail ("Window content was not the pagefunction expected");
            }

            if (!pf.CheckState ())
            {
                NavigationHelper.Fail ("State of PageFunction was incorrect.");
            }

            NavigationHelper.Pass ("PageFunction get/set of child property test passed.");
        }


        private void Test_ChildMultipleGetSetPF ()
        {
            NavigationHelper.CreateLog("PF get/set Child property multiple times");
            NavigationHelper.Output("Setting and getting Child property of PF multiple times (should throw exception)");
            MainNavWindow.Navigate (new ChildMultipleGetSetPF ());
            Application.Current.LoadCompleted += new LoadCompletedEventHandler (Load_ChildMultipleGetSetPF);
        }

        private void Load_ChildMultipleGetSetPF (object sender, NavigationEventArgs e)
        {
            PostVerificationItem (new VerificationDelegate (Verify_ChildMultipleGetSetPF));
        }

        private void Verify_ChildMultipleGetSetPF ()
        {
            NavigationHelper.Output ("Verification of Get/Set of Child property multiple times.");
            ChildMultipleGetSetPF pf = MainNavWindow.Content as ChildMultipleGetSetPF;

            if (pf == null)
            {
                NavigationHelper.Fail ("Window content was not the pagefunction expected");
            }

            if (!pf.CheckState ())
            {
                NavigationHelper.Fail ("State of PageFunction was incorrect.");
            }

            NavigationHelper.Pass ("PageFunction get/set of child property multiple times test passed.");
        }
    }
}
