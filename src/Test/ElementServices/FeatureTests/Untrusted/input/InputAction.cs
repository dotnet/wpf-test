// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Define handlers for input test actions.
 * 
 *
 
  
 * Revision:         $Revision$
 
 * Filename:         $Source$
********************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using System.Xml;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Parser;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Threading;
using Avalon.Test.CoreUI.Trusted;

using Microsoft.Test.Discovery;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.Modeling;
using Microsoft.Test.Serialization;
using Microsoft.Test.Threading;
using Microsoft.Test.Win32;

namespace Avalon.Test.CoreUI.CoreInput
{
    /// <summary>
    /// Test action handler for turning Ime on and off.
    /// </summary>
    public class SetImeStateAction : SequenceAction
    {
        /// <summary>
        /// </summary>
        public SetImeStateAction()
            : base()
        {
        }

        /// <summary>
        /// </summary>
        public override void DoAction(DependencyObject root)
        {
            InputMethod.SetPreferredImeState(root, this.InputMethodState);
        }

        /// <summary>
        /// </summary>
        public override string ToString()
        {
            return "Set InputMethodState " + this.InputMethodState.ToString();
        }

        /// <summary>
        /// </summary>
        public override int GetHashCode()
        {
            int hashCode = base.GetHashCode();

            hashCode ^= this.InputMethodState.GetHashCode();

            return hashCode;
        }

        /// <summary>
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is SetImeStateAction))
                return false;

            SetImeStateAction action = (SetImeStateAction)obj;

            if (!base.Equals(obj))
                return false;

            if (!this.InputMethodState.Equals(action.InputMethodState))
                return false;

            return true;
        }

        /// <summary>
        /// </summary>
        public System.Windows.Input.InputMethodState InputMethodState
        {
            get
            {
                return _inputMethodState;
            }
            set
            {
                _inputMethodState = value;
            }
        }

        private InputMethodState _inputMethodState = System.Windows.Input.InputMethodState.DoNotCare;
    }
    /// <summary>
    /// Test action handler for moving the mouse wheel.
    /// </summary>
    public class MoveWheelAction : SequenceAction
    {
        /// <summary>
        /// </summary>
        public MoveWheelAction()
            : base()
        {
        }

        /// <summary>
        /// </summary>
        public override void DoAction(DependencyObject root)
        {
            MouseHelper.MoveWheel(this.MouseWheelDirection, this.Delta);
        }

        /// <summary>
        /// </summary>
        public override string ToString()
        {
            return "Move mouse wheel " + MouseWheelDirection.ToString() + " " + Delta.ToString() + " notches";
        }

        /// <summary>
        /// </summary>
        public override int GetHashCode()
        {
            int hashCode = base.GetHashCode();

            hashCode ^= this.MouseWheelDirection.GetHashCode() ^ this.Delta.GetHashCode();

            return hashCode;
        }

        /// <summary>
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is MoveWheelAction))
                return false;

            MoveWheelAction action = (MoveWheelAction)obj;

            if (!base.Equals(obj))
                return false;

            if (!this.MouseWheelDirection.Equals(action.MouseWheelDirection))
                return false;

            if (!this.Delta.Equals(action.Delta))
                return false;

            return true;
        }

        /// <summary>
        /// </summary>
        public MouseWheelDirection MouseWheelDirection
        {
            get
            {
                return _mouseWheelDirection;
            }
            set
            {
                _mouseWheelDirection = value;
            }
        }

        /// <summary>
        /// </summary>
        public int Delta
        {
            get
            {
                return _delta;
            }
            set
            {
                _delta = value;
            }
        }

        private MouseWheelDirection _mouseWheelDirection = MouseWheelDirection.Forward;
        private int _delta = 1;
    }

    /// <summary>
    /// Test action handler for mouse clicks.
    /// </summary>
    public class MouseClickAction : MouseAction
    {
        /// <summary>
        /// </summary>
        public MouseClickAction()
            : base()
        {
        }

        /// <summary>
        /// </summary>
        public override void DoAction(DependencyObject root)
        {
            DependencyObject targetElement = this.GetTarget(root);
            MouseButton button = this.GetMouseButton();
            MouseLocation location = this.MouseLocation;

            if (targetElement is ContentElement)
            {
                targetElement = LogicalTreeHelper.GetParent(targetElement);
                MouseHelper.Click(button, (UIElement)targetElement, location);
            }
            else if (targetElement is UIElement)
            {
                MouseHelper.Click(button, (UIElement)targetElement, location);
            }
#if TARGET_NET3_5
            else if (targetElement is UIElement3D)
            {
                MouseHelper.Click(button, (UIElement3D)targetElement, location);
            }
#endif
            else
            {
                throw new InvalidOperationException("Cannot click on element of type '" + targetElement.GetType() + "'.  Click can be done only on UIElement, UIElement3D and ContentElement objects.");
            }

            CoreLogger.LogStatus("Checking MouseDevice.ActiveSource...");
            if (this.CheckActiveSource && PresentationHelper.FromMouse() != PresentationHelper.FromElement(targetElement))
            {
                throw new Microsoft.Test.TestValidationException("MouseDevice.ActiveSource does not equal the target element, which was just clicked.");
            }
        }

        /// <summary>
        /// </summary>
        protected MouseButton GetMouseButton()
        {
            return (MouseButton)Enum.Parse(typeof(MouseButton), this.MouseButton, true);
        }

        /// <summary>
        /// </summary>
        public override string ToString()
        {
            return "Mouse " + MouseButton.ToLower() + "-click at " + MouseLocation + " of " + base.ToString();
        }

        /// <summary>
        /// </summary>
        public override int GetHashCode()
        {
            int hashCode = base.GetHashCode();

            hashCode ^= this.MouseButton.GetHashCode() ^ this.CheckActiveSource.GetHashCode();

            return hashCode;
        }

        /// <summary>
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is MouseClickAction))
                return false;

            MouseClickAction action = (MouseClickAction)obj;

            if (!base.Equals(obj))
                return false;

            if (!this.MouseButton.Equals(action.MouseButton))
                return false;

            if (!this.CheckActiveSource.Equals(action.CheckActiveSource))
                return false;

            return true;
        }

        /// <summary>
        /// </summary>
        public string MouseButton
        {
            get
            {
                return _mouseButton;
            }
            set
            {
                _mouseButton = value;
            }
        }

        /// <summary>
        /// </summary>
        public bool CheckActiveSource
        {
            get
            {
                return _checkActiveSource;
            }
            set
            {
                _checkActiveSource = value;
            }
        }

        private string _mouseButton = "Left";
        private bool _checkActiveSource = true;
    }
    /// <summary>
    /// Test action handler for mouse moves.
    /// </summary>
    public class MouseDragAction : MouseMoveAction
    {
        /// <summary>
        /// </summary>
        public MouseDragAction()
            : base()
        {
        }

        /// <summary>
        /// </summary>
        public override string ToString()
        {
            return "Mouse " + this.MouseButton.ToString() + "-drag to " + this.MouseLocation + " of " + this.Target;
        }

        /// <summary>
        /// </summary>
        public override void DoAction(DependencyObject root)
        {
            DependencyObject targetElement = this.GetTarget(root);
            MouseLocation location = this.MouseLocation;
            MouseButton button = this.MouseButton;

            if (targetElement is ContentElement)
            {
                targetElement = LogicalTreeHelper.GetParent(targetElement);
                MouseHelper.Drag((UIElement)targetElement, location, button);
            }
            else if (targetElement is UIElement)
            {
                MouseHelper.Drag((UIElement)targetElement, location, button);
            }
#if TARGET_NET3_5
            else if (targetElement is UIElement3D)
            {
                MouseHelper.Drag((UIElement3D)targetElement, location, button);
            }
#endif
            else
            {
                throw new InvalidOperationException("Cannot drag mouse on element of type '" + targetElement.GetType().Name + "'.  Only UIElement, UIElement3D and ContentElement objects are supported.");
            }
        }

        /// <summary>
        /// </summary>
        public override int GetHashCode()
        {
            int hashCode = base.GetHashCode();

            hashCode ^= this.MouseButton.GetHashCode();

            return hashCode;
        }

        /// <summary>
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is MouseDragAction))
                return false;

            MouseDragAction action = (MouseDragAction)obj;

            if (!base.Equals(obj))
                return false;

            if (!this.MouseButton.Equals(action.MouseButton))
                return false;

            return true;
        }

        /// <summary>
        /// </summary>
        public MouseButton MouseButton
        {
            get
            {
                return _mouseButton;
            }
            set
            {
                _mouseButton = value;
            }
        }

        private MouseButton _mouseButton = MouseButton.Left;
    }
    /// <summary>
    /// Test action handler for mouse moves.
    /// </summary>
    public class MouseMoveAction : MouseAction
    {
        /// <summary>
        /// </summary>
        public MouseMoveAction()
            : base()
        {
        }

        /// <summary>
        /// </summary>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("Mouse move ");

            if(!IsSynchronous)
                builder.Append("async ");

            if(IsImmediate)
                builder.Append("immediately ");

            builder.Append("to ");
            builder.Append(this.MouseLocation);
            builder.Append(" of ");
            builder.Append(Target);

            return builder.ToString();
        }

        /// <summary>
        /// </summary>
        public override void DoAction(DependencyObject root)
        {
            DependencyObject targetElement = this.GetTarget(root);
            MouseLocation location = this.MouseLocation;
            bool isSynchronous = MouseHelper.IsSynchronous;

            try
            {
                MouseHelper.IsSynchronous = this.IsSynchronous;

                if (targetElement is ContentElement)
                {
                    targetElement = LogicalTreeHelper.GetParent(targetElement);
                    MouseHelper.Move((UIElement)targetElement, location, this.IsImmediate);
                }
                else if (targetElement is UIElement)
                {
                    MouseHelper.Move((UIElement)targetElement, location, this.IsImmediate);
                }
#if TARGET_NET3_5
                else if (targetElement is UIElement3D)
                {
                    MouseHelper.Move((UIElement3D)targetElement, location, this.IsImmediate);
                }
#endif
                else
                {
                    throw new InvalidOperationException("Cannot move to element of type '" + targetElement.GetType() + "'.  Can move only to UIElement, UIElement3D and ContentElement objects.");
                }

                if (this.IsSynchronous)
                {
                    CoreLogger.LogStatus("Checking MouseDevice.ActiveSource...");
                    if (PresentationHelper.FromMouse() != PresentationHelper.FromElement(targetElement))
                    {
                        throw new Microsoft.Test.TestValidationException("MouseDevice.ActiveSource does not equal the target element, to which the mouse just moved.");
                    }
                }
            }
            finally
            {
                MouseHelper.IsSynchronous = isSynchronous;
            }
        }

        /// <summary>
        /// </summary>
        public override int GetHashCode()
        {
            int hashCode = base.GetHashCode();

            hashCode ^= this.IsImmediate.GetHashCode();

            return hashCode;
        }

        /// <summary>
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is MouseMoveAction))
                return false;

            MouseMoveAction action = (MouseMoveAction)obj;

            if (!base.Equals(obj))
                return false;

            if (!this.IsImmediate.Equals(action.IsImmediate))
                return false;

            return true;
        }

        /// <summary>
        /// Directs whether or not the movement should be incremental across 
        /// many points between the start and end.
        /// </summary>
        public bool IsImmediate
        {
            get
            {
                return _isImmediate;
            }
            set
            {
                _isImmediate = value;
            }
        }

        private bool _isImmediate = false;
    }
    /// <summary>
    /// Captures or releases mouse capture.
    /// </summary>
    public class CaptureMouseAction : SequenceAction
    {
        /// <summary>
        /// </summary>
        public CaptureMouseAction()
            : base()
        {
        }

        /// <summary>
        /// </summary>
        public override void DoAction(DependencyObject root)
        {
            DependencyObject targetElement = this.GetTarget(root);

            if (this.ShouldRelease)
            {
                _DoReleaseCapture(targetElement);
            }
            else
            {
                _DoCapture(targetElement);
            }

            DispatcherHelper.DoEvents();
        }

        private void _DoCapture(DependencyObject targetElement)
        {
            if (targetElement is ContentElement)
            {
                ((ContentElement)targetElement).CaptureMouse();
            }
            else if (targetElement is UIElement)
            {
                ((UIElement)targetElement).CaptureMouse();
            }
#if TARGET_NET3_5
            else if (targetElement is UIElement3D)
            {
                ((UIElement3D)targetElement).CaptureMouse();
            }
#endif
            else
            {
                throw new InvalidOperationException("Cannot capture mouse on element of type '" + targetElement.GetType() + "'.  CaptureMouse can be called only on UIElement, UIElement3D and ContentElement objects.");
            }
        }

        private void _DoReleaseCapture(DependencyObject targetElement)
        {
            if (targetElement is ContentElement)
            {
                ((ContentElement)targetElement).ReleaseMouseCapture();
            }
            else if (targetElement is UIElement)
            {
                ((UIElement)targetElement).ReleaseMouseCapture();
            }
#if TARGET_NET3_5
            else if (targetElement is UIElement3D)
            {
                ((UIElement3D)targetElement).ReleaseMouseCapture();
            }
#endif
            else
            {
                throw new InvalidOperationException("Cannot release mouse capture on element of type '" + targetElement.GetType() + "'.  ReleaseMouseCapture can be called only on UIElement, UIElement3D and ContentElement objects.");
            }
        }

        /// <summary>
        /// </summary>
        public override string ToString()
        {
            return "Capture " + Target;
        }

        /// <summary>
        /// </summary>
        public override int GetHashCode()
        {
            int hashCode = base.GetHashCode();

            hashCode ^= this.Target.GetHashCode() ^ this.ShouldRelease.GetHashCode();

            return hashCode;
        }

        /// <summary>
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is CaptureMouseAction))
                return false;

            CaptureMouseAction action = (CaptureMouseAction)obj;

            if (!base.Equals(obj))
                return false;

            if (!this.Target.Equals(action.Target))
                return false;

            if (!this.ShouldRelease.Equals(action.ShouldRelease))
                return false;

            return true;
        }

        /// <summary>
        /// </summary>
        protected DependencyObject GetTarget(DependencyObject root)
        {
            return (DependencyObject)TreeHelper.FindNodeById(root, _target);
        }

        /// <summary>
        /// </summary>
        public string Target
        {
            get
            {
                return _target;
            }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("target", "Target '" + value + "' is invalid. The value must be a non-empty string.");
                }

                _target = value;
            }
        }

        /// <summary>
        /// </summary>
        public bool ShouldRelease
        {
            get
            {
                return _shouldRelease;
            }
            set
            {
                _shouldRelease = value;
            }
        }

        private string _target;
        private bool _shouldRelease = false;
    }
    /// <summary>
    /// Calls focus on an element.
    /// </summary>
    public class FocusAction : SequenceAction
    {
        /// <summary>
        /// </summary>
        public FocusAction()
            : base()
        {
        }

        /// <summary>
        /// </summary>
        public override void DoAction(DependencyObject root)
        {
            DependencyObject targetElement = this.GetTarget(root);

            if (targetElement is ContentElement)
            {
                ((ContentElement)targetElement).Focus();
            }
            else if (targetElement is UIElement)
            {
                ((UIElement)targetElement).Focus();
            }
#if TARGET_NET3_5
            else if (targetElement is UIElement3D)
            {
                ((UIElement3D)targetElement).Focus();
            }
#endif
            else
            {
                throw new InvalidOperationException("Cannot set focus to element of type '" + targetElement.GetType() + "'.  Focus can be set only on UIElement, UIElement3D and ContentElement objects.");
            }

            if(this.IsSynchronous)
                DispatcherHelper.DoEvents();
        }

        /// <summary>
        /// </summary>
        public override string ToString()
        {
            return "Focus " + Target;
        }

        /// <summary>
        /// </summary>
        public override int GetHashCode()
        {
            int hashCode = base.GetHashCode();

            hashCode ^= this.Target.GetHashCode();

            return hashCode;
        }

        /// <summary>
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is FocusAction))
                return false;

            FocusAction action = (FocusAction)obj;

            if (!base.Equals(obj))
                return false;

            if (!this.Target.Equals(action.Target))
                return false;

            return true;
        }

        /// <summary>
        /// </summary>
        protected DependencyObject GetTarget(DependencyObject root)
        {
            return (DependencyObject)TreeHelper.FindNodeById(root, _target);
        }

        /// <summary>
        /// 
        /// </summary>
        public string Target
        {
            get
            {
                return _target;
            }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("target", "Target '" + value + "' is invalid. The value must be a non-empty string.");
                }

                _target = value;
            }
        }

        private string _target;
    }
    /// <summary>
    /// Calls Mouse.Synchronize().
    /// </summary>
    public class MouseSynchronizeAction : SequenceAction
    {
        /// <summary>
        /// </summary>
        public MouseSynchronizeAction()
            : base()
        {
        }

        /// <summary>
        /// </summary>
        public override void DoAction(DependencyObject root)
        {
            Mouse.Synchronize();
        }

        /// <summary>
        /// </summary>
        public override string ToString()
        {
            return "Mouse.Synchronize()";
        }
    }
    /// <summary>
    /// Presses a key+modifiers combination.
    /// </summary>
    public class PressKeyAction : KeyboardAction
    {
        /// <summary>
        /// </summary>
        public PressKeyAction()
            : base()
        {
        }

        /// <summary>
        /// </summary>
        public override void DoAction(DependencyObject root)
        {
            bool isSynchronous = KeyboardHelper.IsSynchronous;

            try
            {
                KeyboardHelper.IsSynchronous = this.IsSynchronous;

                for (int i = 0; i < this.Keys.Count; i++)
                {
                    KeyboardHelper.PressKey(this.Keys[i]);
                }
            }
            finally
            {
                KeyboardHelper.IsSynchronous = isSynchronous;
            }
        }

        /// <summary>
        /// </summary>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is PressKeyAction))
                return false;

            if (!base.Equals(obj))
                return false;

            return true;
        }
    }
    /// <summary>
    /// Release a key+modifiers combination.
    /// </summary>
    public class ReleaseKeyAction : KeyboardAction
    {
        /// <summary>
        /// </summary>
        public ReleaseKeyAction()
            : base()
        {
        }

        /// <summary>
        /// </summary>
        public override void DoAction(DependencyObject root)
        {
            bool isSynchronous = KeyboardHelper.IsSynchronous;

            try
            {
                KeyboardHelper.IsSynchronous = this.IsSynchronous;

                // Release all keys in reverse order.
                for (int i = 0; i < this.Keys.Count; i++)
                {
                    KeyboardHelper.ReleaseKey(this.Keys[this.Keys.Count - i - 1]);
                }
            }
            finally
            {
                KeyboardHelper.IsSynchronous = isSynchronous;
            }
        }

        /// <summary>
        /// </summary>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is ReleaseKeyAction))
                return false;

            if (!base.Equals(obj))
                return false;

            return true;
        }
    }
    /// <summary>
    /// Presses and releases a key+modifiers combination.
    /// </summary>
    public class TypeKeyAction : KeyboardAction
    {
        /// <summary>
        /// </summary>
        public TypeKeyAction()
            : base()
        {
        }

        /// <summary>
        /// </summary>
        public override void DoAction(DependencyObject root)
        {
            bool isSynchronous = KeyboardHelper.IsSynchronous;

            try
            {
                KeyboardHelper.IsSynchronous = this.IsSynchronous;

                // Press all keys.
                for (int i = 0; i < this.Keys.Count; i++)
                {
                    KeyboardHelper.PressKey(this.Keys[i]);
                }

                // Release all keys in reverse order.
                for (int i = 0; i < this.Keys.Count; i++)
                {
                    KeyboardHelper.ReleaseKey(this.Keys[this.Keys.Count - i - 1]);
                }
            }
            finally
            {
                KeyboardHelper.IsSynchronous = isSynchronous;
            }
        }

        /// <summary>
        /// </summary>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is TypeKeyAction))
                return false;

            if (!base.Equals(obj))
                return false;

            return true;
        }
    }
    /// <summary>
    /// Base class for mouse input test action handlers.
    /// </summary>
    public abstract class KeyboardAction : SequenceAction
    {
        /// <summary>
        /// </summary>
        public KeyboardAction()
            : base()
        {
            _keys = new KeyList();
        }

        /// <summary>
        /// </summary>
        public override string ToString()
        {
            return "Type '" + this.Keys + "'";
        }

        /// <summary>
        /// </summary>
        public override int GetHashCode()
        {
            int hashCode = base.GetHashCode();

            hashCode ^= this.Keys.GetHashCode();

            return hashCode;
        }

        /// <summary>
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is KeyboardAction))
                return false;

            KeyboardAction action = (KeyboardAction)obj;

            if (!base.Equals(obj))
                return false;

            if (!this.Keys.Equals(action.Keys))
                return false;

            return true;
        }

        /// <summary>
        /// </summary>
        public KeyList Keys
        {
            get
            {
                return _keys;
            }
            set
            {
                _keys = value;
            }
        }

        KeyList _keys;
    }
    /// <summary>
    /// Base class for mouse input test action handlers.
    /// </summary>
    public abstract class MouseAction : TargetedSequenceAction
    {
        /// <summary>
        /// </summary>
        public MouseAction()
            : base()
        {
            _target = "";
        }

        /// <summary>
        /// </summary>
        public override string ToString()
        {
            return this.MouseLocation + " " + base.ToString();
        }

        /// <summary>
        /// </summary>
        public override int GetHashCode()
        {
            int hashCode = base.GetHashCode();

            hashCode ^= this.Target.GetHashCode() ^ this.MouseLocation.GetHashCode();

            return hashCode;
        }

        /// <summary>
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is MouseAction))
                return false;

            MouseAction action = (MouseAction)obj;

            if (!base.Equals(obj))
                return false;

            if (!this.MouseLocation.Equals(action.MouseLocation))
                return false;

            return true;
        }

        /// <summary>
        /// </summary>
        public MouseLocation MouseLocation
        {
            get
            {
                return _mouseLocation;
            }
            set
            {
                _mouseLocation = value;
            }
        }

        private MouseLocation _mouseLocation = MouseLocation.Center;
    }

    /// <summary>
    /// Abstract sequence action that takes a target.
    /// </summary>
    public abstract class TargetedSequenceAction : SequenceAction
    {
        /// <summary>
        /// </summary>
        public TargetedSequenceAction()
            : base()
        {
        }

        /// <summary>
        /// </summary>
        public override string ToString()
        {
            return "Target " + Target;
        }

        /// <summary>
        /// </summary>
        public override int GetHashCode()
        {
            int hashCode = base.GetHashCode();

            hashCode ^= this.Target.GetHashCode();

            return hashCode;
        }

        /// <summary>
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is TargetedSequenceAction))
                return false;

            TargetedSequenceAction action = (TargetedSequenceAction)obj;

            if (!base.Equals(obj))
                return false;

            if (!this.Target.Equals(action.Target))
                return false;

            return true;
        }

        /// <summary>
        /// </summary>
        protected DependencyObject GetTarget(DependencyObject root)
        {
            DependencyObject obj = (DependencyObject)TreeHelper.FindNodeById(root, _target);

            if (obj == null)
            {
                throw new Microsoft.Test.TestValidationException("Cannot find element with name '" + _target + "'.");
            }

            return obj;
        }

        /// <summary>
        /// </summary>
        public string Target
        {
            get
            {
                return _target;
            }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("Target", "Target '" + value + "' is invalid. The value must be a non-empty string.");
                }

                _target = value;
            }
        }

        /// <summary>
        /// </summary>
        protected string _target;
    }

    /// <summary>
    /// Set IsHitTestVisible property on an element.
    /// </summary>
    public class SetIsHitTestVisibleAction : TargetedSequenceAction
    {
        /// <summary>
        /// </summary>
        public SetIsHitTestVisibleAction()
            : base()
        {
        }

        /// <summary>
        /// Set ths IsHitTestVisible property on the targeted element under root.
        /// </summary>
        public override void DoAction(DependencyObject root)
        {
            DependencyObject targetElement = this.GetTarget(root);

            if (targetElement is UIElement)
            {
                UIElement targetUIElement = (targetElement as UIElement);

                targetUIElement.IsHitTestVisible = (String.Compare(_isHitTestVisibleProperty, "True", true) != 0)?true:false;
            }
#if TARGET_NET3_5
            else if (targetElement is UIElement3D)
            {
                UIElement3D targetUIElement3D = (targetElement as UIElement3D);

                targetUIElement3D.IsHitTestVisible = (String.Compare(_isHitTestVisibleProperty, "True", true) != 0) ? true : false;
            }
#endif
            else
            {
                throw new InvalidOperationException("Cannot set IsHitTestVisible property on element of type '" + targetElement.GetType() + "'.  Visibility properties can be set only on UIElement, UIElement3D and ContentElement objects.");
            }

            DispatcherHelper.DoEvents();
        }

        /// <summary>
        /// </summary>
        public override string ToString()
        {
            return  base.ToString() + " IsHitTestVisible " + _isHitTestVisibleProperty;
        }

        /// <summary>
        /// </summary>
        public override int GetHashCode()
        {
            int hashCode = base.GetHashCode();

            hashCode ^= this.IsHitTestVisibleProperty.GetHashCode();

            return hashCode;
        }

        /// <summary>
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is SetIsHitTestVisibleAction))
                return false;

            if (!base.Equals(obj))
                return false;

            SetIsHitTestVisibleAction action = (SetIsHitTestVisibleAction)obj;

            if (!this.IsHitTestVisibleProperty.Equals(action.IsHitTestVisibleProperty))
                return false;

            return true;
        }

        /// <summary>
        /// Boolean value to set the target element IsHitTestVisible property.
        /// </summary>
        public string IsHitTestVisibleProperty
        {
            get
            {
                return _isHitTestVisibleProperty;
            }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("IsHitTestVisibleProperty", "IsHitTestVisibleProperty '" + value + "' is invalid. The value must be a non-empty string.");
                }

                _isHitTestVisibleProperty = value;
            }
        }

        private string _isHitTestVisibleProperty;
    }

    /// <summary>
    /// Sets Opacity property on an element.
    /// </summary>
    public class SetOpacityAction : TargetedSequenceAction
    {
        /// <summary>
        /// </summary>
        public SetOpacityAction()
            : base()
        {
        }

        /// <summary>
        /// Set ths Opacity property on the targeted element under root.
        /// </summary>
        public override void DoAction(DependencyObject root)
        {
            DependencyObject targetElement = this.GetTarget(root);

            if (targetElement is UIElement)
            {
                System.Windows.UIElement targetUIElement = (targetElement as UIElement);

                targetUIElement.Opacity = Double.Parse(OpacityProperty);
            }
            else
            {
                throw new InvalidOperationException("Cannot set IsHitTestVisible property on element of type '" + targetElement.GetType() + "'.  Visibility properties can be set only on UiElement and ContentElement objects.");
            }

            DispatcherHelper.DoEvents();
        }

        /// <summary>
        /// </summary>
        public override string ToString()
        {
            return base.ToString() + " Opacity " + OpacityProperty;
        }

        /// <summary>
        /// </summary>
        public override int GetHashCode()
        {
            int hashCode = base.GetHashCode();

            hashCode ^= this.OpacityProperty.GetHashCode();

            return hashCode;
        }

        /// <summary>
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is SetOpacityAction))
                return false;

            if (!base.Equals(obj))
                return false;

            SetOpacityAction action = (SetOpacityAction)obj;

            if (!this.OpacityProperty.Equals(action.OpacityProperty))
                return false;

            return true;
        }

        /// <summary>
        /// Boolean value to set the target element IsHitTestVisible property.
        /// </summary>
        public string OpacityProperty
        {
            get
            {
                return _opacityProperty;
            }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("OpacityProperty", "OpacityProperty '" + value + "' is invalid. The value must be a non-empty string.");
                }

                _opacityProperty = value;
            }
        }

        private string _opacityProperty;
    }

    /// <summary>
    /// Waits for Dispatcher processing until some priority.
    /// </summary>
    public class DoEventsAction : SequenceAction
    {
        /// <summary>
        /// </summary>
        public DoEventsAction()
            : base()
        {
        }

        /// <summary>
        /// </summary>
        public override void DoAction(DependencyObject root)
        {
            DispatcherHelper.DoEvents(this.MinWait, this.DispatcherPriority);
        }

        /// <summary>
        /// </summary>
        public override string ToString()
        {
            return "DoEvents to " + Enum.GetName(typeof(DispatcherPriority), this.DispatcherPriority) + " priority, waiting at least " + this.MinWait + " ms";
        }

        /// <summary>
        /// </summary>
        public override int GetHashCode()
        {
            int hashCode = base.GetHashCode();

            hashCode ^= this.DispatcherPriority.GetHashCode() ^ this.MinWait.GetHashCode();

            return hashCode;
        }

        /// <summary>
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is DoEventsAction))
                return false;

            DoEventsAction action = (DoEventsAction)obj;

            if (!base.Equals(obj))
                return false;

            if (!this.DispatcherPriority.Equals(action.DispatcherPriority))
                return false;

            if (!this.MinWait.Equals(action.MinWait))
                return false;

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public DispatcherPriority DispatcherPriority
        {
            get
            {
                return _dispatcherPriority;
            }
            set
            {
                _dispatcherPriority = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int MinWait
        {
            get
            {
                return _minWait;
            }
            set
            {
                _minWait = value;
            }
        }

        private DispatcherPriority _dispatcherPriority = DispatcherPriority.SystemIdle;
        private int _minWait = 0;
    }

    /// <summary>
    /// Sets Visibility property on an element.
    /// </summary>
    public class SetVisibilityAction : TargetedSequenceAction
    {
        /// <summary>
        /// </summary>
        public SetVisibilityAction() : base()
        {
        }

        /// <summary>
        /// Set ths IsHitTestVisible property on the targeted element under root.
        /// </summary>
        public override void DoAction(DependencyObject root)
        {
            DependencyObject targetElement = this.GetTarget(root);

            if (targetElement is UIElement)
            {
                UIElement targetUIElement = (targetElement as UIElement);

                targetUIElement.Visibility = (Visibility)Enum.Parse(typeof(Visibility), _visibilityProperty, true /* Ignore case. */);
            }
#if TARGET_NET3_5
            else if (targetElement is UIElement3D)
            {
                UIElement3D targetUIElement3D = (targetElement as UIElement3D);

                targetUIElement3D.Visibility = (Visibility)Enum.Parse(typeof(Visibility), _visibilityProperty, true /* Ignore case. */);
            }
#endif
            else
            {
                throw new InvalidOperationException("Cannot set Visibility property on element of type '" + targetElement.GetType() + "'.  Visibility properties can be set only on UIElement, UIElement3D and ContentElement objects.");
            }

            if (IsSynchronous)
            {
                DispatcherHelper.DoEvents();
            }
        }

        /// <summary>
        /// </summary>
        public override string ToString()
        {
            return base.ToString() + " Visibility " + _visibilityProperty + " IsSynchronous:" + this.IsSynchronous;
        }

        /// <summary>
        /// </summary>
        public override int GetHashCode()
        {
            int hashCode = base.GetHashCode();

            hashCode ^= this.VisibilityProperty.GetHashCode() ^ this.IsSynchronous.GetHashCode();

            return hashCode;
        }

        /// <summary>
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is SetVisibilityAction))
                return false;

            if (!base.Equals(obj))
                return false;

            SetVisibilityAction action = (SetVisibilityAction)obj;

            if (!this.VisibilityProperty.Equals(action.VisibilityProperty))
                return false;

            if (!this.IsSynchronous.Equals(action.IsSynchronous))
                return false;

            return true;
        }

        /// <summary>
        /// Value to set the target element's Visibility property.
        /// </summary>
        public string VisibilityProperty
        {
            get
            {
                return _visibilityProperty;
            }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("VisibilityProperty", "VisibilityProperty '" + value + "' is invalid. The value must be a non-empty string.");
                }

                _visibilityProperty = value;
            }
        }

        private string _visibilityProperty;
    }

    /// <summary>
    /// Test action handler for changing an item's position in a Canvas.
    /// </summary>
    public class ChangeCanvasPositionAction : TargetedSequenceAction
    {
        /// <summary>
        /// </summary>
        public ChangeCanvasPositionAction()
            : base()
        {
        }

        /// <summary>
        /// Change the canvas position of the targeted element under root.
        /// </summary>
        public override void DoAction(DependencyObject root)
        {
            DependencyObject targetElement = this.GetTarget(root);

            if (targetElement is FrameworkElement)
            {
                FrameworkElement targetFrameworkElement = (targetElement as FrameworkElement);

                if (_animated)
                {
                    DoubleAnimation canvasAnimation = new DoubleAnimation();
                    canvasAnimation.To = _top;
                    canvasAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));

                    targetFrameworkElement.BeginAnimation(Canvas.TopProperty, canvasAnimation);

                    canvasAnimation = new DoubleAnimation();
                    canvasAnimation.To = _left;
                    canvasAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));

                    targetFrameworkElement.BeginAnimation(Canvas.LeftProperty, canvasAnimation);
                   
                    DispatcherHelper.DoEvents(1000);
                }
                else
                {
                    // Move element down so that the mouse can no longer be over it.
                    Canvas.SetTop(targetFrameworkElement, _top);
                    Canvas.SetLeft(targetFrameworkElement, _left);
                }
            }
            else
            {
                throw new InvalidOperationException("Cannot change Canvas position on element of type '" + targetElement.GetType() + "'. Can only change canvas position of FrameworkElement objects.");
            }

            DispatcherHelper.DoEvents(200);
        }

        /// <summary>
        /// </summary>
        public override string ToString()
        {
            return  base.ToString() + " change canvas position " + _left.ToString() + "," + _top.ToString() +
                " animated " + _animated.ToString();
        }

        /// <summary>
        /// </summary>
        public override int GetHashCode()
        {
            int hashCode = base.GetHashCode();

            hashCode ^= this._animated.GetHashCode() ^ this._left.GetHashCode() ^ this._top.GetHashCode();

            return hashCode;
        }

        /// <summary>
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is ChangeCanvasPositionAction))
                return false;

            if (!base.Equals(obj))
                return false;

            ChangeCanvasPositionAction action = (ChangeCanvasPositionAction)obj;

            if (this._animated != action._animated)
                return false;

            if (this._top != action._top)
                return false;

            if (this._left != action._left)
                return false;

            return true;
        }

        /// <summary>
        /// Value to set the target element's Visibility property.
        /// </summary>
        public string Animated
        {
            get
            {
                return _animated.ToString();
            }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("Animated", "Animated '" + value + "' is invalid. The value must be a non-empty string.");
                }

                _animated = String.Equals(value.ToLower(), "true");               
            }
        }

        /// <summary>
        /// Value to set the target element's Canvas.Left attached property.
        /// </summary>
        public string Left
        {
            get
            {
                return _left.ToString();
            }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("Left", "Left '" + value + "' is invalid. The value must be a non-empty string.");
                }

                _left = double.Parse(value);
            }
        }



        /// <summary>
        /// Value to set the target element's Canvas.Top attached property.
        /// </summary>
        public string Top
        {
            get
            {
                return _top.ToString();
            }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("Top", "Top '" + value + "' is invalid. The value must be a non-empty string.");
                }

                _top = double.Parse(value);
            }
        }

        private bool _animated = false;
        private double _top;
        private double _left;
    }

    /// <summary>
    /// Test action handler for changing an item's size.
    /// </summary>
    public class ChangeSizeAction : TargetedSequenceAction
    {
        /// <summary>
        /// </summary>
        public ChangeSizeAction()
            : base()
        {
        }

        /// <summary>
        /// Change the canvas position of the targeted element under root.
        /// </summary>
        public override void DoAction(DependencyObject root)
        {
            DependencyObject targetElement = this.GetTarget(root);

            if (targetElement is FrameworkElement)
            {
                FrameworkElement targetFrameworkElement = (targetElement as FrameworkElement);

                if (_animated)
                {
                    DoubleAnimation sizeAnimation = new DoubleAnimation();
                    sizeAnimation.To = _height;
                    sizeAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));

                    targetFrameworkElement.BeginAnimation(FrameworkElement.HeightProperty, sizeAnimation);

                    sizeAnimation = new DoubleAnimation();
                    sizeAnimation.To = _width;
                    sizeAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));

                    targetFrameworkElement.BeginAnimation(FrameworkElement.WidthProperty, sizeAnimation);

                    DispatcherHelper.DoEvents(1000);
                }
                else
                {
                    targetFrameworkElement.Height = _width;
                    targetFrameworkElement.Width = _height;
                }
            }
            else
            {
                throw new InvalidOperationException("Cannot change size on element of type '" + targetElement.GetType() + "'. Can only change size of FrameworkElement objects.");
            }

            DispatcherHelper.DoEvents();
        }

        /// <summary>
        /// </summary>
        public override string ToString()
        {
            return base.ToString() + _animated.ToString() + " change size to " + _width + ", " + _height;
        }

        /// <summary>
        /// </summary>
        public override int GetHashCode()
        {
            int hashCode = base.GetHashCode();

            hashCode ^= this._animated.GetHashCode() ^ this._width.GetHashCode() ^ this._height.GetHashCode();

            return hashCode;
        }

        /// <summary>
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is ChangeSizeAction))
                return false;

            if (!base.Equals(obj))
                return false;

            ChangeSizeAction action = (ChangeSizeAction)obj;

            if (this._animated != action._animated)
                return false;

            return true;
        }

        /// <summary>
        /// Value to set the target element's Visibility property.
        /// </summary>
        public string Animated
        {
            get
            {
                return _animated.ToString();
            }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("Animated", "Animated '" + value + "' is invalid. The value must be a non-empty string.");
                }

                _animated = String.Equals(value.ToLower(), "true");
            }
        }

        /// <summary>
        /// Value to set the target element's Width.
        /// </summary>
        public string Width
        {
            get
            {
                return _width.ToString();
            }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("Width", "Width '" + value + "' is invalid. The value must be a non-empty string.");
                }

                _width = Double.Parse(value);
            }
        }



        /// <summary>
        /// Value to set the target element's Height.
        /// </summary>
        public string Height
        {
            get
            {
                return _height.ToString();
            }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("Height", "Height '" + value + "' is invalid. The value must be a non-empty string.");
                }

                _height = Double.Parse(value);
            }
        }


        private bool _animated;   // true/false
        private double _width;
        private double _height;
    }

    
    /// <summary>
    /// Test action handler to add an item to a Panel.
    /// </summary>
    public class AddElementAction: TargetedSequenceAction
    {
        /// <summary>
        /// </summary>
        public AddElementAction()
            : base()
        {
        }

        /// <summary>
        /// Change the canvas position of the targeted element under root.
        /// </summary>
        public override void DoAction(DependencyObject root)
        {
            DependencyObject targetElement = this.GetTarget(root);

            if (targetElement is Panel)
            {
                Panel targetPanel = (targetElement as Panel);

                Button b = new Button();
                b.Height = 50;

                if (!String.IsNullOrEmpty(_name))
                {
                    b.Name = _name;
                }

                targetPanel.Children.Insert(0, b);
            }
            else
            {
                throw new InvalidOperationException("Cannot add element to element of type '" + targetElement.GetType() + "'. Can only add elements to Panel objects.");
            }

            DispatcherHelper.DoEvents(500);
        }

        /// <summary>
        /// </summary>
        public override string ToString()
        {
            return base.ToString() + " add element";
        }

        /// <summary>
        /// Optional Name property value to assign to added element.
        /// </summary>
        public string Name
        {
          get
          {
            return _name;
          }
          set
          {
            // Can be empty or null.

            _name = value;
          }
        }

        private string _name = null;

    }
    
    /// <summary>
    /// Test action handler to remove the first child from a panel.
    /// </summary>
    public class RemoveElementAction: TargetedSequenceAction
    {
        /// <summary>
        /// </summary>
        public RemoveElementAction()
            : base()
        {
        }

        /// <summary>
        /// Change the canvas position of the targeted element under root.
        /// </summary>
        public override void DoAction(DependencyObject root)
        {
            DependencyObject targetElement = this.GetTarget(root);

            if (targetElement is Panel)
            {
                Panel targetPanel = (targetElement as Panel);

                targetPanel.Children.RemoveAt(0);
            }
            else
            {
                throw new InvalidOperationException("Cannot remove element from element of type '" + targetElement.GetType() + "'. Can only remove elements from Panel objects.");
            }

            DispatcherHelper.DoEvents();
        }

        /// <summary>
        /// </summary>
        public override string ToString()
        {
            return base.ToString() + " remove first born.";
        }
    }

    /// <summary>
    /// Test action handler to move the top level window.
    /// </summary>
    public class MoveWindowAction : SequenceAction
    {
        /// <summary>
        /// </summary>
        public MoveWindowAction()
            : base()
        {
        }

        /// <summary>
        /// Change the position of the top level window containing root.
        /// </summary>
        public override void DoAction(DependencyObject root)
        {
            Visual rootVisual = root as Visual;
            if (rootVisual == null)
            {
                throw new InvalidOperationException("Cannot move window parent of type '" + root.GetType() + "'. Can only move parent windows of Visual objects.");
            }
            
            // Get HwndSource parent of visual.
            Surface surface = new SurfaceCore(Avalon.Test.CoreUI.Trusted.Interop.PresentationSourceFromVisual(rootVisual));
            
            IntPtr handle = surface.Handle;

            // Find top level window handle.
            while (NativeMethods.GetParent(new HandleRef(this, handle)) != IntPtr.Zero)
            {
                handle = NativeMethods.GetParent(new HandleRef(this, handle));
            }

            NativeMethods.SetWindowPos(handle, IntPtr.Zero, _left, _top, 0, 0, NativeConstants.SWP_NOSIZE);

            // Wait for system idle after move.
            DispatcherHelper.DoEvents();
        }

        /// <summary>
        /// </summary>
        public override string ToString()
        {
            return base.ToString() + " move to " + _left + ", " + _top;
        }

        /// <summary>
        /// </summary>
        public override int GetHashCode()
        {
            int hashCode = base.GetHashCode();

            hashCode ^= this._left.GetHashCode() ^ this._top.GetHashCode();

            return hashCode;
        }

        /// <summary>
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is MoveWindowAction))
                return false;

            if (!base.Equals(obj))
                return false;

            MoveWindowAction action = (MoveWindowAction)obj;

            if (this._top != action._top)
                return false;

            if (this._left != action._left)
                return false;

            return true;
        }

        /// <summary>
        /// Value to set the target element's Width.
        /// </summary>
        public string Left
        {
            get
            {
                return _left.ToString();
            }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("Left", "Left '" + value + "' is invalid. The value must be a non-empty string.");
                }

                _left = int.Parse(value);
            }
        }



        /// <summary>
        /// Value to set the target element's Height.
        /// </summary>
        public string Top
        {
            get
            {
                return _top.ToString();
            }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("Top", "Top '" + value + "' is invalid. The value must be a non-empty string.");
                }

                _top = int.Parse(value);
            }
        }

        private int _top;
        private int _left;
    }

    /// <summary>
    /// Test action handler to apply a render transform to an element.
    /// </summary>
    public class RenderTransformAction : TargetedSequenceAction
    {
        /// <summary>
        /// </summary>
        public RenderTransformAction()
            : base()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public override void DoAction(DependencyObject root)
        {
            DependencyObject targetElement = this.GetTarget(root);

            if (targetElement is UIElement)
            {
                UIElement targetUIElement = targetElement as UIElement;

                Transform t = new TranslateTransform(_tx, _ty);
                targetUIElement.RenderTransform = t;                
            }
            else
            {
                throw new InvalidOperationException("Cannot apply render transform to element of type '" + targetElement.GetType() + "'. Can only apply render transforms to UIElements.");
            }
            DispatcherHelper.DoEvents(500);
        }

        /// <summary>
        /// </summary>
        public override string ToString()
        {
            return base.ToString() + " render transfrom " + _tx + ", " + _ty;
        }

        /// <summary>
        /// </summary>
        public override int GetHashCode()
        {
            int hashCode = base.GetHashCode();

            hashCode ^= this._tx.GetHashCode() ^ this._ty.GetHashCode();

            return hashCode;
        }

        /// <summary>
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is RenderTransformAction))
                return false;

            if (!base.Equals(obj))
                return false;

            RenderTransformAction action = (RenderTransformAction)obj;

            if (this._tx != action._tx)
                return false;

            if (this._ty != action._ty)
                return false;

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public string TranslateY
        {
            get
            {
                return _ty.ToString();
            }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("TranslateX", "TranslateX '" + value + "' is invalid. The value must be a non-empty string.");
                }

                _ty = Double.Parse(value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string TranslateX
        {
            get
            {
                return _tx.ToString();
            }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("TranslateX", "TranslateX '" + value + "' is invalid. The value must be a non-empty string.");
                }

                _tx = Double.Parse(value);
            }
        }

        private double _tx;
        private double _ty;
    }

}

