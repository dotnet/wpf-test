// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
 * Purpose: Custimized Canvas which implements INameScope.
 *
 
  
 * Revision:         $Revision: $
 
 * Filename:         $Source: $
********************************************************************/
using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using Microsoft.Test.Discovery;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Controls;
using System.Collections;

namespace Avalon.Test.CoreUI.IdTest
{
    /// <summary>
    /// Custimized Canvas which implements INameScope.
    /// </summary>
    public class CustomINameScope : Canvas, INameScope
    {
        private Hashtable _idScopeMap = new Hashtable();

        #region INameScope
        /// <summary>
        /// Registers the id - Context combination
        /// </summary>
        /// <param name="id">Id to register</param>
        /// <param name="context">Element where id is defined</param>
        new public void RegisterName(string id, object context)
        {
            _idScopeMap.Add(id, context);
        }

        /// <summary>
        /// Unregisters the id - element combination
        /// </summary>
        /// <param name="id">Id of the element</param>
        new public void UnregisterName(string id)
        {
            _idScopeMap.Remove(id);
        }

        /// <summary>
        /// Find the element given id
        /// </summary>
        /// <param name="id">Id of the element</param>
        object INameScope.FindName(string id)
        {
            return _idScopeMap[id];
        }

        /// <summary>
        /// Number of items in this dictionary.
        /// </summary>
        public int ElementWithIDCount
        {
            get
            {
                return _idScopeMap.Count;
            }
        }

        #endregion INameScope
    }
}
