// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Markup;
using System.ComponentModel;
using System.Threading;

namespace Microsoft.Test.Integration
{
    /// <summary>
    /// 
    /// </summary>   
    public abstract class VariationItem : TestContract
    {
        /// <summary>
        /// 
        /// </summary>
        public VariationItem()
        {
            _id = Interlocked.Increment(ref _idCounter);
        }


        /// <summary>
        /// No matter what is the ContentModel of a specific VI.  This methods
        /// returns the "Children" for the VariationItem.
        /// </summary>
        /// <returns></returns>
        abstract public IEnumerable<VariationItem> GetVIChildren();


        /// <summary>
        /// 
        /// </summary>
        internal long ID
        {
            get
            {
                return _id;
            }
        }

        

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (_tostringCache == null)
            {
                try
                {
                    _tostringCache = XamlWriter.Save(this);
                }
                catch (Exception e)
                {
                    _tostringCache = "Exception trying to get String. " + e.ToString();
                }
            }

            return _tostringCache;
        }

        ///<summary>
        ///</summary>
        protected virtual void InvalidateCache()
        {
            _tostringCache = null;
        }


        /// <summary>
        /// 
        /// </summary>
        public abstract void Execute();
        
        /// <summary>
        /// 
        /// </summary>
        public string Creator
        {
            get
            {
                return _creator;
            }

            set
            {
                _creator = value;
            }
        }

        ///<summary>
        ///</summary>
        public bool ShouldSerializeCreator()
        {
            return false;
        }

        long _id = 0;
        private string _creator;
        private string _tostringCache = null;
        static long _idCounter = 0;

    }
}
