// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
 * Purpose: A class to run test for attached routed event.
 *
 
  
 * Revision:         $Revision: $
 
 * Filename:         $Source: $
********************************************************************/

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using Avalon.Test.CoreUI.Common;
using System.Windows.Controls.Primitives;
using System.Reflection;
using Avalon.Test.CoreUI.Events;


namespace Avalon.Test.CoreUI.Serialization
{
    /// <summary>
    /// A class to test attache RoutedEvent.
    /// </summary>
    [CoreTestsLoader(CoreTestsTestType.ClassBase)]
    [TestCasePriority("1")]
    [TestCaseArea(@"Serialization\AttachedRoutedEvent")]
    [TestCaseMethod("Run")]
    [TestCaseDisabled("0")]
    [TestCaseSecurityLevel(TestCaseSecurityLevel.FullTrust)]
    public class AttachedRoutedEventTests
    {
        /// <summary>
        /// Entrance
        /// </summary>
        public void Run()
        {
            Type handlerType;
            Type argsType;
            Delegate handler;
            RoutedEventArgs args;
            foreach (RoutedEvent id in _eventIdsToTest)
            {
                CoreLogger.LogStatus("Testing event: " + id.Name + " on " + id.OwnerType.ToString());

                AttachedRoudtedEventHelper.GetHandlerAndargs(id, out handler, out args );
                AttachedRoudtedEventHelper.HandlerInvokedCount = 0;
                UIElement uie = new UIElement();
                args.RoutedEvent= id;
                Type hostType = id.OwnerType;
                //get add hander method
                MethodInfo addHandler = hostType.GetMethod("Add" + id.Name + "Handler", BindingFlags.Static | BindingFlags.Public);

                object[] parameters = {uie, handler};
                //Add an handler
                addHandler.Invoke(null, parameters);
                uie.RaiseEvent(args);
                if( 1 != AttachedRoudtedEventHelper.HandlerInvokedCount)
                    throw new Microsoft.Test.TestValidationException("EventHandler should be called 1 time, actual: " + AttachedRoudtedEventHelper.HandlerInvokedCount.ToString() + ".");
                //reset count
                AttachedRoudtedEventHelper.HandlerInvokedCount = 0;
                //Add one more handler
                addHandler.Invoke(null, parameters);
                uie.RaiseEvent(args);
                if (2 != AttachedRoudtedEventHelper.HandlerInvokedCount)
                    throw new Microsoft.Test.TestValidationException("EventHandler should be called 2 time, actual: " + AttachedRoudtedEventHelper.HandlerInvokedCount.ToString() + ".");
                //Get remove handler method
                MethodInfo removeHandler = hostType.GetMethod("Remove" + id.Name + "Handler", BindingFlags.Static | BindingFlags.Public);
                //reset count
                AttachedRoudtedEventHelper.HandlerInvokedCount = 0;
                //remove one handler
                removeHandler.Invoke(null, parameters);
                uie.RaiseEvent(args);
                if (1 != AttachedRoudtedEventHelper.HandlerInvokedCount)
                    throw new Microsoft.Test.TestValidationException("EventHandler should be called 1 time, actual: " + AttachedRoudtedEventHelper.HandlerInvokedCount.ToString() + ".");

                //reset count
                AttachedRoudtedEventHelper.HandlerInvokedCount = 0;
                //remove another handler
                removeHandler.Invoke(null, parameters);
                uie.RaiseEvent(args);
                if (0 != AttachedRoudtedEventHelper.HandlerInvokedCount)
                    throw new Microsoft.Test.TestValidationException("EventHandler should be called 0 time, actual: " + AttachedRoudtedEventHelper.HandlerInvokedCount.ToString() + ".");

                //reset count
                AttachedRoudtedEventHelper.HandlerInvokedCount = 0;
                //remove non-existed handler
                removeHandler.Invoke(null, parameters);
                uie.RaiseEvent(args);
                if (0 != AttachedRoudtedEventHelper.HandlerInvokedCount)
                    throw new Microsoft.Test.TestValidationException("EventHandler should be called 0 time, actual: " + AttachedRoudtedEventHelper.HandlerInvokedCount.ToString() + ".");
            }

        }

        RoutedEvent[] _eventIdsToTest = 
        {
            DataObject.CopyingEvent,
            DataObject.PastingEvent,               
            DataObject.SettingDataEvent,/*
            DragDrop.DropEvent,
            DragDrop.PreviewDropEvent,
            DragDrop.DragLeaveEvent,
            DragDrop.PreviewDragLeaveEvent,
            DragDrop.DragOverEvent,
            DragDrop.PreviewDragOverEvent,
            DragDrop.DragEnterEvent,
            DragDrop.PreviewDragEnterEvent,
            DragDrop.GiveFeedbackEvent,
            DragDrop.PreviewGiveFeedbackEvent,
            DragDrop.QueryContinueDragEvent,
            DragDrop.PreviewQueryContinueDragEvent, internal contructor*/
            AccessKeyManager.AccessKeyPressedEvent,
            Mouse.LostMouseCaptureEvent,
            Mouse.GotMouseCaptureEvent,
            Mouse.MouseLeaveEvent,
            Mouse.MouseEnterEvent,
            Mouse.MouseMoveEvent,
            Mouse.PreviewMouseMoveEvent,
            Mouse.MouseWheelEvent,
            Mouse.PreviewMouseWheelEvent,
            Mouse.MouseUpEvent,
            Mouse.PreviewMouseUpEvent,
            Mouse.MouseDownEvent,
            Mouse.PreviewMouseDownEvent,
            Mouse.PreviewMouseUpOutsideCapturedElementEvent,
            Mouse.PreviewMouseDownOutsideCapturedElementEvent,
            TextCompositionManager.TextInputEvent,
            TextCompositionManager.PreviewTextInputEvent,
            TextCompositionManager.TextInputUpdateEvent,
            TextCompositionManager.PreviewTextInputUpdateEvent,
            TextCompositionManager.TextInputStartEvent,
            TextCompositionManager.PreviewTextInputStartEvent,
            EventHelper.BubbleEvent,
            EventHelper.TunnelEvent
        };
        
    }
}
