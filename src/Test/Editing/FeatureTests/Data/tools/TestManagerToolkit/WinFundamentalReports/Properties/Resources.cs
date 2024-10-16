﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// <autogenerated>
//     This code was generated by a tool.
//     Runtime Version:2.0.40607.46
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </autogenerated>

namespace WinFundamentalReports.Properties
{
    using System;
    using System.IO;
    using System.Resources;

    /// <summary>
    ///    A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the Strongly Typed Resource Builder
    // class via a tool like ResGen or Visual Studio.NET.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    class Resources
    {

        private static System.Resources.ResourceManager s_resMgr;

        private static System.Globalization.CultureInfo s_resCulture;

        /*FamANDAssem*/
        internal Resources()
        {
        }

        /// <summary>
        ///    Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public static System.Resources.ResourceManager ResourceManager
        {
            get
            {
                if ((s_resMgr == null))
                {
                    System.Resources.ResourceManager temp = new System.Resources.ResourceManager("Resources", typeof(Resources).Assembly);
                    s_resMgr = temp;
                }
                return s_resMgr;
            }
        }

        /// <summary>
        ///    Overrides the current thread's CurrentUICulture property for all
        ///    resource lookups using this strongly typed resource class.
        /// </summary>
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public static System.Globalization.CultureInfo Culture
        {
            get
            {
                return s_resCulture;
            }
            set
            {
                s_resCulture = value;
            }
        }
    }
}