// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Diagnostics;
using System.Collections.ObjectModel; 

namespace Microsoft.Test.Input.MultiTouch
{
    public class TouchData
    {
        public List<TouchDevice> snapshots;
        public int currentSnapshot;
        public int touchId;
        public IInputElement capturedBy;
        public CaptureMode captureMode;
    }

    public delegate object GetPropertyValue(DependencyObject obj);
    public delegate object MergePropertyValues(object oldValue, object newValue);
    public delegate void AfterFrameSimulated(int frameNumber, Dictionary<int, TouchData> touchMap);

    /// <summary>
    /// This is the verifier for Touch features 
    /// 
    /// Factors:
    ///     1. Capture Mode - Element, SubTree, None
    ///     2. Capture Location - Root Window, Element, Child, None
    ///     3. Verify Location - Root Window, Parent Chain, Element
    ///     
    /// Verify 
    ///     1. 4 DPs:
    ///         {UIElement/ContentElement/UIElement3D}.AreAnyTouchesCaptured
    ///         {UIElement/ContentElement/UIElement3D}.AreAnyTouchesCapturedWithin
    ///         {UIElement/ContentElement/UIElement3D}.AreAnyTouchesDirectlyOver
    ///         {UIElement/ContentElement/UIElement3D}.AreAnyTouchesOver 
    ///     2. Collection count:
    ///     	{UIElement, UIElement3D, ContentElement}.TouchesCaptured - IEnumerable of TouchDevice
    ///         {UIElement, UIElement3D, ContentElement }.TouchesCapturedWithin - IEnumerable of TouchDevice
    ///     3. Event routing and handledness correctness:
    ///         a. route to elements with capture and future captures to there
    ///         b. not route to elements without capture
    ///         c. The listeners are not called at element locations that follow the location where an event 
    ///            is handled; also the other listeners on the same element location are still called
    ///         d. The “handledEventsToo” listeners are called at element locations that follow the location where an event is handled
    /// 
    /// Other factors:
    ///     1. Events - 
    ///             Preview Down/Move/Up - Tunnel
    ///             Down/Move/Up - bubble
    ///             Got/LostCapture - bubble
    ///             Enter/Leave - direct
    ///     2. Properties - IsHitTestVisible, IsEnabled, Visibility
    ///     3. Visual tree changes - add / remove elements
    ///         
    /// </summary>
    public class TouchVerifier : MultiTouchVerifier
    {
        #region Fields

        public CaptureMode[] captureModes = new CaptureMode[] { CaptureMode.None, CaptureMode.SubTree, CaptureMode.Element };

        public enum VerifyLocation // Verify Location - Root Window, Parent Chain, Element
        { 
            Root = 0,
            Parent = 1, 
            Element = 3,
        }

        private static readonly List<TouchDevice> s_emptyReadOnlyTouchCollection = new List<TouchDevice>();

        #endregion

        #region Constructor

        public TouchVerifier(UIElement element)
            : base()
        {

        }

        #endregion

        #region Helpers

        /// <summary>
        /// Go through all elements and verify that 
        ///     1. AreAnyTouchesOver=false
        ///     2. AreAnyTouchesDirectlyOver=false
        ///     3. AreAnyTouchesCaptured=false 
        ///     4. AreAnyTouchesCapturedWithin=false
        ///     5. TouchesCaptured is empty
        ///     6. TouchesCapturedWithin is empty
        /// </summary>
        public void CheckNoTouchesOverOrCapture(DependencyObject dpObj)
        {
            foreach (DependencyObject obj in MultiTouchVerifier.EnumerateTree(dpObj,
                        false/*include parents*/, true/*include children*/, 
                        true/*visual tree*/, true/*logical tree*/))
            {
                object locObj = null; 

                // 1. AreAnyTouchesOver
                locObj = GetElementValue(obj, UIElement.AreAnyTouchesOverProperty, 
                    ContentElement.AreAnyTouchesOverProperty, UIElement3D.AreAnyTouchesOverProperty);

                bool value = obj is IInputElement ? (bool)locObj : false;
                Utils.Assert(value == false, string.Format("Invalid UIElement.AreAnyTouchesOver - expected: {0}, actual: {1}", false, value));

                // 2. AreAnyTouchesDirectlyOver
                locObj = GetElementValue(obj, UIElement.AreAnyTouchesDirectlyOverProperty,
                    ContentElement.AreAnyTouchesDirectlyOverProperty, UIElement3D.AreAnyTouchesDirectlyOverProperty);

                value = obj is IInputElement ? (bool)locObj : false;
                Utils.Assert(value == false, "Invalid UIElement.AreAnyTouchesDirectlyOver");

                // 3. AreAnyTouchesCaptured
                locObj = GetElementValue(obj, UIElement.AreAnyTouchesCapturedProperty,
                    ContentElement.AreAnyTouchesCapturedProperty, UIElement3D.AreAnyTouchesCapturedProperty);

                value = obj is IInputElement ? (bool)locObj : false;
                Utils.Assert(value == false, "Invalid UIElement.AreAnyTouchesCaptured");

                // 4. AreAnyTouchesCapturedWithin 
                value = obj is IInputElement ? (bool)locObj : false;
                Utils.Assert(value == false, "Invalid UIElement.AreAnyTouchesCapturedWithin");

                // 5. TouchesCaptured
                IEnumerable<TouchDevice> col;
                if (obj is IInputElement)
                {
                    UIElement el = obj as UIElement; 
                    if (el != null)
                    {
                        col = el.TouchesCaptured;
                    }
                    else
                    {
                        ContentElement ce = obj as ContentElement;
                        if (ce != null)
                        {
                            col = ce.TouchesCaptured; 
                        }
                        else
                        {
                            UIElement3D el3D = obj as UIElement3D;
                            if (el3D != null)
                            {
                                col = el3D.TouchesCaptured;
                            }
                            else
                            {
                                col = s_emptyReadOnlyTouchCollection;
                            }
                        }
                    }

                    Utils.Assert(col != null, "Invalid UIElement.TouchesCaptured");
                    Utils.Assert(0 == col.Count(), "Invalid Count for UIElement.TouchesCaptured");
                }                

                // 6. TouchesCapturedWithin
                if (obj is IInputElement)
                {
                    UIElement el = obj as UIElement;
                    if (el != null)
                    {
                        col = el.TouchesCapturedWithin;
                    }
                    else
                    {
                        ContentElement ce = obj as ContentElement;
                        if (ce != null)
                        {
                            col = ce.TouchesCapturedWithin;
                        }
                        else
                        {
                            UIElement3D el3D = obj as UIElement3D;
                            if (el3D != null)
                            {
                                col = el3D.TouchesCapturedWithin;
                            }
                            else
                            {
                                col = s_emptyReadOnlyTouchCollection;
                            }
                        }
                    }
                    Utils.Assert(col != null, "Invalid UIElement.TouchesCapturedWithin");
                    Utils.Assert(0 == col.Count(), "Invalid Count for UIElement.TouchesCapturedWithin");
                }     
            }
        }

        /// <summary>
        /// Get an element's property value
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="expectedUIElementProperty"></param>
        /// <param name="expectedContentElementProperty"></param>
        /// <param name="expectedUIElement3DProperty"></param>
        /// <returns></returns>
        public static object GetElementValue(DependencyObject obj,
            DependencyProperty expectedUIElementProperty,
            DependencyProperty expectedContentElementProperty,
            DependencyProperty expectedUIElement3DProperty)
        {
            // UIElement
            UIElement uiElement = obj as UIElement;
            if (uiElement != null)
            {
                return uiElement.GetValue(expectedUIElementProperty);
            }
            else  // ContentElement
            {
                ContentElement contentElement = obj as ContentElement;
                if (contentElement != null)
                {
                    return contentElement.GetValue(expectedContentElementProperty);
                }
                else
                {
                    UIElement3D uiElement3D = obj as UIElement3D;
                    if (uiElement3D != null)
                    {
                        return uiElement3D.GetValue(expectedUIElement3DProperty);
                    }
                    // otherwise use default value
                    else
                    {
                        return expectedUIElementProperty.DefaultMetadata.DefaultValue;
                    }
                }
            }
        }

        #endregion

    }
}
