// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************************
*    Purpose:  Avalon-specific implementation for primitive mouse simulation. 
*    
 
  
*    Revision:         $Revision: 2 $
 
*    Filename:         $Source: //depot/devdiv/Orcas/feature/element3d/wpf/Test/ElementServices/FeatureTests/Trusted/AvalonMouseSimulation.cs $
******************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

using Microsoft.Test.Win32;
using Microsoft.Test.Threading;

namespace Avalon.Test.CoreUI.Trusted
{
    /// <summary>
    /// Avalon-specific implementation for primitive mouse simulation.
    /// </summary>
    internal partial class AvalonMouseSimulation : MouseSimulation
    {
        /// <summary>
        /// Returns the position for the specied UIElement3D on the specified relative location.       
        /// </summary>
        /// <param name="element3d">UIElement3D where the mouse will be set</param>
        /// <param name="location">Relative postion where the mouse final position will be.</param>
        /// <param name="inside">This will return a position outside of the element. The distance is
        /// defined on the offset proprety. True will be inside.</param>        
        /// <returns>Point</returns>
        public Point GetPoint(UIElement3D element3d, MouseLocation location, bool inside)
        {
            Rect r = ElementUtils.GetScreenRelativeRect(element3d);

            return GetPointInRect(r, location, inside);
        }
    }
}
