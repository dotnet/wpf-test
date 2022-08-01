// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Automation;
using System.Windows.Threading;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.Reflection;
using System.ComponentModel;
using MS.Internal; // PointUtil

namespace DRT
{
    public class ScrollBarSuite : DrtTestSuite
    {
        public ScrollBarSuite() : base("ScrollBar")
        {
            Contact = "Microsoft";
        }

        ScrollBar _vsb; //Vertical ScrollBar
        ScrollBar _hsb; //Horizontal ScrollBar
        ScrollViewer _sv; // ScrollViewer
        ScrollBar _svvsb; // ScrollViewer's Vertical ScrollBar
        ScrollBar _svhsb; // ScrollViewer's Horizontal ScrollBar

        #region Window setup and common hooks

        public override DrtTest[] PrepareTests()
        {
            Keyboard.Focus(null);

            DRT.LoadXamlFile("DrtScrollBar.xaml");
            DRT.Show(DRT.RootElement);

            _vsb = DRT.FindElementByID("VerticalScrollBar") as ScrollBar;
            _hsb = DRT.FindElementByID("HorizontalScrollBar") as ScrollBar;
            _sv = DRT.FindElementByID("ScrollViewer") as ScrollViewer;
            
            ScrollBar[] bars = VisualTreeUtils.FindElementsIn<ScrollBar>(_sv);

            if (bars[0].Orientation == Orientation.Vertical)
            {
                _svvsb = bars[0];
                _svhsb = bars[1];
            }
            else
            {
                _svvsb = bars[1];
                _svhsb = bars[0];
            }
            


            List<DrtTest> tests = new List<DrtTest>();
            if (!DRT.KeepAlive)
            {
                tests.Add(new DrtTest(MouseTest));
            } 
            return tests.ToArray();
        }

        #endregion

        #region Mouse Test

        private void MouseTest()
        {
            if (DRT.Verbose) Console.WriteLine("\n---ScrollBar Mouse Tests");

            DRT.ResumeAt(new DrtTest(MouseTestProc));
        }

        private enum MouseTestStep
        {
            Start,

            // Drag VerticalScrollBar down
            VerticalDown_Prepare,
            VerticalDown_PressMouse,
            VerticalDown_Drag,
            VerticalDown_ReleaseMouse,

            // Drag HorizontalScrollBar right
            HorizontalRight_Prepare,
            HorizontalRight_PressMouse,
            HorizontalRight_Drag,
            HorizontalRight_ReleaseMouse,

            ScrollViewer_BeforeScrollViewerTests,

            // Drag ScrollViewer's VerticalScrollBar down
            ScrollViewerDown_Prepare,
            ScrollViewerDown_PressMouse,
            ScrollViewerDown_Drag,
            ScrollViewerDown_ReleaseMouse,

            // Drag ScrollViewer's HorizontalScrollBar right
            ScrollViewerRight_Prepare,
            ScrollViewerRight_PressMouse,
            ScrollViewerRight_Drag,
            ScrollViewerRight_ReleaseMouse,

            ScrollViewer_LoopForNonLive,
            
            End,
        }

        MouseTestStep _mouseTestStep = MouseTestStep.Start;

        Thumb _thumb;

        private bool _testedLive;

        private void MouseTestProc()
        {
            if (DRT.Verbose) Console.WriteLine("Mouse test = " + _mouseTestStep);

            switch (_mouseTestStep)
            {
                case MouseTestStep.Start:
                    break;


                //Vertical ScrollBar
                case MouseTestStep.VerticalDown_Prepare:
                    _thumb = ((Track)DRT.FindElementByID("PART_Track", _vsb)).Thumb;
                    DRT.MoveMouse(_thumb, 0.5, 0.5);
                    break;

                case MouseTestStep.VerticalDown_PressMouse:
                    DRT.MouseButtonDown();
                    break;

                case MouseTestStep.VerticalDown_Drag:
                    DRT.MoveMouse(_thumb, 0.5, 100);
                    break;

                case MouseTestStep.VerticalDown_ReleaseMouse:
                    DRT.MouseButtonUp();
                    break;



                //Horizontal ScrollBar
                case MouseTestStep.HorizontalRight_Prepare:
                    _thumb = ((Track)DRT.FindElementByID("PART_Track", _hsb)).Thumb;
                    DRT.MoveMouse(_thumb, 0.5, 0.5);
                    break;

                case MouseTestStep.HorizontalRight_PressMouse:
                    DRT.MouseButtonDown();
                    break;

                case MouseTestStep.HorizontalRight_Drag:
                    DRT.MoveMouse(_thumb, 100, 0.5);
                    break;

                case MouseTestStep.HorizontalRight_ReleaseMouse:
                    DRT.MouseButtonUp();
                    break;


                //ScrollViewer's Vertical ScrollBar
                case MouseTestStep.ScrollViewerDown_Prepare:
                    _thumb = ((Track)DRT.FindElementByID("PART_Track", _svvsb)).Thumb;
                    DRT.MoveMouse(_thumb, 0.5, 0.5);
                    break;

                case MouseTestStep.ScrollViewerDown_PressMouse:
                    DRT.MouseButtonDown();
                    break;

                case MouseTestStep.ScrollViewerDown_Drag:
                    DRT.MoveMouse(_thumb, 0.5, 100);
                    break;

                case MouseTestStep.ScrollViewerDown_ReleaseMouse:
                    DRT.MouseButtonUp();
                    break;

                //ScrollViewer's Horizontal ScrollBar
                case MouseTestStep.ScrollViewerRight_Prepare:
                    _thumb = ((Track)DRT.FindElementByID("PART_Track", _svhsb)).Thumb;
                    DRT.MoveMouse(_thumb, 0.5, 0.5);
                    break;

                case MouseTestStep.ScrollViewerRight_PressMouse:
                    DRT.MouseButtonDown();
                    break;

                case MouseTestStep.ScrollViewerRight_Drag:
                    DRT.MoveMouse(_thumb, 100, 0.5);
                    break;

                case MouseTestStep.ScrollViewerRight_ReleaseMouse:
                    DRT.MouseButtonUp();
                    break;


                case MouseTestStep.End:
                    break;
               
            }
            DRT.Pause(10);
            DRT.ResumeAt(new DrtTest(MouseTestVerifyProc));
        }

        private void MouseTestVerifyProc()
        {
            if (DRT.Verbose) Console.WriteLine("Mouse test verify = " + _mouseTestStep);

            switch (_mouseTestStep)
            {
                case MouseTestStep.Start:
                    break;

                case MouseTestStep.VerticalDown_ReleaseMouse:
                    DRT.Assert(_vsb.Value == _vsb.Maximum, "Vertical ScrollBar should be at the maximum");
                    break;


                case MouseTestStep.HorizontalRight_ReleaseMouse:
                    DRT.Assert(_hsb.Value == _hsb.Maximum, "Horizontal ScrollBar should be at the maximum");
                    break;


                case MouseTestStep.ScrollViewerDown_ReleaseMouse:
                    DRT.Assert(Math.Abs(_sv.VerticalOffset + _sv.ViewportHeight - _sv.ExtentHeight) < 0.1, "ScrollViewer should be at the bottom");
                    break;


                case MouseTestStep.ScrollViewerRight_ReleaseMouse:
                    DRT.Assert(Math.Abs(_sv.HorizontalOffset + _sv.ViewportWidth - _sv.ExtentWidth) < 0.1, "ScrollViewer should be at the right");
                    break;

                // Loop back and repeat for non-live scrolling
                case MouseTestStep.ScrollViewer_LoopForNonLive:
                    if (!_testedLive)
                    {
                        _mouseTestStep = MouseTestStep.ScrollViewer_BeforeScrollViewerTests;
                        _testedLive = true;
                        _sv.ScrollToHorizontalOffset(0.0);
                        _sv.ScrollToVerticalOffset(0.0);
                        _sv.IsDeferredScrollingEnabled = true;
                    }
                    else
                    {
                        // Reset to live scrolling
                        _sv.IsDeferredScrollingEnabled = false;
                    }
                    break;

                case MouseTestStep.End:

                    break;
            }

            if (_mouseTestStep != MouseTestStep.End)
            {
                _mouseTestStep++;
                DRT.ResumeAt(new DrtTest(MouseTestProc));
            }
        }

        #endregion

        

    }

    internal sealed class VisualTreeUtils
    {
        VisualTreeUtils()
        { }

        internal static Visual FindElementByName(Visual root, string name)
        {
            return TraverseVisualTree(root, name);
        }

        private static FrameworkElement TraverseVisualTree(DependencyObject ele, string name)
        {
            if (ele == null)
            {
                return null;
            }

            int count = VisualTreeHelper.GetChildrenCount(ele);
            for(int i = 0; i < count; i++)
            {
                DependencyObject vis = VisualTreeHelper.GetChild(ele, i);
                FrameworkElement fe = vis as FrameworkElement;
                if (fe != null && fe.Name == name)
                {
                    return fe;
                }
                FrameworkElement ret = TraverseVisualTree(vis, name);
                if (ret != null) return ret;
            }
            return null;
        }

        internal static ElementType FindFirstElementIn<ElementType>(DependencyObject root)
        {
            if (root is ElementType)
                return (ElementType)((object)root);

            int count = VisualTreeHelper.GetChildrenCount(root);
            for(int i = 0; i < count; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(root, i);
                if (child is ElementType)
                    return (ElementType)((object)child);

                ElementType target = FindFirstElementIn<ElementType>(child);
                 if ( (object)target != null)
                    return target;
            }
            return default(ElementType);
        }

        private static AnotherType CastTo<AnotherType>(object obj)
        {
            return (AnotherType)obj;
        }

        internal static ElementType[] FindElementsIn<ElementType>(DependencyObject element)
        {
            List<ElementType> elements = new List<ElementType>();
            if (element is ElementType)
            {
                elements.Add(CastTo<ElementType>(element));
            }
            else
            {
                int count = VisualTreeHelper.GetChildrenCount(element);
                for(int i = 0; i < count; i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(element, i);
                    if (child is ElementType)
                    {
                        elements.Add(CastTo<ElementType>(child));
                    }
                    else
                    {
                        ElementType[] target = FindElementsIn<ElementType>(child);
                        elements.AddRange(target);
                    }
                }
            }
            return elements.ToArray();
        }
    }

}
