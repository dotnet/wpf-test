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
using Microsoft.Test.Threading;
using System.Runtime.InteropServices;
using Avalon.Test.CoreUI.Source;
using Avalon.Test.CoreUI.Threading;
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
    public class MultipleHwndSourceonTopLevelWindowonDifferentContext : TestCase
    {
        
        /// <summary>
        /// Constructor.  On the base class pass TestCaseType.ContextSupport 
        /// </summary>
        public MultipleHwndSourceonTopLevelWindowonDifferentContext() :base(TestCaseType.None){}
        
        /// <summary>
        /// Creating a top window that contains 4 HwndSource, One dispatcher and 4 context Context and One Thread, making a Click on each Button.
        /// </summary>
        /// <remarks>
        ///     <ol>Scenarios steps:
        ///         <li>Create 1 Source and a tree with Border-StackPanel->TextPanel->Button</li>
        ///     </ol>
        ///     <Owner>Microsoft</Owner>
 
        ///     <Area>Source\SourceServices\Simple</Area>
        ///     <location>MultipleHwndSourceonTopLevelWindowonDifferentContext.cs</location>
        ///</remarks>
        [Test(0, @"Source\Child\Context\Multiple", TestCaseSecurityLevel.FullTrust, "MultipleHwndSourceonTopLevelWindowonDifferentContext", Area = "AppModel")]
        override public void Run()
        {
            CoreLogger.BeginVariation();
            TestCaseFailed = true;

            MainDispatcher = Dispatcher.CurrentDispatcher;

            _Source = new HwndWrapper(0,NativeConstants.WS_VISIBLE | NativeConstants.WS_OVERLAPPEDWINDOW | NativeConstants.WS_CLIPCHILDREN,0,25,25,550,550,"TopLevel Window",IntPtr.Zero,null);

            try
            {
                for(int i = 0; i<4; i++)
                    CreateTree(i);
                    

                DispatcherHelper.EnqueueBackgroundCallback(Dispatcher.CurrentDispatcher, 
                    new DispatcherOperationCallback (onPainted),null);
                
                using(CoreLogger.AutoStatus("Dispatcher.Run"))
                {
                    Dispatcher.Run();
                }

                FinalReportFailure();

            }
            finally
            {
                if (_Source != null) _Source.Dispose();
            }

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

            HwndSource source = null;

            try
            {
            MainDispatcher.Invoke(DispatcherPriority.Normal,
                (DispatcherOperationCallback)delegate(object arg)
                {
                    source = SourceHelper.CreateHwndSource( 200, 200, x, y, _Source.Handle);

                    Border border = null;

                    using(CoreLogger.AutoStatus("Creating a tree"))
                    {

                        border = new Border();
                        border.Background = Brushes.Cyan;

                        StackPanel sp = new StackPanel();
                        border.Child = sp;            

                        FlowDocumentScrollViewer fdsv = new FlowDocumentScrollViewer();
                        fdsv.Document = new FlowDocument();
                        
                        Button b = new Button();
                        b.Name ="Button" + WindowNumber.ToString();
                        b.Click += new  RoutedEventHandler(buttonClicked);
                        _ht.Add(b.Name, b);

                        b.Content = "Button" + WindowNumber.ToString(); 
                        b.Width = 200;
                        b.Height = 200;

                        sp.Children.Add(b);

                    }

                    source.RootVisual = border;

                    
                    return null;
                }, null);

            }
            catch(Exception e)
            {
                ExceptionList.Add(e);
            }
            return source;

        }


        int _count = 0;


        void buttonClicked(object o, RoutedEventArgs args)
        {

            Button button = o as Button;
            _count ++;

            if (_count == 1 && button.Name=="Button1")
            {
                MouseHelper.Click((UIElement)_ht["Button3"]);                
            }
            else if (_count == 2 && button.Name=="Button3")
            {
                MouseHelper.Click((UIElement)_ht["Button2"]);
            }
            else if (_count == 3 && button.Name=="Button2")
            {
                MouseHelper.Click((UIElement)_ht["Button0"]);
            }                
            else if (_count == 4 && button.Name=="Button0")
            {
                TestCaseFailed = false;
                Dispatcher.CurrentDispatcher.BeginInvokeShutdown(DispatcherPriority.Background); 
            }         
            else
            {
                throw new Microsoft.Test.TestValidationException("This event should not be called on this way");
            }

            
        }


        object onPainted(object o)
        {
            DispatcherTimer dTimer = new DispatcherTimer(DispatcherPriority.Normal);
            dTimer.Interval = TimeSpan.FromSeconds(2);
            dTimer.Tick += new EventHandler(startInput);
            dTimer.Start();

            return null;
        }
        bool _painted = false;

        void startInput(object o, EventArgs args)
        {
            if (!_painted)
            {
                _painted = true;
                DispatcherTimer dTimer = (DispatcherTimer)o;
                dTimer.Stop();
                MouseHelper.Click((UIElement)_ht["Button1"]);
            }
        }

        object exitDispatcher(object o)
        {
           
            return false;
        }




        ArrayList _array = new ArrayList(4);

        Hashtable _ht = new Hashtable(4);

        HwndWrapper _Source = null;


        
    }
}





