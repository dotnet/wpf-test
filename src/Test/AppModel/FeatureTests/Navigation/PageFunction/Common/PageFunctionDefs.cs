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
using System.Windows.Documents;
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
using Microsoft.Test.RenderingVerification; 
using Microsoft.Test.RenderingVerification.Model.Analytical;


namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    public class ChildPF_Frame_String : PageFunction<string>
    {
        Frame _frContent;

        public ChildPF_Frame_String ()
        {
            NavigationHelper.Output ("Creating a string child pagefunction");
            _frContent = new Frame ();
            _frContent.Name = "PFContentFrame";
            _frContent.Navigate (new Uri( "PFContent.xaml", UriKind.RelativeOrAbsolute), true); //synchronous navigation
        }

        public bool CheckState ()
        {
            if (_frContent == null)
            {
                NavigationHelper.Fail ("could not find the frame in the pf");
                return false;
            }

            //if (frContent.Uri.

            if (((FrameworkElement)_frContent.Content).Name != "cvsPageFunc")
            {
                NavigationHelper.Fail ("Content in pagefunction frame was wrong");
                return false;
            }

            if (!HitTestVerify ())
            {
                NavigationHelper.Fail ("HitTest failed on the frame");
                return false;
            }

            if (!UIAutomationVerify ())
            {
                NavigationHelper.Fail ("UiAutomation verification failed on the frame");
                return false;
            }

            if (!VisualVerify() ) {
                NavigationHelper.Fail("Visual Verification failed.");
                return false;
            } else {
                NavigationHelper.Output("Visual Verification passed.");
            }

            return true;
        }

        private bool HitTestVerify ()
        {
            NavigationHelper.Output("Doing HitTest Verification for ChildPF_Frame_String");

            //Rect _clientrect = //Bag.GetClientAreaRect(PageFunctionTestApp.CurrentPFTestApp.MainNavWindow);
            //NavigationHelper.Output("Client coords of main window are: "
            //    + _clientrect.Left + "," + _clientrect.Top
            //    + " "
            //    + _clientrect.Right + "," + _clientrect.Bottom);
            Point pt = new Point(/*_clientrect.Left + */ 40, /*_clientrect.Top + */ 40);
            NavigationHelper.Output("HitTesting point " + pt.ToString());
            Visual vis =
                (Visual)VisualTreeHelper.HitTest((System.Windows.Media.Visual)PageFunctionTestApp.CurrentPFTestApp.MainNavWindow.Content, pt).VisualHit;
            if ((vis is DockPanel) && ((DockPanel)vis).Name == "cvsPageFunc") {
                NavigationHelper.Output("Found expected visual at position " + pt.ToString());
                return true;
            } else {
                NavigationHelper.Fail("Found unexpected visual: " + vis.GetType().ToString());
                return false;
            }
        }

        private bool UIAutomationVerify ()
        {
            //
            return true;
        }

        private bool VisualVerify () {
//            NavigationHelper.Output("Doing Visual Verification");
//            // VScan code
            //            Rect _clientrect = Bag.GetClientAreaRect(PageFunctionTestApp.CurrentPFTestApp.MainNavWindow);
//
//            NavigationHelper.Output("Client coords of main window are: "
//                + _clientrect.Left + "," + _clientrect.Top
//                + " "
//                + _clientrect.Right + "," + _clientrect.Bottom);
//
//            System.Drawing.Rectangle r = new System.Drawing.Rectangle ((int)(_clientrect.Left + 40), (int)(_clientrect.Top), 50, 50);
//            System.Drawing.Bitmap windowImage = ImageUtility.CaptureScreen (r);





            //windowImage.Save ("ChildPF_Frame_String.bmp", System.Drawing.Imaging.ImageFormat.Bmp);

//            NavigationHelper.Output("Comparing the captured image for this pf to the baseline image");

//            ImageComparator imageCompare = new ImageComparator();
//            ImageAdapter imageSnapshot = new ImageAdapter(windowImage);
//            ImageAdapter imageSource = new ImageAdapter(new BitmapSW("ChildPF_Frame_String.bmp").InnerObject);

//            NavigationHelper.Output("Image comparison commencing...");
//            if (imageCompare.Compare(imageSnapshot, imageSource))
//                NavigationHelper.Output("Image comparison succeeded");
//            else
//                NavigationHelper.Fail("Image comparison failed");
            return true;
        }
    }

    public class ChildPF_Frame_Int : PageFunction<int>
    {
        Frame _frContent;

        public ChildPF_Frame_Int ()
        {
            NavigationHelper.Output ("Creating a Int child pagefunction");
            _frContent = new Frame ();
            _frContent.Name = "PFContentFrame";
            _frContent.Navigate (new Uri( "PFContent.xaml", UriKind.RelativeOrAbsolute), true); //synchronous navigation
        }

        public bool CheckState ()
        {
            if (_frContent == null)
            {
                NavigationHelper.Fail ("could not find the frame in the pf");
                return false;
            }

            //if (frContent.Uri.
            if (((FrameworkElement)_frContent.Content).Name != "cvsPageFunc")
            {
                NavigationHelper.Fail ("Content in pagefunction frame was wrong");
                return false;
            }

//          if (!VisualVerify ())
//          {
//              return false;
//          }

            if (!HitTestVerify ())
            {
                NavigationHelper.Fail ("HitTest failed on the frame");
                return false;
            }

            if (!UIAutomationVerify ())
            {
                NavigationHelper.Fail ("UiAutomation verification failed on the frame");
                return false;
            }

            return true;
        }

        private bool HitTestVerify ()
        {
            //todo
            return true;
        }

        private bool UIAutomationVerify ()
        {
            //todo
            return true;
        }

        public void RequestFinish ()
        {
            ReturnEventArgs<int> rargs = new ReturnEventArgs<int> ();
            rargs.Result = PageFunctionTestApp.DEFAULT_CHILDPF_FRAME_INT_RETVAL;
            OnReturn(rargs);
        }

        protected override void Start ()
        {
            NavigationHelper.Output ("Start method override called on " + this.GetType ().ToString ());

            Hashtable appeventsht = PageFunctionTestApp.CurrentPFTestApp.TestEventsHT;

            if (appeventsht.ContainsKey ("Numtimes_startcalled_*ChildPF_Frame_Int"))
            {
                int istart = (int)appeventsht["Numtimes_startcalled_*ChildPF_Frame_Int"];

                appeventsht["Numtimes_startcalled_*ChildPF_Frame_Int"] = istart++;
            }
            else
            {
                appeventsht["Numtimes_startcalled_*ChildPF_Frame_Int"] = 1;
            }
        }
    }

    // supports:
    // hit test verification
    // visual verification

    public class ChildPF_String : PageFunction<string>
    {
        Path _El;
        DockPanel _dp;

        public ChildPF_String ()
        {
            NavigationHelper.Output ("Creating a string child pagefunction");
            _dp = new DockPanel();
            _dp.Background = Brushes.Orange;

            _El = new System.Windows.Shapes.Path();
            EllipseGeometry elGeometry = new EllipseGeometry(new Point(100, 150), 50, 45);
            _El.Data = elGeometry; 

            _El.Fill = new LinearGradientBrush (Colors.LightCoral, Colors.White, 90);

            _dp.Children.Add(_El);
            Content = _dp;
        }

        public bool CheckState ()
        {
            if (_El == null)
            {
                NavigationHelper.Fail ("Could not find the path in the pf");
                return false;
            }

            if (Content != _dp)
            {
                NavigationHelper.Fail ("Content in pagefunction frame was wrong");
                return false;
            }
            else
            {
                NavigationHelper.Output ("Content in the PF was correct");
            }


            if (!HitTestVerify ())
            {
                NavigationHelper.Fail ("HitTest failed on the PageFunction");
                return false;
            }

            if (!UIAutomationVerify ())
            {
                NavigationHelper.Fail ("UiAutomation verification failed on the PageFunction");
                return false;
            }

            return true;
        }

        private bool HitTestVerify ()
        {
            NavigationHelper.Output("Doing HitTest Verification for ChildPF_String");

            Point pt = new Point(40,40);

            Visual vis =
                (Visual)VisualTreeHelper.HitTest((System.Windows.Media.Visual)PageFunctionTestApp.CurrentPFTestApp.MainNavWindow.Content, pt).VisualHit;
            if (vis == _dp) {
                NavigationHelper.Output("Found expected visual at position " + pt.ToString());
                return true;
            } else {
                NavigationHelper.Fail("Found unexpected visual: " + vis.GetType().ToString());
                return false;
            }
        }

        private bool UIAutomationVerify ()
        {
            //
            return true;
        }

        public void RequestFinish ()
        {
            ReturnEventArgs<string> rargs = new ReturnEventArgs<string> ();
            rargs.Result = "returning";
            OnReturn(rargs);
        }

    }

    public class ParentBoolPF : PageFunction<bool>
    {
        public bool IsActive
        {
            get
            {
                return true;
            }
        }

        public DateTime CreateTime
        {
            get
            {
                return _creationtime;
            }
        }

        public int foodata = 5;

        private DateTime _creationtime;

        private DockPanel _dpbool;
        public TextBlock tt;

        public ParentBoolPF ()
        {
            NavigationHelper.Output ("Creating a parent boolean pf");
            _dpbool = new DockPanel();
            _dpbool.Background= Brushes.SteelBlue;
            tt = new TextBlock();
            _dpbool.Children.Add(tt);

            Content = _dpbool;
            _creationtime = DateTime.Now;
        }

        public void NavChild (NavigationWindow navwin)
        {
            ChildPF_String pfchild = new ChildPF_String ();

            if (pfchild == null)
            {
                NavigationHelper.Fail ("child pf is null");
                return;
            }

            pfchild.Return += new ReturnEventHandler<string> (OnChildFinish);
            navwin.Navigate (pfchild);
        }

        public void NavChild (NavigationWindow navwin, bool fRemoveFromJournal)
        {
            ChildPF_String pfchild = new ChildPF_String ();
            if (!fRemoveFromJournal)
            {
                pfchild.RemoveFromJournal = false;
            } else {
                pfchild.RemoveFromJournal = true;
            }

            if (pfchild == null)
            {
                NavigationHelper.Fail ("child pf is null");
                return;
            }

            pfchild.Return += new ReturnEventHandler<string> (OnChildFinish);
            navwin.Navigate (pfchild);
        }


        public void OnChildFinish (object sender, ReturnEventArgs<string> args)
        {
            NavigationHelper.Output ("Child pf RETURNED: " + args.Result);
        }

        public bool CheckState ()
        {
            if (!(Content is DockPanel)) return false;


            if (!HitTestVerify ())
            {
                NavigationHelper.Fail ("HitTest failed on the PageFunction");
                return false;
            }

            if (!UIAutomationVerify ())
            {
                NavigationHelper.Fail ("UiAutomation verification failed on the PageFunction");
                return false;
            }

            return true;
        }

        private bool HitTestVerify ()
        {
            NavigationHelper.Output("Doing HitTest Verification");

            Point pt = new Point(140,140);

            Visual vis =
                (Visual)VisualTreeHelper.HitTest((System.Windows.Media.Visual)PageFunctionTestApp.CurrentPFTestApp.MainNavWindow.Content, pt).VisualHit;
            if (vis == tt) {
                NavigationHelper.Output("Found expected visual at position " + pt.ToString());
                return true;
            } else {
                NavigationHelper.Fail("Found unexpected visual: " + vis.GetType().ToString() + " at " + pt.ToString());
                return false;
            }
        }

        private bool UIAutomationVerify ()
        {
            //
            return true;
        }

    }

    public class StartMethodTestParentPF : PageFunction<object>
    {
        Hyperlink _hContent;
        public StartMethodTestParentPF ()
        {
            NavigationHelper.Output ("Creation of : " + this.GetType().ToString());
            TextBlock tb = new TextBlock();
            _hContent = new Hyperlink(new Run("Start Method Test Parent PageFunction"), tb.ContentEnd);
            Content = tb;
        }
        protected override void Start ()
        {
            NavigationHelper.Output ("Start method override called on " + this.GetType().ToString());
            Hashtable appeventsht = PageFunctionTestApp.CurrentPFTestApp.TestEventsHT;
            if (appeventsht.ContainsKey("Numtimes_startcalled"))
            {
                int istart = (int)appeventsht["Numtimes_startcalled"];
                appeventsht["Numtimes_startcalled"]
                    = istart++;
            }
            else
            {
                appeventsht["Numtimes_startcalled"] = 1;
            }

        }

        public void NavChild (NavigationWindow navwin)
        {
            NavigationHelper.Output ("Navigating " + this.GetType ().ToString () + "to child pf");
            ChildPF_Frame_Int pfchild = new ChildPF_Frame_Int ();

            if (pfchild == null)
            {
                NavigationHelper.Fail ("child pf is null");
                return;
            }

            pfchild.Return += new ReturnEventHandler<int> (OnChildFinish);
            navwin.Navigate (pfchild);
        }

        public void NavChild (NavigationWindow navwin, bool fRemoveFromJournal)
        {
            NavigationHelper.Output ("Navigating " + this.GetType().ToString() + "to child pf");
            ChildPF_Frame_Int pfchild = new ChildPF_Frame_Int();

            if (!fRemoveFromJournal)
            {
                pfchild.RemoveFromJournal = false;
            }

            if (pfchild == null)
            {
                NavigationHelper.Fail ("child pf is null");
                return;
            }

            pfchild.Return += new ReturnEventHandler<int> (OnChildFinish);
            navwin.Navigate (pfchild);
        }

        public void OnChildFinish (object sender, ReturnEventArgs<int> args)
        {
            NavigationHelper.Output ("Child pf RETURNED: " + args.Result);
        }

        public bool CheckState ()
        {
            if (!(Content is TextBlock)) {
                NavigationHelper.Fail("This StartMethodTestParentPF's state check failed");
                return false;
            }

            return true;
        }

    }

    public class UILessParentPF : PageFunction<object>
    {

        public UILessParentPF ()
        {
            NavigationHelper.Output ("Creation of uiless parent pf: " + this.GetType ().ToString ());
        }

        protected override void Start ()
        {
            NavigationHelper.Output ("Start method override called on " + this.GetType ().ToString ());

            Hashtable appeventsht = PageFunctionTestApp.CurrentPFTestApp.TestEventsHT;

            if (appeventsht.ContainsKey ("Numtimes_startcalled*uilessparentPF"))
            {
                int istart = (int)appeventsht["Numtimes_startcalled*uilessparentPF"];
                appeventsht["Numtimes_startcalled*uilessparentPF"] = istart++;
                if (((int)appeventsht["Numtimes_startcalled*uilessparentPF"]) > 1) //which it will always be, so we strictly don't need this check
                {
                    NavigationHelper.Fail ("Start method on UILess Pf called more than once: "
                    + (int)appeventsht["Numtimes_startcalled*uilessparentPF"]);
                }
                return;
            }
            else
            {
                appeventsht["Numtimes_startcalled*uilessparentPF"] = 1;
            }
            //to start off, show child pf 1
            NavChild(PageFunctionTestApp.CurrentPFTestApp.MainNavWindow, 1);

        }

        public void NavChild (NavigationWindow navwin, int navchildnumber)
        {
            NavigationHelper.Output ("Navigating " + this.GetType ().ToString () + " to child pf");

            switch (navchildnumber)
            {
                // renavigate to another child when one child finishes
                case 1:
                    ChildPF_Frame_Int pfchild = new ChildPF_Frame_Int ();
                    if (pfchild == null)
                    {
                        NavigationHelper.Fail ("child pf is null");
                        return;
                    }
                    pfchild.Return += new ReturnEventHandler<int> (OnChildFinish_IntRenav);
                    navwin.Navigate (pfchild);
                    break;
                // just navigate to child and when child finishes check retval
                case 2:
                    ChildPF_String pfchild2 = new ChildPF_String ();
                    if (pfchild2 == null)
                    {
                        NavigationHelper.Fail ("string child pf is null");
                        return;
                    }

                    pfchild2.Return += new ReturnEventHandler<string> (OnChildFinish_String);
                    navwin.Navigate (pfchild2);
                    break;

                // finish self when child finishes
                case 3:
                    //
                    break;

                case 4:
                    break;

            }
        }

        public void OnChildFinish_IntRenav (object sender, ReturnEventArgs<int> args)
        {
            NavigationHelper.Output ("Child pf RETURNED: " + args.Result);
            IntRetVal = args.Result;

            if (IntRetVal != PageFunctionTestApp.DEFAULT_CHILDPF_FRAME_INT_RETVAL)
            {
                NavigationHelper.Fail("Incorrect return value from child pagefunction");
            }

            NavigationHelper.Output("UILess PF navigating to another child");

            NavChild(PageFunctionTestApp.CurrentPFTestApp.MainNavWindow, 2);
        }

        public void OnChildFinish_String (object sender, ReturnEventArgs<string> args)
        {
            NavigationHelper.Output ("Child pf RETURNED: " + args.Result);
            StringRetVal = args.Result;
        }

        public bool CheckState ()
        {
            return true;
        }

        public int IntRetVal {
            set
            {
                _intreturnval = value;
            }

            get
            {
                return _intreturnval;
            }
        }

        public string StringRetVal {
            set
            {
                _stringreturnval = value;
            }

            get
            {
                return _stringreturnval;
            }
        }


        private int _intreturnval = -1;
        private string _stringreturnval = String.Empty;
    }
}
