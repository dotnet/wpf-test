// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Automation for the state based MDE model ResizeWindow.
 *          Construct window, resize them and verify.
 *
 
  
 * Revision:         $Revision:  $
 
 *********************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
//using Avalon.Test.CoreUI.IdTest;
using Avalon.Test.CoreUI.Serialization;
using Avalon.Test.CoreUI.Trusted;
//using Avalon.Test.CoreUI.Trusted.Controls;

using Microsoft.Test.Discovery;
using Microsoft.Test.Modeling;

using Microsoft.Test.Serialization;
using Microsoft.Test.Threading;
using Microsoft.Test.Win32;
using Microsoft.Test.Markup;

using Microsoft.Test;

namespace Avalon.Test.CoreUI.Integration
{
    /// <summary>
    /// ResizeModel Model class.
    /// </summary>
    [Model(@"FeatureTests\ElementServices\WindowResize.xtc", 1, @"Interop\WindowResize", TestCaseSecurityLevel.FullTrust, "WindowResizeModel", ExpandModelCases=true, Area="AppModel", Disabled = true)]
    public class ResizeWindowModel: CoreModel
    {
        /// <summary>
        /// Construct new instance of the model.
        /// </summary>
        public ResizeWindowModel()
            : base()
        {
            Name = "ResizeModel";
            Description = "Model ResizeModel";

            // Add State change handlers
            OnBeginCase += new StateEventHandler(BeginCaseHandler);
            OnEndCase += new StateEventHandler(EndCaseHandler);
            OnGetCurrentState += new StateEventHandler(GetCurrentStateHandler);
            
            //Add Action Handlers
            AddAction("OpenWindow", new ActionHandler(OpenWindowHandler));
            AddAction("CloseWindow", new ActionHandler(CloseWindowHandler));
            AddAction("ResizeWindow", new ActionHandler(ResizeWindowHandler));

            // Values for verification pass.
            _surfaceRects = new List<NativeStructs.RECT>();
            _captureRecorder = new VScanRecorder();
        }

      

        #region Model Actions

        /// <summary>
        /// This method is called before each test case.
        /// </summary>
        /// <param name="sender">todo: Sender of the event?</param>
        /// <param name="e">State before test case is started.</param>
        protected void BeginCaseHandler(object sender, StateEventArgs e)
        {
            State beginState = e.State;

            CoreLogger.LogStatus("BeginCase: " + beginState);
        }

        
        /// <summary>
        /// Handler for OpenWindow Action
        /// </summary>
        /// <param name="endState">Expected end state.</param>
        /// <param name="inParams">Input action parameters.</param>
        /// <param name="outParams">Output action parameters.</param>
        /// <returns>Success or failure.</returns>
        protected bool OpenWindowHandler(State endState, State inParams, State outParams)
        {
            CoreLogger.LogStatus( " OpenWindowHandler" );

            if (inParams != null) CoreLogger.LogStatus("  inParams: " + inParams.ToString());
            if (outParams != null) CoreLogger.LogStatus("  outParams: " + outParams.ToString());
            if (endState != null) CoreLogger.LogStatus("  endState: " + endState.ToString());

            bool succeed = true;

            // Save surface and content types for later verification.
            _surfaceTypeParam = inParams["WindowType"];
            _contentParam = inParams["Content"];
            
            CreateSurface(_surfaceTypeParam);
            CreateContent(_contentParam);

            DispatcherHelper.DoEvents(500);

            // Assign callback for taking mid-resize screenshots.
            _resizeHelper = new WindowResizeHelper(SaveScreenshot);

            // Save initial screenshot.
            SaveScreenshot();

            if (null == _surface) return false;

            return succeed;
        }

        /// <summary>
        /// Handler for ResizeWindow
        /// </summary>
        /// <param name="endState">Expected end state.</param>
        /// <param name="inParams">Input action parameters. </param>
        /// <param name="outParams">Output action parameters.</param>
        /// <returns>Success status.</returns>
        protected bool ResizeWindowHandler(State endState, State inParams, State outParams)
        {
            CoreLogger.LogStatus( "* Action ResizeWindow" );

            if (inParams != null) CoreLogger.LogStatus(" inParams: " + inParams.ToString());
            if (endState != null) CoreLogger.LogStatus(" endState: " + endState.ToString());

            bool succeed = true;

            // Parse input params.
            ResizeMethod method = (ResizeMethod)Enum.Parse(typeof(ResizeMethod), inParams["Method"], true);
            ResizeHandle handle = (ResizeHandle)Enum.Parse(typeof(ResizeHandle), inParams["Handle"], true);
            //ResizeAmount amount = (ResizeAmount)Enum.Parse(typeof(ResizeAmount), inParams["Amount"], true);
            ResizeDirection direction = (ResizeDirection)Enum.Parse(typeof(ResizeDirection), inParams["Direction"], true);

            // Helper will call SaveScreenshot
            _resizeHelper.Resize(_surface, method, handle, direction);

            CoreLogger.LogStatus("    Resized " + _surface.Width + ", " + _surface.Height, ConsoleColor.Yellow);

            return succeed;
        }

        /// <summary>
        /// Handler for MaximizeWindow
        /// </summary>
        /// <param name="endState">Expected end state.</param>
        /// <param name="inParams">Input action parameters.</param>
        /// <param name="outParams">Output action parameters.</param>
        /// <returns>Succeess status.</returns>
        private bool MaximizeWindow(State endState, State inParams, State outParams)
        {
            CoreLogger.LogStatus( "Action MaximizeWindow" );

            if (inParams != null) CoreLogger.LogStatus(" inParams: " + inParams.ToString());
            if (outParams != null) CoreLogger.LogStatus(" outParams: " + outParams.ToString());
            if (endState != null) CoreLogger.LogStatus(" endState: " + endState.ToString());

            bool succeed = true;

            NativeMethods.ShowWindow(new HandleRef(null, _surface.Handle), NativeConstants.SW_MAXIMIZE);

            // todo: Add ScreenShot verification here? Need to know the surface is maximized
            // to create a verification surface, right now we only save width and height. The
            // window could be placed halfway off the screen if not maximized, this might 
            // interfere with screenshot.


            return succeed;
        }

        /// <summary>
        /// Handler for MinimizeWindow
        /// </summary>
        /// <param name="endState">Expected end state.</param>
        /// <param name="inParams">Input action parameters.</param>
        /// <param name="outParams">Output action parameters.</param>
        /// <returns>Succeess status.</returns>
        private bool MinimizeWindow(State endState, State inParams, State outParams)
        {
            CoreLogger.LogStatus( "Action MinimizeWindow" );

            if (inParams != null) CoreLogger.LogStatus(" inParams: " + inParams.ToString());
            if (outParams != null) CoreLogger.LogStatus(" outParams: " + outParams.ToString());
            if (endState != null) CoreLogger.LogStatus(" endState: " + endState.ToString());

            bool succeed = true;

            NativeMethods.ShowWindow(new HandleRef(null, _surface.Handle), NativeConstants.SW_MINIMIZE);

            return succeed;
        }

        /// <summary>
        /// Handler for RestoreWindow
        /// </summary>
        /// <param name="endState">Expected end state.</param>
        /// <param name="inParams">Input action parameters.</param>
        /// <param name="outParams">Output action parameters.</param>
        /// <returns>Succeess status.</returns>
        private bool RestoreWindow(State endState, State inParams, State outParams)
        {
            CoreLogger.LogStatus( "Action RestoreWindow" );

            if (inParams != null) CoreLogger.LogStatus(" inParams: " + inParams.ToString());
            if (outParams != null) CoreLogger.LogStatus(" outParams: " + outParams.ToString());
            if (endState != null) CoreLogger.LogStatus(" endState: " + endState.ToString());

            bool succeed = true;

            NativeMethods.ShowWindow(new HandleRef(null, _surface.Handle), NativeConstants.SW_RESTORE);

            SaveScreenshot();

            return succeed;
        }

        /// <summary>
        /// Handler for CloseWindow action.
        /// </summary>
        /// <param name="endState">Expected end state.</param>
        /// <param name="inParams">Input action parameters.</param>
        /// <param name="outParams">Output action parameters.</param>
        /// <returns>Succeess status.</returns>
        private bool CloseWindowHandler(State endState, State inParams, State outParams)
        {
            CoreLogger.LogStatus( "Action CloseWindowHandler" );

            if (inParams != null) CoreLogger.LogStatus(" inParams: " + inParams.ToString());
            if (outParams != null) CoreLogger.LogStatus(" outParams: " + outParams.ToString());
            if (endState != null) CoreLogger.LogStatus(" endState: " + endState.ToString());

            bool succeed = true;

            if (null != _surface)
            {
                _surface.Close();
            }
            else
            {
                succeed = false;
            }

            return succeed;
        }

        /// <summary>
        /// This method will be called after each test case.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">State after test case is ended.</param>
        protected void EndCaseHandler(object sender, StateEventArgs e)
        {
            State endState = e.State;

            CoreLogger.LogStatus("* EndCase: " + endState);

            // Create a new surface for each window size the action handlers passed through
            // and compare a screenshot of this surface to the other.
            foreach (NativeStructs.RECT windowRect in _surfaceRects)
            {
                CoreLogger.LogStatus(" Creating test surface of "
                    + windowRect.Width + "," + windowRect.Height + " at "
                    + windowRect.left + "," + windowRect.top);

                // Create surface.
                Surface testSurface = CreateSurface(_surfaceTypeParam);

                // Add content.
                CreateContent(_contentParam);

                testSurface.SetSize(windowRect.Width, windowRect.Height);

                //testSurface.SetPosition(windowRect.left, windowRect.top);
                NativeMethods.SetWindowPos(testSurface.Handle, IntPtr.Zero, windowRect.left, windowRect.top, 0, 0, NativeConstants.SWP_NOSIZE);

                NativeMethods.SetForegroundWindow(new HandleRef(null, testSurface.Handle));

                // Need some time to draw/resize content.
                DispatcherHelper.DoEvents(500);

                // Call Compare.
                bool succeed = _captureRecorder.Compare(VScanHelper.Capture(testSurface.Handle, true));

                // Close surface.
                testSurface.Close();

                // If different throw TestVerificationException.
                if (!succeed)
                {
                    throw new Microsoft.Test.TestValidationException("Fail! Images do not match");
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get window state.
        /// </summary>
        /// <returns>String.Empty if no handler</returns>
        public string WindowState
        {
            get
            {
                if (_surface == null)
                {
                    return "WindowState_NotOpened";
                }

                if (_surface.IsClosed)
                {
                    return "WindowState_Closed";
                }

                if (_surface.IsVisible)
                {
                    return "WindowState_Opened";
                }

                return String.Empty;
            }
        }

        [DllImport("user32.dll")]
        private static extern bool IsIconic(HandleRef hwnd);

        [DllImport("user32.dll")]
        private static extern bool IsZoomed(HandleRef hwnd);
 
        /// <summary>
        /// Get window size state.
        /// </summary>
        /// <returns>String.Empty if no handler</returns>
        public string SizeState
        {
            get
            {
                if (null == _surface)
                {
                    return "SizeState_Normal";
                }
                if (IsIconic(new HandleRef(null, _surface.Handle)))
                {
                    return "SizeState_Minimized";
                }
                else if (IsZoomed(new HandleRef(null, _surface.Handle)))
                {
                    return "SizeState_Maximized";
                }
                else
                {
                    return "SizeState_Normal";
                }

//
            }
        }

        #endregion


        #region Helpers

        /// <summary>
        /// Save the current surface size and a screenshot.
        /// </summary>
        protected void SaveScreenshot()
        {
            //NativeMethods.ShowWindow(new HandleRef(null, _surface.Handle), NativeConstants.SW_SHOWDEFAULT);
            NativeMethods.SetForegroundWindow(new HandleRef(null, _surface.Handle));
            DispatcherHelper.DoEvents(500);

            NativeStructs.RECT windowRect = NativeStructs.RECT.Empty;
            NativeMethods.GetWindowRect(new HandleRef(null, _surface.Handle), ref windowRect);

            CoreLogger.LogStatus("    Save screenshot, size " + windowRect.Width + "," + windowRect.Height, ConsoleColor.Yellow);

            // Save height for creating verification window.
            //_surfaceRects.Add(new Point(windowRect.Width, windowRect.Height));
            _surfaceRects.Add(windowRect);

            // Capture an image of the surface.
            _captureRecorder.Record(VScanHelper.Capture(_surface.Handle, true));
        }

        /// <summary>
        /// This method will be called after every action to retrieve current state of the modeled system.
        /// If the current state does not match the expected state the case will fail.
        /// </summary>
        /// <param name="sender">Model instance.</param>
        /// <param name="e">Assign current state to e.State.</param>
        protected void GetCurrentStateHandler(object sender, StateEventArgs e)
        {
            State currentState = e.State;

            currentState["WindowState"] = this.WindowState;
            currentState["SizeState"] = this.SizeState;
        }

        /// <summary>
        /// Create surface depending on window type. Sets _surface.
        /// </summary>
        /// <returns>new surface</returns>
        protected Surface CreateSurface(string windowTypeParam)
        {
            _surface = new SurfaceFramework(windowTypeParam, 100, 100, 200, 200);

            //IntPtr handle = _surface.Handle;
            //Point p = new Point(1, 1);
            //Point newPoint = _surface.DeviceUnitsFromMeasureUnits(p);

            return _surface;
        }

        /// <summary>
        /// Add content to the test Surface.
        /// </summary>
        protected void CreateContent(string contentTypeParam)
        {
            object displayObject = null;

            Button b = new Button();
            b.Opacity = 1.0;
            b.Content = "I'm a button in the resize model!";

            switch(contentTypeParam)
            {
                case "DockPanel":
                    DockPanel d = new DockPanel();
                    d.Background = new SolidColorBrush(Colors.Cyan);
                    b.Content = "Button in DockPanel";
                    d.Children.Add(b);

                    displayObject = d;
                    break;

                case "Canvas":
                    Canvas c = new Canvas();
                    c.Background = new SolidColorBrush(Colors.Magenta);

                    b.Content = "Button in Canvas";
                    c.Children.Add(b);
                    Canvas.SetTop(b, 10);
                    Canvas.SetLeft(b, 10);

                    displayObject = c;
                    break;

                case "StackPanel":
                    StackPanel s = new StackPanel();
                    s.Background = new SolidColorBrush(Colors.Yellow);

                    b.Content = "Button in StackPanel";
                    s.Children.Add(b);

                    displayObject = s;
                    break;

                case "ScrollbarViewer":
                    ScrollViewer sv = new ScrollViewer();
                    sv.Background = new SolidColorBrush(Colors.Cyan);

                    b.Content = "Button in ScrollbarViewer";
                    sv.Content = b;

                    displayObject = sv;
                    break;

                case "DocumentViewer":
                    DocumentViewer dv = new DocumentViewer();
                    dv.Background = new SolidColorBrush(Colors.Magenta);
                    // todo: What can go in here?

                    displayObject = dv;
                    break;

                case "RichTextBox":
                    RichTextBox rtb = new RichTextBox();
                    rtb.Background = new SolidColorBrush(Colors.Yellow);
                    // todo: Put a flowdocument in this rtb.

                    displayObject = rtb;
                    break;

                case "HwndHost":
// todo: Not in new tree                    displayObject = new Win32ButtonCtrl();

                    break;

                case "Page":
                    Page p = new Page();
                    p.Background = new SolidColorBrush(Colors.Magenta);

                    b.Content = "Button in Page";
                    p.Content = b;
                    
                    displayObject = p; 
                    break;

                case "Frame":
                    Frame f = new Frame();
                    // What should Source be?
                    f.Background = new SolidColorBrush(Colors.Yellow);

                    displayObject = f;
                    break;

                default:
                    throw new NotSupportedException("contentTypeParam");
            }

            _surface.DisplayObject(displayObject);
        }

        #endregion

        #region Privates

        private Surface _surface;
        WindowResizeHelper _resizeHelper;

        // Values for verification.
        private string _surfaceTypeParam;
        private string _contentParam;

        private List<NativeStructs.RECT> _surfaceRects;   
        private VScanRecorder _captureRecorder;

        #endregion

    }
}
