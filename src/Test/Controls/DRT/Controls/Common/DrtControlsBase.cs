// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
//
// Description: Extensions to DrtBase not generic enough to be in DrtBase.
//

using System;
using System.Reflection;
using System.IO;
using System.Threading;

using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.Runtime.InteropServices;
using System.Text;


namespace DRT
{
    // base class for a DRT application
    public abstract class DrtControlsBase : DrtBase
    {
        protected DrtControlsBase()
        {
            DrtName = "DrtControls";
        }

        protected override void OnDeactivated()
        {
            base.OnDeactivated();

            if (DontStartSuites)
            {
                return;
            }

            if (s_windowDeactivated != WarningLevel.Ignore)
            {
                if (!_shuttingDown)
                {
                    IntPtr foregroundHwnd = GetForegroundWindow();
                    string foregroundWindowTitle = GetWindowTitle(foregroundHwnd);

                    IntPtr focusHwnd = GetFocus();
                    string focusWindowTitle = GetWindowTitle(focusHwnd);

                    string outputString = String.Format("Window deactivated unexpectedly, currently focused window is {0:X} ({1}) foreground window is {2:X} ({3}).", focusHwnd, focusWindowTitle, foregroundHwnd, foregroundWindowTitle);

                    if ((MainWindow != null) &&
                        ((MainWindow.Handle == foregroundHwnd) || (MainWindow.Handle == focusHwnd)))
                    {
                        Console.WriteLine("WARNING: Main Window was deactivated but remained the foreground or focused window. Likely there was another error. An assert or debug window may have opened.");
                    }
                    else
                    {
                        Console.WriteLine("ERROR: " + outputString);

                        if (s_windowDeactivated == WarningLevel.Error)
                        {
                            DRT.Assert(false, "Window deactivated unexpectedly");
                        }
                        else if (s_windowDeactivated == WarningLevel.TentativelySucceed)
                        {
                            Console.WriteLine();
                            Console.WriteLine("Test will now tentatively succeed");
                            DRT.WriteDelayedOutput();
                            System.Environment.Exit(0);
                        }
                    }
                } 
            }
        }

        public WarningLevel WindowDeactivatedWarningLevel
        {
            get { return s_windowDeactivated; }
            set { s_windowDeactivated = value; }
        }
   
        private bool _shuttingDown = false;

        /// <summary>
        /// What to do if input is received that doesn't match input sent.
        /// </summary>
        /// <value></value>
        public WarningLevel MismatchedInputWarningLevel
        {
            set { s_mismatchedInput = value; }
            get { return s_mismatchedInput; }
        }

        protected override void OnStartingUp()
        {
            base.OnStartingUp();
            InputMonitor.BeginMonitoring();
        }

        protected override void OnShuttingDown()
        {
            _shuttingDown = true;
            base.OnShuttingDown ();
            InputMonitor.EndMonitoring();
        }

        public override void MoveMouse(Point pt)
        {
            InputMonitor.NotifyMoveTo(pt);
            base.MoveMouse(pt);
        }

        public override void SendKeyStrokes(params KeyStatePair[] keyStatePairs)
        {
            foreach (KeyStatePair pair in keyStatePairs)
            {
                InputMonitor.NotifySendKeyboardInput(pair.Key, pair.Press);
            }

            base.SendKeyStrokes(keyStatePairs);
        }

        public override void SendMouseButton(MouseButton button, bool press)
        {
            InputMonitor.NotifySendMouseButton(button, press);
            base.SendMouseButton(button, press);
        }

        #region InputMonitor

        /// <summary>
        /// This class monitors input.  It records input that is sent through the DrtBase
        /// methods that send input and watches input that comes to PreProcessInput.  If the
        /// input never gets there, we know that we lost it somewhere along the way.
        /// </summary>
        private class InputMonitor
        {
            private static bool s_isMonitoring;
            public static void BeginMonitoring()
            {
                InputManager.Current.PreProcessInput += new PreProcessInputEventHandler(OnPreProcessInput);
                s_isMonitoring = true;
            }

            public static void EndMonitoring()
            {
                s_isMonitoring = false;
                InputManager.Current.PreProcessInput -= new PreProcessInputEventHandler(OnPreProcessInput);
            }

            public static void NotifyMoveTo(Point pt)
            {
                /*
                if (_isMonitoring)
                {
                    _expectedInput.Add(new MouseMoveInputUnit(pt));
                }
                */
            }

            public static void NotifySendMouseButton(MouseButton b, bool press)
            {
                if (s_isMonitoring && DRT.BlockInput)
                {
                    s_expectedInput.Add(new MouseButtonInputUnit(b, press));
                }
            }

            public static void NotifySendKeyboardInput(Key k, bool press)
            {
                if (s_isMonitoring && DRT.BlockInput)
                {
                    s_expectedInput.Add(new KeyInputUnit(k, press));
                }
            }

            private static void OnPreProcessInput(object sender, PreProcessInputEventArgs args)
            {
                if (!s_isMonitoring || !DRT.BlockInput) return;

                InputEventArgs e = args.StagingItem.Input;

                //_standardConsoleOutput.WriteLine("In preprocess input: " + e.RoutedEvent.Name);

                bool matched = false;
                InputUnit inputUnit = null;

                if (   e.RoutedEvent == Mouse.PreviewMouseDownEvent
                    || e.RoutedEvent == Mouse.PreviewMouseUpEvent
                    || e.RoutedEvent == Keyboard.PreviewKeyDownEvent
                    || e.RoutedEvent == Keyboard.PreviewKeyUpEvent
                    // || e.RoutedEvent == Mouse.PreviewMouseMoveEvent // Mouse moves happen more than we can predict
                    )
                {
                    if (s_expectedInput.Count == 0)
                    {
                        InputError("Received unexpected input: " + GetDescriptiveText(e));
                    }
                    else
                    {

                        // Not going to try to sync up actual and expected.
                        // if there's one mismatch we won't try to recover.
                        inputUnit = s_expectedInput[0];
                        s_expectedInput.RemoveAt(0);

                        if (e.RoutedEvent == Mouse.PreviewMouseMoveEvent)
                        {
                            MouseMoveInputUnit mouseMove = inputUnit as MouseMoveInputUnit;
                            if (mouseMove != null)
                            {
                                // 
                                Console.WriteLine("Received Mouse Move");
                                matched = true;
                            }
                        }

                        if (e.RoutedEvent == Mouse.PreviewMouseDownEvent || e.RoutedEvent == Mouse.PreviewMouseUpEvent)
                        {
                            MouseButtonInputUnit mouseButton = inputUnit as MouseButtonInputUnit;
                            if (mouseButton != null)
                            {
                                if (((MouseButtonEventArgs)e).ChangedButton == mouseButton.button)
                                {
                                    if ((e.RoutedEvent == Mouse.PreviewMouseDownEvent && mouseButton.press)
                                        || (e.RoutedEvent == Mouse.PreviewMouseUpEvent && !mouseButton.press))
                                    {
                                        Console.WriteLine("Received " + mouseButton);
                                        matched = true;
                                    }
                                }
                            }
                        }

                        if (e.RoutedEvent == Keyboard.PreviewKeyDownEvent || e.RoutedEvent == Keyboard.PreviewKeyUpEvent)
                        {
                            KeyInputUnit keyInput = inputUnit as KeyInputUnit;
                            if (keyInput != null)
                            {
                                Key key  = ((KeyEventArgs)e).Key;
                                if (key == Key.System)
                                {
                                    key  = ((KeyEventArgs)e).SystemKey;
                                }

                                if (key == keyInput.key)
                                {
                                    if ((e.RoutedEvent == Keyboard.PreviewKeyDownEvent && keyInput.press)
                                        || (e.RoutedEvent == Keyboard.PreviewKeyUpEvent && !keyInput.press))
                                    {
                                        Console.WriteLine("Received " + keyInput);
                                        matched = true;
                                    }
                                }
                            }
                        }

                        if (!matched)
                        {
                            string message = "Expected " + inputUnit + ", received " + GetDescriptiveText(e);
                            InputError(message);
                        }
                    }

                    // We want to make sure that we received the block of input that we sent. 
                    // Queue an item at Input priority that will check.  All the input that we sent
                    // should be read by the dispatcher before processing another input priority item
                    if (s_waitForEndOfInputGroup == null)
                    {
                        s_waitForEndOfInputGroup = Dispatcher.CurrentDispatcher.BeginInvoke(
                            DispatcherPriority.Input,
                            new DispatcherOperationCallback(ExpectEndOfInputGroup),
                            null
                            );
                    }
                }
                else if (e.RoutedEvent == Mouse.PreviewMouseWheelEvent)
                {
                    Console.WriteLine("Received " + GetDescriptiveText(e));
                }
            
            }

            private static string GetDescriptiveText(InputEventArgs e)
            {
                string message = e.RoutedEvent.Name + " (";
                if (e is KeyEventArgs)
                {
                    message += "Key = " + ((KeyEventArgs)e).Key + ", States = " + ((KeyEventArgs)e).KeyStates;
                }
                if (e is MouseButtonEventArgs)
                {
                    message += "Button = " + ((MouseButtonEventArgs)e).ChangedButton + ", State = " + ((MouseButtonEventArgs)e).ButtonState;
                }
                message += ")";
                return message;
            }

            static DispatcherOperation s_waitForEndOfInputGroup;

            private static object ExpectEndOfInputGroup(object arg)
            {
                s_waitForEndOfInputGroup = null;

                if (s_expectedInput.Count != 0)
                {
                    string message = "Did not receive all expected input, expected: ";
                    foreach (InputUnit inputUnit in s_expectedInput)
                    {
                        message += "(" + inputUnit + ") ";
                    }
                    // We'll probably never get it now, clear out the expected set
                    s_expectedInput.Clear();
                    InputError(message);

                }

                return null;
            }

            private static void InputError(string message)
            {
                if (s_mismatchedInput == WarningLevel.Warning)
                {
                    Console.WriteLine("WARNING: " + message);
                }
                else if (s_mismatchedInput == WarningLevel.Error)
                {
                    DRT.Assert(false, message);
                }
                else if (s_mismatchedInput == WarningLevel.Ignore)
                {
                }
                else if (s_mismatchedInput == WarningLevel.TentativelySucceed)
                {
                    Console.WriteLine("WARNING: " + message);
                    Console.WriteLine("Tentatively succeeding ... ");
                    DRT.WriteDelayedOutput();
                    Environment.Exit(0);
                }
            }


            static List<InputUnit> s_expectedInput = new List<InputUnit>();

            private class InputUnit
            {
            }

            private class MouseMoveInputUnit : InputUnit
            {
                public MouseMoveInputUnit(Point pt) { this.pt = pt; }

                public Point pt;

                public override string ToString()
                {
                    return "InputUnit: " + "Mouse move to " + pt;
                }
            }

            private class MouseButtonInputUnit : InputUnit
            {
                public MouseButtonInputUnit(MouseButton button, bool press) { this.button = button; this.press = press; }

                public MouseButton button;
                public bool press;

                public override string ToString()
                {
                    return "InputUnit: Mouse: " + button + " " + (press ? "Press" : "Release");
                }
            }

            private class KeyInputUnit : InputUnit
            {
                public KeyInputUnit(Key key, bool press) { this.key = key; this.press = press; }

                public Key key;
                public bool press;

                public override string ToString()
                {
                    return "InputUnit: Keyboard: " + key + " " + (press ? "Press" : "Release");
                }

            }
        }

        #endregion

        private static WarningLevel s_mismatchedInput = WarningLevel.Warning;
        private static WarningLevel s_windowDeactivated = WarningLevel.Error;

        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        internal static extern IntPtr GetFocus();
    }

}
