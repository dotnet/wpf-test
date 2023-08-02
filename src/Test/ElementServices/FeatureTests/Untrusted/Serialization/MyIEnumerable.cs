// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Markup;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System;

namespace Avalon.Test.CoreUI.Serialization
{
	/// <summary>
	/// 
	/// </summary>
        [ContentProperty("Children")]
	public class MyIEnumerable: UIElement, IEnumerable, IAddChild
    {
		/// <summary>
		/// 
		/// </summary>
		public MyIEnumerable()
		{
                    _children = new VisualCollectionWithIList(this);		
		}


                /// <summary>
                /// Returns the child at the specified index.
                /// </summary>
                protected override Visual GetVisualChild(int index)
                {            
                    if(_children == null)
                    {
                        throw new ArgumentOutOfRangeException("index is out of range");
                    }
                    if(index < 0 || index >= _children.Count)
                    {
                        throw new ArgumentOutOfRangeException("index is out of range");
                    }

                    return _children[index];
                }

                /// <summary>
                /// Returns the Visual children count.
                /// </summary>   
                protected override int VisualChildrenCount
                {           
                    get 
                    { 
                        if(_children == null)
                        {
                            throw new ArgumentOutOfRangeException("_children is null");
                        }                
                        return _children.Count; 
                    }
                }              
        
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return (IEnumerator)_children.GetEnumerator();
		}
		
		/// <summary>
		/// Children Property
		/// </summary>
                public VisualCollectionWithIList Children
                {
                    get { return _children; }
                }

        #region IAddChild

		/// <summary>
		///  Add a text string to this control
		/// </summary>
		void IAddChild.AddText(string text)
		{
		}

		/// <summary>
		///  Add an object child to this control
		/// </summary>
		void IAddChild.AddChild(object o)
            {
                _children.Add((FrameworkElement)o);
            }

        #endregion IAddChild

		/// <summary>
		/// 
		/// </summary>
		/// <value></value>
		public string stringprop {
			get {return _stringprop;}
			set { _stringprop = value; }
		}

              private VisualCollectionWithIList _children;
		string _stringprop = "default value";
	}
    internal interface ILogicalTreeParent
    {
        void AddLogicalChild(UIElement element);
        void RemoveLogicalChild(UIElement element);
    }

    /// <summary>
    /// VisualCollectionWithIList class
    /// </summary>
    public class VisualCollectionWithIList : Collection<FrameworkElement>
    {
        private VisualCollection _visuals;
        private UIElement _parent;
        private ILogicalTreeParent _logicalTreeParent;

        internal VisualCollectionWithIList(UIElement parent)
        {
            _visuals = new VisualCollection(parent);
        }

        internal VisualCollectionWithIList(UIElement parent, ILogicalTreeParent logicalParent)
            : this(parent)
        {
            _parent = parent;
            _logicalTreeParent = logicalParent;
        }

    /// <summary>
    /// InsertItem
    /// </summary>
        protected override void InsertItem(int index, FrameworkElement item)
        {
            _visuals.Insert(index, item);
            if (_logicalTreeParent != null)
                _logicalTreeParent.AddLogicalChild(item);
            base.InsertItem(index, item);
        }

    /// <summary>
    /// SetItem
    /// </summary>
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
    /// RemoveItem
    /// </summary>
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
    /// ClearItems
    /// </summary>
        protected override void ClearItems()
        {
            _visuals.Clear();
            base.ClearItems();
        }
    }
}

