// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/***********************************************************
 *
 *   Copyright (c) Microsoft Corporation, 2004
 *
 *   Program:   MyFreezable
 
 *
 ************************************************************/
using System;

//------------------------------------------------------------------
namespace Microsoft.Test.ElementServices.Freezables.Objects
{
    //--------------------------------------------------------------
    /*
    //This object is created to provide more granular testings as failures are clearly realted to Freezable 
    //logic, and not influenced by misuse of the framework or core code
    */
    public class MyFreezable : System.Windows.Freezable
    {   
        //--------------------------------
        public MyFreezable()
        {
        }

        //--------------------------------
        public MyFreezable(System.Windows.Freezable ChObj)
        {
            FreezableObj = ChObj;
        }

        //--------------------------------
        public System.Windows.Freezable FreezableObj
        {
            get
            {
                ReadPreamble();
                return _changeableObj;
            }
            set
            {
                if (_changeableObj != value)
                {
                    WritePreamble();
                    OnFreezablePropertyChanged(_changeableObj, value);
                    _changeableObj = value;
                    WritePostscript();
                }   
            }
        }
 
        //--------------------------------
        protected override bool FreezeCore(bool IsChecking)
        {
            return System.Windows.Freezable.Freeze(_changeableObj, IsChecking);
        }

        //--------------------------------
        protected override void CloneCore(System.Windows.Freezable sourceFreezable)
        {
            MyFreezable myFreezable = (MyFreezable)sourceFreezable;
            base.CloneCore(sourceFreezable);
            _changeableObj = myFreezable._changeableObj;

        }
        protected override void GetAsFrozenCore(System.Windows.Freezable sourceFreezable)
        {
            MyFreezable myFreezable = (MyFreezable)sourceFreezable;
            base.GetAsFrozenCore(sourceFreezable);
            _changeableObj = myFreezable._changeableObj;

        }
        protected override void GetCurrentValueAsFrozenCore(System.Windows.Freezable sourceFreezable)
        {
            MyFreezable myFreezable = (MyFreezable)sourceFreezable;
            base.GetCurrentValueAsFrozenCore(sourceFreezable);
            _changeableObj = myFreezable._changeableObj;

        }
        public new MyFreezable GetAsFrozen()
        {
            return (MyFreezable)base.GetAsFrozen();
        }
        protected override System.Windows.Freezable CreateInstanceCore()
        {
            return new MyFreezable();
        }
        
        //--------------------------------
        private System.Windows.Freezable _changeableObj;

    }
}
