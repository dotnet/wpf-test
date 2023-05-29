// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
 * Purpose: Custom types that are built within a project, so they are
 *          locally-defined.  They are used for testing locally-defined
 *          components in xaml, including Styles and Templates.
 * 
 * Contributors: Microsoft
 *
********************************************************************/
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Collections.Generic;
using Avalon.Test.CoreUI.Serialization;

[assembly: XmlnsDefinition(
   "loc",
   "LocallyDefined")]
namespace LocallyDefined
{
    #region Class LocallyDefinedButton
    public class LocallyDefinedButton : LocallyDefinedButtonBase
    {
        public static readonly DependencyProperty LocallyDefinedPropertyProperty = 
            DependencyProperty.Register("LocallyDefinedProperty", 
                                        typeof(string),
                                        typeof(LocallyDefinedButton),
                                        new FrameworkPropertyMetadata("NotSet",
                                                                      new PropertyChangedCallback(OnLocallyDefinedPropertyChagned)));

        public static readonly RoutedEvent LocallyDefinedPropertyChangedEventID = 
            EventManager.RegisterRoutedEvent("LocallyDefinedPropertyChanged",
                                               RoutingStrategy.Bubble,
                                               typeof(RoutedEventHandler),
                                               typeof(LocallyDefinedButton));

        public event RoutedEventHandler LocallyDefinedPropertyChanged
        {
            add { AddHandler(LocallyDefinedPropertyChangedEventID, value); }
            remove { RemoveHandler(LocallyDefinedPropertyChangedEventID, value); }
        }

        public static readonly RoutedEvent LocallyDefinedEventID =
            EventManager.RegisterRoutedEvent("LocallyDefinedEvent",
                                       RoutingStrategy.Tunnel,
                                       typeof(RoutedEventHandler),
                                       typeof(LocallyDefinedButton));

        public event RoutedEventHandler LocallyDefinedEvent
        {
            add { AddHandler(LocallyDefinedEventID, value); }
            remove { RemoveHandler(LocallyDefinedEventID, value); }
        }

        public string LocallyDefinedProperty
        {
            get
            {
              return (string)GetValue(LocallyDefinedPropertyProperty);           
            }
            set
            {
                SetValue(LocallyDefinedPropertyProperty, value);
            }
        }


        private static void OnLocallyDefinedPropertyChagned(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LocallyDefinedButton cb = d as LocallyDefinedButton;
            string oldLocallyDefinedProperty = (string)e.OldValue;
            string newLocallyDefinedProperty = (string)e.NewValue;

            if (oldLocallyDefinedProperty != null)
            {
                RoutedEventArgs ev = new RoutedEventArgs();
                ev.RoutedEvent = LocallyDefinedPropertyChangedEventID;
                ev.Source = cb;
                cb.RaiseEvent(ev);
            }
        }

        public bool EventInvoked
        {
            set
            {
                _eventInvoked = value;
            }
            get
            {
                return _eventInvoked;
            }
        }

        private bool _eventInvoked = false;
    }
    #endregion Class LocallyDefinedButton

    #region Class CustomColorBlenderExtension
    /// <summary>
    /// A custom MarkupExtension.
    /// Adds various colors to a brush and returns the blended brush
    /// </summary>
    [ContentProperty("Mixers")]
    [MarkupExtensionReturnType(typeof(SolidColorBrush))]
    internal class CustomColorBlenderExtension : MarkupExtension
    {
        #region Public constructors
        /// <summary>
        /// Default ctor
        /// </summary>
        public CustomColorBlenderExtension()
            : base()
        {
            _core = Brushes.Black;
            _additive = Colors.Black;
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="core">Core brush</param>
        public CustomColorBlenderExtension(Brush core)
            : base()
        {
            _core = (null == core ? Brushes.Black : core);
            _additive = Colors.Black;
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="additive">Additive color</param>
        public CustomColorBlenderExtension(Color additive)
            : base()
        {
            _core = Brushes.Black;
            _additive = additive; //Color is a struct, so can't be null.
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="core">Core brush</param>
        /// <param name="additive">Additive color</param>
        public CustomColorBlenderExtension(Brush core, Color additive)
            : base()
        {
            _core = (null == core ? Brushes.Black : core);
            _additive = additive; // Color is a struct, so cannot be null
        }
        #endregion Public constructors

        #region Public methods
        /// <summary>
        /// Return the value of the blended brush
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            // Add the additive color and mixers to the core brush
            Color coreColor = (_core as SolidColorBrush).Color;
            Color resultColor = coreColor + _additive;
            if (null != _mixers)
            {
                for (int i = 0; i < _mixers.Count; i++)
                {
                    resultColor = resultColor + _mixers[i];
                }
            }
            return new SolidColorBrush(resultColor);
        }
        #endregion Public methods

        #region Public properties
        /// <summary>
        /// Core brush.
        /// </summary>
        /// <value></value>
        public Brush Core
        {
            get { return _core; }
            set { _core = (null == value ? Brushes.Black : value); }
        }

        /// <summary>
        /// Color to be added to the core brush
        /// </summary>
        /// <value></value>        
        public Color Additive
        {
            get { return _additive; }
            set { _additive = value; } // Color is a struct, so cannot be null
        }

        /// <summary>
        /// Other colors to be added to the core brush.
        /// </summary>
        public List<Color> Mixers
        {
            get
            {
                if (null == _mixers)
                {
                    _mixers = new List<Color>();
                }
                return _mixers;
            }
        }
        #endregion Public properties

        #region Private variables
        private Brush _core;
        private Color _additive;
        private List<Color> _mixers;
        #endregion Private variables
    }
    #endregion Class CustomColorBlenderExtension
}
