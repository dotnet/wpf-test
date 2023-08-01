// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Threading; 
using System.Windows.Threading;
using Microsoft.Test.Win32;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
//using Avalon.Test.CoreUI.

using System.Runtime.InteropServices;
//using Avalon.Test.CoreUI.Source;
//using Avalon.Test.CoreUI.Threading;
//using Avalon.Test.Framework.Dispatchers;
using System.Windows.Controls;
using System.Collections;
using Microsoft.Test.Threading;

namespace Avalon.Test.CoreUI.Trusted.Controls
{


    /// <summary>
    /// Simple UIElement to render something on the screen
    /// </summary>
    public class EmptyElement : System.Windows.FrameworkElement
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public EmptyElement(){}

        /// <summary>
        /// Constructor
        /// </summary>
        protected override Size MeasureOverride(Size constraint)
        {
            Size desiredSize = new Size(0,0);
            return desiredSize;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public event EventHandler Rendered;
        
        /// <summary>
        /// Rendered method that it is called by the MIL
        /// </summary>
        /// <param name="ctx"></param>
        protected override void OnRender(DrawingContext ctx)
        {            
            base.OnRender(ctx);
            if(Rendered != null)
            {
                Rendered(this,EventArgs.Empty);
            }
        }


        /// <summary>
        /// HitTestCore implements precise hit testing against render contents
        /// </summary>
        protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParams)
        {
            Rect r = new Rect(0, 0, RenderSize.Width, RenderSize.Height);
 
            if (r.Contains(hitTestParams.HitPoint))
            {
                return new PointHitTestResult(this, hitTestParams.HitPoint);
            }

            return null;
        }
    }

    ///<summary>
    ///</summary>
    public class SimpleHostControl : HwndHost
    {
        ///<summary>
        ///</summary>
      public SimpleHostControl(){}  

        ///<summary>
        ///</summary>
        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
            if (_called)
                throw new Microsoft.Test.TestValidationException("BuildWindowCore is called twice");

            _called = true;

            IntPtr p = NativeMethods.CreateWindowEx(0, "Button", "Push Me!", NativeConstants.WS_CHILD | NativeConstants.WS_VISIBLE, 0, 0, 100, 100, hwndParent.Handle, IntPtr.Zero, IntPtr.Zero, null);
            return new HandleRef(null, p);
        }

        ///<summary>
        ///</summary>
        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            NativeMethods.DestroyWindow(hwnd);
        }

        bool _called = false;
    }





        /// <summary>
        /// HwndSource-HwndHost-HwndSource
        /// </summary>
        public class AvalonHwndHostAvalon : HwndHost
        {
                /// <summary>
            /// HwndSource-HwndHost-HwndSource
            /// </summary>
            public AvalonHwndHostAvalon(double width, double height)
            {
                Width = width;
                Height = height;
            }

            /// <summary>
            /// HwndSource-HwndHost-HwndSource
            /// </summary>
            public Visual RootVisual
            {
                set
                {
                    _rootVisual = value;
                
                    if (_source != null)
                    _source.RootVisual = _rootVisual;
                }
                get
                {
                    return _rootVisual;
                }
            }

            /// <summary>
            /// HwndSource-HwndHost-HwndSource
            /// </summary>
            protected override HandleRef BuildWindowCore(HandleRef hwndParent)
            {

                _source = SourceHelper.CreateHwndSource(50, 50, 0,0, (IntPtr)hwndParent);            
                _source.RootVisual = _rootVisual;

                return new HandleRef(null, _source.Handle);
            }

            ///<summary>
            ///</summary>
            protected override void DestroyWindowCore(HandleRef hwnd)
            {
                NativeMethods.DestroyWindow(hwnd);
            }

            HwndSource _source;
            Visual _rootVisual;
        }


        /// <summary>
        /// HwndSource-HwndHost-HwndSource
        /// </summary>
        public class AvalonHwndHostHWNDAvalon : HwndHost
        {
                /// <summary>
            /// HwndSource-HwndHost-HwndSource
            /// </summary>
            public AvalonHwndHostHWNDAvalon(double width, double height)
            {
				this.Width = width;
				_width = width;
				_height = height;
				this.Height = height;
				_mainWindow = new HwndWrapper(0, NativeConstants.WS_CHILD | NativeConstants.WS_CLIPCHILDREN, 0, 0, 0, (int)_width,(int)_height, "ContainerWindow", NativeConstants.HWND_MESSAGE, null);
				_hwndWrapperHook =  new HwndWrapperHook (Helper);
				_mainWindow.AddHook(_hwndWrapperHook );

            }

			double _width = 0;

			double _height = 0;

            /// <summary>
            /// HwndSource-HwndHost-HwndSource
            /// </summary>
            public Visual RootVisual
            {
                set
                {
                    _rootVisual = value;
                
                    if (_source != null)
                    _source.RootVisual = _rootVisual;
                }
                get
                {
                    return _rootVisual;
                }
            }

            /// <summary>
            /// HwndSource-HwndHost-HwndSource
            /// </summary>
            protected override HandleRef BuildWindowCore(HandleRef hwndParent)
            {

                NativeMethods.SetParent(new HandleRef(null,_mainWindow.Handle), new HandleRef(null,hwndParent.Handle));
				_source = SourceHelper.CreateHwndSource( (int)_width, (int)_height, 0, 0, _mainWindow.Handle);
				_source.RootVisual = _rootVisual;

                return new HandleRef(null, _mainWindow.Handle);
            }

            ///<summary>
            ///</summary>
            protected override void DestroyWindowCore(HandleRef hwnd)
            {
                NativeMethods.DestroyWindow(hwnd);
            }

            /// <summary>
            /// HwndSource-HwndHost-HwndSource
            /// </summary>
            protected IntPtr Helper(IntPtr window, int message, IntPtr firstParam, IntPtr secondParam, ref bool handled)
            {
				if (window == _mainWindow.Handle && message == NativeConstants.WM_SHOWWINDOW)
				{
					IntPtr _true = new IntPtr(1);
					if (firstParam == _true)
					{
						NativeMethods.ShowWindow(new HandleRef(null, _source.Handle), NativeConstants.SW_SHOW);
					}
				}
				else if (window == _mainWindow.Handle && message == NativeConstants.WM_SIZE)
                {

                     /*  
                    float _devicePixelsPerInchX, _devicePixelsPerInchY;
                    
                     IntPtr hdcW = NativeMethods.GetDC(new HandleRef(null, _mainWindow.Handle));

                    if (hdcW == IntPtr.Zero)
                    {
                        //
                        // If we were unable to obtain HDC for the given
                        // window, assume the default of 96 DPI.
                        //

                        _devicePixelsPerInchX = 96.0f;
                        _devicePixelsPerInchY = 96.0f;
                    }
                    else
                    {
                        //
                        // Obtain and cache DPI values for window's HDC.
                        //

                        _devicePixelsPerInchX = (double)NativeMethods.GetDeviceCaps(
                            new HandleRef(null, hdcW),
                            NativeConstants.LOGPIXELSX);

                        _devicePixelsPerInchY = (double)NativeMethods.GetDeviceCaps(
                            new HandleRef(null, hdcW),
                            NativeConstants.LOGPIXELSY);

                        //
                        // Release DC object.
                        //

                        NativeMethods.ReleaseDC(
                            new HandleRef(null, _mainWindow.Handle),
                            new HandleRef(null, hdcW));
                    }


                    int x = NativeMethods.SignedLOWORD(lParam);
                    int y = NativeMethods.SignedHIWORD(lParam);

                    (x  * 96.0f) / _devicePixelsPerInchX;
                    (y * 96.0f) / _devicePixelsPerInchY;
                    */

                    NativeMethods.PostMessage(new HandleRef(null,_source.Handle), NativeConstants.WM_SIZE, firstParam,  secondParam);

                }

            

            
                handled = false;
                return IntPtr.Zero;
            }


            HwndSource _source;
            Visual _rootVisual;
            HwndWrapper _mainWindow;
		HwndWrapperHook _hwndWrapperHook;
        }









    /// <summary>
    /// Constructor.  On the base class pass TestCaseType.ContextSupport 
    /// </summary>
    public class AvalonHostedControl : HwndHost
    {

        /// <summary>
        /// Constructor.  On the base class pass TestCaseType.ContextSupport 
        /// </summary>
        public AvalonHostedControl()
        {
        }

        /// <summary>
        /// Constructor.  On the base class pass TestCaseType.ContextSupport 
        /// </summary>
        override protected HandleRef BuildWindowCore(HandleRef parent)
        {


            _source = SourceHelper.CreateHwndSource (100, 100, 0,0, parent.Handle);
            _source.AddHook(new HwndSourceHook(Helper));

            StackPanel panel = new StackPanel();
            _rootUIElement = (UIElement)panel;
            _source.RootVisual = panel;            

            Button b = new Button();
            b.Click += new RoutedEventHandler(Click);
            b.Content = "akiaki";

            panel.Children.Add(b);          

            MainWindow = _source.Handle;


            return new HandleRef(null,MainWindow);

        }



        UIElement _rootUIElement;
        HwndSource _source;

        ///<summary>
        ///</summary>
        public UIElement RootUIElement
        {          
            get
            {
                return _rootUIElement;
            }
            set
            {
                _rootUIElement = value;
                if (_source != null)
                {
                    _source.RootVisual = _rootUIElement;
                }
            }
        }
        
        ///<summary>
        ///</summary>
        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            NativeMethods.DestroyWindow(hwnd);
        }

        /// <summary>Clicks on the Button.</summary>
        protected void Click(object o, RoutedEventArgs args)
        {

            //HostingAvalonWithaDispatcher.w.Close();

        }



        /// <summary>
        /// 
        /// </summary>
        public IntPtr MainWindow;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="window"></param>
        /// <param name="message"></param>
        /// <param name="firstParam"></param>
        /// <param name="secondParam"></param>
        /// <param name="handled"></param>
        /// <returns></returns>
        protected IntPtr Helper(IntPtr window, int message, IntPtr firstParam,
            IntPtr secondParam, ref bool handled)
        {
            
            handled = false;
			return IntPtr.Zero;
		}

    }
        

    /// <summary>
    /// Host an HwndSource isolated in a new AppDomain
    /// </summary>
    public class AvalonHostIsolated : HwndHost
    {

        /// <summary>
        /// Constructor.  On the base class pass TestCaseType.ContextSupport 
        /// </summary>
        public AvalonHostIsolated()
        {
			Thread thread = new Thread(new ThreadStart(SecondUIThread));

			thread.SetApartmentState(ApartmentState.STA);
			thread.Start();
			_ev.WaitOne();
            evStop = (AutoResetEvent)AppDomain.CurrentDomain.GetData("stopevent");
			CrossAppPointer pointer = (CrossAppPointer)AppDomain.CurrentDomain.GetData("childHwnd");

			_windowHandle = pointer.Handler;
		}

        /// <summary>
        /// Constructor.  On the base class pass TestCaseType.ContextSupport 
        /// </summary>
        override protected HandleRef BuildWindowCore(HandleRef parent)
        {
            _parent = parent.Handle;
            
			NativeMethods.SetParent(new HandleRef(null,_windowHandle), new HandleRef(null,_parent));

            
            return new HandleRef(null,_windowHandle);

        }

        

        ///<summary>
        ///</summary>
        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            NativeMethods.DestroyWindow(hwnd);
        }

        IntPtr _windowHandle;
        IntPtr _parent;

        /// <summary>
        /// 
        /// </summary>
        public static AutoResetEvent _ev = new AutoResetEvent(false);

        void SecondUIThread()
        {
            AppDomain domain = AppDomain.CreateDomain("Second Domain");

            
			domain.SetData("domain",AppDomain.CurrentDomain);
            domain.SetData("event",_ev);
            domain.SetData("stopevent",evStop);
            
            try
            {
                //domain.DoCallBack(new System.CrossAppDomainDelegate(CreateWindowCrossAppDomain));
                CreateWindowCrossAppDomain();
            }
            catch(Exception e)
            {
                InternalException = e;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Exception InternalException;

        /// <summary>
        /// 
        /// </summary>
        static void CreateWindowCrossAppDomain()
        {

            AutoResetEvent ev = (AutoResetEvent)AppDomain.CurrentDomain.GetData("event");

            try
            {
                
                AppDomain mainDomain = (AppDomain)AppDomain.CurrentDomain.GetData("domain");
                AutoResetEvent evstop = new AutoResetEvent(false);
                mainDomain.SetData("stopevent", evstop);


                HwndSource Source = SourceHelper.CreateHwndSource(100, 100, 0, 0, NativeConstants.HWND_MESSAGE, true, NativeConstants.WS_CHILD);


                Source.AddHook(new HwndSourceHook(Helper));

                ThreadPool.RegisterWaitForSingleObject(evstop, new WaitOrTimerCallback(StopDispatcher), Dispatcher.CurrentDispatcher,
                       600000, true);

                Border bo = new Border();
                bo.Background = Brushes.Blue;

                StackPanel panel = new StackPanel();
                bo.Child = panel;

                Button b = new Button();
                b.Content = "akiaki";
                panel.Children.Add(b);


                Source.RootVisual = bo;

                AppDomain domain = (AppDomain)AppDomain.CurrentDomain.GetData("domain");

                IntPtr child = Source.Handle;
                CrossAppPointer pointer = new CrossAppPointer(child);
                domain.SetData("childHwnd", pointer);
                
            }

            finally
            {
                ev.Set();
            }

            Dispatcher.Run();

           
        }

        static void StopDispatcher (object o , bool timeOut)
        {
            Dispatcher dispatcher = (Dispatcher)o;
            
            dispatcher.InvokeShutdown();

        }

        
        /// <summary>
        /// 
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            evStop.Set();
        }

        /// <summary>
        /// 
        /// </summary>
        public AutoResetEvent evStop = null;


        /// <summary>
        /// 
        /// </summary>
        public void StopControl()
        {
            evStop.Set();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="window"></param>
        /// <param name="message"></param>
        /// <param name="firstParam"></param>
        /// <param name="secondParam"></param>
        /// <param name="handled"></param>
        /// <returns></returns>
        static protected IntPtr Helper(IntPtr window, int message, IntPtr firstParam,
            IntPtr secondParam, ref bool handled)
        {
            
            handled = false;
			return IntPtr.Zero;
		}

    }

   /// <summary>
   /// Host an HwndSource on a separate thread.
   /// </summary>
   public class HwndHostThreaded : HwndHost
   {
       static HwndHostThreaded()
       {
       }

       /// <summary>
       /// Constructor.  On the base class pass TestCaseType.ContextSupport.
       /// </summary>
       public HwndHostThreaded()
       {
           Thread thread = new Thread(new ThreadStart(SecondUIThread));

           thread.SetApartmentState(ApartmentState.STA);
           thread.Start();
           _ev.WaitOne();
       }

       /// <summary>
       /// Constructor.  On the base class pass TestCaseType.ContextSupport
       /// </summary>
       override protected HandleRef BuildWindowCore(HandleRef parent)
       {
           _parent = parent.Handle;

           // Reparent window created on separate thread by ctor.
           NativeMethods.SetParent(new HandleRef(null, _windowHandle), new HandleRef(null, _parent));
           
           return new HandleRef(null, _windowHandle);
       }

       ///<summary>
       ///</summary>
       protected override void DestroyWindowCore(HandleRef hwnd)
       {
           NativeMethods.DestroyWindow(hwnd);
       }

       IntPtr _windowHandle;
       IntPtr _parent;

       /// <summary>
       /// Event set when thread has created nested HwndSource.
       /// </summary>
       public static AutoResetEvent _ev = new AutoResetEvent(false);

       /// <summary>
       /// </summary>
       protected void SecondUIThread()
       {
           try
           {
               // Create temporary message only window.
               HwndSourceParameters sourceParams = new HwndSourceParameters();
               sourceParams.Width = sourceParams.Height = 100;
               sourceParams.PositionY = sourceParams.PositionX = 0;
               sourceParams.ParentWindow = NativeConstants.HWND_MESSAGE;
               sourceParams.WindowStyle = NativeConstants.WS_CHILD;
               HwndSource Source = new HwndSource(sourceParams);

               Source.AddHook(new HwndSourceHook(Helper));

               // Stop eventually or on evstop.
               evStop = new AutoResetEvent(false);
               ThreadPool.RegisterWaitForSingleObject(evStop, new WaitOrTimerCallback(StopDispatcher), Dispatcher.CurrentDispatcher,
                      600000, true);

               // Build hosted HwndSource and content.
               Border bo = new Border();
               bo.Background = Brushes.Blue;
               StackPanel panel = new StackPanel();
               bo.Child = panel;
               Button b = new Button();
               b.Content = "HwndSource on second thread.";
               panel.Children.Add(b);

               Source.RootVisual = bo;

               // Save handle.
               _windowHandle = Source.Handle;
           }
           finally
           {
               // Tell main thread to continue.
               _ev.Set();
           }

           Dispatcher.Run();
       }

       /// <summary>
       /// 
       /// </summary>
       public Exception InternalException;

       static void StopDispatcher(object o, bool timeOut)
       {
           Dispatcher dispatcher = (Dispatcher)o;

           dispatcher.InvokeShutdown();
       }

       /// <summary>
       /// 
       /// </summary>
       protected override void Dispose(bool disposing)
       {
           base.Dispose(disposing);

           evStop.Set();
       }

       /// <summary>
       /// 
       /// </summary>
       public AutoResetEvent evStop = null;


       /// <summary>
       /// 
       /// </summary>
       public void StopControl()
       {
           evStop.Set();
       }

       /// <summary>
       /// 
       /// </summary>
       /// <param name="window"></param>
       /// <param name="message"></param>
       /// <param name="firstParam"></param>
       /// <param name="secondParam"></param>
       /// <param name="handled"></param>
       /// <returns></returns>
       static protected IntPtr Helper(IntPtr window, int message, IntPtr firstParam,
           IntPtr secondParam, ref bool handled)
       {
           handled = false;
           return IntPtr.Zero;
       }

   }


        ///<summary>
    ///</summary>
    public class CrossAppPointer : MarshalByRefObject
    {

        ///<summary>
        ///</summary>
        public CrossAppPointer(IntPtr pointer)
        {
			this.Handler = pointer;
		}


        ///<summary>
        ///</summary>
        public CrossAppPointer(CrossAppPointer data)
        {
            this.Handler = data.Handler;
        }


        ///<summary>
        ///</summary>
        public IntPtr Handler;
    }




    ///<summary>
    ///</summary>
    ///<remarks>
    ///     <filename>Win32ButtonControl.cs</filename>
    ///</remarks>
    public class Win32ButtonControl : HwndHost
    {
		static Win32ButtonControl()
		{

		}

        ///<summary>
        ///</summary>
        public Win32ButtonControl()
        {
            this.MessageHook += new HwndSourceHook(_hook);
        }

        ///<summary>
        ///</summary>
        public void PostMessageToWindow()
        {
			NativeMethods.PostMessage(new HandleRef(null, ((IWin32Window)this).Handle), Win32ButtonControl._listenMessage, IntPtr.Zero, IntPtr.Zero);
		}

        HwndWrapper mainWindow;

        ///<summary>
        ///</summary>
        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
            mainWindow = new HwndWrapper(0,NativeConstants.WS_CHILD,0,0,0,100,100,"ContainerWindow",(IntPtr)hwndParent, null);

            IntPtr _mainWindow = mainWindow.Handle;

            _controlHandle = NativeMethods.CreateWindowEx(0, "Button", "Push Me!", NativeConstants.WS_CHILD | NativeConstants.WS_VISIBLE,0,0,100,100, mainWindow.Handle, IntPtr.Zero, IntPtr.Zero, null);

            return new HandleRef(null,_mainWindow);
        }

        ///<summary>
        ///</summary>
        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            NativeMethods.DestroyWindow(hwnd);
        }

        ///<summary>
        ///</summary>
        IntPtr _hook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {


            handled = false;

            if (hwnd == ((IWin32Window)this).Handle)
            {
                if (HwndHostHook != null)
                    HwndHostHook(hwnd,msg, wParam,lParam,ref handled);

                if (msg == NativeConstants.WM_COMMAND && NativeMethods.IntPtrToInt32(wParam) == NativeConstants.BN_CLICKED && lParam == _controlHandle)
                {
                    if (Click != null)
                        Click(this,EventArgs.Empty);
                }
				else if (msg == Win32ButtonControl._listenMessage)
				{
                    if (Listen != null)
                        Listen(this,EventArgs.Empty);

					handled = true;
				}

            }

			return IntPtr.Zero;
		}

		///<summary>
		///</summary>
		static internal int _listenMessage = NativeMethods.RegisterWindowMessage("Win32ButtonControlType");

        ///<summary>
        ///</summary>
        public event HwndSourceHook HwndHostHook;


        ///<summary>
        ///</summary>
        public event EventHandler Click;

        ///<summary>
        ///</summary>
        public event EventHandler Listen;



        IntPtr _controlHandle = IntPtr.Zero;


    }


 }





