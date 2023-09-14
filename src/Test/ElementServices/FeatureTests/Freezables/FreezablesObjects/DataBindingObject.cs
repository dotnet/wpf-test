// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Security;
using System.Security.Policy;
using System.Windows;
using System.Windows.Media;


namespace Microsoft.Test.ElementServices.Freezables
{
    /**********************************************************************************
    * CLASS:          PenDataBinding
    **********************************************************************************/
    public class PenDataBinding : DataBindingBase
    {
        private SolidColorBrush _brush;
  
        /******************************************************************************
        * Function:          UpdateFreezableProperty
        ******************************************************************************/
        public override void UpdateFreezableProperty()
        {
            DataProperty = new SolidColorBrush(Colors.White);
        }
  
        //---------------------------------------------------------------
        public override DependencyProperty DP
        {
            get
            {
                return Pen.BrushProperty;
            }
        }
        //-------------------------------------------------
        
        public PenDataBinding()
        {
            _brush = new SolidColorBrush(Colors.Blue);
        }

        //-------------------------------------------------
        public SolidColorBrush DataProperty
        {
            get
            { 
                return _brush; 
            }
            set
            {
                _brush = value;
                OnPropertyChanged("DataProperty");
            }
        }
    }


    /**********************************************************************************
    * CLASS:          GeometryDataBinding
    **********************************************************************************/
    public class GeometryDataBinding : DataBindingBase
    {
        private Transform _transform;
       
        /******************************************************************************
        * Function:          UpdateFreezableProperty
        ******************************************************************************/
        public override void UpdateFreezableProperty()
        {
            DataProperty = new TranslateTransform(22, 33);
        }
        //-------------------------------------------------
        public override DependencyProperty DP
        {
            get
            {
                return Geometry.TransformProperty;
            }
        }
    
        //-------------------------------------------------
        public GeometryDataBinding()
        {
            _transform = new TranslateTransform(20, 20);
        }

        //-------------------------------------------------
        public Transform DataProperty
        {
            get
            {
                return _transform;
            }
            set
            {
                _transform = value;
                OnPropertyChanged("DataProperty");
            }
        }
    }


    /**********************************************************************************
    * CLASS:          BrushDataBinding
    **********************************************************************************/
    public class BrushDataBinding : DataBindingBase
    {
        private double _opacity;

        /******************************************************************************
        * Function:          UpdateFreezableProperty
        ******************************************************************************/
        public override void UpdateFreezableProperty()
        {
            DataProperty = 0.6;
        }
        
        //------------------------------------------------
        public override DependencyProperty DP
        {
            get
            {
                return Brush.OpacityProperty;
            }
        }

        //-------------------------------------------------
        public BrushDataBinding()
        {
            _opacity = 0.4;
        }

        //-------------------------------------------------
        public double DataProperty
        {
            get
            {
                return _opacity;
            }
            set
            {
                _opacity = value;
                OnPropertyChanged("DataProperty");
            }
        }
    }

    
    /**********************************************************************************
    * CLASS:          DataBindingBase
    **********************************************************************************/
    public abstract class DataBindingBase : INotifyPropertyChanged
    {
        // Declare event
        public event PropertyChangedEventHandler PropertyChanged;

        /******************************************************************************
        * Function:          OnPropertyChanged
        ******************************************************************************/
        // OnPropertyChanged event handler to update property value in binding
        public void OnPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        /******************************************************************************
        * Function:          UpdateFreezableProperty
        ******************************************************************************/
        public abstract void UpdateFreezableProperty();

        public abstract DependencyProperty DP
        {
            get;
        }

    }
}
  


