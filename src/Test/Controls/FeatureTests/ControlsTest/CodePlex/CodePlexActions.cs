//---------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All rights reserved.
//
//---------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

using Microsoft.Test.Input;
using Microsoft.Test.Logging;


namespace Avalon.Test.ComponentModel.Actions
{
    /// <summary>
    /// Interface that must be implemented by every event handler used as an argument to:
    ///    CodePlexActions.AttachRoutedPropertyChangedEventHandler
    /// and
    ///    CodePlexActions.VerifyRoutedPropertyChangedEventHandled
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICodePlexRoutedChangedPropertyEventHandler<T>
    {
        void Changed(Object sender, RoutedPropertyChangedEventArgs<T> e);

        bool CompareTo(Int32 HitCount, T FirstValue, T LastValue);

        void Query(ref Int32 HitCount, ref T FirstValue, ref T LastValue);
    }

    /// <summary>
    /// CodePlexActions: contains actions specific to CodePlex tests. 
    /// </summary>
    public static class CodePlexActions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        public static bool BugCommentAction(string comment)
        {
            TestLog.Current.LogEvidence("!!!BUG COMMENT: " + comment);
            return true;
        }

        /// <summary>
        /// Wait the specified amount of time.
        /// </summary>
        /// <param name="timeToWait"></param>
        public static bool WaitAction(string waitInterval)
        {
            TimeSpan timeToWait = TimeSpan.Parse(waitInterval);

            if (timeToWait == TimeSpan.Zero)
            {
                QueueHelper.WaitTillQueueItemsProcessed();
            }
            else
            {
                QueueHelper.WaitTillTimeout(timeToWait);
            }
            return true;
        }

        /// <summary>
        /// Set one or more properties on a FrameworkElement.
        /// This is just a wrapper that adds a bool return so it can be used as an Action element
        /// in a "Avalon.Test.ComponentModel.Actions" context.
        /// </summary>
        /// <param name="frmElement">The FrameworkElement instance whose properties to set</param>
        /// <param name="actionParams">Sequence of interleaved property-name, property value objects</param>
        /// <returns>true</returns>
        /// <example_xtc_usage>
        ///  <!--
        ///  <CodePlexActions.ControlSetPropertyAction frmElement="{SceneTreeSearch:CONTROLTOTEST}">
        ///     <actionParams>
        ///        <sys:String>Value</sys:String>
        ///        <sys:Decimal>13.0</sys:Decimal>
        ///     </actionParams>
        ///  </CodePlexActions.ControlSetPropertyAction>
        ///  -->
        ///</example_xtc_usage>
        public static bool ControlSetPropertyAction(FrameworkElement frmElement, params object[] actionParams)
        {
            ControlPropertyAction controlPropertyAction = new ControlPropertyAction();

            controlPropertyAction.Do(frmElement, actionParams);

            return true;
        }

        /// <summary>
        /// Implements a late-binding reflection-based equivalent of expression:
        ///           //targetElement.eventName += handler.Changed;
        /// The event specified by EventName must have the following form:
        ///  // public event RoutedPropertyChangedEventHandler<T> EventName
        /// Intended usage is with follow-on call to VerifyRoutedPropertyChangedEventHandled
        /// with the same Handler_Holder ContentControl argument.
        /// </summary>
        /// <param name="TargetElement">
        /// A control instance having an event to attach a delegate to
        /// </param>
        /// <param name="EventName">
        /// The name of the RoutedChangedEvent type exposed by targetElement
        /// </param>
        /// <param name="Handler_Holder">
        /// Content Control containing an instance of a class implementing
        /// interface ICodePlexRoutedChangedPropertyEventHandler<T>
        /// </param>
        /// <returns>bool</returns>
        /// <example_xtc_usage>
        ///  <!--
        ///  <CodePlexActions.AttachRoutedPropertyChangedEventHandler
        ///                              TargetElement="{SceneTreeSearch:CONTROLTOTEST}"
        ///                              EventName="ValueChanged"
        ///                              Handler_Holder="{SceneTreeSearch:VALUECHANGEDHANDLER_HOLDER}" />
        ///  -->
        ///</example_xtc_usage>
        //public static bool AttachRoutedPropertyChangedEventHandler(FrameworkElement TargetElement,
        //                                                           String EventName,
        //                                                           ContentControl Handler_Holder)
        public static bool AttachRoutedPropertyChangedEventHandler(FrameworkElement TargetElement,
                                                                   String EventName,
                                                                   FrameworkElement Handler)
        {
            EventInfo targetEventInfo =
                TargetElement.GetType().GetEvent(EventName, (BindingFlags.Instance | BindingFlags.Public));

            if (targetEventInfo == null)
                throw new ArgumentException("Could not find event on targetElement", "eventName");

            MethodInfo handlerMethodInfo =
                Handler.GetType().GetMethod("Changed", (BindingFlags.Instance | BindingFlags.Public));

            if (handlerMethodInfo == null)
                throw new ArgumentException("Could not find public instance method 'Changed' on handler", "handler");

            Type targetEventHandlerType = targetEventInfo.EventHandlerType;

            Delegate delegateInstance = Delegate.CreateDelegate( targetEventHandlerType,
                                                                 Handler,
                                                                 handlerMethodInfo);

            targetEventInfo.AddEventHandler(TargetElement, delegateInstance);

            return true;
        }

        /// <summary>
        /// Compare expected to actual values retained by the custom RoutedPropertyChangedEventHandler.
        /// Intended usage is as a follow-on to AttachRoutedPropertyChangedEventHandler, and intervening
        /// actions intended to fire the event for which the custom handler was attached.
        /// </summary>
        /// <param name="Handler_Holder">
        /// Content Control containing an instance of a class implementing
        /// interface ICodePlexRoutedChangedPropertyEventHandler<T>
        /// </param>
        /// <param name="ExpectedHits">
        /// Expected number of times the event was fired.
        /// </param>
        /// <param name="ExpectedFirstValue">
        /// Expected 'before' value at the time the handler handled the first event
        /// </param>
        /// <param name="ExpectedLastValue">
        /// Expected 'after' value at the time the handler handled the last event
        /// </param>
        /// <returns>bool</returns>
        /// <example_xtc_usage>
        ///  <!--
        /// <CodePlexActions.VerifyRoutedPropertyChangedEventHandled
        ///                                         Handler_Holder="{SceneTreeSearch:VALUECHANGEDHANDLER_HOLDER}">
        ///   <ExpectedHits>
        ///     <sys:Int32>1</sys:Int32>
        ///   </ExpectedHits>
        ///   <ExpectedFirstValue>
        ///     <sys:Decimal>12.0</sys:Decimal>
        ///   </ExpectedFirstValue>
        ///   <ExpectedLastValue>
        ///     <sys:Decimal>13.0</sys:Decimal>
        ///   </ExpectedLastValue>
        /// </CodePlexActions.VerifyRoutedPropertyChangedEventHandled>
        ///  -->
        ///</example_xtc_usage>
        public static bool VerifyRoutedPropertyChangedEventHandled(FrameworkElement Handler,
                                                                   Int32 ExpectedHits,
                                                                   Object ExpectedFirstValue,
                                                                   Object ExpectedLastValue)
        {
            MethodInfo handlerMethodInfo =
                Handler.GetType().GetMethod("CompareTo", BindingFlags.Instance | BindingFlags.Public);

            if (handlerMethodInfo == null)
            {
                throw new ArgumentException("Could not find method 'CompareTo' on handler", "handler");
            }
            return (bool)handlerMethodInfo.Invoke(
                           Handler, new Object[] { ExpectedHits, ExpectedFirstValue, ExpectedLastValue });
        }


        public static bool ClickButton(ButtonBase ButtonControl, Int32 TimesCount)
        {
            for (Int32 i = 0; i < TimesCount; ++i)
            {
                ClickButtonWithDelaySpan(ButtonControl, TimeSpan.Zero);
            }
            return true;
        }

        /// <summary>
        /// See related method: "ClickButtonWithDelaySpan" 
        /// Convenience wrapper so XTC doesn't have to instantiate a
        /// TimeSpan object to specify a delay, but can use a string.
        /// </summary>
        /// <param name="ButtonControl"></param>
        /// <param name="DelayAfter"></param>
        /// <returns></returns>
        public static bool ClickButtonWithDelayString(ButtonBase ButtonControl, String DelayAfter)
        {
            return ClickButtonWithDelaySpan(ButtonControl, TimeSpan.Parse(DelayAfter));
        }

        /// <summary>
        /// Click the specified ButtonBase-derived control, and then wait for specified TimeSpan.
        /// </summary>
        /// <param name="ButtonControl">The Button control to click</param>
        /// <param name="DelayAfter">A TimeSpan to wait after clicking, can be TimeSpan.Zero</param>
        /// <returns></returns>
        public static bool ClickButtonWithDelaySpan(ButtonBase ButtonControl, TimeSpan DelayAfter)
        {
            UserInput.MouseLeftClickCenter(ButtonControl);
            if (DelayAfter != TimeSpan.Zero)
            {
                QueueHelper.WaitTillTimeout(DelayAfter);
            }
            else
            {
                QueueHelper.WaitTillQueueItemsProcessed();
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="frmElement"></param>
        /// <param name="routedCommandName"></param>
        /// <param name="startSearchDepth"></param>
        /// <param name="endSearchDepth"></param>
        /// <param name="repeatCount"></param>
        /// <returns></returns>
        public static bool FindAndClickVisualTreeRoutedCommandButton<T>(FrameworkElement frmElement,
                                                                     String routedCommandName,
                                                                     Int32 startSearchDepth,
                                                                     Int32 endSearchDepth,
                                                                     Int32 repeatCount) where T : ButtonBase, new()
        {

            if (endSearchDepth < 1)
            {
                throw new ArgumentOutOfRangeException("endSearchDepth", "Search Depth must be 1 or greater.");
            }


            if (startSearchDepth > endSearchDepth)
            {
                throw new ArgumentOutOfRangeException("startSearchDepth", "Start Search Depth must be no less than End Search Depth.");
            }

            PropertyInfo propertyInfo = frmElement.GetType().GetProperty(
                routedCommandName, BindingFlags.Public | BindingFlags.Static | BindingFlags.GetProperty);

            if (propertyInfo == null)
            {
                throw new ArgumentException("No public instance property by that name.", "routedCommandName");
            }

            RoutedCommand command = propertyInfo.GetGetMethod().Invoke(frmElement, null) as RoutedCommand;

            if (command == null)
            {
                throw new ArgumentException("No public instance RoutedCommand property by that name.", "routedCommandName");
            }

            return FindAndClickVisualRoutedCommandButton<T>(
                frmElement, command, startSearchDepth, endSearchDepth, repeatCount);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="frmElement"></param>
        /// <param name="commandInstance"></param>
        /// <param name="startSearchDepth"></param>
        /// <param name="endSearchDepth"></param>
        /// <param name="repeatCount"></param>
        /// <returns>false if not found</returns>
        internal static bool FindAndClickVisualRoutedCommandButton<T>(FrameworkElement frmElement,
                                                                     RoutedCommand commandInstance,
                                                                     Int32 startSearchDepth,
                                                                     Int32 endSearchDepth,
                                                                     Int32 repeatCount) where T : ButtonBase, new()
        {
            List<DependencyObject> list = new List<DependencyObject>();

            FindExactTypeChildrenInVisualTree(
                (new T()).GetType(), frmElement, startSearchDepth, endSearchDepth, list);

            foreach (DependencyObject x in list)
            {
                // click the first button that satisfies the commandInstance constraint
                if (((T)x).Command == commandInstance) 
                {
                    ClickButton((T)x, repeatCount);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="targetObj"></param>
        /// <param name="startSearchDepth"></param>
        /// <param name="endSearchDepth"></param>
        /// <param name="list"></param>
        /// <returns>void</returns>
        private static void FindExactTypeChildrenInVisualTree(Type type,
                                                                DependencyObject targetObj,
                                                                Int32 startSearchDepth,
                                                                Int32 endSearchDepth,
                                                                List<DependencyObject> list)
        {
            if (endSearchDepth == 0) return;

            Int32 childCount = System.Windows.Media.VisualTreeHelper.GetChildrenCount(targetObj);
            for (Int32 j = 0; j < childCount; ++j)
            {
                DependencyObject dobj = System.Windows.Media.VisualTreeHelper.GetChild(targetObj, j);
                if (startSearchDepth <= 1)
                {
                    if (dobj.GetType() == type)
                    {
                        list.Add(dobj);
                    }
                }
                FindExactTypeChildrenInVisualTree(type, dobj, (startSearchDepth - 1), (endSearchDepth - 1), list);
            }
        }
    }
}
