// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
 
  
 * Revision:         $Revision: $
 
 * Filename:         $Source: $
********************************************************************/
using System;
using System.Windows;

namespace Avalon.Test.CoreUI.Serialization.Security
{
    #region Class FrameworkElementWithShouldSerializeCLR
    /// <summary>
    /// This class defines custom FrameworkElement with a private ShouldSerialize function for clr property.
    /// </summary>
    public class FrameworkElementWithShouldSerializeCLR : FrameworkElement
    {
        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public FrameworkElementWithShouldSerializeCLR() : base()
        {
        }
        #endregion Constructor  

        #region ShouldClrProperty
        private string _ShouldClrProperty = "ShouldBeSerialized";

        /// <summary>
        /// Private ShouldSerialize function.
        /// </summary>
        /// <returns></returns>
        private bool ShouldSerializeShouldClrProperty()
        {
            return true;
        }

        /// <summary>
        /// The Clr property with private ShouldSerialize function.
        /// </summary>
        /// <value></value>
        public string ShouldClrProperty
        {
            get
            {
                return _ShouldClrProperty;
            }
            set
            {
                _ShouldClrProperty = value;
            }
        }
        #endregion ShouldClrProperty
    }
    #endregion Class FrameworkElementWithShouldSerializeCLR
}
