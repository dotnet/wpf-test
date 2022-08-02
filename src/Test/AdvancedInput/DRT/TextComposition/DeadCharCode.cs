// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Input;

namespace DRT
{
    public class DeadCharCode 
    {
        public DeadCharCode( int ModifierType, int vkeyDead, int vkey, int code)
        {
            _ModifierType =  ModifierType;
            _vkeyDead =  vkeyDead;
            _vkey =  vkey;
            _code =  code;
        }

        public void DoKey(DrtBase drt)
        {
            drt.PrepareToSendInput();

             switch (_ModifierType)
             {
                 case 1:
                    drt.SendKeyboardInput(Key.RightShift, true);
                    break;

                 case 2:
                    drt.SendKeyboardInput(Key.RightAlt, true);
                    break;
             }

             drt.PressKey(KeyInterop.KeyFromVirtualKey(_vkeyDead));

             switch (_ModifierType)
             {
                 case 1:
                    drt.SendKeyboardInput(Key.RightShift, false);
                    break;

                 case 2:
                    drt.SendKeyboardInput(Key.RightAlt, false);
                    break;
             }

             drt.PressKey(KeyInterop.KeyFromVirtualKey(_vkey));
        }

        public int _ModifierType;
        public int _vkeyDead;
        public int _vkey;
        public int _code;

    }

    public class AltGreCharCode {
        public AltGreCharCode( int vkey, int code)
        {
            _vkey =  vkey;
            _code =  code;
        }

        public void DoKey(DrtBase drt)
        {
             drt.SendKeyboardInput(Key.RightAlt, true);

             drt.PressKey(KeyInterop.KeyFromVirtualKey(_vkey));

             drt.SendKeyboardInput(Key.RightAlt, false);

        }
        public int _vkey;
        public int _code;
    }

    static public class DeadCharData 
    {
    static public DeadCharCode[] code_dc_041c041c = new DeadCharCode[]  {
        new DeadCharCode(2, 0x30, 0x4F, 0x151),
        new DeadCharCode(2, 0x30, 0x55, 0x171),
        new DeadCharCode(2, 0x32, 0x43, 0x10d),
        new DeadCharCode(2, 0x32, 0x44, 0x10f),
        new DeadCharCode(2, 0x32, 0x45, 0x11b),
        new DeadCharCode(2, 0x32, 0x4C, 0x13e),
        new DeadCharCode(2, 0x32, 0x4E, 0x148),
        new DeadCharCode(2, 0x32, 0x52, 0x159),
        new DeadCharCode(2, 0x32, 0x53, 0x161),
        new DeadCharCode(2, 0x32, 0x54, 0x165),
        new DeadCharCode(2, 0x32, 0x5A, 0x17e),
        new DeadCharCode(2, 0x33, 0x41, 0xe2),
        new DeadCharCode(2, 0x33, 0x49, 0xee),
        new DeadCharCode(2, 0x33, 0x4F, 0xf4),
        new DeadCharCode(2, 0x34, 0x41, 0x103),
        new DeadCharCode(2, 0x35, 0x55, 0x16f),
        new DeadCharCode(2, 0x36, 0x41, 0x105),
        new DeadCharCode(2, 0x36, 0x45, 0x119),
        new DeadCharCode(2, 0x38, 0x5A, 0x17c),
        new DeadCharCode(2, 0x39, 0x41, 0xe1),
        new DeadCharCode(2, 0x39, 0x43, 0x107),
        new DeadCharCode(2, 0x39, 0x45, 0xe9),
        new DeadCharCode(2, 0x39, 0x49, 0xed),
        new DeadCharCode(2, 0x39, 0x4C, 0x13a),
        new DeadCharCode(2, 0x39, 0x4E, 0x144),
        new DeadCharCode(2, 0x39, 0x4F, 0xf3),
        new DeadCharCode(2, 0x39, 0x52, 0x155),
        new DeadCharCode(2, 0x39, 0x53, 0x15b),
        new DeadCharCode(2, 0x39, 0x55, 0xfa),
        new DeadCharCode(2, 0x39, 0x59, 0xfd),
        new DeadCharCode(2, 0x39, 0x5A, 0x17a),
        new DeadCharCode(2, 0xBB, 0x43, 0xe7),
        new DeadCharCode(2, 0xBB, 0x53, 0x15f),
        new DeadCharCode(2, 0xBB, 0x54, 0x163),
        new DeadCharCode(2, 0xBD, 0x41, 0xe4),
        new DeadCharCode(2, 0xBD, 0x45, 0xeb),
        new DeadCharCode(2, 0xBD, 0x4F, 0xf6),
        new DeadCharCode(2, 0xBD, 0x55, 0xfc),
    };
    static public AltGreCharCode[] code_ag_041c041c = new AltGreCharCode[]  {
        new AltGreCharCode(0x42, 0x7b),
        new AltGreCharCode(0x44, 0x110),
        new AltGreCharCode(0x46, 0x5b),
        new AltGreCharCode(0x47, 0x5d),
        new AltGreCharCode(0x4b, 0x142),
        new AltGreCharCode(0x4c, 0x141),
        new AltGreCharCode(0x4d, 0xa7),
        new AltGreCharCode(0x4e, 0x7d),
        new AltGreCharCode(0x51, 0x5c),
        new AltGreCharCode(0x53, 0x111),
        new AltGreCharCode(0x56, 0x40),
        new AltGreCharCode(0x57, 0x7c),
    };
    static public DeadCharCode[] code_dc_04070407 = new DeadCharCode[]  {
        new DeadCharCode(0, 0xDD, 0x41, 0xe1),
        new DeadCharCode(0, 0xDD, 0x45, 0xe9),
        new DeadCharCode(0, 0xDD, 0x49, 0xed),
        new DeadCharCode(0, 0xDD, 0x4F, 0xf3),
        new DeadCharCode(0, 0xDD, 0x55, 0xfa),
        new DeadCharCode(1, 0xDD, 0x41, 0xe0),
        new DeadCharCode(1, 0xDD, 0x45, 0xe8),
        new DeadCharCode(1, 0xDD, 0x49, 0xec),
        new DeadCharCode(1, 0xDD, 0x4F, 0xf2),
        new DeadCharCode(1, 0xDD, 0x55, 0xf9),
    };
    static public AltGreCharCode[] code_ag_04070407 = new AltGreCharCode[]  {
        new AltGreCharCode(0x45, 0x20ac),
        new AltGreCharCode(0x4d, 0xb5),
        new AltGreCharCode(0x51, 0x40),
    };
    static public DeadCharCode[] code_dc_040c040c = new DeadCharCode[]  {
        new DeadCharCode(0, 0xDD, 0x41, 0xe2),
        new DeadCharCode(0, 0xDD, 0x45, 0xea),
        new DeadCharCode(0, 0xDD, 0x49, 0xee),
        new DeadCharCode(0, 0xDD, 0x4F, 0xf4),
        new DeadCharCode(0, 0xDD, 0x55, 0xfb),
        new DeadCharCode(1, 0xDD, 0x41, 0xe4),
        new DeadCharCode(1, 0xDD, 0x45, 0xeb),
        new DeadCharCode(1, 0xDD, 0x49, 0xef),
        new DeadCharCode(1, 0xDD, 0x4F, 0xf6),
        new DeadCharCode(1, 0xDD, 0x55, 0xfc),
        new DeadCharCode(1, 0xDD, 0x59, 0xff),
        new DeadCharCode(2, 0x32, 0x41, 0xe3),
        new DeadCharCode(2, 0x32, 0x4E, 0xf1),
        new DeadCharCode(2, 0x32, 0x4F, 0xf5),
        new DeadCharCode(2, 0x37, 0x41, 0xe0),
        new DeadCharCode(2, 0x37, 0x45, 0xe8),
        new DeadCharCode(2, 0x37, 0x49, 0xec),
        new DeadCharCode(2, 0x37, 0x4F, 0xf2),
        new DeadCharCode(2, 0x37, 0x55, 0xf9),
    };
    static public AltGreCharCode[] code_ag_040c040c = new AltGreCharCode[]  {
        new AltGreCharCode(0x45, 0x20ac),
    };
    static public DeadCharCode[] code_dc_0f0201009 = new DeadCharCode[]  {
        new DeadCharCode(0, 0xDB, 0x41, 0xe2),
        new DeadCharCode(0, 0xDB, 0x43, 0x109),
        new DeadCharCode(0, 0xDB, 0x45, 0xea),
        new DeadCharCode(0, 0xDB, 0x47, 0x11d),
        new DeadCharCode(0, 0xDB, 0x48, 0x125),
        new DeadCharCode(0, 0xDB, 0x49, 0xee),
        new DeadCharCode(0, 0xDB, 0x4A, 0x135),
        new DeadCharCode(0, 0xDB, 0x4F, 0xf4),
        new DeadCharCode(0, 0xDB, 0x53, 0x15d),
        new DeadCharCode(0, 0xDB, 0x55, 0xfb),
        new DeadCharCode(0, 0xDB, 0x57, 0x175),
        new DeadCharCode(0, 0xDB, 0x59, 0x177),
        new DeadCharCode(1, 0xDB, 0x41, 0xe4),
        new DeadCharCode(1, 0xDB, 0x45, 0xeb),
        new DeadCharCode(1, 0xDB, 0x49, 0xef),
        new DeadCharCode(1, 0xDB, 0x4F, 0xf6),
        new DeadCharCode(1, 0xDB, 0x55, 0xfc),
        new DeadCharCode(1, 0xDB, 0x59, 0xff),
        new DeadCharCode(2, 0xDB, 0x41, 0xe0),
        new DeadCharCode(2, 0xDB, 0x45, 0xe8),
        new DeadCharCode(2, 0xDB, 0x49, 0xec),
        new DeadCharCode(2, 0xDB, 0x4F, 0xf2),
        new DeadCharCode(2, 0xDB, 0x55, 0xf9),
        new DeadCharCode(2, 0xDD, 0x41, 0xe3),
        new DeadCharCode(2, 0xDD, 0x49, 0x129),
        new DeadCharCode(2, 0xDD, 0x4E, 0xf1),
        new DeadCharCode(2, 0xDD, 0x4F, 0xf5),
        new DeadCharCode(2, 0xDD, 0x55, 0x169),
    };
    static public AltGreCharCode[] code_ag_0f0201009 = new AltGreCharCode[]  {
        new AltGreCharCode(0x45, 0x20ac),
        new AltGreCharCode(0x58, 0xbb),
        new AltGreCharCode(0x5a, 0xab),
    };
    }
}
