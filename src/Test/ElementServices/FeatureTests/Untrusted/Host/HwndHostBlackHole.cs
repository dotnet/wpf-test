// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Threading;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.Trusted.Controls;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.Modeling;
using Microsoft.Test.Threading;
using Microsoft.Test.Win32;

namespace Avalon.Test.CoreUI.Hosting
{

    /// <summary>
    /// HwndHostBlackHole Model class. The model definition is checked-in at
    /// SDXROOT\testsrc\windowstestdata\REDIST\client\wcptests\Core\bvt\host\HwndHostBlackHole.mbt    
    /// </summary>
    [TestDefaults]
    public class HwndHostBlackHoleTest : MDEIHostedTestBase
    {
        // tolerance to compare doubles
        protected const double tolerance = 1; 

        /// <summary>
        /// The test case already start and we need to add some stuff during initialization so
        /// with this we had some time to start our test case.
        /// </summary>
        protected override void HostedTestEntryPointCore()
        {
            _hwndHost = (HwndHost)LogicalTreeHelper.FindLogicalNode((DependencyObject)XamlRootObject, "Hwnd1");

            DispatcherHelper.EnqueueBackgroundCallback(Dispatcher.CurrentDispatcher,
                new DispatcherOperationCallback(Validation), null);
        }

        /// <summary>
        /// This is our validation.  We validate Size, Position, and Mouse
        /// </summary>
        object Validation(object o)
        {
            ValidateSize(SetupParams.Width, SetupParams.Height, SetupParams.Visibility, 100, 100);
            ValidationPosition();
            ValidationMouse();

            ValidateResize(SetupParams.Width, SetupParams.Height, SetupParams.Visibility, 100, 100);

            DispatcherHelper.EnqueueBackgroundCallback(Dispatcher.CurrentDispatcher,
                new DispatcherOperationCallback(EndTest), null);       

            return null;
        }

        void ValidateSize(double expectedWidth, double expectedHeight, string expectedVisibility, double oldWidth, double oldHeight)
        {
            ValidateHwndHostWidthHeight(expectedWidth, expectedHeight, expectedVisibility);
            ValidateHwndHostActualWidthActualHeight(expectedWidth, expectedHeight, oldWidth, oldHeight, expectedVisibility);
            ValidateHwndVisibility(expectedVisibility);      
            ValidateHwndWidthHeight(expectedWidth, expectedHeight, oldWidth, oldHeight, expectedVisibility);
        }

        // Validate the HwndHost Width and Height properties.
        void ValidateHwndHostWidthHeight(double expectedWidth, double expectedHeight, string expectedVisibility)
        {
            double realHwndHostWidth = _hwndHost.Width;
            double realHwndHostHeight = _hwndHost.Height;
            

            Log("The FrameworkElement.Width is: " + realHwndHostWidth.ToString());
            Log("The FrameworkElement.Height is: " + realHwndHostHeight.ToString());

            // This validation is when Width is not set on the XAML

            if (double.IsNaN(expectedWidth))
            {
                if (!double.IsNaN(_hwndHost.Width))
                {
                    LogTest(false,"The Width doesn't match. It should be NaN.");
                }
            }

            // This validation is when Height is not set on the XAML

            if (double.IsNaN(expectedHeight))
            {
                if (!double.IsNaN(_hwndHost.Height))
                {
                    LogTest(false,"The Height doesn't match. It should be NaN.");
                }
            }            
        }

        // Validate that the HwndHost is really the width and height it thinks it is.
        void ValidateHwndHostActualWidthActualHeight(double expectedWidth, double expectedHeight, double defaultWidth, double defaultHeight,  string expectedVisibility)
        {

            Surface surface = TestContainer.CurrentSurface[0];
            Point p2_Old = surface.MeasureUnitsFromDeviceUnits(new Point(defaultWidth,defaultHeight));
            defaultWidth = p2_Old.X;
            defaultHeight = p2_Old.Y;

            double hwndHostActualWidth = _hwndHost.ActualWidth;
            double hwndHostActualHeight = _hwndHost.ActualHeight;            

            Log("The FrameworkElement.ActualWidth is: " + hwndHostActualWidth.ToString());
            Log("The FrameworkElement.ActualHeight is: " + hwndHostActualHeight.ToString());
            
            // If it is collapsed
            if (expectedVisibility == "Collapsed")
            {
                if (_hwndHost.ActualWidth != 0)
                {
                    LogTest(false, "The HwndHost is collapsed but its ActualWidth is not 0. Real: " + _hwndHost.ActualWidth.ToString());    
                }

                if (_hwndHost.ActualHeight != 0)
                {
                    LogTest(false, "The HwndHost is collapsed but its ActualHeight is not 0. Real: " + _hwndHost.ActualHeight.ToString());    
                }
            }
            else
            {
                //Hidden, Visible and NoSet behave the same.
                
                if (double.IsNaN(expectedWidth) )
                {
                    // If parent is DockPanel the HwndHost will expand to fit when Width is not set.
                    if ((SetupParams.Parent == "Canvas") && (!CompareEqual(_hwndHost.ActualWidth, defaultWidth)))
                    {
                        LogTest(false,"Width was not set but the HwndHost's ActualWidth is not the default: " + defaultWidth.ToString() +
                            ". Actual: " + _hwndHost.ActualWidth.ToString());   
                    }
                    
                }
                else
                {
                    if (!CompareEqual(_hwndHost.ActualWidth, expectedWidth))
                    {
                        LogTest(false, "The width was set to " + expectedWidth.ToString() + " but the HwndHost's ActualWidth is " + 
                            _hwndHost.ActualWidth.ToString());   
                    }
                }


                if (double.IsNaN(expectedHeight))
                {
                    // If parent is DockPanel the HwndHost will expand to fit when Height is not set.
                    if ((SetupParams.Parent == "Canvas") && (!CompareEqual(_hwndHost.ActualHeight, defaultHeight)))
                    {
                        LogTest(false,"Height was not set but HwndHost's ActualHeight is not the default: " + defaultHeight.ToString() +
                            ". Actual: " + _hwndHost.ActualHeight.ToString());   
                    }
                    
                }
                else
                {
                    if (!CompareEqual(_hwndHost.ActualHeight, expectedHeight))
                    {
                        LogTest(false, "The height was set to " + expectedHeight.ToString() + " but the HwndHost's ActualHeight is: " + 
                            _hwndHost.ActualHeight.ToString());   
                    }
                }                
            }           
        }


        

        void ValidateHwndWidthHeight(double expectedWidth, double expectedHeight, double oldWidthPixels, double oldHeightPixels,  string visibility)
        {
            NativeStructs.RECT rcHwndPos = new NativeStructs.RECT();
            NativeMethods.GetWindowRect(new HandleRef(null, _hwndHost.Handle), ref rcHwndPos);

            double hwndWidth = rcHwndPos.right - rcHwndPos.left;
            double hwndHeight = rcHwndPos.bottom - rcHwndPos.top;  
            
            Log("The Hwnd Width (Pixels) is: " + hwndWidth.ToString());
            Log("The Hwnd Height (Pixels) is: " + hwndHeight.ToString());


            // Translating MeasureUnits to Pixels to compare expected values
            Surface surface = TestContainer.CurrentSurface[0];

            if (!double.IsNaN(expectedWidth))
            {
                Point p2Old = surface.DeviceUnitsFromMeasureUnits(new Point(expectedWidth,0));
                expectedWidth = p2Old.X;
            }           
            if (!double.IsNaN(expectedHeight))
            {
                Point p2Old = surface.DeviceUnitsFromMeasureUnits(new Point(expectedHeight,0));
                expectedHeight = p2Old.X;
            }           


            if (visibility == "Visible" || visibility == "NoSet")
            {
                if (double.IsNaN(expectedWidth))
                {
                    if ((SetupParams.Parent == "Canvas") && (!CompareEqual(hwndWidth, oldWidthPixels)))
                    {
                        LogTest(false, "Width was not set but the Hwnd is not the expected default value: " + oldWidthPixels.ToString() +
                            ". Actual: " + hwndWidth.ToString());                       
                    }
                }
                else
                {
                    if (! CompareEqual(hwndWidth, expectedWidth))
                    {
                        LogTest(false, "The width was set to " + expectedWidth.ToString() + " but the Hwnd's actual width is " +
                            hwndWidth.ToString());
                    }
                }

                if (double.IsNaN(expectedHeight))
                {
                    if ((SetupParams.Parent == "Canvas") && (! CompareEqual(hwndHeight, oldHeightPixels)))
                    {
                        LogTest(false, "Height was not set but the Hwnd is not the expected default value: " + oldHeightPixels.ToString() + 
                            ". Actual: " + hwndHeight.ToString());                       
                    }
                }
                else
                {
                    if (! CompareEqual(hwndHeight, expectedHeight))
                    {
                        LogTest(false,"The height was set to " + expectedHeight.ToString() + " but the Hwnd's actual height is " + 
                            hwndHeight.ToString());
                    }
                }
            }   

            
        }

        bool CompareEqual(double x, double y)
        {
            if (Math.Abs(x - y) > tolerance)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        void ValidateHwndVisibility(string expectedVisibility)
        {
            if (expectedVisibility == "NoSet" || expectedVisibility == "Visible")
            {
                ValidateHwndIsVisible(true);                
            }
            else
            {
                ValidateHwndIsVisible(false);
            }
        }   
        
        void ValidateHwndIsVisible(bool expectedValue)
        {
            if (NativeMethods.IsWindowVisible(new HandleRef(null, _hwndHost.Handle)) != expectedValue)
            {
                LogTest(false,"Win32 IsWindowVisible return unexpectedValue. Expected value " + expectedValue.ToString());
            }
        }

        // Resize the HwndHost and verify the size again.
        void ValidateResize(double expectedWidth, double expectedHeight, string expectedVisibility, double oldWidth, double oldHeight)
        {
            double newExpectedWidth = 30; 
            double newExpectedHeight = 40;
            if (!double.IsNaN(expectedWidth))
            {
                newExpectedWidth = expectedWidth + 30;
            }
            if (!double.IsNaN(expectedHeight)) 
            {
                newExpectedHeight = expectedHeight + 40;
            }

            Log("Resizing from " + expectedWidth + ", " + expectedHeight + " to " + newExpectedWidth + ", " + newExpectedHeight + "...");

            _hwndHost.Width = newExpectedWidth;
            _hwndHost.Height = newExpectedHeight;

            // Wait for redraw.
            DispatcherHelper.DoEvents(500);
            DispatcherHelper.DoEvents(500);

            Log("Validating sizes again...");
            ValidateSize(newExpectedWidth, newExpectedHeight, expectedVisibility, oldWidth, oldHeight);
        }

        void ValidationPosition()
        {
            // Testing Position of the HWND and HwndHost
            if (ShouldHwndHostVisible())
            {
                NativeStructs.RECT rcHwndPos = new NativeStructs.RECT();
                NativeMethods.GetWindowRect(new HandleRef(null, _hwndHost.Handle), ref rcHwndPos);
            
                Surface surface = TestContainer.CurrentSurface[0];

                Point p = PointUtil.ClientToScreen(new Point(0,0), surface.GetPresentationSource());


                Log("Where is the Client Point X: " + p.X);
                Log("Where is the Client Point Y: " + p.Y);

                Point screenPoint = GetHwndHostScreenPoint();

                if (!CompareEqual(screenPoint.X, rcHwndPos.left))
                {
                    LogTest(false, "The HWND X (" + screenPoint.X + ") position relative to the screen  doesn't match with the HwndHost Position " + rcHwndPos.left + ").");
                }

                if (!CompareEqual(screenPoint.Y, rcHwndPos.top))
                {
                    LogTest(false,"The HWND Y (" + screenPoint.Y + ") position relative to the screen  doesn't match with the HwndHost Position " + rcHwndPos.top + ").");               
                }
            }
        }

        Point GetHwndHostScreenPoint()
        {

            NativeStructs.RECT rcHwndPos = new NativeStructs.RECT();
            NativeMethods.GetWindowRect(new HandleRef(null, _hwndHost.Handle), ref rcHwndPos); 
            
            Point screenPoint = GetScreenPointFromVisual(_hwndHost, new Point(0,0));

            Log("HWND Window Rect Point X" + rcHwndPos.left);
            Log("HWND Window Rect Point Y" + rcHwndPos.top);
            
            return screenPoint;
        }

        Point GetScreenPointFromVisual(Visual visual, Point offset)
        {
            Surface surface = TestContainer.CurrentSurface[0];
                       
            Visual rootVisual = (Visual)RealRootVisual;

            Matrix m;
            System.Windows.Media.GeneralTransform gt = visual.TransformToAncestor(rootVisual);
            System.Windows.Media.Transform t = gt as System.Windows.Media.Transform;
            if(t==null)
            {
	            throw new System.ApplicationException("//TODO: Handle GeneralTransform Case - introduced by Transforms Breaking Change");
            }
            m = t.Value;
            Point rootPoint = new Point(0 + offset.X ,0 + offset.Y) * m;

            Point clientPoint = PointUtil.RootToClient(rootPoint, surface.GetPresentationSource());
            Point screenPoint = PointUtil.ClientToScreen(clientPoint, surface.GetPresentationSource());

            Log("Screen Point X: " + screenPoint.X);   
            Log("Screen Point HwndHost Y: " + screenPoint.Y);

            Point pointFromAvalonAPI = visual.PointToScreen(offset);

            Log("Avalon (PointToScreen) Screen Point X: " + pointFromAvalonAPI.X);            
            Log("Avalon (PointToScreen) Screen Point HwndHost Y: " + pointFromAvalonAPI.Y);            

            if (pointFromAvalonAPI != screenPoint)
            {
               LogTest(false,"The Visual.PointToScreen from Avalon: " + pointFromAvalonAPI.ToString() + "; and the Point are not the same: " + screenPoint.ToString() );               
            }

            Point point2 = visual.PointFromScreen(pointFromAvalonAPI);

            Log("***Avalon (PointFromScreen) Offset Point X: " + point2.X);            
            Log("***Avalon (PointFromScreen) Offset Point HwndHost Y: " + point2.Y);

            Vector fromScreenToOffset = point2 - offset;     
            if (fromScreenToOffset.Length >= 1.0)
            {
               LogTest(false,"The Visual.PointToScreen from Avalon: " +point2.ToString() + "; and the offset Point are not the same: " + offset.ToString() );               
            }

            return screenPoint; 
        }


        void ValidationMouse()
        {

            DependencyObject dObject = LogicalTreeHelper.GetParent(_hwndHost);
                        
            while (dObject != null)                 
            {
                if (dObject is UIElement || dObject is ContentElement)
                {
                    break;
                }                
                dObject = LogicalTreeHelper.GetParent(dObject) ;
            }

            if (dObject == null)
            {
                return;
            }

            if (dObject is UIElement)
            {
                ((UIElement)dObject).AddHandler(Mouse.MouseMoveEvent, new MouseEventHandler(MoveHandler), true);
            }
            else
            {
                ((ContentElement)dObject).AddHandler(Mouse.MouseMoveEvent, new MouseEventHandler(MoveHandler), true);
            }


            IMouseEvents ctlEvents = (IMouseEvents)_hwndHost;

            ctlEvents.MouseMove += new EventHandler(MoveMouseHwndHandler);

            
            MoveMouseToHwndHost(true);

            _startInput = true;

            MoveMouseToHwndHost(false);

            _startInput = false;
            ValidateMouseEvents();
            
        }


        /// <summary>
        /// Get start and finish point for mouse move over Hwnd, in pixels.
        /// </summary>
        void MoveMouseToHwndHost(bool initial)
        {
            MouseLocation location = MouseLocation.CenterLeft;

            if (!initial)
            {
                location =  MouseLocation.CenterRight;
            }
            
            Log("Start moving mouse over the HwndHost control");            
            MouseHelper.MoveOutside(_hwndHost.Handle, location);
            Log("End moving mouse over the HwndHost control");            
        }


        /// <summary>
        /// Get start and finish point for mouse move over Hwnd, in pixels.
        /// </summary>
        Point GetPointForMouseMove(bool initial)
        {
            Point hwndHostPoint = GetHwndHostScreenPoint();

            NativeStructs.RECT rcHwndPos = new NativeStructs.RECT();
            NativeMethods.GetWindowRect(new HandleRef(null, _hwndHost.Handle), ref rcHwndPos);

            // Testing Size
            double realHwndHeightMiddle = (rcHwndPos.bottom - rcHwndPos.top) / 2;    
            double realHwndWidth = rcHwndPos.right - rcHwndPos.left;   
            
            Point point;
            
            if (initial)
            {
                // Initial X position is left of the HwndHost, Y is in the middle of the Hwnd.
                point = new Point(hwndHostPoint.X - 25, hwndHostPoint.Y + realHwndHeightMiddle);
            }
            else
            {
                // Final X position is right of the HwndHost, Y is in the middle of the Hwnd.
                point = new Point(hwndHostPoint.X + realHwndWidth + 25, hwndHostPoint.Y + realHwndHeightMiddle);
            }
            
            return point;                         
        }

        bool ShouldHwndHostVisible()
        {
            if (SetupParams.Visibility == "Visible" || SetupParams.Visibility == "NoSet")
            {
                return true;
            }
            return false;
        }

        bool ShouldHwndHostEnabled()
        {
            if (SetupParams.Enabled == "Enabled" || SetupParams.Enabled == "NoSet")
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Build an ArrayList of mouse events expected for the HwndHost position.
        /// </summary>
        /// <returns>Expected list of mouse events</returns>
        private ArrayList PredictMouseEvents(NativeStructs.RECT surfaceRect, NativeStructs.RECT hwndRect, Point mouseInitial)
        {
            Log("Predicting Mouse Events");
            ArrayList events = new ArrayList();
            
            if (SurfaceHittable(surfaceRect, mouseInitial))
            {
                if (HwndHostHittable(surfaceRect, hwndRect, mouseInitial))
                {
                    if (!HwndHostOnLeftEdge(surfaceRect, hwndRect))
                    {
                        // First event is on Surface.
                        Log("HwndHost is not on the left edge, first event is from surface.");
                        events.Add(EventType.SurfaceEvent);
                    }
                    else
                    {
                        Log("HwndHost is on the left edge");
                    }

                    // Next event is on HwndHost.           
                    Log("HwndHost is hittable, next event is from HwndHost");
                    events.Add(EventType.HwndEvent);

                    if (!HwndHostOnRightEdge(surfaceRect, hwndRect))
                    {
                        // Add event on Surface.
                        Log("HwndHost is not on the right edge, next event is from surface.");
                        events.Add(EventType.SurfaceEvent);
                    }
                    else
                    {
                        Log("HwndHost is on the right edge");
                    }
                }
                else // hwndHost not hittable.
                {
                    // Only event is on Surface.
                    Log("HwndHost is not hittable, only event will be from surface");
                    events.Add(EventType.SurfaceEvent);
                }
            }

            // note empty events ArrayList is valid if surface is, for some strange reason, not hittable.

            return events;
        }

        /// <summary>
        /// Determine if the mouse will travel over the Surface given it's 
        /// initial position. It might not generate an event if the Surface is covered by an HwndHost.
        /// </summary>
        /// <returns>true if the mouse will travel of the Surface</returns>
        /// <remarks>
        /// Utility for PredictMouseEvents. Assumes mouse only moves horizontally and horizontally coincide
        /// with the Surface (doesn't compare X).
        /// </remarks>
        private bool SurfaceHittable(NativeStructs.RECT surfaceRect, Point mouseInitial)
        {
            return ((mouseInitial.Y < surfaceRect.bottom) && (mouseInitial.Y > surfaceRect.top)) ? true : false;
        }

        /// <summary>
        /// Determine if the mouse is visible, enabled, and on the Surface in a convenient location for mousing over. 
        /// </summary>
        /// <param name="surfaceRect"></param>
        /// <param name="hwndRect"></param>
        /// <param name="mouseInitial"></param>
        /// <returns>Returns false if the HwndHost is outside of the Surface, Disabled or not Visible.</returns>
        /// <remarks>
        /// Utility for PredictMouseEvents. Assumes mouse is started 25 pixels to the left 
        /// of hwndLeft.Rect (see GetPointForMouseMove)
        /// </remarks>
        private bool HwndHostHittable(NativeStructs.RECT surfaceRect, NativeStructs.RECT hwndRect, Point mouseInitial)
        {
            if (!ShouldHwndHostVisible() || !ShouldHwndHostEnabled())
            {
                return false;
            }
           
            // Mouse below hwnd.
            if (mouseInitial.Y > hwndRect.bottom)
            {
                return false;
            }

            // Mouse above hwnd.
            if (mouseInitial.Y < hwndRect.top)
            {
                return false;
            }

            // Hwnd left of visible surface.
            if (hwndRect.right < surfaceRect.left)
            {
                return false;
            }

            // Hwnd right of visible surface.
            if (hwndRect.left > surfaceRect.right)
            {
                return false;
            }

            // Hwnd above visible surface.
            if (hwndRect.bottom < surfaceRect.top)
            {
                return false;
            }

            // Hwnd below visible surface.
            if (hwndRect.top > surfaceRect.bottom)
            {
                return false;
            }

            return true;
        }

        // Determine if hwndHost is on left edge of surface. This would stop any mouse
        // events from appearing for the surface.
        private bool HwndHostOnLeftEdge(NativeStructs.RECT surfaceRect, NativeStructs.RECT hwndRect)
        {
            Log("hwndRect.left " + hwndRect.left + " surfaceRect.left " + surfaceRect.left);

            // hwndHost stradles surface left edge.
            // + 9 to allow for border. - Classic theme has width of 4, Aero has 8.
            if ((hwndRect.left <= surfaceRect.left + 9) && (hwndRect.right > surfaceRect.left))
            {
                return true;
            }

            return false;
        }

        private bool HwndHostOnRightEdge(NativeStructs.RECT surfaceRect, NativeStructs.RECT hwndRect)
        {
            Log("hwndRect.right " + hwndRect.right + " surfaceRect.right " + surfaceRect.right);

            // hwndHost stradles surface left edge.
            // + 9 to allow for border.
            if ((hwndRect.left <= surfaceRect.right) && (hwndRect.right + 9 > surfaceRect.right))
            {
                return true;
            }

            return false;
        }

        void ValidateMouseEvents()
        {
            //
            // Get dimensions and points for validation.
            //

            // Surface rectangle.
            Surface surface = TestContainer.CurrentSurface[0];
            NativeStructs.RECT rcSurfacePos = new NativeStructs.RECT();
            NativeMethods.GetWindowRect(new HandleRef(null, surface.Handle), ref rcSurfacePos);

            // HwndHost rectangle.
            NativeStructs.RECT rchwndPos = new NativeStructs.RECT();
            NativeMethods.GetWindowRect(new HandleRef(null, _hwndHost.Handle), ref rchwndPos);

            // Initial and final mouse positions.
            Point mouseInitial = GetPointForMouseMove(true);
            Point mouseEnd = GetPointForMouseMove(false);

            //
            // Predict mouse events and compare against recorded events.
            //
            ArrayList predictedEvents = PredictMouseEvents(rcSurfacePos, rchwndPos, mouseInitial);

            Log("Predicted Mouse Events: ");
            if (predictedEvents.Count == 0)
            {
                Log("None");
            }
            else
            {
                for (int i = 0; i < predictedEvents.Count; i++)
                {
                    Log("  " + i + " " + predictedEvents[i].ToString());
                }
            }

            Log("Recorded Mouse Events: ");
            if (_mouseList.Count == 0)
            {
                Log("None");
            }
            else
            {
                for (int i = 0; i < _mouseList.Count; i++)
                {
                    Log("  " + i + " " + _mouseList[i].ToString());
                }
            }


            if (predictedEvents.Count != _mouseList.Count)
            {
                LogTest(false, "Incorrect number of mouse events recorded.");
            }
            else
            {
                bool sameMouseEvents = true;
                for (int i = 0; i < predictedEvents.Count; i++)
                {
                    // Compare each EventType
                    if ((EventType)predictedEvents[i] != (EventType)_mouseList[i])
                    {
                        sameMouseEvents = false;
                    }
                }

                if (!sameMouseEvents)
                {
                    LogTest(false, "Mouse events recorded were the wrong type.");
                }
            }
        }
     
        bool _startInput = false;
        ArrayList _mouseList = ArrayList.Synchronized(new ArrayList());

        /// <summary>
        /// Handler for mouse move over hwndHost. Add an object to the event list if the mouse
        /// has moved from one object to another (surface to hwndhost) or if the list is empty.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="args"></param>
        void MoveMouseHwndHandler(object o, EventArgs args)
        {
            if (_startInput)
            {
                if (_mouseList.Count == 0 || (EventType)_mouseList[_mouseList.Count - 1] != EventType.HwndEvent)
                {
                    Log("Mouse Move over HwndHost " + o.ToString());
                    _mouseList.Add(EventType.HwndEvent);
                }

            }
        }

        /// <summary>
        /// Handler for mouse move over Surface. Add an object to the event list if the mouse
        /// has moved from one object to another (hwndhost to surface) or if the list is empty.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="args"></param>
        void MoveHandler(object o, MouseEventArgs args)
        {
            if (_startInput)
            {

                if (_mouseList.Count == 0 || (EventType)_mouseList[_mouseList.Count - 1] != EventType.SurfaceEvent)
                {                
                    _mouseList.Add(EventType.SurfaceEvent);
                    Log("Mouse Move over Parent " + o.ToString());
                }
            }
        }

        /// <summary>
        /// Mouse move event type, either over an HwndHost or the Surface.
        /// </summary>
        enum EventType { HwndEvent, SurfaceEvent };


        object EndTest(object o)
        {   
            TestContainer.EndTest();
            LogTest(IsTestPassed,"Logging Result");
            return null;
        }

        HwndHostBlackHoleTestSetup SetupParams
        {
            get
            {
                return (HwndHostBlackHoleTestSetup)ModelState;
            }            
            set
            {
                ModelState = value;
            }
        }
        
        HwndHost _hwndHost = null;        

    }


    /// <summary>
    /// HwndHostBlackHole Model class
    /// </summary>
    [Model(@"FeatureTests\ElementServices\HwndHostBlackHole_PairWise.xtc", 1, 81, 1, @"Hosting\Layout", TestCaseSecurityLevel.FullTrust, "HwndHostBlackHole", ExpandModelCases = true, Area = "AppModel", Disabled=true)]
    [Model(@"FeatureTests\ElementServices\HwndHostBlackHole_ThirdWise.xtc", 1, 230, 2, @"Hosting\Layout", TestCaseSecurityLevel.FullTrust, "HwndHostBlackHole", ExpandModelCases = true, Area = "AppModel", Disabled = true)]
    public class HwndHostBlackHole : CoreModel 
    {
        /// <summary>
        /// Creates a HwndHostBlackHole Model instance
        /// </summary>
        public HwndHostBlackHole(): base()
        {
            Name = "HwndHostBlackHole";
            Description = "HwndHostBlackHole Model";
            ModelPath = "MODEL_PATH_TOKEN";
   
            //Add Action Handlers
            AddAction("SetupTestCase", new ActionHandler(SetupTestCase));
            
        }
       
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Handler for SetupTestCase</remarks>
        /// <param name="endState">Expected end State object</param>
        /// <param name="inParams">Input action parameters object</param>
        /// <param name="outParams">Output action parameters object</param>
        /// <returns>false if errors</returns>
        private bool SetupTestCase(State endState, State inParams, State outParams)
        {
            //Action Params (listed here for convienence during coding)
            //inParams["SourceType"] - 
            //inParams["HostControlType"] - 
            //inParams["SetPosition"] - 
            //inParams["Parent"] - 
            //inParams["SetSize"] - 
            //inParams["SetVisibility"] - 
            //inParams["SetEnabled"] - 
            
            HwndHostBlackHoleAutomation testCase = new HwndHostBlackHoleAutomation(base.AsyncActions);
            testCase.Run(inParams);

            return true;
        }
    }

    /// <summary>
    /// </summary>
    public class HwndHostBlackHoleAutomation : ModelAutomationBase
    {
        /// <summary>
        /// </summary>
        public HwndHostBlackHoleAutomation(AsyncActionsManager asyncManager) : base(asyncManager)
        {
        }

        /// <summary>
        /// </summary>
        /// <remarks>
        ///    //inParams["SourceType"] - 
        ///    //inParams["HostControlType"] - 
        ///    //inParams["SetPosition"] - 
        ///    //inParams["Parent"] - 
        ///    //inParams["SetSize"] - 
        /// </remarks>
        public void Run(IDictionary dictionary)
        {
            //
            // Parse case parameters with HwndHostBlackHoleTestSetup.
            //
 
            // Setting up Test Case information
            HwndHostBlackHoleTestSetup testSetup = new HwndHostBlackHoleTestSetup(dictionary);
                       
            CoreModelState.Persist(testSetup);
                 
            Log(testSetup.ToString());

            //
            // Build Xaml file.
            //

            BuildXamlFrom(testSetup);

            string[] xamlFileName = {_xamlFile};
            string[] contents = {CoreModelState.GetFullFilePath()};

            //
            // Execute case.
            //

            if (testSetup.CompiledVersion)
            {
                GenericCompileHostedCase.RunCase("Avalon.Test.CoreUI.Hosting","CoreTestsUntrusted.dll", "HwndHostBlackHoleTest","HostedTestEntryPoint",testSetup.Source, null,contents,xamlFileName);
            }
            else
            {
                ExeStubContainerFramework exe = new ExeStubContainerFramework(testSetup.Source);
                exe.Run(new HwndHostBlackHoleTest(),"HostedTestEntryPoint");
            }

        }


        string _xamlFile = "__test1.xaml";
        
        void BuildXamlFrom(HwndHostBlackHoleTestSetup setupParams)
        {
            XmlTextWriter xmlWriter =  null;

            try
            {
                // Setup writer.
                xmlWriter = new XmlTextWriter(_xamlFile, System.Text.Encoding.Unicode);
                xmlWriter.Formatting = System.Xml.Formatting.Indented;

                // Creating header.
                xmlWriter.WriteStartElement("Page");
                xmlWriter.WriteAttributeString("xmlns","x",null,"http://schemas.microsoft.com/winfx/2006/xaml");
                xmlWriter.WriteAttributeString("xmlns",null,null,"http://schemas.microsoft.com/winfx/2006/xaml/presentation");                    

                if (setupParams.CompiledVersion)
                    xmlWriter.WriteAttributeString("x","Class",null,"MyClassRoot");    

                // Creating Parent
                switch (setupParams.Parent)
                {
                    case "Canvas":
                        xmlWriter.WriteStartElement("Canvas");
                        xmlWriter.WriteAttributeString("Background","Green");                          
                        break;
                    case "DockPanel":
                        xmlWriter.WriteStartElement("DockPanel");
                        xmlWriter.WriteAttributeString("Background", "Blue");
                        xmlWriter.WriteAttributeString("LastChildFill", "False"); // Don't expand HwndHost to fill.
                        break;
                    default:
                        throw new Exception("Parent type " + setupParams.Parent + " not supported.");
                }

                // Creating Host Control
                if (setupParams.HwndHostType == "Win32Button")
                {
                    //xmlWriter.WriteStartElement("host","Win32ButtonCtrl","Hosting");
                    //xmlWriter.WriteAttributeString("xmlns","host",null,"Hosting");  
		    xmlWriter.WriteStartElement("host", "Win32ButtonCtrl", 
				"clr-namespace:Avalon.Test.CoreUI.Trusted.Controls;assembly=CoreTestsTrusted");		        		

                    WriteCommonXamlonHwndHost(xmlWriter, setupParams);                    
                }
                else
                {
                    throw new Exception("Host type " + setupParams.HwndHostType + " not supported.");
                }
                
                // Closing Host Control
                xmlWriter.WriteEndElement();

                // Closing Parent
                xmlWriter.WriteEndElement();

                // Closing Page
                xmlWriter.WriteEndElement();

            }
            finally
            {
                if (xmlWriter != null)
                {
                    xmlWriter.Close();
                }
            }
           
        }

        void WriteCommonXamlonHwndHost(XmlTextWriter xmlWriter,HwndHostBlackHoleTestSetup setup)
        {

            xmlWriter.WriteAttributeString("Name","Hwnd1");

            // Set attached properties respective to parent.
            switch (setup.Parent)
            {
                case "Canvas":
                    Point point = HwndHostBlackHoleTestSetup.GetPointFromName(setup.Position);

                    xmlWriter.WriteAttributeString("Canvas.Top", point.Y.ToString());
                    xmlWriter.WriteAttributeString("Canvas.Left", point.X.ToString());
                    break;

                case "DockPanel":
                    xmlWriter.WriteAttributeString("DockPanel.Dock", setup.Position);
                    break;
            }

            if (!Double.IsNaN(setup.Width))
            {
                xmlWriter.WriteAttributeString("Width",setup.Width.ToString());     
            }

            if (!Double.IsNaN(setup.Height))
            {
                xmlWriter.WriteAttributeString("Height",setup.Height.ToString()); 
            }

            if (setup.Visibility != "NoSet")
            {
                xmlWriter.WriteAttributeString("Visibility",setup.Visibility );                                         
            }

            if (setup.Enabled == "Enabled")
            {
                xmlWriter.WriteAttributeString("IsEnabled","true");                                         
            }
            else if (setup.Enabled == "Disabled")
            {
                xmlWriter.WriteAttributeString("IsEnabled","false");   
            }

            if (setup.SetOpacity == "True")
            {
                xmlWriter.WriteAttributeString("Opacity", "0.25");
            }

            //
            // Compound properties.
            //

             if (setup.SetClipGeometry == "True")
             {
                xmlWriter.WriteStartElement("host:Win32ButtonCtrl.Clip");
                    xmlWriter.WriteStartElement("EllipseGeometry");
                        xmlWriter.WriteAttributeString("RadiusX", "20");
                        xmlWriter.WriteAttributeString("RadiusY", "20");
                        xmlWriter.WriteAttributeString("Center", "100,75");
                    xmlWriter.WriteEndElement();
                xmlWriter.WriteEndElement();
             }

            if (setup.AddRenderTransform != "None")
            {

                xmlWriter.WriteStartElement("host:Win32ButtonCtrl.RenderTransform");
                if (setup.AddRenderTransform == "Rotate")
                {
                    xmlWriter.WriteStartElement("RotateTransform");
                    xmlWriter.WriteAttributeString("Center", "0,0");
                    xmlWriter.WriteAttributeString("Angle", "45");
                }
                else // (setup.AddRenderTransform == "Scale")
                {
                    xmlWriter.WriteStartElement("ScaleTransform");
                    xmlWriter.WriteAttributeString("Center", "0,0");
                    xmlWriter.WriteAttributeString("ScaleX", "2");
                    xmlWriter.WriteAttributeString("ScaleY", "2");
                }
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndElement();
            }

            // Disabled pending bug #
            //if (setup.AddLayoutTransform != "None")
            //{
                //xmlWriter.WriteStartElement("host:Win32ButtonCtrl.LayoutTransform");

                ////if (setup.AddRenderTransform == "Rotate")
                ////{
                //xmlWriter.WriteStartElement("RotateTransform");
                //xmlWriter.WriteAttributeString("Center", "0,0");
                //xmlWriter.WriteAttributeString("Angle", "25");
                ////}
                ////else // setup.AddRenderTransform == "Scale"
                ////{
                ////xmlWriter.WriteStartElement("ScaleTransform");
                ////xmlWriter.WriteAttributeString("Center", "0,0");
                ////xmlWriter.WriteAttributeString("ScaleX", "2");
                ////xmlWriter.WriteAttributeString("ScaleY", "2");
                ////}
                //xmlWriter.WriteEndElement();
                //xmlWriter.WriteEndElement();
            //}

        }

        
    }
        

    /// <summary>
    /// </summary>
    [Serializable()]
    public class HwndHostBlackHoleTestSetup : HwndHostHostingState
    {
        // For converting Position property to point coordinates in Canvas parent.
        private static Hashtable s_pointRefTable = new Hashtable();

        static HwndHostBlackHoleTestSetup()
        {
           s_pointRefTable["TopRight"] = new Point(400,0);
           s_pointRefTable["Middle"] = new Point(200,200);
           s_pointRefTable["TopLeft"] = new Point(0,0);
           s_pointRefTable["DownLeft"] = new Point(400,400);
           s_pointRefTable["DownRight"] = new Point(0,400);           
        }
             
        /// <summary>
        /// Parse test case parameters.
        /// </summary>
        /// <remarks>
        /// Source, HwndHostType are parsed by base class HwndHostHostingState.
        /// </remarks>
        public HwndHostBlackHoleTestSetup(IDictionary dictionary) : base(dictionary)
        {
            Dictionary.Add("Parent", (string)dictionary["Parent"]);

            Dictionary.Add("Position", (string)dictionary["SetPosition"]);

            string size = (string)dictionary["SetSize"];
            string[] sizeArray = size.Split("_".ToCharArray());

            if (sizeArray.Length != 3)      
            {
                throw new ArgumentException("The Size is not on the correct format","dictionary");
            }

            if (String.Compare("NaN", sizeArray[1], false) == 0)
            {
                Dictionary.Add("Width", Double.NaN);
            }
            else
            {
                Dictionary.Add("Width", Double.Parse(sizeArray[1]));
            }
    
            if (String.Compare("NaN", sizeArray[2], false) == 0)
            {
                Dictionary.Add("Height", Double.NaN);
            }
            else
            {
                Dictionary.Add("Height", Double.Parse(sizeArray[2]));
            }

            Dictionary.Add("Visibility", (string)dictionary["SetVisibility"]);

            Dictionary.Add("Enabled", (string)dictionary["SetEnabled"]);

            Dictionary.Add("AddRenderTransform", (string)dictionary["AddRenderTransform"]);
            Dictionary.Add("SetClipGeometry", (string)dictionary["SetClipGeometry"]);
            Dictionary.Add("SetOpacity", (string)dictionary["SetOpacity"]);
        }

        
        /// <summary>
        /// </summary>
        public HwndHostBlackHoleTestSetup()
        {
        }

        /// <summary>
        /// </summary>
        public static Point GetPointFromName(string position)
        {
            return (Point)s_pointRefTable[position];               
        }

        /// <summary>
        /// Position is restricted by model depending on Parent. 
        /// </summary>
        public string Position
        {
            get
            {
                return (string)Dictionary["Position"];
            }
        }

        
        /// <summary>
        /// </summary>
        public double Width
        {
            get
            {
                return (double)Dictionary["Width"];
            }
        }


        /// <summary>
        /// </summary>
        public double Height
        {
            get
            {
                return (double)Dictionary["Height"];
            }

        }


        /// <summary>
        /// </summary>
        public string Parent
        {
            get
            {
                return (string)Dictionary["Parent"];
            }
        }


        /// <summary>
        /// </summary>
        public string Visibility
        {
            get
            {
                return (string)Dictionary["Visibility"];
            }
        }

        /// <summary>
        /// </summary>
        public string Enabled
        {
            get
            {
                return (string)Dictionary["Enabled"];
            }
        }

        /// <summary>
        /// </summary>
        public string SetClipGeometry
        {
            get
            {
                return (string)Dictionary["SetClipGeometry"];
            }
        }

        /// <summary>
        /// </summary>
        public string SetOpacity
        {
            get
            {
                return (string)Dictionary["SetOpacity"];
            }
        }

        /// <summary>
        /// </summary>
        public string AddRenderTransform
        {
            get
            {
                return (string)Dictionary["AddRenderTransform"];
            }
        }
        
    }


}

//This file was generated using MDE on: Thursday, January 20, 2005 3:01:46 PM
