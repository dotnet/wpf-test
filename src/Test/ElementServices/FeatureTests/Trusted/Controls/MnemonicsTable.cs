// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************************
*    Mnemonic and Access key helpers for keyboard interop testing.
*    
 
  
*    Revision:         $Revision: $
 
*
******************************************************************************/


using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Interop;

//using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
//using Avalon.Test.Hosting.Controls;

using Microsoft.Test.Win32;
using Microsoft.Test.Logging;
using Microsoft.Test.Threading;

using System.Windows.Input;
using System.Windows.Automation;
using System.Windows.Media.Animation;
using System.ComponentModel;

namespace Avalon.Test.CoreUI.Trusted.Controls
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="o"></param>
    /// <param name="args"></param>
    public delegate void MnemonicsEventHandler(object o, MnemonicsEventArgs args);

    /// <summary>
    /// 
    /// </summary>
    public class MnemonicsEventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="letter"></param>
        public MnemonicsEventArgs(int letter)
        {
            Letter = letter;
        }

        /// <summary>
        /// 
        /// </summary>
        public int Letter;
    }

    /// <summary>
    /// 
    /// </summary>
    public class MnemonicsTable
    {
        /// <summary>
        /// 
        /// </summary>
        public MnemonicsTable()
        {
            _mnemonicsTable = new Hashtable();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="letter"></param>
        /// <param name="modifiers"></param>
        /// <param name="method"></param>
        public void Add(int letter, ModifierKeys modifiers, MnemonicsEventHandler method)
        {
            if (_mnemonicsTable[modifiers] == null)
            {
                Hashtable table = new Hashtable();
                table.Add(letter, method);

                _mnemonicsTable[modifiers] = table;
            }
            else
            {
                Hashtable table = _mnemonicsTable[modifiers] as Hashtable;
                table.Add(letter, method);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="letter"></param>
        /// <param name="modifiers"></param>
        /// <param name="host"></param>
        /// <returns></returns>
        public bool Execute(int letter, ModifierKeys modifiers, HwndHost host)
        {
            if (_mnemonicsTable[modifiers] != null)
            {
                Hashtable table = _mnemonicsTable[modifiers] as Hashtable;
                MnemonicsEventHandler method = table[letter] as MnemonicsEventHandler;

                if (method != null)
                {
                    method(host, new MnemonicsEventArgs(letter));

                    return true;
                }
                
            }
            return false;
        }

        Hashtable _mnemonicsTable = null;
    }


    /// <summary>
    /// 
    /// </summary>
    public static class AccessKeyHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="modifiers"></param>
        /// <returns></returns>
        static public string ConvertKeyToStringForAccessKeyManager(Key key, ModifierKeys modifiers)
        {
            string text = null;

            switch (key)
            {
                case Key.Enter:
                    if (modifiers == ModifierKeys.None)
                        text = "\x000D";
                    break;

                case Key.Escape:
                    if (modifiers == ModifierKeys.None)
                        text = "\x001B";
                    break;

                default:
                    if (modifiers == ModifierKeys.Alt)
                        text = TextFromAccessKey(Keyboard.PrimaryDevice, key, modifiers);
                    break;
            }
            return text;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyboardDevice"></param>
        /// <param name="key"></param>
        /// <param name="modifiers"></param>
        /// <returns></returns>
        private static string TextFromAccessKey(KeyboardDevice keyboardDevice, Key key, ModifierKeys modifiers)
        {
            // Modifier key combinations -- but shift or AltGr (Ctrl+Alt) is ok.
            //    - Shift
            //    - AltGr (= LControl + LAlt)
            //    - RControl(Oem8) + Shift  --- No Modifier for Oem8.
            // ModifierKeys modifiers = keyboardDevice.Modifiers;
            if (((modifiers & ~ModifierKeys.Shift) == 0) ||
                ((modifiers & (ModifierKeys.Alt | ModifierKeys.Control)) == (ModifierKeys.Alt | ModifierKeys.Control)))
            {
                return null;
            }

            // We can not translate the key to char by calling ToUnicodeEx() because it is in KeyEvent handling.
            int virtualKey = KeyInterop.VirtualKeyFromKey(key);
            return new string((char)virtualKey, 1);
        }
    }

}


