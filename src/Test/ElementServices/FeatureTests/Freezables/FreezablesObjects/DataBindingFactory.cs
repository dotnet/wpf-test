// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/***********************************************************
 *
 *   Copyright (c) Microsoft Corporation, 2005
 *
 *   Program:   DataBinding Object Factory
 
 *
 ************************************************************/

using System;
using System.Xml;


namespace Microsoft.Test.ElementServices.Freezables
{
    /**********************************************************************************
    * CLASS:          DataBindingFactory
    **********************************************************************************/
    public class DataBindingFactory
    {
        /******************************************************************************
        * Function:          DataBindingBase
        ******************************************************************************/
        public static DataBindingBase Make(Type t)
        {
            if (t == null)
            {
                throw new ApplicationException("ERROR -- DataBindingBase Make: Type is null.");
            }
            else
            {
                string type = t.ToString();

                switch (type)
                {
                    case "System.Windows.Media.Brush":
                        return new BrushDataBinding();

                    case "System.Windows.Media.Geometry":
                        return new GeometryDataBinding();

                    case "System.Windows.Media.Pen":
                        return new PenDataBinding();

                    default:
                        throw new ApplicationException("This DataBinding type is not supported yet: " + type);
                }
            }
        }
    }
}
