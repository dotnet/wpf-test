// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Drawing;
using System.Threading; 
using System.Windows.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Test.Loaders;
using Microsoft.Test.Win32;
using Microsoft.Test.Loaders.Steps;
using Microsoft.Test.Logging;
// using Microsoft.Test.Internal;
using System.Windows.Automation;
using Microsoft.Test.RenderingVerification;
using Microsoft.Test.RenderingVerification.Model.Analytical;
using Microsoft.Test.RenderingVerification.Model.Synthetical;
using Microsoft.Test.CrossProcess;

namespace Microsoft.Test.Deployment
{
	/// <summary>
	/// UIHandler to do visual verification of various applications based on being given a small .bmp of 
	/// content that needs to be present to pass the test.  Do not use large .bmps as this runs prohibitively slow.
	/// </summary>
	public class VisualVerifier : UIHandler
    {
        #region Private Members
        private static readonly string s_screenCapFN = "clientwin.bmp";
        private string _compareImage = null;
        private int _imageMatches = 1;
        private bool _negativeTest = false;

        /// <summary>
        /// Testlog instance for Visual Verifier.  
        /// </summary>
        protected TestLog TestLog;
        #endregion
        
        #region Constructor
        /// <summary>
        /// Constructor: 
        /// </summary>
        public VisualVerifier()
		{
			GlobalLog.LogDebug("Entering the constructor for " + this.ToString());
        }
        #endregion

        #region Public Members
        /// <summary>
        /// Image file to compare to when doing VScan verification
        /// Needs to be in same directory as AppMonitor.exe is.
        /// </summary>
        /// <value></value>
        public string CompareImage
        {
            get
            {
                return this._compareImage;
            }
            set
            {
                this._compareImage = value;
            }
        }

        /// <summary>
        /// Number of times the reference image should be found in the actual image
        /// </summary>
        /// <value></value>
        public int ImageMatches
        {
            get
            {
                return _imageMatches;
            }
            set
            {
                _imageMatches = value;
            }
        }

        /// <summary>
        /// Value representing whether the test is looking for the negative (i.e. it SHOULDNT find X instances of the image)
        /// Defaults to false.  Needed for 
        /// </summary>
        /// <value></value>
        public bool NegativeTest
        {
            get
            {
                return this._negativeTest;
            }
            set
            {
                this._negativeTest = value;
            }
        }

        /// <summary>
        /// Tolerance value to use for image comparison
        /// </summary>
        public double Tolerance = 0.02;

        /// <summary>
        /// Boolean value representing whether to search for compare image within the reference image
        /// or to compare client window areas.  Default: false (search)
        /// </summary>
        public bool WindowCompare = false;

        /// <summary>
        ///  Integer value representing height to set the window size to before comparison (when WindowCompare = true)
        /// </summary>
        public int WindowHeight = 0;

        /// <summary>
        ///  Integer value representing height to set the window size to before comparison (when WindowCompare = true)
        /// </summary>
        public int WindowWidth = 0;

        /// <summary>
        ///  Determines whether the screen capture will be of the RBW area of the window (no title, frame)
        ///  or of the whole window (including stuff like IE menus, frames, etc).  Will fail if there is no RBW.
        /// </summary>
        public bool BrowserContentOnly = false;

        #endregion

        #region Implementation
        /// <summary>
        /// Calls base HandleWindow(), then cleans up any extra instances of IE that may be left over
        /// </summary>
        /// <param name="topLevelhWnd"></param>
        /// <param name="hwnd"></param>
        /// <param name="process"></param>
        /// <param name="title"></param>
        /// <param name="notification"></param>
        /// <returns></returns>
        public override UIHandlerAction HandleWindow(System.IntPtr topLevelhWnd, System.IntPtr hwnd, System.Diagnostics.Process process, string title, UIHandlerNotification notification)
        {
            DictionaryStore.Current["OtherHandlerExecuting"] = "VisualVerifier";

            this.TestLog = TestLog.Current;
            if (this.TestLog == null)
            {
                throw new InvalidOperationException("Must be run in the context of a test log");
            }

            AutomationElement thisWindow = AutomationElement.FromHandle(topLevelhWnd);
            if (WindowHeight + WindowWidth > 0)
            {
                object patternObject;
                thisWindow.TryGetCurrentPattern(WindowPattern.Pattern, out patternObject);
                WindowPattern wp = patternObject as WindowPattern;
                wp.SetWindowVisualState(WindowVisualState.Normal);
                patternObject = null;
                thisWindow.TryGetCurrentPattern(TransformPattern.Pattern, out patternObject);
                TransformPattern tp = patternObject as TransformPattern;

                tp.Move(0, 0);
                tp.Resize(WindowWidth, WindowHeight);
                GlobalLog.LogEvidence("Moved window to top left corner and resized to " + WindowWidth + " x " + WindowHeight);
            }

            GlobalLog.LogEvidence("Sleeping 15 seconds to allow content to render");
            // Sleep for a while to let stuff finish rendering on the slower machines
            Thread.Sleep(15000);

            // Native structures for calling GetWindowRect
            NativeStructs.RECT nativeWindowRect = new NativeStructs.RECT(0, 0, 0, 0);
            HandleRef windowHandle = new HandleRef(this, topLevelhWnd);

            if (BrowserContentOnly)
            {
                AutomationElement rbw = thisWindow.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.ClassNameProperty, "RootBrowserWindow"));
                if (rbw != null)
                {
                    System.Windows.Rect bounds = (System.Windows.Rect)rbw.GetCurrentPropertyValue(AutomationElement.BoundingRectangleProperty);
                    nativeWindowRect = new NativeStructs.RECT((int)bounds.Left, (int)bounds.Top, (int)bounds.Right, (int)bounds.Bottom);
                }
                else
                {
                    GlobalLog.LogEvidence("Fail: Unable to get RBW area of handled window");
                    return UIHandlerAction.Abort;
                }
            }
            else
            {
                // Get the window rect of the window that started this event
                if (Microsoft.Test.Win32.NativeMethods.GetWindowRect(windowHandle, ref nativeWindowRect) == false)
                    // Quit if we can't verify the window we're interested in.
                    return UIHandlerAction.Abort;
            }
            if (WindowCompare)
            {
                return compareWindow(nativeWindowRect, topLevelhWnd);
            }
            else
            {
                return searchForReferenceImage(nativeWindowRect, topLevelhWnd);
            }
		}
        #endregion

        #region Private Implementation

        private UIHandlerAction searchForReferenceImage(NativeStructs.RECT nativeWindowRect, System.IntPtr topLevelhWnd)
        {
            // Take the top slice of the client window.  (Speeds up verification)
            Rectangle windowRect = new Rectangle(nativeWindowRect.left, nativeWindowRect.top, 640, 550);

            // Capture screen image...
            ImageUtility.CaptureScreen(windowRect).Save(s_screenCapFN);
            GlobalLog.LogEvidence("Took screen capture of client window...");
            GlyphModel VSModel = new GlyphModel();
            System.Drawing.Bitmap screenCap = new Bitmap(s_screenCapFN);
            VSModel.Target = new ImageAdapter(screenCap);
            GlyphImage expected = new GlyphImage(VSModel);
            //expected.Path = compareImage;
            expected.ImageToUse = new ImageAdapter(_compareImage);
            expected.CompareInfo.MaxMatch = _imageMatches;
            expected.CompareInfo.ErrorMax = Tolerance;

            GlobalLog.LogEvidence("Looking for matches in screen shot and reference image...");
            VSModel.MatchAll();

            GlobalLog.LogDebug("Iterating through GlyphBase");
            foreach (GlyphBase glb in VSModel.Glyphs)
            {
                if ((glb.CompareInfo.Matches.Count > 0) && !NegativeTest)
                {
                    GlobalLog.LogEvidence("Found a matching image:");
                    if (glb.CompareInfo.Matches.Count == _imageMatches)
                    {
                        GlobalLog.LogEvidence("Correct Number of instances found: Visual Verification Passed");
                        this.TestLog.Result = TestResult.Pass;
                    }
                    else
                    {
                        GlobalLog.LogEvidence("Found " + glb.CompareInfo.Matches.Count + " instances where there should have been " + _imageMatches + ". Failed.");
                        GlobalLog.LogEvidence("Included failed screen capture with log files.");
                        this.TestLog.Result = TestResult.Fail;
                    }
                }
                else if (!NegativeTest)
                {
                    GlobalLog.LogEvidence("Could not find a matching image : Visual verification failed");
                    GlobalLog.LogEvidence("Included failed screen capture with log files.");
                    this.TestLog.Result = TestResult.Fail;
                }
                // We're doing a negative test... pass if the matching count is 0, fail otherwise
                else
                {
                    if (glb.CompareInfo.Matches.Count == 0)
                    {
                        GlobalLog.LogEvidence("Negative test passed: no instances of reference image found");
                        this.TestLog.Result = TestResult.Pass;
                    }
                    else
                    {
                        GlobalLog.LogEvidence("Negative test failed: One or more instances of reference image found");
                        this.TestLog.Result = TestResult.Fail;
                    }
                    // Get the highest level window (may be same as current) and close it
                    AutomationElement windowToClose = AutomationElement.FromHandle(topLevelhWnd);
                    object patternObject;
                    windowToClose.TryGetCurrentPattern(WindowPattern.Pattern, out patternObject);
                    WindowPattern wp = patternObject as WindowPattern;
                    wp.Close();
                    GlobalLog.LogDebug("Closed main window");

                    // Abort for negative tests... we just want to see if the image rendered
                    return UIHandlerAction.Abort;
                }
            }

            if (this.TestLog.Result == TestResult.Fail)
            {
                MemoryStream pictureStream = new MemoryStream();
                screenCap.Save(pictureStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                GlobalLog.LogFile("errorscreen.jpg", pictureStream);
            }
            DictionaryStore.Current["OtherHandlerExecuting"] = null;

            // Once the window has been handled, return Unhandled so the next UIHandler gets a chance.
            return UIHandlerAction.Unhandled;
        }

        private UIHandlerAction compareWindow(NativeStructs.RECT nativeWindowRect, System.IntPtr topLevelhWnd)
        {
            // Calculate rectangle of window area...
            Rectangle windowRect = new Rectangle(nativeWindowRect.left, nativeWindowRect.top, 
                 (nativeWindowRect.right - nativeWindowRect.left), 
                 (nativeWindowRect.bottom- nativeWindowRect.top));

            // Capture screen image...
            ImageUtility.CaptureScreen(windowRect).Save(s_screenCapFN);
            System.Drawing.Bitmap screenCap = new Bitmap(s_screenCapFN);
            GlobalLog.LogEvidence("Took screen capture of client window...");

            if (!File.Exists(CompareImage))
            {
                this.TestLog.Result = TestResult.Fail;
                GlobalLog.LogEvidence("Failing because reference image (" + CompareImage + ") did not exist.");
                GlobalLog.LogEvidence("Included screencap.png with the log files for reference image generation.");
                MemoryStream pictureStream = new MemoryStream();
                screenCap.Save(pictureStream, System.Drawing.Imaging.ImageFormat.Png);
                GlobalLog.LogFile("screencap.png", pictureStream);
                return UIHandlerAction.Abort;
            }

            ImageAdapter referenceImage = new ImageAdapter(CompareImage);
            ImageAdapter clientImage = new ImageAdapter(s_screenCapFN);

            ImageComparator ic = new ImageComparator();
            ic.FilterLevel = 0;
            ic.ChannelsInUse = ChannelCompareMode.ARGB;

            bool same = ic.Compare(referenceImage, clientImage, true);

            if (same)
            {
                GlobalLog.LogEvidence("Image comparison succeeded.");
                this.TestLog.Result = TestResult.Pass;
            }
            else
            {
                GlobalLog.LogEvidence("Image comparison failed.  Included screen capture with log files.(errorscreen.png)");
                this.TestLog.Result = TestResult.Fail;
            }

            if (this.TestLog.Result == TestResult.Fail)
            {
                MemoryStream pictureStream = new MemoryStream();
                screenCap.Save(pictureStream, System.Drawing.Imaging.ImageFormat.Png);
                GlobalLog.LogFile("errorscreen.png", pictureStream);
            }
            DictionaryStore.Current["OtherHandlerExecuting"] = null;

            // This keeps getting IEExceptions trying to access ClientWin.bmp... clean up to try to avoid this
            screenCap.Dispose();

            // This flavor is the whole verification, so abort from here...
            return UIHandlerAction.Abort;
        }
        #endregion
    }
}
