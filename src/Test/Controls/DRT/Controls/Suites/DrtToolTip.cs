// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.ComponentModel;
using System.Threading;

using System.Text;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Automation;


namespace DRT
{

    public class ToolTipSuite: DrtTestSuite
    {
        static ToolTipSuite()
        {
            EventManager.RegisterClassHandler(typeof(ToolTip), ToolTip.OpenedEvent, new RoutedEventHandler(OnToolTipOpened), true);
            EventManager.RegisterClassHandler(typeof(ToolTip), ToolTip.ClosedEvent, new RoutedEventHandler(OnToolTipClosed), true);
        }

        public ToolTipSuite() : base("ToolTip")
        {
            Contact = "Microsoft";
            s_currentSuite = this;
        }

        public override DrtTest[] PrepareTests()
        {
            Border b = new Border();
            b.Width = 200;
            b.Height = 100;

            b.Background = Brushes.White;

            StackPanel sp = new StackPanel();
            sp.Orientation = System.Windows.Controls.Orientation.Horizontal;

            _b1 = new TestButton();
            _b1.Content = "ToolTip DRT Button";
            ToolTip tooltip = new ToolTip();
            tooltip.Opened += new RoutedEventHandler(OnToolTip1Opened);
            tooltip.Closed += new RoutedEventHandler(OnToolTip1Closed);
            tooltip.Content = "This is a ToolTip";
            _b1.ToolTip = tooltip;

           sp.Children.Add(_b1);

            _b2 = new Button();
            _b2.ToolTipOpening += new ToolTipEventHandler(OnB2ToolTipOpening);
            _b2.Content = "ToolTip change";
            _b2.ToolTip = "ToolTip";

            sp.Children.Add(_b2);

            b.Child = sp;
            DRT.Show(b);

            if (!DRT.KeepAlive)
            {
                return new DrtTest[]
                    {
                        new DrtTest(MouseOverB1),
                        new DrtTest(VerifyB1ToolTip),
                        new DrtTest(MouseDown),
                        new DrtTest(VerifyB1ToolTipGone),
                        new DrtTest(MouseOff),
                        new DrtTest(ResetEventTriggers),

                        new DrtTest(MouseOverB1),
                        new DrtTest(VerifyB1ToolTip),
                        new DrtTest(MouseOff),
                        new DrtTest(VerifyB1ToolTipGone),
                        new DrtTest(ResetEventTriggers),

                        new DrtTest(MouseOverB1),
                        new DrtTest(VerifyB1ToolTip),
                        new DrtTest(ChangeB1ToolTip),
                        new DrtTest(MouseOff),
                        new DrtTest(VerifyB1ToolTipGone),
                        new DrtTest(ResetEventTriggers),

                        new DrtTest(SetB2ToolTip),
                        new DrtTest(MouseOverB2),
                        new DrtTest(VerifyB2ToolTip),
                        new DrtTest(ChangeB2ToolTip),
                        new DrtTest(VerifyChangeB2ToolTip),
                        new DrtTest(MouseOff),
                        new DrtTest(VerifyB2ToolTipGone),

                        new DrtTest(SetPlacement),

                        new DrtTest(ResetEventTriggers),
                };
            }
            else
            {
                return new DrtTest[] {};
            }
        }

        private bool NeedRepeat(bool condition)
        {
            if (!condition)
            {
                if (_repeatedTests++ < 10)
                {
                    DRT.Pause(500);
                    DRT.RepeatTest();
                    return true;
                }
            }

            _repeatedTests = 0;
            return false;
        }

        private void ResetEventTriggers()
        {
            _lastToolTipOpened = null;
            _toolTip1Opened = false;
            _toolTip1Closed = false;
            _b2ToolTipOpened = false;
        }

        private void MouseOverB1()
        {
            if (DRT.Verbose) Console.WriteLine("Mousing over B1");
            DRT.MoveMouse(_b1, 0.5, 0.5);
            DRT.Pause(200);
        }

        private void VerifyB1ToolTip()
        {
            if (!NeedRepeat(_b1.IsToolTipOpen))
            {
                DRT.Assert(_b1.IsToolTipOpen, "ToolTip opening event did not fire on b1");
                DRT.Assert(_toolTip1Opened, "ToolTip opening event did not b1's ToolTip");
            }
        }

        private void MouseDown()
        {
            DRT.ClickMouse();
            DRT.Pause(200);
        }

        private void VerifyB1ToolTipGone()
        {
            DRT.Assert(!_b1.IsToolTipOpen, "ToolTip closing event did not fire on b1 after mouse down.");
            DRT.Assert(_toolTip1Closed, "ToolTip closing event did not fire on b1's ToolTip after mouse down.");
            ToolTip tooltip = (ToolTip)_b1.ToolTip;
            DRT.Assert(!tooltip.IsOpen, "Tooltip was visible after mouse down");
        }

        private void MouseOff()
        {
            Input.MoveTo(new Point(0, 0));
            DRT.Pause(200);
        }

        private void ChangeB1ToolTip()
        {
            ((ToolTip)_b1.ToolTip).Content = "B1 Different Content";
        }

        private void MouseOverB2()
        {
            if (DRT.Verbose) Console.WriteLine("Mousing over B2");
            DRT.MoveMouse(_b2, 0.5, 0.5);
            DRT.Pause(200);
        }

        private void SetB2ToolTip()
        {
            _b2.ToolTip = "B2 ToolTip - Initial content";
        }

        private void ChangeB2ToolTip()
        {
            DRT.Assert(((string)_lastToolTipOpened.Content) == "B2 ToolTip - Initial content", "B2 ToolTip initial content not correct");
            _b2.ToolTip = "B2 ToolTip - Different content";
        }

        private void VerifyChangeB2ToolTip()
        {
            DRT.Assert(((string)_lastToolTipOpened.Content) == "B2 ToolTip - Different content", "B2 ToolTip content did not change");
        }

        private void VerifyB2ToolTip()
        {
            if (!NeedRepeat(_lastToolTipOpened != null && _lastToolTipOpened.IsOpen && _b2ToolTipOpened))
        {
                DRT.Assert(_b2ToolTipOpened, "ToolTip opening event did not B2");
                DRT.Assert(_lastToolTipOpened != null, "ToolTip event did not fire on B2's ToolTip");
                DRT.Assert(_lastToolTipOpened.IsOpen, "B2's ToolTip is not open");
            }
        }

        private void VerifyB2ToolTipGone()
        {
            if (!NeedRepeat(_lastToolTipOpened != null && !_lastToolTipOpened.IsOpen))
            {
                DRT.Assert(_lastToolTipOpened != null, "B2 ToolTip was null");
                DRT.Assert(!_lastToolTipOpened.IsOpen, "Tooltip did not go away");
            }
        }

        private void SetPlacement()
        {
            // 
            try
            {
                ToolTip t = new ToolTip();

                t.Placement = PlacementMode.Absolute;
            }
            catch (StackOverflowException e)
            {
                DRT.BlockInput = false;
                throw new Exception("Stack overflow exception", e);
            }
        }

        private void OnToolTip1Opened(object sender, RoutedEventArgs e)
        {
            _toolTip1Opened = true;
            DRT.Assert(sender != null, "ToolTip Opened Event: sender is null");
            DRT.Assert(sender is ToolTip, "ToolTip Opened Event: sender is not a ToolTip");
        }

        private void OnToolTip1Closed(object sender, RoutedEventArgs e)
        {
            _toolTip1Closed = true;
            DRT.Assert(sender != null, "ToolTip Closed Event: sender is null");
            DRT.Assert(sender is ToolTip, "ToolTip Closed Event: sender is not a ToolTip");
        }

        private void OnB2ToolTipOpening(object sender, ToolTipEventArgs e)
        {
            _b2ToolTipOpened = true;
        }

        private static void OnToolTipOpened(object sender, RoutedEventArgs e)
        {
            s_currentSuite._lastToolTipOpened = (ToolTip)sender;
        }

        private static void OnToolTipClosed(object sender, RoutedEventArgs e)
        {
            s_currentSuite.DRT.Assert(sender == s_currentSuite._lastToolTipOpened, "ToolTip Closed Event: Closed ToolTip should be the same as last opened ToolTip");
        }

        private class TestButton : Button
        {
            protected override void OnToolTipOpening(ToolTipEventArgs e)
            {
                base.OnToolTipOpening(e);
                _isToolTipOpened = true;
            }

            protected override void OnToolTipClosing(ToolTipEventArgs e)
        {
                base.OnToolTipClosing(e);
                _isToolTipOpened = false;
        }

            public bool IsToolTipOpen
        {
                get
            {
                    return _isToolTipOpened;
                }
            }

            private bool _isToolTipOpened = false;
                }

        private static ToolTipSuite s_currentSuite;
        private int _repeatedTests = 0;
        private ToolTip _lastToolTipOpened = null;
        private TestButton _b1;
        private Button _b2;
        private bool _toolTip1Opened = false;
        private bool _toolTip1Closed = false;
        private bool _b2ToolTipOpened = false;
    }
}
