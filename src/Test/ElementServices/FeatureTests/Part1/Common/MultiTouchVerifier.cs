// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows.Input;
using System.Runtime.InteropServices;
using System.Windows;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Media;
using System.Diagnostics;
using System.Windows.Controls; 

namespace Microsoft.Test.Input.MultiTouch
{
    /// <summary>
    /// Base class for MultiTouch testing verifiers
    /// </summary>
    public abstract class MultiTouchVerifier
    {
        #region Fields

        private MultiTouchTestModes _mtTestMode; 
        private TabletDevice _inputDevice;
        private static MultiTouchVerifier s_verifier;

        const int VistaMajorVersion = 6;

        #endregion
        
        #region Constructor

        public MultiTouchVerifier()
        {
            _mtTestMode = MultiTouchTestModes.Manipulations; 
            s_verifier = this;
        }

        #endregion 

        #region Public and Protected Properties

        public MultiTouchTestModes MultiTouchMode
        {
            get 
            { 
                return _mtTestMode; 
            }
            set 
            { 
                _mtTestMode = value; 
            }
        }

        public TabletDevice InputDevice
        {
            get
            {
                return _inputDevice;
            }
            set
            {
                _inputDevice = value;
            }
        }

        public static bool IsMultiTouchDigitizer()
        {
            int value = MultiTouchNativeMethods.GetSystemMetrics(MultiTouchNativeMethods.SM_DIGITIZER);
            return ((value & 0x40) == 0);
        }

        /// <summary>
        /// for the time being only Win7 and Win2k8 RC have MT support, so check only these OSs for now
        /// </summary>
        /// <returns></returns>
        public static bool IsSupportedOS()
        {
            return (Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor == 1); 
        }

        public static bool IsTabletDeviceAvailable()
        {
            uint deviceCount = 0;
            // Determine the # of devices (result will be -1 if fails and cDevices will have count)
            int result = (int)MultiTouchNativeMethods.GetRawInputDeviceList(null, ref deviceCount, (uint)Marshal.SizeOf(typeof(MultiTouchNativeMethods.RAWINPUTDEVICELIST)));

            if (result >= 0 && deviceCount != 0)
            {
                MultiTouchNativeMethods.RAWINPUTDEVICELIST[] ridl = new MultiTouchNativeMethods.RAWINPUTDEVICELIST[deviceCount];
                int count = (int)MultiTouchNativeMethods.GetRawInputDeviceList(ridl, ref deviceCount, (uint)Marshal.SizeOf(typeof(MultiTouchNativeMethods.RAWINPUTDEVICELIST)));

                if (count > 0)
                {
                    for (int i = 0; i < count; i++)
                    {
                        if (ridl[i].dwType == MultiTouchNativeMethods.RIM_TYPEHID)
                        {
                            MultiTouchNativeMethods.RID_DEVICE_INFO deviceInfo = new MultiTouchNativeMethods.RID_DEVICE_INFO();
                            deviceInfo.cbSize = (uint)Marshal.SizeOf(typeof(MultiTouchNativeMethods.RID_DEVICE_INFO));
                            uint cbSize = (uint)deviceInfo.cbSize;
                            int cBytes = (int)MultiTouchNativeMethods.GetRawInputDeviceInfo(ridl[i].hDevice, MultiTouchNativeMethods.RIDI_DEVICEINFO, ref deviceInfo, ref cbSize);

                            if (cBytes > 0)
                            {
                                if (deviceInfo.hid.usUsagePage == MultiTouchNativeMethods.HID_USAGE_PAGE_DIGITIZER)
                                {
                                    switch (deviceInfo.hid.usUsage)
                                    {
                                        case MultiTouchNativeMethods.HID_USAGE_DIGITIZER_DIGITIZER:
                                        case MultiTouchNativeMethods.HID_USAGE_DIGITIZER_PEN:
                                        case MultiTouchNativeMethods.HID_USAGE_DIGITIZER_TOUCHSCREEN:
                                        case MultiTouchNativeMethods.HID_USAGE_DIGITIZER_LIGHTPEN:
                                            {
                                                return true;
                                            }
                                    }
                                }
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine("TabletDeviceCollection: GetRawInputDeviceInfo failed!");
                            }
                        }
                    }
                }
                else if (count < 0)
                {
                    System.Diagnostics.Debug.WriteLine("TabletDeviceCollection: GetRawInputDeviceList failed!");
                }
            }

            return false;
        }

        public bool IsSimulationAvailable
        { 
            get 
            {
                if (MultiTouchVerifier.IsSupportedOS() && 
                    //MultiTouchVerifier.IsTabletDeviceAvailable() && 
                    Tablet.TabletDevices.Count > 0)
                {                    
                    foreach (TabletDevice tablet in Tablet.TabletDevices)
                    {
                        if ((string.Compare(tablet.Name, "VHidPen", false, System.Globalization.CultureInfo.InvariantCulture) == 0) &&
                            tablet.Type == TabletDeviceType.Touch)
                        {
                            _inputDevice = tablet;
                            return true;
                        }
                    }                    
                }

                return false;
            }
        }
        
        public bool IsTouchEnabled
        {
            get { return true; } // 
        }

        protected static MultiTouchVerifier Verifier
        {
            get { return s_verifier; }
            set { s_verifier = value; }
        }

        #endregion

        #region General MP Methods
        
        /// <summary>
        /// check if a given manipulation modes is valid
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        public bool IsValidManipulationMode(ManipulationModes mode)
        {
            return (mode & (ManipulationModes.Translate | ManipulationModes.Scale | ManipulationModes.Rotate)) != ManipulationModes.None;
        }

        #endregion 
        
        #region Validation helpers for MP/IP params

        /// <summary>
        /// Checks if the given value is valid.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="property"></param>
        /// <param name="paramName"></param>
        public void CheckOriginalValue(double value, string property, string paramName)
        {
            Utils.CheckFinite(value, property, paramName);
        }

        /// <summary>
        /// Checks if the given value is a valid velocity.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="property"></param>
        /// <param name="paramName"></param>
        public void CheckVelocity(double value, string property, string paramName)
        {
            Utils.CheckFiniteOrNaN(value, property, paramName);
        }

        /// <summary>
        /// Checks if the given value is a valid offset.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="property"></param>
        /// <param name="paramName"></param>
        public void CheckOffset(double value, string property, string paramName)
        {
            Utils.CheckFiniteNonNegative(value, property, paramName);
        }

        /// <summary>
        /// Checks if the given value is a valid deceleration.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="property"></param>
        /// <param name="paramName"></param>
        public void CheckDeceleration(double value, string property, string paramName)
        {
            Utils.CheckFiniteNonNegative(value, property, paramName);
        }

        /// <summary>
        /// Checks if the given value is valid radius.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="paramName"></param>
        public void CheckRadius(double value, string paramName)
        {
            if (value < 1 || double.IsInfinity(value) || double.IsNaN(value))
            {
                throw new ArgumentOutOfRangeException(paramName, value, string.Format(CultureInfo.InvariantCulture, "CheckRadius for param [{0}]", paramName));
            }
        }

        #endregion

        #region Asserts

        /// <summary>
        /// add it here so all tests can use it directly.
        /// </summary>
        /// <param name="element"></param>
        public void AssertNotNull(object element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
        }

        #endregion 

        #region Hit Testing and Tree Helpers - TODO: refactoring to outside the verifier

        /// <summary>
        /// Checks whether the given element is hit-testable or not.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsHitTestable(object obj)
        {
            UIElement uiElement = obj as UIElement;
            if (uiElement != null)
            {
                return uiElement.IsEnabled && uiElement.IsVisible && uiElement.IsHitTestVisible;
            }
            
            ContentElement contentElement = obj as ContentElement;
            if (contentElement != null)
            {
                return contentElement.IsEnabled;
            }

            UIElement3D uiElement3D = obj as UIElement3D;
            if (uiElement3D != null)
            {
                return uiElement3D.IsEnabled && uiElement3D.IsHitTestVisible && uiElement3D.IsVisible; 
            }

            return false;
        }

        /// <summary>
        /// Returns random element satisfying the given predicate.
        /// </summary>
        /// <param name="enumerable"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static DependencyObject GetRandomElement(IEnumerable enumerable, Predicate<object> predicate)
        {
            // get count
            int count = 0;
            foreach (object obj in enumerable)
            {
                if (predicate == null || predicate(obj))
                {
                    count++;
                }
            }

            if (count == 0)
            {
                // no elements satisfying the given criteria
                return null;
            }

            // get random int
            int random = RandomGenerator.GetInt(count);

            // pick the element
            int i = 0;
            foreach (DependencyObject obj in enumerable)
            {
                if (predicate == null || predicate(obj))
                {
                    if (i == random)
                    {
                        return obj;
                    }
                    i++;
                }
            }

            Utils.Assert(false);
            return null;
        }

        /// <summary>
        /// Modifies visual tree randomly
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="options"></param>
        public static void ModifyVisualTree(Panel panel, VisualTreeOptions options)
        {
            if (panel == null)
            {
                throw new ArgumentNullException("panel");
            }

            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            if (options.RemoveCount > 0)
            {
                // delete some existing elements
                for (int i = 0; i < options.RemoveCount; i++)
                {
                    UIElement element = (UIElement)GetRandomElement(panel.Children, 
                        delegate(object obj) 
                        { 
                            return obj is UIElement && IsHitTestable(obj); 
                        });

                    if (element == null)
                    {
                        // not enough elements
                        break;
                    }

                    panel.Children.Remove(element);
                }
            }

            // modify position of existing elements
            if (options.ModifyPositionCount > 0)
            {
                for (int i = 0; i < options.ModifyPositionCount; i++)
                {
                    UIElement element = (UIElement)GetRandomElement(panel.Children, 
                        delegate(object obj) 
                        { 
                            return obj is UIElement && IsHitTestable(obj); 
                        });

                    if (element == null)
                    {
                        // not enough elements
                        break;
                    }

                    panel.Children.Add(element);
                    
                    // 
                }
            }

            // modify properties of exiting elements
            if (options.ModifyPropertiesCount > 0)
            {
                for (int i = 0; i < options.ModifyPropertiesCount; i++)
                {
                    UIElement element = (UIElement)GetRandomElement(panel.Children, 
                        delegate(object obj) 
                        { 
                            return obj is UIElement && IsHitTestable(obj); 
                        });

                    if (element == null)
                    {
                        // not enough elements
                        break;
                    }

                    SetHitTestRelatedFlags(element, options);
                }
            }

            // 

            // transform
            if (options.RandomPanelTransform)
            {
                panel.RenderTransform = RandomGenerator.GetTransform(RandomGenerator.TransformOptions.LightTransform);
            }
        }

        /// <summary>
        /// enumerates through the visual and/or logical trees.
        /// </summary>
        /// <param name="obj">An object to start enumeration.</param>
        /// <param name="includeParents">Indicates whether parent chain needs to be enumerated.</param>
        /// <param name="includeChildren">Indicates whether children should be included in the enumeration.</param>
        /// <param name="visualTree">Indicates enumeration through the visual tree.</param>
        /// <param name="logicalTree">Indicates enumeration through the logical tree.</param>
        /// <returns></returns>
        public static IEnumerable<DependencyObject> EnumerateTree(DependencyObject obj,
            bool includeParents, bool includeChildren,
            bool visualTree, bool logicalTree)
        {
            // check parameters
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            yield return obj;

            if (includeParents || includeChildren)
            {
                Dictionary<DependencyObject, bool> processed = new Dictionary<DependencyObject, bool>();
                processed[obj] = true;

                if (includeParents)
                {
                    // go recursively through all the parents
                    foreach (DependencyObject parent in EnumerateRecursivelyUp(obj, visualTree, logicalTree, processed, true/*skipRootCheck*/))
                    {
                        yield return parent;
                    }
                }

                if (includeChildren)
                {
                    // go recursively through all the children
                    foreach (DependencyObject child in EnumerateRecursivelyDown(obj, visualTree, logicalTree, processed, true/*skipRootCheck*/))
                    {
                        yield return child;
                    }
                }
            }
        }

        /// <summary>
        /// get a random element under the give element
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static IInputElement GetRandomHitTestableElement(DependencyObject element)
        {
            return (IInputElement)GetRandomElement(EnumerateTree(element, false/*include parent*/, true/*include children*/,
                true/*visual tree*/, true/*logical tree*/), IsHitTestable);
        }

        /// <summary>
        /// get a list of values exptected
        /// </summary>
        /// <param name="getValue"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        public static Dictionary<DependencyObject, object> ReadAllValues(GetPropertyValue getValue, UIElement element)
        {
            // enumerate all child elements
            Dictionary<DependencyObject, object> values = new Dictionary<DependencyObject, object>();

            foreach (DependencyObject obj in
                EnumerateTree(element, false/*include parent*/, true/*include children*/, true/*visual tree*/, true/*logicalTree*/))
            {
                values.Add(obj, getValue(obj));
            }

            return values;
        }

        /// <summary>
        /// enumerates through parents
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="visualTree"></param>
        /// <param name="logicalTree"></param>
        /// <param name="processed"></param>
        /// <param name="skipRootCheck"></param>
        private static IEnumerable<DependencyObject> EnumerateRecursivelyUp(
            DependencyObject obj, bool visualTree, bool logicalTree,
            Dictionary<DependencyObject, bool> processed, bool skipRootCheck)
        {
            Utils.Assert(obj != null, "EnumerateRecursivelyUp - the param obj should not be null");
            Utils.Assert(processed != null, "EnumerateRecursivelyUp - the param processed should not be null");

            if (!skipRootCheck)
            {
                if (processed.ContainsKey(obj))
                {
                    // the object has been already processed
                    yield break;
                }

                else
                {
                    // the object requires processing
                    processed[obj] = true;
                    yield return obj;
                }
            }

            // visual tree
            DependencyObject visualParent = null;
            if (visualTree)
            {
                visualParent = obj is Visual ? VisualTreeHelper.GetParent(obj) : null;
                if (visualParent != null)
                {
                    foreach (DependencyObject parent in
                        EnumerateRecursivelyUp(visualParent, visualTree, logicalTree, processed, false/*skipRootCheck*/))
                    {
                        yield return parent;
                    }
                }
            }

            if (logicalTree)
            {
                DependencyObject logicalParent = LogicalTreeHelper.GetParent(obj);
                if (logicalParent != null && logicalParent != visualParent) // do nothing if logicalParent is the same as coreParent
                {
                    foreach (DependencyObject parent in
                        EnumerateRecursivelyUp(logicalParent, visualTree, logicalTree, processed, false/*skipRootCheck*/))
                    {
                        yield return parent;
                    }
                }
            }
        }

        /// <summary>
        /// enumerates through children
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="visualTree"></param>
        /// <param name="logicalTree"></param>
        /// <param name="processed"></param>
        /// <param name="skipRootCheck"></param>
        /// <returns></returns>
        private static IEnumerable<DependencyObject> EnumerateRecursivelyDown(
            DependencyObject obj, bool visualTree, bool logicalTree,
            Dictionary<DependencyObject, bool> processed, bool skipRootCheck)
        {
            Utils.Assert(obj != null, "EnumerateRecursivelyDown - the param obj should not be null");
            Utils.Assert(processed != null, "EnumerateRecursivelyDown - the param processed should not be null");

            if (!skipRootCheck)
            {
                if (processed.ContainsKey(obj))
                {
                    // the object has been already processed
                    yield break;
                }

                else
                {
                    // the object requires processing
                    processed[obj] = true;
                    yield return obj;
                }
            }

            // visual children
            if (visualTree)
            {
                int coreChildrenCount = obj is Visual ? VisualTreeHelper.GetChildrenCount(obj) : 0;
                for (int i = 0; i < coreChildrenCount; i++)
                {
                    DependencyObject visualChild = VisualTreeHelper.GetChild(obj, i);
                    if (visualChild != null)
                    {
                        foreach (DependencyObject child in
                            EnumerateRecursivelyDown(visualChild, visualTree, logicalTree, processed, false/*skipRootCheck*/))
                        {
                            yield return child;
                        }
                    }
                }
            }

            // logical children
            if (logicalTree)
            {
                foreach (object logicalChildObject in LogicalTreeHelper.GetChildren(obj))
                {
                    DependencyObject logicalChild = logicalChildObject as DependencyObject;
                    if (logicalChild != null)
                    {
                        foreach (DependencyObject child in
                            EnumerateRecursivelyDown(logicalChild, visualTree, logicalTree, processed, false/*skipRootCheck*/))
                        {
                            yield return child;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// set related visual tree change options
        /// </summary>
        /// <param name="element"></param>
        /// <param name="options"></param>
        public static void SetHitTestRelatedFlags(UIElement element, VisualTreeOptions options)
        {
            if (options.GetIsVisible != null)
            {
                element.Visibility = options.GetIsVisible();
            }
            if (options.GetIsEnabled != null)
            {
                element.IsEnabled = options.GetIsEnabled();
            }
            if (options.GetIsHitTestVisible != null)
            {
                element.IsHitTestVisible = options.GetIsHitTestVisible();
            }
        }

        #endregion

        #region General Event handlers helpers

        /// <summary>
        /// Adds an event handler
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="routedEvent"></param>
        /// <param name="handler"></param>
        /// <param name="addToParents">add event handler to all parents</param>
        /// <param name="addToChildren">add event handler to all children</param>
        public static void AddHandler(DependencyObject obj, RoutedEvent routedEvent, Delegate handler, 
            bool addToParents, bool addToChildren)
        {
            // check parameters
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }
            if (routedEvent == null)
            {
                throw new ArgumentNullException("routedEvent");
            }
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }

            AddRemoveHandler(obj, addToParents, addToChildren,
                delegate(DependencyObject curObj)
                {
                    AddHandler(curObj, routedEvent, handler);
                });
        }

        /// <summary>
        /// Removes an event handler
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="routedEvent"></param>
        /// <param name="handler"></param>
        /// <param name="removeFromParents">add event handler to all parents</param>
        /// <param name="removeFromChildren">add event handler to all children</param>
        public static void RemoveHandler(DependencyObject obj, RoutedEvent routedEvent, Delegate handler,
            bool removeFromParents, bool removeFromChildren)
        {
            // check parameters
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }
            if (routedEvent == null)
            {
                throw new ArgumentNullException("routedEvent");
            }
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }

            AddRemoveHandler(obj, removeFromParents, removeFromChildren,
                delegate(DependencyObject curObj)
                {
                    RemoveHandler(curObj, routedEvent, handler);
                });
        }


        public delegate void AddRemoveHandlerDelegate(DependencyObject obj);

        public static void AddRemoveHandler(DependencyObject obj,
            bool addToParents, bool addToChildren, AddRemoveHandlerDelegate addRemoveHandler)
        {
            Debug.Assert(obj != null);
            Debug.Assert(addRemoveHandler != null);

            // 

            foreach (DependencyObject cur in EnumerateTree(obj, addToParents, addToChildren,
                false/*visual tree*/, true/*logical tree*/))
            {
                if (cur is IInputElement)
                {
                    addRemoveHandler(cur);
                }
            }
        }

        /// <summary>
        /// Adds a specified event handler for a specified attached event.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="routedEvent"></param>
        /// <param name="handler"></param>
        public static void AddHandler(DependencyObject element, RoutedEvent routedEvent, Delegate handler)
        {
            // make sure that parameter name 'element' matches the parameter name of the public caller
            //

            Debug.Assert(routedEvent != null, "In AddHandler - RoutedEvent must not be null");

            UIElement uiElement = element as UIElement;
            if (uiElement != null)
            {
                // this is an UIElement
                uiElement.AddHandler(routedEvent, handler);
            }
            else
            {
                ContentElement contentElement = element as ContentElement;
                if (contentElement != null)
                {
                    // this is a ContentElement
                    contentElement.AddHandler(routedEvent, handler);
                }
                else
                {
                    UIElement3D uiElement3D = element as UIElement3D;
                    if (uiElement3D != null)
                    {
                        // this is a UIElement3D
                        uiElement3D.AddHandler(routedEvent, handler);
                    }
                    else 
                    { 
                        //
                    }
                }
            }
        }

        /// <summary>
        ///  Removes a handler for the given attached event
        /// </summary>
        public static void RemoveHandler(DependencyObject element, RoutedEvent routedEvent, Delegate handler)
        {
            // make sure that parameter name 'element' matches the parameter name of the public caller
            //

            Debug.Assert(routedEvent != null, "In RemoveHandler - RoutedEvent must not be null");

            UIElement uiElement = element as UIElement;
            if (uiElement != null)
            {
                // This is an UIElement
                uiElement.RemoveHandler(routedEvent, handler);
            }
            else
            {
                ContentElement contentElement = element as ContentElement;
                if (contentElement != null)
                {
                    // This is an ContentElement
                    contentElement.RemoveHandler(routedEvent, handler);
                }
                else
                {
                    UIElement3D uiElement3D = element as UIElement3D;
                    if (uiElement3D != null)
                    {
                        // this is a ContentElement
                        uiElement3D.AddHandler(routedEvent, handler);
                    }
                    else
                    {
                        // 
                    }
                }
            }
        }

        #endregion

        #region Validation Helpers - TODO: refactoring to outside the verifier

        /// <summary>
        /// compare property values
        /// </summary>
        /// <param name="frameNumber"></param>
        /// <param name="expectedValues"></param>
        /// <param name="actualValues"></param>
        /// <param name="logFilter"></param>
        /// <returns></returns>
        public static bool ComparePropertyValues(int frameNumber,
            Dictionary<DependencyObject, object> expectedValues,
            Dictionary<DependencyObject, object> actualValues,
            Predicate<object> logFilter)
        {
            Debug.WriteLine("");
            Debug.WriteLine("COMPARE FRAME: " + frameNumber);

            bool ok = true;

            Debug.WriteLine("  number of elements, actual=" + actualValues.Count + " expected=" + expectedValues.Count);
            if (expectedValues.Count != actualValues.Count)
            {
                Debug.WriteLine("FAILURE: Invalid number of elements");
                ok = false;
            }

            int i = 0;
            foreach (KeyValuePair<DependencyObject, object> pair in expectedValues)
            {
                object expectedValue = pair.Value;
                object actualValue;
                bool equal;

                if (!actualValues.TryGetValue(pair.Key, out actualValue))
                {
                    equal = false;
                    actualValue = "<value is missing>";
                }
                else
                {
                    equal = object.Equals(expectedValue, actualValue);
                }

                if (!equal || logFilter(actualValue))
                {
                    Debug.WriteLine((equal ? "  " : "**") + (i++).ToString() + ": " + pair.Key + "[" + pair.Key.GetHashCode() + "]" +
                          " actual=" + actualValue + " expected=" + expectedValue +
                          (equal ? "" : " - FAILURE"));
                }

                if (!equal)
                {
                    ok = false;
                }
            }

            return ok;
        }

        #endregion

    }
}
