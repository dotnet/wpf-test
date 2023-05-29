// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System.Xml;
using System.Threading; 
using System.Windows.Threading;
using System.Windows.Interop;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Trusted;

using Microsoft.Test.Win32;
using Microsoft.Win32;
//using Microsoft.Test.Avalon.Input;

namespace Avalon.Test.CoreUI.Trusted.Controls
{
    /// <summary>
    /// Control that Host Avalon Again
    /// </summary>
    public class AvalonHwndControl : HwndHost , IKeyboardInputSink
    {
        /// <summary>
        /// 
        /// </summary>
        public AvalonHwndControl()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public bool TranslateAccelerator(ref MSG msg, ModifierKeys modifiers)
        {
            return ((IKeyboardInputSink)_source).TranslateAccelerator(ref msg, modifiers);
        }

        /// <summary>
        /// 
        /// </summary>
        public bool TabInto(TraversalRequest request)
        {
            return ((IKeyboardInputSink)_source).TabInto(request);
        }

        /// <summary>
        /// 
        /// </summary>
        public Visual RootVisual
        {
            get
            {
                if (_source != null)
                    return _source.RootVisual;

                return _rootVisualCache;
            }
            set
            {
                if (_source != null)
                    _source.RootVisual = value;

                _rootVisualCache = value;
                
            }
        }



        /// <summary>
        /// 
        /// </summary>
        override protected HandleRef BuildWindowCore(HandleRef parent)
        {
            _source = SourceHelper.CreateHwndSource( 300, 300, 0, 0, parent.Handle);
            
            //Resgister the Child
            ((IKeyboardInputSink)_source).KeyboardInputSite = ((IKeyboardInputSink)this).KeyboardInputSite;
            _source.RootVisual = _rootVisualCache;

            return new HandleRef(null, _source.Handle);
        }

        ///<summary>
        ///</summary>
        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            NativeMethods.DestroyWindow(hwnd);
        }


        HwndSource _source;
        Visual _rootVisualCache = null;
    }


    /// <summary>
    /// Control that Host Avalon Again
    /// </summary>
    public class NoTabAllowControl : HwndHost , IKeyboardInputSink
    {
        /// <summary>
        /// 
        /// </summary>
        public NoTabAllowControl()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public bool TranslateAccelerator(ref MSG msg, ModifierKeys modifiers)
        {
            return ((IKeyboardInputSink)_source).TranslateAccelerator(ref msg, modifiers);
        }

        /// <summary>
        /// 
        /// </summary>
        public bool TabInto(TraversalRequest request)
        {
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        public Visual RootVisual
        {
            get
            {
                if (_source != null)
                    return _source.RootVisual;

                return _rootVisualCache;
            }
            set
            {
                if (_source != null)
                    _source.RootVisual = value;

                _rootVisualCache = value;
                
            }
        }



        /// <summary>
        /// 
        /// </summary>
        override protected HandleRef BuildWindowCore(HandleRef parent)
        {
            _source = SourceHelper.CreateHwndSource( 300, 300, 0,0, parent.Handle);
            
            //Resgister the Child
            ((IKeyboardInputSink)_source).KeyboardInputSite = ((IKeyboardInputSink)this).KeyboardInputSite;
            _source.RootVisual = _rootVisualCache;

            return new HandleRef(null, _source.Handle);
        }

        ///<summary>
        ///</summary>
        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            NativeMethods.DestroyWindow(hwnd);
        }

        HwndSource _source;
        Visual _rootVisualCache = null;
    }



    /// <summary>
    /// 
    /// </summary>
    public class MnemonicEventArgs : EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public MnemonicEventArgs(Key key)
        {
            _key = key;
        }

        /// <summary>
        /// 
        /// </summary>
        public Key TextKey
        {
            get
            {
                return _key;
            }
        }

        Key _key;

    }




    /// <summary>
    /// 
    /// </summary>
    public class AcceleratorEventArgs : EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public AcceleratorEventArgs(Key key)
        {
            _key = key;
        }

        /// <summary>
        /// 
        /// </summary>
        public Key TextKey
        {
            get
            {
                return _key;
            }
        }

        Key _key;

    }





    /// <summary>
    /// 
    /// </summary>
    public delegate void MnemonicEventHandler(object o, MnemonicEventArgs args);

    /// <summary>
    /// 
    /// </summary>
    public delegate void AcceleratorEventHandler(object o, AcceleratorEventArgs args);

    /// <summary>
    /// 
    /// </summary>
    public struct AccelTest
    {
        /// <summary>
        /// 
        /// </summary>
        public AccelTest(byte fVirt, short key)
        {
            this.fVirt = fVirt;
            this.key = key;
        }

        /// <summary>
        /// 
        /// </summary>
        public byte fVirt;

        /// <summary>
        /// 
        /// </summary>
        public short key;
    }


    /// <summary>
    /// Control that Host Avalon Again
    /// </summary>
    public class SingleHwndControl : HwndHost, IKeyboardInputSink
    {
        /// <summary>
        /// 
        /// </summary>
        public SingleHwndControl()
        {
			this.MessageHook += new HwndSourceHook(HelperHwndSourceHook);
			_hwndWrapperHook = new HwndWrapperHook(Helper);

            _hwndWrapper = new HwndWrapper(0, NativeConstants.WS_CHILD, 0, 0, 0, 150, 80, "Main Window Test", NativeConstants.HWND_MESSAGE ,null);


            _b1 = NativeMethods.CreateWindowEx(0,"Button","Push One",NativeConstants.WS_CHILD ,10,0,60,60,_hwndWrapper.Handle, IntPtr.Zero,IntPtr.Zero, null);

            _b1Sub = new HwndSubclass(_hwndWrapperHook);
            _b1Sub.Attach(_b1);

            Mnemonic += mNemonicsHandler;

	   }


        /// <summary>
        /// 
        /// </summary>
        public void RegisterMnemonics(MnemonicsTable table)
        {
            if (table == null)
            {
                throw new ArgumentNullException("table");
            }
            
            _tbMnemonic = table;
            
         }



        /// <summary>
        /// 
        /// </summary>
        public void RegisterAccelerators(List<AccelTest> list)
        {
            if (list == null || list.Count <=0 )
            {
                throw new ArgumentException("The list parameter is invalid");
            }

            
            NativeStructs.ACCEL[] arrayAccelerator = register(list, 0x00FF);
            _tbAccelerators = new AccelaratorTable(arrayAccelerator);
            _tbAccelerators.Create();
            
        }


        
        NativeStructs.ACCEL[] register(List<AccelTest> list, short val)
        {

            NativeStructs.ACCEL[] arrayMnemonic = new NativeStructs.ACCEL[list.Count];            
            NativeStructs.ACCEL accel;

            for (int i=0; i < list.Count; i++)
            {
                accel = new NativeStructs.ACCEL();
                accel.fVirt = list[i].fVirt;
                accel.key = (short)(list[i].key & 0x00FF);
                accel.cmd = val;
                arrayMnemonic[i] = accel;
            }

            return arrayMnemonic;
        }

        private void mNemonicsHandler(object o, MnemonicEventArgs args)
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        public event MnemonicEventHandler Mnemonic;

        /// <summary>
        /// 
        /// </summary>
        public event AcceleratorEventHandler Accelerator;
        
        /// <summary>
        /// 
        /// </summary>
        public bool TranslateAccelerator(ref MSG msg, ModifierKeys modifiers)
        {
            if (_tbAccelerators != null)
            {
             	int virtualKey = NativeMethods.IntPtrToInt32(msg.wParam);
				_cache = KeyInterop.KeyFromVirtualKey(virtualKey);
    			if (0 != NativeMethods.TranslateAccelerator(new HandleRef(null, _hwndWrapper.Handle), new HandleRef(null, _tbAccelerators.ACCELTable), ref msg))
    			{
    				return true;
    			}
            }

            return false;
        
        }


        Key _cache;
        
        /// <summary>
        ///     This method is called whenever one of the component's
        ///     mnemonics is invoked.  The message must either be WM_KEYDOWN
        ///     or WM_SYSKEYDOWN.  It's illegal to modify the MSG structrure,
        ///     it's passed by reference only as a performance optimization.
        ///     If this component contains child components, the container
        ///     OnMnemonic will need to call the child's OnMnemonic method.
        /// </summary>
        public bool OnMnemonic(ref MSG msg, ModifierKeys modifiers)
        {
            if (_tbMnemonic != null)
            {

                
    			int virtualKey = NativeMethods.IntPtrToInt32(msg.wParam);
    			_cache = KeyInterop.KeyFromVirtualKey(virtualKey);
                
                if (_tbMnemonic.Execute(virtualKey,modifiers,this))
    			{
    				return true;
    			}
            }

            return false;
        }


        /// <summary>
        /// 
        /// </summary>
        public bool TabInto(TraversalRequest request)
        {


			IntPtr ptr = IntPtr.Zero;
            if (request.FocusNavigationDirection == FocusNavigationDirection.First || ( request.FocusNavigationDirection== FocusNavigationDirection.Right))
            {
                ptr = NativeMethods.SetFocus(new HandleRef(null,_b1));
            }
            else if (request.FocusNavigationDirection == FocusNavigationDirection.Last || (request.FocusNavigationDirection == FocusNavigationDirection.Left))
            {
				ptr = NativeMethods.SetFocus(new HandleRef(null, _b1));

			}
            else
            {
				throw new Exception("The request was not expected with the current values.  Mode: " + request.FocusNavigationDirection.ToString());
			}

			if (ptr != IntPtr.Zero)
            {
				return true;
			}

			return false;         

        }



        /// <summary>
        /// 
        /// </summary>
        public IKeyboardInputSite RegisterKeyboardInputSink(IKeyboardInputSink sink)
        {
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                if (_b1Sub != null)
                    _b1Sub.Detach(true);

            }

            base.Dispose(disposing);

        }



        /// <summary>
        /// 
        /// </summary>
        override protected HandleRef BuildWindowCore(HandleRef parent)
        {


            NativeMethods.SetParent(new HandleRef(this,_hwndWrapper.Handle), parent);

            NativeMethods.ShowWindow(new HandleRef(this,_hwndWrapper.Handle),NativeConstants.SW_SHOW);
                NativeMethods.ShowWindow(new HandleRef(this,_b1),NativeConstants.SW_SHOW);

			return new HandleRef(null, _hwndWrapper.Handle);
        }

        ///<summary>
        ///</summary>
        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            NativeMethods.DestroyWindow(hwnd);
        }


		IntPtr HelperHwndSourceHook(IntPtr window, int message, IntPtr firstParam,
			IntPtr secondParam, ref bool handled)
		{
			Helper(window, message, firstParam, secondParam, ref handled);
			return IntPtr.Zero;
		}

		IntPtr Helper(IntPtr window, int message, IntPtr firstParam,
            IntPtr secondParam, ref bool handled)
        {

			if (message == NativeConstants.WM_COMMAND || message == NativeConstants.WM_SYSCOMMAND)
			{

                // This is for to diffiiciente on the Accelerator and Press

                int v = (NativeMethods.IntPtrToInt32(firstParam) & 0x0000FFFF);
                
				if ( v == 0x00FF)
				{
                    if (Accelerator != null)
                        Accelerator(this, new AcceleratorEventArgs(_cache));
                
				}				
			}


			if (message == NativeConstants.WM_KEYDOWN
                && ( firstParam == new IntPtr(NativeConstants.VK_TAB) ||
				firstParam == new IntPtr(NativeConstants.VK_LEFT) || 
				firstParam == new IntPtr(NativeConstants.VK_RIGHT) ))
            {


               short b =  NativeMethods.GetKeyState(NativeConstants.VK_SHIFT);

               IntPtr _focus = NativeMethods.GetFocus();
			   if (_focus == _b1)
			   {
				   if (b >= 0 && firstParam == new IntPtr(NativeConstants.VK_TAB))   // The high-order bit is 0, SHIFT is up
				   {
                        foo( new TraversalRequest(FocusNavigationDirection.Next));
    			   }
				   else if (b < 0 && firstParam == new IntPtr(NativeConstants.VK_TAB))     // The high-order bit is 0, SHIFT is down
				   {
                        foo(new TraversalRequest(FocusNavigationDirection.Previous));
                   }
                   else if (firstParam == new IntPtr(NativeConstants.VK_LEFT))
                   {
                        foo(new TraversalRequest(FocusNavigationDirection.Left));
                   }
                   else if (firstParam == new IntPtr(NativeConstants.VK_RIGHT))
                   {
                        foo(new TraversalRequest(FocusNavigationDirection.Right));
                   }

    		   }
    		
            }
            
            handled = false;
            return IntPtr.Zero;
        }

		object foo(object o)
		{
            TraversalRequest tr = (TraversalRequest)o;
			((IKeyboardInputSink)this).KeyboardInputSite.OnNoMoreTabStops(tr);
			return null;
		}

        HwndWrapperHook _hwndWrapperHook;
        HwndWrapper _hwndWrapper;
        IntPtr  _b1;
        HwndSubclass  _b1Sub;
        AccelaratorTable _tbAccelerators = null;
        MnemonicsTable _tbMnemonic = null;

    }// end HwndHostControls



    /// <summary>
    /// Control that Host Avalon Again on a different Thread
    /// </summary>
    public class SingleHwndDiffThreadControl : HwndHost, IKeyboardInputSink
    {
        /// <summary>
        /// 
        /// </summary>
        public SingleHwndDiffThreadControl()
        {
			this.MessageHook += new HwndSourceHook(HelperHwndSourceHook);
			_hwndWrapperHook = new HwndWrapperHook(Helper);

            _hwndWrapper = new HwndWrapper(0, NativeConstants.WS_CHILD, 0, 0, 0, 150, 80, "Main Window Test", NativeConstants.HWND_MESSAGE ,null);


            _b1 = NativeMethods.CreateWindowEx(0,"Button","Push One",NativeConstants.WS_CHILD ,10,0,60,60,_hwndWrapper.Handle, IntPtr.Zero,IntPtr.Zero, null);

            _b1Sub = new HwndSubclass(_hwndWrapperHook);
            _b1Sub.Attach(_b1);

	   }


        /// <summary>
        /// 
        /// </summary>
        public void RegisterMnemonics(List<AccelTest> list)
        {
            if (list == null || list.Count <=0 )
            {
                throw new ArgumentException("The list parameter is invalid");
            }

            NativeStructs.ACCEL[] arrayMnemonic = register(list, 0x00FE);
            _tbMnemonic = new AccelaratorTable(arrayMnemonic);
            _tbMnemonic.Create();
         }



        /// <summary>
        /// 
        /// </summary>
        public void RegisterAccelerators(List<AccelTest> list)
        {
            if (list == null || list.Count <=0 )
            {
                throw new ArgumentException("The list parameter is invalid");
            }

            
            NativeStructs.ACCEL[] arrayAccelerator = register(list, 0x00FF);
            _tbAccelerators = new AccelaratorTable(arrayAccelerator);
            _tbAccelerators.Create();
            
        }


        
        NativeStructs.ACCEL[] register(List<AccelTest> list, short val)
        {

            NativeStructs.ACCEL[] arrayMnemonic = new NativeStructs.ACCEL[list.Count];            
            NativeStructs.ACCEL accel;

            for (int i=0; i < list.Count; i++)
            {
                accel = new NativeStructs.ACCEL();
                accel.fVirt = list[i].fVirt;
                accel.key = (short)(list[i].key & 0x00FF);
                accel.cmd = val;
                arrayMnemonic[i] = accel;
            }

            return arrayMnemonic;
        }


        /// <summary>
        /// 
        /// </summary>
        public event MnemonicEventHandler Mnemonic;

        /// <summary>
        /// 
        /// </summary>
        public event AcceleratorEventHandler Accelerator;
        
        /// <summary>
        /// 
        /// </summary>
        public bool TranslateAccelerator(ref MSG msg, ModifierKeys modifiers)
        {
            if (_tbAccelerators != null)
            {
                int virtualKey = NativeMethods.IntPtrToInt32(msg.wParam);
                _cache = KeyInterop.KeyFromVirtualKey(virtualKey);
    			if (0 != NativeMethods.TranslateAccelerator(new HandleRef(null, _hwndWrapper.Handle), new HandleRef(null, _tbAccelerators.ACCELTable), ref msg))
    			{
    				return true;
    			}
            }

            return false;
        
        }


        Key _cache;
        
        /// <summary>
        ///     This method is called whenever one of the component's
        ///     mnemonics is invoked.  The message must either be WM_KEYDOWN
        ///     or WM_SYSKEYDOWN.  It's illegal to modify the MSG structrure,
        ///     it's passed by reference only as a performance optimization.
        ///     If this component contains child components, the container
        ///     OnMnemonic will need to call the child's OnMnemonic method.
        /// </summary>
        public bool OnMnemonic(ref MSG msg, ModifierKeys modifiers)
        {
            if (_tbMnemonic != null)
            {

    			int virtualKey = NativeMethods.IntPtrToInt32(msg.wParam);
    			_cache = KeyInterop.KeyFromVirtualKey(virtualKey);
    			if (0 != NativeMethods.TranslateAccelerator(new HandleRef(null, _hwndWrapper.Handle), new HandleRef(null, _tbMnemonic.ACCELTable), ref msg))
    			{

    				return true;
    			}
            }

            return false;
        }


        /// <summary>
        /// 
        /// </summary>
        public bool TabInto(TraversalRequest request)
        {


			IntPtr ptr = IntPtr.Zero;
            if (request.FocusNavigationDirection == FocusNavigationDirection.First || ( request.FocusNavigationDirection== FocusNavigationDirection.Right))
            {
                ptr = NativeMethods.SetFocus(new HandleRef(null,_b1));
            }
            else if (request.FocusNavigationDirection == FocusNavigationDirection.Last || (request.FocusNavigationDirection == FocusNavigationDirection.Left))
            {
				ptr = NativeMethods.SetFocus(new HandleRef(null, _b1));

			}
            else
            {
				throw new Exception("The request was not expected with the current values.  Mode: " + request.FocusNavigationDirection.ToString());
			}

			if (ptr != IntPtr.Zero)
            {
				return true;
			}

			return false;         

        }



        /// <summary>
        /// 
        /// </summary>
        public IKeyboardInputSite RegisterKeyboardInputSink(IKeyboardInputSink sink)
        {
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                if (_b1Sub != null)
                    _b1Sub.Detach(true);

            }

            base.Dispose(disposing);

        }



        /// <summary>
        /// 
        /// </summary>
        override protected HandleRef BuildWindowCore(HandleRef parent)
        {


            NativeMethods.SetParent(new HandleRef(this,_hwndWrapper.Handle), parent);

            NativeMethods.ShowWindow(new HandleRef(this,_hwndWrapper.Handle),NativeConstants.SW_SHOW);
                NativeMethods.ShowWindow(new HandleRef(this,_b1),NativeConstants.SW_SHOW);

			return new HandleRef(null, _hwndWrapper.Handle);
        }


        ///<summary>
        ///</summary>
        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            NativeMethods.DestroyWindow(hwnd);
        }

        IntPtr HelperHwndSourceHook(IntPtr window, int message, IntPtr firstParam,
            IntPtr secondParam, ref bool handled)
		{
			Helper(window, message, firstParam, secondParam, ref handled);
			return IntPtr.Zero;
		}

        IntPtr Helper(IntPtr window, int message, IntPtr firstParam,
            IntPtr secondParam, ref bool handled)
        {

			if (message == NativeConstants.WM_COMMAND || message == NativeConstants.WM_SYSCOMMAND)
			{

                // This is for to diffiiciente on the Accelerator and Press

                int v = (NativeMethods.IntPtrToInt32(firstParam) & 0x0000FFFF);
                
				if ( v == 0x00FF)
				{
                    if (Accelerator != null)
                        Accelerator(this, new AcceleratorEventArgs(_cache));
                
				}
				if ( v == 0x00FE)
				{                    
                    if (Mnemonic != null)
                        Mnemonic(this,new MnemonicEventArgs(_cache));
				}
			}


			if (message == NativeConstants.WM_KEYDOWN
                && ( firstParam == new IntPtr(NativeConstants.VK_TAB) ||
				firstParam == new IntPtr(NativeConstants.VK_LEFT) || 
				firstParam == new IntPtr(NativeConstants.VK_RIGHT) ))
            {


               short b =  NativeMethods.GetKeyState(NativeConstants.VK_SHIFT);

               IntPtr _focus = NativeMethods.GetFocus();
			   if (_focus == _b1)
			   {
				   if (b >= 0 && firstParam == new IntPtr(NativeConstants.VK_TAB))   // The high-order bit is 0, SHIFT is up
				   {
                        foo(new TraversalRequest(FocusNavigationDirection.Next));
    			   }
				   else if (b < 0 && firstParam == new IntPtr(NativeConstants.VK_TAB))     // The high-order bit is 0, SHIFT is down
				   {
                        foo(new TraversalRequest(FocusNavigationDirection.Previous));

                   }
                   else if (firstParam == new IntPtr(NativeConstants.VK_LEFT))
                   {
                        foo(new TraversalRequest(FocusNavigationDirection.Left));
                   }
                   else if (firstParam == new IntPtr(NativeConstants.VK_RIGHT))
                   {
                        foo(new TraversalRequest(FocusNavigationDirection.Right));
                   }

    		   }
    		
            }
            
            handled = false;
            return IntPtr.Zero;
        }

		object foo(object o)
		{
            TraversalRequest tr = (TraversalRequest)o;
			((IKeyboardInputSink)this).KeyboardInputSite.OnNoMoreTabStops(tr);
			return null;
		}

        HwndWrapperHook _hwndWrapperHook;
        HwndWrapper _hwndWrapper;
        IntPtr  _b1;
        HwndSubclass  _b1Sub;
        AccelaratorTable _tbAccelerators = null;
        AccelaratorTable _tbMnemonic = null;

    }// end HwndHostControls









    /// <summary>
    /// Control that Host Avalon Again
    /// </summary>
    public class MultipleHwndControl : HwndHost, IKeyboardInputSink
    {
        /// <summary>
        /// 
        /// </summary>
        public MultipleHwndControl()
        {
			this.MessageHook += new HwndSourceHook(HelperHwndSourceHook);
			_hwndWrapperHook = new HwndWrapperHook(Helper);
        }

        /// <summary>
        /// 
        /// </summary>
        public  bool TranslateAccelerator(ref MSG msg, ModifierKeys modifiers)
        {


		   return false;
            
        }


        /// <summary>
        /// 
        /// </summary>
        public  bool TabInto(TraversalRequest request)
        {


			IntPtr ptr = IntPtr.Zero;
            if (request.FocusNavigationDirection == FocusNavigationDirection.First || ( request.FocusNavigationDirection== FocusNavigationDirection.Right))
            {
                ptr = NativeMethods.SetFocus(new HandleRef(null,_b1));

            }
            else if (request.FocusNavigationDirection == FocusNavigationDirection.Last || (request.FocusNavigationDirection == FocusNavigationDirection.Left))
            {
                ptr = NativeMethods.SetFocus(new HandleRef(null,_b2));

            }
            else
            {
				throw new Exception("The request was not expected with the current values.  FocusNavigationDirection: " + request.FocusNavigationDirection.ToString());
			}

			if (ptr != IntPtr.Zero)
            {
				return true;
			}

			return false;         

        }



        /// <summary>
        /// 
        /// </summary>
        public  IKeyboardInputSite RegisterKeyboardInputSink(IKeyboardInputSink sink)
        {
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                if (_b1Sub != null)
                    _b1Sub.Detach(true);

                if (_b2Sub != null)
                    _b2Sub.Detach(true);

            }

            base.Dispose(disposing);

        }



        /// <summary>
        /// 
        /// </summary>
        override protected HandleRef BuildWindowCore(HandleRef parent)
        {

             _hwndWrapper = new HwndWrapper(0, NativeConstants.WS_VISIBLE | NativeConstants.WS_CHILD, 0, 0, 0, 150, 80, "Main Window Test", parent.Handle,null);

            _b1 = NativeMethods.CreateWindowEx(0,"Button","Push One",NativeConstants.WS_CHILD | NativeConstants.WS_VISIBLE,10,0,60,60,_hwndWrapper.Handle, IntPtr.Zero,IntPtr.Zero, null);

            _b1Sub = new HwndSubclass(_hwndWrapperHook);
            _b1Sub.Attach(_b1);
            
            _b2 = NativeMethods.CreateWindowEx(0,"Button","Push Two",NativeConstants.WS_CHILD | NativeConstants.WS_VISIBLE,75,0,60,60,_hwndWrapper.Handle, IntPtr.Zero,IntPtr.Zero, null);
            _b2Sub = new HwndSubclass(_hwndWrapperHook);
            _b2Sub.Attach(_b2);





			return new HandleRef(null, _hwndWrapper.Handle);
        }

        IntPtr HelperHwndSourceHook(IntPtr window, int message, IntPtr firstParam,
            IntPtr secondParam, ref bool handled)
		{
			Helper(window, message, firstParam, secondParam, ref handled);
			return IntPtr.Zero;
		}

        IntPtr Helper(IntPtr window, int message, IntPtr firstParam,
            IntPtr secondParam, ref bool handled)
        {

            if (message == NativeConstants.WM_KEYDOWN
                && ( firstParam == new IntPtr(NativeConstants.VK_TAB) ||
				firstParam == new IntPtr(NativeConstants.VK_LEFT) || 
				firstParam == new IntPtr(NativeConstants.VK_RIGHT) ))
            {


               short b =  NativeMethods.GetKeyState(NativeConstants.VK_SHIFT);

               IntPtr _focus = NativeMethods.GetFocus();
    		   if (_focus == _b1)
    		   {
				   if ((b >= 0 && firstParam == new IntPtr(NativeConstants.VK_TAB)) || 
					   firstParam == new IntPtr(NativeConstants.VK_RIGHT))  // The high-order bit is 0, SHIFT is up
				   {
        			   IntPtr ptr = NativeMethods.SetFocus(new HandleRef(null, _b2));

        			   if (ptr != IntPtr.Zero)
        			   {
        				   return IntPtr.Zero;
        			   }
    			   }
				   else if (b < 0 && firstParam == new IntPtr(NativeConstants.VK_TAB))     // The high-order bit is 0, SHIFT is down
				   {
                        _focus = IntPtr.Zero;
                        foo(new TraversalRequest(FocusNavigationDirection.Previous));

                   }
                   else if (firstParam == new IntPtr(NativeConstants.VK_LEFT))
                   {
                        _focus = IntPtr.Zero;
                        foo(new TraversalRequest(FocusNavigationDirection.Left));
                   }
    		   }
    		   else if (_focus == _b2)
    		   {
                    if (b >= 0 && firstParam == new IntPtr(NativeConstants.VK_TAB))    // The high-order bit is 0, SHIFT is up
                    {
                       _focus = IntPtr.Zero;
                       foo(new TraversalRequest(FocusNavigationDirection.Next));

                    }
                    else if (firstParam == new IntPtr(NativeConstants.VK_RIGHT))
                    {
                        _focus = IntPtr.Zero;
                        foo(new TraversalRequest(FocusNavigationDirection.Right));
                    }                           
				   else if ((b < 0 && firstParam == new IntPtr(NativeConstants.VK_TAB)) 
					   || firstParam == new IntPtr(NativeConstants.VK_LEFT))  // The high-order bit is 0, SHIFT is down
				   {
					   IntPtr ptr = NativeMethods.SetFocus(new HandleRef(null, _b1));

        			   if (ptr != IntPtr.Zero)
        			   {
        				   return IntPtr.Zero;
        			   }
    			   }
                

    		   }
            }
            
            handled = false;
            return IntPtr.Zero;
        }


        ///<summary>
        ///</summary>
        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            NativeMethods.DestroyWindow(hwnd);
        }

        object foo(object o)
		{
            TraversalRequest tr = (TraversalRequest)o;
			((IKeyboardInputSink)this).KeyboardInputSite.OnNoMoreTabStops(tr);
			return null;
		}

        HwndWrapperHook _hwndWrapperHook;
        HwndWrapper _hwndWrapper;
        IntPtr _b2, _b1;
        HwndSubclass _b2Sub, _b1Sub;

    }// end HwndHostControls






}
    


