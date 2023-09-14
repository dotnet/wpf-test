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
    ///     CustomControl class is a subclass of FrameworkElement
    /// </summary>
    public class CustomControl : FrameworkElement
    {
        #region Construction
        
        /// <summary>
        ///     Constructor for  CustomControl
        /// </summary>
        public CustomControl() : base()
        {
            _children = new VisualCollection(this);              
        }
        
        #endregion Construction
                
        #region External API
        
        /// <summary>
        ///     Appends model child
        /// </summary>
        public void AppendModelChild(object modelChild)
        {
            AddLogicalChild(modelChild);
        }
        
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

        /// <summary>
        /// Returns the child at the specified index.
        /// </summary>
        protected override Visual GetVisualChild(int index)
        {
            // if you have a template
            if(base.VisualChildrenCount != 0 && index == 0)
            {
                return base.GetVisualChild(0);
            }            
            // otherwise you can have your own children
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
                //you can either have a Template or your own children
                if(base.VisualChildrenCount > 0) return 1;
                else return  _children.Count; 
            }            
        }          
        #endregion External API
        
        private VisualCollection _children;  
	}
	/// <summary>
	/// 
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="args"></param>
	/// <param name="other"></param>
	public delegate void CustomDelegate(object sender, EventArgs args, object other);
}

