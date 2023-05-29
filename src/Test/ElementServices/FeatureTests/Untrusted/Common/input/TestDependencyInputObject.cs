// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Diagnostics;
using System.Threading;
using System.Windows.Threading;

using System.Windows;
using System.Windows.Input;

namespace Avalon.Test.CoreUI.CoreInput.Common.Controls
{
    /// <summary>
    /// A dependency object that is also an input element (IInputElement).
    /// </summary>
    /// <remarks>
    /// By design, an object of this class cannot actually be used for input.
    /// Therefore, it is a good class to use for negative testing of input APIs that accept IInputElement.
    /// All interface members are implemented, but stubbed out.
    /// </remarks>
    internal class TestDependencyInputObject: DependencyObject, IInputElement
    {
        #region Keyboard and Mouse Events

        public event KeyboardFocusChangedEventHandler GotKeyboardFocus;
        public event MouseEventHandler GotMouseCapture;
        public event KeyEventHandler KeyDown;
        public event KeyEventHandler KeyUp;
        public event KeyboardFocusChangedEventHandler LostKeyboardFocus;
        public event MouseEventHandler LostMouseCapture;
        public event MouseEventHandler MouseEnter;
        public event MouseEventHandler MouseLeave;
        public event MouseButtonEventHandler MouseLeftButtonDown;
        public event MouseButtonEventHandler MouseLeftButtonUp;
        public event MouseEventHandler MouseMove;
        public event MouseButtonEventHandler MouseRightButtonDown;
        public event MouseButtonEventHandler MouseRightButtonUp;
        public event MouseWheelEventHandler MouseWheel;
        public event KeyboardFocusChangedEventHandler PreviewGotKeyboardFocus;
        public event KeyEventHandler PreviewKeyDown;
        public event KeyEventHandler PreviewKeyUp;
        public event KeyboardFocusChangedEventHandler PreviewLostKeyboardFocus;
        public event MouseButtonEventHandler PreviewMouseLeftButtonDown;
        public event MouseButtonEventHandler PreviewMouseLeftButtonUp;
        public event MouseEventHandler PreviewMouseMove;
        public event MouseButtonEventHandler PreviewMouseRightButtonDown;
        public event MouseButtonEventHandler PreviewMouseRightButtonUp;
        public event MouseWheelEventHandler PreviewMouseWheel;
        public event TextCompositionEventHandler PreviewTextInput;
        public event TextCompositionEventHandler TextInput;

        #endregion

        #region Stylus Events

        public event StylusEventHandler GotStylusCapture;
        public event StylusEventHandler LostStylusCapture;
        public event StylusEventHandler PreviewStylusEnter;
        public event StylusEventHandler PreviewStylusInRange;
        public event StylusEventHandler PreviewStylusLeave;
        public event StylusEventHandler PreviewStylusOutOfRange;
        public event StylusDownEventHandler PreviewStylusDown;
        public event StylusEventHandler PreviewStylusUp;
        public event StylusEventHandler PreviewStylusMove;
        public event StylusEventHandler PreviewStylusInAirMove;
        public event StylusSystemGestureEventHandler PreviewStylusSystemGesture;
        public event StylusDownEventHandler StylusDown;
        public event StylusEventHandler StylusEnter;
        public event StylusEventHandler StylusInAirMove;
        public event StylusEventHandler StylusInRange;
        public event StylusEventHandler StylusLeave;
        public event StylusEventHandler StylusMove;
        public event StylusEventHandler StylusOutOfRange;
        public event StylusSystemGestureEventHandler StylusSystemGesture;
        public event StylusEventHandler StylusUp;
        public event StylusButtonEventHandler PreviewStylusButtonDown;
        public event StylusButtonEventHandler PreviewStylusButtonUp;
        public event StylusButtonEventHandler StylusButtonDown;
        public event StylusButtonEventHandler StylusButtonUp;

        #endregion

        #region Methods

        // Method to touch all the above events.
        // Implemented only to avoid compiler error CS0067.
        private void TestEvents() 
        {
            if (GotKeyboardFocus != null) { GotKeyboardFocus(null,null); }
            if (GotMouseCapture != null) { GotMouseCapture(null,null); }
            if (KeyDown != null) { KeyDown(null,null); }
            if (KeyUp != null) { KeyUp(null,null); }
            if (LostKeyboardFocus != null) { LostKeyboardFocus(null,null); }
            if (LostMouseCapture != null) { LostMouseCapture(null,null); }
            if (MouseEnter != null) { MouseEnter(null,null); }
            if (MouseLeave != null) { MouseLeave(null,null); }
            if (MouseLeftButtonDown != null) { MouseLeftButtonDown(null,null); }
            if (MouseLeftButtonUp != null) { MouseLeftButtonUp(null,null); }
            if (MouseMove != null) { MouseMove(null,null); }
            if (MouseRightButtonDown != null) { MouseRightButtonDown(null,null); }
            if (MouseRightButtonUp != null) { MouseRightButtonUp(null,null); }
            if (MouseWheel != null) { MouseWheel(null,null); }
            if (PreviewGotKeyboardFocus != null) { PreviewGotKeyboardFocus(null,null); }
            if (PreviewKeyDown != null) { PreviewKeyDown(null,null); }
            if (PreviewKeyUp != null) { PreviewKeyUp(null,null); }
            if (PreviewLostKeyboardFocus != null) { PreviewLostKeyboardFocus(null,null); }
            if (PreviewMouseLeftButtonDown != null) { PreviewMouseLeftButtonDown(null,null); }
            if (PreviewMouseLeftButtonUp != null) { PreviewMouseLeftButtonUp(null,null); }
            if (PreviewMouseMove != null) { PreviewMouseMove(null,null); }
            if (PreviewMouseRightButtonDown != null) { PreviewMouseRightButtonDown(null,null); }
            if (PreviewMouseRightButtonUp != null) { PreviewMouseRightButtonUp(null,null); }
            if (PreviewMouseWheel != null) { PreviewMouseWheel(null,null); }
            if (PreviewTextInput != null) { PreviewTextInput(null,null); }
            if (TextInput != null) { TextInput(null,null); }
            if (GotStylusCapture != null) { GotStylusCapture(null,null); }
            if (LostStylusCapture != null) { LostStylusCapture(null,null); }
            if (PreviewStylusEnter != null) { PreviewStylusEnter(null,null); }
            if (PreviewStylusInRange != null) { PreviewStylusInRange(null,null); }
            if (PreviewStylusLeave != null) { PreviewStylusLeave(null,null); }
            if (PreviewStylusOutOfRange != null) { PreviewStylusOutOfRange(null,null); }
            if (PreviewStylusDown != null) { PreviewStylusDown(null, null); }
            if (PreviewStylusUp != null) { PreviewStylusUp(null, null); }
            if (PreviewStylusMove != null) { PreviewStylusMove(null, null); }
            if (PreviewStylusInAirMove != null) { PreviewStylusInAirMove(null, null); }
            if (PreviewStylusSystemGesture != null) { PreviewStylusSystemGesture(null,null); }
            if (StylusDown != null) { StylusDown(null,null); }
            if (StylusEnter != null) { StylusEnter(null,null); }
            if (StylusInAirMove != null) { StylusInAirMove(null,null); }
            if (StylusInRange != null) { StylusInRange(null,null); }
            if (StylusLeave != null) { StylusLeave(null,null); }
            if (StylusMove != null) { StylusMove(null,null); }
            if (StylusOutOfRange != null) { StylusOutOfRange(null,null); }
            if (StylusSystemGesture != null) { StylusSystemGesture(null,null); }
            if (StylusUp != null) { StylusUp(null, null); }
            if (PreviewStylusButtonDown != null) { PreviewStylusButtonDown(null, null); }
            if (PreviewStylusButtonUp != null) { PreviewStylusButtonUp(null, null); }
            if (StylusButtonDown != null) { StylusButtonDown(null, null); }
            if (StylusButtonUp != null) { StylusButtonUp(null, null); }
        }

        public bool CaptureMouse() { return false; }
        public bool Focus() { return false; }
        public RoutedEvent[] GetRoutedEvents() { return null; }
        public void RaiseEvent(RoutedEventArgs args) { }
        public void ReleaseMouseCapture() { }
        public bool CaptureStylus() { return false; }
        public void ReleaseStylusCapture() { }

        /// <summary>
        ///     Builds the event route
        /// </summary>
        /// <param name="route">
        ///     The <see cref="EventRoute"/> being
        ///     built
        /// </param>
        /// <param name="args">
        ///     <see cref="RoutedEventArgs"/> for the RoutedEvent to be raised
        ///     post building the route.
        /// </param>
        public void BuildRoute(EventRoute route, RoutedEventArgs args) { }

        /// <summary>
        ///     Add an instance handler for the given RoutedEvent
        /// </summary>
        public void AddHandler(RoutedEvent routedEvent, Delegate handler) { }

        /// <summary>
        ///     Remove all instances of the given 
        ///     handler for the given RoutedEvent
        /// </summary>
        /// <param name="routedEvent"/>
        /// <param name="handler"/>
        public void RemoveHandler(RoutedEvent routedEvent, Delegate handler) { }
        /// <summary>
        ///     Get RoutedEvents with handlers
        /// </summary>
        /// <remarks>
        ///     Used by XamlSerializer to serialize Events
        /// </remarks>
        public RoutedEvent[] GetRoutedEventsWithHandlers(){return null;}

        #endregion

        #region Properties

        public bool IsEnabled { get { return false; } }
        public bool IsKeyboardFocused { get { return false; } }
        public bool IsMouseCaptured { get { return false; } }
        public bool IsMouseDirectlyOver { get { return false; } }
        public bool IsStylusCaptured { get { return false; } }
        public bool IsMouseOver      { get { return false; } }
        public bool IsStylusDirectlyOver { get { return false; } }
        public bool IsStylusOver         { get { return false; } }
        public bool IsKeyboardFocusWithin        { get { return false; } }

         
         //   quick and dirty fix to get past build break caused by change windows@154809
        public bool Focusable {
            get { return _focusable; }
            set { _focusable = value; }
        }
        private bool _focusable;
        
       #endregion Properties

    }
}
