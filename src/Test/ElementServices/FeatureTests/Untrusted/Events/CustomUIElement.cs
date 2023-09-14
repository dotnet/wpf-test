// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Microsoft.Test;
using Avalon.Test.CoreUI;
using System.Windows;
using System.Threading;
using System.Windows.Threading;

using System.Windows.Media;
using System.Collections;

namespace Avalon.Test.CoreUI.Events
{
    /// <summary>
	///     CustomUIElement class is a subclass of UIElement with functions to append and remove Visual children 
    /// </summary>
	public class CustomUIElement : UIElement
    {
        #region Construction
        
        /// <summary>
        ///     Constructor for  CustomControl
        /// </summary>
        public CustomUIElement() : base()
        {
            _children = new VisualCollection(this);            
        }
        
        #endregion Construction
                
        #region External API
        
        /// <summary>
        /// Appends a child.
        /// </summary>
        public void AppendChild(Visual child)
        {
            _children.Add(child);
        }
        /// <summary>
        /// Remove a child.
        /// </summary>
        public void RemoveChild(Visual child)
        {
            _children.Remove(child);
        }
        #endregion External API

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


        private VisualCollection _children;         
    }    
}

