// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides logging services to describe the eventing system.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 6 $ $Source: //depot/private/WCP_dev_platform/Windowstest/client/wcptests/uis/Text/BVT/Editing/TextEditorTests.cs $")]

namespace Test.Uis.Loggers
{
    #region Namespaces.
    
    using System;
    using System.Text;
    using System.Windows;

    using ReflectionUtils = Test.Uis.Utils.ReflectionUtils;
    
    #endregion Namespaces.

    /// <summary>
    /// Provides methods to log information about the eventing system.
    /// </summary>
    public static class EventLogger
    {
        #region Public Methods.

        /// <summary>
        /// Describes a System.Delegate instance.
        /// </summary>
        /// <param name="item">Delegate to describe.</param>
        /// <returns>
        /// A text description of the delegate, or [null] if item is null.
        /// </returns>
        public static string DescribeDelegate(Delegate item)
        {
            string result;  // Resulting description;

            new System.Security.Permissions.ReflectionPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();

            if (item == null)
            {
                return "[null]";
            }

            result = item.GetType().Name + " [" + item.Method.Name;
            if (item.Target == null)
            {
                result += ", static]";
            }
            else
            {
                result += ", " + item.Target + "]";
            }

            return result;
        }

        /// <summary>
        /// Describes a System.Windows.EventRoute instance.
        /// </summary>
        /// <param name="route">Route to describe.</param>
        /// <returns>
        /// A text description of the event route, or [null] if route is null.
        /// </returns>
        public static string DescribeEventRoute(EventRoute route)
        {
            string result;                  // Resulting description.
            object routeItemList;           // List of route items.
            RoutedEvent routedEventID;    // Event ID for route.
            Array routeItems;               // Array of route items.

            if (route == null)
            {
                return "[null]";
            }

            routedEventID = (RoutedEvent)
                ReflectionUtils.GetField(route, "_routedEventID");
            routeItemList = ReflectionUtils.GetField(route, "_routeItemList");
            routeItems = (Array) ReflectionUtils.InvokeInstanceMethod(
                routeItemList, "ToArray", new object[0]);
            
            result = "EventRoute [ID=" + routedEventID;
            foreach(object routeItem in routeItems)
            {
                result += ", " + DescribeRouteItem(routeItem);
            }
            result += "]";
            
            return result;
        }

        /// <summary>
        /// Describes a System.Windows.RoutedEventArgs instance.
        /// </summary>
        /// <param name="args">Arguments to describe.</param>
        /// <returns>
        /// A text description of the routed event arguments, or [null]
        /// if the args is null.
        /// </returns>
        public static string DescribeRoutedEventArgs(RoutedEventArgs args)
        {
            string result;  // Resulting description.

            if (args == null)
            {
                return "[null]";
            }

            result = "RoutedEventArgs [ID=" + args.RoutedEvent +
                ", Handled=" + args.Handled + ", Source=" + args.Source + "]";
            
            return result;
        }

        /// <summary>
        /// Describes a System.Windows.Events.RouteItem instance.
        /// </summary>
        /// <param name="item">Arguments to describe.</param>
        /// <returns>
        /// A text description of the route item, or [null] if item is null.
        /// </returns>
        public static string DescribeRouteItem(object item)
        {
            string result;  // Resulting description.
            object target;  // Target of route item.
            object handler; // Information on handler (RoutedEventHandlerInfo).
            Delegate handlerDelegate; // Delegate stored in handler.

            if (item == null)
            {
                return "[null]";
            }

            target = ReflectionUtils.GetField(item, "_target");
            handler = ReflectionUtils.GetField(item, "_routedEventHandlerInfo");
            handlerDelegate = (handler == null)? null :
                (Delegate) ReflectionUtils.GetField(handler, "_handler");
            result = "RouteItem [Target=" + target + ", Handler=" +
                DescribeDelegate(handlerDelegate) + "]";
            
            return result;
        }

        #endregion Public Methods.
    }
}

