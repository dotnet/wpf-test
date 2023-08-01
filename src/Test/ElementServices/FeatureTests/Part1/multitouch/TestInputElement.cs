// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace Microsoft.Test.Input.MultiTouch.Tests
{
    /// <summary>
    /// The purpose of this class is to have a custom implementation of IInputElement 
    /// so we can use one object for various invalid cases in test code 
    /// 
    /// </summary>
    public class TestInputElement: IInputElement
    {
        #region IInputElement Members

        void IInputElement.AddHandler(RoutedEvent routedEvent, Delegate handler)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        bool IInputElement.CaptureMouse()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        bool IInputElement.CaptureStylus()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        bool IInputElement.Focus()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        bool IInputElement.Focusable
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        event KeyboardFocusChangedEventHandler IInputElement.GotKeyboardFocus
        {
            add { throw new Exception("The method or operation is not implemented."); }
            remove { throw new Exception("The method or operation is not implemented."); }
        }

        event MouseEventHandler IInputElement.GotMouseCapture
        {
            add { throw new Exception("The method or operation is not implemented."); }
            remove { throw new Exception("The method or operation is not implemented."); }
        }

        event StylusEventHandler IInputElement.GotStylusCapture
        {
            add { throw new Exception("The method or operation is not implemented."); }
            remove { throw new Exception("The method or operation is not implemented."); }
        }

        bool IInputElement.IsEnabled
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        bool IInputElement.IsKeyboardFocusWithin
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        bool IInputElement.IsKeyboardFocused
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        bool IInputElement.IsMouseCaptured
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        bool IInputElement.IsMouseDirectlyOver
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        bool IInputElement.IsMouseOver
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        bool IInputElement.IsStylusCaptured
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        bool IInputElement.IsStylusDirectlyOver
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        bool IInputElement.IsStylusOver
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        event KeyEventHandler IInputElement.KeyDown
        {
            add { throw new Exception("The method or operation is not implemented."); }
            remove { throw new Exception("The method or operation is not implemented."); }
        }

        event KeyEventHandler IInputElement.KeyUp
        {
            add { throw new Exception("The method or operation is not implemented."); }
            remove { throw new Exception("The method or operation is not implemented."); }
        }

        event KeyboardFocusChangedEventHandler IInputElement.LostKeyboardFocus
        {
            add { throw new Exception("The method or operation is not implemented."); }
            remove { throw new Exception("The method or operation is not implemented."); }
        }

        event MouseEventHandler IInputElement.LostMouseCapture
        {
            add { throw new Exception("The method or operation is not implemented."); }
            remove { throw new Exception("The method or operation is not implemented."); }
        }

        event StylusEventHandler IInputElement.LostStylusCapture
        {
            add { throw new Exception("The method or operation is not implemented."); }
            remove { throw new Exception("The method or operation is not implemented."); }
        }

        event MouseEventHandler IInputElement.MouseEnter
        {
            add { throw new Exception("The method or operation is not implemented."); }
            remove { throw new Exception("The method or operation is not implemented."); }
        }

        event MouseEventHandler IInputElement.MouseLeave
        {
            add { throw new Exception("The method or operation is not implemented."); }
            remove { throw new Exception("The method or operation is not implemented."); }
        }

        event MouseButtonEventHandler IInputElement.MouseLeftButtonDown
        {
            add { throw new Exception("The method or operation is not implemented."); }
            remove { throw new Exception("The method or operation is not implemented."); }
        }

        event MouseButtonEventHandler IInputElement.MouseLeftButtonUp
        {
            add { throw new Exception("The method or operation is not implemented."); }
            remove { throw new Exception("The method or operation is not implemented."); }
        }

        event MouseEventHandler IInputElement.MouseMove
        {
            add { throw new Exception("The method or operation is not implemented."); }
            remove { throw new Exception("The method or operation is not implemented."); }
        }

        event MouseButtonEventHandler IInputElement.MouseRightButtonDown
        {
            add { throw new Exception("The method or operation is not implemented."); }
            remove { throw new Exception("The method or operation is not implemented."); }
        }

        event MouseButtonEventHandler IInputElement.MouseRightButtonUp
        {
            add { throw new Exception("The method or operation is not implemented."); }
            remove { throw new Exception("The method or operation is not implemented."); }
        }

        event MouseWheelEventHandler IInputElement.MouseWheel
        {
            add { throw new Exception("The method or operation is not implemented."); }
            remove { throw new Exception("The method or operation is not implemented."); }
        }

        event KeyboardFocusChangedEventHandler IInputElement.PreviewGotKeyboardFocus
        {
            add { throw new Exception("The method or operation is not implemented."); }
            remove { throw new Exception("The method or operation is not implemented."); }
        }

        event KeyEventHandler IInputElement.PreviewKeyDown
        {
            add { throw new Exception("The method or operation is not implemented."); }
            remove { throw new Exception("The method or operation is not implemented."); }
        }

        event KeyEventHandler IInputElement.PreviewKeyUp
        {
            add { throw new Exception("The method or operation is not implemented."); }
            remove { throw new Exception("The method or operation is not implemented."); }
        }

        event KeyboardFocusChangedEventHandler IInputElement.PreviewLostKeyboardFocus
        {
            add { throw new Exception("The method or operation is not implemented."); }
            remove { throw new Exception("The method or operation is not implemented."); }
        }

        event MouseButtonEventHandler IInputElement.PreviewMouseLeftButtonDown
        {
            add { throw new Exception("The method or operation is not implemented."); }
            remove { throw new Exception("The method or operation is not implemented."); }
        }

        event MouseButtonEventHandler IInputElement.PreviewMouseLeftButtonUp
        {
            add { throw new Exception("The method or operation is not implemented."); }
            remove { throw new Exception("The method or operation is not implemented."); }
        }

        event MouseEventHandler IInputElement.PreviewMouseMove
        {
            add { throw new Exception("The method or operation is not implemented."); }
            remove { throw new Exception("The method or operation is not implemented."); }
        }

        event MouseButtonEventHandler IInputElement.PreviewMouseRightButtonDown
        {
            add { throw new Exception("The method or operation is not implemented."); }
            remove { throw new Exception("The method or operation is not implemented."); }
        }

        event MouseButtonEventHandler IInputElement.PreviewMouseRightButtonUp
        {
            add { throw new Exception("The method or operation is not implemented."); }
            remove { throw new Exception("The method or operation is not implemented."); }
        }

        event MouseWheelEventHandler IInputElement.PreviewMouseWheel
        {
            add { throw new Exception("The method or operation is not implemented."); }
            remove { throw new Exception("The method or operation is not implemented."); }
        }

        event StylusButtonEventHandler IInputElement.PreviewStylusButtonDown
        {
            add { throw new Exception("The method or operation is not implemented."); }
            remove { throw new Exception("The method or operation is not implemented."); }
        }

        event StylusButtonEventHandler IInputElement.PreviewStylusButtonUp
        {
            add { throw new Exception("The method or operation is not implemented."); }
            remove { throw new Exception("The method or operation is not implemented."); }
        }

        event StylusDownEventHandler IInputElement.PreviewStylusDown
        {
            add { throw new Exception("The method or operation is not implemented."); }
            remove { throw new Exception("The method or operation is not implemented."); }
        }

        event StylusEventHandler IInputElement.PreviewStylusInAirMove
        {
            add { throw new Exception("The method or operation is not implemented."); }
            remove { throw new Exception("The method or operation is not implemented."); }
        }

        event StylusEventHandler IInputElement.PreviewStylusInRange
        {
            add { throw new Exception("The method or operation is not implemented."); }
            remove { throw new Exception("The method or operation is not implemented."); }
        }

        event StylusEventHandler IInputElement.PreviewStylusMove
        {
            add { throw new Exception("The method or operation is not implemented."); }
            remove { throw new Exception("The method or operation is not implemented."); }
        }

        event StylusEventHandler IInputElement.PreviewStylusOutOfRange
        {
            add { throw new Exception("The method or operation is not implemented."); }
            remove { throw new Exception("The method or operation is not implemented."); }
        }

        event StylusSystemGestureEventHandler IInputElement.PreviewStylusSystemGesture
        {
            add { throw new Exception("The method or operation is not implemented."); }
            remove { throw new Exception("The method or operation is not implemented."); }
        }

        event StylusEventHandler IInputElement.PreviewStylusUp
        {
            add { throw new Exception("The method or operation is not implemented."); }
            remove { throw new Exception("The method or operation is not implemented."); }
        }

        event TextCompositionEventHandler IInputElement.PreviewTextInput
        {
            add { throw new Exception("The method or operation is not implemented."); }
            remove { throw new Exception("The method or operation is not implemented."); }
        }

        void IInputElement.RaiseEvent(RoutedEventArgs e)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        void IInputElement.ReleaseMouseCapture()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        void IInputElement.ReleaseStylusCapture()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        void IInputElement.RemoveHandler(RoutedEvent routedEvent, Delegate handler)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        event StylusButtonEventHandler IInputElement.StylusButtonDown
        {
            add { throw new Exception("The method or operation is not implemented."); }
            remove { throw new Exception("The method or operation is not implemented."); }
        }

        event StylusButtonEventHandler IInputElement.StylusButtonUp
        {
            add { throw new Exception("The method or operation is not implemented."); }
            remove { throw new Exception("The method or operation is not implemented."); }
        }

        event StylusDownEventHandler IInputElement.StylusDown
        {
            add { throw new Exception("The method or operation is not implemented."); }
            remove { throw new Exception("The method or operation is not implemented."); }
        }

        event StylusEventHandler IInputElement.StylusEnter
        {
            add { throw new Exception("The method or operation is not implemented."); }
            remove { throw new Exception("The method or operation is not implemented."); }
        }

        event StylusEventHandler IInputElement.StylusInAirMove
        {
            add { throw new Exception("The method or operation is not implemented."); }
            remove { throw new Exception("The method or operation is not implemented."); }
        }

        event StylusEventHandler IInputElement.StylusInRange
        {
            add { throw new Exception("The method or operation is not implemented."); }
            remove { throw new Exception("The method or operation is not implemented."); }
        }

        event StylusEventHandler IInputElement.StylusLeave
        {
            add { throw new Exception("The method or operation is not implemented."); }
            remove { throw new Exception("The method or operation is not implemented."); }
        }

        event StylusEventHandler IInputElement.StylusMove
        {
            add { throw new Exception("The method or operation is not implemented."); }
            remove { throw new Exception("The method or operation is not implemented."); }
        }

        event StylusEventHandler IInputElement.StylusOutOfRange
        {
            add { throw new Exception("The method or operation is not implemented."); }
            remove { throw new Exception("The method or operation is not implemented."); }
        }

        event StylusSystemGestureEventHandler IInputElement.StylusSystemGesture
        {
            add { throw new Exception("The method or operation is not implemented."); }
            remove { throw new Exception("The method or operation is not implemented."); }
        }

        event StylusEventHandler IInputElement.StylusUp
        {
            add { throw new Exception("The method or operation is not implemented."); }
            remove { throw new Exception("The method or operation is not implemented."); }
        }

        event TextCompositionEventHandler IInputElement.TextInput
        {
            add { throw new Exception("The method or operation is not implemented."); }
            remove { throw new Exception("The method or operation is not implemented."); }
        }

        #endregion
    }
}
