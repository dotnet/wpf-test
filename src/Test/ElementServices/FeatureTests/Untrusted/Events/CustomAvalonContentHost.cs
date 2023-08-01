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
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Avalon.Test.CoreUI.Events
{
    /// <summary>
    ///     CustomAvalonContentHost class is a subclass of FrameworkElement
    /// </summary>
    /// <ExternalAPI/>
    public class CustomAvalonContentHost : FrameworkElement, IContentHost
    {
        #region Construction
        
        /// <summary>
        ///     Constructor for  CustomAvalonContentHost
        /// </summary>
        public CustomAvalonContentHost(Dispatcher context) : base()
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
        
        #region HitTest
        
        /// <summary>
        ///     Hit tests to the correct ContentElement 
        ///     within the ContentHost that the mouse 
        ///     is over
        /// </summary>
        /// <param name="p">
        ///     Mouse coordinates relative to 
        ///     the ContentHost
        /// </param>
        IInputElement IContentHost.InputHitTest(Point p)
        {
            return this;
        }
        
        #endregion HitTest       


        /// <summary>
        /// 



        ReadOnlyCollection<Rect> IContentHost.GetRectangles(ContentElement e){return null;}

        /// <summary>
        /// Returns elements hosted by the content host as an enumerator class
        /// </summary>
        IEnumerator<IInputElement> IContentHost.HostedElements 
        { 
            get
            {
                return null;
            }
        }
        
        /// <summary>
        /// Called when a UIElement-derived class which is hosted by a IContentHost changes it’s DesiredSize
        /// NOTE: This method already exists for this class and is not specially implemented for IContentHost.
        /// If this method is called through IContentHost for this class it will fire an assert
        /// </summary>
        /// <param name="child">
        /// Child element whose DesiredSize has changed
        /// </param> 
        void IContentHost.OnChildDesiredSizeChanged(UIElement child)
        {
            return;
        }

        private VisualCollection _children;         
    }    
}

