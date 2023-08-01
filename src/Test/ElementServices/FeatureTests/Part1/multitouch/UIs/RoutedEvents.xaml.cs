// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Test.Input.MultiTouch;

namespace Microsoft.Test.Input.MultiTouch.Tests
{
    /// <summary>
    /// Interaction logic for RoutedEvents.xaml
    /// </summary>
    public partial class RoutedEvents : Window
    {
        #region Private Fields

        static readonly FontFamily s_fontfam = new FontFamily("Lucida Console");
        const string strFormat = "{0,-30} {1,-15}{2,-15} {3,-15}";
        StackPanel _stackOutput;
        DateTime _dtLast;

        #endregion

        #region Constructor

        public RoutedEvents()
        {
            InitializeComponent();

            TestWindow.Loaded += new RoutedEventHandler(OnLoaded);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Build the UI tree
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnLoaded(object sender, RoutedEventArgs e)
        {
            Grid grid = new Grid();
            TestWindow.Content = grid; 

            RowDefinition rowdef = new RowDefinition();
            rowdef.Height = GridLength.Auto;
            grid.RowDefinitions.Add(rowdef);
            rowdef = new RowDefinition();
            rowdef.Height = GridLength.Auto;
            grid.RowDefinitions.Add(rowdef);
            rowdef = new RowDefinition();
            rowdef.Height = new GridLength(100, GridUnitType.Star);
            grid.RowDefinitions.Add(rowdef);
                        
            Button btn = new Button();
            btn.HorizontalAlignment = HorizontalAlignment.Center;
            btn.Margin = new Thickness(16);
            btn.Padding = new Thickness(16);
            grid.Children.Add(btn);
            
            TextBlock text = new TextBlock();
            text.FontSize = 24;
            text.Text = TestWindow.Title;
            btn.Content = text;
            
            TextBlock textHeadings = new TextBlock();
            textHeadings.FontFamily = s_fontfam;
            textHeadings.Inlines.Add(new Underline(new Run(
                String.Format(strFormat, 
                "Routed Event", "sender", "Source", "OriginalSource"))));
            grid.Children.Add(textHeadings);
            Grid.SetRow(textHeadings, 1);
            
            ScrollViewer scroll = new ScrollViewer();
            grid.Children.Add(scroll);
            Grid.SetRow(scroll, 2);
            
            // add the stackpanel 
            _stackOutput = new StackPanel();
            scroll.Content = _stackOutput;
            
            // events
            UIElement[] els = { TestWindow, grid, btn, text };            
            foreach (UIElement el in els)
            {
                // keyboard
                el.PreviewKeyDown += AllPurposeEventHandler;
                el.PreviewKeyUp += AllPurposeEventHandler;
                el.PreviewTextInput += AllPurposeEventHandler;
                el.KeyDown += AllPurposeEventHandler;
                el.KeyUp += AllPurposeEventHandler;
                el.TextInput += AllPurposeEventHandler;

                // mouse 
                el.MouseDown += AllPurposeEventHandler;
                el.MouseUp += AllPurposeEventHandler;
                el.PreviewMouseDown += AllPurposeEventHandler;
                el.PreviewMouseUp += AllPurposeEventHandler;
                
                // stylus                
                el.StylusDown += AllPurposeEventHandler;
                el.StylusUp += AllPurposeEventHandler;
                el.PreviewStylusDown += AllPurposeEventHandler;
                el.PreviewStylusUp += AllPurposeEventHandler;

                // Touch
                el.PreviewTouchDown += AllPurposeEventHandler;
                el.PreviewTouchMove += AllPurposeEventHandler;
                el.PreviewTouchUp += AllPurposeEventHandler;
                el.GotTouchCapture += AllPurposeEventHandler;
                el.LostTouchCapture += AllPurposeEventHandler;
                el.TouchEnter += AllPurposeEventHandler;
                el.TouchLeave += AllPurposeEventHandler;
                el.TouchDown += AllPurposeEventHandler;
                el.TouchMove += AllPurposeEventHandler;
                el.TouchUp += AllPurposeEventHandler;

                // Manipulations
                el.ManipulationStarting += new EventHandler<ManipulationStartingEventArgs>(el_ManipulationStarting);
                el.ManipulationStarted += AllPurposeEventHandler;
                el.ManipulationDelta += AllPurposeEventHandler;
                el.ManipulationInertiaStarting += AllPurposeEventHandler;
                el.ManipulationCompleted += AllPurposeEventHandler;
                el.ManipulationBoundaryFeedback += AllPurposeEventHandler;

                // click                
                el.AddHandler(Button.ClickEvent, new RoutedEventHandler(AllPurposeEventHandler));
            }
        }

        void el_ManipulationStarting(object sender, ManipulationStartingEventArgs e)
        {
            e.Mode = ManipulationModes.All;
        }    
    
        void AllPurposeEventHandler(object sender, RoutedEventArgs args)
        {
            // add a blank line after 100 ms
            DateTime dtNow = DateTime.Now;
            if (dtNow - _dtLast > TimeSpan.FromMilliseconds(100))
            {
                _stackOutput.Children.Add(new TextBlock(new Run(" ")));
            }
            _dtLast = dtNow;
           
            // display the event info
            TextBlock text = new TextBlock();
            text.FontFamily = s_fontfam;
            text.Text = String.Format(strFormat,
                                       args.RoutedEvent.Name,
                                       TypeWithoutNamespace(sender),
                                       TypeWithoutNamespace(args.Source),
                                       TypeWithoutNamespace(args.OriginalSource));

            _stackOutput.Children.Add(text);
            (_stackOutput.Parent as ScrollViewer).ScrollToBottom();          
         }
     
        string TypeWithoutNamespace(object obj)
        {
            string[] astr = obj.GetType().ToString().Split('.');
            return astr[astr.Length-1];
        }

        #endregion
    }
}
