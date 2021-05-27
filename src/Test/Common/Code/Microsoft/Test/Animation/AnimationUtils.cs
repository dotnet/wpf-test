// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Microsoft.Test.RenderingVerification;

namespace Microsoft.Test.Animation
{

    /// <summary>
    /// TickEvent delegate
    /// </summary>
    public delegate void TickEvent(object sender,TimeControlArgs e);

    /// <summary>
    /// TimeControlArgs EventArgs
    /// </summary>
    public class TimeControlArgs : EventArgs
    {
        /// <summary>
        /// the currentTime of the system
        /// </summary>
        public int curTime;

        /// <summary>
        /// is this the last tick in your supplied list
        /// </summary>
        public bool lastTick = false;

    }

    /// <summary>
    /// Bounding box properties
    /// </summary>
    public struct BoundingBoxProperties
    {
        /// <summary>
        /// Top
        /// </summary>
        public float top;

        /// <summary>
        /// Left
        /// </summary>
        public float left;

        /// <summary>
        /// Right
        /// </summary>
        public float right;

        /// <summary>
        /// Bottom
        /// </summary>
        public float bottom;

        /// <summary>
        /// Width
        /// </summary>
        public float width;

        /// <summary>
        /// Height
        /// </summary>
        public float height;
    }


    /// <summary>
    /// VisualVerify method for getting color from a point on the screen
    /// </summary>
    public class VisualVerifier
    {

        /// <summary>
        /// window hWnd
        /// </summary>
        public IntPtr hWnd
        {
            get { return internalHWnd; }
            set { internalHWnd = value; }
        }

        /// <summary>
        /// Visual Verify comments
        /// </summary>
        public String vvComments
        {
            get { return internalComments; }
            set { internalComments = value; }
        }
        
        // InitRender.  It must be called by a test case after the Navigation Window is created.
        /// <summary>
        /// InitRender sets the internal hWnd using a point 50 pixels from the top left of the window. This ensures a point within the window
        /// </summary>
        public void InitRender(Window win)
        {
            WindowInteropHelper windowInteropHelper = new WindowInteropHelper(win);
            internalHWnd = windowInteropHelper.Handle;
        }


        private void logComment(String comment)
        {
            internalComments = internalComments + "\n" + comment;

        }

        //Translates sRGB (Nonlinear) color value to scRGB (Linear).
        //The input parameter (val) indicates the progress of a Color Animation from 0.0 to 1.0.
        //Avalon interpolates in sRGB which is nonlinear, so the value returned by
        //roboHelper.GetColorAtPos must be translated from sRGB to scRGB in order to match the
        //expected progress based on a Color Animation's duration.
        /// <summary>
        /// Convert sRGB TO SCRGB
        /// </summary>
        public float NonlinearToLinear(float val)
        {
            if (val <= 0.0)
                return (0.0f);
            else if (val < 0.081)
                return (val / 4.5f);
            else if (val < 1.0f)
                return (float)Math.Pow(((double)val+0.099)/1.099, 1.0/0.45);
            else
                return (1.0f);
        }

        //Translates scRGB (Linear) color value to sRGB (Nonlinear).
        /// <summary>
        /// Convert scRGB to sRGB
        /// </summary>
        public float LinearToNonlinear(float val)
        {
            if (val <= 0.0)
                return(0.0f);
            else if (val < 0.018)
                return(val * 4.5f);
            else if (val < 1.0)
                return((1.099f*(float)Math.Pow((double)val, 0.45))-0.099f);
            else
                return(1.0f);
        }


        /// <summary>
        /// Return the color on the screen at a certain point
        /// </summary>
        public System.Windows.Media.Color getColorAtPoint(int X, int Y)
        {

            this.screen  = ImageUtility.CaptureScreen(internalHWnd, true);

            System.Drawing.Color myColor = System.Drawing.Color.FromArgb(0x00, 0x00, 0x00, 0x00);

            try
            {
                myColor = screen.GetPixel(X,Y);
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                logComment(" Error in ImageUtils: " + ex); return Colors.Black;
            }

            logComment(" Color at position " + X.ToString() + " " + Y.ToString() + " was " + myColor.A.ToString() + " " + myColor.R.ToString()+ " " + myColor.G.ToString()+ " " + myColor.B.ToString());

            System.Windows.Media.Color returnColor = System.Windows.Media.Color.FromArgb(myColor.A,myColor.R,myColor.G,myColor.B);

            return returnColor;
         
        }


        /// <summary>
        /// get the bounding box properties for a particular color on the screen
        /// </summary>
        public BoundingBoxProperties getBoundingBoxProperties(System.Windows.Media.Color myColor)
        {

            BoundingBoxProperties rValue = new BoundingBoxProperties();
            rValue.top = rValue.left = rValue.right = rValue.bottom = rValue.width = rValue.height = 0;
            System.Drawing.Color checkColor;

            checkColor = System.Drawing.Color.FromArgb(myColor.A,myColor.R,myColor.G,myColor.B);

            this.screen  = ImageUtility.CaptureScreen(internalHWnd, true);

            try
            {
                System.Drawing.Rectangle myRect = ImageUtility.GetBoundingBoxForColor(this.screen, checkColor);

                rValue.top = myRect.Top;
                rValue.left = myRect.Left;
                rValue.right = myRect.Right;
                rValue.bottom = myRect.Bottom;
                rValue.width = myRect.Width;
                rValue.height = myRect.Height;
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                logComment(" Error in ImageUtils: " + ex);
            }

            logComment(" Bounding box for color: " + checkColor.A.ToString() + " " + checkColor.R.ToString()+ " " + checkColor.G.ToString()+ " " + checkColor.B.ToString());
            logComment(" L: " + rValue.left +  " R: " + rValue.right +  " T: " + rValue.top +  " B: " + rValue.bottom + " -- H: " + rValue.height + " W: " + rValue.width + "\n");

            return rValue;

        }

        //Finds an element in Markup, given its Name.
        /// <summary>
        /// Find a frameworkElement by it's ID
        /// </summary>
        public FrameworkElement FindElement(string id, UIElement root)
        {
            if (root == null) return null;
            FrameworkElement fe = root as FrameworkElement;

            if (fe.Name == id)
            {
                return fe;
            }

            int count = VisualTreeHelper.GetChildrenCount(root);
            for(int i = 0; i < count; i++)
            {
                // Common base class for Visual and Visual3D is DependencyObject
                DependencyObject child = VisualTreeHelper.GetChild(root,i);

                FrameworkElement feRet = FindElement(id, child as UIElement);
                if (feRet != null)
                    return feRet;
            }
            return null;
        }


        #region Private Members

        /// <summary>
        /// screen Bitmap
        /// </summary>
        private Bitmap screen = null;

        /// <summary>
        /// window hWnd
        /// </summary>
        private IntPtr internalHWnd;

        /// <summary>
        /// Visual Verify comments
        /// </summary>
        private String internalComments;


        #endregion

    }


    /// <summary>
    /// The public class for controling the Clock Manager
    /// </summary>
    public class ClockManager
    {
        private int[] timeArray;
        private Clock[] timeClocks;
        private int timeClockIndex = 0;
        private int timeArrayIndex = 0;


        /// <summary>
        /// static Tick Event
        /// </summary>
        public static event TickEvent Ticked;

        /// <summary>
        /// The InternalTimeManager associated with the aplication
        /// </summary>
        public InternalTimeManager hostManager;

        
        /// <summary>
        /// The CurrentTime of the system
        /// </summary>
        public Nullable<TimeSpan> CurrentTime
        {
            get
            {
                return hostManager.CurrentTime;
            }
        }

        /// <summary>
        /// ClockManager constructor
        /// </summary>
        public ClockManager(int[] hostTimeArray)
        {
            this.hostManager = new InternalTimeManager();
            hostManager.Pause();
            
            this.timeArray = hostTimeArray;
            setUpTicks();
        }

        // internally, hook up each tick requested
        private void setUpTicks()
        {
            this.timeClocks = new Clock[timeArray.Length];

            ParallelTimeline eventTimeline;

            foreach(int duration in timeArray)
            {
                eventTimeline = new ParallelTimeline();
                eventTimeline.BeginTime = null;
                eventTimeline.Duration = new Duration(TimeSpan.FromMilliseconds(duration));
                timeClocks[timeClockIndex] = eventTimeline.CreateClock();
                timeClocks[timeClockIndex].CurrentStateInvalidated += new EventHandler(OnEnded);

                // The last time in the list should be the only one with CurrentTimeInvalidated hooked up
                if (timeClockIndex == (timeArray.Length - 1))
                {
                    timeClocks[timeClockIndex].CurrentTimeInvalidated += new EventHandler(EnsureIndividualTick);
                }

                timeClocks[timeClockIndex].Controller.Begin();
                timeClockIndex ++;
            }
            timeClockIndex ++;
        }


        // Event handler for ticking
        private void OnEnded(object sender, EventArgs e)
        {
            if (((Clock)sender).CurrentState != ClockState.Active)
            {
                hostManager.Pause();

                //hostManager.Seek((int)timeClocks[timeArrayIndex].Timeline.Duration,TimeSeekOrigin.BeginTime);
                TimeControlArgs tca1 = new TimeControlArgs();
                tca1.curTime = hostManager.CurrentTime.HasValue ? (int)hostManager.CurrentTime.Value.TotalMilliseconds : 0;
                if (timeArrayIndex == (timeArray.Length - 1)) { tca1.lastTick = true; }
                this.FireTickEvent(tca1);

                hostManager.Resume();
                timeArrayIndex ++;
            }
        }

        private void EnsureIndividualTick(object sender, EventArgs e)
        {
            /*
                The purpose of the empty event is to ensure that the TimeManager ticks at every tick it possibly can.
                One artifact of how animation is validated is that in some *yet unknown* states, animation clocks
                fail to tick in between the ClockManager's ticks. This is directly related to how we validate animation

                The issue is being investigated, and a 


*/
        }

        /// <summary>
        /// fire a tick Event
        /// </summary>
        public void FireTickEvent(TimeControlArgs tca)
        {
            if(Ticked != null){ Ticked(this,tca); }
        }
    }
}
