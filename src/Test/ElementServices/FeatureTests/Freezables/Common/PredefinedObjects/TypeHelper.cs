// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/***********************************************************
 *
 *     Copyright (c) Microsoft Corporation, 2003
 *
 *     Program:    TypeHelper Class
 
 *
 ************************************************************/

using System;
using System.Xml;
using System.Reflection;


namespace Microsoft.Test.ElementServices.Freezables.Objects
{    
    //--------------------------------------------------------------

    public class TypeHelper
    {
        public static bool IsDerivative(Type t, string name)
        {
            for (Type b = t.BaseType; b != null; b = b.BaseType)
            {
                if (b.ToString() == name)
                {
                    return true;
                }
            }
            return false;
        }


        //----------------------------------------------------------
        public static bool IsType(Type t, string checkedType)
        {
            return (t.ToString() == checkedType) ? true : false;
        }

        public static bool IsFreezable(Type t)
        {
            return IsDerivative(t, "System.Windows.Freezable");
        }

        public static bool IsDependencyObject(Type t)
        {
            return IsDerivative(t, "System.Windows.DependencyObject");
        }

        public static bool IsMediaTimeline(Type t)
        {
            return (IsDerivative (t, "System.Windows.Media.MediaTimeline") || (t.ToString() == "System.Windows.Media.MediaTimeline"));
        }
        public static bool IsMedia3D(Type t)
        {
            return (t.ToString().StartsWith("System.Windows.Media.Media3D"));
        }

        public static bool IsAnimatable(Type t)
        {
            return IsDerivative(t, "System.Windows.Media.Animation.Animatable");
        }

        public static bool IsAnimationCollection(Type t)
        {
            return IsDerivative(t, "System.Windows.Media.Animation.AnimationCollection");
        }

        // returns true if there are embedded freezable types;    false otherwise
        public static bool IsComplexChangeable(Type t)
        {
            // get the type's fields
            System.Reflection.FieldInfo[] fi = t.GetFields (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

            for (int i = 0; i < fi.Length; i++)
            {
                // check if any of the members are changeables
                if (IsFreezable (fi[i].FieldType))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
