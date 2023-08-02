// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

////////////////////////////////////////////////////////////////////////////////////////
/// 
/// Helper classes
/// 
/// ////////////////////////////////////////////////////////////////////////////////////

using System;
using System.IO;
using System.Xml;
using System.Security;
using System.Security.Permissions;

using System.Windows;
using System.Windows.Controls;
using System.Threading;
using System.Windows.Threading;

//using AvalonTools.FrameworkUtils;

using System.Collections;
using System.Drawing;
//using Microsoft.Test.Security.Wrappers;
//using Microsoft.Test.RenderingVerification;
using System.Windows.Media;
using System.Runtime.InteropServices;


namespace Avalon.Test.ComponentModel
{
/*
    /// <summary>
    /// Wrapper for AutomationFramework
    /// </summary>
    public static class AutomationFrwk
    {
        /// <summary>
        /// Create Framework component
        /// </summary>
        static AutomationFrwk()
        {
            // Create the Framework Component        
            frmwk = new AutomationFramework();
        }


        /// <summary>
        /// Log status
        /// </summary>
        /// <param name="str"></param>
        public static void Log(string str)
        {
            frmwk.Status(str);
        }

        /// <summary>
        /// Log Test result
        /// </summary>
        /// <param name="status"></param>
        /// <param name="str"></param>
        public static void LogTest(bool status, string str)
        {
            frmwk.LogTest(status, ((status == true) ? ("PASSED: ") : ("FAILED: ")) + str);
        }

        /// <summary>
        /// Log Test result, ShutDown the application
        /// </summary>
        /// <param name="status"></param>
        /// <param name="str"></param>
        public static void EndTest(bool status, string str)
        {
            frmwk.LogTest(status, ((status == true) ? ("PASSED: ") : ("FAILED: ")) + str);
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Log File
        /// </summary>
        /// <param name="status"></param>
        /// <param name="str"></param>
        public static void LogFile(string fileName)
        {
            frmwk.LogFile( fileName );
        }

        public static AutomationFramework FrwkObj
        {
            get
            {
                return frmwk;
            }
        }

        /// <summary>
        /// AutomationFramework
        /// </summary>
        private static AutomationFramework frmwk;

    }
*/
    
    /// <summary>
    /// Helper functions to block till all queue items are processed.
    /// </summary>
    public static class QueueHelper
    {

        /// <summary>
        /// Helper function to block till for a certain time.
        /// </summary>
        /// <param name="timeout">The time span to block for.</param>
        public static void WaitTillTimeout(TimeSpan timeout)
        {
            // To keep this thread busy, we'll have to push a frame.
            DispatcherFrame frame = new DispatcherFrame();

            // Set a timer to terminate our loop in the frame after the
            // timeout has expired.
            DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.Normal);
            timer.Interval = timeout;
            timer.Tick += delegate(object sender, EventArgs e)
            {
                ((DispatcherTimer)sender).IsEnabled = false;
                frame.Continue = false;
            };
            timer.Start();

            // Keep the thread busy processing events until the timeout has expired.
            Dispatcher.PushFrame(frame);
        }

        /// <summary>
        /// Helper function to block till all queue items are processed, also introduce time
        /// lag between subsequent test runs
        /// </summary>
        /// <param name="postMethod">Method to post</param>
        public static void WaitTillQueueItemsProcessed()
        {
            // To keep this thread busy, we'll have to push a frame.
            DispatcherFrame frame = new DispatcherFrame();

            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new DispatcherOperationCallback(
                delegate(object arg)
                {
                    frame.Continue = false;
                    return null;
                }), null);

            // Keep the thread busy processing events until the timeout has expired.
            Dispatcher.PushFrame(frame);
        }

        /// <summary>
        /// Change the priority from Inactive to ApplicationIdle
        /// </summary>
        private static void SchedulePriorityChange(DispatcherOperation op, TimeSpan timeSpan)
        {
            DispatcherTimer dTimer = new DispatcherTimer(DispatcherPriority.Background);
            dTimer.Tick += new System.EventHandler(ChangePriority);
            dTimer.Tag = op;
            dTimer.Interval = timeSpan;
            dTimer.Start();
        }

        /// <summary>
        /// Change priority of the DispatcherOperation
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        private static void ChangePriority(object obj, EventArgs args)
        {
            DispatcherTimer dTimer = obj as DispatcherTimer;
            dTimer.Stop();
            DispatcherOperation op = dTimer.Tag as DispatcherOperation;
            op.Priority = DispatcherPriority.ApplicationIdle;
        }

        /// <summary>
        /// Used to post a method to the context of your ApplicationWindow after a certain amount of time has passed
        /// </summary>
        /// <param name="postMethod">Method to post</param>
        /// <param name="TimerInterval">int time in milliseconds</param>
        public static void PostNextTestStep(DispatcherOperationCallback postMethod, int timerInterval)
        {
            System.Object args = new System.Object();

            System.TimeSpan timespan = new System.TimeSpan(0, 0, 0, 0, timerInterval);

            DispatcherOperation op = Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Inactive, postMethod, args);

            SchedulePriorityChange(op, timespan);
        }

        /// <summary>
        /// Post an item to UIContext
        /// </summary>
        /// <param name="postMethod">Method to post</param>
        public static void PostNextTestStep(DispatcherOperationCallback postMethod)
        {
            System.Object args = new System.Object();

            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, postMethod, args);
        }
    }

/*
    public class VScanHelpers
    {
        /// <summary>
        /// Positions and resizes the given window at Left 0, Top 0, Width 800, Height 480.
        /// </summary>
        /// <param name="window">The window you want to resize and position.</param>
        public static void PositionResizeWindow(System.Windows.Window window)
        {
            PositionResizeWindow(window, 0, 0, 800, 480);
        }

        /// <summary>
        /// Positions and resizes the given window with the specified params.
        /// </summary>
        /// <param name="window">The window you want to resize and position.</param>
        /// <param name="top">The Top position.</param>
        /// <param name="left">The Left position.</param>
        /// <param name="width">The window Width.</param>
        /// <param name="height">The window Height.</param>
        public static void PositionResizeWindow(System.Windows.Window window, double top, double left, double width, double height)
        {
            window.Top = top;
            window.Left = left;
            window.Width = width;
            window.Height = height;
        }

        /// <summary>
        /// Takes a screenshot of the render image of a UIElement.
        /// Get the master bitmap from SD bitplace location.
        /// Compare the render and master bitmap, and save the VScan Package.
        /// The starting capture point is at top left corner of UIElement.
        /// </summary>
        /// <param name="control">The UIElement which you want to take a screenshot of.</param>
        /// <param name="masterid">Master Bitmap Name</param>
        public static void ImagesCompare(UIElement control, string masterid)
        {
            System.Drawing.Size boundingBoxSize = new System.Drawing.Size(0, 0);
            System.Drawing.Point offsetPoint = new System.Drawing.Point(0, 0);
            ImagesCompare(control, boundingBoxSize, offsetPoint, "", masterid);
        }

        /// <summary>
        /// ImagesCompare overload function takes five args.
        /// </summary>
        /// <param name="control">The UIElement which you want to take a screenshot of.</param>
        /// <param name="boundingBoxSize">Bounding Box Size</param>
        /// <param name="offsetPoint">Bounding Box Size</param>
        /// <param name="toleranceFileName">Tolerance xml file name</param>
        /// <param name="masterid">Master Bitmap Name</param>
        public static void ImagesCompare(UIElement control, System.Drawing.Size boundingBoxSize, System.Drawing.Point offsetPoint, string toleranceFileName, string masterid)
        {
            Package _vscanPackage = null;
            Bitmap _captureBmp;
            BitmapSW masterBMP;

            AutomationFramework _frmwrk = new AutomationFramework();
            string classicPath = PathSW.Combine(@"Client\WcpTests\ComponentModel\VScan\MasterImages\Classic\", masterid + ".bmp");
            string testPath = _frmwrk.GetCustomFolderLocation("*");
            string theming = _frmwrk["LOG_THEME"];
            string masterPath = "";
            //theming = Microsoft.Test.Internal.UxTheme.GetCurrentThemeName();
            switch (theming)
            {
                case "1":
                    masterPath = PathSW.Combine(testPath, classicPath);
                    break;
                default:
                    _frmwrk.LogTest(false, "Unknown Theme.");
                    break;
            }

            // Get rendering Bitmap.
            System.Drawing.Rectangle controlRect = ImageUtility.GetScreenBoundingRectangle(control);
            System.Drawing.Point capturePoint = new System.Drawing.Point((int)controlRect.X + (int)offsetPoint.X, (int)controlRect.Y + (int)offsetPoint.Y);
            System.Drawing.Rectangle boundingBox;

            if (boundingBoxSize.Width != 0 && boundingBoxSize.Height != 0)
            {
                boundingBox = new System.Drawing.Rectangle(capturePoint, new System.Drawing.Size((int)boundingBoxSize.Width, (int)boundingBoxSize.Height));
            }
            else
            {
                boundingBox = new System.Drawing.Rectangle(capturePoint, new System.Drawing.Size((int)control.RenderSize.Width, (int)control.RenderSize.Height));
            }
            _captureBmp = ImageUtility.CaptureScreen(boundingBox);


            // Create a work path for VScan Package.
            string workPath = PathSW.Combine(DirectorySW.GetCurrentDirectory(), masterid + ".vscan");

            // Use ImageComparator
            ImageComparator imageCompare = new ImageComparator();
            IImageAdapter imageSource = null;
            IImageAdapter imageTarget = null;

            ChannelCompareMode Channel = ChannelCompareMode.ARGB;
            imageCompare.ChannelsInUse = Channel;
            
            try
            {
                // Get master Bitmap
                masterBMP = new BitmapSW(masterPath);
            }
            catch (ArgumentException)
            {
                // Save capture bitmap when no master bitmap.
                _vscanPackage = Package.Create(workPath, _captureBmp, _captureBmp);
                _vscanPackage.MasterSDLocation = masterPath;
                imageSource = new ImageAdapter(_vscanPackage.MasterBitmap);
                imageTarget = new ImageAdapter(_vscanPackage.CapturedBitmap);
                _vscanPackage.Save();
                if (imageCompare.Compare(imageSource, imageTarget, false))
                {
                    FailureResolutionPage.CreatePointerToFile(_frmwrk, _vscanPackage.PackageName, "Analyze");
                }
                _frmwrk.LogTest(false, "No Master Bitmap.");
                return;
            }

            _vscanPackage = Package.Create(workPath, masterBMP.InnerObject, _captureBmp);
            _vscanPackage.MasterSDLocation = masterPath;

            imageSource = new ImageAdapter(_vscanPackage.MasterBitmap);
            imageTarget = new ImageAdapter(_vscanPackage.CapturedBitmap);
            _vscanPackage.Save();
            
            // test TOLERANCE
            if (toleranceFileName != "")
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(toleranceFileName);
                imageCompare.Curve.CurveTolerance.LoadTolerance(xmlDoc);
                _vscanPackage.Tolerance = xmlDoc.DocumentElement;
                xmlDoc.RemoveAll();
            }
            else
            {
                // use default Tolerance if user didnt specify toleranceFileName.
                imageCompare.Curve.CurveTolerance.Entries.Clear();
                imageCompare.Curve.CurveTolerance.Entries.Add(0, 1.0);
                imageCompare.Curve.CurveTolerance.Entries.Add(2, 0.0005);
                imageCompare.Curve.CurveTolerance.Entries.Add(10, 0.0004);
                imageCompare.Curve.CurveTolerance.Entries.Add(15, 0.0003);
                imageCompare.Curve.CurveTolerance.Entries.Add(25, 0.0002);
                imageCompare.Curve.CurveTolerance.Entries.Add(35, 0.0001);
                imageCompare.Curve.CurveTolerance.Entries.Add(45, 0.00001);
            }


            if (!imageCompare.Compare(imageSource, imageTarget, false))
            {
                FailureResolutionPage.CreatePointerToFile(_frmwrk, _vscanPackage.PackageName, "Analyze");
                _frmwrk.LogTest(false, "Render and Master Bitmaps look different.");
            }
            else
            {
                _frmwrk.LogTest(true, "Render and Master Bitmaps look the same!");
            }
        }
    }

    /// <summary>
    /// Used for testing parts of Controls so you don't have to dig into the Visual Tree of a Control
    /// to get at it's parts.
    /// </summary>
    public static class ControlPartHelper
    {
        /// <summary>
        /// Used to get a part of a control by using the Name of a part.
        /// So to find the "ToolBarThumb" part of a ToolBar.
        /// FindPartByID(myToolBar,"ToolBarThumb");
        /// </summary>
        /// <param name="control">The Control who's part you want to get.</param>
        /// <param name="partId">The Name of part you want to find.</param>
        /// <returns>The part of the control as a FrameworkElement</returns>
        public static FrameworkElement FindPartById(Control curControl, string partId)
        {
		if (curControl != null)
            	{
			ControlTemplate curCT = curControl.Template;
            FrameworkElement frmElement = curCT.FindName(partId, curControl) as FrameworkElement;

            return frmElement;
                }
            	else
            	{
                	return null;
            	}
        }
    }

    /// <summary>
    /// Used for testing Controls that require you to dig into the Visual Tree of a Control
    /// to get at it's parts.
    /// </summary>
    public static class VisualTreeUtils
    {
        /// <summary>
        /// Used to get an item of a specific type in the visual tree.
        /// So to find the 2nd item of type Button it would be something like.
        /// FindPartByType(visual,typeof(Button),1);
        /// </summary>
        /// <param name="vis">The visual who's tree you want to search.</param>
        /// <param name="visType">The type of object you want to find.</param>
        /// <param name="index">The count of the item as it is found in the tree.</param>
        /// <returns>The object of type visType found in vis that is the index item</returns>
        public static object FindPartByType(System.Windows.Media.Visual vis, System.Type visType, int index)
        {
            if (vis != null)
            {
                System.Collections.ArrayList parts = FindPartByType(vis, visType);

                if (index >= 0 && index < parts.Count)
                {
                    return parts[index];
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Returns an ArrayList of all of the items of the specified type in the visual tree.
        /// </summary>
        /// <param name="vis">The visual who's tree you want to search.</param>
        /// <param name="visType">The type of object you want to find.</param>
        /// <returns>An ArrayList containing all of the objects of type visType found in the tree of vis.</returns>
        public static System.Collections.ArrayList FindPartByType(System.Windows.Media.Visual vis, System.Type visType)
        {
            System.Collections.ArrayList parts = new System.Collections.ArrayList();

            if (vis != null)
            {
                parts = FindPartByTypeRecurs(vis, visType, parts);
            }

            return parts;
        }

        private static System.Collections.ArrayList FindPartByTypeRecurs(DependencyObject vis, System.Type visType, System.Collections.ArrayList parts)
        {
            if (vis != null)
            {
                if (vis.GetType() == visType)
                {
                    parts.Add(vis);
                }

                int count = System.Windows.Media.VisualTreeHelper.GetChildrenCount(vis);

                for(int i = 0; i < count; i++)
                {
                    DependencyObject curVis = VisualTreeHelper.GetChild(vis,i);
                    parts = FindPartByTypeRecurs(curVis, visType, parts);
                }
            }

            return parts;
        }

        /// <summary>
        /// tranverse the visual tree to get a visua by Name.
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="ele"></param>
        /// <returns></returns>
        public static DependencyObject FindPartByName(DependencyObject ele, string name)
        {
            DependencyObject result;
            if (ele == null)
            {
                return null;
            }
            if (name.Equals(ele.GetValue(FrameworkElement.NameProperty)))
            {
                return ele;
            }
            int count = VisualTreeHelper.GetChildrenCount(ele);
            for(int i = 0; i < count; i++)
            {
                DependencyObject vis = VisualTreeHelper.GetChild(ele,i);
                if ((result = FindPartByName(vis, name)) != null)
                {
                    return result;
                }
            }
            return null;
        }
    }
        /// <summary>
        /// Sets the input locale identifier (formerly called the keyboard
        /// layout handle) for the calling thread.
        /// </summary>
        /// <example>
        /// <Action Name="ChangeIMESystemLocal">
        ///	<Parameter Value="00000409"/>	
        /// </Action>
        ///<code>
        /// Set input locale to: Arabic (Saudi Arabia) - Arabic (101) <Parameter Value = "00000401">
        /// Set input locale to: English (United States) - US <Parameter Valu = "00000409">
        /// Set input locale to: Hebrew - Hebrew <Parameter Value = "0000040d">
        /// Set input locale to: Japanese - Japanese Input System (MS-IME2002) <Parameter Value = "e0010411">
        /// Set input locale to: Spanish (Argentina) - Latin American <Parameter Value = "00002c0a">
        /// </code></example> 
        /// </summary>       
        public static class SetKeyboardLayout
        {

            [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto, SetLastError = true)]
            private static extern IntPtr ActivateKeyboardLayout(IntPtr hkl, int uFlags);
            /// <summary>
            /// Sets the input locale identifier (formerly called the keyboard
            /// layout handle) for the calling thread or the current process.
            /// </summary>
            /// <param name="hkl">Input locale identifier to be activated.</param>
            /// <param name="flags">Specifies how the input locale identifier is to be activated.</param>
            /// <returns>
            /// If the function succeeds, the return value is the previous input
            /// locale identifier. Otherwise, it is IntPtr.Zero.
            /// </returns>
            public static IntPtr SafeActivateKeyboardLayout(IntPtr hkl, int flags)
            {
                new System.Security.Permissions.SecurityPermission(
                     System.Security.Permissions.PermissionState.Unrestricted)
                     .Assert();
                return ActivateKeyboardLayout(hkl, flags);
            }

            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            private static extern IntPtr LoadKeyboardLayout(string pwszKLID, int flags);
            /// <summary>
            /// Loads a new input locale identifier (formerly called the
            /// keyboard layout) into the system.
            /// </summary>
            /// <param name="pwszKLID">A string composed of the hexadecimal value of the Language Identifier (low word) and a device identifier (high word).</param>
            /// <param name="flags">Specifies how the input locale identifier is to be loaded.</param>
            /// <returns>
            /// The input locale identifier to the locale matched with the
            /// requested name. If no matching locale is available, the return
            /// value is IntPtr.Zero.
            /// </returns>
            public static IntPtr SafeLoadKeyboardLayout(string pwszKLID, int flags)
            {

                new System.Security.Permissions.SecurityPermission(

                    System.Security.Permissions.PermissionState.Unrestricted)

                    .Assert();

                return LoadKeyboardLayout(pwszKLID, flags);

            }
        }    
*/
}


