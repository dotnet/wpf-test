// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Threading; 
using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.CoreInput.Common;
using Avalon.Test.CoreUI.Source;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.Trusted.Controls;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Win32;

namespace Avalon.Test.CoreUI.Source
{
    /// <summary>
    ///         
    /// </summary>
    /// <remarks>
    ///     <Owner>Microsoft</Owner>
 
	///     <Area>Source\Nesting\Simple</Area>
	///     <location>Avalon_Hwnd_Avalon_Hwnd.cs</location>
	///</remarks>
    [TestDefaults]
    public class Avalon_Hwnd_Avalon_Hwnd: TestCase
    {
        
        /// <summary>
        /// Constructor.  On the base class pass TestCaseType.ContextSupport 
        /// </summary>
        public Avalon_Hwnd_Avalon_Hwnd() :base(TestCaseType.None){}
        Button _btn;

        /// <summary>
        /// HwndSource-HwndHost-HwndSource
        /// </summary>
        [TestAttribute(0, @"Source\Nesting\Simple", TestCaseSecurityLevel.FullTrust, "AvalonHwndAvalonHwnd", Area = "AppModel")]
        override public void Run()
        {
            CoreLogger.BeginVariation();
            _source = SourceHelper.CreateHwndSource( 500, 500,0,0);

            StackPanel sp = new StackPanel();            
            _source.RootVisual = sp;

            Button b = new Button();
	        _btn =b;
            b.Content = "Main Window";
            b.Width = 200;
            b.Height = 200;
            
            sp.Children.Add(b);
			CoreLogger.LogStatus("Creating a HwndHost that host an HwndSource");
			_avhh = new AvalonHwndHostAvalon(200, 200);

            StackPanel panel = new StackPanel();

			
			Button b1 = new Button();
            b1.Content = "Inner Window";
            b1.Width = 100;
            b1.Height = 100;
            b1.Click += new RoutedEventHandler(_clickedButton);
            ((AvalonHwndHostAvalon)_avhh).RootVisual = panel;
			CoreLogger.LogStatus("Adding a button");
			panel.Children.Add(b1);
            
            sp.Children.Add(_avhh);

            EmptyElement empty = new EmptyElement();
            empty.Rendered += new EventHandler(_startInput);
            panel.Children.Add(empty);

            Dispatcher.Run();

            CoreLogger.EndVariation();      
        }   

        HwndSource _source;    
        HwndHost _avhh;
        
        void _clickedButton(object o, RoutedEventArgs args)
        {
			CoreLogger.LogStatus("Click is received");
			Button button = o as Button;
            CoreLogger.LogStatus(button.Content.ToString() + " was clicked");
            Microsoft.Test.Threading.DispatcherHelper.ShutDown();
            
        }


        void _startInput(object o, EventArgs args)
        {
			CoreLogger.LogStatus("Enqueue a MouseClick on the Button");
			Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.SystemIdle, new DispatcherOperationCallback(foo),
                null);
       

        }

        object foo (object o)
        {
			CoreLogger.LogStatus("Moving the mouse");

            UIElement el;
            if (_avhh is AvalonHwndHostHWNDAvalon)
            {
                el = (UIElement)((Panel)((AvalonHwndHostHWNDAvalon)_avhh).RootVisual).Children[0];
            }
            else
            {
                el = (UIElement)((Panel)((AvalonHwndHostAvalon)_avhh).RootVisual).Children[0];
            }
            
            MouseHelper.Click(el);

            return null;
        }

        
        /// <summary>
        /// Click on a Button on this kind of app. HwndSource->HwndHost->HWND->HwndSource->Button
        /// </summary>
        [TestAttribute(0, @"Source\Nesting\Simple", TestCaseSecurityLevel.FullTrust, "AvalonHwndAvalonHwnd2", Area = "AppModel")]
         public void RunTwo()
        {
            CoreLogger.BeginVariation();
            _source = SourceHelper.CreateHwndSource( 500, 500,0,0);            



            StackPanel sp = new StackPanel();            
            _source.RootVisual = sp;

            Button b = new Button();
            _btn = b;
            b.Content = "Main Window";
            b.Width = 200;
            b.Height = 200;
            
            sp.Children.Add(b);
			CoreLogger.LogStatus("Creating a HwndHost that Host a Hwnd that has a child HWND that is is HwndSource");
			_avhh = new AvalonHwndHostHWNDAvalon(200, 200);

            StackPanel panel = new StackPanel();
            

            Button b1 = new Button();
            b1.Content = "Inner Window";
            b1.Width = 100;
            b1.Height = 100;
            b1.Click += new RoutedEventHandler(_clickedButton);
            ((AvalonHwndHostHWNDAvalon)_avhh).RootVisual = panel;
            panel.Children.Add(b1);
            
            sp.Children.Add(_avhh);

            EmptyElement empty = new EmptyElement();
            empty.Rendered += new EventHandler(_startInput);
            panel.Children.Add(empty);

            Dispatcher.Run();

            CoreLogger.EndVariation();         
        }   

        
  

        /// <summary>
        /// Click on a Button on this kind of app HwndSource->HwndHost->HwndSource->HwndHost->HWND->HwndSource->Button
        /// </summary>
        [TestAttribute(0, @"Source\Nesting\Simple", TestCaseSecurityLevel.FullTrust, "AvalonHwndAvalonHwnd3", Area = "AppModel")]
         public void RunThree()
        {
            CoreLogger.BeginVariation();
            _source = SourceHelper.CreateHwndSource( 500, 500,0,0);            



            StackPanel sp = new StackPanel();            
            _source.RootVisual = sp;

            Button b = new Button();
            _btn = b;
            b.Content = "Main Window";
            b.Width = 200;
            b.Height = 200;
            
            sp.Children.Add(b);
		    
			CoreLogger.LogStatus("Creating a HwndHost that hosts a HwndSource");
			AvalonHwndHostAvalon avhh = new AvalonHwndHostAvalon(200, 200);
            sp.Children.Add(avhh);

            StackPanel s = new StackPanel();           

            avhh.RootVisual = s;
			
			CoreLogger.LogStatus("Creating a HwndHost that Host a Hwnd that has a child HWND that is is HwndSource");
			_avhh = new AvalonHwndHostHWNDAvalon(300, 300);
			s.Children.Add(_avhh);

			StackPanel panel = new StackPanel();

            ((AvalonHwndHostHWNDAvalon)_avhh).RootVisual = panel;

          
            Button b1 = new Button();
            b1.Content = "Inner Window";
            b1.Width = 100;
            b1.Height = 100;
            b1.Click += new RoutedEventHandler(_clickedButton);
            ((AvalonHwndHostHWNDAvalon)_avhh).RootVisual = panel;
			CoreLogger.LogStatus("Adding a button to the lowest HwndSource");
			panel.Children.Add(b1);
            
            EmptyElement empty = new EmptyElement();
            empty.Rendered += new EventHandler(_startInput);
            panel.Children.Add(empty);

            Dispatcher.Run();
            CoreLogger.EndVariation();
        }   

    }
}


