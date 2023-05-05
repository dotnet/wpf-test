// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

//------------------------------------------------------------------

namespace                       Microsoft.Test.Animation.ObjectPatterns
{
    //--------------------------------------------------------------
    // Unfortunately I cannot use inheritance to factor out all the commonalities
    // between different Modifiers I use in testing IAnimatable. So I am using delegation.
    // This class to be hooked in every Modifier used for IAnimatable property & rendering
    // testing

    /// <summary>
    /// 
    /// </summary>
    public class                ModifierController
    {
        //----------------------------------------------------------

        /// <summary>
        /// 
        /// </summary>
        public void             SetDefaultParentTimeline ( System.Windows.Media.Animation.Timeline tml )
        {
            _timeline = tml;
        }

        //----------------------------------------------------------

        /// <summary>
        /// 
        /// </summary>
        public System.Windows.Media.Animation.Timeline
                                Timeline                    { get { return _timeline; } }
        /// <summary>
        /// 
        /// </summary>
        public bool             UsesBaseValue               { get { return _usesBase; }      set { _usesBase = value; } }

        //----------------------------------------------------------

        private System.Windows.Media.Animation.Timeline     _timeline;
        private bool                                        _usesBase;
    }
}
