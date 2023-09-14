// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Abstracts core-level display/presentation operations.  
 *          Those include displaying the tree or reading or modifying 
 *          the presentation surface, i.e. hwnd or whatever. The 
 *          intention of abstracting the operations is to reduce the 
 *          effect of future API changes in the product's core level.
 *
 
  
 * Revision:         $Revision: 2 $
 
 * Filename:         $Source: //depot/devdiv/Orcas/feature/element3d/wpf/Test/ElementServices/FeatureTests/Trusted/PresentationHelper.cs $
********************************************************************/
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Windows.Media;

using Microsoft.Test.Win32;

namespace Avalon.Test.CoreUI.Trusted
{
    /// <summary>
    /// Abstraction of core-level display/presentation operations
    /// </summary>
    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Assert, Name = "FullTrust")]
    public partial class PresentationHelper
    {
        /// <summary>
        /// Returns a PresentationSource from a dependency object - new in 3.5.
        /// </summary>
        public static PresentationSource FromDependencyObject(DependencyObject dobj)
        {
            if (dobj == null)
            {
                throw new ArgumentNullException("dobj");
            }
         
            return PresentationSource.FromDependencyObject(dobj);
        }
        
    }
}

