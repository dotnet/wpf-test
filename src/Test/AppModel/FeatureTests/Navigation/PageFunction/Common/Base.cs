// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*
*************************************************************************************
*                                                                                   *
*  Title:                                                                           *
*      Some helper classes for PageFunction Tests                                   *
*                                                                                   *
*  Description:                                                                     *
*      Logging and some Utility classes for Bitmap Verification                     *
*           Classes: Bag                                                            *
*                                                                                   *                                                  *
*                                                                                   *
*                                                                                   *
*                                                                                   *
*                                                                                   *
*************************************************************************************
*/

#define use_tools
#undef createbaselineimage
#define createfailureimage

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
using Microsoft.Test.Win32;
using Microsoft.Test.RenderingVerification;
using Microsoft.Test.Graphics.TestTypes;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    public class Bag
    {
        public static int GetJournalCount (NavigationWindow wNav)
        {
              throw new NotImplementedException("Cannot enumerate through the journal anymore. Cannot get the number of items in the journal");
//              return -1;
//            int count = 1;
//            IEnumerator journalenum = wNav.Journal.GetEnumerator ();
//
//            while (journalenum.MoveNext ())
//            {
//                //NavigationHelper.Output(((JournalEntry)journalenum.Current).Uri.AbsoluteUri);
//                count++;
//            }
//            return count;
        }

        public static Size FindWindowClientSize(Window w)
        {
            double width = -1, height = -1;
            IntPtr hwnd;
            WindowInteropHelper wih = new WindowInteropHelper(w);
            if (wih == null)
                return Size.Empty;
            else
                hwnd = wih.Handle;

            if (hwnd == IntPtr.Zero)
                return Size.Empty;

            System.Drawing.Rectangle _rect_ = new System.Drawing.Rectangle();
            if (!Interop.GetClientRect(hwnd, ref _rect_))
                return Size.Empty;

            width  = (double)_rect_.Right;
            height = (double)_rect_.Bottom;
            return new Size(width,height);
        }

        [System.Security.Permissions.PermissionSet (System.Security.Permissions.SecurityAction.Assert, Name = "FullTrust")]
        public static Rect GetClientAreaRect (NavigationWindow nw)
        {
            // using fully-qualified names here because adding a using causes other ambiguous name conflicts elsewhere
            float dpiXRatio = System.Drawing.Graphics.FromHwnd(IntPtr.Zero).DpiX / 96;
            float dpiYRatio = System.Drawing.Graphics.FromHwnd(IntPtr.Zero).DpiY / 96;

            NavigationHelper.Output("Window Left/Top is " + nw.Left + "," + nw.Top);
            Point origcoords = new Point(0, 0);
            Matrix TranslateMatrix;
            GeneralTransform gt = ((Visual)nw.Content).TransformToAncestor(nw);
            Transform t = gt as Transform;
            if(t==null)
            {
                throw new ApplicationException("//TODO: Handle GeneralTransform Case - introduced by Transforms Breaking Change");
            }
            TranslateMatrix = t.Value;

            origcoords = TranslateMatrix.Transform(origcoords);

            // fix coords if DPI is > 96
            if (dpiXRatio > 1)
            {
                origcoords.X *= dpiXRatio;
                origcoords.Y *= dpiYRatio;
                origcoords.X++;
                origcoords.Y++;
            }

            IntPtr hwnd;
            WindowInteropHelper iwh = new WindowInteropHelper(nw);
            if (iwh == null)
                throw new ApplicationException("Could not find the hwnd of the main window");
            else
                hwnd = iwh.Handle;

            System.Drawing.Rectangle rect = new System.Drawing.Rectangle();
            Interop.GetClientRect(hwnd, ref rect);

            System.Drawing.Point
                    pt_LT = new System.Drawing.Point (/*0,0*/ rect.Left , rect.Top),
                    pt_RB = new System.Drawing.Point (        rect.Right, rect.Bottom);

            Interop.ClientToScreen(
                hwnd, ref pt_LT);
            Interop.ClientToScreen(
                hwnd, ref pt_RB);

            Rect clientrect =
                new Rect (
                    new Point( pt_LT.X + origcoords.X,
                               pt_LT.Y + origcoords.Y ) ,
                    new Point( pt_RB.X,
                               pt_RB.Y ) );

            NavigationHelper.Output("Returning Client Coordinates");

            return clientrect;

        }

        /// <summary>
        /// </summary>
        public static bool VisualVerifyClientArea (
                            string pathToBaselineBitmap,
                            int left,
                            int top,
                            int width,
                            int height,
                            NavigationWindow nw
                    )
        {
            bool retval = false;

            NavigationHelper.Output("Doing Visual Verification 2");

            // Sync render implementation
            Assembly mcasm = Assembly.GetAssembly(typeof (System.Windows.Interop.HwndTarget));
            Type mcType = mcasm.GetType("System.Windows.Media.MediaContext");
            object mc = mcType.InvokeMember("From", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.InvokeMethod, null, null, new object[] { System.Windows.Threading.Dispatcher.CurrentDispatcher });
            mcType.InvokeMember("CompleteRender", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod, null, mc, new object[] {});





            Rect _clientrect =  GetClientAreaRect(nw);

            NavigationHelper.Output("Client coords of visual verify window are: "
                + _clientrect.Left + "," + _clientrect.Top
                + " "
                + _clientrect.Right + "," + _clientrect.Bottom);

            System.Drawing.Rectangle r =
                new System.Drawing.Rectangle (
                    (int)(_clientrect.Left) + left,
                    (int)(_clientrect.Top) + top,
                    width,
                    height);

            System.Drawing.Bitmap windowImage = ImageUtility.CaptureScreen (r);
            System.Drawing.Bitmap SafeWindowImage = new System.Drawing.Bitmap(windowImage);

#if createbaselineimage
            windowImage.Save (pathToBaselineBitmap,
                    System.Drawing.Imaging.ImageFormat.Bmp);
#else

            NavigationHelper.Output("Comparing the captured image for this pf to the baseline image");

            ImageComparator imageCompare =
                    new ImageComparator();

            ImageAdapter imageSnapshot = new ImageAdapter(windowImage);
            ImageAdapter imageSource =
                new ImageAdapter(
                  new System.Drawing.Bitmap(pathToBaselineBitmap));

            NavigationHelper.Output("Image comparison commencing...");

            if (imageCompare.Compare(imageSnapshot, imageSource, false))
            {
                NavigationHelper.Output("Image comparison succeeded");
                retval = true;
            }
            else
            {
               NavigationHelper.Fail("Image comparison failed");
#if createfailureimage
                SafeWindowImage.Save ("failbitmap.bmp" ,
                        System.Drawing.Imaging.ImageFormat.Bmp);
#endif
                retval = false;
            }

#endif
            return retval;
        }

        public static bool CheckNavigatorBackFwd(
                        NavigationWindow navigator,
                        bool ExpectedNavigatorCanGoBack,
                        bool ExpectedNavigatorCanGoForward
                        )
        {
            bool retval = true;
            int counter = 0;
            IEnumerator journalEnum = ((IEnumerable)navigator.GetValue(NavigationWindow.BackStackProperty)).GetEnumerator();
            while (journalEnum.MoveNext())
            {
                counter++;
            }

            // If we are expecting that we can go back, check that back stack count > 0
            if (ExpectedNavigatorCanGoBack && navigator.CanGoBack)
            {
                if ((counter > 0) && ExpectedNavigatorCanGoBack)
                    NavigationHelper.Output("CanGoBack is as expected");
                else
                {
                    NavigationHelper.Fail("CanGoBack of INavigator is not as expected");
                    retval &= false;
                }
            }
            // Else, if we are expecting that we cannot go back, check that back stack count = 0
            else if (!ExpectedNavigatorCanGoBack && !navigator.CanGoBack)
            {
                if ((counter == 0) && !ExpectedNavigatorCanGoBack)
                    NavigationHelper.Output("CanGoBack is as expected");
                else
                {
                    NavigationHelper.Fail("CanGoBack of INavigator is not as expected");
                    retval &= false;
                }
            }

            NavigationHelper.Output(
                "Navigator CanGoBack: Expected: "
                + ExpectedNavigatorCanGoBack.ToString()
                + " Actual: "
                + counter.ToString());

            // Check forward journal... remember to reset counter.
            counter = 0;
            journalEnum = ((IEnumerable)navigator.GetValue(NavigationWindow.ForwardStackProperty)).GetEnumerator();
            while(journalEnum.MoveNext())
            {
                counter++;
            }

            // If we are expecting that we can go forward, check that fwd stack count > 0
            if (ExpectedNavigatorCanGoForward && navigator.CanGoForward)
            {
                if ((counter > 0) && ExpectedNavigatorCanGoForward)
                    NavigationHelper.Output("CanGoForward is as expected");
                else
                {
                    NavigationHelper.Fail("CanGoForward of INavigator is not as expected");
                    retval &= false;
                }
            }
            // Else, if we are expecting that we cannot go forward, check that fwd stack count = 0
            else if (!ExpectedNavigatorCanGoForward && !navigator.CanGoForward)
            {
                if ((counter == 0) && !ExpectedNavigatorCanGoForward)
                    NavigationHelper.Output("CanGoForward is as expected");
                else
                {
                    NavigationHelper.Fail("CanGoForward of INavigator is not as expected");
                    retval &= false;
                }
            }

            NavigationHelper.Output(
                "Navigator CanGoForward: Expected: "
                + ExpectedNavigatorCanGoForward.ToString()
                + " Actual: "
                + counter.ToString());

            return retval;
         }

        public static bool CheckJournalCount (NavigationWindow wNav, int expectedjnlcount)
        {
//            int jnlcount = GetJournalCount(wNav);
//            NavigationHelper.Output("Journal count for INavigator. Expected: " + expectedjnlcount + " Actual: " + jnlcount);
//            if (jnlcount != expectedjnlcount)
//            {
//                Logging.LogFail("Unexpected journal count for this window");
//                return false;
//            }
//            else
//            {
//                NavigationHelper.Output("Journal count for this window was as expected");
//            }
            return true;
        }

        public static void SaveFailureImage(System.Drawing.Bitmap failureImage, string filename)
        {
            System.Drawing.Bitmap safeWindowImage = new System.Drawing.Bitmap(failureImage);
            safeWindowImage.Save(filename, System.Drawing.Imaging.ImageFormat.Bmp);
        }

        public static void SaveFailureImage(System.Drawing.Bitmap failureImage)
        {
            System.Drawing.Bitmap safeWindowImage = new System.Drawing.Bitmap(failureImage);
            safeWindowImage.Save("FailImage.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
        }

        public static bool SupportsSerializationToJournal(FrameworkElement sender)
        {
            LocalValueEnumerator _enum = sender.GetLocalValueEnumerator();
            while (_enum.MoveNext())
            {
                // go through all DPs of the framewkelement
                LocalValueEntry lve = _enum.Current;
                DependencyProperty dp = lve.Property;
                FrameworkPropertyMetadata metadata = dp.GetMetadata(sender) as FrameworkPropertyMetadata;

                if (metadata.Journal)
                    return true;
            }
            return false;
        }

    }


}
