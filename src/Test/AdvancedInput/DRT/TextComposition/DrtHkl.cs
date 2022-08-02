// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Collections;
using System.Threading;



namespace DRT
{
    public class DrtHkl
    {

        [DllImport("User32.dll", ExactSpelling=true, CharSet=CharSet.Auto)]
        public static extern UIntPtr GetKeyboardLayout(int dwLayout);

        [DllImport("User32.dll", ExactSpelling=true, CharSet=CharSet.Auto)]
        public static extern UIntPtr ActivateKeyboardLayout(UIntPtr hkl, int uFlags);

        [DllImport("User32.dll", ExactSpelling=true, CharSet=CharSet.Auto)]
        public static extern int GetKeyboardLayoutList(int size, [Out, MarshalAs(UnmanagedType.LPArray)] UIntPtr [] hkls);

        [DllImport("User32.dll", CharSet=CharSet.Auto)]
        public static extern UIntPtr LoadKeyboardLayout(string klid, int uFlags);

        [DllImport("User32.dll", CharSet=CharSet.Auto)]
        public static extern int UnloadKeyboardLayout(UIntPtr hkl);

        public DrtHkl(UIntPtr hkl, string layout)
        {
            _hkl = hkl;
            _layout = layout;
            _hklPrev = (UIntPtr)0;
            _hklTemp = (UIntPtr)0;
            _activated = false;
        }


        public bool Activate()
        {
            int nCount = GetKeyboardLayoutList(0, null);
            UIntPtr[] hkls = new UIntPtr[nCount];
            GetKeyboardLayoutList(nCount, hkls);

            foreach (UIntPtr hkl in hkls)
            {
                if (hkl == _hkl)
                {
                    _hklPrev = ActivateKeyboardLayout(hkl, 0);
                    _activated = true;
                    return true;
                }
            }

            _hklPrev = GetKeyboardLayout(0);
            _hklTemp = LoadKeyboardLayout(_layout, 1);
            if (_hklTemp == _hkl)
            {
                _activated = true;
                return true;
            }

            return false;
        }

        public void Restore()
        {
            if (_hklPrev != (UIntPtr)0)
                ActivateKeyboardLayout(_hklPrev, 0);

            _hklPrev = (UIntPtr)0;

            if (_hklTemp != (UIntPtr)0)
                UnloadKeyboardLayout(_hklTemp);
           
            _hklTemp = (UIntPtr)0;
            _activated = false;
        }

        public bool IsActivated
        {
            get {return _activated;}
        }
 
        public UIntPtr Hkl
        {
            get {return _hkl;}
        }

        private readonly UIntPtr _hkl;
        private readonly string  _layout;
        private UIntPtr _hklPrev;
        private UIntPtr _hklTemp;
        private bool _activated;
    }
}
