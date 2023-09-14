using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Media;
using System.Xml;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Avalon.Test.ComponentModel.Validations;
using Avalon.Test.ComponentModel.Actions;
using System.Globalization;
using Microsoft.Test.Logging;
using System.Windows.Data;
using System.Windows.Shapes;

using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.RenderingVerification.Model.Analytical;
using Microsoft.Test.RenderingVerification;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using Microsoft.Test.Win32;

namespace Avalon.Test.ComponentModel.IntegrationTests
{
    public class MyPopup : Popup
    {
        public void CallOnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            TestLog.Current.LogDebug("CallOnMouseRightButtonUp function got called");
            OnMouseRightButtonUp(e);
            QueueHelper.WaitTillQueueItemsProcessed();
        }
        public void CallOnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            TestLog.Current.LogDebug("CallOnMouseRightButtonDown function got called");
            OnMouseRightButtonDown(e);
            QueueHelper.WaitTillQueueItemsProcessed();
        }
    }

    public class PopupPublicAPIsTest : IIntegrationTest
    {
        bool mouseRightButtonUpInvoked = false;
        bool mouseRightButtonDownInvoked = false;
        bool openedInvoked = false;
        bool closedInvoked = false;
        public PopupPublicAPIsTest()
        {
        }
        /// <summary>
        /// Test Popup Public APIs.
        /// </summary>
        /// <param name="popup"></param>
        /// <param name="variation"></param>
        public TestResult Perform(object obj, XmlElement variation)
        {
            Panel panel = (Panel)obj;
            Popup popup = new Popup();
            MyPopup myPopup = new MyPopup();
            Border border = new Border();

            // test OnVisualChildrenChanged popup api.
            popup.Child = border;

            popup.PopupAnimation = PopupAnimation.Fade;

            Popup popup2 = new Popup();

            popup2.CustomPopupPlacementCallback = new CustomPopupPlacementCallback(myCustomPopupPlacementCallback);
            panel.Children.Add(popup2);

            popup.Opened += new EventHandler(popup_Opened);
            popup.Closed += new EventHandler(popup_Closed);
            QueueHelper.WaitTillQueueItemsProcessed();
            popup.IsOpen = true;
            QueueHelper.WaitTillQueueItemsProcessed();
            popup.IsOpen = false;
            QueueHelper.WaitTillQueueItemsProcessed();

            panel.Children.Add(myPopup);
            myPopup.MouseRightButtonUp += new MouseButtonEventHandler(myPopup_MouseRightButtonUp);
            myPopup.MouseRightButtonDown += new MouseButtonEventHandler(myPopup_MouseRightButtonDown);
            QueueHelper.WaitTillQueueItemsProcessed();
            myPopup.CallOnMouseRightButtonUp(null);
            myPopup.CallOnMouseRightButtonDown(null);
            QueueHelper.WaitTillQueueItemsProcessed();

            CustomPopupPlacement myCustomPopupPlacement = new CustomPopupPlacement();
            if (myCustomPopupPlacement.Equals(null))
            {
                TestLog.Current.LogDebug("myCustomPopupPlacement.Equals(null) is true, and it should be false.");
            }
            else
            {
                TestLog.Current.LogEvidence("myCustomPopupPlacement.Equals(null) = false");
                TestLog.Current.LogEvidence("myCustomPopupPlacement.GetHashCode() = " + myCustomPopupPlacement.GetHashCode());
                TestLog.Current.LogEvidence("myCustomPopupPlacement.GetType().ToString() = " + myCustomPopupPlacement.GetType().ToString());
            }
            myCustomPopupPlacement.Point = new Point(10, 10);
            myCustomPopupPlacement.PrimaryAxis = PopupPrimaryAxis.Horizontal;

            // if (mouseRightButtonUpInvoked && mouseRightButtonDownInvoked && openedInvoked && closedInvoked && PopupAnimation.Fade == popup.PopupAnimation)
            if (openedInvoked && closedInvoked && PopupAnimation.Fade == popup.PopupAnimation)
            {
                return TestResult.Pass;
            }
            else
            {
                TestLog.Current.LogDebug("mouseRightButtonUpInvoked = " + mouseRightButtonUpInvoked);
                TestLog.Current.LogDebug("mouseRightButtonDownInvoked = " + mouseRightButtonDownInvoked);
                return TestResult.Fail;
            }
        }

        private CustomPopupPlacement[] myCustomPopupPlacementCallback(Size size, Size target, Point offset)
        {
            TestLog.Current.LogDebug("myCustomPopupPlacementCallback function got called");
            CustomPopupPlacement myCustomPopupPlacement = new CustomPopupPlacement();
            myCustomPopupPlacement.Point = new Point(10, 10);

            CustomPopupPlacement[] result = new CustomPopupPlacement[1];
            result[0] = myCustomPopupPlacement;
            return result;
        }

        void popup_Opened(object sender, EventArgs e)
        {
            openedInvoked = true;
        }

        void popup_Closed(object sender, EventArgs e)
        {
            closedInvoked = true;
        }

        void myPopup_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            mouseRightButtonUpInvoked = true;
        }

        void myPopup_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            mouseRightButtonDownInvoked = true;
        }
    }

    internal class BackgroundWindow : IDisposable
    {
        HwndSource source;


        public BackgroundWindow() : this(NativeConstants.HWND_BOTTOM)
        {
        }

        public BackgroundWindow(IntPtr beforeHwnd)
        {
            IntPtr activeWindow = NativeMethods.GetActiveWindow();

            HwndSourceParameters param = new HwndSourceParameters("Background Window", 3000, 3000);
            param.SetPosition(-100, -100);


            //set WindorStyle for Borderless window
            param.WindowStyle = unchecked((int)0x90000000);

            source = new HwndSource(param);

            DrawingVisual parentV = new DrawingVisual();
            using (DrawingContext ctx = parentV.RenderOpen())
            {
                ctx.DrawGeometry(Brushes.White, null, new RectangleGeometry(new Rect(-100, -100, 3000, 3000)));

            }

            source.RootVisual = parentV;
            
            NativeMethods.SetWindowPos(source.Handle, beforeHwnd, -100, -100, 3000, 3000, 0);
            
            //restore the active window
            if (activeWindow != IntPtr.Zero)
                NativeMethods.SetActiveWindow(new HandleRef(this, activeWindow));
        }

        public void Dispose()
        {
            source.Dispose();
        }


    }

}



