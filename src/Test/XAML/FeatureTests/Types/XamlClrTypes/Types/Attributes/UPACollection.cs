// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Markup;
using System.Xaml;
using Microsoft.Test.Xaml.Types;

namespace Microsoft.Test.Xaml.Types.Attributes
{
    /// <summary>
    /// UPACollectionRO Test class
    /// </summary>
    [UidProperty("ObjectCollection")]
    public class UPACollectionRO : CustomUDIObject
    {
        /// <summary>
        /// Object Collection
        /// </summary>
        private readonly Collection<object> _objCollection;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPACollectionRO"/> class.
        /// </summary>
        public UPACollectionRO()
        {
            _objCollection = new Collection<object>();
        }

        /// <summary>
        /// Gets the object collection.
        /// </summary>
        /// <value>The object collection.</value>
        public Collection<object> ObjectCollection
        {
            get
            {
                return _objCollection;
            }
        }
    }

    /// <summary>
    /// UPACollectionRW Test class
    /// </summary>
    [UidProperty("ObjectCollection")]
    public class UPACollectionRW : Custom_Clr_StringID
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPACollectionRW"/> class.
        /// </summary>
        public UPACollectionRW()
        {
            ObjectCollection = new Collection<object>();
        }

        /// <summary>
        /// Gets or sets the object collection.
        /// </summary>
        /// <value>The object collection.</value>
        public Collection<object> ObjectCollection { get; set; }
    }
}
