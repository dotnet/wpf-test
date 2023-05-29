// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Holds verification routines for Template parsing and serialization tests.
 * Contributors: Microsoft
 *
 
  
 * Revision:         $Revision: 8 $
 
 * Filename:         $Source: //depot/vbl_wcp_avalon_dev/windowstest/client/wcptests/Core/Framework/Testcases/Serialization/TemplateVerifiers.cs $
********************************************************************/

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.ComponentModel.Design.Serialization;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Markup;
using System.Xml;
using System.Windows.Controls.Primitives;
using Avalon.Test.CoreUI.Serialization;
using System.Windows.Navigation;
using Microsoft.Test.Threading;

using Avalon.Test.CoreUI.Common;

namespace Avalon.Test.CoreUI.Integration
{
	/// <summary>
    /// Holds verification routines for Template parsing and serialization tests.
    /// </summary>
    public class FrameIntegration
    {
        /// <summary>
        /// Verifies PropertyTag syntax works in Template.
        /// </summary>
        public static void Verify(UIElement root)
        {
            CoreLogger.LogStatus("Inside FrameIntegration.Verify.");
            s_page = root as Window;
            s_outsideButton = s_page.Content as CustomCanvasForFrameIntegration;
            s_outsideButton.EventHandlerInvoked = false;
            s_outsideButton.EventSourceIsFrame = false;
            
            //Verify frame1 whose content is defined with Content property
            VerifyFrame("frame1");

            //Verify frame2 whose content is defined with Source
            VerifyFrame("frame2");

            //Verify frame3
            Frame frame = s_page.FindName("frame3") as Frame;
            Button button = new Button();
            frame.Navigate(button);
            
            frame.LoadCompleted += new LoadCompletedEventHandler(VerifyFrame3);
            frame.Refresh();
            

            //Verify frame4
            frame = s_page.FindName("frame4") as Frame;
            frame.Source = new Uri(@"ButtonForFrameIntegration.xaml", UriKind.RelativeOrAbsolute);
            
            
            frame.LoadCompleted += new LoadCompletedEventHandler(VerifyFrame4);
                 
            frame.Refresh();

            //Verify frame5, verify the effect of Tempalte.
            frame = s_page.FindName("frame5") as Frame;
            VerifyElement.VerifyDouble(frame.Height, 500.0);
            VerifyElement.VerifyDouble(frame.Width, 300.0);
            DispatcherHelper.DoEvents();
        }
        static void VerifyFrame3(Object sender, NavigationEventArgs e)
        {
            VerifyFrame("frame3");
        }
            
        static void VerifyFrame4(Object sender, NavigationEventArgs e)
        {
            VerifyFrame("frame4");
        }

        static void VerifyFrame(string FrameName)
        {
            CoreLogger.LogStatus("Verifying Frame: " + FrameName);
            Frame frame = s_page.FindName(FrameName) as Frame;
            //Verigy style on Frame
            VerifyElement.VerifyBool(frame.SandboxExternalContent, true);

            s_outsideButton.EventHandlerInvoked = false;
            s_outsideButton.EventSourceIsFrame = false;

            DispatcherHelper.DoEvents();

            Button button = frame.Content as Button;
            RoutedEventArgs args = new RoutedEventArgs(ButtonBase.ClickEvent);
            button.RaiseEvent(args);
            if(s_outsideButton.EventHandlerInvoked)
            {
                CoreLogger.LogStatus("Event handler invoked.");
            }
            else
            {
                throw new Microsoft.Test.TestValidationException("Event handler on button outside has not be invoked.");
            }
            
            if(s_outsideButton.EventSourceIsFrame)
            {
                CoreLogger.LogStatus("Source of the event is a Frame.");
            }
            else
            {
                throw new Microsoft.Test.TestValidationException("Source of the event is not a Frame.");
            }
            //verify inheritable property are blocked on 


            //Verify Property Trigger for style, style is defined in a resource dictionary outside the Frame.
            //Verify that resource lookup fails. 
            SolidColorBrush brush = button.Background as SolidColorBrush;
            CoreLogger.LogStatus(button.Background.ToString());
            VerifyElement.VerifyBool(brush == null, true);
            //Verify Style defined in a resource dictionary inside the Frame.
            if (String.Equals(FrameName, "frame1", StringComparison.InvariantCulture))
            {
                Button childButton = button.Content as Button;
                VerifyElement.VerifyBool(childButton == null, false);
                brush = childButton.Background as SolidColorBrush;
                VerifyElement.VerifyBool(brush == null, false);
                VerifyElement.VerifyColor(brush.Color, Colors.Blue);
            }
            


            //Name lookup block on 




          }
          static Window s_page;
          static CustomCanvasForFrameIntegration s_outsideButton;  
	}
    /// <summary>
    /// A custom Button with two boolean properties to record the effect of event handler. 
    /// </summary>
    public class CustomCanvasForFrameIntegration : Canvas
    {
        
        /// <summary>
        /// Contructor
        /// </summary>
        public  CustomCanvasForFrameIntegration() : base(){}
        
        /// <summary>
        /// A property to indict whether the event handler has been invoked. 
        /// </summary>
        public bool EventHandlerInvoked
        {
            get 
            {
                return _eventHandlerInvoked;
            }
            set
            {
                _eventHandlerInvoked = value;
            }
        }
        bool _eventHandlerInvoked = false;
        /// <summary>
        /// A property to indict whether the source of the event is a Frame.
        /// </summary>
        public bool EventSourceIsFrame
        {
            get 
            {
                return _eventSourceIsFrame;
            }
            set
            {
                _eventSourceIsFrame = value;
            }
        }
        bool _eventSourceIsFrame = false;  
    }
}

