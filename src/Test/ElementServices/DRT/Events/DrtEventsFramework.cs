// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// 
//
// Description: Framework used by DrtEvents
//
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

using System.Windows.Media;

namespace DRT
{
    /// <summary>
    ///     CustomControl class is a subclass of Control
    /// </summary>
    public class CustomControl : Control
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
        ///     Returns enumerator to logical children
        /// </summary>
        protected override IEnumerator LogicalChildren
        {
            get { return _logicalChildren.GetEnumerator(); }
        }

        /// <summary>
        ///     Appends model child
        /// </summary>
        public void AppendModelChild(DependencyObject modelChild)
        {
            AddLogicalChild(modelChild);
            _logicalChildren.Add(modelChild);
        }
        
        /// <summary>
        /// Appends a child.
        /// </summary>
        public void AppendChild(Visual child)
        {
            _children.Add(child);
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
                //you can either have a Template or your own children
                if(base.VisualChildrenCount > 0) return 1;
                else return  _children.Count; 
            }            
        }      

        #endregion External API

        #region Data
        private VisualCollection _children;        
        private ArrayList _logicalChildren = new ArrayList(1);
        
        #endregion Data
    
    }    

    /// <summary>
    ///     CustomSubControl class is a subclass of CustomControl
    /// </summary>
    /// <ExternalAPI/>
    public class CustomSubControl : CustomControl
    {
        #region Construction
    
        /// <summary>
        ///     Constructor for  CustomSubControl
        /// </summary>
        public CustomSubControl() : base()
        {
        }
        
        /// <summary>
        ///     Static Constructor for  CustomSubControl
        /// </summary>
        static CustomSubControl()
        {
            // Fetch RoutedEvent for bubble event           
            RoutedEvent routedEventBubble = DrtEvents.RoutedEvent1;
            
            // Fetch RoutedEvent for tunnel event           
            RoutedEvent routedEventTunnel = DrtEvents.RoutedEvent2;
            
            EventManager.RegisterClassHandler(typeof(CustomSubControl), routedEventBubble, new CustomRoutedEventHandler(DrtEvents.OnStaticRoutedEvent));
            EventManager.RegisterClassHandler(typeof(CustomSubControl), routedEventTunnel, new CustomRoutedEventHandler(DrtEvents.OnStaticRoutedEvent));
        }        
    
        #endregion Construction                
    }    
    
    /// <summary>
    ///     CustomSubSubControl class is a subclass of CustomControl
    /// </summary>
    public class CustomSubSubControl : CustomSubControl
    {
        #region Construction
    
        /// <summary>
        ///     Constructor for  CustomSubSubControl
        /// </summary>
        public CustomSubSubControl() : base()
        {
        }
        
        /// <summary>
        ///     Static Constructor for  CustomSubSubControl
        /// </summary>
        static CustomSubSubControl()
        {
            // Fetch RoutedEvent for bubble event           
            RoutedEvent routedEventBubble = DrtEvents.RoutedEvent1;
            
            // Fetch RoutedEvent for tunnel event           
            RoutedEvent routedEventTunnel = DrtEvents.RoutedEvent2;
            
            EventManager.RegisterClassHandler(typeof(CustomSubSubControl), routedEventBubble, new CustomRoutedEventHandler(DrtEvents.OnStaticRoutedEvent));
            EventManager.RegisterClassHandler(typeof(CustomSubSubControl), routedEventTunnel, new CustomRoutedEventHandler(DrtEvents.OnStaticRoutedEvent));
            EventManager.RegisterClassHandler(typeof(CustomSubSubControl), routedEventBubble, new CustomRoutedEventHandler(DrtEvents.OnStaticRoutedEvent), true);
            EventManager.RegisterClassHandler(typeof(CustomSubSubControl), routedEventTunnel, new CustomRoutedEventHandler(DrtEvents.OnStaticRoutedEvent), true);
        }        
    
        #endregion Construction                
    }    

    public class CustomContentElement : FrameworkContentElement
    {
        #region Construction
        
        /// <summary>
        ///     Constructor for  CustomContentElement
        /// </summary>
        public CustomContentElement() : base()
        {
        }
        
        #endregion Construction
                
        #region External API
        
        /// <summary>
        ///     Appends model child
        /// </summary>
        public void AppendModelChild(DependencyObject modelChild)
        {
            AddLogicalChild(modelChild);
        }

        #endregion External API
        
    }
    
    /// <summary>
    ///     CustomSubContentElement class is a subclass of CustomContentElement
    /// </summary>
    public class CustomSubContentElement : CustomContentElement
    {
        #region Construction
    
        /// <summary>
        ///     Constructor for  CustomSubContentElement
        /// </summary>
        public CustomSubContentElement() : base()
        {
        }
        
        /// <summary>
        ///     Static Constructor for  CustomSubContentElement
        /// </summary>
        static CustomSubContentElement()
        {
            // Fetch RoutedEvent for bubble event           
            RoutedEvent routedEventBubble = DrtEvents.RoutedEvent1;
            
            // Fetch RoutedEvent for tunnel event           
            RoutedEvent routedEventTunnel = DrtEvents.RoutedEvent2;
            
            EventManager.RegisterClassHandler(typeof(CustomSubContentElement), routedEventBubble, new CustomRoutedEventHandler(DrtEvents.OnStaticRoutedEvent));
            EventManager.RegisterClassHandler(typeof(CustomSubContentElement), routedEventTunnel, new CustomRoutedEventHandler(DrtEvents.OnStaticRoutedEvent));
        }        
    
        #endregion Construction                
    }    
    
    /// <summary>
    ///     CustomContentHost class is a subclass of ContentHost
    /// </summary>
    public class CustomContentHost : FrameworkElement, IContentHost
    {
        #region Construction
        
        /// <summary>
        ///     Constructor for  CustomContentHost
        /// </summary>
        public CustomContentHost() : base()
        {
            _children = new VisualCollection(this);         
        }
        
        #endregion Construction
                
        #region External API
        
        /// <summary>
        ///     Appends model child
        /// </summary>
        public void AppendModelChild(DependencyObject modelChild)
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

        #endregion External API

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

        
        #region HitTest
        
        /// <summary>
        ///     Hit tests to the correct ContentElement 
        ///     within the ContentHost that the mouse 
        ///     is over
        /// </summary>
        /// <param name="point">
        ///     Mouse coordinates relative to 
        ///     the ContentHost
        /// </param>
        IInputElement IContentHost.InputHitTest(Point point)
        {
            return this;
        }
        
        // Not implemented
        ReadOnlyCollection<Rect> IContentHost.GetRectangles(ContentElement child)
        {
            throw new NotImplementedException("[IContentHost.GetRectangles] not implemented!");
        }

        // Not implemented
        IEnumerator<IInputElement> IContentHost.HostedElements
        {
            get
            {
                throw new NotImplementedException("[IContentHost.HostedElements] not implemented!");
            }
        }

        // Not implemented
        void IContentHost.OnChildDesiredSizeChanged(UIElement child)
        {
            throw new NotImplementedException("[IContentHost.OnChildDesiredSizeChanged] not implemented!");
        }

        #endregion HitTest        
        
    }   
    
    /// <summary>
    ///     CustomSubContentHost class is a subclass of CustomContentHost
    /// </summary>
    public class CustomSubContentHost : CustomContentHost
    {
        #region Construction
    
        /// <summary>
        ///     Constructor for  CustomSubContentHost
        /// </summary>
        public CustomSubContentHost() : base()
        {
        }
        
        /// <summary>
        ///     Static Constructor for  CustomSubContentHost
        /// </summary>
        static CustomSubContentHost()
        {
            // Fetch RoutedEvent for bubble event           
            RoutedEvent routedEventBubble = DrtEvents.RoutedEvent1;
            
            // Fetch RoutedEvent for tunnel event           
            RoutedEvent routedEventTunnel = DrtEvents.RoutedEvent2;
            
            EventManager.RegisterClassHandler(typeof(CustomSubContentHost), routedEventBubble, new CustomRoutedEventHandler(DrtEvents.OnStaticRoutedEvent));
            EventManager.RegisterClassHandler(typeof(CustomSubContentHost), routedEventTunnel, new CustomRoutedEventHandler(DrtEvents.OnStaticRoutedEvent));
        }        
    
        #endregion Construction                
    }    

    /// <summary>
    ///     FooClass class
    /// </summary>
    public class FooClass
    {
        #region Construction
        
        /// <summary>
        ///     Constructor for  FooClass
        /// </summary>
        public FooClass()
        {
        }
        
        #endregion Construction
    }    

    public class FooSubClass : FooClass
    {
        #region Construction
        
        /// <summary>
        ///     Constructor for  FooSubClass
        /// </summary>
        public FooSubClass() : base()
        {
        }
        
        #endregion Construction
    }

    /// <summary>
    ///     StrangeControl class is a subclass of Control
    /// </summary>
    public class StrangeControl : FrameworkElement
    {
        #region Construction
        
        /// <summary>
        ///     Constructor for  StrangeControl
        /// </summary>
        public StrangeControl() : base()
        {
            _children = new VisualCollection(this);
        
        }
        
        #endregion Construction
                
        #region External API
        
        /// <summary>
        ///     Appends model child
        /// </summary>
        public void AppendModelChild(DependencyObject modelChild)
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


        #endregion External API
    
    }  

    public class InitializedPanel : Panel
    {
        #region Construction

        public InitializedPanel() : base()
        {
            Initialized += new EventHandler(TestSuite9.OnInitialized);
        }

        #endregion Construction
    }
}


