// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.IO;
using System.Xml;
using System.Threading;
using System.Windows.Threading;
using System.Collections;
using System.Windows.Interop;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Media;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using Microsoft.Test.Win32;
using Microsoft.Win32;
//using Microsoft.Test.Avalon.Input;
using Microsoft.Test.Threading;

namespace Avalon.Test.CoreUI.Trusted.Controls.KeyboardInteropModelControls
{
    /// <summary>
    /// 
    /// </summary>
    public interface IKeyboardInteropTestControl
    {
        /// <summary>
        /// 
        /// </summary>
        bool NeedRecord { set; }

        /// <summary>
        /// 
        /// </summary>
        AcceleratorTestState RecordedAcceleratorTestState { get; }

        /// <summary>
        /// 
        /// </summary>
        ExpectedAccelTestState ExpectedAcceleratorTestState { set; }

        /// <summary>
        /// 
        /// </summary>
        TabTestState ExpectedTabTestState { get; set;}

        /// <summary>
        /// 
        /// </summary>
        TabTestState RecordedTabTestState { get; }

        /// <summary>
        /// 
        /// </summary>
        void AddMnemonic(string letter, ModifierKeys modifier);

        /// <summary>
        /// 
        /// </summary>
        int RecordedMnemonics { get; } 

        /// <summary>
        /// 
        /// </summary>
        IKeyboardInteropTestControl AddChild(string childType);

        /// <summary>
        /// 
        /// </summary>
        void SetFocusToFirstChild(bool first);

        //void ChangeBackColor(Brush brush);

        /// <summary>
        /// 
        /// </summary>
        void ResetTestState();

    }

    /// <summary>
    /// 
    /// </summary>
    public enum AccelTestType 
    { 
        /// <summary/>
        Common,
        /// <summary/>
        Unique,
        /// <summary/>
        Global,
        /// <summary/>
        POverride,
        /// <summary/>
        COverride,
        /// <summary/>
        PFirst,
        /// <summary/>
        Default
    };

    /// <summary>
    /// 
    /// </summary>
    public enum TabTestType 
    {
        /// <summary/>
        Default,
        /// <summary/>
        Normal,
        /// <summary/>
        Loop 
    };

    /// <summary>
    /// 
    /// </summary>
    public struct AcceleratorTestState
    {
        /// <summary>
        /// key combinations
        /// </summary>
        public bool ControlState;                   
        /// <summary>
        /// 
        /// </summary>
        public bool KeyState;                       

        /// <summary>
        /// associated command
        /// </summary>
        public RoutedCommand RecordedCommand;       

        /// <summary>
        /// total number of being called
        /// </summary>
        public int SumCommand;                      
        
        /// <summary>
        /// for accelerators affecting child
        /// </summary>
        public Brush BackColor;                     
        
        /// <summary>
        /// for override sce. 
        /// </summary>
        public string Owner;                        

        /// <summary>
        /// 
        /// </summary>
        public void ResetTestState()
        {
            SumCommand = 0;
            ControlState = false;
            KeyState = false;
            RecordedCommand = null;
            Owner = null;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public struct ExpectedAccelTestState
    {
        /// <summary>
        /// 
        /// </summary>
        public Brush BackColor;

        /// <summary>
        /// 
        /// </summary>
        public AccelTestType TestType;

        /// <summary>
        /// 
        /// </summary>
        public void ResetTestState()
        {
            BackColor = Brushes.White;
            TestType = AccelTestType.Default;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public struct TabTestState
    {
        /// <summary>
        /// 
        /// </summary>
        public int RecordedTabStops;

        /// <summary>
        /// 
        /// </summary>
        public IntPtr RecordedFirstTab;

        /// <summary>
        /// 
        /// </summary>
        public IntPtr RecordedLastTab;

        /// <summary>
        /// 
        /// </summary>
        public UIElement RecordedFirstTabElement;

        /// <summary>
        /// 
        /// </summary>
        public UIElement RecordedLastTabElement;

        /// <summary>
        /// 
        /// </summary>
        public TabTestType TestType;

        /// <summary>
        /// 
        /// </summary>
        public void ResetTestState()
        {
            RecordedTabStops = 0;
            RecordedFirstTab = IntPtr.Zero;
            RecordedLastTab = IntPtr.Zero;
            RecordedFirstTabElement = null;
            RecordedLastTabElement = null;
        }
    }

    /// <summary>
    /// Site given to child components of an Hwnd.
    /// </summary>
    public class Win32KeyboardInputSite : IKeyboardInputSite
    {
        private IntPtr _nextHwnd, _prevHwnd;
        private IKeyboardInputSink _sink;
        private UIElement _sinkElement;
        private IKeyboardInteropTestControl _parent;

        /// <summary>
        /// 
        /// </summary>
        public int tabCount = 0;

        /// <summary>
        /// 
        /// </summary>
        public Win32KeyboardInputSite(IntPtr prevHwnd, IKeyboardInputSink sink, IntPtr nextHwnd, IKeyboardInteropTestControl parent)
        {
            _nextHwnd = nextHwnd;
            _prevHwnd = prevHwnd;

            _sink = sink;
            _sink.KeyboardInputSite = this;

            _sinkElement = sink as UIElement;

            // we have to work around the issue that logic focus does not cross boundary.
            _parent = parent;
        }

        private IntPtr _firstHwnd, _lastHwnd;

        /// <summary>
        /// 
        /// </summary>
        public IntPtr FirstHwnd
        {
            set
            {
                _firstHwnd = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IntPtr LastHwnd
        {
            set
            {
                _lastHwnd = value;
            }
        }

        private TabTestType _tabType = TabTestType.Default; 

        /// <summary>
        /// 
        /// </summary>
        public TabTestType TabType
        {
            set
            {
                _tabType = value;
            }
        }

        #region IKeyboardInputSite

        /// <summary>
        /// Called by the child sink when it wants do go away.
        /// </summary>
        void IKeyboardInputSite.Unregister()
        {
            _sink = null;
            _sinkElement = null;
        }

        /// <summary>
        /// Returns child sink associated with this site.
        /// </summary>
        IKeyboardInputSink IKeyboardInputSite.Sink
        {
            get
            {
                return _sink;
            }
        }

        bool IKeyboardInputSite.OnNoMoreTabStops(TraversalRequest request)
        {
            CoreLogger.LogStatus("Win32KeyboardInputSite.OnNoMoreTabStops: direction: "+request.FocusNavigationDirection.ToString());
            IntPtr ptr = IntPtr.Zero;
            if (_tabType == TabTestType.Loop)
            {
                CoreLogger.LogStatus("  returned false");
                return false;
            }

            if ((request.FocusNavigationDirection == FocusNavigationDirection.Previous) || (request.FocusNavigationDirection == FocusNavigationDirection.Left))
            {
                // Set focus on the next win32 element.
                CoreLogger.LogStatus("Win32KeyboardInputSite.OnNoMoreTabStops: Previous || Left");
                ptr = NativeMethods.SetFocus(new HandleRef(null, _prevHwnd));
                Win32Window win32p = _parent as Win32Window;
                if (win32p != null)
                {
                    win32p.SetTabTrace(_prevHwnd);
                    win32p.ResetRecordToken();
                }
            }
            else if ((request.FocusNavigationDirection == FocusNavigationDirection.Next) || (request.FocusNavigationDirection == FocusNavigationDirection.Right))
            {
                // Set focus on the next win32 element.
                CoreLogger.LogStatus("Win32KeyboardInputSite.OnNoMoreTabStops: Next || right");
                ptr = NativeMethods.SetFocus(new HandleRef(null, _nextHwnd));
                Win32Window win32p = _parent as Win32Window;
                if (win32p != null)
                {
                    win32p.SetTabTrace(_nextHwnd);
                    win32p.ResetRecordToken();
                }
            }

            if (ptr != IntPtr.Zero)
            {
                CoreLogger.LogStatus(" returned true");
                return true;    // Non-null return value means focus was successfully set.
            }

            CoreLogger.LogStatus("  returned false");
            return false;
        }

        #endregion
    }

}
