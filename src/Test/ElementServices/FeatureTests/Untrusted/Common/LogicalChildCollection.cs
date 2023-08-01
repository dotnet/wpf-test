// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Windows;
using System.Collections;

namespace Avalon.Test.CoreUI.CoreInput.Common.Controls
{
    /// <summary>
    /// A class that is a collection to hold children 
    /// </summary>
    public class LogicalChildCollection : ArrayList
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
       public LogicalChildCollection()
       {
       }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="UpdateLayout"></param>
        public LogicalChildCollection(ILogicalParent parent, bool UpdateLayout)
        {
            Debug.Assert(parent is UIElement, "Parent is not UIELement for InputElementCollection.");
            _parent = parent;
            _updateLayout = UpdateLayout;
        }

        /// <summary>
        /// Method for IList interface.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override int Add(object value)
        {
            Debug.Assert(null != _parent, "_parent is null.");
            if (_updateLayout)
                _parent.CallAddLogicalChild(value);
            return base.Add(value);
        }


        ILogicalParent _parent;
        bool _updateLayout;
    }

    /// <summary>
    /// A interface the host of LogicalChildCollection should implement.
    /// </summary>
    public interface ILogicalParent
    {
        /// <summary>
        /// The method LogicalChildCollection calls to update the logical tree.
        /// </summary>
        /// <param name="child"></param>
        void CallAddLogicalChild(object child);
    }
}
