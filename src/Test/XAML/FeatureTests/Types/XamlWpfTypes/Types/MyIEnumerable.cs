// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace Microsoft.Test.Xaml.Types
{
    /// <summary>
    /// ILogicalTreeParent Interface
    /// </summary>
    internal interface ILogicalTreeParent
    {
        /// <summary>
        /// Adds the logical child.
        /// </summary>
        /// <param name="element">The element.</param>
        void AddLogicalChild(UIElement element);

        /// <summary>
        /// Removes the logical child.
        /// </summary>
        /// <param name="element">The element.</param>
        void RemoveLogicalChild(UIElement element);
    }

    /// <summary>
    /// MyIEnumerable class
    /// </summary>
    [ContentProperty("Children")]
    public class MyIEnumerable : UIElement, IEnumerable, IAddChild
    {
        /// <summary>
        /// VisualCollectionWithIList children
        /// </summary>
        private readonly VisualCollectionWithIList _children;

        /// <summary>
        /// data string
        /// </summary>
        private string _stringprop = "default value";

        /// <summary>
        /// Initializes a new instance of the <see cref="MyIEnumerable"/> class.
        /// </summary>
        public MyIEnumerable()
        {
            _children = new VisualCollectionWithIList(this);
        }

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <value>The children.</value>
        public VisualCollectionWithIList Children
        {
            get
            {
                return _children;
            }
        }

        /// <summary>
        /// Gets or sets the stringprop.
        /// </summary>
        /// <value>The stringprop.</value>
        public string Stringprop
        {
            get
            {
                return _stringprop;
            }

            set
            {
                _stringprop = value;
            }
        }

        /// <summary>
        /// Gets the Visual children count.
        /// </summary>
        /// <value>The visual children count.</value>
        protected override int VisualChildrenCount
        {
            get
            {
                if (_children == null)
                {
                    throw new ArgumentOutOfRangeException("children is null");
                }

                return _children.Count;
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator) _children.GetEnumerator();
        }

        /// <summary>
        /// Add a text string to this control
        /// </summary>
        /// <param name="text">The text value.</param>
        void IAddChild.AddText(string text)
        {
        }

        /// <summary>
        /// Add an object child to this control
        /// </summary>
        /// <param name="o">The object o.</param>
        void IAddChild.AddChild(object o)
        {
            _children.Add((FrameworkElement) o);
        }

        /// <summary>
        /// Returns the child at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>Visual value</returns>
        protected override Visual GetVisualChild(int index)
        {
            if (_children == null)
            {
                throw new ArgumentOutOfRangeException("index is out of range");
            }

            if (index < 0 || index >= _children.Count)
            {
                throw new ArgumentOutOfRangeException("index is out of range");
            }

            return _children[index];
        }
    }

    /// <summary>
    /// VisualCollectionWithIList class
    /// </summary>
    public class VisualCollectionWithIList : Collection<FrameworkElement>
    {
        /// <summary>VisualCollection visuals </summary>
        private readonly VisualCollection _visuals;

        /// <summary> ILogicalTreeParent logicalTreeParent</summary>
        private readonly ILogicalTreeParent _logicalTreeParent;

        /// <summary>UIElement parent </summary>
        private UIElement _parent;

        /// <summary>
        /// Initializes a new instance of the <see cref="VisualCollectionWithIList"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        internal VisualCollectionWithIList(UIElement parent)
        {
            _visuals = new VisualCollection(parent);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VisualCollectionWithIList"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="logicalParent">The logical parent.</param>
        internal VisualCollectionWithIList(UIElement parent, ILogicalTreeParent logicalParent)
            : this(parent)
        {
            this._parent = parent;
            _logicalTreeParent = logicalParent;
        }

        /// <summary>
        /// Insert Item
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The object to insert. The value can be null for reference types.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException"></exception>
        protected override void InsertItem(int index, FrameworkElement item)
        {
            _visuals.Insert(index, item);
            if (_logicalTreeParent != null)
            {
                _logicalTreeParent.AddLogicalChild(item);
            }

            base.InsertItem(index, item);
        }

        /// <summary>
        /// Set Item value
        /// </summary>
        /// <param name="index">The zero-based index of the element to replace.</param>
        /// <param name="item">The new value for the element at the specified index. The value can be null for reference types.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException"></exception>
        protected override void SetItem(int index, FrameworkElement item)
        {
            _visuals[index] = item;
            if (_logicalTreeParent != null)
            {
                _logicalTreeParent.RemoveLogicalChild(this[index]);
                _logicalTreeParent.AddLogicalChild(item);
            }

            base.SetItem(index, item);
        }

        /// <summary>
        /// Remove Item
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">.</exception>
        protected override void RemoveItem(int index)
        {
            _visuals.RemoveAt(index);
            if (_logicalTreeParent != null)
            {
                _logicalTreeParent.RemoveLogicalChild(this[index]);
            }

            base.RemoveItem(index);
        }

        /// <summary>
        /// Clear Items
        /// </summary>
        protected override void ClearItems()
        {
            _visuals.Clear();
            base.ClearItems();
        }
    }
}
