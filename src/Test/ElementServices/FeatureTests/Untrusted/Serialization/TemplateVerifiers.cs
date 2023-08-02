// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Holds verification routines for Template parsing and serialization tests.
 * Contributors: Microsoft
 *
 
  
 * Revision:         $Revision: 1 $
 
********************************************************************/

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

namespace Avalon.Test.CoreUI.TestCases.Serialization
{
	/// <summary>
    /// Holds verification routines for Template parsing and serialization tests.
    /// </summary>
    public class TemplateVerifiers
    {
        /// <summary>
        /// Verifies PropertyTag syntax works in Template.
        /// </summary>
        public static void Template_PropertyTagSyntax(UIElement root)
        {
            Control element = (Control)(((FrameworkElement)root).FindName("element1"));

            //
            // Verify element property value set in Template.
            //
            CoreLogger.LogStatus("Verifying Element's property...");

            if (element.Background == Brushes.Red)
            {
                throw new Microsoft.Test.TestValidationException("Element's Background != Brushes.Red.");
			}
		}
	}
}

