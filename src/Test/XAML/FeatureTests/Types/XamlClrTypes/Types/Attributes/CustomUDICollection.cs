// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Markup;

namespace Microsoft.Test.Xaml.Types.Attributes
{
    /// <summary>
    /// CustomUDICollection_Object Test
    /// </summary>
    public class CustomUDICollection_Object : Collection<CustomUDIObject>
    {
        /// <summary>
        /// CustomUDIObjectWCollection owner
        /// </summary>
        private readonly CustomUDIObjectWCollection _owner;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomUDICollection_Object"/> class.
        /// </summary>
        public CustomUDICollection_Object()
            : base()
        {
            _owner = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomUDICollection_Object"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public CustomUDICollection_Object(CustomUDIObjectWCollection owner)
            : base()
        {
            this._owner = owner;
        }

        /// <summary>
        /// Inserts an element into the <see cref="T:System.Collections.ObjectModel.Collection`1"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The object to insert. The value can be null for reference types.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero.-or-<paramref name="index"/> is greater than <see cref="P:System.Collections.ObjectModel.Collection`1.Count"/>.</exception>
        protected override void InsertItem(int index, CustomUDIObject item)
        {
            base.InsertItem(index, item);
            item.LogAction("Adding to Parent");
            if (_owner != null)
            {
                _owner.OrderArray.Add("Adding Child");
            }
        }
    }   
}
