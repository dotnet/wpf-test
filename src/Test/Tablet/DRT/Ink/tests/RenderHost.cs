// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
// Description:
//      Defines a control for IncrementalRendering and Static Rendering DRTTest    
//
// Features:
//
//  8/16/2004 Microsoft:       Created 
//
// 


using System;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;

namespace DRT
{

    /// <summary>
    /// class StaticRenderingHost
    /// </summary>
    public class StaticRenderHost : Control
    {
        public StaticRenderHost()
        {
            _children = new VisualCollection(this);                
            _renderer = new Renderer();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if(!_children.Contains(_renderer.RootVisual))
            {
                _children.Add(_renderer.RootVisual);
            }
        }

        public Renderer StaticRenderer
        {
            get
            {
                return _renderer;
            }
        }

        /// <summary>
        ///   Derived class must implement to support Visual children. The method must return
        ///    the child at the specified index. Index must be between 0 and GetVisualChildrenCount-1.
        ///
        ///    By default a Visual does not have any children.
        ///
        ///  Remark: 
        ///       During this virtual call it is not valid to modify the Visual tree. 
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
        ///  Derived classes override this property to enable the Visual code to enumerate 
        ///  the Visual children. Derived classes need to return the number of children
        ///  from this method.
        ///
        ///    By default a Visual does not have any children.
        ///
        ///  Remark: During this virtual method the Visual tree must not be modified.
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

        private Renderer _renderer = null;
    }

    /// <summary>
    /// IncrementalRenderHost
    /// </summary>
    public class IncrementalRenderHost : Control
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public IncrementalRenderHost()
        {
            _children = new VisualCollection(this);                
            CreateIncrementalRenderer();
        }

        /// <summary>
        /// ApplyTemplate
        /// </summary>
        /// <returns></returns>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            
            if (!_children.Contains(_incRenderer.RootVisual))
            {
                _children.Add(_incRenderer.RootVisual);
            }
        }

        /// <summary>
        /// IncRenderer
        /// </summary>
        /// <value></value>
        public IncrementalRenderer IncRenderer
        {
            get
            {
                return _incRenderer;
            }
        }

        /// <summary>
        /// CreateIncrementalRenderer
        /// </summary>
        private void CreateIncrementalRenderer()
        {
            DrawingAttributes da = new DrawingAttributes();

            StylusPacketValueMetric[] props = new StylusPacketValueMetric[2];
            props[0] = new StylusPacketValueMetric(StylusPacketValue.X, Int32.MinValue, Int32.MaxValue, StylusPacketValueUnit.Centimeters, (float)1000.000);
            props[1] = new StylusPacketValueMetric(StylusPacketValue.Y, Int32.MinValue, Int32.MaxValue, StylusPacketValueUnit.Centimeters, (float)1000.000);

            StylusPacketDescription spd = new StylusPacketDescription(props);
            _incRenderer = new IncrementalRenderer(da, spd);
        }

        /// <summary>
        ///   Derived class must implement to support Visual children. The method must return
        ///    the child at the specified index. Index must be between 0 and GetVisualChildrenCount-1.
        ///
        ///    By default a Visual does not have any children.
        ///
        ///  Remark: 
        ///       During this virtual call it is not valid to modify the Visual tree. 
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
        ///  Derived classes override this property to enable the Visual code to enumerate 
        ///  the Visual children. Derived classes need to return the number of children
        ///  from this method.
        ///
        ///    By default a Visual does not have any children.
        ///
        ///  Remark: During this virtual method the Visual tree must not be modified.
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


        private IncrementalRenderer _incRenderer = null;
    }


    /// <summary>
    /// 
    /// </summary>
    public class DrawHost : Control
    {

        public DrawHost()
        {
            _children = new VisualCollection(this);                
            _visual = new ContainerVisual();
        }

        public ContainerVisual RootVisual
        {
            get
            {
                return _visual;
            }
        }
        /// <summary>
        /// ApplyTemplate
        /// </summary>
        /// <returns></returns>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (!_children.Contains(_visual))
            {
                _children.Add(_visual);
            }
        }

        /// <summary>
        ///   Derived class must implement to support Visual children. The method must return
        ///    the child at the specified index. Index must be between 0 and GetVisualChildrenCount-1.
        ///
        ///    By default a Visual does not have any children.
        ///
        ///  Remark: 
        ///       During this virtual call it is not valid to modify the Visual tree. 
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
        ///  Derived classes override this property to enable the Visual code to enumerate 
        ///  the Visual children. Derived classes need to return the number of children
        ///  from this method.
        ///
        ///    By default a Visual does not have any children.
        ///
        ///  Remark: During this virtual method the Visual tree must not be modified.
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
        ContainerVisual _visual;
    }

}

