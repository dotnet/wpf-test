// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description: Provides abstraction for Tabbing through annotations.

using System;
using System.Collections;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Media;
using Annotations.Test.Framework;
using System.Windows.Input;
using System.Windows.Controls;

namespace Avalon.Test.Annotations
{
    public class TabHelper
    {
        #region Public Methods

        /// <summary>
        /// Tab until we see the same Element twice or until we reach a StickyNoteControl.
        /// Depending upon the value of Direction property this will move forward or backward
        /// through the tab order.
        /// </summary>
        public void TabToAnnotationGroup()
        {
            _currentlyVisibleNoteCache = new VisualTreeWalker<StickyNoteControl>().FindChildren(Root);

            int tabLimit = 50;
            int tabCount = 0;
            IInputElement currentElement = CurrentlyFocusedElement;
            // While we haven't seen this element before and it isn't a StickyNoteControl keep tabbing.
            while (!IsStickyNoteControl(currentElement) && tabCount++ < tabLimit)
            {
                if (Direction == TabDirection.Forward)
                    CtrlTab();
                else
                    CtrlShiftTab();
                currentElement = CurrentlyFocusedElement;
            }

            TestSuite.Current.printStatus("Tabbed through " + tabCount + " unique elements searching for Annotation.");
        }

        public void Tab() { Tab(1); }
        public void Tab(int times)
        {
            for (int i = 0; i < times; i++)
                UIAutomationModule.PressKey(Key.Tab);
        }

        public void CtrlTab() { CtrlTab(1); }
        public void CtrlTab(int times)
        {
            for (int i = 0; i < times; i++)
                UIAutomationModule.Ctrl(Key.Tab);
        }

        public void CtrlShiftTab() { CtrlShiftTab(1); }
        public void CtrlShiftTab(int times)
        {
            for (int i = 0; i < times; i++)
                UIAutomationModule.CtrlShift(Key.Tab);
        }

        public void MoveToNextAnnotation()
        {
            if (Direction == TabDirection.Forward)
                CtrlTab();
            else
                CtrlShiftTab();
        }

        public void MoveToPreviousAnnotation()
        {
            if (Direction == TabDirection.Backward)
                CtrlTab();
            else
                CtrlShiftTab();
        }

        /// <summary>
        /// Returns the currently focused element or null if no element in window is focused.
        /// </summary>
        public IInputElement CurrentlyFocusedElement
        {
            get
            {
                return Keyboard.FocusedElement;
            }
        }

        /// <summary>
        /// Returns a StickyNoteWrapper reperesenting the currently focused StickyNote or null
        /// if currently focused element is not a StickyNote.
        /// </summary>
        public StickyNoteWrapper CurrentlyFocusedStickyNote
        {
            get
            {
                IInputElement element = CurrentlyFocusedElement;
                StickyNoteWrapper wrapper = null;
                if (element is StickyNoteControl)
                    wrapper = new StickyNoteWrapper(element as StickyNoteControl, "note");
                else
                {
                    StickyNoteControl note = new VisualTreeWalker<StickyNoteControl>().FindParent(element as Visual);
                    if (note != null)
                        wrapper = new StickyNoteWrapper(note, "note");
                }
                return wrapper;
            }
        }

        public Window Root
        {
            get
            {
                return _root;
            }
            set
            {
                _root = value;
            }
        }

        public TabDirection Direction
        {
            get
            {
                return _direction;
            }
            set
            {
                _direction = value;
            }
        }

        public bool IsCurrentElementAStickyNote
        {
            get
            {
                return IsStickyNoteControl(CurrentlyFocusedElement);
            }
        }
        public void LogFocusedElement()
        {
            TestSuite.Current.printStatus("Focused Element -> " + CurrentlyFocusedElement);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Check to see if the given Element is or is within a StickyNoteControl.
        /// </summary>
        /// <param name="element">Element to test.</param>
        /// <returns>True if this element is a StickyNoteControl or is a child of one.</returns>
        private bool IsStickyNoteControl(IInputElement element)
        {
            if (Type.Equals(element.GetType(), typeof(StickyNoteControl)))
                return true;
            if (element is Visual)
                return new VisualTreeWalker<StickyNoteControl>().FindParent(element as Visual) != null;
            return false;
        }

        #endregion

        #region Fields

        private Window _root;
        private TabDirection _direction = TabDirection.Forward;
        private IList<StickyNoteControl> _currentlyVisibleNoteCache;

        #endregion

        public enum TabDirection
        {
            Forward,
            Backward
        }
    }
}	

