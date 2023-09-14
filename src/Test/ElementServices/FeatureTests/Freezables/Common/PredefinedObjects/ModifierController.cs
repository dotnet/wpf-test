// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/***********************************************************
 *
 *   Copyright (c) Microsoft Corporation, 2003
 *
 *   Program:   Common part of all the test Modifiers
 
 *
 ************************************************************/

using System;


namespace                       Microsoft.Test.ElementServices.Freezables.Modifiers
{
    //--------------------------------------------------------------
    // Unfortunately I cannot use inheritance to factor out all the commonalities
    // between different Modifiers I use in testing IAnimatable. So I am using delegation.
    // This class to be hooked in every Modifier used for IAnimatable property & rendering
    // testing

    public class                ModifierController
    {
        //----------------------------------------------------------

        public void             SetDefaultParentTimeline ( System.Windows.Media.Animation.Timeline tml )
        {
            _timeline = tml;
        }

        //----------------------------------------------------------

        public System.Windows.Media.Animation.Timeline
                                Timeline                    { get { return _timeline; } }
        public bool             UsesBaseValue               { get { return _usesBase; }      set { _usesBase = value; } }

        //----------------------------------------------------------

        private System.Windows.Media.Animation.Timeline     _timeline;
        private bool                                        _usesBase;
    }
}
