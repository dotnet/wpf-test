// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Microsoft.Test;
using Microsoft.Test.Discovery;
using Avalon.Test.CoreUI;
using System.Threading; 
using System.Windows.Threading;

using System.Windows.Interop;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Avalon.Test.CoreUI.Common;

using System.IO;
using System.Runtime.InteropServices;
using Avalon.Test.CoreUI.Source;
using Microsoft.Test.Win32;
using Avalon.Test.CoreUI.Threading;
//using Avalon.Test.Framework.Dispatchers;
using System.Collections;
using System.Windows.Controls;
using System.Windows.Markup;
using Microsoft.Test.Threading;
using Avalon.Test.CoreUI.CoreInput.Common;
using Avalon.Test.CoreUI.Trusted.Controls;

namespace Avalon.Test.CoreUI.Hosting
{
    ///<summary>
    ///</summary>
    ///<remarks>
    ///     <filename>HwndHostInXaml.cs</filename>
    ///</remarks>
    [TestDefaults]
    public class HwndHostInXaml : TestCase
    {
        

        /// <summary>
        /// Constructor.  On the base class pass TestCaseType.ContextSupport 
        /// </summary>
        public HwndHostInXaml() :base(TestCaseType.HwndSourceSupport)
        {

        }

        /// <summary>
        /// Create a  HwndSource that host a Win32Button using HwndHost and later click on the Win32Button and recive the message. The tree is loaded using Xaml file
        /// </summary>
        /// <remarks>
        ///  <ol>Description Steps:
        ///     <li>Create a HwndDispatcher and Context</li>
        ///     <li>Create a canvas and a HwndHost to host a Win32ButtonElement loaded from Xaml using XamlReader.Load</li>
        ///     <li>When the button is painted we click on the Win32Button</li>
        ///     <li>When we receive the message, exit the dispatcher</li>
        ///  </ol>
	    ///     <filename>HwndHostInXaml.cs</filename>
        /// </remarks>
        [TestAttribute(0, @"Hosting\Xaml\Simple", TestCaseSecurityLevel.FullTrust, "HwndHostInXaml", SupportFiles = @"FeatureTests\ElementServices\SimpleWin32HwndHost.xaml", Area = "AppModel")]
        public override void Run()
        {
            CoreLogger.BeginVariation();
            TestCaseFailed = true;
            
            if (!File.Exists("SimpleWin32HwndHost.xaml"))
                throw new Microsoft.Test.TestSetupException("The file SimpleWin32HwndHost.xaml is not found");

            Border border;

            using (CoreLogger.AutoStatus("Creating a Win32ButtonElement and adding to the Tree"))
            {

                FileStream fs = new FileStream("SimpleWin32HwndHost.xaml", FileMode.Open);
                ParserContext pc = new ParserContext();
                pc.BaseUri = System.IO.Packaging.PackUriHelper.Create(new Uri("siteoforigin://"));
                Page rootPage = System.Windows.Markup.XamlReader.Load(fs, pc) as Page;
                if(null == rootPage)
                    throw new Microsoft.Test.TestSetupException("Page cast filed");

                border = rootPage.Content as Border;

                StackPanel fPanel = border.Child as StackPanel;

                if (fPanel == null)
                    throw new Microsoft.Test.TestSetupException("StackPanel cast filed");

                _host = fPanel.Children[1] as Win32ButtonElement;
                _host.Name = "Win32Button";

                _host.ContainerWindowHook += new HwndSourceHook(mouseListener);

                if (_host == null)
                    throw new Microsoft.Test.TestSetupException("Win32ButtonElement cast filed");

                _host.Painted += new EventHandler(OnPainted);

            }


            Source.RootVisual = border;

            using (CoreLogger.AutoStatus("Dispatcher.Run"))
            {
                Dispatcher.Run();
            }


            FinalReportFailure();
            CoreLogger.EndVariation();
        }


        void OnPainted(object o, EventArgs args)
        {
            if (!_firstPaintDone)
            {
                _firstPaintDone = true;

                UIElement e = o as UIElement;

                ThreadingHelper.DispatcherTimerHelper(DispatcherPriority.Background, new TimeSpan(0,0,2), new EventHandler (_mouseClick), e);
                
            }
        }

        void _mouseClick(object o, EventArgs args)
        {
            DispatcherTimer dt = (DispatcherTimer)o;
            dt.Stop();
            MouseHelper.Click((IntPtr)((Win32ButtonElement)_host).Win32Handle);
        }


        IntPtr mouseListener(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == NativeConstants.WM_COMMAND && lParam == _host.Win32Handle && (Int64)wParam == NativeConstants.BN_CLICKED)
            {

                TestCaseFailed = false;
                
                using (CoreLogger.AutoStatus("Exit the dispatcher"))
                {
                    MainDispatcher.InvokeShutdown();
                }
            }

            handled = false;

            return IntPtr.Zero;
        }


       Win32ButtonElement _host = null;

       bool _firstPaintDone = false;


    }
        
 }












