// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Text;
using System.Globalization;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Input;
using System.Reflection;
using System.IO;
using System.Windows.Markup;

namespace DRT
{

    public class DrtExpanderSuite : DrtTestSuite
    {
        public DrtExpanderSuite() : base("Expander")
        {
            Contact = "Microsoft";
        }

        Expander _expander;
        Expander _expanderDown;
        Expander _expanderRight;
        ListBox _listbox;
        static bool s_isPageLoaded = false;
        bool _currentIsExpanded;

        public override DrtTest[] PrepareTests()
        {
            if (!s_isPageLoaded)
            {
                string fullname = DRT.BaseDirectory + "DrtExpander.xaml";
                System.IO.Stream stream = File.OpenRead(fullname);
                Visual root = (Visual)XamlReader.Load(stream);

                InitTree(root);

                DRT.Show(root);

                s_isPageLoaded = true;
            }
            else
            {
                Keyboard.Focus(null);
            }

            //Because the animation duration is 300 miliseconds
            //DRT.TestFrequency=300;

            if (!DRT.KeepAlive)
            {
                List<DrtTest> tests = new List<DrtTest>();
                tests.Add(new DrtTest(Start));

                tests.Add(new DrtTest(BasicTest));

                tests.Add(new DrtTest(ExpandDirectionTest));

                tests.Add(new DrtTest(KeyboardInputTest));

                tests.Add(new DrtTest(MouseInputTest));

                tests.Add(new DrtTest(Cleanup));

                return tests.ToArray();
            }
            else
            {
                return new DrtTest[]{};
            }
        }

        private void InitTree(DependencyObject root)
        {
            _expander = DRT.FindElementByID("expander", root) as Expander;
            _expanderDown = DRT.FindElementByID("expanderDown", root) as Expander;
            _listbox = DRT.FindElementByID("listbox", root) as ListBox;
            _expanderRight = DRT.FindElementByID("expanderRight", root) as Expander;
            
            DRT.Assert(_expander != null, "Can't find the element whose ID is 'expander' in DrtExpander.xaml");
            DRT.Assert(_expanderDown != null, "Can't find the element whose ID is 'expanderDown' in DrtExpander.xaml");
            DRT.Assert(_listbox != null, "Can't find the element whose ID is 'listbox' in DrtExpander.xaml");
            DRT.Assert(_expanderRight != null, "Can't find the element whose ID is 'expanderRight' in DrtExpander.xaml");

            ((FrameworkElement)root).DataContext = this;
        }

        DispatcherTimer _suicideTimer = null;

        private void Start()
        {
            if (!DRT.KeepAlive)
            {
                _suicideTimer = new DispatcherTimer();
                _suicideTimer.Interval = new TimeSpan(0, 5, 0);
                _suicideTimer.Tick += new EventHandler(OnTimeout);
                _suicideTimer.Start();
            }
        }

        public void Cleanup()
        {
            if (_suicideTimer != null)
            {
                _suicideTimer.Stop();
            }
        }

        private void OnTimeout(object sender, EventArgs e)
        {
            throw new TimeoutException();
        }

        private class TimeoutException : Exception
        {
            public TimeoutException() : base("Timeout expired, quitting") { }
        }

        void OnExpanded(object sender, RoutedEventArgs e)
        {
            _isExpanded = true;
        }

        void OnCollapsed(object sender, RoutedEventArgs e)
        {
            _isCollapsed = true;
        }

        #region BasicTests

        private bool _isExpanded;
        private bool _isCollapsed;

        private void InitFlags()
        {
            _isExpanded = false;
            _isCollapsed = false;
        }

        public void BasicTest()
        {
            //Verify the default property values
            DRT.Assert(ExpandDirection.Down == _expander.ExpandDirection, "Basic-1 Expander.ExpandDirection should be Down by default");
            DRT.Assert(!_expander.IsExpanded, "Basic-2 Expander.IsExpanded should be false by default");

            //Verify the property values in xaml
            DRT.Assert(ExpandDirection.Right == _expanderRight.ExpandDirection, "Basic-3 _expanderRight.ExpandDirection should be Right because it's set in xaml");
            DRT.Assert(_expanderDown.IsExpanded, "Basic-4 _expanderDown.IsExpanded should be false");

            //Events
            _expander.Expanded += new RoutedEventHandler(OnExpanded);
            _expander.Collapsed += new RoutedEventHandler(OnCollapsed);
            _expanderDown.Expanded += new RoutedEventHandler(OnExpanded);
            _expanderDown.Collapsed += new RoutedEventHandler(OnCollapsed);

            //IsExpanded value should reflect the events
            InitFlags();
            _expander.IsExpanded = true;
            DRT.Assert(_isExpanded, "Basic-5 Expanded event should be fired");
            DRT.Assert(!_isCollapsed, "Basic-6 Collapsed event should not be fired");

            InitFlags();
            _expander.IsExpanded = false;
            DRT.Assert(!_isExpanded, "Basic-7 Expanded event should not be fired");
            DRT.Assert(_isCollapsed, "Basic-8 Collapsed event should be fired");


            //set the _expanderDown.IsExpanded to true
            InitFlags();
            _expanderDown.IsExpanded = true;


            //verify IExpandCollapseProvider interface works
            InitFlags();
            _expander.IsExpanded = false;

            AutomationPeer eap = UIElementAutomationPeer.CreatePeerForElement(_expander);
            DRT.Assert(eap != null,"Basic-9 Expander should has an automation peer");
            IExpandCollapseProvider provider = (IExpandCollapseProvider)eap;

            DoExpand(_expander);
            DRT.Assert(ExpandCollapseState.Expanded == provider.ExpandCollapseState, 
                "Basic-10 ExpandCollapseState should be Expanded");

            DoCollapse(_expander);
            DRT.Assert(ExpandCollapseState.Collapsed == provider.ExpandCollapseState, 
                "Basic-11 ExpandCollapseState should be Collapsed");
        }

        #endregion

        enum ExpandDirectionStep
        {
            Start,
            //Down
            Test1_Down,
            Test1_Verify,

            //Up
            Test2_Up,
            Test2_Verify,

            //Left
            Test3_Left,
            Test3_Verify,

            //Right
            Test4_Right,
            Test4_Verify,

            End,
        }

        ExpandDirectionStep _expandDirectionStep;

        public void ExpandDirectionTest()
        {
            if (DRT.Verbose) Console.WriteLine("ExpandDirectionTest: " + _expandDirectionStep);

            switch (_expandDirectionStep)
            {
                case ExpandDirectionStep.Start:
                    _expander.IsExpanded = true;
                    break;

                ////////////////////////////

                case ExpandDirectionStep.Test1_Down:
                    _expander.ExpandDirection = ExpandDirection.Down;
                    break;

                case ExpandDirectionStep.Test1_Verify:
                    DRT.Assert(ExpandDirection.Down == _expander.ExpandDirection,
                        "ED-1" + _expander.Name + ".ExpandDirection should be Down");
                    break;

                ////////////////////////////

                case ExpandDirectionStep.Test2_Up:
                    _expander.ExpandDirection = ExpandDirection.Up;
                    break;

                case ExpandDirectionStep.Test2_Verify:
                    DRT.Assert(ExpandDirection.Up == _expander.ExpandDirection,
                        "ED-2" + _expander.Name + ".ExpandDirection should be Up");
                    break;


                ////////////////////////////

                case ExpandDirectionStep.Test3_Left:
                    _expander.ExpandDirection = ExpandDirection.Left;
                    break;

                case ExpandDirectionStep.Test3_Verify:
                    DRT.Assert(ExpandDirection.Left == _expander.ExpandDirection,
                        "ED-3" + _expander.Name + ".ExpandDirection should be Left");
                    break;

                ////////////////////////////

                case ExpandDirectionStep.Test4_Right:
                    _expander.ExpandDirection = ExpandDirection.Right;
                    break;

                case ExpandDirectionStep.Test4_Verify:
                    DRT.Assert(ExpandDirection.Right == _expander.ExpandDirection,
                        "ED-4" + _expander.Name + ".ExpandDirection should be Right");
                    break;

                case ExpandDirectionStep.End:
                    _expander.ExpandDirection = ExpandDirection.Down;
                    _expander.IsExpanded = false;
                    break;
                default:
                    break;
            }

            if (_expandDirectionStep++ <= ExpandDirectionStep.End)
            {
                DRT.RepeatTest();
            }
        }

        #region Input Tests

        enum KeyboardInputStep
        {
            Start,
            //Invoke with Spacebar
            Test1_Focus,
            Test1_SpaceBarDown,
            Test1_SpaceBarUp,
            Test1_Verify,

            // Invoke with Enter
            Test2_EnterDown,
            Test2_EnterUp,
            Test2_Verify,

            // Invoke with Tab on Collapsed expander
            Test3_Focus,
            Test3_Tab,
            Test3_Verify,

            // Invoke with Tab on Expanded expander with non-focusable content
            Test4_Focus,
            Test4_Tab,
            Test4_Verify,

            // Invoke with Tab on Expanded expander with focusable content
            Test5_Focus,
            Test5_Tab,
            Test5_Verify,

            End,
        }

        KeyboardInputStep _keyboardInputStep;
        //Keyboard Behavior
        //Spacebar - MoveTo, Down, Up
        //Enter - MoveTo, Down, Up
        //Tab - non-focus and focus content
        //Left/Right navigation
        //Up/Down navigation
        public void KeyboardInputTest()
        {
            if (DRT.Verbose) Console.WriteLine("KeyboardInputTest: " + _keyboardInputStep);

            ToggleButton headerSite = null;
            
            switch (_keyboardInputStep)
            {
                case KeyboardInputStep.Start:
                    break;

                ////////////////////////////

                case KeyboardInputStep.Test1_Focus:
                    headerSite = DRT.FindElementByID("HeaderSite", _expanderDown) as ToggleButton;
                    DRT.MoveMouse(headerSite, 0.5, 0.5);
                    _currentIsExpanded = _expanderDown.IsExpanded;
                    DRT.ClickMouse();
                    //_expanderDown.Focus();
                    break;

                case KeyboardInputStep.Test1_SpaceBarDown:
                    DRT.Assert(_expanderDown.IsKeyboardFocusWithin, 
                        "KI-1" + _expanderDown.Name + " should have focus");
                    DRT.SendKeyboardInput(Key.Space, true);
                    break;

                case KeyboardInputStep.Test1_SpaceBarUp:
                    DRT.Assert(!_currentIsExpanded == _expanderDown.IsExpanded,
                        "KI-2" + _expanderDown.Name + ".IsExpanded should be " + (!_currentIsExpanded).ToString());
                    DRT.SendKeyboardInput(Key.Space, false);
                    break;

                case KeyboardInputStep.Test1_Verify:
                    DRT.Assert(_currentIsExpanded == _expanderDown.IsExpanded,
                        "KI-3" + _expanderDown.Name + ".IsExpanded should be " + _currentIsExpanded.ToString());
                    break;

                ////////////////////

                case KeyboardInputStep.Test2_EnterDown:
                    _currentIsExpanded = _expanderDown.IsExpanded;
                    DRT.Assert(_expanderDown.IsKeyboardFocusWithin,
                        "KI-4" + _expanderDown.Name + " should have focus");
                    DRT.SendKeyboardInput(Key.Enter, true);
                    break;

                case KeyboardInputStep.Test2_EnterUp:
                    DRT.Assert(!_currentIsExpanded == _expanderDown.IsExpanded,
                        "KI-5" + _expanderDown.Name + ".IsExpanded should be " + (!_currentIsExpanded).ToString());
                    DRT.SendKeyboardInput(Key.Enter, false);
                    break;

                case KeyboardInputStep.Test2_Verify:
                    DRT.Assert(!_currentIsExpanded == _expanderDown.IsExpanded,
                        "KI-6" + _expanderDown.Name + ".IsExpanded should be " + (!_currentIsExpanded).ToString());
                    break;

                ////////////////////
                //Invoke with Tab on Collapsed expander
                case KeyboardInputStep.Test3_Focus:
                    _expander.IsExpanded = true;
                    headerSite = DRT.FindElementByID("HeaderSite", _expander) as ToggleButton;
                    DRT.MoveMouse(headerSite, 0.5, 0.5);
                    DRT.ClickMouse();
                    break;

                case KeyboardInputStep.Test3_Tab:
                    DRT.Assert(!_expander.IsExpanded, 
                        "KI-7" + _expander.Name + ".IsExpanded should be false");
                    DRT.Assert(_expander.IsKeyboardFocusWithin, 
                        "KI-8" + _expander.Name + " should have focus");
                    DRT.PressKey(Key.Tab);
                    break;

                case KeyboardInputStep.Test3_Verify:
                    DRT.Assert(!_expander.IsKeyboardFocusWithin, 
                        "KI-9" + _expander.Name + " should not have focus");
                    break;

                ////////////////////
                //Invoke with Tab on Expanded expander with non-focusable content
                case KeyboardInputStep.Test4_Focus:
                    _expander.IsExpanded = false;
                    headerSite = DRT.FindElementByID("HeaderSite", _expander) as ToggleButton;
                    DRT.MoveMouse(headerSite, 0.5, 0.5);
                    DRT.ClickMouse();
                    break;

                case KeyboardInputStep.Test4_Tab:
                    DRT.Assert(_expander.IsExpanded, 
                        "KI-10" + _expander.Name + ".IsExpanded should be true");
                    DRT.Assert(_expander.IsKeyboardFocusWithin, 
                        "KI-11" + _expander.Name + " should have focus");
                    DRT.PressKey(Key.Tab);
                    break;

                case KeyboardInputStep.Test4_Verify:
                    DRT.Assert(!_expander.IsKeyboardFocusWithin, 
                        "KI-12" + _expander.Name + " should not have focus");
                    break;

                ////////////////////
                // Invoke with Tab on Expanded expander with focusable content
                case KeyboardInputStep.Test5_Focus:
                    _expanderDown.IsExpanded = false;
                    headerSite = DRT.FindElementByID("HeaderSite", _expanderDown) as ToggleButton;
                    DRT.MoveMouse(headerSite, 0.5, 0.5);
                    DRT.ClickMouse();
                    break;

                case KeyboardInputStep.Test5_Tab:
                    DRT.Assert(_expanderDown.IsExpanded, 
                        "KI-13" + _expanderDown.Name + ".IsExpanded should be true");
                    DRT.Assert(_expanderDown.IsKeyboardFocusWithin, 
                        "KI-14" + _expanderDown.Name + " should have focus");
                    DRT.PressKey(Key.Tab);
                    break;

                case KeyboardInputStep.Test5_Verify:
                    DRT.Assert(_listbox.IsKeyboardFocusWithin, 
                        "KI-15" + _expanderDown.Name + ".Content should have focus");
                    break;



                case KeyboardInputStep.End:
                default:
                    break;
            }

            if (_keyboardInputStep++ <= KeyboardInputStep.End)
            {
                DRT.RepeatTest();
            }
        }

        enum MouseInputStep
        {
            Start,
            // Try clicking on the Expander
            Test1_MoveTo,
            Test1_MouseClick,
            Test1_Verify,
            Test2_MouseClick,
            Test2_Verify,

            End,
        }

        MouseInputStep _mouseInputStep;

        public void MouseInputTest()
        {
            ToggleButton headerSite = null;
            
            switch (_mouseInputStep)
            {
                case MouseInputStep.Start:
                    break;

                case MouseInputStep.Test1_MoveTo:
                    headerSite = DRT.FindElementByID("HeaderSite", _expanderRight) as ToggleButton;
                    DRT.MoveMouse(headerSite, 0.5, 0.5);
                    break;

                case MouseInputStep.Test1_MouseClick:
                case MouseInputStep.Test2_MouseClick:
                    _currentIsExpanded = _expanderRight.IsExpanded;
                    DRT.ClickMouse();
                    break;

                case MouseInputStep.Test1_Verify:
                case MouseInputStep.Test2_Verify:
                    DRT.Assert(!_currentIsExpanded == _expanderRight.IsExpanded, _expanderRight.Name + ".IsExpanded should be " + (!_currentIsExpanded).ToString());
                    break;

                case MouseInputStep.End:
                default:
                    break;
            }

            if (_mouseInputStep++ <= MouseInputStep.End)
            {
                DRT.RepeatTest();
            }
        }

        #endregion

        #region Helper

        /// <summary>
        /// Helper. Invokes Click on a given uielement. 
        /// </summary>
        private void DoExpand(UIElement uielement)
        {
            if (uielement != null)
            {
                AutomationPeer ap = UIElementAutomationPeer.CreatePeerForElement(uielement);
                if (ap != null)
                {
                    IExpandCollapseProvider provider = (IExpandCollapseProvider)ap.GetPattern(PatternInterface.ExpandCollapse);
                    if (provider != null)
                    {
                        provider.Expand();
                    }
                }
            }
        }

        /// <summary>
        /// Helper. Invokes Toggle on a given uielement. 
        /// </summary>
        private void DoCollapse(UIElement uielement)
        {
            if (uielement != null)
            {
                AutomationPeer ap = UIElementAutomationPeer.CreatePeerForElement(uielement);
                if (ap != null)
                {
                    IExpandCollapseProvider provider = (IExpandCollapseProvider)ap.GetPattern(PatternInterface.ExpandCollapse);
                    if (provider != null)
                    {
                        provider.Collapse();
                    }
                }
            }
        }


        #endregion

    }
}
