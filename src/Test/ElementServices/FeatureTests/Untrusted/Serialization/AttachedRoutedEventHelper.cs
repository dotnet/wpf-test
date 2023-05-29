// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Helper class to ger a hander and a args for a certain RoutedEvent.
 *
 
  
 * Revision:         $Revision: $
 
 * Filename:         $Source: $
 * ********************************************************************/

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.ComponentModel.Design.Serialization;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Markup;
using System.Xml;

using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Events;
using System.Windows.Interop;

namespace Avalon.Test.CoreUI.Serialization
{
    /// <summary>
    /// Helper class to ger a hander and a args for a certain RoutedEvent.
    /// </summary>
    public class AttachedRoudtedEventHelper
    {

        /// <summary>
        /// Given a HandlerType, return an instance of corresponding 
        /// Args and an handler, whcih increader a HandlerInvokedCount by one
        /// </summary>
        /// <param name="id"></param>
        /// <param name="handler"></param>
        /// <param name="args"></param>
        public static void GetHandlerAndargs(RoutedEvent id, out Delegate handler, out RoutedEventArgs args)
        {
            handler = null;
            args = null;
            string handlerType =id.HandlerType.ToString();
            switch (handlerType)
            {
                case "System.Windows.DataObjectCopyingEventHandler":
                    args = new DataObjectCopyingEventArgs(new DataObject(), true);
                    handler = (Delegate)(new DataObjectCopyingEventHandler(MyDataObjectCopyingEventHandler));
                    break;
                case "System.Windows.DataObjectPastingEventHandler":
                    args = new DataObjectPastingEventArgs(new DataObject(), true, "");
                    handler = (Delegate)(new DataObjectPastingEventHandler(MyDataObjectPastingEventHandler));
                    break;
                case "System.Windows.DataObjectSettingDataEventHandler":
                    args = new DataObjectSettingDataEventArgs(new DataObject(), "");
                    handler = (Delegate)(new DataObjectSettingDataEventHandler(MyDataObjectSettingDataEventHandler));
                    break;
                case "System.Windows.Input.AccessKeyPressedEventHandler":
                    args = new AccessKeyPressedEventArgs();
                    handler = (Delegate)(new AccessKeyPressedEventHandler(MyAccessKeyPressedEventHandler));
                    break;
                case "System.Windows.Input.KeyboardFocusChangedEventHandler":
                    args = new KeyboardFocusChangedEventArgs(null, 0, null, null);
                    handler = (Delegate)(new KeyboardFocusChangedEventHandler(MyKeyboardFocusChangedEventHandler));
                    break;
                case "System.Windows.Input.KeyEventHandler":            
                    SurfaceCore surfaceCore = new SurfaceCore("HwndSource",0,0,800,700);
                    args = new KeyEventArgs(null, (HwndSource)surfaceCore.SurfaceObject, 0, Key.None);
                    handler = (Delegate)(new KeyEventHandler(MyKeyEventHandler));
                    break;
                case "System.Windows.Input.QueryCursorEventHandler":
                    args = new QueryCursorEventArgs(null, 0);
                    handler = (Delegate)(new QueryCursorEventHandler(MyQueryCursorEventHandler));
                    break;
                case "System.Windows.Input.MouseEventHandler":
                    args = new MouseEventArgs(InputManager.Current.PrimaryMouseDevice, 0);
                    handler = (Delegate)(new MouseEventHandler(MyMouseEventHandler));
                    break;
                case "System.Windows.Input.MouseWheelEventHandler":
                    args = new MouseWheelEventArgs(InputManager.Current.PrimaryMouseDevice, 0, 0);
                    handler = (Delegate)(new MouseWheelEventHandler(MyMouseWheelEventHandler));
                    break;
                case "System.Windows.Input.MouseButtonEventHandler":
                    args = new MouseButtonEventArgs(InputManager.Current.PrimaryMouseDevice, 0, MouseButton.Left);
                    handler = (Delegate)(new MouseButtonEventHandler(MyMouseButtonEventHandler));
                    break;
                case "System.Windows.Input.StylusEventHandler":
                    args = new StylusEventArgs(null, 0);
                    handler = (Delegate)(new StylusEventHandler(MyStylusEventHandler));
                    break;
                case "System.Windows.Input.StylusSystemGestureEventHandler":
                    args = new StylusSystemGestureEventArgs(null, 0, SystemGesture.Tap);
                    handler = (Delegate)(new StylusSystemGestureEventHandler(MyStylusSystemGestureEventHandler));
                    break;
                case "System.Windows.Input.TextCompositionEventHandler":
                    args = new TextCompositionEventArgs(null, new TextComposition(null, null, ""));
                    handler = (Delegate)(new TextCompositionEventHandler(MyTextCompositionEventHandler));
                    break;
                case "System.Windows.Controls.IsSelectedChangedEventHandler":
                    args = new RoutedPropertyChangedEventArgs<bool>(true, false);
                    handler = (Delegate)(new RoutedPropertyChangedEventHandler<bool>(MyIsSelectedChangedEventHandler));
                    break;
                case "Avalon.Test.CoreUI.Events.CustomRoutedEventHandler":
                    args = new CustomRoutedEventArgs(null, null);
                    handler = (Delegate)(new CustomRoutedEventHandler(MyCustomRoutedEventHandler));
                    break;
                default:
                    break;
            }
            return;
        }

        /// <summary>
        /// An handler of type CustomRoutedEventHandler to increase HandlerInvokedCount by 1 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void MyCustomRoutedEventHandler(object sender, CustomRoutedEventArgs e)
        {
            HandlerInvokedCount++;
        }

        /// <summary>
        /// An handler of type IsSelectedChangedEventHandler to increase HandlerInvokedCount by 1 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void MyIsSelectedChangedEventHandler(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            HandlerInvokedCount++;
        }
        /// <summary>
        /// An handler of type TextCompositionEventHandler to increase HandlerInvokedCount by 1 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void MyTextCompositionEventHandler(object sender, TextCompositionEventArgs e)
        {
            HandlerInvokedCount++;
        }
        /// <summary>
        /// An handler of type StylusSystemGestureEventHandler to increase HandlerInvokedCount by 1 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void MyStylusSystemGestureEventHandler(object sender, StylusSystemGestureEventArgs e)
        {
            HandlerInvokedCount++;
        }

        /// <summary>
        /// An handler of type StylusEventHandler to increase HandlerInvokedCount by 1 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void MyStylusEventHandler(object sender, StylusEventArgs e)
        {
            HandlerInvokedCount++;
        }

        /// <summary>
        /// An handler of type MouseButtonEventHandler to increase HandlerInvokedCount by 1 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void MyMouseButtonEventHandler(object sender, MouseButtonEventArgs e)
        {
            HandlerInvokedCount++;
        }
        /// <summary>
        /// An handler of type MouseWheelEventHandler to increase HandlerInvokedCount by 1 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void MyMouseWheelEventHandler(object sender, MouseWheelEventArgs e)
        {
            HandlerInvokedCount++;
        }
        /// <summary>
        /// An handler of type MouseEventHandler to increase HandlerInvokedCount by 1 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void MyMouseEventHandler(object sender, MouseEventArgs e)
        {
            HandlerInvokedCount++;
        }
        /// <summary>
        /// An handler of type QueryCursorEventHandler to increase HandlerInvokedCount by 1 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void MyQueryCursorEventHandler(object sender, QueryCursorEventArgs e)
        {
            HandlerInvokedCount++;
        }
 
        /// <summary>
        /// An handler of type KeyEventHandler to increase HandlerInvokedCount by 1 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void MyKeyEventHandler(object sender, KeyEventArgs e)
        {
            HandlerInvokedCount++;
        }
        /// <summary>
        /// An handler of type KeyboardFocusChangedEventHandler to increase HandlerInvokedCount by 1 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        public static void MyKeyboardFocusChangedEventHandler(object sender, KeyboardFocusChangedEventArgs e)
        {
            HandlerInvokedCount++;
        }
        /// <summary>
        /// An handler of type DataObjectCopyingEventHandler to increase HandlerInvokedCount by 1 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void MyDataObjectCopyingEventHandler(object sender, DataObjectCopyingEventArgs e)
        {
            HandlerInvokedCount++;
        }
        /// <summary>
        /// An handler of type DataObjectPastingEventHandler to increase HandlerInvokedCount by 1 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void MyDataObjectPastingEventHandler(object sender, DataObjectPastingEventArgs e)
        {
            HandlerInvokedCount++;
        }

        /// <summary>
        /// An handler of type DataObjectSettingDataEventHandler to increase HandlerInvokedCount by 1 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void MyDataObjectSettingDataEventHandler(object sender, DataObjectSettingDataEventArgs e)
        {
            HandlerInvokedCount++;
        }
        /// <summary>
        /// An handler of type AccessKeyPressedEventHandler to increase HandlerInvokedCount by 1 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void MyAccessKeyPressedEventHandler(object sender, AccessKeyPressedEventArgs e)
        {
            HandlerInvokedCount++;
        }

        /// <summary>
        /// A property to indicate how many time handler has been called.
        /// </summary>
        public static int HandlerInvokedCount
        {
            get
            {
                return s_handlerInvokedCount;
            }
            set
            {
                s_handlerInvokedCount = value;
            }
        }
       
        static int s_handlerInvokedCount = 0;

    }
}

