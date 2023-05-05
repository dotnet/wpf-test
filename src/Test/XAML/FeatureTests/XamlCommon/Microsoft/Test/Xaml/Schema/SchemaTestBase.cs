// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Schema
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using System.Xaml;
    using Microsoft.Test.Logging;
    using Microsoft.Test.Xaml.Utilities;

    /// <summary>
    /// Base class for Schema tests.
    /// </summary>
    public abstract class SchemaTestBase
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the SchemaTestBase class.
        /// </summary>
        protected SchemaTestBase()
        {
            SchemaContext = new XamlSchemaContext();
        }

        #endregion

        #region Protected properties

        /// <summary>
        /// Gets or sets XamlSchemaContext for use by all derived classes.
        /// </summary>
        protected XamlSchemaContext SchemaContext { get; set; }

        /// <summary>
        /// Gets or sets StringResourceIdName.
        /// If set, makes the test negative. 
        /// An exception is expected and the exception message is matched with the resource pointed by StringResourceIdName.
        /// </summary>
        protected string StringResourceIdName { get; set; }

        #endregion

        #region Public members

        /// <summary>
        /// Mandatory method to be overridden, to implement the core functionality.
        /// </summary>
        public abstract void Run();

        #endregion
    }
}
