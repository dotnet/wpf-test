// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Markup;
using System;
using Avalon.Test.CoreUI.Trusted;
using Microsoft.Test;
using Microsoft.Test.Discovery;
using Avalon.Test.CoreUI;
using System.Threading; 
using System.Windows.Threading;
using Microsoft.Test.Win32;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Interop;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Threading;
using System.Runtime.InteropServices;
using Avalon.Test.CoreUI.Source;
using Microsoft.Test.Threading;
using System.Collections;

namespace Avalon.Test.CoreUI.Source.Hwnd
{
    /// <summary>
    ///         Validating the SourceServices works correctly
    ///         
    /// </summary>
    /// <remarks>
    ///     <Owner>Microsoft</Owner>
 
    ///     <Area>Source\SimpleThreadSingleContext</Area>
    ///     <location>SourcesServicesSimple.cs</location>
    ///</remarks>
    //[CoreTestsLoader(CoreTestsTestType.MethodBase)]
    [TestDefaults]
    public class MultipleHwndSourceonTopLevelWindow : TestCase
    {
        
        /// <summary>
        /// Constructor.  On the base class pass TestCaseType.ContextSupport 
        /// </summary>
        public MultipleHwndSourceonTopLevelWindow() :base(TestCaseType.None){}
        
        /// <summary>
        /// Creating a top window that contains 4 HwndSource, One dispatcher and One Context and One Thread, making a Click on each Button.
        /// </summary>
        /// <remarks>
        ///     <ol>Scenarios steps:
        ///         <li></li>
        ///     </ol>
        ///     <Owner>Microsoft</Owner>
 
        ///     <Area>Source\Child\Context\Single</Area>
        ///     <location>MultipleHwndSourceonTopLevelWindow.cs</location>
        ///</remarks>
        [Test(0, @"Source\Child\Context\Single", TestCaseSecurityLevel.FullTrust, "MultipleHwndSourceonTopLevelWindow", Area = "AppModel")]
        override public void Run()
        {
            CoreLogger.BeginVariation();
            _Source = new HwndWrapper(0,NativeConstants.WS_VISIBLE | NativeConstants.WS_OVERLAPPEDWINDOW | NativeConstants.WS_CLIPCHILDREN,0,25,25,550,550,"TopLevel Window",IntPtr.Zero,null);


            for(int i = 0; i<4; i++)
                CreateTree(i);
                
            ThreadingHelper.BeginInvokeOnSignalHandle(_ev.AutoEvent, MainDispatcher, DispatcherPriority.Background, new DispatcherOperationCallback(startInput), null);

            
            using(CoreLogger.AutoStatus("Dispatcher.Run"))
            {
                Dispatcher.Run();
            }

            

            using(CoreLogger.AutoStatus("Last Validation for Items be dispatched"))
            {
                if (!_isPass)
                    throw new Microsoft.Test.TestValidationException("The clicks where not correct");
            }

            _Source.Dispose();

            CoreLogger.EndVariation();
        }


        HwndSource CreateTree(int WindowNumber)
        {
            int y = 0, x = 0;


            if (WindowNumber == 0)
            {
            }
            else if (WindowNumber == 1)
            {
                x = 300;
            }
            else if (WindowNumber== 2)
            {
                y = 300;
            }                    
            else
            {
                x = y = 300;
            }
        
            HwndSource source = SourceHelper.CreateHwndSource( 200, 200, x, y,_Source.Handle);

        
            Border border = null;

            using(CoreLogger.AutoStatus("Creating a tree"))
            {

                border = new Border();
                border.Background = Brushes.Cyan;

                StackPanel sp = new StackPanel();

                border.Child = sp;            


                HelloElement hello = new HelloElement();
                hello.RenderedSourcedHandlerEvent += new HelloElement.RenderHandler(onPainted);
                hello.Source = source;
                hello.Width = 20;
                hello.Height = 20;
                sp.Children.Add(hello);



                Button b = new Button();
                b.Name ="Button" + WindowNumber.ToString();
                b.Click += new  RoutedEventHandler(buttonClicked);
                _ht.Add(b.Name, b);

                b.Content = "My Button";
                b.Width = 200;
                b.Height = 200;

                sp.Children.Add(b);



            }

            source.RootVisual = border;

            return source;

        }


        int _count = 0;


        void buttonClicked(object o, RoutedEventArgs args)
        {

            Button button = o as Button;
            _count ++;

            if (_count == 1 && button.Name=="Button1")
            {
                ClickOnButton("Button3");
            }
            else if (_count == 2 && button.Name=="Button3")
            {
                ClickOnButton("Button2");
            }
            else if (_count == 3 && button.Name=="Button2")
            {
                ClickOnButton("Button0");
            }                
            else if (_count == 4 && button.Name=="Button0")
            {
                _isPass = true;
                Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(exitDispatcher), null);
            }         
            else
            {
                throw new Microsoft.Test.TestValidationException("This event should not be called on this way");
            }

            
        }


        void onPainted(UIElement target, HwndSource Source)
        {
            lock (_array)
            {
                if (!_array.Contains(Source))
                {
                    _array.Add(Source);
                    _ev.Set();
                }
            }
        }


        void ClickOnButton(string name)
        {
            Button button = ((Button)_ht[name]);

            MouseHelper.Click(button);
        }

        object startInput(object o)
        {
            ClickOnButton("Button1");
                        
            return null;
        }

        object exitDispatcher(object o)
        {

            Microsoft.Test.Threading.DispatcherHelper.ShutDown();
            
            return false;
        }


        CoreAutoResetEvent _ev = new CoreAutoResetEvent(false,4);

        ArrayList _array = new ArrayList(4);

        Hashtable _ht = new Hashtable(4);

        bool _isPass = false;

        HwndWrapper _Source = null;
        
    }
}




