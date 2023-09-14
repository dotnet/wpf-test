// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
 * Purpose: Holds verification routines for HyphenDictionary serialization tests.
 * Contributors: Microsoft
 *
 
  
 * Revision:         $Revision: 1 $
 
********************************************************************/
using System;
using System.Windows;
using System.Windows.Documents;
using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Parser;

namespace Avalon.Test.CoreUI.Serialization
{
    /// <summary>
    /// Holds verification routines for HyphenDictionary serialization tests.
    /// </summary>
	public class HyphenationDictionaryVerifier
    {
		/// <summary>
        /// Verifies that a HyphenationDictionary instance may be retrieved from a resource.
		/// </summary>
		public static void Verify1(UIElement uiElement)
        {
            FrameworkElement fe = (FrameworkElement)uiElement;

            HyphenationDictionary dict = (HyphenationDictionary)fe.FindResource("MyHyphenationDictionary");

            if (dict == null)
            {
                throw new Microsoft.Test.TestValidationException("Couldn't find MyHyphenationDictionary resource.");
            }
		}

    }
}
