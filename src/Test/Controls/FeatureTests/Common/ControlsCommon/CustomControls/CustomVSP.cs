using System;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Microsoft.Test;
using Microsoft.Test.Input;
using Microsoft.Test.Logging;
using Microsoft.Test.Discovery;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Verification;
using Avalon.Test.ComponentModel;
using Avalon.Test.ComponentModel.Actions;
using Avalon.Test.ComponentModel.Utilities;

namespace Avalon.Test.Controls
{
    /// <summary>
    /// A Custom VSP to test the VSP related API changes for WPF DataGrid's Column Virtualization     
    /// </summary>
    public class CustomVSP : VirtualizingStackPanel
    {
        #region Fields

        private Size _oldViewportSize, _newViewportSize;
        private Vector _oldViewportOffset, _newViewportOffset;

        #endregion

        #region Constructor

        /// <summary>
        /// default Constructor
        /// </summary>
        public CustomVSP()
            : base()
        {           
        }

        #endregion

        #region Properties

        //-----------------------------
        // custom property
        //----------------------------- 
        public double CustomWidth
        {
            get { return (double)GetValue(CustomWidthProperty); }
            set { SetValue(CustomWidthProperty, value); }
        }
        public static readonly DependencyProperty CustomWidthProperty =
            DependencyProperty.Register("CustomWidth", typeof(double), typeof(CustomVSP), new FrameworkPropertyMetadata());

        public Size OldViewportSize
        {
            get { return _oldViewportSize; }
            set { _oldViewportSize = value; }
        }
        public Size NewViewportSize
        {
            get { return _newViewportSize; }
            set { _newViewportSize = value; }
        }
        public Vector OldViewportOffset
        {
            get { return _oldViewportOffset; }
            set { _oldViewportOffset = value; }       
        }
        public Vector NewViewportOffset
        {
            get { return _newViewportOffset; }
            set { _newViewportOffset = value; }
        }

        #endregion

        #region Methods

        //-----------------------------
        // Overrides 
        //----------------------------- 
        
        /// <summary>
        /// Called when an item is being re-virtualized       
        /// </summary>
        /// <param name="e"></param>
        protected override void OnCleanUpVirtualizedItem(CleanUpVirtualizedItemEventArgs e)
        {            
            base.OnCleanUpVirtualizedItem(e);
        }        
        
        /// <summary>
        /// communicate the viewport size change
        /// </summary>
        /// <param name="oldViewportSize"></param>
        /// <param name="newViewportSize"></param>
        protected override void OnViewportSizeChanged(Size oldViewportSize, Size newViewportSize) 
        {
            this._oldViewportSize = oldViewportSize;
            this._newViewportSize = newViewportSize;
        }
        
        /// <summary>
        /// communicate the viewport offset change
        /// </summary>
        /// <param name="oldViewportOffset"></param>
        /// <param name="newViewportOffset"></param>
        protected override void OnViewportOffsetChanged(Vector oldViewportOffset, Vector newViewportOffset) 
        {
            this._oldViewportOffset = oldViewportOffset;
            this._newViewportOffset = newViewportOffset;
        }
                
        //-----------------------------
        // IScrollInfo methods 
        //-----------------------------

        public override void PageUp()  
        {
            SetVerticalOffset(VerticalOffset - 20);
        }

        public override void PageDown() 
        {
            SetVerticalOffset(VerticalOffset + 20);
        }

        public override void PageLeft()  
        {
            SetHorizontalOffset(HorizontalOffset - 20);
        }

        public override void PageRight() 
        {
            SetHorizontalOffset(HorizontalOffset + 20);
        }
        
        public override void LineUp() 
        {
            SetVerticalOffset(VerticalOffset - 1);
        }

        public override void LineDown() 
        {
            SetVerticalOffset(VerticalOffset + 1);
        }

        public override void LineLeft()
        {
            SetHorizontalOffset(HorizontalOffset - 1);
        }

        public override void LineRight() 
        {
            SetHorizontalOffset(HorizontalOffset + 1);
        }

        public override void MouseWheelUp() 
        {
            SetVerticalOffset(VerticalOffset - 10); 
        }

        public override void MouseWheelDown() 
        {
            SetVerticalOffset(VerticalOffset + 10);
        }

        public override void MouseWheelLeft() 
        {
            SetHorizontalOffset(HorizontalOffset - 10);
        }

        public override void MouseWheelRight() 
        {
            SetHorizontalOffset(HorizontalOffset + 10);
        }

        #endregion
    }
}
