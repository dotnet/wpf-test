// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Media;
using System.Security;
using System.Runtime.InteropServices;
using System.Reflection;

namespace Avalon.Test.CoreUI
{
    /// <summary>
    ///     
    /// Utils to convert avalon coordinates to screen coordinats
    /// This use a Internal utility in Avalon (MS.Internal.PointUtil)
    /// </summary>
    public static class PointUtil
    {        

        private static Type GetPointUtilType()
        {

        
            Type tsw = typeof(UIElement);
            Assembly assembly = tsw.Assembly;

            Type type = assembly.GetType("MS.Internal.PointUtil");

            return type;
        }

        private static Point InvokeStaticMethod(Type type, string name, object[] args)
        {
            return (Point)type.InvokeMember(name, 
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod | BindingFlags.Static,
                null, 
                null, 
                args,
                System.Globalization.CultureInfo.InvariantCulture);
        }


        /// <summary>
        ///     Convert a point from "client" coordinate space of a window into
        ///     the coordinate space of the root element of the same window.
        /// </summary>
        public static Point ClientToRoot(Point pt, PresentationSource presentationSource)
        {
            object[] args = {pt, presentationSource};
           
            return InvokeStaticMethod(GetPointUtilType(),"ClientToRoot", args);
        }

        /// <summary>
        ///     Convert a point from the coordinate space of a root element of
        ///     a window into the "client" coordinate space of the same window.
        /// </summary>
        public static Point RootToClient(Point pt, PresentationSource presentationSource)
        {
            object[] args = {pt, presentationSource};
           
            return InvokeStaticMethod(GetPointUtilType(),"RootToClient", args);

        }
        
        /// <summary>
        ///     Convert a point from "above" the coordinate space of a
        ///     visual into the the coordinate space "below" the visual.
        /// </summary>
        public static Point ApplyVisualTransform(Point pt, Visual v, bool inverse)
        {

            object[] args = {pt, v, inverse};
           
            return InvokeStaticMethod(GetPointUtilType(),"ApplyVisualTransform", args);
        }

        /// <SecurityNote>
        /// SecurityCritical: This code causes eleveation to unmanaged code via call to GetWindowLong
        /// SecurityTreatAsSafe: This data is ok to give out
        /// validate all code paths that lead to this.
        /// </SecurityNote>
        /// <summary>
        ///     Convert a point from "client" coordinate space of a window into
        ///     the coordinate space of the screen.
        /// </summary>
        public static Point ClientToScreen(Point ptClient, PresentationSource presentationSource)
        {
                 
            object[] args = {ptClient, presentationSource};
           
            return InvokeStaticMethod(GetPointUtilType(),"ClientToScreen", args);
        }
        
        /// <summary>
        ///     Convert a point from the coordinate space of the screen into
        ///     the "client" coordinate space of a window.
        /// </summary>
        public static Point ScreenToClient(Point ptScreen, PresentationSource presentationSource)
        {
             object[] args = {ptScreen, presentationSource};
           
            return InvokeStaticMethod(GetPointUtilType(),"ScreenToClient", args);
        }

    }
}

