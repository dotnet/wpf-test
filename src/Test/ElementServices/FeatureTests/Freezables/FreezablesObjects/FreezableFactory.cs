// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Xml;
using System.Security;
using System.Security.Policy;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

using Microsoft.Test.ElementServices.Freezables.Objects;


namespace Microsoft.Test.ElementServices.Freezables
{
    /**********************************************************************************
    * CLASS:          FreezableFactory
    **********************************************************************************/
    public class FreezableFactory
    {
        public static Freezable Make(Type t)
        {
            if (t == null)
            {
                throw new ApplicationException("ERROR -- Make: Type is null.");
            }
            else
            {
                string type = t.ToString();

                switch (type)
                {
                    case "System.Windows.Media.Brush":
                        Brush brush = (Brush)PredefinedObjects.MakeValue(t);
                        return brush;

                    case "System.Windows.Media.Geometry":
                        Geometry geo = (Geometry)PredefinedObjects.MakeValue(t);
                        return geo;

                    case "System.Windows.Media.Pen":
                        Pen pen = (Pen)PredefinedObjects.MakeValue(t);
                        return pen;

                    case "System.Windows.Media.Transform":
                        Transform transform = (Transform)PredefinedObjects.MakeValue(t);
                        return transform;

                    default:
                        throw new ApplicationException("Unknown type " + type);
                }
            }
        }

    }
}

