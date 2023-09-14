// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;
using Microsoft.Test.Modeling;
using Avalon.Test.CoreUI.Trusted;
using Microsoft.Test;
//using Avalon.Test.CoreUI.Trusted.Controls;
using Microsoft.Test.Threading;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI;
using System.Collections;
using System.Collections.Generic;

using Microsoft.Test.Logging;
using System.Windows.Markup;
using System.Windows.Threading;
using Avalon.Test.CoreUI.Threading;
using System.Reflection;
using System.IO;
using System.Windows;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using Microsoft.Test.Win32;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using System.Threading;
using Avalon.Test.CoreUI.Trusted.Controls.KeyboardInteropModelControls;

using SWF = System.Windows.Forms;


namespace Avalon.Test.Hosting
{
	public partial class KeyboardInteropTest
	{
		private class AppState
        {
            public AppState(WinForm f) { FormToRun = f; }
            public readonly WinForm FormToRun;

            public void RunApp()
            {
                SWF.Application.Run(FormToRun);               
            }
        }

        private WinForm LaunchForm()
        {
            _form = new WinForm();

            try
            {
                Thread t = new Thread(new ThreadStart(new AppState(_form).RunApp));
                t.Priority = ThreadPriority.Highest;
                t.SetApartmentState(ApartmentState.STA);
                t.IsBackground = true;
                t.Start();
            }
            catch (Exception e)
            {
                CoreLogger.LogStatus(e.Message);
            }

            return _form;
        }


        private void InjectCommonAccelerator()
        {
            KeyboardHelper.TypeKey(Key.A, ModifierKeys.Control);
        }

        private void InjectPFirstAccelerator()
        {
            KeyboardHelper.TypeKey(Key.Q, ModifierKeys.Control);
        }

        private void InjectGlobalAccelerator()
        {
            KeyboardHelper.TypeKey(Key.M, ModifierKeys.Control);
        }

        private void InjectChildAccelerator()
        {
            KeyboardHelper.TypeKey(Key.I, ModifierKeys.Control);
        }

        private void InjectParentAccelerator()
        {
            KeyboardHelper.TypeKey(Key.J, ModifierKeys.Control);
        }

        private void InjectUniqueAccelerator()
        {
            // AvalonWindow
            KeyboardHelper.TypeKey(Key.Z, ModifierKeys.Control);
            // HostedWnd
            KeyboardHelper.TypeKey(Key.Y, ModifierKeys.Control);
            // SourcedAvalon 
            KeyboardHelper.TypeKey(Key.X, ModifierKeys.Control);
            // Win32Window
            KeyboardHelper.TypeKey(Key.W, ModifierKeys.Control);
            // WinForm
            KeyboardHelper.TypeKey(Key.L, ModifierKeys.Control);

        }


        void injectMnemonics(bool bCommon)
        {
            if (bCommon)
            {
                KeyboardHelper.TypeKey(Key.A, ModifierKeys.Alt);
            }
            else
            {
                foreach (Key k in _testMnemonics)
                {
                    KeyboardHelper.TypeKey(k, ModifierKeys.Alt);
                }
            }
        }

        private bool _cmMneInstalled = false;
        private bool _uniMneInstalled = false;

        private void AddMnemonicsToControls(string mneType)
        {

            string[] mneLetter = { "z", "y", "x", "w", "v", "u", "t", null };
            Key[] keys = { Key.Z, Key.Y, Key.X, Key.W, Key.V, Key.U, Key.T };

            if (mneType == "common")
            {
                if (_cmMneInstalled == true)
                    return;
                else
                    _cmMneInstalled = true;
                foreach (IKeyboardInteropTestControl testControl in _testControls)
                {
                    testControl.AddMnemonic("a", ModifierKeys.Alt);
                }
            }
            else
            {
                if (_uniMneInstalled == true)
                    return;
                else
                    _uniMneInstalled = true;

                if (_testMnemonics == null)
                    _testMnemonics = new List<Key>();
                int i = 0;
                foreach (IKeyboardInteropTestControl testControl in _testControls)
                {
                    testControl.AddMnemonic(mneLetter[i], ModifierKeys.Alt);
                    _testMnemonics.Add(keys[i]);
                    i++;
                }
            }
        }
		
	}

}
