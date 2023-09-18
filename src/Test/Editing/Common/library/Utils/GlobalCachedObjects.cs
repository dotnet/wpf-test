// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Keyboard and mouse emulation services for test cases.

namespace Test.Uis.Utils
{
    #region Namespaces.

    using System;
    using System.Threading; using System.Windows.Threading;

    #endregion Namespaces.

    /// <summary>
    /// 
    /// </summary>
    public class GlobalCachedObjects
    {
        /// <summary>
        /// Default ctor
        /// </summary>
        private GlobalCachedObjects() { }

        /// <summary>
        /// 
        /// </summary>
        public void Init()
        {
            if (this._context == null)
            {
                this._context = Dispatcher.CurrentDispatcher;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public Dispatcher MainDispatcher
        {
            get { return this._context; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public static GlobalCachedObjects Current
        {
            get
            {
                return s_globalCachedObjects;
            }
        }

        #region Private members
       
        private Dispatcher _context = null;
        private static GlobalCachedObjects s_globalCachedObjects = new GlobalCachedObjects();
        
        #endregion
    }
}
