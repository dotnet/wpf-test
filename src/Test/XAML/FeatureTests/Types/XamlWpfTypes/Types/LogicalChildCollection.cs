// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Diagnostics;
using System.Windows;

namespace Microsoft.Test.Xaml.Types
{
    /// <summary>
    /// A interface the host of LogicalChildCollection should implement.
    /// </summary>
    public interface ILogicalParent
    {
        /// <summary>
        /// The method LogicalChildCollection calls to update the logical tree.
        /// </summary>
        /// <param name="child">The child.</param>
        void CallAddLogicalChild(object child);
    }

    /// <summary>
    /// A class that is a collection to hold children 
    /// </summary>
    public class LogicalChildCollection : ArrayList
    {
        /// <summary>
        /// ILogicalParent parent
        /// </summary>
        private readonly ILogicalParent _parent;

        /// <summary>
        /// bool updateLayout
        /// </summary>
        private readonly bool _updateLayout;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogicalChildCollection"/> class.
        /// </summary>
        public LogicalChildCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogicalChildCollection"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="updateLayout">if set to <c>true</c> [update layout].</param>
        public LogicalChildCollection(ILogicalParent parent, bool updateLayout)
        {
            if (!(parent is UIElement))
            {
                throw new Exception("Parent is not UIELement for InputElementCollection.");
            }

            this._parent = parent;
            this._updateLayout = updateLayout;
        }

        /// <summary>
        /// Method for IList interface.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Object"/> to be added to the end of the <see cref="T:System.Collections.ArrayList"/>. The value can be null.</param>
        /// <returns>
        /// The <see cref="T:System.Collections.ArrayList"/> index at which the <paramref name="value"/> has been added.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.ArrayList"/> is read-only.-or- The <see cref="T:System.Collections.ArrayList"/> has a fixed size. </exception>
        public override int Add(object value)
        {
            if (null == _parent)
            {
                throw new ArgumentNullException("parent");
            }

            if (_updateLayout)
            {
                _parent.CallAddLogicalChild(value);
            }

            return base.Add(value);
        }
    }
}
